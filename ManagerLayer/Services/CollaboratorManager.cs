using System.Collections.Generic;
using ManagerLayer.Interfaces;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;

namespace ManagerLayer.Services
{
    public class CollaboratorManager : ICollaboratorManager
    {
        private readonly ICollaboratorRepository collaboratorRepository;

        public CollaboratorManager(ICollaboratorRepository collaboratorRepository)
        {
            this.collaboratorRepository = collaboratorRepository;
        }

        public CollaboratorEntity AddCollaborator(CollaboratorEntity collaborator)
        {
            return collaboratorRepository.AddCollaborator(collaborator);
        }

        public bool RemoveCollaborator(int collaboratorId, int userId)
        {
            return collaboratorRepository.RemoveCollaborator(collaboratorId, userId);
        }

        public List<CollaboratorEntity> GetCollaboratorsByUser(int userId)
        {
            return collaboratorRepository.GetCollaboratorsByUser(userId);
        }
    }
}
