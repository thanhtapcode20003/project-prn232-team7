using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Role
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
