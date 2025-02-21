using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Stock;
using A1QSystem.View;
using A1QSystem.View.AdminDashboard;
using A1QSystem.View.Stock;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Stock.BlockLogStock
{
    public class BlockLogStockViewModel : ViewModelBase
    {
        private ObservableCollection<StockMaintenanceDetails> _blockLogStock;
        private string userName;
        private string state;
        private List<UserPrivilages> privilages;
        private bool canExecute;
        private List<MetaData> metaData;
        private string _version;
        private ICommand _updateCommand;
        private ICommand _stockCommand;
        private ICommand _refreshCommand;

        private ICommand _homeCommand;
        private ICommand _adminDashboardCommand;

        public BlockLogStockViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = Privilages;
            metaData = md;
            canExecute = true;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
            BlockLogStock = new ObservableCollection<StockMaintenanceDetails>();
            LoadRawStock();
        }

        private void LoadRawStock()
        {
            BlockLogStock=  DBAccess.GetRawStockDetails();
        }

        private void RefreshData()
        {
            BlockLogStock.Clear();
            LoadRawStock();
        }

        private void OpenUpdateView(object parameter)
        {
            int index = BlockLogStock.IndexOf(parameter as StockMaintenanceDetails);
            if (index > -1 && index < BlockLogStock.Count)
            {
                var childWindow = new ChildWindowView();
                childWindow.editRawStock_Closed += (r =>
                {
                    RefreshData();
                });
                childWindow.ShowEditRawStockWindow(BlockLogStock[index]);                
            }
            
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        public ObservableCollection<StockMaintenanceDetails> BlockLogStock
        {
            get
            {
                return _blockLogStock;
            }
            set
            {
                _blockLogStock = value;
                RaisePropertyChanged(() => this.BlockLogStock);
            }
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

        public ICommand UpdateCommand
        {
            get
            {
                if (_updateCommand == null)
                {
                    _updateCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, OpenUpdateView);
                }
                return _updateCommand;
            }
        }


        public ICommand StockCommand
        {
            get
            {
                return _stockCommand ?? (_stockCommand = new LogOutCommandHandler(() => Switcher.Switch(new StockMenuView(userName, state, privilages, metaData)), canExecute));
            }
        }       

        public ICommand HomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new LogOutCommandHandler(() => RefreshData(), canExecute));
            }
        }

        public ICommand AdminDashboardCommand
        {
            get
            {
                return _adminDashboardCommand ?? (_adminDashboardCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new AdminDashboardView(userName, state, privilages, metaData)), canExecute));
            }
        }
        #endregion
    }
}
