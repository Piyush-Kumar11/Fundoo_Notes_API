using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entities;

namespace RepositoryLayer.Interfaces
{
    public interface INotesRepository
    {
        public NotesEntity CreateNote(NotesModel notesModel, int UserId);
        List<NotesEntity> FindAllNotes(int UserId);
        public NotesEntity UpdateNote(int noteId, NotesModel notesModel, int UserId);
        public bool DeleteNote(int noteId, int UserId);
        public NotesEntity GetNoteByIds(int UserId, int notesId);
        public bool TogglePinNote(int UserId, int noteId);
        public bool ToggleArchiveNote(int UserId, int noteId);
        public bool ToggleTrashNote(int UserId, int noteId);
        public bool UpdateNoteColor(int noteId, string color, int userId);
        public bool UpdateNoteRemainder(int noteId, DateTime remainder, int userId);
        public bool UpdateNoteImage(int noteId, string imageUrl, int userId);
    }
}
