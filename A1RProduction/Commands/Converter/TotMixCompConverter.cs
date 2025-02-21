using A1QSystem.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace A1QSystem.Converter
{
   
    public class TotMixCompConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var users = value as IEnumerable<object>;
            if (users == null)
                return 0;

            double sum = 0;

            foreach (var u in users)
            {
                sum += ((GridItem)u).MixesCompleted;
            }

            Console.WriteLine("SUM " + sum);

            return sum;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
