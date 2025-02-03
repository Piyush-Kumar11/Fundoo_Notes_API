using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;

namespace RepositoryLayer.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly FundooDBContext context;
        private readonly IConfiguration configuration;

        // Constructor to inject the database context and configuration via dependency injection
        public UserRepository(FundooDBContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        // Method to register a new user and save the details in the database
        public UserEntity Registration(RegisterModel model)
        {
            UserEntity users = new UserEntity();
            users.FirstName = model.FirstName;
            users.LastName = model.LastName;
            users.DOB = model.DOB;
            users.Gender = model.Gender;
            users.Email = model.Email;
            users.Password = model.Password;

            // Adding and saving the new user entity to the database
            context.Users.Add(users);
            context.SaveChanges();

            return users;
        }
        
    }
}
