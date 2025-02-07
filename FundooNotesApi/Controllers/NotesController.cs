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
        [HttpPut]
        [Route("UpdateNote")]
        public IActionResult UpdateNote([FromQuery] int noteId, NotesModel updatedNote)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserID");
                if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
                {
                    return BadRequest(new { Success = false, Message = "User not authorized" });
                }

                int userId = Convert.ToInt32(userIdClaim.Value);
                NotesEntity note = notesManager.UpdateNote(noteId, updatedNote, userId);

                if (note != null)
                {
                    return Ok(new { Success = true, Message = "Note updated successfully!", Data = note });
                }
                else
                {
                    return BadRequest(new { Success = false, Message = "Note not found or not authorized!" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("DeleteNote")]
        public IActionResult DeleteNotePermanently([FromQuery] int noteId)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserID");
                if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
                {
                    return BadRequest(new { Success = false, Message = "User not authorized" });
                }

                int userId = Convert.ToInt32(userIdClaim.Value);
                bool isDeleted = notesManager.DeleteNote(noteId, userId);

                if (isDeleted)
                {
                    return Ok(new { Success = true, Message = "Note deleted successfully!" });
                }
                else
                {
                    return BadRequest(new { Success = false, Message = "Note not found or not authorized!" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
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
                    return BadRequest(new { Success = false, Message = "No notes found for the user!" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetNoteById")]
        public IActionResult GetNoteById([FromQuery] int noteId)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserID");
                if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
                {
                    return Unauthorized(new { Success = false, Message = "User not authorized" });
                }

                int userId = Convert.ToInt32(userIdClaim.Value);
                NotesEntity note = notesManager.GetNoteByIds(userId, noteId);

                if (note != null)
                {
                    return Ok(new { Success = true, Message = "Note fetched successfully!", Data = note });
                }
                else
                {
                    return NotFound(new { Success = false, Message = "Note not found!" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("TogglePin")]
        public IActionResult TogglePin([FromQuery] int noteId)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("UserID").Value);
                bool isUpdated = notesManager.TogglePinNote(userId, noteId);

                if (isUpdated)
                {
                    return Ok(new { Success = true, Message = "Note pin status toggled successfully!" });
                }
                return NotFound(new { Success = false, Message = "Note not found!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("ToggleArchive")]
        public IActionResult ToggleArchive([FromQuery] int noteId)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("UserID").Value);
                bool isUpdated = notesManager.ToggleArchiveNote(userId, noteId);

                if (isUpdated)
                {
                    return Ok(new { Success = true, Message = "Note archive status toggled successfully!" });
                }
                return NotFound(new { Success = false, Message = "Note not found!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("ToggleTrash")]
        public IActionResult ToggleTrash([FromQuery] int noteId)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("UserID").Value);
                bool isUpdated = notesManager.ToggleTrashNote(userId, noteId);

                if (isUpdated)
                {
                    return Ok(new { Success = true, Message = "Note trash status toggled successfully!" });
                }
                return NotFound(new { Success = false, Message = "Note not found!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }
    }
}
