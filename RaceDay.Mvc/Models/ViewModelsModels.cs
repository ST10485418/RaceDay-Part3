
namespace RaceDay.Mvc.Models
{
    public class EventViewModel
    {
        public int EventID { get; set; }

        public int OrganiserID { get; set; }

        public string EventName { get; set; }
            = string.Empty;

        public string Description { get; set; }
            = string.Empty;

        public DateTime EventDate { get; set; }

        public string Location { get; set; }
            = string.Empty;

        public decimal DistanceKm { get; set; }
        
        public string EventType { get; set; }
            = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
