using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Vehicles;
using A1QSystem.View;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.WorkOrders
{
    public class OdometerAccepatnceViewModel : ViewModelBase
    {
        //private string option;
        private CalculationModel calculation;        
        private List<VehicleWorkOrder> _odometerReadingList;
        private VehicleWorkOrder vehicleWorkOrder;
        private Int64 _odometerReading;
        private string _odometerString;
        private string _odometerReadingStr;
        private bool canExecute;
        private string lastOperation;
        private string textboxType;
        private bool newDisplayRequired = false;
        public event Action<Int64> Closed;
        private DelegateCommand _closeCommand;
        private ICommand _submitCommand;
        private DelegateCommand<string> _digitButtonPressCommand;

        public OdometerAccepatnceViewModel(VehicleWorkOrder vwo)
        {
            vehicleWorkOrder = vwo;
            OdometerReadingList = new List<VehicleWorkOrder>();
            OdometerReadingList = DBAccess.GetVehicleOdometerReadings(vehicleWorkOrder.Vehicle.ID);
            if (OdometerReadingList.Count > 0)
            {
                OdometerReadingList = OdometerReadingList.OrderBy(x=>x.CompletedDate).ToList();
            }

            OdometerString = GetOdometerReadingTitle(vehicleWorkOrder.Vehicle.VehicleBrand);
            OdometerReading=DBAccess.GetVehicleOdometerReading(vehicleWorkOrder.VehicleWorkOrderID);
            if (OdometerReading > 0)
            {
                OdometerReadingStr=OdometerReading.ToString();
            }
            _closeCommand = new DelegateCommand(CloseForm);
            canExecute = true;

            this.calculation = new CalculationModel();
            this.FirstOperand = string.Empty;
            this.SecondOperand = string.Empty;
            this.Operation = string.Empty;
            this.lastOperation = string.Empty;
            this.fullExpression = string.Empty;
        }

        private void SubmitDetails()
        {
            //CloseFormNow(OdometerReading);
            if (OdometerReading > 0 && OdometerReading >= vehicleWorkOrder.LastOdometerReading)
            {                vehicleWorkOrder.OdometerReading = OdometerReading;
                
                ChildWindowView myChildWindow = new ChildWindowView();
                myChildWindow.ShowVehicleMaintenanceDescription(vehicleWorkOrder);
                myChildWindow.viewVehicleMaintenanceDesc_Closed += (r =>
                {

                });                    
            } 
            else
            {
                Msg.Show("Odometer reading has to be greater than or equal to " + vehicleWorkOrder.LastOdometerReading + GetOdometerReadingTitle(vehicleWorkOrder.Vehicle.VehicleBrand), "Valid Odometer Reading Required", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
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


        //deals with button inputs and sorts out the display accordingly
        public void DigitButtonPress(string button)
        {
            switch (button)
            {
                case "C": 
                    OdometerReadingStr = "0";
                    FirstOperand = string.Empty;
                    SecondOperand = string.Empty;
                    Operation = string.Empty;
                    LastOperation = string.Empty;
                    FullExpression = string.Empty;
                    break;
                case "Del":
                    if (_odometerReadingStr.Length > 1)
                        OdometerReadingStr = _odometerReadingStr.Substring(0, _odometerReadingStr.Length - 1);
                    else OdometerReadingStr = "0";
                    break;
                case "+/-":
                    if (_odometerReadingStr.Contains("-") || _odometerReadingStr == "0")
                    {
                        OdometerReadingStr = _odometerReadingStr.Remove(_odometerReadingStr.IndexOf("-"), 1);
                    }
                    else OdometerReadingStr = "-" + _odometerReadingStr;
                    break;
                case ".":
                    if (newDisplayRequired)
                    {
                        OdometerReadingStr = "0.";
                    }
                    else
                    {
                        if (!_odometerReadingStr.Contains("."))
                        {
                            OdometerReadingStr = _odometerReadingStr + ".";
                        }
                    }
                    break;
                default:
                    if (_odometerReadingStr == "0" || newDisplayRequired)
                        OdometerReadingStr = button;
                    else
                        OdometerReadingStr = _odometerReadingStr + button;
                    break;
            }
            newDisplayRequired = false;
        }


        private void CloseForm()
        {
            CloseFormNow(0);
        }

        private void CloseFormNow(Int64 n)
        {
            if (Closed != null)
            {
                Closed(n);
            }
        }

        public string FirstOperand
        {
            get { return calculation.FirstOperand; }
            set { calculation.FirstOperand = value; }
        }

        public string SecondOperand
        {
            get { return calculation.SecondOperand; }
            set { calculation.SecondOperand = value; }
        }


        public string Operation
        {
            get { return calculation.Operation; }
            set { calculation.Operation = value; }
        }

        public string LastOperation
        {
            get { return lastOperation; }
            set { lastOperation = value; }
        }

        private string fullExpression;
        public string FullExpression
        {
            get { return fullExpression; }
            set
            {
                fullExpression = value;
                RaisePropertyChanged(() => this.FullExpression);
            }
        }

        public List<VehicleWorkOrder> OdometerReadingList
        {
            get
            {
                return _odometerReadingList;
            }
            set
            {
                _odometerReadingList = value;
                RaisePropertyChanged(() => this.OdometerReadingList);
            }
        }

        public Int64 OdometerReading
        {
            get
            {
                return _odometerReading;
            }
            set
            {
                _odometerReading = value;
                RaisePropertyChanged(() => this.OdometerReading);
            }
        }

        public string OdometerReadingStr
        {
            get
            {
                return _odometerReadingStr;
            }
            set
            {
                _odometerReadingStr = value;
                RaisePropertyChanged(() => this.OdometerReadingStr);
                if (!string.IsNullOrWhiteSpace(OdometerReadingStr))
                {
                    OdometerReading = Convert.ToInt64(OdometerReadingStr);
                }
            }
        }

        public string OdometerString
        {
            get
            {
                return _odometerString;
            }
            set
            {
                _odometerString = value;
                RaisePropertyChanged(() => this.OdometerString);               
                
            }
        }       

        private static bool CanDigitButtonPress(string button)
        {
            return true;
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public ICommand SubmitCommand
        {
            get
            {
                return _submitCommand ?? (_submitCommand = new A1QSystem.Commands.LogOutCommandHandler(() => SubmitDetails(), canExecute));
            }
        }

        public ICommand DigitButtonPressCommand
        {
            get
            {
                if (_digitButtonPressCommand == null)
                {
                    _digitButtonPressCommand = new DelegateCommand<string>(
                        DigitButtonPress, CanDigitButtonPress);
                }
                return _digitButtonPressCommand;
            }
        }
    }
}
