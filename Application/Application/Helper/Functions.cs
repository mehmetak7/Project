using System;
using Application.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Application.Helper
{
    public class Functions
    {
    public string LoadLoginsFromFile(string filePath, int InputSicilNo)
    {
            var login = string.Empty;
            try
        {
            string[] lines = System.IO.File.ReadAllLines(filePath);

            // LINQ sorgusu ile gerekli satırı buluyoruz.
             login = lines.FirstOrDefault(line =>
            {
                string[] parts = line.Split(';');
                return int.TryParse(parts[0], out int sicilNo) && sicilNo == InputSicilNo;
            });

            return login;
        }
        catch
        {
            // Hata durumunda boş login = null döner...
            return login;
        }
    }

       
        /*
        public bool LoadBDay(string filepath, int InputSicilNo)
        {
            var BDay = string.Empty;
            bool check = false;
            
            string[] lines = System.IO.File.ReadAllLines(filepath);

        
            var Bday = lines.FirstOrDefault(line =>
            {
                string[] parts = line.Split(";");
                string todayDate = DateTime.Now.ToString();
                string[] today = todayDate.Split(".");
                string date = parts[parts.Length - 1];
                string[] dateBD = date.Split("/");
                if (string.Compare(dateBD[0], today[0]) == string.Compare(today[1], dateBD[1]))
                {
                        check = true;
                }
                else
                    check = false;

                return (int.TryParse(parts[0], out int sicilNo) && sicilNo == InputSicilNo) && (string.Compare(dateBD[0], today[0]) == string.Compare(today[1], dateBD[1]));
            });
            

            return check;
            

        }
        //Burada hata var date'ler tutuyorsa da liste dönüyor her türlü false dönüyor sicilNo kontrolü yani sadece
        //o kişinin doğuum günü mü değil mi o yapılmıyor... asagidaki fonksiyon sadece login de girilen kişinin doğum günü mü
        //değil mi o yapılıyor...
        */
        public bool IsBirthdayToday(string filepath, int InputSicilNo)
        {
            string todayDate = DateTime.Now.ToString("dd/MM");
            bool isBirthdayToday = false;
            int? test = InputSicilNo;

            if (InputSicilNo != null)
            {
                string[] lines = System.IO.File.ReadAllLines(filepath);

                var birthdayLine = lines.FirstOrDefault(line =>
                {
                    string[] parts = line.Split(";");

                    int sicilNo;
                    string date = parts[parts.Length - 1];

                    if (int.TryParse(parts[0], out sicilNo) && sicilNo == InputSicilNo)
                    {
                        string[] dateBD = date.Split("/");
                        string birthday = dateBD[0] + "." + dateBD[1];

                        if (birthday == todayDate)
                        {
                            isBirthdayToday = true;
                            return true;
                        }
                    }

                    return false;
                });
            }
            else
            {
                return false;
            }
            
            return isBirthdayToday;     
        }
    }
}

