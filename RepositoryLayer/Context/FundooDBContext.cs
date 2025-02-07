using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entities;

namespace RepositoryLayer.Context
{
    // FundooDBContext class inherits from DbContext to provide a connection to the database and define DB sets.
    public class FundooDBContext : DbContext
    {
        // Constructor to initialize the DbContext with options passed from dependency injection.
        public FundooDBContext(DbContextOptions options) : base(options) { }

        // DbSet representing the Users table in the database.
        // The Users property is mapped to the UserEntity class, which represents the structure of the table.
        public DbSet<UserEntity> Users { get; set; }

        // DbSet representing the Notes table in the database.
        // The Notes property is mapped to the NotesEntity class, which represents the structure of the table.
        public DbSet<NotesEntity> Notes { get; set; }
    }
}
