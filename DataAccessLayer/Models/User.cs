using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public Guid? CampusId { get; set; }

    public Guid? RoleId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Campus? Campus { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<ReturnRecord> ReturnRecordFoundUsers { get; set; } = new List<ReturnRecord>();

    public virtual ICollection<ReturnRecord> ReturnRecordReceiverUsers { get; set; } = new List<ReturnRecord>();

    public virtual Role? Role { get; set; }
}
