using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AsyncSocketServer.ViewModel.Converter
{
    public class StartModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string para = (string)parameter;
            string val = (string)value;
            //手动
            if (para == "ChangeToManual")
            {
                if (val == "Manual")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (val == "Manual")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
