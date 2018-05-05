﻿using System;
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

namespace ProjektWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            paczka.Add(new Paczka(49.90M, "Zwierzyniecka 8", 5,"W magazynie"));
            paczka.Add(new Paczka(59.90M, "Wiejska 50", 3, "Wdrodze"));
            paczka.Add(new Paczka(34.90M, "Wesoła 16", 2, "Dostarczona"));
            paczka.Add(new Paczka(46.90M, "Jurowiecka 31", 4, "Zwrócona"));


            lista.ItemsSource = paczka;
        }

        private List<object> paczka = new List<object>();
        private ListCollectionView View
        {
            get
            {
                return (ListCollectionView)CollectionViewSource.GetDefaultView(paczka);
            }
        }
        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            Okno_Dodaj dialog = new Okno_Dodaj();
            if (dialog.ShowDialog() == true)
            {
                lista.Items.Add(dialog.czlowiek);
            }
            else
            {
            }
        }

        private void Wyszukaj_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
