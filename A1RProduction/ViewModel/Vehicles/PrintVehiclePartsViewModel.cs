using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Vehicles;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Vehicles
{
    public class PrintVehiclePartsViewModel : ViewModelBase
    {
        private ObservableCollection<Int32> _workOrderNo;
        private ObservableCollection<Vehicle> _vehicles;
        private ObservableCollection<VehiclePartDescription> _vehiclePartDescription;
        private List<UserPrivilages> privilages;
        private string userName;
        private string state;
        private string _searchButtonBackground;
        private List<MetaData> metaData;
        private int _selectedVehicle;
        private Int32 _selectedWorkOrderNo;
        private bool _searchEnabled;
        private bool _printEnabled;
        private string _version;
        private string _printButtonBackground;
        private bool canExecute;
        private ObservableCollection<VehiclePartDescription> vpdList;

        private ICommand _homeCommand;
        private ICommand _searchCommand;
        private ICommand _printCommand;

        public PrintVehiclePartsViewModel(string UserName, string State, List<UserPrivilages> UserPrivilages, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = UserPrivilages;
            metaData = md;
            VehiclePartDescription = new ObservableCollection<VehiclePartDescription>();
            WorkOrderNo = new ObservableCollection<Int32>();
            SearchEnabled = false;
            LoadVehicles();
            canExecute = true;           
        }

        private void LoadVehicleParts()
        {
            PrintEnabled = false;
            PrintButtonBackground = "#FFDEDEDE";
            VehiclePartDescription.Clear();
            vpdList = DBAccess.GetVehicleParts(SelectedVehicle);
            WorkOrderNo.Clear();
            if (vpdList.Count > 0)
            {
                WorkOrderNo.Add(0);
                foreach (var item in vpdList)
                {
                    if (item.Vehicle.ID == SelectedVehicle)
                    {
                        var data = WorkOrderNo.SingleOrDefault(x=>x==item.ID);
                        if (data == 0)
                        {
                            WorkOrderNo.Add(item.ID);
                        }
                    }
                }
                SelectedWorkOrderNo = 0;
            }
            else
            {
                WorkOrderNo.Clear();
                WorkOrderNo.Add(0);
                SelectedWorkOrderNo = 0;
            }
        }

        private void LoadVehicles()
        {
            Vehicles = DBAccess.GetAllVehicles();
            Vehicles.Add(new Vehicle() { ID = 0, VehicleString = "Select" });
            SelectedVehicle = 0;
        }

        private void SearchPart()
        {
            if (vpdList.Count > 0)
            {
                VehiclePartDescription = vpdList;
            }
        }

        private void EnableSearch()
        {
            PrintEnabled = false;
            PrintButtonBackground = "#FFDEDEDE";

            if(SelectedVehicle > 0 && SelectedWorkOrderNo > 0)
            {
                SearchEnabled = true;
                SearchButtonBackground = "#FF787C7A";
            }
            else
            {
                SearchEnabled = false;
                SearchButtonBackground = "#FFDEDEDE";
            }
        }

        private void PrintPart()
        {
             Exception exception = null;
            BackgroundWorker worker = new BackgroundWorker();
            ChildWindowView LoadingScreen;
            LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Printing");

            worker.DoWork += (_, __) =>
            {
                Vehicle vehicle = Vehicles.SingleOrDefault(x=>x.ID == SelectedVehicle);

                PartPDF ppdf = new PartPDF(vehicle, SelectedWorkOrderNo, VehiclePartDescription);
                exception = ppdf.createWorkOrderPDF();
            };

            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();
                if (exception != null)
                {
                    Msg.Show("A problem has occured while prining and did not print properly. Please try again later.", "Printing Error", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }

            };
            worker.RunWorkerAsync();
        }

        public ObservableCollection<VehiclePartDescription> VehiclePartDescription
        {
            get
            {
                return _vehiclePartDescription;
            }
            set
            {
                _vehiclePartDescription = value;
                RaisePropertyChanged(() => this.VehiclePartDescription);
                if(VehiclePartDescription.Count > 0)
                {
                    PrintEnabled = true;
                    PrintButtonBackground = "#FF787C7A";
                }
                else
                {
                    PrintEnabled = false;
                    PrintButtonBackground = "#FFDEDEDE";
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

        public ObservableCollection<Vehicle> Vehicles
        {
            get
            {
                return _vehicles;
            }
            set
            {
                _vehicles = value;
                RaisePropertyChanged(() => this.Vehicles);
            }
        }

        public int SelectedVehicle
        {
            get
            {
                return _selectedVehicle;
            }
            set
            {
                _selectedVehicle = value;
                RaisePropertyChanged(() => this.SelectedVehicle);
                LoadVehicleParts();
            }
        }

        public ObservableCollection<Int32> WorkOrderNo
        {
            get
            {
                return _workOrderNo;
            }
            set
            {
                _workOrderNo = value;
                RaisePropertyChanged(() => this.WorkOrderNo);
            }
        }

        public Int32 SelectedWorkOrderNo
        {
            get
            {
                return _selectedWorkOrderNo;
            }
            set
            {
                _selectedWorkOrderNo = value;
                RaisePropertyChanged(() => this.SelectedWorkOrderNo);
                EnableSearch();
            }
        }

        public bool SearchEnabled
        {
            get
            {
                return _searchEnabled;
            }
            set
            {
                _searchEnabled = value;
                RaisePropertyChanged(() => this.SearchEnabled);                
            }
        }

        public string SearchButtonBackground
        {
            get
            {
                return _searchButtonBackground;
            }
            set
            {
                _searchButtonBackground = value;
                RaisePropertyChanged(() => this.SearchButtonBackground);
            }
        }

        public bool PrintEnabled
        {
            get
            {
                return _printEnabled;
            }
            set
            {
                _printEnabled = value;
                RaisePropertyChanged(() => this.PrintEnabled);
            }
        }

        public string PrintButtonBackground
        {
            get
            {
                return _printButtonBackground;
            }
            set
            {
                _printButtonBackground = value;
                RaisePropertyChanged(() => this.PrintButtonBackground);
            }
        }
        
        

        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new A1QSystem.Commands.LogOutCommandHandler(() => SearchPart(), canExecute));
            }
        }

        public ICommand PrintCommand
        {
            get
            {
                return _printCommand ?? (_printCommand = new A1QSystem.Commands.LogOutCommandHandler(() => PrintPart(), canExecute));
            }
        }

        

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, privilages, metaData)), canExecute));
            }
        }
    }
}
