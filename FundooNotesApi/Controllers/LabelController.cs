using System;
using System.Collections.Generic;
using CommonLayer.Models;
using GreenPipes.Caching;
using ManagerLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RepositoryLayer.Entities;

namespace FundooNotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelController : ControllerBase
    {
        private readonly ILabelManager labelManager;
        private readonly IDistributedCache _cache;

        public LabelController(ILabelManager labelManager, IDistributedCache _cache)
        {
            this.labelManager = labelManager;
            this._cache = _cache;
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
        [HttpGet]
        [Route("GetLabelsWithRedis")]
        public IActionResult GetLabelsWithRedis()
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("UserID").Value);
                string cacheKey = $"UserLabels_{userId}";

                // Try fetching from Redis cache
                var cachedLabels = _cache.GetString(cacheKey);
                if (!string.IsNullOrEmpty(cachedLabels))
                {
                    var labelsFromCache = JsonConvert.DeserializeObject<List<LabelEntity>>(cachedLabels);
                    return Ok(new { Success = true, Message = "Labels fetched successfully from cache!", Data = labelsFromCache });
                }

                // If not in cache, fetch from database
                List<LabelEntity> labels = labelManager.GetLabelsByUser(userId);

                if (labels.Count > 0)
                {
                    // Store in cache with expiration of 10 minutes
                    var serializedLabels = JsonConvert.SerializeObject(labels);
                    var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                    _cache.SetString(cacheKey, serializedLabels, options);

                    return Ok(new { Success = true, Message = "Labels fetched successfully from database!", Data = labels });
                }
                else
                {
                    return NotFound(new { Success = false, Message = "No labels found!" });
                }
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
