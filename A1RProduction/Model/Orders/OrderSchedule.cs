
using A1QSystem.Core;
using A1QSystem.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MsgBox;
using A1QSystem.View;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;

namespace A1QSystem.Model.Orders
{
    public class OrderSchedule : ObservableObject
    {

        public OrderSchedule()
        {
            _editProductionOrderCommand = new DelegateCommand(ShowEditOrderWindow);
            _canExecute = true;
        }

        public int OrderProductionID { get; set; }

        private string _orderProductionIDString;
        public string OrderProductionIDString
        {
            get 
            {
                _orderProductionIDString = " - " + GroupBy;
                return _orderProductionIDString;
            }
        }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int RawProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }

        private int _orderQty;
        public int OrderQty
        {
            get { return _orderQty; }
            set { _orderQty = value;
                RaisePropertyChanged(() => this.OrderQty);
            }
        }

        public string Unit { get; set; }
        public string ProductionDate { get; set; }
        public string Instruction { get; set; }
        public string GroupBy { get; set; }

        private DelegateCommand _editProductionOrderCommand;
        private ICommand _acceptCommand;
        private ICommand _declineCommand;

        private bool _canExecute;        

        private void DeclineOrder(int OrderID)
        {
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();

            DateTime nextDate = bdg.AddBusinessDays(Convert.ToDateTime(ProductionDate), 1);

            if (Msg.Show("Are you sure you want to shift this order to " + nextDate.Date.ToString("d") + " ?", "Order Shifting Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
            {
                int res = DBAccess.DeclineProductionOrder(nextDate.Date.ToString("d"), OrderID);

                if (res > 0)
                {
                    Msg.Show("Order has been shifted to " + nextDate.Date.ToString("d"), "Order Successfully Shifted", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);
                }
            }

        }

        private void AcceptOrder(int OrderID)
        {
            Console.WriteLine("Accepted : " + OrderID);
        }

        private void ShowEditOrderWindow()
        {
            var childWindow = new ChildWindowView();
            childWindow.orderProduction_Closed += (r =>{});
            childWindow.ShowOrderProductionDetails(OrderProductionID,OrderID);
        }

        #region COMMANDS
        public ICommand DeclineCommand
        {
            get
            {
                return _declineCommand ?? (_declineCommand = new A1QSystem.Commands.LogOutCommandHandler(() => DeclineOrder(OrderID), _canExecute));
            }
        }

        public ICommand AcceptCommand
        {
            get
            {
                return _acceptCommand ?? (_acceptCommand = new A1QSystem.Commands.LogOutCommandHandler(() => AcceptOrder(OrderID), _canExecute));
            }
        }

        public DelegateCommand EditProductionOrderCommand
        {
            get { return _editProductionOrderCommand; }
        }

        #endregion
       
    }

}
