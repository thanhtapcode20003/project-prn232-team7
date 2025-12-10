using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class ReturnRecord
{
    public Guid ReturnId { get; set; }

    public Guid ItemId { get; set; }

    public Guid FoundUserId { get; set; }

    public Guid? ReceiverUserId { get; set; }

    public DateTime ReturnDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual User FoundUser { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;

    public virtual User? ReceiverUser { get; set; }
}
