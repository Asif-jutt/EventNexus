using System;
using System.Collections.Generic;

namespace EventNexus.Models;

public partial class Venue
{
    public int VenueId { get; set; }

    public string? Name { get; set; }

    public string? Location { get; set; }

    public int? Capacity { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
