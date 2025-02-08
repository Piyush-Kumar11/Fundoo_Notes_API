using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CommonLayer.Models;
using FundooNotesApi.Helpers;
using ManagerLayer.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;

namespace FundooNotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager manager;
        private readonly IBus _bus;
        private readonly FundooDBContext dbContext;

        // Constructor to initialize the dependencies for user manager and RabbitMQ bus
        public UsersController(IUserManager manager, IBus bus, FundooDBContext dbContext)
        {
            this.manager = manager;
            this._bus = bus;
            this.dbContext = dbContext;
        }

        // Endpoint for user registration
        [HttpPost]
        [Route("Reg")]
        public IActionResult Register(RegisterModel model)
        {
            try
            {
                // Check if the email already exists
                var checkEmail = manager.MailExist(model.Email);
                if (checkEmail)
                {
                    return BadRequest(new ResponseModel<UserEntity> { Success = false, Message = "Email Already Exist!" });
                }
                else
                {
                    // Perform the registration process
                    var result = manager.Registration(model);
                    if (result != null)
                    {
                        return Ok(new ResponseModel<UserEntity> { Success = true, Message = "Register Successful", Data = result });
                    }
                    else
                    {
                        return BadRequest(new ResponseModel<UserEntity> { Success = false, Message = "Register Failed" });
                    }
                }
            }
            catch (AppException ex)
            {
                // Handle custom application-specific exceptions
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        // Endpoint for user login
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginModel login)
        {
            // Authenticate the user and fetch the result
            var result = manager.Login(login);

            if (result != "Invalid Email or Password")
            {
                return Ok(new ResponseModel<string> { Success = true, Message = "Login Successful", Data = result });
            }

            return BadRequest(new ResponseModel<string> { Success = false, Message = result });
        }

        // Endpoint to handle "Forget Password" requests
        [HttpGet("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            try
            {
                // Validate the provided email
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Email cannot be empty", Data = null });
                }

                // Generate a token for password reset
                ForgetPasswordModel forgetPasswordModel = manager.ForgetPassword(email);

                if (forgetPasswordModel == null)
                {
                    return NotFound(new ResponseModel<string> { Success = false, Message = "Email not found", Data = null });
                }

                // Attempt to send a password reset email
                Send send = new Send();
                try
                {
                    send.SendMail(forgetPasswordModel.Email, forgetPasswordModel.Token);
                }
                catch (Exception emailEx)
                {
                    // Handle email-sending failures
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Failed to send email", Data = emailEx.Message });
                }

                // Send a message to RabbitMQ
                Uri uri = new Uri("rabbitmq://localhost/FunDooNotesEmailQueue");
                var endPoint = await _bus.GetSendEndpoint(uri);
                await endPoint.Send(forgetPasswordModel);

                return Ok(new ResponseModel<string> { Success = true, Message = "Mail sent for resetting password!", Data = forgetPasswordModel.Token });
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Mail not sent", Data = ex.Message });
            }
        }

        // Endpoint to reset the user's password
        [Authorize]
        [HttpPost]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            try
            {
                // Check if password and confirm password match
                if (resetPasswordModel.Password == resetPasswordModel.ConfirmPassword)
                {
                    // Retrieve the user's email from claims
                    string email = User.FindFirstValue("Email");
                    if (manager.ResetPassword(email, resetPasswordModel))
                    {
                        return Ok(new ResponseModel<bool> { Success = true, Message = "Password Reset Success", Data = true });
                    }
                    else
                    {
                        return BadRequest(new ResponseModel<bool> { Success = false, Message = "Password Reset failed", Data = false });
                    }
                }
                else
                {
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "User reset password failed", Data = "Password Mismatch" });
                }
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors during the password reset process
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = "An error occurred while resetting the password", Data = ex.Message });
            }
        }

        //---------------------------------------------------API Review Task--------------------------------------------------------------------------

        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var users = dbContext.Users.ToList();
            return Ok(new { Success = true, Message = "Users fetched successfully!", Data = users });
        }

        [HttpGet("GetUserById")]
        public IActionResult GetUserById(int userId)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.UserID == userId);
            if (user != null)
            {
                return Ok(new { Success = true, Message = "User found!", Data = user });
            }

            return BadRequest(new { Success = false, Message = "User not found!" });
        }

        [HttpGet("GetUsersStartingWithA")]
        public IActionResult GetUsersStartingWithA()
        {
            var users = dbContext.Users.Where(u => u.FirstName.StartsWith("A")).ToList();
            return Ok(new { Success = true, Message = "Users fetched successfully!", Data = users });
        }

        [HttpGet("GetTotalUserCount")]
        public IActionResult GetTotalUserCount()
        {
            int count = dbContext.Users.Count();
            return Ok(new { Success = true, Message = "User count retrieved successfully!", Data = count });
        }

        [HttpGet("GetUsersOrderedByNameAsc")]
        public IActionResult GetUsersOrderedByNameAsc()
        {
            var users = dbContext.Users.OrderBy(u => u.FirstName).ToList();
            return Ok(new { Success = true, Message = "Users ordered by name ascending!", Data = users });
        }

        [HttpGet("GetUsersOrderedByNameDesc")]
        public IActionResult GetUsersOrderedByNameDesc()
        {
            var users = dbContext.Users.OrderByDescending(u => u.FirstName).ToList();
            return Ok(new { Success = true, Message = "Users ordered by name descending!", Data = users });
        }

        [HttpGet("GetAverageUserAge")]
        public IActionResult GetAverageUserAge()
        {
            var users = dbContext.Users.ToList();
            if (users.Count == 0)
                return BadRequest(new { Success = false, Message = "No users found!" });

            double averageAge = users.Average(u => DateTime.Today.Year - u.DOB.Year);

            return Ok(new { Success = true, Message = "Average user age calculated!", Data = averageAge });
        }

        [HttpGet("GetOldestAndYoungestUser")]
        public IActionResult GetOldestAndYoungestUser()
        {
            var users = dbContext.Users.ToList();
            if (users.Count == 0)
                return BadRequest(new { Success = false, Message = "No users found!" });

            int oldestAge = users.Max(u => DateTime.Today.Year - u.DOB.Year);
            int youngestAge = users.Min(u => DateTime.Today.Year - u.DOB.Year);

            return Ok(new{Success = true, Message = "Oldest and youngest user age retrieved!", Data = new { Oldest = oldestAge, Youngest = youngestAge } });
        }
    }

}