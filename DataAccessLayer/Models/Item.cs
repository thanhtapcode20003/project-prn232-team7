using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Item
{
    public Guid ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? LostDate { get; set; }

    public TimeOnly? LostTime { get; set; }

    public Guid CategoryId { get; set; }

    public Guid UserId { get; set; }

    public Guid LocationId { get; set; }

    public string Status { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ServiceLocation Location { get; set; } = null!;

    public virtual ICollection<ReturnRecord> ReturnRecords { get; set; } = new List<ReturnRecord>();

    public virtual ICollection<Upload> Uploads { get; set; } = new List<Upload>();

    public virtual User User { get; set; } = null!;
}
