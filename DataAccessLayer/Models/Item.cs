using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Item
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Img { get; set; }

    public int CategoryId { get; set; }

    public int Status { get; set; }

    public DateTime? Date { get; set; }

    public string? FoundLocation { get; set; }

    public int? CurrentLocationId { get; set; }

    public string? Content { get; set; }

    public int? UserId { get; set; }

    public DateTime? FoundDate { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ServiceLocation? CurrentLocation { get; set; }

    public virtual ICollection<ReturnRecord> ReturnRecords { get; set; } = new List<ReturnRecord>();

    public virtual ICollection<Upload> Uploads { get; set; } = new List<Upload>();

    public virtual User? User { get; set; }
}
