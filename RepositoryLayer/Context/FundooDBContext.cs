using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entities;

namespace RepositoryLayer.Context
{
    // FundooDBContext class inherits from DbContext to provide a connection to the database and define DB sets
    public class FundooDBContext : DbContext
    {
        public FundooDBContext(DbContextOptions options) : base(options) { }

        // DbSet representing the Users table in the database
        public DbSet<UserEntity> Users { get; set; } // Users is mapped to the UserEntity entity
    }
}
