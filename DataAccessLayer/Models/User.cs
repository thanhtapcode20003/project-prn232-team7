using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Avt { get; set; }

    public string? Address { get; set; }

    public int Status { get; set; }

    public string? Gmail { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<ReturnRecord> ReturnRecords { get; set; } = new List<ReturnRecord>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Upload> UploadStaffIdAcceptNavigations { get; set; } = new List<Upload>();

    public virtual ICollection<Upload> UploadStaffs { get; set; } = new List<Upload>();

    public virtual ICollection<Upload> UploadUsers { get; set; } = new List<Upload>();
}
