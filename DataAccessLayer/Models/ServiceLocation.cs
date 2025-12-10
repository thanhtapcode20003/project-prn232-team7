using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class ServiceLocation
{
    public Guid ServiceLocationId { get; set; }

    public Guid CampusId { get; set; }

    public string LocationName { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Campus Campus { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
