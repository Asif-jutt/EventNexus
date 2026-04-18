using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventNexus.Models;

public partial class Event
{
    public int EventId { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Event date is required")]
    [DataType(DataType.DateTime)]
    public DateTime? EventDate { get; set; }

    [Required(ErrorMessage = "Venue is required")]
    public int? VenueId { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [RegularExpression("^(Active|Draft|Cancelled)$", ErrorMessage = "Invalid status")]
    public string? Status { get; set; }

    public virtual Venue? Venue { get; set; }
}