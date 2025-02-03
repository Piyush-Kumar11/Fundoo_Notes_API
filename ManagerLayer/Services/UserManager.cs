using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using ManagerLayer.Interfaces;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;

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
    }
}
