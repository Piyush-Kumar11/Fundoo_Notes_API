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

        // DbSet representing the Labels table in the database.
        // The Labels property is mapped to the LabelsEntity class, which represents the structure of the table.
        public DbSet<LabelEntity> Labels { get; set; }

        // DbSet representing the Collaborator table in the database.
        // The Collaborator property is mapped to the CollaboratorEntity class, which represents the structure of the table.
        public DbSet<CollaboratorEntity> Collaborators { get; set; }

        public DbSet<ProductEntity> Products { get; set; }

    }
}