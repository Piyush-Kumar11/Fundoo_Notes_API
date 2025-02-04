using System;
using System.Threading.Tasks;
using CommonLayer.Models;
using ManagerLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entities;

namespace FundooNotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager manager;

        public UsersController(IUserManager manager)
        {
            this.manager = manager;
        }

        [HttpPost]
        [Route("Reg")]
        public IActionResult Register(RegisterModel model)
        {
            var checkEmail = manager.MailExist(model.Email);
            if(checkEmail)
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
                ForgetPasswordModel forgetPasswordModel = manager.ForgetPassword(email);
                Send send = new Send();

                send.SendMail(forgetPasswordModel.Email,forgetPasswordModel.Token);
                //Uri uri = new Uri("rabbitmq://localhost/FunDooNotesEmailQueue");
                //var endPoint = await bus.GetSendEndPoint(uri);
                //await endPoint.Send(forgetPasswordModel);

                return Ok(new ResponseModel<string> { Success = true, Message = "Mail Sent for resetting password!", Data = forgetPasswordModel.Token });
            }
            catch(Exception ex)
            {
                return BadRequest(new ResponseModel<string> { Success=true,Message="Mail not sent"});
            }
            //var result = manager.ForgetPassword(email);

            //if (result.Message == "Email not found!")
            //{
            //    return BadRequest(new ResponseModel<ForgetPasswordModel>
            //    {
            //        Success = false,
            //        Message = result.Message
            //    });
            //}

            //return Ok(new ResponseModel<ForgetPasswordModel>
            //{
            //    Success = true,
            //    Message = result.Message,
            //    Data = result
            //});
        }

    }
}