using System;
using System.Collections.Generic;

namespace PennyPilot.Backend.Domain.Entities;

public partial class Income
{
    public Guid IncomeId { get; set; }

    public Guid UserId { get; set; }

    public Guid CategoryId { get; set; }

    public string Source { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
