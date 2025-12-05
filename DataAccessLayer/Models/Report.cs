using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Report
{
    public Guid Id { get; set; }

    public Guid? ItemId { get; set; }

    public Guid UserId { get; set; }

    public string Description { get; set; } = null!;

    public string Location { get; set; } = null!;

    public DateTime ActionDate { get; set; }

    public string Status { get; set; } = null!;

    public string Type { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public virtual Item? Item { get; set; }

    public virtual User User { get; set; } = null!;
}
