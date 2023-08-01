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

        public bool IsBirthdayToday(string filepath, int InputSicilNo)
        {
            string todayDate = DateTime.Now.ToString("dd.MM");

            if (InputSicilNo <= 0) // Sicil numarası geçerli bir değer olmalıdır.
                return false;

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

                    return birthday == todayDate;
                }

                return false;
            });

            return birthdayLine != null;
        }
          public int findLastId(string filepath)
        {
            string[] data = System.IO.File.ReadAllLines(filepath);

            if (data == null || data.Length == 0)
                throw new ArgumentException("Veri listesi boş veya null olamaz.");

            for (int i = data.Length - 1; i >= 0; i--)
            {
                if (!string.IsNullOrWhiteSpace(data[i]))
                {
                    string lastRow = data[i];
                    string[] lastRowFields = lastRow.Split(';');

                    if (lastRowFields.Length >= 1 && int.TryParse(lastRowFields[0], out int id))
                    {
                        return id;
                    }
                    else
                    {
                        throw new ArgumentException("Son satırda geçerli bir ID bulunamadı.");
                    }
                }
            }

            throw new ArgumentException("Dosya içinde dolu bir satır bulunamadı.");
        }
        public bool isManager(int id)
        {
            if (id.ToString()[0].ToString() == "1")
            {
                return true;
            }
            return false;
        }
    }
}
