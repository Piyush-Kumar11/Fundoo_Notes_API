using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonLayer.Models;
using GreenPipes.Caching;
using ManagerLayer.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using RepositoryLayer.Migrations;

namespace FundooNotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorManager collaboratorManager;
        private readonly FundooDBContext dbContext;
        private readonly IBus _bus;
        private readonly ILogger<CollaboratorController> _logger;
        private readonly IDistributedCache _cache;

        public CollaboratorController(ICollaboratorManager collaboratorManager, FundooDBContext dbContext, IBus _bus, ILogger<CollaboratorController> _logger, IDistributedCache _cache)
        {
            this.collaboratorManager = collaboratorManager;
            this.dbContext = dbContext;
            this._bus = _bus;
            this._logger = _logger;
            this._cache = _cache;
        }

        [Authorize]
        [HttpPost("AddCollaborator")]
        public async Task<IActionResult> AddCollaborator(CollaboratorModel collaboratorModel)
        {
            int userId = 0;

            try
            {
                userId = Convert.ToInt32(User.FindFirst("UserID").Value);
                _logger.LogInformation("Adding collaborator for UserID: {UserId}, NoteID: {NoteId}", userId, collaboratorModel.NotesId);

                var note = dbContext.Notes.FirstOrDefault(n => n.NotesId == collaboratorModel.NotesId && n.UserID == userId);
                if (note == null)
                {
                    _logger.LogWarning("Note with ID {NoteId} does not exist for UserID: {UserId}", collaboratorModel.NotesId, userId);
                    return BadRequest(new { Success = false, Message = $"Note with ID {collaboratorModel.NotesId} does not exist!" });
                }

                // Create collaborator entity
                CollaboratorEntity newCollaborator = new CollaboratorEntity
                {
                    CollaboratorEmail = collaboratorModel.CollaboratorEmail,
                    NotesId = collaboratorModel.NotesId,
                    UserID = userId
                };

                var createdCollaborator = collaboratorManager.AddCollaborator(newCollaborator);
                _logger.LogInformation("Collaborator added successfully for UserID: {UserId}, NoteID: {NoteId}", userId, collaboratorModel.NotesId);

                // Send Email Notification
                try
                {
                    Send send = new Send();
                    send.SendMailToCollaborator(collaboratorModel.CollaboratorEmail, note.Title, note.Description, note.CreatedAt);
                    _logger.LogInformation("Email sent to collaborator: {Email}", collaboratorModel.CollaboratorEmail);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send email to collaborator: {Email}", collaboratorModel.CollaboratorEmail);
                }

                // Send RabbitMQ Message
                Uri uri = new Uri("rabbitmq://localhost/FunDooNotesEmailQueue");
                var endPoint = await _bus.GetSendEndpoint(uri);
                await endPoint.Send(collaboratorModel);
                _logger.LogInformation("RabbitMQ message sent for collaborator: {Email}", collaboratorModel.CollaboratorEmail);

                return Ok(new { Success = true, Message = "Collaborator added & mail sent successfully!", Data = createdCollaborator });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddCollaborator API for UserID: {UserId}", userId);
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("RemoveCollaborator")]
        public IActionResult RemoveCollaborator(int collaboratorId)
        {
            int userId = Convert.ToInt32(User.FindFirst("UserID").Value);
            bool isDeleted = collaboratorManager.RemoveCollaborator(collaboratorId, userId);

            if (isDeleted)
                return Ok(new { Success = true, Message = "Collaborator removed successfully!" });

            return BadRequest(new { Success = false, Message = "Collaborator not found!" });
        }

        [Authorize]
        [HttpGet("GetCollaborators")]
        public IActionResult GetCollaborators()
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("UserID").Value);
                var collaborators = collaboratorManager.GetCollaboratorsByUser(userId);

                if (collaborators == null || collaborators.Count == 0)
                {
                    return BadRequest(new { Success = false, Message = "No collaborators found for this user." });
                }

                return Ok(new { Success = true, Message = "Collaborators retrieved successfully!", Data = collaborators });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetCollaboratorsWithRedis")]
        public IActionResult GetCollaboratorsWithRedis()
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("UserID").Value);
                string cacheKey = $"UserCollaborators_{userId}";

                // Check if data exists in Redis cache
                var cachedCollaborators = _cache.GetString(cacheKey);
                if (!string.IsNullOrEmpty(cachedCollaborators))
                {
                    var collaboratorsFromCache = JsonConvert.DeserializeObject<List<CollaboratorEntity>>(cachedCollaborators);
                    return Ok(new { Success = true, Message = "Collaborators retrieved successfully from cache!", Data = collaboratorsFromCache });
                }

                // If cache miss, fetch from database
                var collaborators = collaboratorManager.GetCollaboratorsByUser(userId);

                if (collaborators == null || collaborators.Count == 0)
                {
                    return NotFound(new { Success = false, Message = "No collaborators found for this user." });
                }

                // Store data in Redis cache with a 10-minute expiration
                var serializedCollaborators = JsonConvert.SerializeObject(collaborators);
                var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                _cache.SetString(cacheKey, serializedCollaborators, options);

                return Ok(new { Success = true, Message = "Collaborators retrieved successfully from database!", Data = collaborators });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }
    }
}
