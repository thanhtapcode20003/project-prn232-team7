using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Receipt
{
    public Guid Id { get; set; }

    public Guid ItemId { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdate { get; set; }

    public Guid? StaffId { get; set; }

    public string? VerifyNotes { get; set; }

    public DateTime? DateVerified { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual User? Staff { get; set; }

    public virtual User User { get; set; } = null!;
}
