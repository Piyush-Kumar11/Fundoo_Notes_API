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
        public List<NotesEntity> FindAllNotes(int UserId)
        {
            return notesRepository.FindAllNotes(UserId);
        }
    }
}
