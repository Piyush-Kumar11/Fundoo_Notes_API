using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Models
{
    // The RegisterModel class is used to define the properties required for user registration.
    // This class acts as a data model to capture and store user input during registration.
    public class RegisterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
