using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Other;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Mixing;
using A1QSystem.Model.Products;
using A1QSystem.Model.Shifts;
using A1QSystem.Model.Stock;
using A1QSystem.Model.Transaction;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using A1QSystem.View.Dashboard;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using MainMenu = A1QSystem.View.MainMenu;

namespace A1QSystem.ViewModel.Productions.Mixing
{


    public class MixingScheduleViewModel : ViewModelBase
    {

        private ObservableCollection<MixingProductionDetails> _MixingProduction = null;
        private ObservableCollection<MixingProductionDetails> _mixingProduction1 = null;
        private ObservableCollection<MixingProductionDetails> _mixingProduction2 = null;
        private ObservableCollection<MixingProductionDetails> _mixingProduction3 = null;
        private ObservableCollection<MixingProductionDetails> _allMixingProduction = null;
        //private ObservableCollection<MixingProductionDetails> _mixingProduction5 = null;
        private List<MetaData> metaData;
        private List<Shift> _shiftDetails;
        private string _disCurrentDate;
        private DateTime CurrentDate;
        public int TotOrdersPending { get; set; }
        private string userName;
        private string state;
        private ChildWindowView LoadingScreen;
        private List<UserPrivilages> privilages;
        private string _allGridVisibility;
        private string _dailyGridVisibility;
        private string _dailyDate;
        private System.Windows.Forms.Timer tmr = null;

        private ICommand _viewMixingScheduleCommand;
        private ICommand navHomeCommand;
        private ICommand _workStattionCommand;
        private ICommand _printMixingPendingCommand;
        private ICommand _iBCChangeCommand;
        private ICommand _allMixingOrdersCommand;
        private ICommand _dailyMixingOrdersCommand;
        private ICommand _returnCommandAll;
        private ICommand _returnCommandGrid1;
        private ICommand _returnCommandGrid2;
        private ICommand _returnCommandGrid3;
        private string _version;
        private bool canExecute;

        public Dispatcher UIDispatcher { get; set; }
        public MixingOrdersNotifier mixingOrdersNotifier { get; set; }

        public MixingScheduleViewModel(string UserName, string State, List<UserPrivilages> Privilages, Dispatcher uidispatcher, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = Privilages;
            canExecute = true;
            metaData = md;
            CurrentDate = NTPServer.GetNetworkTime();
            _shiftDetails = DBAccess.GetAllShifts();
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
            SwitchView("daily");
            StartTimer();
            this.UIDispatcher = uidispatcher;

            this.mixingOrdersNotifier = new MixingOrdersNotifier();

            BackgroundWorker worker = new BackgroundWorker();
            LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Loading");

            worker.DoWork += (_, __) =>
            {
                this.mixingOrdersNotifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_NewMessage);
                ObservableCollection<MixingProductionDetails> opd = this.mixingOrdersNotifier.RegisterDependency();

                this.LoadData(opd);                
            };

            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();

            };
            worker.RunWorkerAsync();
        }

        private void LoadData(ObservableCollection<MixingProductionDetails> psd)
        {
            CurrentDate = NTPServer.GetNetworkTime();

            this.UIDispatcher.BeginInvoke((Action)delegate()
            {
                if (psd != null)
                {
                    MixingProduction1 = new ObservableCollection<MixingProductionDetails>();
                    MixingProduction2 = new ObservableCollection<MixingProductionDetails>();
                    MixingProduction3 = new ObservableCollection<MixingProductionDetails>();
                    AllMixingProduction = new ObservableCollection<MixingProductionDetails>();
                    List<MixingProductionDetails> TempAllMixingProduction = new List<MixingProductionDetails>();

                    int y = 0;
                    int cshift = 0;                   

                    foreach (var item in _shiftDetails)
                    {
                        bool isShift = TimeBetween(DateTime.Now, item.StartTime, item.EndTime);

                        if (isShift == true)
                        {
                            cshift = item.ShiftID;
                        }
                    }

                    foreach (var item in psd)
                    {
                        bool val = false;
                        if (cshift == 1)
                        {
                            val = item.RawProductsActive.DayShift;
                        }
                        else if (cshift == 2)
                        {
                            val = item.RawProductsActive.EveningShift;
                        }
                        else if (cshift == 3)
                        {
                            val = item.RawProductsActive.NightShift;
                        }

                        if (Convert.ToDateTime(item.ProductionDate).Date == CurrentDate.Date && val == true)
                        {
                            DisCurrentDate = CurrentDate.DayOfWeek + " " + CurrentDate.ToString("dd/MM/yyy");

                            switch (y)
                            {
                                case 0:
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                case 6:                                                                                              

                                    MixingProduction1.Add(new MixingProductionDetails()
                                    {
                                        RawProduct = new RawProduct()
                                        {
                                            RawProductID = item.RawProduct.RawProductID,
                                            RawProductCode = item.RawProduct.RawProductCode,
                                            Description = item.RawProduct.Description,
                                            RawProductType = item.RawProduct.RawProductType
                                        },
                                        Customer = new Customer()
                                        {
                                            CompanyName = item.Customer.CompanyName
                                        },
                                        MixingCurrentCapacityID = item.MixingCurrentCapacityID,
                                        ProdTimeTableID = item.ProdTimeTableID,
                                        MixingTimeTableID = item.MixingTimeTableID,
                                        SalesOrder = item.SalesOrder,
                                        SalesOrderId = item.SalesOrderId,
                                        RequiredDate = item.RequiredDate,
                                        MixingShift = item.MixingShift,
                                        Comment = item.Comment,
                                        BlockLogQty = Math.Ceiling(item.BlockLogQty),
                                        RawProDetailsID = item.RawProDetailsID,
                                        MixingFormula = item.MixingFormula,
                                        IsReturnEnabled = (item.MixingOnlyList.Select(x => x.RawProductID).Contains(item.RawProduct.RawProductID)) ? false : true,
                                        OrderType = item.OrderType,
                                        RowBackgroundColour = item.RowBackgroundColour,
                                        Rank = item.Rank,
                                        MixingActive = item.ActiveOrder,
                                        IsNotesVisible = item.IsNotesVisible,
                                        IsReqDateVisible = item.ReqDateSelected == true ? "Visible" : "Collapsed"
                                    });

                                    break;


                                case 7:
                                case 8:
                                case 9:
                                case 10:
                                case 11:
                                case 12:
                                case 13:                                                   

                                    MixingProduction2.Add(new MixingProductionDetails()
                                    {
                                        RawProduct = new RawProduct()
                                        {
                                            RawProductID = item.RawProduct.RawProductID,
                                            RawProductCode = item.RawProduct.RawProductCode,
                                            Description = item.RawProduct.Description,
                                            RawProductType = item.RawProduct.RawProductType
                                        },
                                        Customer = new Customer()
                                        {
                                            CompanyName = item.Customer.CompanyName
                                        },
                                        MixingCurrentCapacityID = item.MixingCurrentCapacityID,
                                        ProdTimeTableID = item.ProdTimeTableID,
                                        MixingTimeTableID = item.MixingTimeTableID,
                                        SalesOrder = item.SalesOrder,
                                        SalesOrderId = item.SalesOrderId,
                                        RequiredDate = item.RequiredDate,
                                        MixingShift = item.MixingShift,
                                        Comment = item.Comment,
                                        BlockLogQty = Math.Ceiling(item.BlockLogQty),
                                        RawProDetailsID = item.RawProDetailsID,
                                        MixingFormula = item.MixingFormula,
                                        IsReturnEnabled = (item.MixingOnlyList.Select(x => x.RawProductID).Contains(item.RawProduct.RawProductID)) ? false : true,
                                        OrderType = item.OrderType,
                                        RowBackgroundColour = item.RowBackgroundColour,
                                        Rank = item.Rank,
                                        MixingActive = item.ActiveOrder,
                                        IsNotesVisible = item.IsNotesVisible,
                                        IsReqDateVisible = item.ReqDateSelected == true ? "Visible" : "Collapsed"
                                    });

                                    break;

                                case 14:
                                case 15:
                                case 16:
                                case 17:
                                case 18:
                                case 19:
                                case 20:                                   

                                    MixingProduction3.Add(new MixingProductionDetails()
                                    {
                                        RawProduct = new RawProduct()
                                        {
                                            RawProductID = item.RawProduct.RawProductID,
                                            RawProductCode = item.RawProduct.RawProductCode,
                                            Description = item.RawProduct.Description,
                                            RawProductType = item.RawProduct.RawProductType
                                        },
                                        Customer = new Customer()
                                        {
                                            CompanyName = item.Customer.CompanyName
                                        },
                                        MixingCurrentCapacityID = item.MixingCurrentCapacityID,
                                        ProdTimeTableID = item.ProdTimeTableID,
                                        MixingTimeTableID = item.MixingTimeTableID,
                                        SalesOrder = item.SalesOrder,
                                        SalesOrderId = item.SalesOrderId,
                                        RequiredDate = item.RequiredDate,
                                        MixingShift = item.MixingShift,
                                        Comment = item.Comment,
                                        BlockLogQty = Math.Ceiling(item.BlockLogQty),
                                        RawProDetailsID = item.RawProDetailsID,
                                        MixingFormula = item.MixingFormula,
                                        IsReturnEnabled = (item.MixingOnlyList.Select(x => x.RawProductID).Contains(item.RawProduct.RawProductID)) ? false : true,
                                        OrderType = item.OrderType,
                                        RowBackgroundColour = item.RowBackgroundColour,
                                        Rank = item.Rank,
                                        MixingActive = item.ActiveOrder,
                                        IsNotesVisible = item.IsNotesVisible,
                                        IsReqDateVisible = item.ReqDateSelected == true ? "Visible" : "Collapsed"
                                    });

                                    break;
                            }
                            y++;

                            //All mixing orders
                            TempAllMixingProduction.Add(new MixingProductionDetails()
                            {
                                RawProduct = new RawProduct()
                                {
                                    RawProductID = item.RawProduct.RawProductID,
                                    RawProductCode = item.RawProduct.RawProductCode,
                                    Description = item.RawProduct.Description + " X " + Math.Ceiling(item.BlockLogQty) + " " + item.RawProduct.RawProductType + AddS(Math.Ceiling(item.BlockLogQty)),
                                    RawProductType = item.RawProduct.RawProductType
                                },
                                Customer = new Customer()
                                {
                                    CompanyName = item.Customer.CompanyName
                                },
                                MixingCurrentCapacityID = item.MixingCurrentCapacityID,
                                ProdTimeTableID = item.ProdTimeTableID,
                                MixingTimeTableID = item.MixingTimeTableID,
                                SalesOrder = item.SalesOrder,
                                SalesOrderId = item.SalesOrderId,
                                RequiredDate = item.RequiredDate,
                                MixingShift = item.MixingShift,
                                Comment = item.Comment,
                                BlockLogQty = Math.Ceiling(item.BlockLogQty),
                                RawProDetailsID = item.RawProDetailsID,
                                MixingFormula = item.MixingFormula,
                                IsReturnEnabled = (item.MixingOnlyList.Select(x => x.RawProductID).Contains(item.RawProduct.RawProductID)) ? false : true,
                                OrderType = item.OrderType,
                                RowBackgroundColour = item.RowBackgroundColour,
                                Rank = item.Rank,
                                MixingActive = item.ActiveOrder,
                                IsNotesVisible = item.IsNotesVisible,
                                IsReqDateVisible = item.ReqDateSelected == true ? "Visible" : "Collapsed"
                            });

                            var result = TempAllMixingProduction.Distinct(new MixingItemEqualityComparer());
                            AllMixingProduction = new ObservableCollection<MixingProductionDetails>(result);
                        }
                    }
                }
            });
        }

        private void StartTimer()
        {
            tmr = new System.Windows.Forms.Timer();
            tmr.Interval = 1000;
            tmr.Tick += new EventHandler(tmr_Tick);
            tmr.Enabled = true;
        }

        void tmr_Tick(object sender, EventArgs e)
        {
            DailyDate = DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm:ss tt");
        }

        private string GetRowColour(int x)
        {
            string col = "#ffffff";

            if (x == 1)
            {
                col = "#e62d00";//Urgent
            }
            else if (x == 2)
            {
                col = "#006600";//Urgent Graded
            }
            else if (x == 3)
            {
                col = "#ffffff";//Normal
            }
            else if (x == 4)
            {
                col = "#33cc33";//Normal
            }

            return col;
        }

        private string AddS(decimal x)
        {
            string s = string.Empty;
            if (x > 1)
            {
                s = "s";
            }
            return s;
        }

        private void PrintMixingPending()
        {

            List<MixingOrder> mo = new List<MixingOrder>();
            mo = DBAccess.GetCurrentMixingList();

            if (mo.Count > 0)
            {
                Exception exception = null;
                BackgroundWorker worker = new BackgroundWorker();
                ChildWindowView LoadingScreen = new ChildWindowView();
                LoadingScreen.ShowWaitingScreen("Printing");

                worker.DoWork += (_, __) =>
                {
                    GradingCompletedMixingStatusPDF pdf = new GradingCompletedMixingStatusPDF(mo);
                    pdf.CreatePDF();
                };

                worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                {
                    LoadingScreen.CloseWaitingScreen();
                    if (exception != null)
                    {
                        Msg.Show("A problem has occured while the work order is prining. Please try again later." + System.Environment.NewLine + exception, "Printing Error", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                    }
                };
                worker.RunWorkerAsync();
            }
            else
            {
                Msg.Show("There are no items pending in mixing", "No Items found", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
            }
        }

        private void IBCChangeOver()
        {
            var childWindow = new ChildWindowView();
            childWindow.ShowIBCChangeWindow();
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

        void notifier_NewMessage(object sender, SqlNotificationEventArgs e)
        {
            this.LoadData(this.mixingOrdersNotifier.RegisterDependency());
        }

        private decimal BlockLogCalculator(decimal qty, decimal yield)
        {
            decimal res = 0;

            res = Math.Ceiling(qty / yield);

            return res;
        }

        private void OpenMixingSchedule()
        {
            var childWindow = new ChildWindowView();
            childWindow.ShowMixingWeeklySchedule();
        }

        private void SwitchView(string s)
        {
            if (s == "daily")
            {
                DailyGridVisibility = "Visible";
                AllGridVisibility = "Collapsed";
            }
            else if(s == "all")
            {
                DailyGridVisibility = "Collapsed";
                AllGridVisibility = "Visible";
            }
        }

        private void ReturnBinAllGrid(object parameter)
        {
            int index = AllMixingProduction.IndexOf(parameter as MixingProductionDetails);
            if (index > -1 && index < AllMixingProduction.Count)
            {

                List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
                bool has = systemParameters.Any(x => x.Value == true);
                if (has == true)
                {
                    Msg.Show("Orders are bieng shifted at the moment. Please try again in few minutes ", "Orders Shifting", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                }
                else
                {
                    int sp1 = DBAccess.UpdateSystemParameter("ReturnBin", true);
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
                            "Do you want to return 1 " + AllMixingProduction[index].RawProduct.Description + " bin to Grading?",
                            "Bin Returning Confirmation",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (dialogRes.ToString() == "Yes")
                        {                                

                            BackgroundWorker worker = new BackgroundWorker();
                            LoadingScreen = new ChildWindowView();
                            LoadingScreen.ShowWaitingScreen("Processing");

                            worker.DoWork += (_, __) =>
                            {
                                //Determine the ProductionTimeTableID for grading
                                List<RawProductMachine> RawProductMachine = DBAccess.GetMachineIdByRawProdId(AllMixingProduction[index].RawProduct.RawProductID);
                                List<ProductionTimeTable> prt = DBAccess.GetProductionTimeTableDateByID(AllMixingProduction[index].ProdTimeTableID);
                                DateTime date = prt[0].ProductionDate;
                                List<ProductionTimeTable> ptt = DBAccess.GetProductionTimeTableByMachineID(RawProductMachine[0].GradingMachineID, date.Date);

                                DBAccess.ReturnBin(AllMixingProduction[index], ptt[0].ID);
                                //Finally refresh the grid
                                AllMixingProduction=DBAccess.GetAllMixingOrders();                                
                            };

                            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                            {
                                LoadingScreen.CloseWaitingScreen();
                            };
                            worker.RunWorkerAsync();
                        }

                        int sp2 = DBAccess.UpdateSystemParameter("ReturnBin", false);
                    }

                    //Finally refresh the grid
                    //this.LoadData(this.mixingOrdersNotifier.RegisterDependency());
                }     
            }
        }

        private void ReturnBinGrid1(object parameter)
        {
            int index = MixingProduction1.IndexOf(parameter as MixingProductionDetails);
            if (index > -1 && index < MixingProduction1.Count)
            {

                List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
                bool has = systemParameters.Any(x => x.Value == true);
                if (has == true)
                {
                    Msg.Show("Orders are bieng shifted at the moment. Please try again in few minutes ", "Orders Shifting", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                }
                else
                {
                    int sp1 = DBAccess.UpdateSystemParameter("ReturnBin", true);
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
                            "Do you want to return 1 " + MixingProduction1[index].RawProduct.Description + " bin to Grading?",
                            "Bin Returning Confirmation",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (dialogRes.ToString() == "Yes")
                        {                            

                            BackgroundWorker worker = new BackgroundWorker();
                            LoadingScreen = new ChildWindowView();
                            LoadingScreen.ShowWaitingScreen("Processing");

                            worker.DoWork += (_, __) =>
                            {
                                //Determine the ProductionTimeTableID for grading
                                List<RawProductMachine> RawProductMachine = DBAccess.GetMachineIdByRawProdId(MixingProduction1[index].RawProduct.RawProductID);
                                List<ProductionTimeTable> prt = DBAccess.GetProductionTimeTableDateByID(MixingProduction1[index].ProdTimeTableID);
                                DateTime date = prt[0].ProductionDate;
                                List<ProductionTimeTable> ptt = DBAccess.GetProductionTimeTableByMachineID(RawProductMachine[0].GradingMachineID, date.Date);

                                DBAccess.ReturnBin(MixingProduction1[index], ptt[0].ID);
                                //Finally refresh the grid
                                MixingProduction1 = DBAccess.GetAllMixingOrders();                                             
                            };

                            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                            {
                                LoadingScreen.CloseWaitingScreen();
                            };
                            worker.RunWorkerAsync();
                        }
                        int sp2 = DBAccess.UpdateSystemParameter("ReturnBin", false);
                    }                    
                }     
            }
        }
        private void ReturnBinGrid2(object parameter)
        {
            int index = MixingProduction2.IndexOf(parameter as MixingProductionDetails);
            if (index > -1 && index < MixingProduction2.Count)
            {

                List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
                bool has = systemParameters.Any(x => x.Value == true);
                if (has == true)
                {
                    Msg.Show("Orders are bieng shifted at the moment. Please try again in few minutes ", "Orders Shifting", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                }
                else
                {
                    int sp1 = DBAccess.UpdateSystemParameter("ReturnBin", true);
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
                            "Do you want to return 1 " + MixingProduction2[index].RawProduct.Description + " bin to Grading?",
                            "Bin Returning Confirmation",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (dialogRes.ToString() == "Yes")
                        {                            
                            BackgroundWorker worker = new BackgroundWorker();
                            LoadingScreen = new ChildWindowView();
                            LoadingScreen.ShowWaitingScreen("Processing");

                            worker.DoWork += (_, __) =>
                            {
                                //Determine the ProductionTimeTableID for grading
                                List<RawProductMachine> RawProductMachine = DBAccess.GetMachineIdByRawProdId(MixingProduction2[index].RawProduct.RawProductID);
                                List<ProductionTimeTable> prt = DBAccess.GetProductionTimeTableDateByID(MixingProduction2[index].ProdTimeTableID);
                                DateTime date = prt[0].ProductionDate;
                                List<ProductionTimeTable> ptt = DBAccess.GetProductionTimeTableByMachineID(RawProductMachine[0].GradingMachineID, date.Date);

                                DBAccess.ReturnBin(MixingProduction2[index], ptt[0].ID);
                                //Finally refresh the grid
                                MixingProduction2 = DBAccess.GetAllMixingOrders(); 
                            };

                            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                            {
                                LoadingScreen.CloseWaitingScreen();
                            };
                            worker.RunWorkerAsync();
                        }

                        int sp2 = DBAccess.UpdateSystemParameter("ReturnBin", false);
                    }
                }
            }
        }

        private void ReturnBinGrid3(object parameter)
        {
            int index = MixingProduction3.IndexOf(parameter as MixingProductionDetails);
            if (index > -1 && index < MixingProduction3.Count)
            {

                List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
                bool has = systemParameters.Any(x => x.Value == true);
                if (has == true)
                {
                    Msg.Show("Orders are bieng shifted at the moment. Please try again in few minutes ", "Orders Shifting", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                }
                else
                {
                    int sp1 = DBAccess.UpdateSystemParameter("ReturnBin", true);
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
                            "Do you want to return 1 " + MixingProduction3[index].RawProduct.Description + " bin to Grading?",
                            "Bin Returning Confirmation",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (dialogRes.ToString() == "Yes")
                        {                            
                            BackgroundWorker worker = new BackgroundWorker();
                            LoadingScreen = new ChildWindowView();
                            LoadingScreen.ShowWaitingScreen("Processing");

                            worker.DoWork += (_, __) =>
                            {
                                //Determine the ProductionTimeTableID for grading
                                List<RawProductMachine> RawProductMachine = DBAccess.GetMachineIdByRawProdId(MixingProduction3[index].RawProduct.RawProductID);
                                List<ProductionTimeTable> prt = DBAccess.GetProductionTimeTableDateByID(MixingProduction3[index].ProdTimeTableID);
                                DateTime date = prt[0].ProductionDate;
                                List<ProductionTimeTable> ptt = DBAccess.GetProductionTimeTableByMachineID(RawProductMachine[0].GradingMachineID, date.Date);

                                DBAccess.ReturnBin(MixingProduction3[index], ptt[0].ID);
                                //Finally refresh the grid
                                MixingProduction3 = DBAccess.GetAllMixingOrders(); 
                            };

                            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                            {
                                LoadingScreen.CloseWaitingScreen();
                            };
                            worker.RunWorkerAsync();
                        }

                        int sp2 = DBAccess.UpdateSystemParameter("ReturnBin", false);
                    }
                }
            }
        }

        private static bool CanButtonPress(string button)
        {
            return true;
        }

        #region PUBLIC PROPERTIES

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

        public ObservableCollection<MixingProductionDetails> MixingProduction
        {
            get
            {
                return _MixingProduction;
            }
            set
            {
                _MixingProduction = value;
            }
        }

        public ObservableCollection<MixingProductionDetails> MixingProduction1
        {
            get
            {
                return _mixingProduction1;
            }

            set
            {
                _mixingProduction1 = value;
                RaisePropertyChanged(() => this.MixingProduction1);
            }
        }

        public ObservableCollection<MixingProductionDetails> MixingProduction2
        {
            get
            {
                return _mixingProduction2;
            }
            set
            {
                _mixingProduction2 = value;
                RaisePropertyChanged(() => this.MixingProduction2);
            }
        }

        public ObservableCollection<MixingProductionDetails> MixingProduction3
        {
            get
            {
                return _mixingProduction3;
            }
            set
            {
                _mixingProduction3 = value;
                RaisePropertyChanged(() => this.MixingProduction3);
            }
        }

        public ObservableCollection<MixingProductionDetails> AllMixingProduction
        {
            get
            {
                return _allMixingProduction;
            }
            set
            {
                _allMixingProduction = value;
                RaisePropertyChanged(() => this.AllMixingProduction);
            }
        }

        

        public string DisCurrentDate
        {
            get
            {
                return _disCurrentDate;
            }
            set
            {
                _disCurrentDate = value;
                RaisePropertyChanged(() => this.DisCurrentDate);
            }
        }

        public string DailyGridVisibility
        {
            get
            {
                return _dailyGridVisibility;
            }
            set
            {
                _dailyGridVisibility = value;
                RaisePropertyChanged(() => this.DailyGridVisibility);
            }
        }

        public string AllGridVisibility
        {
            get
            {
                return _allGridVisibility;
            }
            set
            {
                _allGridVisibility = value;
                RaisePropertyChanged(() => this.AllGridVisibility);
            }
        }

        public string DailyDate
        {
            get { return _dailyDate; }
            set
            {
                _dailyDate = value;
                RaisePropertyChanged(() => this.DailyDate);
            }
        }       

        #endregion

        #region COMMANDS

        public ICommand ViewMixingScheduleCommand
        {
            get
            {
                return _viewMixingScheduleCommand ?? (_viewMixingScheduleCommand = new A1QSystem.Commands.LogOutCommandHandler(OpenMixingSchedule, canExecute));
            }
        }

        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, privilages, metaData)), canExecute));
            }
        }
        public ICommand WorkStationsCommand
        {
            get
            {
                return _workStattionCommand ?? (_workStattionCommand = new LogOutCommandHandler(() => Switcher.Switch(new WorkStationsView(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand PrintMixingPendingCommand
        {
            get
            {
                return _printMixingPendingCommand ?? (_printMixingPendingCommand = new A1QSystem.Commands.LogOutCommandHandler(PrintMixingPending, canExecute));
            }
        }

        public ICommand IBCChangeCommand
        {
            get
            {
                return _iBCChangeCommand ?? (_iBCChangeCommand = new A1QSystem.Commands.LogOutCommandHandler(IBCChangeOver, canExecute));
            }
        }

        public ICommand DailyMixingOrdersCommand
        {
            get
            {
                if (_dailyMixingOrdersCommand == null)
                {
                    _dailyMixingOrdersCommand = new DelegateCommand<string>(SwitchView, CanButtonPress);
                }
                return _dailyMixingOrdersCommand;
            }
        }

        public ICommand AllMixingOrdersCommand
        {
            get
            {
                if (_allMixingOrdersCommand == null)
                {
                    _allMixingOrdersCommand = new DelegateCommand<string>(SwitchView, CanButtonPress);
                }
                return _allMixingOrdersCommand;
            }
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        public ICommand ReturnCommandAll
        {
            get
            {
                if (_returnCommandAll == null)
                {
                    _returnCommandAll = new A1QSystem.Commands.DelegateCommand(CanExecute, ReturnBinAllGrid);
                }
                return _returnCommandAll;
            }
        }

        public ICommand ReturnCommandGrid1
        {
            get
            {
                if (_returnCommandGrid1 == null)
                {
                    _returnCommandGrid1 = new A1QSystem.Commands.DelegateCommand(CanExecute, ReturnBinGrid1);
                }
                return _returnCommandGrid1;
            }
        }

        public ICommand ReturnCommandGrid2
        {
            get
            {
                if (_returnCommandGrid2 == null)
                {
                    _returnCommandGrid2 = new A1QSystem.Commands.DelegateCommand(CanExecute, ReturnBinGrid2);
                }
                return _returnCommandGrid2;
            }
        }

        public ICommand ReturnCommandGrid3
        {
            get
            {
                if (_returnCommandGrid3 == null)
                {
                    _returnCommandGrid3 = new A1QSystem.Commands.DelegateCommand(CanExecute, ReturnBinGrid3);
                }
                return _returnCommandGrid3;
            }
        }

        #endregion
    }
}
