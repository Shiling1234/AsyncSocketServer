using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AsyncSocketServer.ViewModel.Converter
{
   public class StatusConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = (string)value;
            String para = (String)parameter;
            if (para == "Start")
            {
                if (status == "Stopped")
                    return true;
                else
                    return false;
            }
            else
            {
                if (status == "Stopped")
                    return false;
                else
                    return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
