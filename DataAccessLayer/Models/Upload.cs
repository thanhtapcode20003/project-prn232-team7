using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Upload
{
    public Guid UploadId { get; set; }

    public Guid ItemId { get; set; }

    public string FileUrl { get; set; } = null!;

    public DateTime? UploadTime { get; set; }

    public string Status { get; set; } = null!;

    public string? StatusAccept { get; set; }

    public virtual Item Item { get; set; } = null!;
}
