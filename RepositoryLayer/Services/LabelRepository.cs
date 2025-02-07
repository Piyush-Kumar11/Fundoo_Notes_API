using System;
using System.Collections.Generic;
using System.Linq;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;

namespace RepositoryLayer.Services
{
    public class LabelRepository : ILabelRepository
    {
        private readonly FundooDBContext dbContext;

        public LabelRepository(FundooDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public LabelEntity CreateLabel(LabelEntity label)
        {
            dbContext.Labels.Add(label);
            dbContext.SaveChanges();
            return label;
        }

        public List<LabelEntity> GetLabelsByUser(int UserId)
        {
            return dbContext.Labels.Where(label => label.UserID == UserId).ToList();
        }

        public LabelEntity UpdateLabel(int labelId, string newLabelName, int UserId)
        {
            var label = dbContext.Labels.FirstOrDefault(l => l.LabelId == labelId && l.UserID == UserId);
            if (label != null)
            {
                label.LabelName = newLabelName;
                dbContext.SaveChanges();
                return label;
            }
            return null;
        }

        public bool DeleteLabel(int labelId, int UserId)
        {
            var label = dbContext.Labels.FirstOrDefault(l => l.LabelId == labelId && l.UserID == UserId);
            if (label != null)
            {
                dbContext.Labels.Remove(label);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
