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
    }
}
