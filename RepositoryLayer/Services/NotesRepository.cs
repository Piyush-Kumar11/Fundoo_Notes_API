using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;

namespace RepositoryLayer.Services
{
    public class NotesRepository:INotesRepository
    {
        private readonly FundooDBContext dbContext;

        public NotesRepository(FundooDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public NotesEntity CreateNote(NotesModel notesModel, int UserId)
        {
            NotesEntity notesEntity = new NotesEntity();
            notesEntity.Title = notesModel.Title;
            notesEntity.Description = notesModel.Description;
            notesEntity.CreatedAt = DateTime.Now;
            notesEntity.LastUpdatedAt = DateTime.Now;
            notesEntity.UserID = UserId;

            dbContext.Notes.Add(notesEntity);
            dbContext.SaveChanges();

            return notesEntity;
        }
        public List<NotesEntity> FindAllNotes(int UserId)
        {
            return dbContext.Notes.Where(note => note.UserID == UserId).ToList();
        }
    }
}
