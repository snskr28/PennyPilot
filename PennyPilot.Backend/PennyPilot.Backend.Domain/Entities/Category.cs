using System;
using System.Collections.Generic;

namespace PennyPilot.Backend.Domain.Entities;

public partial class Category
{
    public Guid CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Type { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();

    public virtual ICollection<UserCategory> UserCategories { get; set; } = new List<UserCategory>();
}
