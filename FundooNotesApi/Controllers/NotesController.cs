using System;
using System.Collections.Generic;
using CommonLayer.Models;
using ManagerLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;

namespace FundooNotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INotesManager notesManager;
        private readonly FundooDBContext dbContext;

        public NotesController(INotesManager notesManager, FundooDBContext dbContext)
        {
            this.notesManager = notesManager;
            this.dbContext = dbContext;
        }

        [Authorize]
        [HttpPost]
        [Route("CreateNote")]
        public IActionResult CreateNote(NotesModel notesModel)
        {
            try
            {
                int UserId = int.Parse(User.FindFirst("UserID").Value);
                NotesEntity notesEntity = notesManager.CreateNote(notesModel, UserId);

                if (notesEntity != null)
                {
                    return Ok(new ResponseModel<NotesEntity> { Success = true, Message="Note Added Success!", Data=notesEntity});
                }
                else
                {
                    return BadRequest(new ResponseModel<NotesEntity> { Success = false, Message = "Note didn't Added!", Data = null });
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [Authorize]
        [HttpGet]
        [Route("FindAllNotes")]
        public IActionResult FindAllNotes()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserID");
                if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
                {
                    return Unauthorized(new { Success = false, Message = "User not authorized" });
                }

                int userId = Convert.ToInt32(userIdClaim.Value);
                List<NotesEntity> notes = notesManager.FindAllNotes(userId);

                if (notes.Count > 0)
                {
                    return Ok(new { Success = true, Message = "Notes fetched successfully!", Data = notes });
                }
                else
                {
                    return NotFound(new { Success = false, Message = "No notes found for the user!" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }
    }
}
