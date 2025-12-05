using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class RefreshToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public bool? IsRevoked { get; set; }

    public DateTime DateCreated { get; set; }

    public virtual User User { get; set; } = null!;
}
