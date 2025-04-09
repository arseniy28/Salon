using System;
using System.Collections.Generic;

namespace pomoi.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
