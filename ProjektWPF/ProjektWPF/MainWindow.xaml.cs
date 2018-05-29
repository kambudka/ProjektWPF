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

namespace ProjektWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
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
            paczka.Add(new Paczka(49.90M, "Zwierzyniecka 8", "5 kg", "W magazynie"));
            paczka.Add(new Paczka(59.90M, "Wiejska 50", "3 kg", "Wdrodze"));
            paczka.Add(new Paczka(34.90M, "Wesoła 16", "2 kg", "Dostarczona"));
            paczka.Add(new Paczka(46.90M, "Jurowiecka 31", "4 kg", "Zwrócona"));
            lista.ItemsSource = paczka;

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
        int nr = 1;  // numer faktury
        private List<Paczka> paczka = new List<Paczka>();
        Paczka znalezionaPaczka;  // do wypelniania faktury
        int fakturaIloscPozycjiStalych = 0;  // do aktuaalizacji canvasa

        private ListCollectionView View
        {
            get
            {
                return (ListCollectionView)CollectionViewSource.GetDefaultView(paczka);
            }
        }
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

        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            Okno_Dodaj dialog = new Okno_Dodaj();
            if (dialog.ShowDialog() == true)
            {
                paczka.Add(dialog.NowaPaczka);
                View.Refresh();
            }
            else
            {
            }
        }

        private void Wyszukaj_Click(object sender, RoutedEventArgs e)
        {

        }

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

        private void WiecejKopii_Click(object sender, RoutedEventArgs e)
        {
            int temp;
            bool cos = int.TryParse(IloscKopii.Text, out temp);
            if ((IloscKopii.Text != "") && (cos == true) && (temp < 10)) temp++;
            IloscKopii.Text = temp.ToString();
        }

        private void MniejKopii_Click(object sender, RoutedEventArgs e)
        {
            int temp;
            bool cos = int.TryParse(IloscKopii.Text, out temp);
            if ((IloscKopii.Text != "") && (cos == true) && (temp > 1)) temp--;
            IloscKopii.Text = temp.ToString();
        }

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
        
    }
}
