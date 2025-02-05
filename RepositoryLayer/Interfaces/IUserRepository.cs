using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entities;

namespace RepositoryLayer.Interfaces
{
    public interface IUserRepository
    {
        public UserEntity Registration(RegisterModel model);
        public string Login(LoginModel login);
        public ForgetPasswordModel ForgetPassword(string email);
        public bool ResetPassword(string email, ResetPasswordModel resetPassword);
    }
}
