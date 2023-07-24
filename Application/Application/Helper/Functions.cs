using System;
using Application.Models;
using System.Collections.Generic;
using System.Linq;

namespace Application.Helper
{
    public class Functions
    {
        public Functions()
        {
        }

        //Listeyi okuma yapar Login için return list yapar...(LoginController'da kullanıyoruz...)
        /* public string LoadLoginsFromFile(string filePath,int InputSicilNo)
        {
            string login = null;

            try
            {
                string[] lines = System.IO.File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    string[] parts = line.Split(';');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int sicilNo))
                    {
                        if (InputSicilNo == sicilNo)
                        {
                            login = line;
                            break;
                        }
                    }
                 
                }
            }
            catch
            {
                //Hata durumunda boş login = null döner...
            }

            return login;
        }
        */

        //test

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
                return parts.Length == 2 && int.TryParse(parts[0], out int sicilNo) && sicilNo == InputSicilNo;
            });

            return login;
        }
        catch
        {
            // Hata durumunda boş login = null döner...
            return login;
        }
    }
}
}

