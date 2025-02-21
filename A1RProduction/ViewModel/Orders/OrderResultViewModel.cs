using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Orders
{
    public class OrderResultViewModel : ViewModelBase
    {
        private List<OrderResult> _orderResult;
        public event Action Closed;
        private ICommand _closeCommand;

        public OrderResultViewModel(List<Tuple<DateTime,string, int, string>> data)
        {
            OrderResult = new List<OrderResult>();
            if (data.Count > 0)
            {
                foreach (var item in data)
	            {
                    OrderResult.Add(new OrderResult() {Date = item.Item1, ProdStartDate = item.Item1.ToString("dd/MM") +" ("+item.Item1.DayOfWeek + ")",ShiftName = item.Item2, RawProduct = new RawProduct() { Description = item.Item4 } });
	            }

                OrderResult.OrderBy(d => d.Date);  
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        public List<OrderResult> OrderResult
        {
            get { return _orderResult; }
            set
            {
                _orderResult = value;
                RaisePropertyChanged(() => this.OrderResult);
            }
        }


        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new LogOutCommandHandler(() => CloseForm(), true));
            }
        }
    }
}
