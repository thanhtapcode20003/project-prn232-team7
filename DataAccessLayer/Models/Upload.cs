using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Upload
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Img { get; set; }

    public string? Description { get; set; }

    public Guid CategoryId { get; set; }

    public string? LostLocation { get; set; }

    public DateTime? LostDate { get; set; }

    public string? Content { get; set; }

    public string? Status { get; set; }

    public Guid? Staffid { get; set; }

    public DateTime? DateCreate { get; set; }

    public Guid Userid { get; set; }

    public string? Type { get; set; }

    public string? Note { get; set; }

    public DateTime? DateUpdate { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User? Staff { get; set; }

    public virtual User User { get; set; } = null!;
}
