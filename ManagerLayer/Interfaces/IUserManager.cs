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

    }
}
