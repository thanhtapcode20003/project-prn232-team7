using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class ReturnRecord
{
    public Guid Id { get; set; }

    public Guid ItemId { get; set; }

    public Guid StaffId { get; set; }

    public Guid UserId { get; set; }

    public string ImgCccdFront { get; set; } = null!;

    public string ImgCccdBack { get; set; } = null!;

    public string EvidenceImg { get; set; } = null!;

    public string ConfirmImg { get; set; } = null!;

    public string VerifyNotes { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdate { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual User Staff { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
