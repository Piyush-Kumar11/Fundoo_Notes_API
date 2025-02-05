using System;
using System.Threading.Tasks;
using CommonLayer.Models;
using FundooNotesApi.Helpers;
using ManagerLayer.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Entities;

namespace FundooNotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager manager;
        private readonly IBus _bus;

        public UsersController(IUserManager manager, IBus bus)
        {
            this.manager = manager;
            this._bus = bus;
        }

        [HttpPost]
        [Route("Reg")]
        public IActionResult Register(RegisterModel model)
        {
            try
            {
                var checkEmail = manager.MailExist(model.Email);
                if (checkEmail)
                {
                    return BadRequest(new ResponseModel<UserEntity> { Success = false, Message = "Email Already Exist!" });
                }
                else
                {
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
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginModel login)
        {
            var result = manager.Login(login);

            if (result != "Invalid Email or Password")
            {
                return Ok(new ResponseModel<string> { Success = true, Message = "Login Successful", Data = result });
            }

            return BadRequest(new ResponseModel<string> { Success = false, Message = result });
        }


        [HttpGet("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            try
            {
                // Validate email input
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new ResponseModel<string>
                    {Success = false, Message = "Email cannot be empty", Data = null });
                }

                // Fetch the token for password reset
                ForgetPasswordModel forgetPasswordModel = manager.ForgetPassword(email);

                if (forgetPasswordModel == null)
                {
                    return NotFound(new ResponseModel<string>
                    {Success = false, Message = "Email not found", Data = null });
                }

                // Send email
                Send send = new Send();
                try
                {
                    send.SendMail(forgetPasswordModel.Email, forgetPasswordModel.Token);
                }
                catch (Exception emailEx)
                {
                    return BadRequest(new ResponseModel<string>
                    {Success = false, Message = "Failed to send email", Data = emailEx.Message });
                }

                // Send message to RabbitMQ
                Uri uri = new Uri("rabbitmq://localhost/FunDooNotesEmailQueue");
                var endPoint = await _bus.GetSendEndpoint(uri);
                await endPoint.Send(forgetPasswordModel);

                return Ok(new ResponseModel<string>
                {Success = true, Message = "Mail sent for resetting password!", Data = forgetPasswordModel.Token });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
                {Success = false, Message = "Mail not sent", Data = ex.Message});
            }
        }

        //[Authorize]
        [HttpPost]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(string email, ResetPasswordModel resetPasswordModel)
        {
            if(resetPasswordModel.Password == resetPasswordModel.ConfirmPassword)
            {
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
    }
}