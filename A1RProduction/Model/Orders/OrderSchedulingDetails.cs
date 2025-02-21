using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model.Products;
using A1QSystem.Model.Stock;
using A1QSystem.View;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Model.Orders
{
    public class OrderSchedulingDetails
    {
        private ICommand _acceptCommand;
        private ICommand _shiftOrderCommand;
        private ICommand _viewOrderCommand;

        public OrderDetails OrderDetails { get; set; }
        public RawProduct RawProducts { get; set; }
        public A1QSystem.Model.Stock.Stock Stock { get; set; }
        public decimal QtyToMake { get; set; }
        public StockStatus Status { get; set; }
        public string StatusBackColour { get; set; }
        public string ShiftName { get; set; }    

        private bool _canExecute;

        public OrderSchedulingDetails()
        {
            _canExecute = true;           
        }

        private void AcceptOrder()
        {
            //TO DO
        }

        private void ShiftOrder()
        {
            //var childWindow = new ChildWindowView();
            //childWindow.ShowShiftOrderWindow();
        }

        private void ViewOrder()
        {

        }

        #region COMMANDS
        public ICommand ShiftOrderCommand
        {
            get
            {
                return _shiftOrderCommand ?? (_shiftOrderCommand = new LogOutCommandHandler(() => ShiftOrder(), _canExecute));
            }
        }

        public ICommand AcceptCommand
        {
            get
            {
                return _acceptCommand ?? (_acceptCommand = new LogOutCommandHandler(() => AcceptOrder(), _canExecute));
            }
        }
        public ICommand ViewOrderCommand
        {
            get
            {
                return _viewOrderCommand ?? (_viewOrderCommand = new LogOutCommandHandler(() => ViewOrder(), _canExecute));
            }
        }
        
        #endregion
    }
}
