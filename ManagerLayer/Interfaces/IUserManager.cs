using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entities;

namespace ManagerLayer.Interfaces
{
    public interface IUserManager
    {
        public UserEntity Registration(RegisterModel model);
        public bool MailExist(string email);
        public string Login(LoginModel login);
        public ForgetPasswordModel ForgetPassword(string email);
        public bool ResetPassword(string email, ResetPasswordModel resetPassword);

    }
}
