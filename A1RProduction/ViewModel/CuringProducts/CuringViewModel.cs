using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.RawMaterials;
using A1QSystem.View;
using A1QSystem.View.Dashboard;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.CuringProducts
{
    public class CuringViewModel : ViewModelBase
    {
        private ObservableCollection<Curing> _curingList;
        private string userName;
        private string state;
        private List<UserPrivilages> privilages;
        private List<MetaData> metaData;
        private bool canExecute;
        private string _version;
        private ICommand navHomeCommand;
        private ICommand _workStationsCommand;
        private ICommand _refreshCommand;
        private ICommand _endCuringCommand;

        public CuringViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = Privilages;
            metaData = md;
            canExecute = true;
            CuringList = new ObservableCollection<Curing>();
            LoadCuringData();
            canExecute = true;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;

            
        }

        private void LoadCuringData()
        {
            CuringList = DBAccess.GetAllCuringStock();
        }

        private void GoHome()
        {
            Switcher.Switch(new MainMenu(userName, state, privilages, metaData));
        }

        private void GoToProduction()
        {
            Switcher.Switch(new WorkStationsView(userName, state, privilages, metaData));
        }

        private void RefreshData()
        {
            CuringList.Clear();
            LoadCuringData();
        }

        private void EndCuring(Object parameter)
        {
            int index = CuringList.IndexOf(parameter as Curing);
            if (index >= 0)
            {
                //Check if the block log has already been cured
                int x = DBAccess.CheckCuringExist(CuringList[index].id);

                if (x > 0)
                {
                    bool re = DBAccess.GetSystemParameter("CheckCuringEnded");
                    if (re == true)
                    {
                        Msg.Show("Stock is bieng updated at the moment. Please try again in 5 minutes ", "Stock Updating", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                    }
                    else
                    {
                        if (Msg.Show("Are you sure you want to end curing?", "End Curing Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                        {
                            int res1 = DBAccess.UpdateBlockLogStockAfterDemoulding(CuringList[index]);//Update RawStock by 1 and set to cured in BlockLogCuring
                            if (1 > 0)
                            {
                                PendingOrderManager po = new PendingOrderManager(CuringList[index]);
                                int res2 = po.ProcessPendingOrders();
                                if (res2 > 1)
                                {
                                    Console.WriteLine("Added to slitting/peeling");
                                }
                                else
                                {
                                    Console.WriteLine("Did not process any pending orders");
                                    //Msg.Show("Could not process the pending slitting and peeling orders", "Pending Order Processing Failed", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                                }
                                RefreshData();
                            }
                        }
                    }
                }
                else
                {
                    RefreshData();
                    Msg.Show("This block/log has been already ended curing", "Curing Has Already Been Ended", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                }
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

        public ObservableCollection<Curing> CuringList
        {
            get
            {
                return _curingList;
            }
            set
            {
                _curingList = value;
                RaisePropertyChanged(() => this.CuringList);
            }
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }


        #region COMMANDS
        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => GoHome(), canExecute));
            }
        }

        public ICommand WorkStationsCommand
        {
            get
            {
                return _workStationsCommand ?? (_workStationsCommand = new LogOutCommandHandler(() => GoToProduction(), canExecute));
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new LogOutCommandHandler(() => RefreshData(), canExecute));
            }
        }
        public ICommand EndCuringCommand
        {

             get
            {
                if (_endCuringCommand == null)
                {
                    _endCuringCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, EndCuring);
                }
                return _endCuringCommand;
            }          
        }



        #endregion
    }
}
