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

    public class CenaConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string cena = (string)value;
            char[] price = new char[cena.Length - 1];
            int i;
            for (i = 1; i < cena.Length; i++)
                price[i - 1] = cena[i];
            string p = new string(price);
            decimal temp = System.Convert.ToDecimal(p);
            temp = System.Math.Round(temp, 2);
            string result = temp.ToString();
            
            if (cena[0] == 'p')
            {
                result = result + " PLN";
            }
            else if (cena[0] =='e')
            {
                result = result + " EUR";
            }
            else
            {
                result = result + " USD";
            }          
            return result;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
          
            return value;
        }


    }
}
