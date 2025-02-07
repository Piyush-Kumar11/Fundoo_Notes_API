using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using ManagerLayer.Interfaces;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;

namespace ManagerLayer.Services
{
    public class NotesManager:INotesManager
    {
        private readonly INotesRepository notesRepository;

        public NotesManager(INotesRepository notesRepository)
        {
            this.notesRepository = notesRepository;
        }

        public NotesEntity CreateNote(NotesModel notesModel, int UserId)
        {
            return notesRepository.CreateNote(notesModel, UserId);
        }

        public bool DeleteNote(int noteId, int UserId)
        {
            return notesRepository.DeleteNote(noteId, UserId);
        }

        public List<NotesEntity> FindAllNotes(int UserId)
        {
            return notesRepository.FindAllNotes(UserId);
        }

        public NotesEntity GetNoteByIds(int UserId, int notesId)
        {
            return notesRepository.GetNoteByIds(UserId, notesId);
        }

        public bool ToggleArchiveNote(int UserId, int noteId)
        {
            return notesRepository.ToggleArchiveNote(UserId, noteId);
        }

        public bool TogglePinNote(int UserId, int noteId)
        {
            return notesRepository.TogglePinNote(UserId, noteId);
        }

        public bool ToggleTrashNote(int UserId, int noteId)
        {
            return notesRepository.ToggleTrashNote(UserId, noteId);
        }

        public NotesEntity UpdateNote(int noteId, NotesModel notesModel, int UserId)
        {
            return notesRepository.UpdateNote(noteId, notesModel, UserId);
        }

        public bool UpdateNoteColor(int noteId, string color, int userId)
        {
            return notesRepository.UpdateNoteColor(noteId, color, userId);
        }

        public bool UpdateNoteRemainder(int noteId, DateTime remainder, int userId)
        {
            return notesRepository.UpdateNoteRemainder(noteId, remainder, userId);
        }

        public bool UpdateNoteImage(int noteId, string imageUrl, int userId)
        {
            return notesRepository.UpdateNoteImage(noteId, imageUrl, userId);
        }
    }
}
