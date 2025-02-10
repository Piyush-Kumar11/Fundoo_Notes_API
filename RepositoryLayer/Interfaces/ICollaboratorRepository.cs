using System.Collections.Generic;
using RepositoryLayer.Entities;

namespace RepositoryLayer.Interfaces
{
    public interface ICollaboratorRepository
    {
        CollaboratorEntity AddCollaborator(CollaboratorEntity collaborator);
        bool RemoveCollaborator(int collaboratorId, int userId);
        List<CollaboratorEntity> GetCollaboratorsByUser(int userId);
    }
}
