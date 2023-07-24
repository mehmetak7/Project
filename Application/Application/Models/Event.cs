using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Models
{

    public class EventRequest
    {
        public string EventName { get; set; }
        public string EventType { get; set; }
        public DateTime MeatingDateTime { get; set; }
        public string MeetingNotes { get; set; }
    }
    public class EventResponse
    {
       // public bool IsSuccessfull { get; set; }
       // public string Message { get; set; }
        public string EventName { get; set; }
        public string EventType { get; set; }
        public DateTime MeatingDateTime { get; set; }
        public string MeetingNotes { get; set; }

    }


}
