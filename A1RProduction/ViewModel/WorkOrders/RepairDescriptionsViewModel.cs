using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Vehicles;
using A1QSystem.PDFGeneration;
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
    public class RepairDescriptionsViewModel : ViewModelBase
    {
        private ObservableCollection<VehicleWorkDescription> _vehicleWorkDescription;
        private VehicleWorkOrder _vehicleWorkOrder;
        private ObservableCollection<VehicleRepairDescription> _vehicleRepairDescriptions;
        public event Action Closed;
        private DelegateCommand _closeCommand;
        private ICommand _printCommand;
        private bool canExecute;

        public RepairDescriptionsViewModel(VehicleWorkOrder vwo)
        {
            VehicleWorkOrder = vwo;
             _closeCommand = new DelegateCommand(CloseForm);
             canExecute = true;

             ObservableCollection<VehicleRepairDescription> vehicleRepairDescriptionList = DBAccess.GetVehicleRepairDescriptionByID2(VehicleWorkOrder.VehicleWorkOrderID);
             ObservableCollection<VehicleParts> vehiclePartsList = DBAccess.GetVehiclePartsDescriptionByID(vehicleRepairDescriptionList);
             Int32 id = 0;
             foreach (var item in vehicleRepairDescriptionList)
             {
                 id = item.VehicleWorkDescriptionID;
                 break;
             }

             VehicleWorkDescription = new ObservableCollection<VehicleWorkDescription>();
             //VehicleWorkDescription = DBAccess.GetVehicleWorkDescriptionForRepair(id);

             foreach (var item in vehicleRepairDescriptionList)
             {
                 item.Vehicleparts = new ObservableCollection<VehicleParts>();
                 foreach (var items in vehiclePartsList)
                 {
                     if (item.ID == items.VehicleRepairID)
                     {
                         item.Vehicleparts.Add(items);
                     }
                 }
             }
             if (vehicleRepairDescriptionList.Count > 0 && VehicleWorkDescription.Count > 0)
             {
                 VehicleWorkDescription[0].VehicleRepairDescription = vehicleRepairDescriptionList;
             }
             else
             {
                 VehicleWorkDescription.Add(new VehicleWorkDescription() { Description = "Repair order for " + VehicleWorkOrder.Vehicle.SerialNumber, VehicleRepairDescription = vehicleRepairDescriptionList });
             }

             VehicleRepairDescriptions = vehicleRepairDescriptionList;
             VehicleRepairDescriptions.Add(new VehicleRepairDescription() { StrSequenceNumber = "Select" });

             Int32 res = DBAccess.UpdateVehicleWorkOrderViewed(VehicleWorkOrder.VehicleWorkOrderID, 0);

        }

        private void PrintDescription()
        {
            Exception exception = null;
            BackgroundWorker worker = new BackgroundWorker();
            ChildWindowView LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Printing");

            worker.DoWork += (_, __) =>
            {
                RepairVehicleWorkOrderPDF vwopdf = new RepairVehicleWorkOrderPDF(VehicleWorkOrder, VehicleWorkDescription);
                exception = vwopdf.createWorkOrderPDF();
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

        public ObservableCollection<VehicleWorkDescription> VehicleWorkDescription
        {
            get
            {
                return _vehicleWorkDescription;
            }
            set
            {
                _vehicleWorkDescription = value;
                RaisePropertyChanged(() => this.VehicleWorkDescription);
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

        public ObservableCollection<VehicleRepairDescription> VehicleRepairDescriptions
        {
            get
            {
                return _vehicleRepairDescriptions;
            }
            set
            {
                _vehicleRepairDescriptions = value;
                RaisePropertyChanged(() => this.VehicleRepairDescriptions);
            }
        }        


        private bool CanExecute(object parameter)
        {
            return true;
        }

        private void CloseForm()
        {
            FinalClose();
        }

        private void FinalClose()
        {
            if (Closed != null)
            {                
                Closed();
            }
        }

        #region COMMANDS

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
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
