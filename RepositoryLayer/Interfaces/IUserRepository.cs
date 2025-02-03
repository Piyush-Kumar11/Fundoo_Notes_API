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
    }
}
