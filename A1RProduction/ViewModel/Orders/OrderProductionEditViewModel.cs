using A1QSystem.DB;
using A1QSystem.Model.Orders;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MsgBox;
using A1QSystem.Core;
using System.ComponentModel;

namespace A1QSystem.ViewModel.Orders
{
    public class OrderProductionEditViewModel : ViewModelBase
    {
        private DelegateCommand _closeCommand;
        private ICommand _updateCommand;
        private int _orderProductionNo;
        private int _orderNo;
        private DateTime _orderProDateStart;
        private DateTime _orderProDateEnd;
        private DateTime _selectedOrderDate;
        private bool _canExecute;

        public int _orderQty;
        private BindingList<OrderSchedule> _orderProduction;
        public event Action<OrderSchedule> Closed;

        public OrderProductionEditViewModel(int opNo, int oNo)
        {
            _orderProductionNo = opNo;
            _orderNo = oNo;
            _closeCommand = new DelegateCommand(CloseForm);

            orderProduction = new BindingList<OrderSchedule>();
            
            orderProduction = DBAccess.GetOrderOrderProdDetByOrderID(OrderNo);

            foreach (var item in orderProduction)
	        {
                _selectedOrderDate = Convert.ToDateTime(item.ProductionDate);
                break;
            }

            _canExecute = true;           

            DateTime today = DateTime.Today;
                      
            OrderProDateStart = today.Date;
            OrderProDateEnd = Convert.ToDateTime(SubtractBusinessDays(today, 5));

            //DateTime today = DateTime.Today;

            BusinessDaysGenerator bg = new BusinessDaysGenerator();

            DateTime start = today.Date;
            DateTime end = bg.AddBusinessDays(today, 5);

            List<DateTime> bs = new List<DateTime>();
            int days = 6;

            for (int x = 0; x < days; x++)
            {
                bs.Add(bg.AddBusinessDays(today, x));
            }
        }

        private List<DateTime> bs;
        public List<DateTime> Bs
        {
            get { return bs; }
            set
            {
                bs = value;
                RaisePropertyChanged(() => this.Bs);
            }
        }

        
        private void CloseForm()
        {
            if (Closed != null)
            {
                var customer = new OrderSchedule();

                Closed(customer);
            }
        }

        private void UpdateProductionOrderDetails()
        {
            int res = DBAccess.UpdateOrderProductionQty(OrderNo, 50, SelectedOrderDate);           
            
            if (res > 0)
            {
                Msg.Show("Order has been updated.", "Order Updated", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);
            }
            else
            {

            }
        }

        public string SubtractBusinessDays(DateTime current, int days)
        {  
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();

            string WorkingDays = bdg.AddBusinessDays(current, days).ToString("dd/MM/yyyy");

            return WorkingDays;
        }

        #region PUBLIC PROPERTIES

        public BindingList<OrderSchedule> orderProduction
        {
            get { return _orderProduction; }
            set
            {
                _orderProduction = value;
                if (_orderProduction != null)
                {
                    _orderProduction.ListChanged += (o, e) => RaisePropertyChanged(() => this.OrderQty);
                }

                RaisePropertyChanged(() => this.orderProduction);
               
            }
        }

        public int OrderQty
        {
            get { return _orderQty = _orderProduction.Sum(x => x.OrderQty); }
            set
            {
                _orderQty = value;
                
                Console.WriteLine(OrderQty);
            }
        }
           

        public int OrderProductionNo
        {
            get { return _orderProductionNo; }
            set { _orderProductionNo = value;}
        }
        public int OrderNo
        {
            get { return _orderNo;}
            set { _orderNo = value;}
        }

        public DateTime OrderProDateStart
        {
            get { return _orderProDateStart; }
            set { _orderProDateStart = value;}
        }

        public DateTime OrderProDateEnd
        {
            get { return _orderProDateEnd; }
            set { _orderProDateEnd = value;}
        }
        public DateTime SelectedOrderDate
        {
            get { return _selectedOrderDate; }
            set { _selectedOrderDate = value;
                RaisePropertyChanged(() => this.SelectedOrderDate); }
        }      


        #endregion

        #region COMMANDS

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public ICommand UpdateCommand
        {
            get
            {
                return _updateCommand ?? (_updateCommand = new A1QSystem.Commands.LogOutCommandHandler(() => UpdateProductionOrderDetails(), _canExecute));
            }
        }

        #endregion  
    }
}
