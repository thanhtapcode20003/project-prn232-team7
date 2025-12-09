using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class ReturnRecord
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public int StaffId { get; set; }

    public string? Name { get; set; }

    public string? ImgCccdFront { get; set; }

    public string? ImgCccdBack { get; set; }

    public string? EvidenceImg { get; set; }

    public string? ConfirmImg { get; set; }

    public string? VerifyNotes { get; set; }

    public int? Status { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdate { get; set; }

    public string? Mssv { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual User Staff { get; set; } = null!;
}
