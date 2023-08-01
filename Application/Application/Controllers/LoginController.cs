using Application.Helper;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace Application.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public static Functions functions = new Functions();
        [HttpPost("Login")]
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

                if (SicilNo == null || Sifre != loginRequest.Sifre || Sifre == null)
                {
                    var response = new LoginResponse
                    {
                        IsSuccessfull = false,
                        Message = "Hatalı sicil numarası veya sifre."

                    };
                    return BadRequest(response); //400 Bad Request döner..
                }
                else
                {
                    #region Doğum Günü Kontrolü
                    bool isBirthday = false;
                    isBirthday = functions.IsBirthdayToday("Users.txt", loginRequest.SicilNo);
                    #endregion

                    #region Yönetici Kontrol
                    bool isManager = false;
                    isManager = functions.isManager(loginRequest.SicilNo);
                    #endregion

                    var successResponse = new LoginResponse
                    {
                        IsManager = isManager,
                        IsSuccessfull = true,
                        Message = "Giriş basarili",
                        IsBirthday = isBirthday
                    };
                    return Ok(successResponse);
                }
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


