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
    public class LoginController : ControllerBase
    {

        public static Functions functions = new Functions();
        // POST: api/Login/login
        //[HttpPost("login")]
        [HttpPost]
        public ActionResult Login([FromBody] LoginRequest loginRequest)
        {
            
            string logins = functions.LoadLoginsFromFile("Users.txt", loginRequest.SicilNo);
            int? SicilNo;
            string Sifre;

            //mevcut listede kullanıcıyı bul...
            //var user = logins.FirstOrDefault(l => l.SicilNo == loginRequest.SicilNo);
            //kullanıcı yok veya sifre eslesmiyorsa...
            if (!string.IsNullOrEmpty(logins))
            {
                string[] parts = logins.Split(';');
                SicilNo = int.Parse(parts[0]);
                Sifre = parts[1];

                if (SicilNo == null || Sifre != loginRequest.Sifre)
                {
                    var response = new LoginResponse
                    {
                        IsSuccessfull = false,
                        Message = "Hatalı sicil numarası veya sifre."

                    };
                    return BadRequest(response); //400 Bad Request döner..
                }
                var successResponse = new LoginResponse
                {
                    IsSuccessfull = true,
                    Message = "Giriş basarili"
                };
                return Ok(successResponse);

            }
            else
            {
                var response = new LoginResponse
                {
                    IsSuccessfull = false,
                    Message = "Hatalı sicil numarası veya sifre."

                };
                return BadRequest(response); //Kullanıcı Null dönerse...
            }
            
        }
    }

}

