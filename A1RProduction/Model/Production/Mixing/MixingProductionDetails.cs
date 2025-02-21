using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model.Formula;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Other;
using A1QSystem.Model.Products;
using A1QSystem.Model.RawMaterials;
using A1QSystem.Model.Shifts;
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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace A1QSystem.Model.Production.Mixing
{
    public class MixingProductionDetails : RawProductionDetails
    {
        public Int32 MixingCurrentCapacityID { get; set; }
        public int ProdTimeTableID { get; set; }
        public int MixingTimeTableID { get; set; }
        public string MixingFormula { get; set; }
        public string MixingStatus { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<MixingOnly> MixingOnlyList { get; set; }
        //public int OrderType { get; set; }
        private ICommand _completedCommand;
        private ICommand _printCommand;
        //private ICommand _returnCommand;
        private ICommand _viewCommand;

        private Product _product;
        private bool canExecute;
        private bool _isReturnEnabled;
        private ChildWindowView LoadingScreen;
        private string _repeatAnimation;
        private bool _mixingActive;
        private int _currentShift;
        private int _currentMixingTimeTable;
        private string _comment;
        private string _productVisibility;
        public RawProductsActive RawProductsActive { get; set; }
        public A1QSystem.Model.Transaction.Transaction Transaction { get; set; }
        
        
        public MixingProductionDetails()
        {
            MixingOnlyList= DBAccess.GetMixingOnly();            
            canExecute = true;
            RepeatAnimation = "0x";     
        }

        private void OrderValidation()
        {
            bool re = DBAccess.GetSystemParameter("ChangingShiftForMixing");
            if (re == true)
            {
                Msg.Show("Orders are bieng shifted at the moment. Please try again in 5 minutes ", "Orders Shifting", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                bool isMixingActive =DBAccess.CheckMixingOrderActive(this);
                if(isMixingActive == false)
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
                        "Have you printed the formula and completed 1 " + RawProduct.RawProductType + " of " + RawProduct.Description + "?",
                        "Mixing Complete Confirmation",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (dialogRes.ToString() == "Yes")
                    {
                        CompleteOrder();
                    }                         
                }
                else
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
                        "Have you completed 1 " + RawProduct.RawProductType + " of " + RawProduct.Description + "?" + System.Environment.NewLine + "Click yes if you have only completed 1 " + RawProduct.RawProductType + " of " + RawProduct.Description,
                        "Mixing Complete Confirmation",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (dialogRes.ToString() == "Yes")
                    {
                        CompleteOrder();
                    }

                    int result = DBAccess.UpdateMixingActive(this, false);                    
                }
            }
        }

        private void CompleteOrder()
        {
            BackgroundWorker worker = new BackgroundWorker();
            LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Processing");

            worker.DoWork += (_, __) =>
            {
                string pcName = System.Environment.MachineName;
                if (string.IsNullOrEmpty(pcName))
                {
                    pcName = "Unknown";
                }

                //Get the current shift
                List<Shift> ShiftDetails = DBAccess.GetAllShifts();
                foreach (var item in ShiftDetails)
                {
                    bool isShift = TimeBetween(DateTime.Now, item.StartTime, item.EndTime);

                    if (isShift == true)
                    {
                        _currentShift = item.ShiftID;
                    }
                }

                List<RawProductMachine> rpm = DBAccess.GetMachineIdByRawProdId(RawProduct.RawProductID);
                if (rpm.Count != 0)
                {
                    _currentMixingTimeTable = GetProdTimeTableID(rpm[0].MixingMachineID);
                }
                
                /*****Process curing******/
                Curing pendingCuring = new Curing();
                string curDes = "Up";
                GenerateCuringTime();
                Tuple<Int32, int, bool> tuple = DBAccess.CheckToAddCuring(RawProduct.RawProductID);

                if (tuple.Item1 == 0 && tuple.Item2 == 0 && tuple.Item3 == false || tuple.Item3==true)
                {
                    //Get BlockLog Curing
                    pendingCuring = DBAccess.GetTopCuringProduct(SalesOrderId, RawProduct.RawProductID);
                    pendingCuring.StartTime = StartTime;
                    pendingCuring.EndTime = EndTime;
                    pendingCuring.IsEnabled = true;
                    curDes = "Up";
                }
                else
                {
                    pendingCuring.OrderNo = tuple.Item1;
                    pendingCuring.Product = new Product() { ProductID = tuple.Item2, RawProduct = new RawProduct() { RawProductID = RawProduct.RawProductID } };
                    pendingCuring.Qty = 1;
                    pendingCuring.StartTime = StartTime;
                    pendingCuring.EndTime = EndTime;
                    pendingCuring.IsCured = false;
                    pendingCuring.IsEnabled = true;
                    curDes = "Ins";
                }

                List<GradingCompleted> ggList = CalculateGradingRubber(this.RawProduct.RawProductID, _currentShift);
               
                decimal b = DBAccess.UpdateMixingRawProdQty(this, pcName, _currentShift, _currentMixingTimeTable, ggList, pendingCuring,curDes);               
            };

            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();
            };
            worker.RunWorkerAsync();
        }

        private int GetProdTimeTableID(int mId)
        {
            int id = 0;
            DateTime curDate = DateTime.Now.Date;
            List<ProductionTimeTable> prodTT = DBAccess.GetProductionTimeTableByID(mId, curDate);
            if (prodTT.Count != 0)
            {
                id = prodTT[0].ID;
            }

            return id;
        }

        private List<GradingCompleted> CalculateGradingRubber(int rawProdId, int curShift)
        {
            List<GradingCompleted> ggCompleted = new List<GradingCompleted>();

            List<Formulas> fList = DBAccess.GetFormulaDetailsByRawProdID(rawProdId);
            if (fList.Count > 0)
            {
                if (fList[0].ProductCapacity1 != 0 && fList[0].ProductCapacity2 == 0)
                {
                    ggCompleted.Add(new GradingCompleted() { GradingID = fList[0].ProductCapacity1, KGCompleted = fList[0].GradingWeight1, CreatedDate = DateTime.Now, ProdTimeTableID = _currentMixingTimeTable, Shift = curShift });
                }
                else if (fList[0].ProductCapacity1 != 0 && fList[0].ProductCapacity2 != 0)
                {
                    ggCompleted.Add(new GradingCompleted() { GradingID = fList[0].ProductCapacity1, KGCompleted = fList[0].GradingWeight1, CreatedDate = DateTime.Now, ProdTimeTableID = _currentMixingTimeTable, Shift = curShift });
                    ggCompleted.Add(new GradingCompleted() { GradingID = fList[0].ProductCapacity2, KGCompleted = fList[0].GradingWeight2, CreatedDate = DateTime.Now, ProdTimeTableID = _currentMixingTimeTable, Shift = curShift });
                }
            }

            return ggCompleted;
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

        private void GenerateCuringTime()
        {
            StartTime = DateTime.Now;
          
            EndTime = StartTime.AddHours(+7);
            //DateTime DeMouldingTime = new DateTime(year, month, day, 05, 00, 00).AddDays(+1);

            //if(StartTime < DeMouldingTime.AddHours(-7))
            //{
            //    EndTime= DeMouldingTime;
            //}
            //else
            //{
            //    EndTime = DeMouldingTime.AddDays(+1);
            //}          
        }

        private void PrintGradingFormula()
        {            
            if (this.MixingFormula != "Not Available" || this.MixingFormula != null)
            {
                if (Msg.Show("Do you want to PRINT Formula for " + RawProduct.Description + "?", "Formula Printing Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    Tuple<string, Exception> tup = null;
                    BackgroundWorker worker = new BackgroundWorker();
                    LoadingScreen = new ChildWindowView();
                    LoadingScreen.ShowWaitingScreen("Printing");

                    worker.DoWork += (_, __) =>
                    {
                        //Adding Date to the formula
                        PrintingManager pm = new PrintingManager();
                        tup = pm.AddDateTime(MixingFormula, "MP" + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".pdf", "I:/PRODUCTION/DONOTDELETE/MixingPrints/");

                        ProcessStartInfo info = new ProcessStartInfo(tup.Item1);
                        info.Verb = "Print";
                        info.CreateNoWindow = true;
                        info.WindowStyle = ProcessWindowStyle.Hidden;
                        Process.Start(info);
                    };

                    worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                    {
                        LoadingScreen.CloseWaitingScreen();
                        if (tup.Item2 != null)
                        {
                            Msg.Show("A problem has occured while formula is prining. Please close all the opened formulas and try again.", "Printing Error", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                        }
                    };
                    worker.RunWorkerAsync();
                    int result = DBAccess.UpdateMixingActive(this, true);
                }
            }
            else
            {
                Msg.Show("Formula is not available for " + RawProduct.Description, "Formula Not Availabable", MsgBoxButtons.OK, MsgBoxImage.Process_Stop, MsgBoxResult.Yes);
            }
        }
       

        private void ViewFormula()
        {
            if (!String.IsNullOrEmpty(MixingFormula) || MixingFormula == "Not Available")
            {
                var childWindow = new ChildWindowView();

                childWindow.ShowFormula(MixingFormula.Replace("\\\\","/"));
            }
            else
            {
                Msg.Show("Cannot locate the formula for " + RawProduct.Description, "Formula Not Found", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
        }

        #region PUBLIC PROPERTIES

        public string ProductVisibility
        {
            get
            {
                return _productVisibility;
            }
            set
            {
                _productVisibility = value;
                RaisePropertyChanged(() => this.ProductVisibility);
            }
        }


        public Product Product
        {
            get 
            {
                return _product;
            }
            set
            {
                _product = value;
                RaisePropertyChanged(() => this.Product);               
            }
        }

        public bool IsReturnEnabled
        {
            get 
            {
                return _isReturnEnabled;
            }
            set
            {
                _isReturnEnabled = value;
                RaisePropertyChanged(() => this.IsReturnEnabled);               
            }
        }

        private int _ordertype;
        public new int OrderType
        {
            get { return _ordertype; }
            set
            {
                _ordertype = value;
                RaisePropertyChanged(() => this.OrderType);
                GetRowColour();
                //Order Types:
                //1 - Urgent
                //2 - Urgent Graded 
                //3 - Normal
                //4 - Normal Graded

                //Console.WriteLine("xx" + Rank);

               
            }
        }

        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                RaisePropertyChanged(() => this.Comment);
                if(String.IsNullOrEmpty(Comment))
                {
                    IsNotesVisible = "Collapsed";
                }
                else
                {
                    IsNotesVisible = "Visible";
                }
            }
        }

        private int _rank;
        public int Rank
        {
            get { return _rank; }
            set
            {
                _rank = value;
                RaisePropertyChanged(() => this.Rank);

                GetRowColour();
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


        public bool MixingActive
        {
            get { return _mixingActive; }
            set
            {
                _mixingActive = value;
                RaisePropertyChanged(() => this.MixingActive);
                if (MixingActive == true)
                {
                    RepeatAnimation = "Forever";
                }
                else
                {
                    RepeatAnimation = "0x";
                }
            }
        }
        #endregion

        private void GetRowColour()
        {
            if (OrderType == 1 || OrderType == 2)
            {
                RowBackgroundColour = "#d4383a";//Urgent
            }
            else
                //if (OrderType == 3 || OrderType == 4 || Rank == 1)
                //{
                //    RowBackgroundColour = "#ffffff";
                //}
                //else 
                if (Rank == 1)
                {
                    RowBackgroundColour = "#d4383a";
                }
                else if (Rank == 2)
                {
                    RowBackgroundColour = "#0066ff";
                }
                else if (Rank == 3)
                {
                    RowBackgroundColour = "#ffffff";
                }
                else if (Rank == 4)
                {
                    RowBackgroundColour = "#e5bf43";
                }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        #region COMMANDS

        public ICommand CompletedCommand
        {
            get
            {
                return _completedCommand ?? (_completedCommand = new LogOutCommandHandler(() => OrderValidation(), canExecute));
            }
        }
        public ICommand PrintCommand
        {
            get
            {
                return _printCommand ?? (_printCommand = new LogOutCommandHandler(() => PrintGradingFormula(), canExecute));
            }
        }

        //public ICommand ReturnCommand
        //{
        //    get
        //    {
        //        return _returnCommand ?? (_returnCommand = new LogOutCommandHandler(() => ReturnBin(), canExecute));
        //    }
        //}
        public ICommand ViewCommand
        {
            get
            {
                return _viewCommand ?? (_viewCommand = new LogOutCommandHandler(() => ViewFormula(), canExecute));
            }
        }
         
        #endregion
    }
}
