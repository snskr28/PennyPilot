using System;
using System.Collections.Generic;

namespace PennyPilot.Backend.Domain.Entities;

/// <summary>
/// Stores user account information
/// </summary>
public partial class User
{
    public Guid Userid { get; set; }

    public string Username { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string? Middlename { get; set; }

    public string Lastname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public DateTime? Createdat { get; set; }

    public bool Isenabled { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime? Dob { get; set; }

    public string? Passwordresettoken { get; set; }

    public DateTime? Passwordresettokenexpiry { get; set; }

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();

    public virtual ICollection<Usercategory> Usercategories { get; set; } = new List<Usercategory>();
}
