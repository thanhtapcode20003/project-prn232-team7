using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class ServiceLocation
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Address { get; set; }

    public string? Status { get; set; }

    public DateTime? Datecreate { get; set; }

    public DateTime? Dateupdate { get; set; }

    public Guid CampusId { get; set; }

    public virtual Campus Campus { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
