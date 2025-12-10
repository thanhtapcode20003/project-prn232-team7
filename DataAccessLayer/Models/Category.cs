using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Category
{
    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
