
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.SlitingPeeling;
using A1QSystem.Model.Products;
using A1QSystem.Model.Shifts;
using A1QSystem.View;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;

namespace A1QSystem.ViewModel.Productions.SlitPeel
{
    public class ShiftSlitPeelViewModel : ViewModelBase
    {
        public event Action Closed;
        private DelegateCommand _closeCommand;
        private DelegateCommand _shiftOrderCommand;

        public string ProductCode { get; set; }
        public SlitPeelSchedule SlitPeelSchedule { get; set; }
        private decimal _qty;
        public int SelectedShift { get; set; }
        public string Type { get; set; }
        public List<Shift> ShiftList { get; set; }
        public DateTime CurrentDate { get; set; }
        public DateTime SelectedDate { get; set; }
        public Int32 SelectedProdTimeTableID { get; set; }

        private ChildWindowView LoadingScreen;

        public ShiftSlitPeelViewModel(SlitPeelSchedule slitPeelSchedule)
        {
            SlitPeelSchedule = slitPeelSchedule;
            Qty = slitPeelSchedule.RawQty;
            ProductCode = slitPeelSchedule.Product.ProductCode;
            Type = GetType(slitPeelSchedule.Product.ProductUnit);
            _closeCommand = new DelegateCommand(CloseForm);
            _shiftOrderCommand = new DelegateCommand(ShiftOrder);

            CurrentDate = Convert.ToDateTime(NTPServer.GetNetworkTime().ToString("dd/MM/yyyy"));
            SelectedDate = Convert.ToDateTime(slitPeelSchedule.SlitPeel.ProductionDate);

            SelectedShift = 1;
            LoadShiftData();

        }
        private void LoadShiftData()
        {
            ShiftList = DBAccess.GetAllShifts();
        }

        public void ShiftOrder()
        {
            if (SelectedShift == 0)
            {
                Msg.Show("Please select a shift to move ", "Select Shift", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (SelectedShift == SlitPeelSchedule.SlitPeel.Shift && SelectedDate == Convert.ToDateTime(SlitPeelSchedule.SlitPeel.ProductionDate))
            {
                Msg.Show("Cannot shift order to " + GetShiftNameByID(SelectedShift.ToString()) + " shift. Please select a different shift", "Select A Different Shift", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                Qty = SlitPeelSchedule.RawQty;
            }
            else if (Qty > SlitPeelSchedule.RawQty)
            {
                Msg.Show("Quantity must be less than or equal to " + SlitPeelSchedule.RawQty, "Enter Correct Quantity", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
               
            }
            else if (Qty <= 0)
            {
                Msg.Show("Invalid quantity ", "Enter Correct Quantity", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (SlitPeelSchedule.SlitPeel.OrdertypeID == 1)
            {
                Msg.Show("Cannot shift Urgent Orders ", "Shifting Declined", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else
            {
                BackgroundWorker worker = new BackgroundWorker();
                LoadingScreen = new ChildWindowView();
                LoadingScreen.ShowWaitingScreen("Processing");

                worker.DoWork += (_, __) =>
                {
                    //Get MachineID;
                    int machineId = 0;
                    List<RawProductMachine> rawProductMachine = DBAccess.GetMachineIdByRawProdId(SlitPeelSchedule.Product.RawProduct.RawProductID);
                    foreach (var item in rawProductMachine)
                    {
                        machineId = item.SlitPeelMachineID;
                    }

                    //Get Selected RawProductID
                    List<ProductionTimeTable> prodTimeTable = DBAccess.GetProductionTimeTableByID(machineId, SelectedDate);
                    foreach (var item in prodTimeTable)
                    {
                        SelectedProdTimeTableID = item.ID;
                    }

                    int newSlitPeelID = 0;
                    DBAccess.MoveSlitPeelProduction(this, ref newSlitPeelID);

                    //CheckCapacity
                    decimal dolVal = DBAccess.CheckSlitPeelCapacity(SelectedProdTimeTableID, SelectedShift);
                    decimal maxCap = 0;// DBAccess.GetSlitPeelMaxCapacity(SelectedProdTimeTableID, SelectedShift);
                    bool skip = false;

                    if (dolVal <= maxCap)
                    {
                        skip = true;
                    }
                    else
                    {
                        skip = false;
                    }

                    Console.WriteLine(skip);
                    //Shift order
                    try
                    {
                        //A1ConsoleAutomator.ViewModel.MainSwitcherViewModel avm = new A1ConsoleAutomator.ViewModel.MainSwitcherViewModel("slitpeel");
                        //avm.ChangingShiftForSlitPeel(SelectedDate, SelectedShift, SlitPeelSchedule.SlitPeel.ID, newSlitPeelID, skip, SlitPeelSchedule.SlitPeel.ProdTimeTableID, SlitPeelSchedule.SlitPeel.Shift, SlitPeelSchedule.SlitPeel.SalesOrderID, SlitPeelSchedule.SlitPeel.ProductID, SlitPeelSchedule.SlitPeel.RawProductID);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Error on shifting orders function at Move button in SlitPeel Production Scheduler: " + e);
                    }
                };
                worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                {
                    LoadingScreen.CloseWaitingScreen();
                    CloseForm();
                };
                worker.RunWorkerAsync();
            }
        }

        private string GetShiftNameByID(string shiftId)
        {

            string result = string.Empty;

            switch (shiftId)
            {
                case "1": result = "Day";
                    break;
                case "2": result = "Afternoon";
                    break;
                case "3": result = "Night";
                    break;
                default: result = "Unspecified";
                    break;
            }

            return result;
        }
        private string GetType(string type)
        {
            string t = string.Empty;
            if(type == "TILE" || type == "EA")
            {
                t = "Block";
            }
            else if (type == "ROLL" || type == "M2")
            {
                t = "Log";
            }
            else
            {
                t = "N/A";
            }
            return t;
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        #region PUBLIC PROPERTIES

        public decimal Qty
        {
            get
            {
                return _qty;
            }
            set
            {
                _qty = value;
                RaisePropertyChanged(() => this.Qty);
            }
        }
#endregion

        #region COMMANDS

        public DelegateCommand ShiftOrderCommand
        {
            get { return _shiftOrderCommand; }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        #endregion
    }
}
