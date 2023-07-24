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
        public static Functions functions = new Functions();
        [HttpGet]
        public IActionResult GetEvent()
        {
            try
            {
                var events = System.IO.File.ReadAllLines("Events.txt")
                    .Select(line =>
                    {
                        string[] values = line.Split(';');

                        if (values.Length < 4)
                            return null;

                        return new EventResponse
                        {
                            // IsSuccessfull = true,
                            // Message = "Basarili",
                            EventName = values[0],
                            EventType = values[1],
                            MeatingDateTime = DateTime.Parse(values[2]),
                            MeetingNotes = values[3]
                        };

                    })
                    .Where(action => action != null)
                    .ToList();

                return Ok(events);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error : {ex.Message}");
            }
        }

        [HttpPost]
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

            if (eventRequest.MeatingDateTime == default(DateTime))
            {
                return BadRequest("MeetingDateTime is not provided or invalid.");
            }

            try
            {
                string newEvent = $"{eventRequest.EventName};{eventRequest.EventType};{eventRequest.MeatingDateTime};{eventRequest.MeetingNotes}";
                System.IO.File.AppendAllText("Events.txt", Environment.NewLine + newEvent);


                return Ok("Event basarili sekilde eklendi...");
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Error : {ex.Message}");
            }
           


        }
    }
}

