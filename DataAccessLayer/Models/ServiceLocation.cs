using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class ServiceLocation
{
    public Guid Id { get; set; }

    public Guid CampusId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdate { get; set; }

    public virtual Campus Campus { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
