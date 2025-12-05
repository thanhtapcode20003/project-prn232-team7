using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Campus
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<ServiceLocation> ServiceLocations { get; set; } = new List<ServiceLocation>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
