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
        public Paczka(decimal cena, string adres, decimal waga) //Konstruktor użytkowy, status i numer jest generowany
        {
            Cena = cena;
            Adres = adres;
            Waga = waga;
            Wagaadd = 'k' + Waga.ToString();
            Cenaadd = "p" + Cena.ToString();
            Status = "W magazynie"; //Po zatwierdzeniu paczki, składowana jest w magazynie
            ImagePath = Status + ".jpg";
        }
        public Paczka(decimal cena, string adres, decimal waga, string status, int numer, string kurier) //Konstruktor do przypisywania wszystkiego ręcznie(oprócz numeru)
        {
            Cena = cena;
            Cenaadd = "p" + Cena.ToString();
            Adres = adres;
            Waga = waga;
            Wagaadd = 'k' + Waga.ToString();
            Status = status; //Po zatwierdzeniu paczki, składowana jest w magazynie
            Numer = numer; 
            ImagePath = Status + ".jpg";
            Kurier = kurier;
        }
        public string ImagePath { get; set; }
        public int Numer {get; set;}
        public decimal Cena { get; set; }
        public string Adres { get; set; }
        public decimal Waga { get; set; }
        public string Status { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Kurier { get; set; }
        public string Info { get; set; }
        public string Cenaadd { get; set; }
        public string Wagaadd { get; set; }


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
                    if (Waga == 5)
                        return "Waga nie moze być większa niż 5 kg.";
                }
                return null;
            }
        }
    }
}
