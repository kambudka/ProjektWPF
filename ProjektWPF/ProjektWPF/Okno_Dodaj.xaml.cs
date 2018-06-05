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
using System.Security.Permissions;

namespace ProjektWPF
{
    [PrincipalPermission(SecurityAction.Demand, Role = "Administrators")]
    public partial class Okno_Dodaj : Window, IView
    {
        public Okno_Dodaj()
        {
            InitializeComponent();
        }
        #region IView Members
        public IViewModel ViewModel
        {
            get
            {
                return DataContext as IViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
        #endregion
        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public Paczka NowaPaczka;
        private void Ok_Click(object sender, RoutedEventArgs e)
        { 
            NowaPaczka = new Paczka(Decimal.Parse(Cena.Text), Adres.Text, Decimal.Parse(Waga.Text));
            DialogResult = true;
            Close();
        }

    }
}
