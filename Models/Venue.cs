using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventNexus.Models;

public partial class Venue
{
    public int VenueId { get; set; }

    [Required(ErrorMessage = "Venue name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Venue name must be between 3 and 100 characters")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Location is required")]
    [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
    public string? Location { get; set; }

    [Required(ErrorMessage = "Capacity is required")]
    [Range(1, 100000, ErrorMessage = "Capacity must be greater than 0")]
    public int? Capacity { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}