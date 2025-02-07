using System.Collections.Generic;
using RepositoryLayer.Entities;

namespace RepositoryLayer.Interfaces
{
    public interface ILabelRepository
    {
        LabelEntity CreateLabel(LabelEntity label);
        List<LabelEntity> GetLabelsByUser(int UserId);
        LabelEntity UpdateLabel(int labelId, string newLabelName, int UserId);
        bool DeleteLabel(int labelId, int UserId);
    }
}
