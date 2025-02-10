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
        private readonly ILogger<UsersController> _logger;


        // Constructor to initialize the dependencies for user manager and RabbitMQ bus
        public UsersController(IUserManager manager, IBus bus, FundooDBContext dbContext, ILogger<UsersController> logger)
        {
            this.manager = manager;
            this._bus = bus;
            this.dbContext = dbContext;
            this._logger = logger;
        }

        // Endpoint for user registration
        [HttpPost]
        [Route("Reg")]
        public IActionResult Register(RegisterModel model)
        {
            try
            {
                _logger.LogInformation("User registration initiated for Email: {Email}", model.Email);

                var checkEmail = manager.MailExist(model.Email);
                if (checkEmail)
                {
                    _logger.LogWarning("Registration failed: Email already exists - {Email}", model.Email);
                    return BadRequest(new ResponseModel<UserEntity> { Success = false, Message = "Email Already Exist!" });
                }

                var result = manager.Registration(model);
                if (result != null)
                {
                    _logger.LogInformation("User registered successfully: {Email}", model.Email);
                    return Ok(new ResponseModel<UserEntity> { Success = true, Message = "Register Successful", Data = result });
                }
                else
                {
                    _logger.LogWarning("Registration failed for Email: {Email}", model.Email);
                    return BadRequest(new ResponseModel<UserEntity> { Success = false, Message = "Register Failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during registration for Email: {Email}", model.Email);
                return StatusCode(500, new { success = false, message = "Internal Server Error", error = ex.Message });
            }
        }

        // Endpoint for user login
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginModel login)
        {
            _logger.LogInformation("Login attempt for Email: {Email}", login.Email);

            var result = manager.Login(login);

            if (result != "Invalid Email or Password")
            {
                _logger.LogInformation("User logged in successfully: {Email}", login.Email);
                return Ok(new ResponseModel<string> { Success = true, Message = "Login Successful", Data = result });
            }

            _logger.LogWarning("Failed login attempt for Email: {Email}", login.Email);
            return BadRequest(new ResponseModel<string> { Success = false, Message = result });
        }

        // Endpoint to handle "Forget Password" requests
        [HttpGet("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Forget Password failed: Email cannot be empty.");
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Email cannot be empty", Data = null });
                }

                _logger.LogInformation("Forget password request received for Email: {Email}", email);
                ForgetPasswordModel forgetPasswordModel = manager.ForgetPassword(email);

                if (forgetPasswordModel == null)
                {
                    _logger.LogWarning("Forget password failed: Email not found - {Email}", email);
                    return NotFound(new ResponseModel<string> { Success = false, Message = "Email not found", Data = null });
                }

                try
                {
                    Send send = new Send();
                    send.SendMail(forgetPasswordModel.Email, forgetPasswordModel.Token);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send email for forget password: {Email}", email);
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Failed to send email", Data = emailEx.Message });
                }

                Uri uri = new Uri("rabbitmq://localhost/FunDooNotesEmailQueue");
                var endPoint = await _bus.GetSendEndpoint(uri);
                await endPoint.Send(forgetPasswordModel);

                _logger.LogInformation("Forget password email sent successfully: {Email}", email);
                return Ok(new ResponseModel<string> { Success = true, Message = "Mail sent for resetting password!", Data = forgetPasswordModel.Token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ForgetPassword API for Email: {Email}", email);
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
                string email = User.FindFirstValue("Email");
                _logger.LogInformation("Reset password request for Email: {Email}", email);

                if (resetPasswordModel.Password == resetPasswordModel.ConfirmPassword)
                {
                    if (manager.ResetPassword(email, resetPasswordModel))
                    {
                        _logger.LogInformation("Password reset successfully for Email: {Email}", email);
                        return Ok(new ResponseModel<bool> { Success = true, Message = "Password Reset Success", Data = true });
                    }
                    else
                    {
                        _logger.LogWarning("Password reset failed for Email: {Email}", email);
                        return BadRequest(new ResponseModel<bool> { Success = false, Message = "Password Reset failed", Data = false });
                    }
                }
                else
                {
                    _logger.LogWarning("Password mismatch during reset for Email: {Email}", email);
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "User reset password failed", Data = "Password Mismatch" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resetting password for Email: {Email}", User.FindFirstValue("Email"));
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