using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ProjektWPF
{
    public class Paczka
    {
        private static readonly Random Random = new Random();
        private static int GenerateRandomNumber()
        {
            return Random.Next(3000) + 1000;
        }
        public Paczka(decimal cena, string adres, string waga) //Konstruktor użytkowy, status i numer jest generowany
        {
            Cena = cena;
            Adres = adres;
            Waga = waga;
            Status = "W magazynie"; //Po zatwierdzeniu paczki, składowana jest w magazynie
            Numer = GenerateRandomNumber(); //Zostaje nadany Randomowy dla numer paczki
            ImagePath = Status + ".png";
        }
        public Paczka(decimal cena, string adres, string waga, string status) //Konstruktor do przypisywania wszystkiego ręcznie(oprócz numeru)
        {
            Cena = cena;
            Adres = adres;
            Waga = waga;
            Status = status; //Po zatwierdzeniu paczki, składowana jest w magazynie
            Numer = GenerateRandomNumber(); //Zostaje nadany Randomowy dla numer paczki
            ImagePath = Status + ".png";
        }
        public string ImagePath { get; set; }
        public int Numer {get; set;}
        public decimal Cena { get; set; }
        public string Adres { get; set; }
        public string Waga { get; set; }
        public string Status { get; set; }

        public string Error
        {
            get { return null; }
        }

        public string this[string columnname]
        {
            get
            {
                if (columnname == "Waga")
                {
                    if (Waga == "5 kg")
                        return "Waga nie moze być większa niż 5 kg.";
                }
                return null;
            }
        }
    }
}
