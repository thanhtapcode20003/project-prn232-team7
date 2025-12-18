using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class ReturnRecord
{
    public Guid Id { get; set; }

    public Guid ItemId { get; set; }

    public Guid StaffId { get; set; }

    public string? ImgCccdFont { get; set; }

    public string? ImgCccdBack { get; set; }

    public string? EvidenceImg { get; set; }

    public string? ConfirmImg { get; set; }

    public string? VerifyNotes { get; set; }

    public string? Status { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdate { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual User Staff { get; set; } = null!;
}
