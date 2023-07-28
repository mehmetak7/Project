using Application.Helper;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Application.Controllers
{


    [Route("[controller]")]
    [ApiController]
    public class MeetingController : ControllerBase
    {
        string filePath = "Meeting.txt";

        [HttpPost("GetMeeting")]
        public IActionResult GetMeeting()
        {
            try
            {
                var today = DateTime.Today;

                var meetings = System.IO.File.ReadAllLines(filePath)
                    .Select(line =>
                    {
                        string[] values = line.Split(';');

                        if (values.Length < 7)
                            return null;

                        return new MeetingResponse
                        {
                            ID = Int32.Parse(values[0]),
                            IsMeeting = true,
                            TeamName = values[1],
                            MeetingName = values[2],
                            MeetingDate = DateTime.Parse(values[3]),
                            MeetingTime = DateTime.Parse(values[4]),
                            MeetingContext = values[5],
                            MeetingContent = values[6]
                        };
                    })
                    .Where(meeting => meeting != null && meeting.MeetingDate >= today)
                    .ToList();

                return Ok(meetings.OrderBy(meeting => meeting.MeetingDate).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost("AddMeeting")]
        public IActionResult AddMeeting([FromBody] MeetingRequest meeting)
        {
            try
            {
                string newMeeting = $"{meeting.ID};{meeting.TeamName};{meeting.MeetingName};{meeting.MeetingDate};{meeting.MeetingTime};{meeting.MeetingContext};{meeting.MeetingContent}";
                System.IO.File.AppendAllText(filePath, newMeeting + Environment.NewLine); //environment newlin = /n anlamýnda

                // Baþarýlý yanýt dönelim.
                return Ok("Toplanti eklendi...");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hata: {ex.Message}");
            }
        }


            [HttpPost("GetPreviousMeetings")]
            public IActionResult GetPreviousMeeting()
            {
                try
                {
                    var today = DateTime.Today;

                    var meetings = System.IO.File.ReadAllLines(filePath)
                        .Select(line =>
                        {
                            string[] values = line.Split(';');

                            if (values.Length < 6)
                                return null;

                            return new MeetingResponse
                            {
                                ID = Int32.Parse(values[0]),
                                IsMeeting = true,
                                TeamName = values[1],
                                MeetingName = values[2],
                                MeetingDate = DateTime.Parse(values[3]),
                                MeetingTime = DateTime.Parse(values[4]),
                                MeetingContext = values[5],
                                MeetingContent = values[6]
                            };
                        })
                        .Where(meeting => meeting != null && meeting.MeetingDate < today)
                        .ToList();

                    return Ok(meetings.OrderBy(meeting => meeting.MeetingDate).ToList());
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error: {ex.Message}");
                }
            }

        [HttpPost("PostDelete")]
        public IActionResult DeleteEvent([FromBody] DeleteMeetingRequest deleteRequest)
        {
            int removeWithID = deleteRequest.DeleteID;
            try
            {
                var lines = System.IO.File.ReadAllLines("Meeting.txt").ToList();
                var newLines = new List<string>();

                foreach (var line in lines)
                {
                    string[] values = line.Split(';');

                    if (values.Length >= 4 && int.TryParse(values[0], out int eventId) && eventId == removeWithID)
                    {
                        continue;
                    }

                    newLines.Add(line);
                }

                System.IO.File.WriteAllLines("Meeting.txt", newLines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
            return Ok("Deleted With Successfully...");
        }


    }

}
