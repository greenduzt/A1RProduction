using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace A1QSystem.Core
{
    public class IgnoreNewItemPlaceHolderConverter : IValueConverter
    {
        private const string NewItemPlaceholderName = "{NewItemPlaceholder}";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value.ToString() == NewItemPlaceholderName)
            {
                value = DependencyProperty.UnsetValue;
            }
            return value;
        }
    }
}
