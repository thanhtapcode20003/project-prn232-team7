using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Role
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? Datecreate { get; set; }

    public DateTime? Dateupdate { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
