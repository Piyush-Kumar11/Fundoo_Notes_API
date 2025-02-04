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
            if (result == "Login Successful")
            {
                return Ok(new ResponseModel<string> { Success = true, Message = result });
            }

            return BadRequest(new ResponseModel<string> { Success = false, Message = result });
        }
    }
}