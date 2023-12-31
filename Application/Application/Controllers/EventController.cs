using Application.Helper;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using static Application.Models.Search;

namespace Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        readonly string filePath = "Events.txt";
        readonly Functions functions = new();

        [HttpPost("GetEvent")]
        public IActionResult GetEvent()
        {
            //null degerleri get yapmıyor ancak hata da vermiyor null'a denk geldiginde
            //null göstemriyor null olmayanları front'a gönderiyor... Null kontrolünü öyle yaptım.(Ak)
            try
            {
                var today = DateTime.Today;

                var events = System.IO.File.ReadAllLines(filePath)
                    .Select(line =>
                    {
                        string[] values = line.Split(';');

                        if (values.Length < 5)
                            return null;


                        if (!int.TryParse(values[0], out int id))
                            return null;

                        string eventName = values[1].Trim();
                        if (string.IsNullOrEmpty(eventName))
                            return null;

                        string eventType = values[2].Trim();
                        if (string.IsNullOrEmpty(eventType))
                            return null;

                        if (!DateTime.TryParse(values[3], out DateTime eventDateTime))
                            return null;

                        string eventNotes = values[4].Trim();

                        return new EventResponse
                        {
                            Id = id,
                            EventName = eventName,
                            EventType = eventType,
                            EventDateTime = eventDateTime,
                            EventNotes = eventNotes.Replace("|", "\n")
                        };
                    })
                    .Where(action => action != null && action.EventDateTime >= today)
                    .ToList();

                if (events.Count == 0)
                {
                    return NotFound("No events found.");
                }

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
            EventResponse response = new();
            //date null gelirse "" hata veriyor..
            if (eventRequest == null)
                return BadRequest("EventRequest is null.");

            string[] properties = { eventRequest.EventName, eventRequest.EventType, eventRequest.EventDateTime.ToString(), eventRequest.EventNotes };
            // string[] properties = { eventRequest.Id.ToString(), eventRequest.EventName, eventRequest.EventType, eventRequest.EventDateTime.ToString(), eventRequest.EventNotes };
            string[] propertyNames = { "EventName", "EventType", "EventDateTime", "EventNotes" };

            for (int i = 0; i < properties.Length; i++)
            {
                if (string.IsNullOrEmpty(properties[i]))
                    return BadRequest($"{propertyNames[i]} cannot be null or empty.");
            }

            try
            {
                string newEvent = $"{response.Id = functions.FindLastId(filePath) + 1}; {eventRequest.EventName};{eventRequest.EventType};{eventRequest.EventDateTime};{eventRequest.EventNotes.Replace("\n", "|")}";
                //string newEvent = $"{eventRequest.Id};{eventRequest.EventName};{eventRequest.EventType};{eventRequest.EventDateTime};{eventRequest.EventNotes}";
                System.IO.File.AppendAllText(filePath, Environment.NewLine + newEvent);

                return Ok("Event başarılı bir şekilde eklendi...");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost("UpdateEvent")]
        public IActionResult UpdateMeeting(EventRequest eventRequest)
        {
            //tarih dışında herhangibir alan boş olsa da kabul ediyor sadece tarih dolu gelsin digerleri null oalbilir...
            try
            {
                string[] events = System.IO.File.ReadAllLines(filePath);
                string updateMeeting = $"{eventRequest.Id};{eventRequest.EventName};{eventRequest.EventType};{eventRequest.EventDateTime};{eventRequest.EventNotes.Replace("\n", "|")}";

                int index = Array.FindIndex(events, m => int.TryParse(m.Split(';')[0], out int id) && id == eventRequest.Id);
                if (index >= 0)
                {
                    events[index] = updateMeeting;
                    System.IO.File.WriteAllLines(filePath, events);
                    return Ok("Etkinlik guncellendi...");
                }
                else
                {
                    return NotFound("Etkinlik bulunamadi...");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hata: {ex.Message}");
            }
        }

        [HttpPost("GetPreviousEvents")]
        public IActionResult GetPreviousEvent()
        {
            //tarihe göre karşılaştırıp öncekileri döndüğü için tarih dışında her alan null gelebilir.
            //tarih dışındaki alanlar null olsa bile ekrana önceki event olarak gösterilecektir....
            try
            {
                var today = DateTime.Today;

                var events = System.IO.File.ReadAllLines(filePath)
                    .Select(line =>
                    {
                        string[] values = line.Split(';');

                        if (values.Length < 5)
                            return null;

                        // Tarih dışında diğer alanlardan biri veya birkaçı null olsa bile eventi gösteriyoruz.
                        if (!int.TryParse(values[0], out int id))
                            id = 0;

                        string eventName = values[1]?.Trim() ?? string.Empty;
                        string eventType = values[2]?.Trim() ?? string.Empty;

                        if (!DateTime.TryParse(values[3], out DateTime eventDateTime))
                            eventDateTime = DateTime.MinValue;

                        string eventNotes = values[4]?.Trim() ?? string.Empty;

                        return new EventResponse
                        {
                            Id = id,
                            EventName = eventName,
                            EventType = eventType,
                            EventDateTime = eventDateTime,
                            EventNotes = eventNotes
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

        [HttpPost("DeleteEvent")]
        public IActionResult DeleteEvent([FromBody] DeleteEventRequest deleteRequest)
        {
            // null gelirse lütfen doldurun mesajı olmayan bir id gelirse de böyle bir id yok gibi bir mesaj dönüyorum ve program calısıyor..
            //verilen id degerini de basariyla siliyor...
            int removeWithID = deleteRequest?.DeleteId ?? 0;

            if (removeWithID == 0)
            {
                return BadRequest("Please fill the ID area...");
            }

            try
            {
                var lines = System.IO.File.ReadAllLines(filePath).ToList();
                var newLines = new List<string>();

                bool found = false;
                foreach (var line in lines)
                {
                    string[] values = line.Split(';');

                    if (values.Length >= 4 && int.TryParse(values[0], out int eventId) && eventId == removeWithID)
                    {
                        found = true;
                        continue;
                    }

                    newLines.Add(line);
                }

                if (!found)
                {
                    return NotFound("Not found the given ID's line.");
                }

                System.IO.File.WriteAllLines(filePath, newLines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
            return Ok("Deleted with successfully..");
        }

        [HttpPost("SearchEvent")]
        public IActionResult Search([FromBody] SearchEventRequest searchRequest)
        {
            try
            {
                string[] events = System.IO.File.ReadAllLines(filePath);

                string searchEventName = searchRequest.EventName;

                List<EventResponse> foundEvents = new List<EventResponse>();

                for (int i = 0; i < events.Length; i++)
                {
                    string eventText = events[i];
                    string[] values = eventText.Split(';');

                    if (values.Length >= 2)
                    {
                        if (string.IsNullOrEmpty(searchEventName) || values[1].Contains(searchEventName, StringComparison.OrdinalIgnoreCase))
                        {
                            EventResponse foundEvent = new EventResponse
                            {
                                Id = Int32.Parse(values[0]),
                                EventName = values[1],
                                EventType = values[2],
                                EventDateTime = DateTime.Parse(values[3]),
                                EventNotes = values[4]
                            };

                            foundEvents.Add(foundEvent);
                        }
                    }
                }

                if (foundEvents.Count > 0)
                {
                    return Ok(foundEvents);
                }

                return NotFound("Aranan etkinlik bulunamadı.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hata: {ex.Message}");
            }
        }
    }
}
