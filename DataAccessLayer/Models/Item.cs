using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Item
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Img { get; set; }

    public Guid CategoryId { get; set; }

    public string? Status { get; set; }

    public DateTime? Date { get; set; }

    public string? FoundLocation { get; set; }

    public Guid? CurrentLocationId { get; set; }

    public string? Context { get; set; }

    public Guid? UserId { get; set; }

    public DateTime? FoundDate { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ServiceLocation? CurrentLocation { get; set; }

    public virtual ICollection<ReturnRecord> ReturnRecords { get; set; } = new List<ReturnRecord>();

    public virtual User? User { get; set; }
}
