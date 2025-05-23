using PennyPilot.Backend.Domain.Interfaces;
using PennyPilot.Backend.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IUserRepository Users { get; }
        public IExpenseRepository Expenses { get; }
        public ICategoryRepository Categories { get; }
        public IUserCategoryRepository UserCategories { get; }
        public IIncomeRepository Incomes { get; }

        public UnitOfWork(ApplicationDbContext context, IUserRepository userRepository,
            IExpenseRepository expenseRepo, ICategoryRepository categoryRepo, IUserCategoryRepository userCategoryRepo, IIncomeRepository income)
        {
            _context = context;
            Users = userRepository;
            Expenses = expenseRepo;
            Categories = categoryRepo;
            UserCategories = userCategoryRepo;
            Incomes = income;
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new Repository<T>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
