using System;
using System.Collections.Generic;
using System.Linq;
using CommonLayer.Models;
using ManagerLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;

namespace FundooNotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorManager collaboratorManager;
        private readonly FundooDBContext dbContext;

        public CollaboratorController(ICollaboratorManager collaboratorManager, FundooDBContext dbContext)
        {
            this.collaboratorManager = collaboratorManager;
            this.dbContext = dbContext;
        }

        [Authorize]
        [HttpPost("AddCollaborator")]
        public IActionResult AddCollaborator([FromBody] CollaboratorModel collaboratorModel)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("UserID").Value);

                var noteExists = dbContext.Notes.Any(n => n.NotesId == collaboratorModel.NotesId && n.UserID == userId);

                if (!noteExists)
                {
                    return BadRequest(new { Success = false, Message = $"Note with ID {collaboratorModel.NotesId} does not exist!" });
                }

                CollaboratorEntity newCollaborator = new CollaboratorEntity
                {
                    CollaboratorEmail = collaboratorModel.CollaboratorEmail,
                    NotesId = collaboratorModel.NotesId,
                    UserID = userId
                };

                var createdCollaborator = collaboratorManager.AddCollaborator(newCollaborator);

                return Ok(new { Success = true, Message = "Collaborator added successfully!", Data = createdCollaborator });
            }
            catch (Exception ex)
            {
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
    }
}
