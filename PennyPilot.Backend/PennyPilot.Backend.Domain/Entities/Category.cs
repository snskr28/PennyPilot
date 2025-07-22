using System;
using System.Collections.Generic;

namespace PennyPilot.Backend.Domain.Entities;

/// <summary>
/// Stores income and expense categories
/// </summary>
public partial class Category
{
    public Guid Categoryid { get; set; }

    public string Name { get; set; } = null!;

    public string? Type { get; set; }

    public bool Isenabled { get; set; }

    public bool Isdeleted { get; set; }

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();

    public virtual ICollection<Usercategory> Usercategories { get; set; } = new List<Usercategory>();
}
