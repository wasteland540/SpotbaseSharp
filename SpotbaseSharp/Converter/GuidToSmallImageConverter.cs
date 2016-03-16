using System;
using System.Globalization;
using System.Windows.Data;
using SpotbaseSharp.Util;

namespace SpotbaseSharp.Converter
{
    public class GuidToSmallImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var smallFile = new Guid(value.ToString());

            return ImageUtility.LoadSmallImage(smallFile);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}