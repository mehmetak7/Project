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

    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        [HttpPost("GetEvent")]
        public IActionResult GetEvent()
        {
            try
            {
                var today = DateTime.Today;

                var events = System.IO.File.ReadAllLines("Events.txt")
                    .Select(line =>
                    {
                        string[] values = line.Split(';');

                        if (values.Length < 5)
                            return null;

                        return new EventResponse
                        {
                            ID = Int32.Parse(values[0]),
                            EventName = values[1],
                            EventType = values[2],
                            EventDateTime = DateTime.Parse(values[3]),
                            EventNotes = values[4]
                        };

                    })
                    .Where(action => action != null && action.EventDateTime >= today)
                    .ToList();

                return Ok(events.OrderBy(e => e.EventDateTime).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error : {ex.Message}");
            }
        }

        [HttpPost("PostEvent")]
        public IActionResult PostEvent([FromBody] EventRequest eventRequest)
        {
            if (eventRequest == null)
            {
                return BadRequest("EventRequest is null.");
            }

            if (string.IsNullOrEmpty(eventRequest.EventName))
            {
                return BadRequest("EventName cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(eventRequest.EventType))
            {
                return BadRequest("EventType cannot be null or empty.");
            }

            if (eventRequest.EventDateTime == default(DateTime))
            {
                return BadRequest("MeetingDateTime is not provided or invalid.");
            }

            try
            {
                string newEvent = $"{eventRequest.ID};{eventRequest.EventName};{eventRequest.EventType};{eventRequest.EventDateTime};{eventRequest.EventNotes}";
                System.IO.File.AppendAllText("Events.txt", Environment.NewLine + newEvent);


                return Ok("Event basarili sekilde eklendi...");
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Error : {ex.Message}");
            }

        }

        [HttpPost("GetPreviousEvents")]
        public IActionResult GetPreviousEvent()
        {
            try
            {
                var today = DateTime.Today;

                var events = System.IO.File.ReadAllLines("Events.txt")
                    .Select(line =>
                    {
                        string[] values = line.Split(';');

                        if (values.Length < 5)
                            return null;

                        return new EventResponse
                        {
                            ID = Int32.Parse(values[0]),
                            EventName = values[1],
                            EventType = values[2],
                            EventDateTime = DateTime.Parse(values[3]),
                            EventNotes = values[4]
                        };

                    })
                    .Where(action => action != null && action.EventDateTime < today)
                    .ToList();

                return Ok(events.OrderBy(e => e.EventDateTime).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error : {ex.Message}");
            }
        }
        
        [HttpPost("PostDelete")]
        public IActionResult DeleteEvent([FromBody] DeleteEventRequest deleteRequest)
        {
            int removeWithID = deleteRequest.DeleteID;
            try
            {
                var lines = System.IO.File.ReadAllLines("Events.txt").ToList();
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

                System.IO.File.WriteAllLines("Events.txt", newLines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
            return Ok("Deleted With Successfully...");
        }

    }
}

