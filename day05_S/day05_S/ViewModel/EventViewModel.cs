using day05_S.Models;

namespace day05_S.ViewModel
{
    public class EventViewModel
    {
        public string UserId { get; set; } 
        public List<Event> Events { get; set; } = new List<Event>();
        public List<EventRegistraion> MyRegistrations { get; set; } = new List<EventRegistraion>();
    }
}
