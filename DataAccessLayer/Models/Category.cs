using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? Datecreate { get; set; }

    public DateTime? Dateupdate { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Upload> Uploads { get; set; } = new List<Upload>();
}
