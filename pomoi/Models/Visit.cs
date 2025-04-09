using System;
using System.Collections.Generic;

namespace pomoi.Models;

public partial class Visit
{
    public int VisitId { get; set; }

    public int? ClientId { get; set; }

    public DateTime VisitDate { get; set; }

    public virtual Client? Client { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
