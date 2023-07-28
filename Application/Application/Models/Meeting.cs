using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Models
{

    public class MeetingRequest
    {
        public int ID { get; set; }
        public string TeamName { get; set; }
        public string MeetingName { get; set; }
        public DateTime MeetingDate { get; set; }
        public DateTime MeetingTime { get; set; }
        public string MeetingContext { get; set; }
        public string MeetingContent { get; set; }
    }
    public class MeetingResponse
    {
        public int ID { get; set; }
        public bool IsMeeting { get; set; }
        public string TeamName { get; set; }
        public string MeetingName { get; set; }
        public DateTime MeetingDate { get; set; }
        public DateTime MeetingTime { get; set; }
        public string MeetingContext { get; set; }
        public string MeetingContent { get; set; }
    }

    public class DeleteMeetingRequest
    {
        public int DeleteID { get; set; }
    }

}
