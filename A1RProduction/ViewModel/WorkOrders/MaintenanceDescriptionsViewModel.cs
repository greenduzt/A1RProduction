using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Vehicles;
using A1QSystem.View;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.WorkOrders
{
    public class MaintenanceDescriptionsViewModel : ViewModelBase
    {
        private ObservableCollection<VehicleMaintenanceInfo> _vehicleMaintenanceInfo;
        private ObservableCollection<VehicleMaintenanceInfo> _safetyInspectionInfo;
        private VehicleWorkOrder _vehicleWorkOrder;
        private Int64 _odometer;
        private string _currentOdometerTitle;
        private string _maintenanceDesHeader;
        public string frequency;
        public string unit;
        private string _hideOdo;      
        private bool canExecute;
        public bool isPrinted;
        public event Action<MaintenanceDescriptionsViewModel> Closed;
        private DelegateCommand _closeCommand;
        private ICommand _submitCommand;
        private ICommand _printCommand;

        public MaintenanceDescriptionsViewModel(VehicleWorkOrder vwo)
        {
            //GridVisibility = "Collapsed";
            _closeCommand = new DelegateCommand(CloseForm);
            canExecute = true;
            VehicleMaintenanceInfo = new ObservableCollection<VehicleMaintenanceInfo>();
            VehicleWorkOrder = vwo;
            CurrentOdometerTitle = "Odometer reading in " + GetOdometerReadingTitle(VehicleWorkOrder.Vehicle.VehicleBrand);
            Odometer = VehicleWorkOrder.OdometerReading;
            List<int> nos = new List<int>();
            List<VehicleMaintenanceSequence> VehicleMaintenanceSequenceList = DBAccess.GetVehicleMaintenanceSequence(VehicleWorkOrder.Vehicle.VehicleCategory.ID);
            foreach (var item in VehicleMaintenanceSequenceList)
            {
                if (item.Kmhrs == 0)
                {
                    nos.Add(item.ID);
                }
            }
            if (VehicleWorkOrder.Vehicle.VehicleBrand == "HDK" || VehicleWorkOrder.Vehicle.ID == 51 || VehicleWorkOrder.Vehicle.ID == 54)
            {
                HideOdo = "Collapsed";
            }
            else
            {
                HideOdo = "Visible";
            }

            Int32 res = DBAccess.UpdateVehicleWorkOrderViewed(VehicleWorkOrder.VehicleWorkOrderID, Odometer);

            SafetyInspectionInfo = DBAccess.GetMaintenanceInfoByMeterReading(nos, 1);
            LoadDescriptions();
            //OdoVisibility = VehicleWorkOrder.Vehicle.VehicleBrand == "HDK" ? "Collapsed" : "Visible";
        }

        private string GetOdometerReadingTitle(string brand)
        {
            string n = string.Empty;

            if (brand == "Linde")
            {
                n = "Hours";
            }
            else
            {
                n = "Km";
            }
           
            return n;
        }

        private void LoadDescriptions()
        {
            List<VehicleMaintenanceSequence> VehicleMaintenanceSequenceList = DBAccess.GetVehicleMaintenanceSequence(VehicleWorkOrder.Vehicle.VehicleCategory.ID);
            Tuple<int, List<Tuple<string, string>>> tup = null;
            VehicleWorkOrderManager vwom = new VehicleWorkOrderManager();
            tup=vwom.CreateVehicleWorkOrder(VehicleWorkOrder,Odometer);

            int msid = 0;
            msid = tup.Item1;

            foreach (var item in tup.Item2)
            {
                if(item.Item1 == "MaintenanceDesHeader")
                {
                    //if(VehicleWorkOrder.Vehicle.VehicleCategory.ID == 8)
                    //{
                    //    MaintenanceDesHeader = "";
                    //}
                    //else
                    //{
                        MaintenanceDesHeader = item.Item2;
                    //}
                }
                else if (item.Item1 == "frequency")
                {
                    if (VehicleWorkOrder.Vehicle.ID == 51)
                    {
                        frequency = "8";
                    }
                    if (VehicleWorkOrder.Vehicle.ID == 54)
                    {
                        frequency = "9";
                    }
                    else
                    {
                        frequency = item.Item2;
                    }
                    
                }
                else if (item.Item1 == "unit")
                {
                    unit = item.Item2;
                }
            }
            if (msid == 0)
            {
                Msg.Show("Cannot retrieve information. Please try again later.", "Error Fetching Data", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else
            {
                List<int> nos = new List<int>();
                bool foundItem = false;
                foreach (var item in VehicleMaintenanceSequenceList)
                {
                    if (item.ID <= msid)
                    {
                        nos.Add(item.ID);
                        if(item.ID==3)
                        {
                            foundItem = true;
                        }
                    }
                }
                if (foundItem == true)
                {
                    int value = nos[2];
                    nos.RemoveAt(2);
                    nos.Insert(1, value);

                }
                VehicleMaintenanceInfo = DBAccess.GetMaintenanceInfoByMeterReading(nos, 1);
            }
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        private void CloseForm()
        {
            isPrinted = false;
            FinalClose();
        }

        private void FinalClose()
        {
            if (Closed != null)
            {                
                Closed(this);
            }
        }

        private void PrintDescription()
        {

            //if (VehicleWorkOrder.Vehicle.VehicleBrand == "HDK")
            //{
            //    foreach (var item in SafetyInspectionInfo)
            //    {
            //        VehicleMaintenanceInfo.Add(new VehicleMaintenanceInfo() { ID = item.ID,Code=item.Code,Description=item.Description});
            //    }
            //}

            isPrinted = true;

            Exception exception = null;
            BackgroundWorker worker = new BackgroundWorker();
            ChildWindowView LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Printing");

            worker.DoWork += (_, __) =>
            {
                VehicleWorkOrderPDF vwopdf = new VehicleWorkOrderPDF(VehicleWorkOrder, VehicleMaintenanceInfo, SafetyInspectionInfo, frequency, unit, Odometer);
                exception=vwopdf.createWorkOrderPDF();
            };

            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();
                if (exception != null)
                {
                    Msg.Show("A problem has occured while the work order is prining. Please try again later." + System.Environment.NewLine + exception, "Printing Error", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }
                FinalClose();
            };
            worker.RunWorkerAsync();

            
            
            
            //VehicleWorkOrderPDF vwopdf = new VehicleWorkOrderPDF(VehicleWorkOrder, VehicleMaintenanceInfo, SafetyInspectionInfo, frequency,unit,Odometer);
            //vwopdf.createWorkOrderPDF();
        }

        //public string GridVisibility
        //{
        //    get { return _gridVisibility; }
        //    set
        //    {
        //        _gridVisibility = value;
        //        RaisePropertyChanged(() => this.GridVisibility);
        //    }
        //}

        public Int64 Odometer
        {
            get
            {
                return _odometer;
            }
            set
            {
                _odometer = value;
                RaisePropertyChanged(() => this.Odometer);
            }
        }

        public ObservableCollection<VehicleMaintenanceInfo> VehicleMaintenanceInfo
        {
            get
            {
                return _vehicleMaintenanceInfo;
            }
            set
            {
                _vehicleMaintenanceInfo = value;
                RaisePropertyChanged(() => this.VehicleMaintenanceInfo);
            }
        }

        public VehicleWorkOrder VehicleWorkOrder
        {
            get
            {
                return _vehicleWorkOrder;
            }
            set
            {
                _vehicleWorkOrder = value;
                RaisePropertyChanged(() => this.VehicleWorkOrder);
            }
        }

        public ObservableCollection<VehicleMaintenanceInfo> SafetyInspectionInfo
        {
            get
            {
                return _safetyInspectionInfo;
            }
            set
            {
                _safetyInspectionInfo = value;
                RaisePropertyChanged(() => this.SafetyInspectionInfo);
            }
        }

        public string CurrentOdometerTitle
        {
            get
            {
                return _currentOdometerTitle;
            }
            set
            {
                _currentOdometerTitle = value;
                RaisePropertyChanged(() => this.CurrentOdometerTitle);
            }
        }

        //public string OdoVisibility
        //{
        //    get
        //    {
        //        return _odoVisibility;
        //    }
        //    set
        //    {
        //        _odoVisibility = value;
        //        RaisePropertyChanged(() => this.OdoVisibility);
        //    }
        //}
        public string MaintenanceDesHeader
        {
            get
            {
                return _maintenanceDesHeader;
            }
            set
            {
                _maintenanceDesHeader = value;
                RaisePropertyChanged(() => this.MaintenanceDesHeader);
            }
        }

        public string HideOdo
        {
            get
            {
                return _hideOdo;
            }
            set
            {
                _hideOdo = value;
                RaisePropertyChanged(() => this.HideOdo);
            }
        }

        

        //public string PrintingBackgroundCol
        //{
        //    get
        //    {
        //        return _printingBackgroundCol;
        //    }
        //    set
        //    {
        //        _printingBackgroundCol = value;
        //        RaisePropertyChanged(() => this.PrintingBackgroundCol);
        //    }
        //}

        //public bool PrintingEnabled
        //{
        //    get
        //    {
        //        return _printingEnabled;
        //    }
        //    set
        //    {
        //        _printingEnabled = value;
        //        RaisePropertyChanged(() => this.PrintingEnabled);
        //    }
        //}
        
        

        #region COMMANDS
        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public ICommand SubmitCommand
        {
            get
            {
                return _submitCommand ?? (_submitCommand = new A1QSystem.Commands.LogOutCommandHandler(() => LoadDescriptions(), canExecute));
            }
        }

        public ICommand PrintCommand
        {
            get
            {
                return _printCommand ?? (_printCommand = new A1QSystem.Commands.LogOutCommandHandler(() => PrintDescription(), canExecute));
            }
        }

        
        
        #endregion
    }
}
