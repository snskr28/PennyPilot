using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Application.Interfaces;
using PennyPilot.Backend.Application.Utilities;
using PennyPilot.Backend.Domain.Entities;
using PennyPilot.Backend.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExpenseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> AddExpenseAsync(Guid userId, AddExpenseDto dto)
        {
            string formattedCategory = dto.CategoryName.ToPascalCase();
            var category = await _unitOfWork.Categories.GetByNameAsync(formattedCategory);

            if (category == null)
            {
                category = new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = formattedCategory,
                    IsEnabled = true,
                    IsDeleted = false
                };
                await _unitOfWork.Categories.AddAsync(category);
            }

            // Map User and Category
            bool isMapped = await _unitOfWork.UserCategories.ExistsAsync(userId, category.CategoryId);
            if (!isMapped)
            {
                var userCategory = new UserCategory
                {
                    UserCategoryId = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.CategoryId
                };
                await _unitOfWork.UserCategories.AddAsync(userCategory);
            }

            var expense = new Expense
            {
                ExpenseId = Guid.NewGuid(),
                UserId = userId,
                CategoryId = category.CategoryId,
                Title = dto.Title,
                Description = dto.Description,
                Amount = dto.Amount,
                PaymentMode = dto.PaymentMode,
                PaidBy = dto.PaidBy,
                Date = dto.Date,
                ReceiptImage = dto.ReceiptImage,
                CreatedAt = DateTime.UtcNow,
                IsEnabled = true,
                IsDeleted = false
            };

            await _unitOfWork.Expenses.AddAsync(expense);
            await _unitOfWork.SaveChangesAsync();

            return expense.ExpenseId;
        }

        public async Task UpdateExpenseAsync(Guid userId, UpdateExpenseDto dto)
        {
            var expense = await _unitOfWork.Expenses.GetByIdAsync(dto.ExpenseId)
                ?? throw new Exception("Expense not found");

            if (expense.UserId != userId)
                throw new UnauthorizedAccessException();

            string formattedCategory = dto.CategoryName.ToPascalCase();
            var category = await _unitOfWork.Categories.GetByNameAsync(formattedCategory);
            if (category == null)
            {
                category = new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = formattedCategory,
                    IsEnabled = true,
                    IsDeleted = false
                };
                await _unitOfWork.Categories.AddAsync(category);
            }

            bool isMapped = await _unitOfWork.UserCategories.ExistsAsync(userId, category.CategoryId);
            if (!isMapped)
            {
                var userCategory = new UserCategory
                {
                    UserCategoryId = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.CategoryId
                };
                await _unitOfWork.UserCategories.AddAsync(userCategory);
            }

            expense.Title = dto.Title;
            expense.Description = dto.Description;
            expense.Amount = dto.Amount;
            expense.PaymentMode = dto.PaymentMode;
            expense.PaidBy = dto.PaidBy;
            expense.Date = dto.Date;
            expense.ReceiptImage = dto.ReceiptImage;
            expense.CategoryId = category.CategoryId;
            expense.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteExpenseAsync(Guid userId, Guid expenseId)
        {
            var expense = await _unitOfWork.Expenses.GetByIdAsync(expenseId)
                ?? throw new Exception("Expense not found");

            if (expense.UserId != userId)
                throw new UnauthorizedAccessException();

            expense.IsDeleted = true;
            expense.IsEnabled = false;
            expense.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
