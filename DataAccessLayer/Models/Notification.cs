using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Notification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string Type { get; set; } = null!;

    public Guid? ReferenceId { get; set; }

    public bool IsRead { get; set; }

    public DateTime DateCreated { get; set; }

    public virtual User User { get; set; } = null!;
}
