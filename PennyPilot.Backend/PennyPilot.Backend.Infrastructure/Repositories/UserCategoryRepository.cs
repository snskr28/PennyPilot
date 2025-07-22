using Microsoft.EntityFrameworkCore;
using PennyPilot.Backend.Domain.Entities;
using PennyPilot.Backend.Domain.Interfaces;
using PennyPilot.Backend.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Infrastructure.Repositories
{
    public class UserCategoryRepository : Repository<Usercategory>, IUserCategoryRepository
    {
        public UserCategoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<bool> ExistsAsync(Guid userId, Guid categoryId)
        {
            return await _context.Usercategories
                .AnyAsync(uc => uc.Userid == userId && uc.Categoryid == categoryId);
        }
    }
}
