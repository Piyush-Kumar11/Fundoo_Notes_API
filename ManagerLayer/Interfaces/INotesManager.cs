using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entities;

namespace ManagerLayer.Interfaces
{
    public interface INotesManager
    {
        public NotesEntity CreateNote(NotesModel notesModel, int UserId);
        List<NotesEntity> FindAllNotes(int UserId);

    }
}
