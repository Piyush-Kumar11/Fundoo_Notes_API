using System.Collections.Generic;
using System.Linq;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;

namespace RepositoryLayer.Services
{
    public class CollaboratorRepository : ICollaboratorRepository
    {
        private readonly FundooDBContext dbContext;

        public CollaboratorRepository(FundooDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public CollaboratorEntity AddCollaborator(CollaboratorEntity collaborator)
        {
            dbContext.Collaborators.Add(collaborator);
            dbContext.SaveChanges();
            return collaborator;
        }

        public bool RemoveCollaborator(int collaboratorId, int userId)
        {
            var collaborator = dbContext.Collaborators
                .FirstOrDefault(c => c.CollaboratorId == collaboratorId && c.UserID == userId);

            if (collaborator != null)
            {
                dbContext.Collaborators.Remove(collaborator);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public List<CollaboratorEntity> GetCollaboratorsByUser(int userId)
        {
            return dbContext.Collaborators.Where(c => c.UserID == userId).ToList();
        }
    }
}
