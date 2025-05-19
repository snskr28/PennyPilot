using System;
using System.Collections.Generic;

namespace PennyPilot.Backend.Domain.Entities;

public partial class UserCategory
{
    public Guid UserCategoryId { get; set; }

    public Guid UserId { get; set; }

    public Guid CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
