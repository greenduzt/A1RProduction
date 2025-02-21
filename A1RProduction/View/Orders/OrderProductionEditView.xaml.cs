using A1QSystem.Core;
using A1QSystem.Model.Orders;
using A1QSystem.ViewModel.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace A1QSystem.View.Orders
{
    /// <summary>
    /// Interaction logic for OrderProductionEditView.xaml
    /// </summary>
    public partial class OrderProductionEditView : UserControl
    {
        public OrderProductionEditView(int OrderProductionID, int orderNo)
        {
            InitializeComponent();
            DataContext = new OrderProductionEditViewModel(OrderProductionID, orderNo);
        }
               

        private void datePicker_Loaded(object sender, RoutedEventArgs e)
        {
            DatePicker picker = sender as DatePicker;

            if (picker.DisplayDateStart == null || picker.DisplayDateEnd == null) return;

            picker.BlackoutDates.Clear();

            DateTime today = DateTime.Today;

            BusinessDaysGenerator bg = new BusinessDaysGenerator();


            DateTime start = today.Date;
            DateTime end = bg.AddBusinessDays(today, 5);
                       

            List<DateTime> bs = new List<DateTime>();
            int days = 6;

            for (int x = 0; x < days; x++)
            {
                bs.Add(bg.AddBusinessDays(today, x));
            }


            while (start <= end)
            {
                if (!bs.Contains(start))
                {
                    picker.BlackoutDates.Add(new CalendarDateRange(start, start));
                }
                start = start.AddDays(1);
            }
        }

      
    }
}
