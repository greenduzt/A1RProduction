using A1QSystem.Commands;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Orders.Status
{
    public class OrderStatusViewModel
    {
        private List<MetaData> metaData;
        private List<UserPrivilages> privilages;
        private string userName;
        private string state;
        private bool canExecute;
        private ICommand _homeCommand;
        private ICommand _ordersCommand;

        public OrderStatusViewModel(string UserName, string State, List<UserPrivilages> UserPrivilages, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = UserPrivilages;
            metaData = md;
            canExecute = true;
        }



        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, privilages,metaData)), canExecute));
            }
        }
        public ICommand OrdersCommand
        {
            get
            {
                return _ordersCommand ?? (_ordersCommand = new LogOutCommandHandler(() => Switcher.Switch(new OrdersMainMenuView(userName, state, privilages, metaData)), canExecute));
            }
        }
    }
}
