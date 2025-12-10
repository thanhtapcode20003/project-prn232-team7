using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Role
{
    public Guid RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
