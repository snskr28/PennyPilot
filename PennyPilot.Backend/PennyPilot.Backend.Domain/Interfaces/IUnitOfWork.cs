using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IExpenseRepository Expenses { get; }
        ICategoryRepository Categories { get; }
        IUserCategoryRepository UserCategories { get; }
        IIncomeRepository Incomes { get; }
        IRepository<T> GetRepository<T>() where T : class;
        Task<int> SaveChangesAsync();
    }
}
