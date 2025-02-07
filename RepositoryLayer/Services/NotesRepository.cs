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

        public bool DeleteNote(int noteId, int UserId)
        {
            var note = dbContext.Notes.FirstOrDefault(n => n.NotesId == noteId && n.UserID == UserId);
            if (note == null) return false;

            dbContext.Notes.Remove(note);
            dbContext.SaveChanges();

            return true;
        }

        public List<NotesEntity> FindAllNotes(int UserId)
        {
            return dbContext.Notes.Where(note => note.UserID == UserId).ToList();
        }

        public NotesEntity UpdateNote(int noteId, NotesModel notesModel, int UserId)
        {
            var note = dbContext.Notes.FirstOrDefault(n => n.NotesId == noteId && n.UserID == UserId);
            if (note == null) return null;

            note.Title = notesModel.Title;
            note.Description = notesModel.Description;
            note.LastUpdatedAt = DateTime.Now;

            dbContext.Notes.Update(note);
            dbContext.SaveChanges();

            return note;
        }

        public NotesEntity GetNoteByIds(int UserId, int notesId)
        {
            return dbContext.Notes.FirstOrDefault(n => n.NotesId == notesId && n.UserID == UserId);
        }

        public bool TogglePinNote(int UserId, int noteId)
        {
            var note = GetNoteByIds(UserId, noteId);
            if(note != null)
            {
                note.IsPin = !note.IsPin;
                note.LastUpdatedAt = DateTime.Now;
                dbContext.SaveChanges();
                return true;
            }
            else
            {
                throw new Exception("Note not found with requested Id: " + noteId);
            }
        }
        public bool ToggleArchiveNote(int UserId, int noteId)
        {
            var note = GetNoteByIds(UserId, noteId);
            if (note != null)
            {
                if (note.IsPin)
                {
                    note.IsPin = false;
                }
                note.IsArchive = !note.IsArchive;
                note.LastUpdatedAt = DateTime.Now;
                dbContext.SaveChanges();
                return true;
            }
            else
            {
                throw new Exception("Note not found with requested Id: " + noteId);
            }
        }

        public bool ToggleTrashNote(int UserId, int noteId)
        {
            var note = GetNoteByIds(UserId, noteId);
            if (note != null)
            {
                if (note.IsPin)
                {
                    note.IsPin = false;
                }

                if (note.IsTrash)
                {
                    // If the note is already in trash, restore it
                    note.IsTrash = false;
                }
                else
                {
                    // If the note is being trashed, also remove archive status
                    note.IsTrash = true;
                    note.IsArchive = false;  // Ensures a trashed note is NOT archived
                }

                note.LastUpdatedAt = DateTime.Now;
                dbContext.SaveChanges();
                return true;
            }
            else
            {
                throw new Exception("Note not found with requested Id: " + noteId);
            }
        }
    }
}
