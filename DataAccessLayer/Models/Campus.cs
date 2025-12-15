using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Campus
{
    public Guid CampusId { get; set; }

    public string CampusName { get; set; } = null!;

    public string? Address { get; set; }

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<ServiceLocation> ServiceLocations { get; set; } = new List<ServiceLocation>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
