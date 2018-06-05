using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using System.Drawing.Printing;
using System.Printing;
using System.Drawing;
using System.Windows.Xps.Packaging;
using System.IO;
using System.Windows.Xps;
using MahApps.Metro.Controls;
using System.Threading;
using System.Security;

namespace ProjektWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    //IView Model
    
    public interface IView
    {
        IViewModel ViewModel
        {
            get;
            set;
        }

        void Show();
    }
    public partial class MainWindow : Window, IView
    {
        
        public MainWindow(AuthenticationViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            paczka.Add(new Paczka(25.90M, "Zwierzyniecka 8", 5, "W magazynie", 5398,"kurier"));
            paczka.Add(new Paczka(59.90M, "Wiejska 50", 3, "W drodze", 6547, "kurier"));
            paczka.Add(new Paczka(34.90M, "Wesoła 16", 2, "Dostarczona", 3264, "kurier2"));
            paczka.Add(new Paczka(46.90M, "Jurowiecka 31", 4, "W magazynie",6742, "kurier"));
            paczka.Add(new Paczka(70.90M, "Sienkiewicza 31", 7, "W magazynie",3198, "kurier2"));
            paczka.Add(new Paczka(80.90M, "Lipowa 31", 4, "W drodze",2387, "kurier2"));
            paczka.Add(new Paczka(120.90M, "Pogodna 31", 4, "Dostarczona",3269, "kurier"));
            lista.ItemsSource = paczka; //Nadanie kilku początkowych paczek

            Drukarka.SelectedIndex = 0;
            KolorWydruku.Items.Add("Czarno-biały");
            KolorWydruku.Items.Add("Skala szarości");
            KolorWydruku.Items.Add("Kolorowy");
            KolorWydruku.SelectedIndex = 0;

        }

        #region IView Members
        public IViewModel ViewModel
        {
            get { return DataContext as IViewModel; }
            set { DataContext = value; }
        }
        #endregion

        int nr = 1;  //Numer faktury
        private List<Paczka> paczka = new List<Paczka>();

        Paczka znalezionaPaczka;  //Do wypelniania faktury
        Paczka statusPaczka; //Do statusu
        private bool handle = true; //Do obsługi wyglądu.
        int fakturaIloscPozycjiStalych = 0;  //Do aktualizacji Canvasa
        private static readonly Random Random = new Random();   //Do nadawania numeru dla nowej paczki.

        private ListCollectionView View
        {
            get
            {
                return (ListCollectionView)CollectionViewSource.GetDefaultView(paczka);
            }
        }

        //[Drukowanie] Funkcja generująca PDFa na podstawie Canvasa i danych z wybranej paczki.
        private void GenerujFakture()
        {
            if (fakturaIloscPozycjiStalych == 0) fakturaIloscPozycjiStalych = Faktura.Children.Count;  //pierwsze wejscie
            else
            {
                for(int i = Faktura.Children.Count-1; i >= fakturaIloscPozycjiStalych; i--)
                {
                    Faktura.Children.RemoveAt(i);
                }
            }
            int rozmiarCzcionki = 3;
            Label data = new Label();                 
            data.FontSize = rozmiarCzcionki;
            data.Content = DateTime.Today.ToString("d");
            Canvas.SetLeft(data, 120);
            Faktura.Children.Add(data);

            Label numerFaktury = new Label();
            numerFaktury.FontSize = rozmiarCzcionki;
            numerFaktury.Content = "nr " + nr.ToString() + "/" + DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString();
            Canvas.SetTop(numerFaktury, 50);
            Canvas.SetLeft(numerFaktury, 63);
            Faktura.Children.Add(numerFaktury);
            nr++;
            Label nabywca = new Label();
            nabywca.FontSize = rozmiarCzcionki;
            nabywca.Content = "Gal Anonim";   // rodo i takie tam 
            Canvas.SetTop(nabywca, 15);
            Canvas.SetLeft(nabywca, 80);
            Faktura.Children.Add(nabywca);

            Label adresNabywcy = new Label();
            adresNabywcy.FontSize = rozmiarCzcionki;
            adresNabywcy.Content = znalezionaPaczka.Adres;
            Canvas.SetTop(adresNabywcy, 20);
            Canvas.SetLeft(adresNabywcy, 80);
            Faktura.Children.Add(adresNabywcy);

            Label numerPaczki = new Label();
            numerPaczki.FontSize = rozmiarCzcionki;
            numerPaczki.Content = NumerPrzesylki.Text;
            Canvas.SetTop(numerPaczki, 70);
            Canvas.SetLeft(numerPaczki, 10);
            Faktura.Children.Add(numerPaczki);

            Label wagaPaczki = new Label();
            wagaPaczki.FontSize = rozmiarCzcionki;
            wagaPaczki.Content = znalezionaPaczka.Waga;
            Canvas.SetTop(wagaPaczki, 70);
            Canvas.SetLeft(wagaPaczki, 40);
            Faktura.Children.Add(wagaPaczki);

            Label netto = new Label();
            netto.FontSize = rozmiarCzcionki;
            netto.Content = znalezionaPaczka.Cena;
            Canvas.SetTop(netto, 70);
            Canvas.SetLeft(netto, 55);
            Faktura.Children.Add(netto);

            Label vat = new Label();
            vat.FontSize = rozmiarCzcionki;
            vat.Content = "23%";
            Canvas.SetTop(vat, 70);
            Canvas.SetLeft(vat, 70);
            Faktura.Children.Add(vat);

            Label vatKwota = new Label();
            decimal tempVat;
            tempVat = znalezionaPaczka.Cena*(decimal)0.23;
            vatKwota.FontSize = rozmiarCzcionki;
            vatKwota.Content = Decimal.Round(tempVat, 2).ToString();
            Canvas.SetTop(vatKwota, 70);
            Canvas.SetLeft(vatKwota, 90);
            Faktura.Children.Add(vatKwota);

            Label brutto = new Label();
            decimal tempBrutto;
            brutto.FontSize = rozmiarCzcionki;
            tempBrutto = tempVat + znalezionaPaczka.Cena;
            brutto.Content = Decimal.Round(tempBrutto, 2).ToString();
            Canvas.SetTop(brutto, 70);
            Canvas.SetLeft(brutto, 110);
            Faktura.Children.Add(brutto);

            Label razem = new Label();
            razem.FontSize = rozmiarCzcionki;
            razem.Content = Decimal.Round(tempBrutto, 2).ToString();
            Canvas.SetTop(razem, 85);
            Canvas.SetLeft(razem, 110);
            Faktura.Children.Add(razem);

            Label calosc = new Label();
            calosc.FontSize = rozmiarCzcionki;
            calosc.Content = Decimal.Round(tempBrutto, 2).ToString();
            Canvas.SetTop(calosc, 100);
            Canvas.SetLeft(calosc, 30);
            Faktura.Children.Add(calosc);
        }

        //[Paczki] Generowanie randomowego numeru dla nowej paczki.
        private static int GenerateRandomNumber()
        {
            return Random.Next(3000) + 1000;
        }

        //[Paczki] Obsługa zdarzenia Kliknięcia przycisku Dodaj Paczke. 
        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            Okno_Dodaj dialog = new Okno_Dodaj();
            if (dialog.ShowDialog() == true)
            {
                do
                {
                    dialog.NowaPaczka.Numer = GenerateRandomNumber(); //Zostaje nadany Randomowy dla numer paczki

                } while (paczka.Exists(element => element.Numer == dialog.NowaPaczka.Numer) == true);   //Sprawdzanie czy daney numer już istnieje na liscie.
                paczka.Add(dialog.NowaPaczka);
                View.Refresh();
            }
            else
            {
            }
        }

        //[Status] Obsługa zdarzenia zmiany tekstu w TextBoxie.
        private void Wyszukaj_TextChanged(object sender, RoutedEventArgs e)
        {
            string idpaczki;
            idpaczki = ID.Text;
            int temp;
            Wyszukaj.IsEnabled = false;
 
            if (ID.Text.Length == 4)    //Sprawdzenie poprawności długości wprowadzonego numeru.
            {
                bool cos = int.TryParse(ID.Text, out temp);
                if (cos == true)
                {
                    statusPaczka = paczka.Find(element => element.Numer == temp);
                    if (statusPaczka != null)   //Jeżeli odnaleziono paczkę w liscie można kliknąć w wyszukaj przez co wyswietli się jej status.
                    {
                        Wyszukaj.IsEnabled = true;
                    }
                }
            }
        }

        //[Status] Obsuga zdarzenia klikniecia Button Wyszukaj.
        private void Wyszukaj_Click(object sender, RoutedEventArgs e)
        {
            StatusPaczki.Text = statusPaczka.Status;
            if (StatusPaczki.Text == "Dostarczona")
                StatusPaczki.Foreground = System.Windows.Media.Brushes.DarkGreen;
            else if(StatusPaczki.Text == "W drodze")
                StatusPaczki.Foreground = System.Windows.Media.Brushes.Gold;
            else if (StatusPaczki.Text == "W magazynie")
                StatusPaczki.Foreground = System.Windows.Media.Brushes.Red;

            StatusPaczki.FontSize = 50;
            StatusPaczki.FontWeight = FontWeights.UltraBold;
        }

        //[Drukowanie] Osługa zdarzenia kliknięcia Button Drukuj.
        private void Drukuj_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            PrintServer ps = new PrintServer();
            PrintQueue pq = ps.GetPrintQueue(Drukarka.SelectedValue.ToString());
            pd.PrintQueue = pq;

            Faktura.LayoutTransform = new ScaleTransform(5, 5);

            int pageMargin = 3;
            System.Windows.Size pageSize = new System.Windows.Size(pd.PrintableAreaWidth - pageMargin * 2, pd.PrintableAreaHeight - pageMargin);
            Faktura.Measure(pageSize);
            Faktura.Arrange(new Rect(pageMargin, pageMargin, pageSize.Width, pageSize.Height));

            switch (KolorWydruku.SelectedIndex)
            {
                case 0:
                    pd.PrintQueue.DefaultPrintTicket.OutputColor = OutputColor.Monochrome;
                    break;
                case 1:
                    pd.PrintQueue.DefaultPrintTicket.OutputColor = OutputColor.Grayscale;
                    break;
                case 2:
                    pd.PrintQueue.DefaultPrintTicket.OutputColor = OutputColor.Color;
                    break;
            }
            int temp;
            bool cos = int.TryParse(IloscKopii.Text, out temp);
            if (IloscKopii.Text != "" && cos == true && temp > 1) pd.PrintQueue.DefaultPrintTicket.CopyCount = temp;
            
            pd.PrintTicket = pd.PrintQueue.DefaultPrintTicket;
            //pd.ShowDialog();
            string nazwaWydruku;
            nazwaWydruku = "Faktura" + NumerPrzesylki.Text;
            pd.PrintVisual(Faktura, nazwaWydruku);
            Faktura.LayoutTransform = null;
        }

        //[Drukowanie] Osługa zdarzenia zwiększenia liczby kopi faktury do wydrukowania.
        private void WiecejKopii_Click(object sender, RoutedEventArgs e)
        {
            int temp;
            bool cos = int.TryParse(IloscKopii.Text, out temp);
            if ((IloscKopii.Text != "") && (cos == true) && (temp < 10)) temp++;
            IloscKopii.Text = temp.ToString();
        }

        //[Drukowanie] Osługa zdarzenia zmniejszenia liczby kopi faktury do wydrukowania.
        private void MniejKopii_Click(object sender, RoutedEventArgs e)
        {
            int temp;
            bool cos = int.TryParse(IloscKopii.Text, out temp);
            if ((IloscKopii.Text != "") && (cos == true) && (temp > 1)) temp--;
            IloscKopii.Text = temp.ToString();
        }

        //[Drukowanie] Osługa zdarzenia zmiany text w TextBoxie wczytującego numer przesyłki.
        private void NumerPrzesylki_TextChanged(object sender, TextChangedEventArgs e)
        {
            int temp;
            Drukuj.IsEnabled = false;
            if(NumerPrzesylki.Text.Length == 4)
            {
                bool cos = int.TryParse(NumerPrzesylki.Text, out temp);
                if (cos == true)
                {
                    znalezionaPaczka = paczka.Find(element => element.Numer == temp);
                    if (znalezionaPaczka != null)
                    {
                        GenerujFakture();
                        Drukuj.IsEnabled = true;
                    }
                }
            }
            
        }

        //Obsługa zdarzenia zmiany wartości w Comboboxie.
        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (handle) Handle();
            handle = true;
        }

        //Obsługa zdarzenia zmiany wartości w Comboboxie.
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            handle = !cmb.IsDropDownOpen;
            Handle();
        }

        //Klasa potrzeba do wczytywania zasobów wykorzystywanych w stylach.
        public class ThemeResourceDictionary : ResourceDictionary
        {
        }

        //Funkcja dodająca źródła dla stylów Metro.
        private void AddMetro() 
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml", UriKind.RelativeOrAbsolute) });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml", UriKind.RelativeOrAbsolute) });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml", UriKind.RelativeOrAbsolute) });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml", UriKind.RelativeOrAbsolute) });
        }

        //Obsługa Comboboxa z listą wzorów wyglądu.
        private void Handle()
        {
            int ile = Application.Current.Resources.MergedDictionaries.Count;
            switch (cmbThemes.SelectedIndex)
            {
                //Usuwanie wszystkich źródeł, przywracanie wyglądu standardowego.
                case 0: 
                    Application.Current.Resources.MergedDictionaries.Clear();
                    break;
                //Wczytanie wzoru Metro Light
                case 1:
                    if(ile == 0)
                    {
                        AddMetro();
                        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml", UriKind.RelativeOrAbsolute) });
                    }
                    else
                    Application.Current.Resources.MergedDictionaries[4].Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml", UriKind.RelativeOrAbsolute);
                    break;
                //Wczytanie wzoru Metro Dark
                case 2:
                    if (ile == 0)
                    {
                        AddMetro();
                        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml", UriKind.RelativeOrAbsolute) });
                    }
                    else
                        Application.Current.Resources.MergedDictionaries[4].Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml", UriKind.RelativeOrAbsolute);
                    break;
            }
        }


        private void WagaKg_Checked(object sender, RoutedEventArgs e)
        {
            int ile = paczka.Count;
            for(int i=0;i<ile;i++)
            {
                if(paczka[i].Wagaadd[0]=='l')
                {
                    double a = Convert.ToDouble(paczka[i].Waga);
                    a *= 0.45359237;
                    paczka[i].Waga = Convert.ToDecimal(a);
                    paczka[i].Wagaadd = 'k' + paczka[i].Waga.ToString();
                }
            }
        
        }
        private void WagaLbs_Checked(object sender, RoutedEventArgs e)
        {
            int ile = paczka.Count;
            for (int i = 0; i < ile; i++)
            {
                if (paczka[i].Wagaadd[0] == 'k')
                {
                    double a = Convert.ToDouble(paczka[i].Waga);
                    a *= 2.20462262;
                    paczka[i].Waga = Convert.ToDecimal(a);
                    paczka[i].Wagaadd = 'l' + paczka[i].Waga.ToString();
                }
            }

        }
        private void WalutaPLN_Checked(object sender, RoutedEventArgs e)
        {
            int ile = paczka.Count;
            for (int i = 0; i < ile; i++)
            {
                if (paczka[i].Cenaadd[0] == 'e')
                {
                    double a = Convert.ToDouble(paczka[i].Cena);
                    a *= 4.27161738;
                    paczka[i].Cena = Convert.ToDecimal(a);
                    paczka[i].Cenaadd = "p" + paczka[i].Cena.ToString();
                }
                else if (paczka[i].Cenaadd[0] == 'u')
                {
                    double a = Convert.ToDouble(paczka[i].Cena);
                    a *= 3.65204752;
                    paczka[i].Cena = Convert.ToDecimal(a);
                    paczka[i].Cenaadd = "p" + paczka[i].Cena.ToString();
                }
                
            }         

        }
        private void WalutaEU_Checked(object sender, RoutedEventArgs e)
        {
            int ile = paczka.Count;
            for (int i = 0; i < ile; i++)
            {
                if (paczka[i].Cenaadd[0] == 'p')
                {
                    double a = Convert.ToDouble(paczka[i].Cena);
                    a *= 0.234103364;                    
                    paczka[i].Cena = Convert.ToDecimal(a);
                    paczka[i].Cenaadd = "e" + paczka[i].Cena.ToString();
                }
                else if (paczka[i].Cenaadd[0] == 'u')
                {
                    double a = Convert.ToDouble(paczka[i].Cena);
                    a *= 0.854956611;                    
                    paczka[i].Cena = Convert.ToDecimal(a);
                    paczka[i].Cenaadd = "e" + paczka[i].Cena.ToString();
                }

            }
        }
        private void WalutaUSD_Checked(object sender, RoutedEventArgs e)
        {
            int ile = paczka.Count;
            for (int i = 0; i < ile; i++)
            {
                if (paczka[i].Cenaadd[0] == 'p')
                {
                    double a = Convert.ToDouble(paczka[i].Cena);
                    a *= 0.273819;
                    paczka[i].Cena = Convert.ToDecimal(a);
                    paczka[i].Cenaadd = "u" + paczka[i].Cena.ToString();
                }
                else if (paczka[i].Cenaadd[0] == 'e')
                {
                    double a = Convert.ToDouble(paczka[i].Cena);
                    a *= 1.16965;
                    paczka[i].Cena = Convert.ToDecimal(a);
                    paczka[i].Cenaadd = "u" + paczka[i].Cena.ToString();
                }      

            }
        }

        //[Paczki] Funkcje grupujące.
        private void GroupCena(object sender, RoutedEventArgs e)
        {
            View.GroupDescriptions.Clear();
            View.SortDescriptions.Add(new SortDescription("Cena", ListSortDirection.Ascending));
            PriceRangeProductGrouper grouper = new PriceRangeProductGrouper();
            grouper.GroupInterval = 20;
            View.GroupDescriptions.Add(new PropertyGroupDescription("Cena", grouper));
        }
        private void GroupStatus(object sender, RoutedEventArgs e)
        {
            View.GroupDescriptions.Clear();
            View.GroupDescriptions.Add(new PropertyGroupDescription("Status"));
        }
        private void GroupNone(object sender, RoutedEventArgs e)
        {
            View.GroupDescriptions.Clear();
        }

        //[Paczki] Funkcje filtrujące.

        private void SortujKuriera()
        {
            if (Thread.CurrentPrincipal.IsInRole("Kurier") == true)
            {
                View.Filter = delegate (object item)
                {
                    Paczka product = item as Paczka;
                    if (product != null)
                    {
                        return (product.Kurier == Thread.CurrentPrincipal.Identity.Name);
                    }
                    return false;
                };
            }
            else View.Filter = null;
        }
        private void Filter(object sender, RoutedEventArgs e)
        {
            decimal minimumPrice;
            if (Decimal.TryParse(txtMinPrice.Text, out minimumPrice))
            {
                View.Filter = delegate (object item)
                {
                    Paczka product = item as Paczka;
                    if (product != null)
                    {
                        return (product.Cena > minimumPrice);
                    }
                    return false;
                };
            }
        }
        private void FilterNone(object sender, RoutedEventArgs e)
        {
            SortujKuriera();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabPaczki.IsSelected)
                SortujKuriera();
        }
    }
}
