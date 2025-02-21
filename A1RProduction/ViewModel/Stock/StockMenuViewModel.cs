using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.AdminDashboard;
using A1QSystem.View.Stock.BlockLogStock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Stock
{
    public class StockMenuViewModel : ViewModelBase
    {
        private bool canExecute;
        private string userName;
        private string state;
        private List<UserPrivilages> privilages;
        private List<MetaData> metaData;
        private string _version;
        //private ICommand _productStockCommand;
        private ICommand _blockLogStockCommand;
        private ICommand _homeCommand;
        private ICommand _adminDashboardCommand;

        public StockMenuViewModel(string UserName, string State, List<UserPrivilages> uPriv, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = uPriv;
            canExecute = true;
            metaData = md;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
        }

        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
                RaisePropertyChanged(() => this.Version);
            }
        }


        #region Commands


        public ICommand ProductStockCommand
        {
            get
            {
                return null;// _productStockCommand ?? (_productStockCommand = new LogOutCommandHandler(() => Switcher.Switch(new WorkStationsView(userName, state, privilages)), canExecute));
            }
        }
        public ICommand BlockLogStockCommand
        {
            get
            {
                return _blockLogStockCommand ?? (_blockLogStockCommand = new LogOutCommandHandler(() => Switcher.Switch(new BlockLogStockView(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand HomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand AdminDashboardCommand
        {
            get
            {
                return _adminDashboardCommand ?? (_adminDashboardCommand = new LogOutCommandHandler(() => Switcher.Switch(new AdminDashboardView(userName, state, privilages, metaData)), canExecute));
            }
        }

        #endregion
    }
}
