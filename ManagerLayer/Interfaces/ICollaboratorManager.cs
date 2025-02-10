using System.Collections.Generic;
using RepositoryLayer.Entities;

namespace ManagerLayer.Interfaces
{
    public interface ICollaboratorManager
    {
        CollaboratorEntity AddCollaborator(CollaboratorEntity collaborator);
        bool RemoveCollaborator(int collaboratorId, int userId);
        List<CollaboratorEntity> GetCollaboratorsByUser(int userId);
    }
}
