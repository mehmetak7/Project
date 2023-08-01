namespace Application.Models
{
    public class Search
    {
        public class SearchRequest
        {
            public int Id { get; set; }
            public string TeamName { get; set; }
            public string MeetingName { get; set; }
        }
        public class SearchEventRequest
        {
            public int Id { get; set; }
            public string EventName { get; set; }
            public string EventType { get; set; }
        }
    }
}
