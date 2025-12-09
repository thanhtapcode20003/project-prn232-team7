using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Upload
{
    public int Id { get; set; }

    public string? Content { get; set; }

    public int IdItem { get; set; }

    public int? Status { get; set; }

    public int? StaffId { get; set; }

    public DateTime? DateCreate { get; set; }

    public int? UserId { get; set; }

    public int? StatusAccept { get; set; }

    public int? StaffIdAccept { get; set; }

    public DateTime? DateAccept { get; set; }

    public string? Note { get; set; }

    public virtual Item IdItemNavigation { get; set; } = null!;

    public virtual User? Staff { get; set; }

    public virtual User? StaffIdAcceptNavigation { get; set; }

    public virtual User? User { get; set; }
}
