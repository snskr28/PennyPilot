using System;
using System.Collections.Generic;

namespace PennyPilot.Backend.Domain.Entities;

/// <summary>
/// Stores income transactions
/// </summary>
public partial class Income
{
    public Guid Incomeid { get; set; }

    public Guid Userid { get; set; }

    public Guid Categoryid { get; set; }

    public string Source { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public bool Isenabled { get; set; }

    public bool Isdeleted { get; set; }

    public string Title { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
