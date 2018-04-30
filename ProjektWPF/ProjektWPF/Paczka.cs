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
        public int Numer {get; set;}
        public decimal Cena { get; set; }
        public string Adres { get; set; }
        public int Waga { get; set; }
        public string Status { get; set; }

        public string this[string columnname]
        {
            get
            {
                if (columnname == "Waga")
                {
                    if (Waga >= 5)
                        return "Waga nie moze być większa niż 5 kg.";
                }
                return null;
            }
        }
    }
}
