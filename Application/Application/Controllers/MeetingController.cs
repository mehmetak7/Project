using Application.Helper;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Application.Controllers
{


    [Route("[controller]")]
    [ApiController]
    public class MeetingController : ControllerBase
    {
        string filePath = "Meeting.txt";

        [HttpGet]
        public IActionResult GetMeeting()
        {
            try
            {
                var meetings = System.IO.File.ReadAllLines(filePath)
                    .Select(line =>
                    {
                        string[] values = line.Split(';');

                        if (values.Length < 6)
                            return null;

                        return new MeetingResponse
                        {
                            IsMeeting = true,
                            TeamName = values[0],
                            MeetingName = values[1],
                            MeetingDate = DateTime.Parse(values[2]),
                            MeetingTime = DateTime.Parse(values[3]),
                            MeetingContext = values[4],
                            MeetingContent = values[5]
                        };
                    })
                    .Where(meeting => meeting != null)
                    .ToList();

                return Ok(meetings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult AddMeeting([FromBody] MeetingRequest meeting)
        {
            try
            {
                string newMeeting = $"{meeting.TeamName};{meeting.MeetingName};{meeting.MeetingDate};{meeting.MeetingTime};{meeting.MeetingContext};{meeting.MeetingContent}";
                System.IO.File.AppendAllText(filePath, newMeeting + Environment.NewLine); //environment newlin = /n anlamýnda

                // Baþarýlý yanýt dönelim.
                return Ok("Toplantý eklendi...");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hata: {ex.Message}");
            }
        }


    }

}
