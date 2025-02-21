using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace A1QSystem.Core
{
    public class BlackOutDatesAdapter
    {
        public static List<DateTime> GetBlackOutDates(DependencyObject obj)
        {
            return (List<DateTime>)obj.GetValue(BlackOutDatesProperty);
        }

        public static void SetBlackOutDates(DependencyObject obj, List<DateTime> value)
        {
            obj.SetValue(BlackOutDatesProperty, value);
        }

        // Using a DependencyProperty as the backing store for BlackOutDates.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlackOutDatesProperty =
            DependencyProperty.RegisterAttached("BlackOutDates", typeof(List<DateTime>), typeof(BlackOutDatesAdapter), new PropertyMetadata(null, OnBlackOutDatesChanged));

        private static void OnBlackOutDatesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as DatePicker;
            var list = (List<DateTime>)e.NewValue;
            foreach (var date in list)
            {
                control.BlackoutDates.Add(new CalendarDateRange(date));
            }
        }
    }
}
