using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Ext { get; set; }

    public string? Address { get; set; }

    public string? Status { get; set; }

    public string Gmail { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public Guid RoleId { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<ReturnRecord> ReturnRecordStaffs { get; set; } = new List<ReturnRecord>();

    public virtual ICollection<ReturnRecord> ReturnRecordUsers { get; set; } = new List<ReturnRecord>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Upload> UploadStaffs { get; set; } = new List<Upload>();

    public virtual ICollection<Upload> UploadUsers { get; set; } = new List<Upload>();
}
