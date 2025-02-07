using System.Collections.Generic;
using ManagerLayer.Interfaces;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;

namespace ManagerLayer.Services
{
    public class LabelManager : ILabelManager
    {
        private readonly ILabelRepository labelRepository;

        public LabelManager(ILabelRepository labelRepository)
        {
            this.labelRepository = labelRepository;
        }

        public LabelEntity CreateLabel(LabelEntity label)
        {
            return labelRepository.CreateLabel(label);
        }

        public List<LabelEntity> GetLabelsByUser(int UserId)
        {
            return labelRepository.GetLabelsByUser(UserId);
        }

        public LabelEntity UpdateLabel(int labelId, string newLabelName, int UserId)
        {
            return labelRepository.UpdateLabel(labelId, newLabelName, UserId);
        }

        public bool DeleteLabel(int labelId, int UserId)
        {
            return labelRepository.DeleteLabel(labelId, UserId);
        }
    }
}
