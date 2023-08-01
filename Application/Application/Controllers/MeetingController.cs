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
            //null deger add'de kabul edilmiyor bu tüzden de null olan deger txt'de var ise de okuma yapmaz.
            //o satırı okuyup ekrana göndermez...
            try
            {
                var today = DateTime.Today;

                var meetings = System.IO.File.ReadAllLines(filePath)
                    .Select(line =>
                    {
                        string[] values = line.Split(';');

                        if (values.Length < 7)
                            return null;

                        int id;
                        if (!int.TryParse(values[0], out id))
                            return null;

                        string teamName = values[1].Trim();
                        if (string.IsNullOrEmpty(teamName))
                            return null;

                        string meetingName = values[2].Trim();
                        if (string.IsNullOrEmpty(meetingName))
                            return null;

                        DateTime meetingDate;
                        if (!DateTime.TryParse(values[3], out meetingDate))
                            return null;

                        DateTime meetingTime;
                        if (!DateTime.TryParse(values[4], out meetingTime))
                            return null;

                        string meetingContext = values[5].Trim();
                        if (string.IsNullOrEmpty(meetingContext))
                            return null;

                        string meetingContent = values[6].Trim();

                        return new MeetingResponse
                        {
                            Id = id,
                            TeamName = teamName,
                            MeetingName = meetingName,
                            MeetingDate = meetingDate,
                            MeetingTime = meetingTime,
                            MeetingContext = meetingContext,
                            MeetingContent = meetingContent
                        };
                    })
                    .Where(meeting => meeting != null && meeting.MeetingDate >= today)
                    .OrderByDescending(meeting => meeting.MeetingDate)
                    .ThenByDescending(meeting => meeting.MeetingTime)
                    .ToList();

                return Ok(meetings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost("AddMeeting")]
        public IActionResult AddMeeting([FromBody] MeetingRequest meeting)
        {
            //Sadece şöyle bir durum var date null geldiğinde hata veriyor istediğim return yapmıyor.
            if (meeting == null)
            {
                return BadRequest("EventRequest is null.");
            }

            string[] properties = { meeting.Id.ToString(), meeting.TeamName, meeting.MeetingName, meeting.MeetingDate.ToString(), meeting.MeetingTime.ToString(), meeting.MeetingContext};
            string[] propertyNames = { "Meeting id", "Meeting TeamName", "Meeting name", "Meetingdate", "MeetingTime", "MeetingContent"};

            for (int i = 0; i < properties.Length; i++)
            {
                if (string.IsNullOrEmpty(properties[i]))
                {
                    return BadRequest($"{propertyNames[i]} cannot be null or empty.");
                }
            }

            try
            {
                string newMeeting = $"{meeting.Id};{meeting.TeamName};{meeting.MeetingName};{meeting.MeetingDate};{meeting.MeetingTime};{meeting.MeetingContext};{meeting.MeetingContent}";
                System.IO.File.AppendAllText(filePath, newMeeting + Environment.NewLine);

                return Ok("Toplanti eklendi...");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hata: {ex.Message}");
            }
        }

        [HttpPost("UpdateMeeting")]
        public IActionResult UpdateMeeting(MeetingRequest meeting)
        {
            try
            {
                // Null veya boşluk kontrolü yap
                if (meeting == null)
                {
                    return BadRequest("Toplantı bilgileri boş olamaz.");
                }

                // Diğer tüm değerler için null veya boşluk kontrolü yap
                if (string.IsNullOrEmpty(meeting.TeamName) ||
                    string.IsNullOrEmpty(meeting.MeetingName) ||
                    string.IsNullOrEmpty(meeting.MeetingContext) ||
                    string.IsNullOrEmpty(meeting.MeetingContent))
                {
                    return BadRequest("Tüm alanlar doldurulmalıdır.");
                }

                string[] meetings = System.IO.File.ReadAllLines(filePath);
                string updateMeeting = $"{meeting.Id};{meeting.TeamName};{meeting.MeetingName};{meeting.MeetingDate};{meeting.MeetingTime};{meeting.MeetingContext};{meeting.MeetingContent}";

                int index = Array.FindIndex(meetings, m => int.TryParse(m.Split(';')[0], out int id) && id == meeting.Id);
                if (index >= 0)
                {
                    meetings[index] = updateMeeting;
                    System.IO.File.WriteAllLines(filePath, meetings);
                    return Ok("Toplanti guncellendi...");
                }
                else
                {
                    return NotFound("Toplanti bulunamadi...");
                }
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
                            Id = Int32.Parse(values[0]),
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

        [HttpPost("DeleteMeeting")]
        public IActionResult DeleteEvent([FromBody] DeleteMeetingRequest deleteRequest)
        {
            // null gelirse lütfen doldurun mesajı olmayan bir id gelirse de böyle bir id yok gibi bir mesaj dönüyorum ve program calısıyor..
            //verilen id degerini de basariyla siliyor...
            int removeWithID = deleteRequest?.DeleteId ?? 0;

            if (removeWithID == 0)
            {
                return BadRequest("Please fill the ID area..");
            }
            try
            {
                var lines = System.IO.File.ReadAllLines(filePath).ToList();
                var newLines = new List<string>();

                bool isFound = false;
                foreach (var line in lines)
                {
                    string[] values = line.Split(';');

                    if (values.Length >= 4 && int.TryParse(values[0], out int eventId) && eventId == removeWithID)
                    {
                        isFound = true;
                        continue;
                    }

                    newLines.Add(line);
                }
                if (!isFound)
                {
                    return NotFound("Not found the given ID's line.");
                }
                System.IO.File.WriteAllLines(filePath, newLines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
            return Ok("Deleted With Successfully...");
        }

     [HttpPost("SearchMeeting")]
        public IActionResult SearchMeeting(SearchRequest searchRequest)
        {
            try
            {
                string[] meetings = System.IO.File.ReadAllLines(filePath);

                // Ekip adı ve toplantı türü aramak için verileri alalım
                string searchTeamName = searchRequest.TeamName;
                string searchMeetingName = searchRequest.MeetingName;

                for (int i = 0; i < meetings.Length; i++)
                {
                    string meetingText = meetings[i];
                    string[] values = meetingText.Split(';');

                    if (values.Length >= 2)
                    {
                        // Ekip adı veya toplantı türüne göre arama yapalım
                        if ((string.IsNullOrEmpty(searchTeamName) || values[1].Contains(searchTeamName)) ||
                            (string.IsNullOrEmpty(searchMeetingName) || values[2].Contains(searchMeetingName)))
                        {
                            // Toplantı verisini oluşturup geri dönelim
                            MeetingResponse foundMeeting = new MeetingResponse
                            {
                                Id = Int32.Parse(values[0]),
                                IsMeeting = true,
                                TeamName = values[1],
                                MeetingName = values[2],
                                MeetingDate = DateTime.Parse(values[3]),
                                MeetingTime = DateTime.Parse(values[4]),
                                MeetingContext = values[5],
                                MeetingContent = values[6]
                            };

                            return Ok(foundMeeting);
                        }
                    }
                }

                // Toplantı bulunamazsa NotFound döndürelim
                return NotFound("Aranan toplantı bulunamadı.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hata: {ex.Message}");
            }
        }
    }
}
