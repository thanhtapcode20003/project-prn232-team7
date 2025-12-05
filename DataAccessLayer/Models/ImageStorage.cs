using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class ImageStorage
{
    public Guid Id { get; set; }

    public string EntityType { get; set; } = null!;

    public Guid EntityId { get; set; }

    public string ImageType { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public int? ImageOrder { get; set; }

    public long? FileSize { get; set; }

    public string? MimeType { get; set; }

    public Guid UploadedBy { get; set; }

    public DateTime DateCreated { get; set; }

    public string Status { get; set; } = null!;

    public virtual User UploadedByNavigation { get; set; } = null!;
}
