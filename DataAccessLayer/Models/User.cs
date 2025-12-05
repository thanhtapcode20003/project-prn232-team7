using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Avatar { get; set; }

    public string? Phone { get; set; }

    public string? Mssv { get; set; }

    public Guid RoleId { get; set; }

    public Guid? CampusId { get; set; }

    public string Status { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Gmail { get; set; } = null!;

    public DateTime? Dob { get; set; }

    public DateTime DateCreated { get; set; }

    public virtual Campus? Campus { get; set; }

    public virtual ICollection<ImageStorage> ImageStorages { get; set; } = new List<ImageStorage>();

    public virtual ICollection<ItemHistory> ItemHistories { get; set; } = new List<ItemHistory>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Receipt> ReceiptStaffs { get; set; } = new List<Receipt>();

    public virtual ICollection<Receipt> ReceiptUsers { get; set; } = new List<Receipt>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<ReturnRecord> ReturnRecordStaffs { get; set; } = new List<ReturnRecord>();

    public virtual ICollection<ReturnRecord> ReturnRecordUsers { get; set; } = new List<ReturnRecord>();

    public virtual Role Role { get; set; } = null!;
}
