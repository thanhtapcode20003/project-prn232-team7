using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Campus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Address { get; set; }

    public int Status { get; set; }

    public DateTime? Datecreate { get; set; }

    public DateTime? Dateupdate { get; set; }

    public virtual ICollection<ServiceLocation> ServiceLocations { get; set; } = new List<ServiceLocation>();
}
