using System;
using System.Collections.Generic;

namespace PennyPilot.Backend.Domain.Entities;

/// <summary>
/// Stores expense transactions
/// </summary>
public partial class Expense
{
    public Guid Expenseid { get; set; }

    public Guid Userid { get; set; }

    public Guid Categoryid { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public string Paymentmode { get; set; } = null!;

    public string Paidby { get; set; } = null!;

    public DateTime Date { get; set; }

    public byte[]? Receiptimage { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public bool Isenabled { get; set; }

    public bool Isdeleted { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
