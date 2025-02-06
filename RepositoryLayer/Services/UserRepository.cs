using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using CommonLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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

        public ForgetPasswordModel ForgetPassword(string email)
        {
            //UserEntity user = GetUserByEmail(email);
            var user = context.Users.FirstOrDefault(e => e.Email == email);

            if (user != null)
            {
                // Create a password reset model
                return new ForgetPasswordModel
                {
                    UserId = user.UserID, Email = user.Email, Token = GenerateJWTToken(user.Email, user.UserID) // Generate JWT Token
                };
            }
            else
            {
                throw new Exception("User Not Exist for required email!");
            }
        }

        public bool ResetPassword(string email, ResetPasswordModel resetPasswordModel)
        {
            // Retrieve the user from the database whose Email matches the given email.
            UserEntity user = context.Users.FirstOrDefault(e => e.Email == email);

            // Check if a user with the specified email exists.
            if (user != null)
            {
                // Encode the new password provided in the resetPasswordModel and update it in the database.
                user.Password = EncodePassword(resetPasswordModel.Password);

                // Save the changes to the database.
                context.SaveChanges();

                return true;
            }
            else
            {
                // Throw an exception if no user exists for the given email.
                throw new Exception("User Not Exist for this email!");
            }
        }

        public static string EncodePassword(string password)
        {
            try
            {
                // Convert the input password string into a byte array using UTF-8 encoding.
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);

                // Convert the byte array into a Base64 encoded string.
                string encodedData = Convert.ToBase64String(encData_byte);

                return encodedData;
            }
            catch (Exception ex)
            {
                // Throw a new exception with an error message and the original exception details if encoding fails.
                throw new Exception("Error in base64Encode: " + ex.Message);
            }
        }

        private string GenerateJWTToken(string email, int userID)
        {
            // Create a symmetric security key using the JWT key from configuration
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            // Set signing credentials with the security key and the algorithm for signing (HmacSha256)
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Define claims (additional information to be included in the token)
            var claims = new[]
            {
                new Claim("Email", email),  // Add email as a claim
                new Claim("UserID", userID.ToString())  // Add user ID as a claim
            };

            // Create the JWT token with expiration time (15 minutes) and signing credentials
            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],  // Issuer
                configuration["Jwt:Audience"],  // Audience
                claims,  // Claims
                expires: DateTime.Now.AddMinutes(15),  // Token expiration time
                signingCredentials: credentials);  // Signing credentials

            // Return the generated token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string Login(LoginModel login)
        {
            string encodedPassword = EncodePassword(login.Password);
            var check = context.Users.FirstOrDefault(e => e.Email == login.Email && e.Password == encodedPassword);
            if (check != null)
            {
                var token = GenerateJWTToken(check.Email, check.UserID);
                return token;
            }
            return null;
        }
    }
}
