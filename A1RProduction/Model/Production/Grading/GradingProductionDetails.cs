using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model.Capacity;
using A1QSystem.Model.Formula;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Other;
using A1QSystem.Model.Production.Grading;
using A1QSystem.Model.Production.Mixing;
using A1QSystem.Model.RawMaterials;
using A1QSystem.Model.Shifts;
using A1QSystem.Model.Shreding;
using A1QSystem.Model.Stock;
using A1QSystem.Model.Transaction;
using A1QSystem.View;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace A1QSystem.Model.Production.Grading
{
    public class GradingProductionDetails : RawProductionDetails
    {
        private List<FormulaOptions> _formulaOptions;
        //public List<FormulaOptions> FormulaOptions { get; set; }
        public string GradingFormula { get; set; }
        public string GradingStatus { get; set; }
        private List<ShredStock> shredStock;
        private ICommand _completedCommand;
        private ICommand _printCommand;
        private ICommand _viewCommand;

        private bool canExecute;
        public Int32 MixingProdDetID { get; set; }
        public List<Machines> MachinesList { get; set; }
        public bool SysParameter { get; set; }
        private ChildWindowView LoadingScreen;
        private bool _gradingActive;
        private string _repeatAnimation;
        private string _isReqDateVisible;
        private int _currentShift;
        private int _currentProdTimeTable;
        private int _printCounter;
        private string _formulaOptionVisibility;
        private string _gradeString;

        public GradingProductionDetails()
        {
            canExecute = true;
            RepeatAnimation = "0x";
            _currentProdTimeTable = GetProdTimeTableID();
            IsReqDateVisible = "Collapsed";
            shredStock = new List<ShredStock>();
            shredStock = DBAccess.GetShredStock();
        }    

        #region PUBLIC_PROPERTIES        

        public bool GradingActive
        {
            get { return _gradingActive; }
            set
            {
                _gradingActive = value;
                RaisePropertyChanged(() => this.GradingActive);
                if(GradingActive == true)
                {
                    RepeatAnimation = "Forever";
                }
                else
                {
                    RepeatAnimation = "0x";
                }
            }
        }



        public string GradeString
        {
            get { return _gradeString; }
            set
            {
                _gradeString = value;
                RaisePropertyChanged(() => this.GradeString);
             
            }
        }

        public List<FormulaOptions> FormulaOptions
        {
            get { return _formulaOptions; }
            set
            {
                _formulaOptions = value;
                RaisePropertyChanged(() => this.FormulaOptions);
             
            }
        }

        public string RepeatAnimation
        {
            get { return _repeatAnimation; }
            set
            {
                _repeatAnimation = value;
                RaisePropertyChanged(() => this.RepeatAnimation);
            }
        }

        public int PrintCounter
        {
            get { return _printCounter; }
            set
            {
                _printCounter = value;
                RaisePropertyChanged(() => this.PrintCounter);
            }
        }

        public string IsReqDateVisible
        {
            get { return _isReqDateVisible; }
            set
            {
                _isReqDateVisible = value;
                RaisePropertyChanged(() => this.IsReqDateVisible);
            }
        }

        public string FormulaOptionVisibility
        {
            get { return _formulaOptionVisibility; }
            set
            {
                _formulaOptionVisibility = value;
                RaisePropertyChanged(() => this.FormulaOptionVisibility);
            }
        }

        


#endregion
        



        private void OrderValidion()
        {
            List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
            bool has = systemParameters.Any(x => x.Value == true);
            if (has == true)
            {
                Msg.Show("System is processing some orders at the moment. Please try again in few minutes ", "System Busy", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                int sp1 = DBAccess.UpdateSystemParameter("CompleteGradingOrder", true);
                if (sp1 > 0)
                {

                    var owner = new Form { TopMost = true };
                    Task.Delay(10000).ContinueWith(t => {
                        owner.Invoke(new Action(() =>
                        {
                            if (!owner.IsDisposed)
                            {
                                owner.Close();
                            }
                        }));
                    });
                    var dialogRes = MessageBox.Show(owner, 
                        "Have you completed 1 " + RawProduct.RawProductType + " of " + RawProduct.Description + "?" + System.Environment.NewLine + System.Environment.NewLine + "Click yes if you have only completed 1 " + RawProduct.RawProductType + " of " + RawProduct.Description,
                        "Production Complete Confirmation", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Question);

                        if (dialogRes.ToString() == "Yes")
                        {
                            CompleteOrder();
                        }
                    
                    int sp2 = DBAccess.UpdateSystemParameter("CompleteGradingOrder", false);
                }         
            }
        }

        private void CompleteOrder()
        {
            int result = 0;
            BackgroundWorker worker = new BackgroundWorker();
            LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Processing");

            worker.DoWork += (_, __) =>
            {
                MixingProdDetID = 0;
                int curProdTimeTableId = GetProdTimeTableID();
                string pcName = System.Environment.MachineName;
                if (string.IsNullOrEmpty(pcName))
                {
                    pcName = "Unknown";
                }

                List<Formulas> fList = DBAccess.GetFormulaDetailsByRawProdID(RawProduct.RawProductID);                

                foreach (var item in fList)
                {
                    List<ProductionTimeTable> pttList = DBAccess.GetProductionTimeTableByID(item.MachineID, Convert.ToDateTime(ProductionDate));
                    foreach (var items in pttList)
                    {
                        MixingProdDetID = items.ID;
                    }
                }
                //Get the current shhift
                List<Shift> ShiftDetails = DBAccess.GetAllShifts();
                foreach (var item in ShiftDetails)
                {
                    bool isShift = TimeBetween(DateTime.Now, item.StartTime, item.EndTime);

                    if (isShift == true)
                    {
                        _currentShift = item.ShiftID;
                    }
                }

                OrderStatus ordeStatus = OrderStatus.None;
                ordeStatus = OrderStatus.Grading;
                ObservableCollection<RawProductionDetails> coll = null;
                coll = DBAccess.GetAllRawProductsByID(this, ordeStatus);

                if (coll.Count != 0)
                {
                    MachinesList = DBAccess.GetNumberOfMachines();
                    List<MixingOrder> mixingOrdersList = new List<MixingOrder>();

                    foreach (var x in coll)
                    {
                        if (x.BlockLogQty >= 1)
                        {
                            MixingOrder mol = new MixingOrder();
                            mol.Product = new Product() { ProductID = 0, ProductCode = string.Empty, RawProduct = new Products.RawProduct() { RawProductID = RawProduct.RawProductID }, UnitPrice = 0, ProductUnit = RawProduct.RawProductType };
                            mol.Qty = 0;
                            mol.Order = new Order() { OrderType = OrderType,OrderNo = SalesOrderId,RequiredDate = Convert.ToDateTime(RequiredDate),SalesNo = SalesOrder,Comments=Notes,Customer=Customer};
                            mol.BlocksLogs = 1;
                            mixingOrdersList.Add(mol);
                            if (mixingOrdersList.Count != 0)
                            {
                                Tuple<List<GradingCompleted>, Int32, DateTime> tupleElements = CalculateGradingRubber(this.RawProduct.RawProductID, _currentShift, ShiftDetails);

                                if (this.RawProduct.RawProductType != "Box" && this.RawProduct.RawProductType != "BoxPallet")
                                {
                                    //Deduct grading and add to mixing  
                                    result = ProductionManager.AddToMixing(mixingOrdersList, this, x.OrderType, _currentShift, _currentProdTimeTable, tupleElements.Item1, pcName, tupleElements.Item2, tupleElements.Item3, false);                                  
                                }
                                else if (this.RawProduct.RawProductType == "BoxPallet")//Update PendingOrder-TODO                      
                                {
                                    result = DBAccess.UpdateRawProdQty(this, x.OrderType, _currentShift, _currentProdTimeTable, tupleElements.Item1, pcName, tupleElements.Item2, tupleElements.Item3);
                                }
                                else if (this.RawProduct.RawProductType == "Box")//Add to box stock
                                {
                                    List<GradedStock> gsl = new List<GradedStock>();
                                    gsl.Add(new GradedStock() { ID = tupleElements.Item1[0].GradingID, Qty = tupleElements.Item1[0].KGCompleted });
                                    ProductionManager.AddToGradedStock(gsl);//Add to graded stock   

                                    result = DBAccess.UpdateRawProdQty(this, x.OrderType, _currentShift, _currentProdTimeTable, tupleElements.Item1, pcName, tupleElements.Item2, tupleElements.Item3);
                                }                                
                                else
                                {
                                    Console.WriteLine("Didn't add to mixing");
                                }                                
                            }
                            else
                            {
                                Console.WriteLine("MixingOrdersList is empty grading complete button");
                            }
                            break;
                        }
                    }
                }
            };
            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();

                if(result == 0)
                {
                    Msg.Show("Could not complete order " + System.Environment.NewLine + "Please try again later", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
                }
                else if (result == -1)
                {
                    Msg.Show("Could not complete grading order " + System.Environment.NewLine + "Please try again later", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
                }
                else if (result == -2)
                {
                    Msg.Show("Grading order was successful but could not send it to mixing." + System.Environment.NewLine + "Reverting the order back to Grading. Please try again later", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
                }
            };
            worker.RunWorkerAsync();
        }
        private Tuple<List<GradingCompleted>,Int32,DateTime> CalculateGradingRubber(int rawProdId, int curShift,List<Shift> shiftDetails)
        {
            Int32 actid = 0;
            List<GradingCompleted> ggCompleted = new List<GradingCompleted>();
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            DateTime realDate = DateTime.Now;          

            if(curShift == 3)//checking if still the night shift, if so goes to the previous working day
            { 
                TimeSpan midNight = new TimeSpan(00,00,00);
                TimeSpan nightEndTime = shiftDetails[2].EndTime;
                TimeSpan curTime = DateTime.Now.TimeOfDay;

                if ((curTime >= midNight) && (curTime < nightEndTime))
                {
                    DateTime dt = bdg.SubstractBusinessDays(DateTime.Now, 1);
                    List<ProductionTimeTable> prodTT = DBAccess.GetProductionTimeTableByID(1, dt);
                    if (prodTT.Count != 0)
                    {
                        actid = prodTT[0].ID;
                        realDate = dt;
                    }
                }
                else
                {
                    actid = _currentProdTimeTable;
                }
            }
            else
            {
                actid = _currentProdTimeTable;
            }
            //List<ShredStock> tempShredStock = new List<ShredStock>(); 
            List<Formulas> fList = DBAccess.GetFormulaDetailsByRawProdID(rawProdId);
            if (fList.Count > 0)
            {
                if(fList[0].ProductCapacity1 != 0 && fList[0].ProductCapacity2 == 0)
                {
                    ggCompleted.Add(new GradingCompleted() { GradingID = fList[0].ProductCapacity1, KGCompleted = fList[0].GradingWeight1, CreatedDate = realDate, ProdTimeTableID = actid, Shift = curShift, SalesID = SalesOrderId, RawProduct = new Product() { RawProduct = new Products.RawProduct() { RawProductID=RawProduct.RawProductID} },OrderType=OrderType });
                }
                else if (fList[0].ProductCapacity1 != 0 && fList[0].ProductCapacity2 != 0)
                {
                    ggCompleted.Add(new GradingCompleted() { GradingID = fList[0].ProductCapacity1, KGCompleted = fList[0].GradingWeight1, CreatedDate = realDate, ProdTimeTableID = actid, Shift = curShift, SalesID = SalesOrderId, RawProduct = new Product() { RawProduct = new Products.RawProduct() { RawProductID = RawProduct.RawProductID } }, OrderType = OrderType });
                    ggCompleted.Add(new GradingCompleted() { GradingID = fList[0].ProductCapacity2, KGCompleted = fList[0].GradingWeight2, CreatedDate = realDate, ProdTimeTableID = actid, Shift = curShift, SalesID = SalesOrderId, RawProduct = new Product() { RawProduct = new Products.RawProduct() { RawProductID = RawProduct.RawProductID } }, OrderType = OrderType });
                }               
            }

            return Tuple.Create(ggCompleted, actid, realDate);
        }

       

        private int GetProdTimeTableID()
        {
            int id = 0;
            DateTime curDate = DateTime.Now.Date;
            List<ProductionTimeTable> prodTT = DBAccess.GetProductionTimeTableByID(1,curDate);
            if (prodTT.Count != 0)
            {
                id = prodTT[0].ID;
            }
            
            return id;
        }

        private void PrintGradingFormula()
        {

            if (this.GradingFormula != "Not Available" || this.GradingFormula != null)
            {
                   
                if (Msg.Show("Do you want to PRINT Formula for " + RawProduct.Description + "?", "Formula Printing Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    List<Formulas> mixingFormula = new List<Formulas>();
                    Tuple<string,Exception> tupG=null;
                    Tuple<string, Exception> tupM = null;
                    BackgroundWorker worker = new BackgroundWorker();
                    LoadingScreen = new ChildWindowView();
                    LoadingScreen.ShowWaitingScreen("Printing");
                                                       
                            
                    //Update Print Counter
                    int ugc = DBAccess.UpdateGradingCounter(this);
                    worker.DoWork += (_, __) =>
                    {
                        //Adding Date to the formula
                        //Printing Grading Formula
                        PrintingManager pm = new PrintingManager();
                        tupG = pm.AddDateTime(GradingFormula, "GP" + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".pdf", "I:/PRODUCTION/DONOTDELETE/GradingPrints/");

                        ProcessStartInfo info1 = new ProcessStartInfo(tupG.Item1);
                        info1.Verb = "Print";
                        info1.CreateNoWindow = true;
                        info1.WindowStyle = ProcessWindowStyle.Hidden;
                        Process.Start(info1);

                        //Printing Mixing Formula
                        mixingFormula = DBAccess.GetFormulaDetailsByRawProdID(RawProduct.RawProductID);
                        if (mixingFormula != null && mixingFormula.Count > 0)
                        {
                            tupM = pm.AddDateTime(mixingFormula[0].MixingFormula, "MP" + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".pdf", "I:/PRODUCTION/DONOTDELETE/MixingPrints/");

                            ProcessStartInfo info2 = new ProcessStartInfo(tupM.Item1);
                            info2.Verb = "Print";
                            info2.CreateNoWindow = true;
                            info2.WindowStyle = ProcessWindowStyle.Hidden;
                            Process.Start(info2);
                        }
                    };

                    worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                    {
                        LoadingScreen.CloseWaitingScreen();
                        if (tupG.Item2 != null)
                        {
                            Msg.Show("A problem has occured while formula is prining. Please close all the opened formulas and try again.", "Printing Error", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                        }
                        else if (mixingFormula == null || mixingFormula.Count == 0)
                        {
                            Msg.Show("Could not print the mixing formula." + System.Environment.NewLine + "Mixing formula not found", "Formula Not Found", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                        }
                    };
                    worker.RunWorkerAsync();
                    int res = DBAccess.UpdateGradingActive(this, true);
                }                    
               
            }
            else
            {
                Msg.Show("Formula is not available for " + RawProduct.Description, "Formula Not Availabable", MsgBoxButtons.OK, MsgBoxImage.Process_Stop, MsgBoxResult.Yes);
            }
        }

        private void ViewFormula()
        {
            if (!String.IsNullOrEmpty(GradingFormula) || GradingFormula == "Not Available")
            {
                var childWindow = new ChildWindowView();

                childWindow.ShowFormula(GradingFormula.Replace("\\\\", "/"));
            }
            else
            {
                Msg.Show("Cannot locate the formula for " + RawProduct.Description, "Formula Not Found", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
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


        #region COMMANDS

        public ICommand CompletedCommand
        {
            get
            {
                return _completedCommand ?? (_completedCommand = new LogOutCommandHandler(() => OrderValidion(), canExecute));
            }
        }
        public ICommand PrintCommand
        {
            get
            {
                return _printCommand ?? (_printCommand = new LogOutCommandHandler(() => PrintGradingFormula(), canExecute));
            }
        }

        public ICommand ViewCommand
        {
            get
            {
                return _viewCommand ?? (_viewCommand = new LogOutCommandHandler(() => ViewFormula(), canExecute));
            }
        }

        private bool CanCanOptionTick(object parameter)
        {
            return true;
        }

        public void DigitButtonPress(object parameter)
        {
            int res = 0;
            bool same = true;
            BackgroundWorker worker = new BackgroundWorker();
            LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Processing");

            worker.DoWork += (_, __) =>
            {
                same = FormulaOptions.Any(x => x.RawProduct.RawProductID == RawProduct.RawProductID && x.Checked);
                if (same == false)
                {
                    List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
                    bool has = systemParameters.Any(x => x.Value == true);
                    if (has == true)
                    {
                        //Tickle table
                        DBAccess.TickleGradingScheduling(GradingSchedulingID, SalesOrderId);
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            Msg.Show("System is processing some orders at the moment. Please try again in few minutes ", "System Busy", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                        });
                    }
                    else
                    {
                        int index = FormulaOptions.IndexOf(parameter as FormulaOptions);
                        if (index > -1 && index < FormulaOptions.Count)
                        {
                            ShredStock data = new ShredStock();
                            bool exe = false;
                            List<CurrentCapacity> ccl = new List<CurrentCapacity>();
                            ShiftManager sm = new ShiftManager();
                            List<ShredStock> tempShredStock = new List<ShredStock>();
                            List<Shift> ShiftDetails = DBAccess.GetAllShifts();
                            List<ShredStock> shredStock = DBAccess.GetShredStock();
                            Tuple<List<GradingCompleted>, Int32, DateTime> tuple = CalculateGradingRubber(FormulaOptions[index].RawProduct.RawProductID, Shift, ShiftDetails);

                            foreach (var item in tuple.Item1)
                            {
                                data = shredStock.SingleOrDefault(x => x.Shred.ID == item.GradingID);

                                if (data != null)
                                {
                                    if (data.Qty >= (item.KGCompleted * BlockLogQty))
                                    {
                                        exe = true;
                                    }
                                    else
                                    {
                                        exe = false;
                                        //Tickle table
                                        DBAccess.TickleGradingScheduling(GradingSchedulingID, SalesOrderId);
                                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            Msg.Show("Insufficient " + data.Shred.Name + System.Environment.NewLine + "Only " + data.Qty.ToString("G29") + "Kg of " + data.Shred.Name + " available" + System.Environment.NewLine + "This order requires " + (item.KGCompleted * BlockLogQty).ToString("G29") + "Kg", "Insufficient " + data.Shred.Name, MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                                        });
                                        break;
                                    }
                                }
                            }


                            if (exe || data == null)
                            {
                                Tuple<List<GradingCompleted>, Int32, DateTime> oldTuple = CalculateGradingRubber(RawProduct.RawProductID, Shift, ShiftDetails);

                                foreach (var item in tuple.Item1)
                                {
                                    ccl.Add(new CurrentCapacity() { SalesID = SalesOrderId, ProdTimeTableID = ProdTimeTableID, RawProductID = FormulaOptions[index].RawProduct.RawProductID, Shift = Shift, ProductCapacityID = item.GradingID, BlocksLogs = BlockLogQty, CapacityKG = item.KGCompleted * BlockLogQty, OrderType = OrderType });
                                }

                                foreach (var item in oldTuple.Item1)
                                {
                                    item.ProdTimeTableID = ProdTimeTableID;
                                }

                                /**Calculates adding to shredding stock or deducting**/
                                var f = FormulaOptions.SingleOrDefault(x => x.RawProduct.RawProductID == RawProduct.RawProductID);
                                if (f != null)
                                {
                                    var a = shredStock.SingleOrDefault(z => z.Shred.ID == f.Formula.ProductCapacity1);
                                    var b = shredStock.SingleOrDefault(y => y.Shred.ID == f.Formula.ProductCapacity2);
                                    //Adding to shredded stock
                                    if (a != null || b != null)
                                    {
                                        if (a != null)
                                        {
                                            tempShredStock.Add(new ShredStock() { Shred = new Shred() { ID = f.Formula.ProductCapacity1 }, Qty = a.Qty + (f.Formula.GradingWeight1 * BlockLogQty)});
                                        }

                                        if (b != null)
                                        {
                                            tempShredStock.Add(new ShredStock() { Shred = new Shred() { ID = f.Formula.ProductCapacity2 }, Qty = b.Qty + (f.Formula.GradingWeight2 * BlockLogQty)});
                                        }
                                    }
                                    else
                                    {//Deducting from shredded stock
                                        var c = FormulaOptions.SingleOrDefault(x => x.Checked == true);
                                        if (c != null)
                                        {
                                            var d = shredStock.SingleOrDefault(z => z.Shred.ID == c.Formula.ProductCapacity1);
                                            var e = shredStock.SingleOrDefault(y => y.Shred.ID == c.Formula.ProductCapacity2);

                                            if (d != null)
                                            {
                                                tempShredStock.Add(new ShredStock() { Shred = new Shred() { ID = d.Shred.ID }, Qty = (d.Qty - (c.Formula.GradingWeight1 * BlockLogQty)) < 0 ? 0 : d.Qty - (c.Formula.GradingWeight1 * BlockLogQty)});
                                            }
                                            if (e != null)
                                            {
                                                tempShredStock.Add(new ShredStock() { Shred = new Shred() { ID = e.Shred.ID }, Qty = (e.Qty - (c.Formula.GradingWeight2 * BlockLogQty)) < 0 ? 0 : e.Qty - (c.Formula.GradingWeight2 * BlockLogQty)});
                                            }
                                        }
                                    }
                                }
                                /**End of Calculates adding to shredding stock or deducting**/
                                var optionChecked = FormulaOptions.SingleOrDefault(x => x.Checked);
                                res = DBAccess.UpdateFormulaOptions(GradingSchedulingID, FormulaOptions[index].RawProduct.RawProductID, ccl, oldTuple.Item1, tempShredStock);
                                if (res > 0)
                                {
                                    //RawProduct.Description = "Black Shockpad With Shred";
                                    //RawProduct.RawProductID = 81;

                                    Transaction = new A1QSystem.Model.Transaction.Transaction()
                                    {
                                        TransDateTime = DateTime.Now,
                                        Transtype = "Switched",
                                        SalesOrderID = SalesOrderId,

                                        Products = new List<RawStock>()
                                        {
                                          new RawStock(){RawProductID = FormulaOptions[index].RawProduct.RawProductID,Qty=BlockLogQty},  
                                        },
                                        CreatedBy = System.Environment.MachineName
                                    };
                                    int r = DBAccess.InsertTransaction(Transaction);
                                }
                                else
                                {
                                    Transaction = new A1QSystem.Model.Transaction.Transaction()
                                    {
                                        TransDateTime = DateTime.Now,
                                        Transtype = "Did Not Switch",
                                        SalesOrderID = SalesOrderId,

                                        Products = new List<RawStock>()
                                        {
                                          new RawStock(){RawProductID = FormulaOptions[index].RawProduct.RawProductID,Qty=BlockLogQty},  
                                        },
                                        CreatedBy = System.Environment.MachineName
                                    };
                                    int r = DBAccess.InsertTransaction(Transaction);
                                }
                            }

                        }
                    }
                }
            };

            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();
                

                if (same == false && res == 0)
                {
                    Msg.Show("Could not switch to the other formula" + System.Environment.NewLine + "Please try again","", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.No);                   
                }
            };
            worker.RunWorkerAsync();
        }


        private ICommand _optionClickedCommand;
        public ICommand OptionClickedCommand
        {
            get
            {
                if (_optionClickedCommand == null)
                {
                    _optionClickedCommand = new A1QSystem.Commands.DelegateCommand(CanCanOptionTick, DigitButtonPress);
                }
                return _optionClickedCommand;
            }
        }


        #endregion
    }
}
