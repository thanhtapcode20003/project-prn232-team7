using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Item
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string FoundLocation { get; set; } = null!;

    public Guid? FoundCampusId { get; set; }

    public DateTime FoundDate { get; set; }

    public Guid CurrentLocationId { get; set; }

    public string Status { get; set; } = null!;

    public Guid CategoryId { get; set; }

    public Guid UserId { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ServiceLocation CurrentLocation { get; set; } = null!;

    public virtual Campus? FoundCampus { get; set; }

    public virtual ICollection<ItemHistory> ItemHistories { get; set; } = new List<ItemHistory>();

    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<ReturnRecord> ReturnRecords { get; set; } = new List<ReturnRecord>();

    public virtual User User { get; set; } = null!;
}
