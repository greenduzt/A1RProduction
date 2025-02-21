using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model.Capacity;
using A1QSystem.Model.Formula;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Production;
using A1QSystem.Model.RawMaterials;
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
using System.Collections.ObjectModel;
using A1QSystem.Model.Transaction;
using A1QSystem.Model.Products;
using A1QSystem.Model.Stock;

namespace A1QSystem.ViewModel.Orders
{
    public class ShiftProductionViewModel : ViewModelBase
    {
        public event Action Closed;
        private DelegateCommand _closeCommand;
        private DelegateCommand _shiftOrderCommand;
        public int CurrentProdTimeTableID { get; set; }
        public int CurentShift { get; set; }
        public string RawProductCode { get; set; }
        public string Type { get; set; }
        //public DateTime CurrentDate { get; set; }
        public RawProductionDetails RawProductionDetails { get; set; }
        public Int32 NewProdTimeTableID { get; set; }
        public int OldProductionTimeTableID { get; set; }
        public Transaction Transaction { get; set; }
        private decimal _qty;
        private string originType;
        private string _selectedMove;
        private DateTime _currentDate;
        private DateTime _selectedDate;
        private DateTime _selectedMixingDate;
        private List<Shift> _shiftList;
        private int _selectedShift;
        private int _tabIndex;
        private bool _tab2Enabled;
        private string _selectedMixingShift;
        private List<RawProductMachine> rpm;
      
        private ChildWindowView LoadingScreen;
        public List<ProductionTimeTable> shiftList { get; set; }
        //public List<Shift> shiftDetails { get; set; }
        public ObservableCollection<ProductCapacity> CapacityLimitations { get; set; }


        public ShiftProductionViewModel(RawProductionDetails rawProductionDetails, string type)
        {
            RawProductionDetails = rawProductionDetails;
            Qty = RawProductionDetails.BlockLogQty;
            RawProductCode = RawProductionDetails.RawProduct.RawProductCode;
            Type = RawProductionDetails.RawProduct.RawProductType;
            _closeCommand = new DelegateCommand(CloseForm);
            _shiftOrderCommand = new DelegateCommand(ShiftOrder);
            CurrentDate = Convert.ToDateTime(NTPServer.GetNetworkTime().ToString("dd/MM/yyyy"));
            SelectedDate = Convert.ToDateTime(rawProductionDetails.ProductionDate);
            originType = type;
            CapacityLimitations = new ObservableCollection<ProductCapacity>();
            SelectedMove = "Select";

            SelectedMixingDate = rawProductionDetails.MixingDate;
            SelectedMixingShift = rawProductionDetails.MixingShift;

            LoadShiftData();

            if (Convert.ToDateTime(rawProductionDetails.ProductionDate) == CurrentDate && CurentShift == rawProductionDetails.Shift)
            {
                Tab2Enabled = true;
            }
            else
            {
                Tab2Enabled = false;
            }

        }
      
        private void LoadShiftData()
        {
            rpm = new List<RawProductMachine>();
            rpm=DBAccess.GetMachineByRawProductID(RawProductionDetails.RawProduct.RawProductID);
            ShiftIProcessor si = new ShiftIProcessor(SelectedDate, rpm[0].GradingMachineID);
            CurentShift = si.GetCurrentShift();
            CurrentProdTimeTableID = si.GetCurrentProdTimeTableID();
            ShiftList = si.GetShiftDetails();
            SelectedShift = 0;
        }

        public void ShiftOrder()
        {
            if (TabIndex == 0)//Option1
            {
                if (SelectedShift == 0 && (SelectedMixingShift.Equals(RawProductionDetails.MixingShift) && SelectedMixingDate.Date == Convert.ToDateTime(RawProductionDetails.MixingDate).Date))
                {
                    Msg.Show("Please select a date/shift to move or change mixing date or mixing shift", "Select Date/Shift", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                }
                else if (SelectedShift == RawProductionDetails.Shift && SelectedDate == Convert.ToDateTime(RawProductionDetails.ProductionDate))
                {
                    Msg.Show("Cannot shift order to " + GetShiftNameByID(SelectedShift.ToString()) + " shift. Please select a different shift", "Select A Different Shift", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                    Qty = RawProductionDetails.BlockLogQty;
                }
                else
                {
                    ShiftOption1();
                }
            }
            else if (TabIndex == 1)//Option2
            {
                if(SelectedMove == "Select")
                {
                    Msg.Show("Select Urgent to move this order to the top of the shift", "Selection Required", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);                    
                }
                else
                {
                    ShiftOption2();
                }             
            }
        }
        private void ShiftOption1()
        {
            int res = 0;
            int prodCap1 = 0;
            int prodCap2 = 0;
            decimal curCapacity1 = 0;
            decimal curCapacity2 = 0;
            List<Formulas> formulaDetails = new List<Formulas>();
            List<ProductionTimeTable> prodTimeTable = new List<ProductionTimeTable>();
            List<CurrentCapacity> newCurrCap = new List<CurrentCapacity>();

            if (Qty > RawProductionDetails.BlockLogQty)
            {
                Msg.Show("Quantity must be less than or equal to " + RawProductionDetails.BlockLogQty, "Enter Correct Quantity", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (Qty <= 0)
            {
                Msg.Show("Invalid quantity ", "Enter Correct Quantity", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (RawProductionDetails.OrderType == 1)
            {
                Msg.Show("Cannot shift Urgent Orders ", "Shifting Declined", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }          
            else
            {               
                bool validate = CheckCapacity(RawProductionDetails.RawProduct.RawProductID, RawProductionDetails.OrderType);
                if (validate)
                {           
        
                    //if(SelectedDate.Date != Convert.ToDateTime(RawProductionDetails.ProductionDate).Date)
                    //{
                    //   DateTime mixingDate = SelectedMixingDate.Date;
                    //   BusinessDaysGenerator bdg = new BusinessDaysGenerator();
                    //   mixingDate = SelectedDate.AddDays(1);
                    //   mixingDate = bdg.SkipWeekends(mixingDate);

                    //   if(Msg.Show("Also, would you like to change the mixing date to " + mixingDate.ToString("dd/MM/yyyy") + "?", "", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                    //   {
                    //       SelectedMixingDate = mixingDate;
                    //   }
                    //}
                  
                    BackgroundWorker worker = new BackgroundWorker();
                    LoadingScreen = new ChildWindowView();
                    LoadingScreen.ShowWaitingScreen("Processing");

                    worker.DoWork += (_, __) =>
                    {                        

                        bool skip = false;
                        //GETTING THE SELECTED TIME TABLE ID
                        prodTimeTable = DBAccess.GetProductionTimeTableByID(rpm[0].GradingMachineID, SelectedDate);
                        if (prodTimeTable.Count != 0)
                        {
                            foreach (var item in prodTimeTable)
                            {
                                if (item.MachineID == rpm[0].GradingMachineID)
                                {
                                    NewProdTimeTableID = item.ID;
                                    break;
                                }
                            }

                            /*************************************************************************************************/
                            //GETTING FORMULA DETAILS
                            formulaDetails = DBAccess.GetFormulaDetailsByRawProdID(RawProductionDetails.RawProduct.RawProductID);
                            if (formulaDetails.Count != 0)
                            {
                                List<int> gradingList = new List<int>();
                                foreach (var item in formulaDetails)
                                {
                                    decimal kgToMake1 = 0;
                                    decimal kgToMake2 = 0;

                                    if (item.GradingWeight1 != 0)
                                    {
                                        prodCap1 = item.ProductCapacity1;
                                        curCapacity1 = DBAccess.CheckCapacityByDateShift(NewProdTimeTableID, prodCap1, SelectedShift, RawProductionDetails.OrderType);//GETTING CURRENT CAPACITY DETAILS FOR PRODUCT CAPACITY1
                                        kgToMake1 = (item.GradingWeight1 * Qty) + curCapacity1;
                                        gradingList.Add(prodCap1);
                                    }
                                    if (item.GradingWeight2 != 0)
                                    {
                                        prodCap2 = item.ProductCapacity2;
                                        curCapacity2 = DBAccess.CheckCapacityByDateShift(NewProdTimeTableID, prodCap2, SelectedShift, RawProductionDetails.OrderType);//GETTING CURRENT CAPACITY DETAILS FOR PRODUCT CAPACITY2
                                        kgToMake2 = (item.GradingWeight2 * Qty) + curCapacity2;
                                        gradingList.Add(prodCap2);
                                    }

                                    //GETTING MAXIMUM CAPACITY DETAILS FROM PRODUCT CAPACITY
                                    List<ProductCapacity> maxCapList = DBAccess.GetCapacityLimitationsById(NewProdTimeTableID, gradingList, SelectedShift);

                                    decimal remainder1 = 0;
                                    decimal remainder2 = 0;

                                    foreach (var itemM in maxCapList)
                                    {
                                        if (itemM.RubberGradingID == prodCap1 && itemM.CapacityKG <= kgToMake1)
                                        {
                                            remainder1 = itemM.CapacityKG - kgToMake1;
                                        }

                                        if (itemM.RubberGradingID == prodCap2 && itemM.CapacityKG <= kgToMake2 && prodCap1 != 0 && prodCap2 != 0)
                                        {
                                            remainder2 = itemM.CapacityKG - kgToMake2;
                                        }
                                    }
                                    if (remainder1 <= kgToMake1 && remainder2 == 0 && kgToMake2 == 0)
                                    {
                                        skip = true;
                                    }
                                    else if (remainder1 <= kgToMake1 && remainder2 <= kgToMake2 && kgToMake2 != 0 && remainder2 < 0)
                                    {
                                        skip = true;
                                    }
                                    else
                                    {
                                        skip = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("RAW PRODUCT ID NOT AVAILABLE!!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("DID NOT FIND THE SELECTED DATE!!!");
                        }                        

                        //Move order
                        int newGradingSchedulingID = 0;
                        res = DBAccess.MoveGradingOrder(this, ref newGradingSchedulingID,SelectedMixingDate,SelectedMixingShift);                                
                                
                    };

                    worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                    {
                        LoadingScreen.CloseWaitingScreen();
                        CloseForm();
                        string transType = string.Empty;
                        if(res==-1)
                        {
                            Msg.Show("Could not move this item. Please try again", "Could Not Move Item", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                            transType = "Move failed";
                        }
                        else if (res == 1)
                        {
                            transType = "Moved";
                        }

                        if (res == -1 || res == 1)
                        {
                            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                            if (String.IsNullOrEmpty(userName))
                            {
                                userName = "Unknown";
                            }
                            Transaction = new Transaction()
                            {
                                TransDateTime = DateTime.Now,
                                Transtype = transType,
                                SalesOrderID = RawProductionDetails.SalesOrderId,
                                Products = new List<RawStock>()
                                {
                                    new RawStock(){RawProductID = RawProductionDetails.RawProduct.RawProductID,Qty=Qty},  
                                },
                                CreatedBy = userName
                            };
                            int r = DBAccess.InsertTransaction(Transaction);
                        }
                    };
                    worker.RunWorkerAsync();                    
                }
            }
        }

        private void ShiftOption2()
        {
            if (Qty > RawProductionDetails.BlockLogQty)
            {
                Msg.Show("Quantity must be less than or equal to " + RawProductionDetails.BlockLogQty, "Enter Correct Quantity", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (Qty <= 0)
            {
                Msg.Show("Invalid quantity ", "Enter Correct Quantity", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (RawProductionDetails.OrderType == 1)
            {
                Msg.Show("Cannot shift Urgent Orders ", "Shifting Declined", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (RawProductionDetails.OrderType == 2 || RawProductionDetails.OrderType == 4)
            {
                Msg.Show("Cannot shift graded Orders ", "Shifting Declined", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (RawProductionDetails.ActiveOrder == true)
            {
                Msg.Show("This order is being currently processed and cannot be shifted ", "Shifting Declined", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else
            {
                int res = DBAccess.MoveOrderUrgent(this);
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                if (String.IsNullOrEmpty(userName))
                {
                    userName = "Unknown";
                }
                if(res == 0)
                {
                    Msg.Show("There was a problem when shifting this order ", "Shifting Failed", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                    int val = DBAccess.InsertErrorLog(DateTime.Now + " Error - Shifting Order Urgent | User: " + userName + " | Sales Id: " + RawProductionDetails.SalesOrderId + " | Raw Product ID: " + RawProductionDetails.RawProduct.RawProductID + " | Qty: " + Qty);
                }
                else if(res == 1)
                {
                    
                    Transaction = new Transaction()
                    {
                        TransDateTime = DateTime.Now,
                        Transtype = "Moved Up",
                        SalesOrderID = RawProductionDetails.SalesOrderId,
                        Products = new List<RawStock>()
                                    {
                                      new RawStock(){RawProductID = RawProductionDetails.RawProduct.RawProductID,Qty=Qty},  
                                    },
                        CreatedBy = userName
                    };
                    int r = DBAccess.InsertTransaction(Transaction);
                }                

                CloseForm();
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

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        private bool CheckCapacity(int rawProductId, int orderType)
        {
            bool validate = true;
            bool res1 = true;
            bool res2 = true;
            bool twoGrading = false;
            string errMsg = "";

            if (SelectedDate.Date != Convert.ToDateTime(RawProductionDetails.ProductionDate).Date || (SelectedShift != 0 && SelectedShift != RawProductionDetails.Shift))
            {
                //Get GradingMachine ID
                List<RawProductMachine> rpM = DBAccess.GetMachineByRawProductID(rawProductId);

                if (rawProductId != 12 && orderType != 4 && orderType != 2)
                {
                    List<Formulas> fList = DBAccess.GetFormulaDetailsByRawProdID(rawProductId);
                    if (fList.Count > 0)
                    {
                        if (fList[0].ProductCapacity1 > 0 && fList[0].ProductCapacity2 > 0)
                        {
                            twoGrading = true;
                        }

                        res1 = CheckCapacityByDateAndShift(SelectedDate, SelectedShift, fList[0].ProductCapacity1, fList[0].GradingWeight1, orderType, ref errMsg, rpM[0].GradingMachineID);
                        if (res1)
                        {
                            if (twoGrading)
                            {
                                res2 = CheckCapacityByDateAndShift(SelectedDate, SelectedShift, fList[0].ProductCapacity2, fList[0].GradingWeight2, orderType, ref errMsg, rpM[0].GradingMachineID);
                                if (res2)
                                {
                                }
                                else
                                {
                                    validate = false;
                                    Msg.Show(errMsg, "Choose Date", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                                }
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                            validate = false;

                            Msg.Show(errMsg, "Choose Date", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                        }
                    }
                }
            }
            else if (SelectedMixingDate.Date != RawProductionDetails.MixingDate.Date || SelectedMixingShift != RawProductionDetails.MixingShift)
            {
                validate = true;
            }

            return validate;
        }

        private bool CheckCapacityByDateAndShift(DateTime IDate, int shift, int prodCapId, decimal gradingWeight, int orderTypeId, ref string message,int gradingMachineId)
        {
            //int curShift = 0;
            decimal existingBlkLogs = 0;
            bool result = true;
            decimal capLeft = 0;
            shiftList = new List<ProductionTimeTable>();
            shiftList = DBAccess.GetProductionTimeTableByID(gradingMachineId, IDate);


            List<ProductionTimeTable> pList = DBAccess.GetProductionTimeTableByMachineID(gradingMachineId, IDate);
            int prodTimeTableId = 0;
            foreach (var itemPL in pList)
            {
                prodTimeTableId = itemPL.ID;
            }

            //foreach (var item in shiftList)
            //{
            CapacityLimitations = DBAccess.GetCapacityLimitations(prodTimeTableId);
            // break;
            //}

            if (shiftList.Count != 0)
            {
                foreach (var itemSL in shiftList)
                {
                    capLeft = 0;

                    if (itemSL.MachineID == gradingMachineId)
                    {
                        if (itemSL.IsMachineActive == true)
                        {
                            if (prodCapId != 6 && prodCapId != 7 && prodCapId != 8)
                            {
                                if (itemSL.IsDayShiftActive == true && shift == 1)
                                {
                                    var data = CapacityLimitations.SingleOrDefault(c => c.ProductionTimeTableID == itemSL.ID && c.RubberGradingID == prodCapId && c.Shift == 1);//ProductCapacity

                                    decimal cWeight = DBAccess.CheckCapacityByDateShift(itemSL.ID, prodCapId, 1, orderTypeId);//CurrentCapacity
                                    decimal avaWeight = data.CapacityKG - cWeight;
                                    if (avaWeight > 0)
                                    {
                                        existingBlkLogs = CalNoOfBlocks(avaWeight, gradingWeight);
                                        //decimal maxBlkLogs = CalNoOfBlocks(data.CapacityKG, gradingWeight);

                                        if (existingBlkLogs > 0)
                                        {
                                            capLeft = 1;
                                        }
                                    }
                                }
                                if (itemSL.IsEveningShiftActive == true && shift == 2)
                                {
                                    var data = CapacityLimitations.Single(c => c.ProductionTimeTableID == itemSL.ID && c.RubberGradingID == prodCapId && c.Shift == 2);//ProductCapacity

                                    decimal cWeight = DBAccess.CheckCapacityByDateShift(itemSL.ID, prodCapId, 2, orderTypeId);//CurrentCapacity
                                    decimal avaWeight = data.CapacityKG - cWeight;
                                    if (avaWeight > 0)
                                    {
                                        existingBlkLogs = CalNoOfBlocks(avaWeight, gradingWeight);
                                        //decimal maxBlkLogs = CalNoOfBlocks(data.CapacityKG, gradingWeight);

                                        if (existingBlkLogs > 0)
                                        {
                                            capLeft += 1;
                                        }
                                    }
                                }
                                if (itemSL.IsNightShiftActive == true && shift == 3)
                                {
                                    var data = CapacityLimitations.Single(c => c.ProductionTimeTableID == itemSL.ID && c.RubberGradingID == prodCapId && c.Shift == 3);//ProductCapacity

                                    decimal cWeight = DBAccess.CheckCapacityByDateShift(itemSL.ID, prodCapId, 3, orderTypeId);//CurrentCapacity
                                    decimal avaWeight = data.CapacityKG - cWeight;
                                    if (avaWeight > 0)
                                    {
                                        existingBlkLogs = CalNoOfBlocks(avaWeight, gradingWeight);
                                        //decimal maxBlkLogs = CalNoOfBlocks(data.CapacityKG, gradingWeight);

                                        if (existingBlkLogs > 0)
                                        {
                                            capLeft += 1;
                                        }
                                    }
                                }
                            }

                            if (itemSL.IsDayShiftActive == false && itemSL.IsEveningShiftActive == false && itemSL.IsNightShiftActive == false)
                            {
                                result = false;
                                message = "No shift available on the selected date.  Please choose another date to move the record!";
                                break;
                            }

                            if (prodCapId != 6 && prodCapId != 7 && prodCapId != 8)
                            {
                                if (capLeft <= 0)
                                {
                                    result = false;
                                    message = "Capacity is full on the selected date, please choose another date to move the record!";
                                    break;
                                }
                                else
                                {
                                    if (Qty > existingBlkLogs)
                                    {
                                        result = false;
                                        message = "Capacity is not enough to move " + Qty + " block/log(s) of " + RawProductionDetails.RawProduct.RawProductCode + System.Environment.NewLine + "The maximum " + existingBlkLogs + " block/log(s) can be moved!";
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            result = false;
                            message = "Machine is switched off on the selected date.  Please choose another date to move the record!";
                            //Msg.Show("Machine is switched off on the selected date.  Please choose another date!", "Choose Date", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                            break;
                        }
                    }
                }

            }

            return result;
        }

        private decimal CalNoOfBlocks(decimal amount, decimal gradingWeight)
        {
            decimal blk = 0;

            blk = decimal.Floor(amount / gradingWeight);

            return blk;
        }

        bool TimeBetween(DateTime datetime, TimeSpan start, TimeSpan end)
        {
            // convert datetime to a TimeSpan
            TimeSpan now = datetime.TimeOfDay;
            // see if start comes before end
            if (start < end)
                return start <= now && now <= end;
            // start is after end, so do the inverse comparison
            return !(end < now && now < start);
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

        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                RaisePropertyChanged(() => this.SelectedDate);

                LoadShiftData();
            }
        }

        public DateTime SelectedMixingDate
        {
            get
            {
                return _selectedMixingDate;
            }
            set
            {
                _selectedMixingDate = value;
                RaisePropertyChanged(() => this.SelectedMixingDate);
                
            }
        }

        public DateTime CurrentDate
        {
            get
            {
                return _currentDate;
            }
            set
            {
                _currentDate = value;
                RaisePropertyChanged(() => this.CurrentDate);
            }
        }

        public List<Shift> ShiftList
        {
            get
            {
                return _shiftList;
            }
            set
            {
                _shiftList = value;
                RaisePropertyChanged(() => this.ShiftList);
            }
        }

        public int SelectedShift
        {
            get
            {
                return _selectedShift;
            }
            set
            {
                _selectedShift = value;
                RaisePropertyChanged(() => this.SelectedShift);
            }
        }

        public string SelectedMove
        {
            get
            {
                return _selectedMove;
            }
            set
            {
                _selectedMove = value;
                RaisePropertyChanged(() => this.SelectedMove);
            }
        }

        public int TabIndex
        {
            get
            {
                return _tabIndex;
            }
            set
            {
                _tabIndex = value;
                RaisePropertyChanged(() => this.TabIndex);

                SelectedMove = "Select";
                SelectedDate = Convert.ToDateTime(RawProductionDetails.ProductionDate);
                SelectedShift = 0;
            }
        }
        public bool Tab2Enabled
        {
            get
            {
                return _tab2Enabled;
            }
            set
            {
                _tab2Enabled = value;
                RaisePropertyChanged(() => this.Tab2Enabled);              
            }
        }

        public string SelectedMixingShift
        {
            get
            {
                return _selectedMixingShift;
            }
            set
            {
                _selectedMixingShift = value;
                RaisePropertyChanged(() => this.SelectedMixingShift);              
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
