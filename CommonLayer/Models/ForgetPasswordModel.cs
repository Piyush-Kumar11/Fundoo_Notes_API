using System;

namespace CommonLayer.Models
{
    public class ForgetPasswordModel
    {
        public int UserId { get; set; }
        public string Email { get; set; } 
        public string Token { get; set; } 
    }
}
