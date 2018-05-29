using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace ProjektWPF
{
    [ValueConversion(typeof(decimal), typeof(string))]
    public class CenaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal cena = (decimal)value;
            return cena.ToString("C", culture);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string cena = value.ToString();
            decimal result;
            if (Decimal.TryParse(cena, NumberStyles.Any, culture, out result))
            {
                return result;
            }
            return value;
        }
    }
}
