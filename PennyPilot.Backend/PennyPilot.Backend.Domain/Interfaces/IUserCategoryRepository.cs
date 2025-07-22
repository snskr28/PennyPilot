using PennyPilot.Backend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Domain.Interfaces
{
    public interface IUserCategoryRepository : IRepository<Usercategory>
    {
        Task<bool> ExistsAsync(Guid userId, Guid categoryId);
    }
}
