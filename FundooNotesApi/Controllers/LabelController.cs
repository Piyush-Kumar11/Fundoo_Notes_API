using System;
using System.Collections.Generic;
using CommonLayer.Models;
using ManagerLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entities;

namespace FundooNotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelController : ControllerBase
    {
        private readonly ILabelManager labelManager;

        public LabelController(ILabelManager labelManager)
        {
            this.labelManager = labelManager;
        }

        [Authorize]
        [HttpPost]
        [Route("CreateLabel")]
        public IActionResult CreateLabel([FromBody] LabelModel labelModel)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("UserID").Value);

                LabelEntity newLabel = new LabelEntity();
                newLabel.LabelName = labelModel.LabelName;
                newLabel.NotesId = labelModel.NotesId;
                newLabel.UserID = userId;  // Assigning authenticated user's ID
                
                var createdLabel = labelManager.CreateLabel(newLabel);
                return Ok(new { Success = true, Message = "Label created successfully!", Data = createdLabel });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetLabels")]
        public IActionResult GetLabels()
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("UserID").Value);
                List<LabelEntity> labels = labelManager.GetLabelsByUser(userId);

                return Ok(new { Success = true, Message = "Labels fetched successfully!", Data = labels });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("UpdateLabel")]
        public IActionResult UpdateLabel(int labelId, string newLabelName)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("UserID").Value);
                LabelEntity updatedLabel = labelManager.UpdateLabel(labelId, newLabelName, userId);

                if (updatedLabel != null)
                {
                    return Ok(new { Success = true, Message = "Label updated successfully!", Data = updatedLabel });
                }
                return NotFound(new { Success = false, Message = "Label not found!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("DeleteLabel")]
        public IActionResult DeleteLabel(int labelId)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("UserID").Value);
                bool isDeleted = labelManager.DeleteLabel(labelId, userId);

                if (isDeleted)
                {
                    return Ok(new { Success = true, Message = "Label deleted successfully!" });
                }
                return NotFound(new { Success = false, Message = "Label not found!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }
    }
}
