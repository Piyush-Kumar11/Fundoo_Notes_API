using System;
using System.Collections.Generic;
using System.Linq;
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
                    return BadRequest(new { Success = false, Message = "User not authorized" });
                }

                int userId = Convert.ToInt32(userIdClaim.Value);
                NotesEntity note = notesManager.GetNoteByIds(userId, noteId);

                if (note != null)
                {
                    return Ok(new { Success = true, Message = "Note fetched successfully!", Data = note });
                }
                else
                {
                    return BadRequest(new { Success = false, Message = "Note not found!" });
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
                return BadRequest(new { Success = false, Message = "Note not found!" });
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
                return BadRequest(new { Success = false, Message = "Note not found!" });
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
                return BadRequest(new { Success = false, Message = "Note not found!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("UpdateColor")]
        public IActionResult UpdateNoteColor(int noteId, string color)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("UserID").Value);
                bool isUpdated = notesManager.UpdateNoteColor(noteId, color, userId);

                if (isUpdated)
                    return Ok(new { Success = true, Message = "Note color updated successfully!" });

                return BadRequest(new { Success = false, Message = "Note not found or unauthorized access!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("UpdateRemainder")]
        public IActionResult UpdateNoteRemainder(int noteId, DateTime remainder)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("UserID").Value);
                bool isUpdated = notesManager.UpdateNoteRemainder(noteId, remainder, userId);

                if (isUpdated)
                    return Ok(new { Success = true, Message = "Note remainder updated successfully!" });

                return BadRequest(new { Success = false, Message = "Note not found or unauthorized access!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("UpdateImage")]
        public IActionResult UpdateNoteImage(int noteId, string imageUrl)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("UserID").Value);
                bool isUpdated = notesManager.UpdateNoteImage(noteId, imageUrl, userId);

                if (isUpdated)
                    return Ok(new { Success = true, Message = "Note image updated successfully!" });

                return BadRequest(new { Success = false, Message = "Note not found or unauthorized access!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }

        //---------------------------------------------------API Review Task--------------------------------------------------------------------------

        [Authorize]
        [HttpGet("FetchNotesByTitle&Desc")]
        public IActionResult FetchNotes(string title, string description)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserID").Value);
            var notes = dbContext.Notes.Where(n => n.UserID == userId && (n.Title.Contains(title) || n.Description.Contains(description))).ToList();

            if (notes.Count == 0)
                return BadRequest(new { Success = false, Message = "No matching notes found!" });

            return Ok(new { Success = true, Message = "Notes fetched successfully!", Data = notes });
        }

        [Authorize]
        [HttpGet("GetUserNoteCount")]
        public IActionResult GetUserNoteCount()
        {
            var userId = Convert.ToInt32(User.FindFirst("UserID").Value);
            int count = dbContext.Notes.Count(n => n.UserID == userId);

            return Ok(new { Success = true, Message = "User note count retrieved!", Data = count });
        }
    }
}
