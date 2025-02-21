using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace A1QSystem.Converter
{
    public class FreightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double _Sum =0;
            string _Sum1 = "0";
            string _Sum2 = "0";

            if (values[0].ToString() == "" || values[1].ToString() == "" || values[0] == null || values[1] == null )
            {
                return _Sum;
            }
            else
            {
                _Sum1 = values[0].ToString();
                _Sum2 = values[1].ToString();

                _Sum = double.Parse(_Sum1) * double.Parse(_Sum2);
            }

         /*     foreach (var item in values)
          {
                double _Value;
                if (double.TryParse(item.ToString(), out _Value))
                    _Sum += _Value;
            }
          */
         return _Sum;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
