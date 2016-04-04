using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace AsyncSocketServer.ViewModel.Converter
{
  public  class ServerDescConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            string description = value as string;
            if (description.Length <= 13) return description;
            int count=13;
            while (MeasureTextWidth(description.Substring(0, count), 11, "Consolas") < 140)
            {
                count++;
                if (count == description.Length) break;
            }
            return description.Substring(0, count) + "...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        private double MeasureTextWidth(string text, double fontSize, string fontFamily)
        {
            FormattedText formattedText = new FormattedText(
            text,
            System.Globalization.CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface(fontFamily.ToString()),
            fontSize,
            Brushes.Black
            );
            return formattedText.WidthIncludingTrailingWhitespace;
        }
    }
}
