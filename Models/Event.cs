namespace EventNexus.Models
{
    
        public class Event
        {
            public int EventId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime EventDate { get; set; }
            public int VenueId { get; set; }
            public string Status { get; set; }
        }
    
}
