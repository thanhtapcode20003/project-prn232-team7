using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class ItemHistory
{
    public Guid Id { get; set; }

    public Guid ItemId { get; set; }

    public string Action { get; set; } = null!;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public Guid ChangedBy { get; set; }

    public string? Notes { get; set; }

    public DateTime DateCreated { get; set; }

    public virtual User ChangedByNavigation { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;
}
