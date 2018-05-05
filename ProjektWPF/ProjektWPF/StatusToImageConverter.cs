using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace ProjektWPF
{
    public class StatusToImageConverter : IValueConverter
    {
        public StatusToImageConverter()
        {
            string dir = null, imgdir;
            do
            {
                if (dir == null)
                    dir = Directory.GetCurrentDirectory();
                else
                    dir = Directory.GetParent(dir).ToString();
                imgdir = System.IO.Path.Combine(dir, "images");
            } while (!Directory.Exists(imgdir));
            ImageDirectory = imgdir;
        }
        public string ImageDirectory { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string imagePath = System.IO.Path.Combine(ImageDirectory, (string)value);
                return new BitmapImage(new Uri(imagePath));
            }
            catch
            {
                return Binding.DoNothing;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

