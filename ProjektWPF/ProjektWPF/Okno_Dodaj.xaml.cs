using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjektWPF
{
    /// <summary>
    /// Interaction logic for Okno_Dodaj.xaml
    /// </summary>
    public partial class Okno_Dodaj : Window
    {
        public Okno_Dodaj()
        {
            InitializeComponent();
        }
        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            public Paczka NowaPaczka = new Paczka(46.90M, "Jurowiecka 31", 4, "Zwrócona");
            NowaPaczka.Adres = Adres.Text;
            NowaPaczka.Waga = Waga.Text;
            NowaPaczka.Cena = Cena.Text;
            DialogResult = true;
            Close();
            //  Close();
        }
    }
}
