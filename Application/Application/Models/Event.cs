using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Models
{

    public class EventRequest
    {
        public int ID { get; set; }
        public string EventName { get; set; }
        public string EventType { get; set; }
        public DateTime EventDateTime { get; set; }
        public string EventNotes { get; set; }
        
    }
    public class EventResponse
    {
        // public bool IsSuccessfull { get; set; }
        // public string Message { get; set; }
        public int ID { get; set; }
        public string EventName { get; set; }
        public string EventType { get; set; }
        public DateTime EventDateTime { get; set; }
        public string EventNotes { get; set; }
        
    }
    public class DeleteEventRequest
    {
        public int DeleteID { get; set; }
    }
}
