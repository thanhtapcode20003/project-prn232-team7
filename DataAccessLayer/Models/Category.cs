using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Category
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int Status { get; set; }

    public DateTime? Datecreate { get; set; }

    public DateTime? Dateupdate { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
