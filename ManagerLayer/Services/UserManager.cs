using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLayer.Models;
using ManagerLayer.Interfaces;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;

namespace ManagerLayer.Services
{
    // UserManager class implements the IUserManager interface for managing user-related business logic
    public class UserManager : IUserManager
    {
        private readonly IUserRepository user;
        private readonly FundooDBContext context;

        // Constructor to inject the user repository and database context via dependency injection
        public UserManager(IUserRepository user, FundooDBContext context)
        {
            this.user = user;
            this.context = context;
        }

        public UserEntity Registration(RegisterModel model)
        {
            return user.Registration(model);
        }

        public bool MailExist(string email)
        {
            var isExist = this.context.Users.FirstOrDefault(e =>e.Email == email);
            if (isExist != null)
            {
                return true;
            }
            return false;
        }

        public string Login(LoginModel login)
        {
            var encodedPassword = UserRepository.EncodePassword(login.Password);
            var userEntity = context.Users.FirstOrDefault(e => e.Email == login.Email && e.Password == encodedPassword);

            if (userEntity != null)
            {
                return "Login Successful";
            }

            return "Invalid Email or Password";
        }

        public ForgetPasswordModel ForgetPassword(string email)
        {
            return user.ForgetPassword(email);
        }
    }
}
