using System;
using System.Collections.Generic;
using System.Linq;
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
            users.Password = EncodePassword(model.Password);

            // Adding and saving the new user entity to the database
            context.Users.Add(users);
            context.SaveChanges();

            return users;
        }

        public UserEntity Login(LoginModel login)
        {
            string encodedPassword = EncodePassword(login.Password);
            return context.Users.FirstOrDefault(e => e.Email == login.Email && e.Password == encodedPassword);
        }

        public ForgetPasswordModel ForgetPassword(string email)
        {
            var user = context.Users.FirstOrDefault(e => e.Email == email);

            if (user == null)
            {
                return new ForgetPasswordModel
                {
                    Email = email,
                    Message = "Email not found!"
                };
            }

            return new ForgetPasswordModel
            {
                Email = email,
                Message = "An email has been sent to reset your password !"
            };
        }

        public static string EncodePassword(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }

    }
}
