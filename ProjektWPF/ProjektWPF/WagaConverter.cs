using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace ProjektWPF
{
    [ValueConversion(typeof(decimal), typeof(string))]

    public class WagaConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string waga = (string)value;
            char[] weight = new char[waga.Length - 1];
            int i;
            for (i = 1; i < waga.Length; i++)
                weight[i - 1] = waga[i];
            string p = new string(weight);
            decimal temp = System.Convert.ToDecimal(p);
            temp = System.Math.Round(temp, 2);
            string result = temp.ToString();

            if (waga[0] == 'k')
            {
                result = result + " Kg";
            }
            else
            {
                result = result + " Lbs";
            }
           
            return result;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return value;
        }


    }
}
