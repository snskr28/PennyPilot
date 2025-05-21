using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PennyPilot.Backend.Domain.Entities;
using PennyPilot.Backend.Domain.Interfaces;
using PennyPilot.Backend.Infrastructure.Data;

namespace PennyPilot.Backend.Infrastructure.Repositories
{
    public class IncomeRepository : Repository<Income>, IIncomeRepository
    {
        public IncomeRepository(ApplicationDbContext context) : base(context) { }
    }
}
