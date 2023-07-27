using Application.Helper;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;


namespace Application.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BDayController : Controller
    {
        string filePath = "Users.txt";
        [HttpPost("GetAllBDay")]
        public ActionResult GetAllBDay()
        {
            try
            {
                string todayDate = DateTime.Now.ToString("dd.MM.yyyy");
                var Bdays = System.IO.File.ReadAllLines(filePath)
                    .Select(line =>
                    {
                        string[] values = line.Split(';');
                        if (values.Length >= 5) 
                        {
                            string date = values[4];
                            string[] dateBD = date.Split("/");
                            return new BDayResponse
                            {
                                Name = values[2],
                                Surname = values[3],
                                BirthDay = values[4]
                            };
                        }
                        return null;
                    })
                   .Where(bdayResponse =>
                   {
                       int compareDateResult = string.Compare(todayDate.Split(".")[0], bdayResponse.BirthDay.Split('/')[0]);
                       int compareTimeResult = string.Compare(todayDate.Split(".")[1], bdayResponse.BirthDay.Split("/")[1]);

                       return compareDateResult == 0 && compareTimeResult == 0;
                   })
                    .ToList();

                return Ok(Bdays);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }


        }
    }
}
