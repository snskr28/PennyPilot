using System;
using System.Collections.Generic;

namespace PennyPilot.Backend.Domain.Entities;

/// <summary>
/// Junction table linking users to their custom categories
/// </summary>
public partial class Usercategory
{
    public Guid Usercategoryid { get; set; }

    public Guid Userid { get; set; }

    public Guid Categoryid { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
