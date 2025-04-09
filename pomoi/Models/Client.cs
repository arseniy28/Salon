using System;
using System.Collections.Generic;

namespace pomoi.Models;

public partial class Client
{
    public int ClientId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public DateOnly? BirthDate { get; set; }

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
