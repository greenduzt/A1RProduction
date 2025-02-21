using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model.Formula;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Other;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Grading;
using A1QSystem.Model.Production.Mixing;
using A1QSystem.Model.Products;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Model.RawMaterials
{
    public class RawProductionDetails : ViewModelBase
    {
        public int GradingSchedulingID { get; set; }
        public string CurrentCapacityID { get; set; }
        public int ProdTimeTableID { get; set; }
        private int _RawProDetailsID;       
        private RawProduct _rawProduct;
        public RawProductMachine _rawProductMachine;
        private decimal _blockLogQty;
        private bool _moveEnabled;
        private bool _deleteEnabled;
        
        public string ProductionDate { get; set; }
        public DateTime PDate { get; set; }
        public int Shift { get; set; }
        public string ShiftName { get; set; }
        public string OriginType { get; set; }
        public int PrintCounter { get; set; }
        public A1QSystem.Model.Transaction.Transaction Transaction { get; set; }
        public bool MachineActive { get; set; }
        public bool DayActive { get; set; }
        public bool EveningActive { get; set; }
        public bool NightActive { get; set; }
        public bool IsExpanded { get; set; }
        public string ItemPresenterVivibility { get; set; }
        //public bool IsExpandedEnabled { get; set; }
       


        public DateTime FreightArrDate { get; set; }
        public DateTime OrderRequiredDate { get; set; }
        private bool _activeOrder;
        public string FreightArrTime { get; set; }
        private Customer _customer;
        private string _isOrderActive;
        private string _activeText;
        private string _activeOrderBehaviour;
        private int _ordertype;
        private string _rowBackgroundColour;
        private string _freightName;
        private bool _freightTimeAvailable;
        private bool _freightDateAvailable;
        private string _isNotesVisible;
        private string _notes;
        private string _salesOrder;
        private bool _reqDateSelected;
        private string _salesOrderVisibility;
        private string _rawBackgroundColour;
        private string _bottomRowVisibility;
        public string FreightDateTimeVisibility { get; set; }
        public int SalesOrderId { get; set; }
        public string FreightVisiblity { get; set; }
        public string ArrivalDateTime { get; set; }
        public string RequiredDate { get; set; }
        public DateTime MixingDate { get; set; }
        public string MixingShift { get; set; }
        public string IsReqDateVisible { get; set; }
        private string _shiftText;
        private string _shiftBtnBackColour;
        private bool _convertEnable;
        private string _itemBackgroundColour;
        private string _enDisVisibilty;
        private Int32 MixingProdDetID;
        private int _currentShift;
        private List<Machines> MachinesList;
        private int _currentProdTimeTable;
        private string _customerNameVisibility;
        private string _blockLogStockStr;

        private ICommand _moveCommand;
        private ICommand _deleteCommand;
        private ICommand _convertCommand;
        
        private bool canExecute;

        public RawProductionDetails()
        {             
            canExecute = true;
            RowBackgroundColour = "#ffffff";
            IsNotesVisible = "Collapsed";
            MoveEnabled = true;
            DeleteEnabled = true;
            IsReqDateVisible = "Collapsed";
            SalesOrderVisibility = "Collapsed";
            ReqDateSelected = false;
        }
        

        private bool EnableDisableGradingBttn(decimal reqAmo)
        {
            bool res = false;

            if (reqAmo == 0)
            {
                res = false;
            }
            else if (reqAmo > 0)
            {
                res = true;
            }

            return res;
        }

        private string ChangeTextGradingBttn(decimal reqAmo)
        {
            string res = string.Empty;

            if (reqAmo == 0)
            {
                res = "Allocatated";
            }
            else if (reqAmo > 0)
            {
                res = "Allocate";
            }
            return res;
        }

        private void MoveOrder()
        {
            List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
            bool has = systemParameters.Any(x => x.Value == true);
            if (has == true)
            {
                Msg.Show("System is busy, performing some operations. Please try again in few minutes ", "System Busy", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                int sp1 = DBAccess.UpdateSystemParameter("MoveOrder", true);
                if (sp1 > 0)
                {
                    var childWindow = new ChildWindowView();
                    childWindow.ShowShiftOrderWindow(this, OriginType);
                    int sp2 = DBAccess.UpdateSystemParameter("MoveOrder", false);
                }
            }
        }

        private void DeleteOrder()
        {
            List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
            bool has = systemParameters.Any(x => x.Value == true);
            if (has == true)
            {
                Msg.Show("System is busy, performing some operations. Please try again in few minutes ", "System Busy", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {

                int sp1 = DBAccess.UpdateSystemParameter("DeleteOrder", true);
                if (sp1 > 0)
                {
                    if (Msg.Show("Are you sure, you want to delete this order?", "Order Delete Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Information_Red, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                    {
                        List<ShredStock> tempShredStock = new List<ShredStock>();
                        //List<FormulaOptions> formulaOptions = DBAccess.GetFormulaOptions();
                        List<ShredStock> shredStock = DBAccess.GetShredStock();
                        ObservableCollection<Formulas> formulas = DBAccess.GetFormulas();
                        var f = formulas.SingleOrDefault(x => x.RawProductID == RawProduct.RawProductID);
                        if (f != null)
                        {
                            var a = shredStock.SingleOrDefault(z => z.Shred.ID == f.ProductCapacity1);
                            var b = shredStock.SingleOrDefault(y => y.Shred.ID == f.ProductCapacity2);
                            if (a != null || b != null)
                            {
                                if (a != null)
                                {
                                    tempShredStock.Add(new ShredStock() { Shred = new Shred() { ID = f.ProductCapacity1 }, Qty = a.Qty + (f.GradingWeight1 * BlockLogQty) });
                                }

                                if (b != null)
                                {
                                    tempShredStock.Add(new ShredStock() { Shred = new Shred() { ID = f.ProductCapacity2 }, Qty = b.Qty + (f.GradingWeight2 * BlockLogQty) });
                                }
                            }
                        }


                        int res = DBAccess.DeleteGradingOrder(GradingSchedulingID, tempShredStock);
                        if (res == 1)
                        {
                            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                            if (String.IsNullOrEmpty(userName))
                            {
                                userName = "Unknown";
                            }
                            Transaction = new A1QSystem.Model.Transaction.Transaction()
                            {
                                TransDateTime = DateTime.Now,
                                Transtype = "Deleted",
                                SalesOrderID = SalesOrderId,
                                Products = new List<RawStock>()
                                    {
                                      new RawStock(){RawProductID = RawProduct.RawProductID,Qty=BlockLogQty},  
                                    },
                                CreatedBy = userName
                            };
                            int r = DBAccess.InsertTransaction(Transaction);
                            Msg.Show("The selected order has been successfully deleted!", "Order Deleted", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);
                        }
                        else
                        {
                            Msg.Show("There was an error and the Order has not been deleted!", "Order Deleted Failure", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                        }
                    }

                    int sp2 = DBAccess.UpdateSystemParameter("DeleteOrder", false);
                }
            }
        }

        private void EnableDisableShift()
        {
            

            List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
            bool has = systemParameters.Any(x => x.Value == true);
            if (has == true)
            {
                Msg.Show("System is busy, performing some operations. Please try again in few minutes ", "System Busy", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {

                int sp1 = DBAccess.UpdateSystemParameter("DisableEnableDates", true);
                if (sp1 > 0)
                {
                    GradingManager gm = new GradingManager();
                    if (ShiftText == "Enable")
                    {
                        if (Msg.Show("Are you sure, you want to enable this shift?", "Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                        {

                            int r = DBAccess.EnableDisableSingleShift(Convert.ToDateTime(ProductionDate), RawProductMachine.GradingMachineID, true, Shift);
                            if (r > 0)
                            {
                                if (Msg.Show("Do you want to allocate existing orders to this shift?", "Shift Enabling Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                                {
                                    gm.ShiftOrders("Enable", Shift, Convert.ToDateTime(ProductionDate), "ShiftButton");
                                    ShiftText = "Disable";
                                    ShiftBtnBackColour = "Green";

                                }
                            }
                        }
                    }
                    else
                    {
                        if (Msg.Show("Are you sure, you want to disable this shift?" + System.Environment.NewLine + "Existing orders will be allocated to the next available shifts", "Shift Disabling Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                        {
                            int r = DBAccess.EnableDisableSingleShift(Convert.ToDateTime(ProductionDate), RawProductMachine.GradingMachineID, false, Shift);
                            if (r > 0)
                            {
                                gm.ShiftOrders("Disable", Shift, Convert.ToDateTime(ProductionDate), "ShiftButton");
                                ShiftText = "Enable";
                                ShiftBtnBackColour = "#FFCC3700";
                            }
                        }
                    }

                    int sp2 = DBAccess.UpdateSystemParameter("DisableEnableDates", false);
                }
            }
        }

        private void ConvertOrder()
        {
            List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
            bool has = systemParameters.Any(x => x.Value == true);
            if (has == true)
            {
                Msg.Show("System is busy, performing some operations. Please try again in few minutes ", "System Busy", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                int sp1 = DBAccess.UpdateSystemParameter("ConvertOrder", true);
                if (sp1 > 0)
                {
                    var childWindow = new ChildWindowView();
                    childWindow.ShowConvertOrderWindow(this);

                    int sp2 = DBAccess.UpdateSystemParameter("ConvertOrder", false);
                }
            }
        }

        //private void CompleteOrder()
        //{
        //    List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
        //    bool has = systemParameters.Any(x => x.Value == true);
        //    if (has == true)
        //    {
        //        Msg.Show("System is processing some orders at the moment. Please try again in few minutes ", "System Busy", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
        //    }
        //    else
        //    {
        //        int sp1 = DBAccess.UpdateSystemParameter("CompleteGradingOrder", true);
        //        if (sp1 > 0)
        //        {
        //            if (Msg.Show("Are you sure you want to complete 1" + RawProduct.RawProductType + " of " + RawProduct.Description + "?" , "Grading Completing Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
        //            {
        //                _currentProdTimeTable = GetProdTimeTableID();
        //                BackgroundWorker worker = new BackgroundWorker();
        //                ChildWindowView LoadingScreen = new ChildWindowView();
        //                LoadingScreen.ShowWaitingScreen("Processing");

        //                worker.DoWork += (_, __) =>
        //                {
        //                    MixingProdDetID = 0;
        //                    int curProdTimeTableId = GetProdTimeTableID();
        //                    string pcName = System.Environment.MachineName;
        //                    if (string.IsNullOrEmpty(pcName))
        //                    {
        //                        pcName = "Unknown";
        //                    }

        //                    List<Formulas> fList = DBAccess.GetFormulaDetailsByRawProdID(RawProduct.RawProductID);

        //                    foreach (var item in fList)
        //                    {
        //                        List<ProductionTimeTable> pttList = DBAccess.GetProductionTimeTableByID(item.MachineID, Convert.ToDateTime(ProductionDate));
        //                        foreach (var items in pttList)
        //                        {
        //                            MixingProdDetID = items.ID;
        //                        }
        //                    }
        //                    //Get the current shhift
        //                    List<Shift> ShiftDetails = DBAccess.GetAllShifts();
        //                    foreach (var item in ShiftDetails)
        //                    {
        //                        bool isShift = TimeBetween(DateTime.Now, item.StartTime, item.EndTime);

        //                        if (isShift == true)
        //                        {
        //                            _currentShift = item.ShiftID;
        //                        }
        //                    }

        //                    OrderStatus ordeStatus = OrderStatus.None;
        //                    ordeStatus = OrderStatus.Grading;
        //                    ObservableCollection<RawProductionDetails> coll = null;
        //                    RawProductionDetails rpd = new RawProductionDetails();
        //                    rpd.RawProDetailsID = this.ProdTimeTableID;
        //                    rpd.SalesOrderId = this.SalesOrderId;
        //                    rpd.RawProduct = new Products.RawProduct() {RawProductID = this.RawProduct.RawProductID};
        //                    rpd.Shift = this.Shift;
        //                    rpd.OrderType = this.OrderType;

        //                    coll = DBAccess.GetAllRawProductsByID(rpd, ordeStatus);

        //                    if (coll.Count != 0)
        //                    {
        //                        MachinesList = DBAccess.GetNumberOfMachines();
        //                        List<MixingOrder> mixingOrdersList = new List<MixingOrder>();

        //                        foreach (var x in coll)
        //                        {
        //                            if (x.BlockLogQty >= 1)
        //                            {
        //                                MixingOrder mol = new MixingOrder();
        //                                mol.Product = new Product() { ProductID = 0, ProductCode = string.Empty, RawProduct = new Products.RawProduct() { RawProductID = RawProduct.RawProductID }, UnitPrice = 0, ProductUnit = RawProduct.RawProductType };
        //                                mol.Qty = 0;
        //                                mol.Order = new Order() { OrderType = OrderType, OrderNo = SalesOrderId, RequiredDate = Convert.ToDateTime(RequiredDate), SalesNo = SalesOrder, Comments = Notes, Customer = Customer };
        //                                mol.BlocksLogs = 1;
        //                                mixingOrdersList.Add(mol);
        //                                if (mixingOrdersList.Count != 0)
        //                                {
        //                                    Tuple<List<GradingCompleted>, Int32, DateTime> tupleElements = CalculateGradingRubber(this.RawProduct.RawProductID, _currentShift, ShiftDetails);
        //                                    GradingProductionDetails gps = new GradingProductionDetails();
        //                                    gps.RawProDetailsID = this.ProdTimeTableID;
        //                                    gps.SalesOrderId = this.SalesOrderId;
        //                                    gps.RawProduct = new RawProduct() {RawProductID = this.RawProduct.RawProductID };
        //                                    gps.Shift = this.Shift;
        //                                    gps.GradingActive = this.ActiveOrder;

        //                                    int r = DBAccess.UpdateRawProdQty(gps, x.OrderType, _currentShift, _currentProdTimeTable, tupleElements.Item1, pcName, tupleElements.Item2, tupleElements.Item3);
        //                                    if (r > 0 && this.RawProduct.RawProductType != "Box" && this.RawProduct.RawProductType != "BoxPallet")
        //                                    {
        //                                        GradingProductionDetails gpd = null;
        //                                        List<GradingCompleted> ggList = null;                                                

        //                                        ProductionManager.AddToMixing(mixingOrdersList, gpd, 0, 0, 0, ggList, "",0,new DateTime());//Add to mixing                                    
        //                                    }
        //                                    else if (this.RawProduct.RawProductType == "Box")//Add to box stock
        //                                    {
        //                                        List<GradedStock> gsl = new List<GradedStock>();
        //                                        gsl.Add(new GradedStock() { ID = tupleElements.Item1[0].GradingID, Qty = tupleElements.Item1[0].KGCompleted });
        //                                        ProductionManager.AddToGradedStock(gsl);//Add to graded stock   

        //                                        //Update PendingOrder-TODO                                   

        //                                    }
        //                                    else if (this.RawProduct.RawProductType == "BoxPallet")//Update PendingOrder-TODO                      
        //                                    {
        //                                        //Update PendingOrder-TODO   
        //                                        //PendingOrderManager pom = new PendingOrderManager();
        //                                        //pom.UpdatePendingOrder(this.SalesOrderId, this.RawProduct.RawProductID, 0, 1, 1);

        //                                    }
        //                                    else
        //                                    {
        //                                        Console.WriteLine("Didn't add to mixing");
        //                                    }

        //                                    if (r != 0)
        //                                    {

        //                                        /*****Add to Transaction*********************************************************************/
        //                                        string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        //                                        if (String.IsNullOrEmpty(userName))
        //                                        {
        //                                            userName = "Unknown";
        //                                        }
        //                                        Transaction = new A1QSystem.Model.Transaction.Transaction()
        //                                        {
        //                                            TransDateTime = DateTime.Now,
        //                                            Transtype = "Completed (Grading)",
        //                                            SalesOrderID = this.SalesOrderId,
        //                                            Products = new List<RawStock>()
        //                                {
        //                                  new RawStock(){RawProductID = this.RawProduct.RawProductID,Qty=1},  
        //                                },
        //                                            CreatedBy = userName
        //                                        };
        //                                        DBAccess.InsertTransaction(Transaction);
        //                                        /*****Finish adding to Transaction************************************************************/
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    Console.WriteLine("MixingOrdersList is empty grading complete button");
        //                                }
        //                                break;
        //                            }
        //                        }
        //                    }
        //                };

        //                worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
        //                {
        //                    LoadingScreen.CloseWaitingScreen();
        //                };
        //                worker.RunWorkerAsync();

        //            }

        //            int sp2 = DBAccess.UpdateSystemParameter("CompleteGradingOrder", false);
        //        }
        //    }
        //}

        private int GetProdTimeTableID()
        {
            int id = 0;
            DateTime curDate = DateTime.Now.Date;
            List<ProductionTimeTable> prodTT = DBAccess.GetProductionTimeTableByID(1, curDate);
            if (prodTT.Count != 0)
            {
                id = prodTT[0].ID;
            }

            return id;
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

        private Tuple<List<GradingCompleted>, Int32, DateTime> CalculateGradingRubber(int rawProdId, int curShift, List<Shift> shiftDetails)
        {
            Int32 actid = 0;
            List<GradingCompleted> ggCompleted = new List<GradingCompleted>();
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            DateTime realDate = DateTime.Now;

            if (curShift == 3)//checking if still the night shift, if so goes to the previous working day
            {
                TimeSpan midNight = new TimeSpan(00, 00, 00);
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
                if (fList[0].ProductCapacity1 != 0 && fList[0].ProductCapacity2 == 0)
                {
                    ggCompleted.Add(new GradingCompleted() { GradingID = fList[0].ProductCapacity1, KGCompleted = fList[0].GradingWeight1, CreatedDate = realDate, ProdTimeTableID = actid, Shift = curShift, SalesID = SalesOrderId, RawProduct = new Product() { RawProduct = new Products.RawProduct() { RawProductID = RawProduct.RawProductID } }, OrderType = OrderType });
                }
                else if (fList[0].ProductCapacity1 != 0 && fList[0].ProductCapacity2 != 0)
                {
                    ggCompleted.Add(new GradingCompleted() { GradingID = fList[0].ProductCapacity1, KGCompleted = fList[0].GradingWeight1, CreatedDate = realDate, ProdTimeTableID = actid, Shift = curShift, SalesID = SalesOrderId, RawProduct = new Product() { RawProduct = new Products.RawProduct() { RawProductID = RawProduct.RawProductID } }, OrderType = OrderType });
                    ggCompleted.Add(new GradingCompleted() { GradingID = fList[0].ProductCapacity2, KGCompleted = fList[0].GradingWeight2, CreatedDate = realDate, ProdTimeTableID = actid, Shift = curShift, SalesID = SalesOrderId, RawProduct = new Product() { RawProduct = new Products.RawProduct() { RawProductID = RawProduct.RawProductID } }, OrderType = OrderType });
                }
            }

            return Tuple.Create(ggCompleted, actid, realDate);
        }

        private void ViewFormula()
        {
            string formula = string.Empty;
            List<Formulas> f = DBAccess.GetFormulaDetailsByRawProdID(this.RawProduct.RawProductID);
            if(f != null)
            {
                formula = f[0].GradingFormula;
            }

            if (!String.IsNullOrEmpty(formula) || formula == "Not Available")
            {
                var childWindow = new ChildWindowView();

                childWindow.ShowFormula(formula.Replace("\\\\", "/"));
            }
            else
            {
                Msg.Show("Cannot locate the formula for " + RawProduct.Description, "Formula Not Found", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
        }

        #region PUBLIC PROPERTIES


        public string EnDisVisibilty
        {
            get { return _enDisVisibilty; }
            set
            {
                _enDisVisibilty = value;
                RaisePropertyChanged(() => this.EnDisVisibilty);      
            }
        }

        public Customer Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                RaisePropertyChanged(() => this.Customer);      
            }
        }

        public string FreightDescription
        {
            get { return _freightName; }
            set
            {
                _freightName = value;
                if (FreightDescription == "A1Rubber Stock")
                {
                    FreightVisiblity = "Hidden";
                    FreightDateTimeVisibility = "Hidden";
                }
                else
                {
                    FreightVisiblity = "Visible";
                    FreightDateTimeVisibility = "Visible";
                }
            }
        }




        public string ItemBackgroundColour
        {
            get { return _itemBackgroundColour; }
            set
            {
                _itemBackgroundColour = value;
                RaisePropertyChanged(() => this.ItemBackgroundColour);                
            }
        }
        

        public string SalesOrderVisibility
        {
            get { return _salesOrderVisibility; }
            set
            {
                _salesOrderVisibility = value;
                RaisePropertyChanged(() => this.SalesOrderVisibility);                
            }
        }

        public string SalesOrder
        {
            get { return _salesOrder; }
            set
            {
                _salesOrder = value;
                RaisePropertyChanged(() => this.SalesOrder);

                if (String.IsNullOrEmpty(SalesOrder))
                {
                    SalesOrderVisibility = "Collapsed";
                }
                else
                {
                    SalesOrderVisibility = "Visible";
                }
            }
        }


        public bool ConvertEnable
        {
            get { return _convertEnable; }
            set
            {
                _convertEnable = value;
                RaisePropertyChanged(() => this.ConvertEnable);
            }
        }

        public string ShiftBtnBackColour
        {
            get { return _shiftBtnBackColour; }
            set
            {
                _shiftBtnBackColour = value;
                RaisePropertyChanged(() => this.ShiftBtnBackColour);
            }
        }

        public bool ReqDateSelected
        {
            get { return _reqDateSelected; }
            set
            {
                _reqDateSelected = value;
                RaisePropertyChanged(() => this.ReqDateSelected);
            }
        }
        
        public string ActiveText
        {
            get { return _activeText; }
            set
            {
                _activeText = value;
                RaisePropertyChanged(() => this.ActiveText);
            }
        }

        public bool ActiveOrder
        {
            get { return _activeOrder; }
            set
            {
                _activeOrder = value;
                RaisePropertyChanged(() => this.ActiveOrder);

                if(ActiveOrder == true)
                {
                    IsOrderActive = "Visible";
                    ActiveOrderBehaviour = "Forever";
                    ActiveText = "Now Working";
                    //MoveEnabled = false;
                    //DeleteEnabled = false;
                    //ConvertEnable = false;
                }
                else
                {
                    //IsOrderActive = "Collapsed";
                    ActiveOrderBehaviour = "1x";
                    ActiveText = string.Empty;
                    //MoveEnabled = true;
                    //DeleteEnabled = true;
                    //ConvertEnable = true;
                }
            }
        }

         

        public string ActiveOrderBehaviour
        {
            get { return _activeOrderBehaviour; }
            set
            {
                _activeOrderBehaviour = value;
                RaisePropertyChanged(() => this.ActiveOrderBehaviour);
            }
        }

        public string RowBackgroundColour
        {
            get { return _rowBackgroundColour; }
            set
            {
                _rowBackgroundColour = value;
                RaisePropertyChanged(() => this.RowBackgroundColour);
            }
        }

        
        public bool FreightTimeAvailable
        {
            get { return _freightTimeAvailable; }
            set
            {
                _freightTimeAvailable = value;
                if (FreightTimeAvailable == true)
                {
                    FreightArrTime = string.Empty;
                }
            }
        }

        public bool FreightDateAvailable
        {
            get { return _freightDateAvailable; }
            set
            {
                _freightDateAvailable = value;
                if (FreightDateAvailable == true)
                {
                    FreightDateTimeVisibility = "Hidden";
                }
                else
                {
                    FreightDateTimeVisibility = "Visible";
                }
            }
        }

        public decimal BlockLogQty
        {
            get { return _blockLogQty; }
            set
            {
                _blockLogQty = value;
                RaisePropertyChanged(() => this.BlockLogQty);
            }
        }

        public int RawProDetailsID
        {
            get { return _RawProDetailsID; }
            set
            {
                _RawProDetailsID = value;
                RaisePropertyChanged(() => this.RawProDetailsID);
            }
        }

        public int OrderType
        {
            get { return _ordertype; }
            set
            {
                _ordertype = value;
                RaisePropertyChanged(() => this.OrderType);

                //Order Types:
                //1 - Urgent
                //2 - Urgent Graded 
                //3 - Normal
                //4 - Normal Graded


                if(OrderType == 1)
                {
                    IsOrderActive = "Visible";
                    RowBackgroundColour = "#cc3700";//Urgent                   
                }
                if(OrderType == 2)
                {
                    IsOrderActive = "Visible";
                    RowBackgroundColour = "#175a44";//Urgent Graded                    
                }
                if (OrderType == 3)
                {
                    RowBackgroundColour = "#ffffff";//Normal
                    
                    if (ActiveOrder == true)
                    {
                        IsOrderActive = "Visible";
                    }
                    else
                    {
                        IsOrderActive = "Collapsed";
                    }
                }
                if (OrderType == 4)
                {
                    IsOrderActive = "Visible";
                    RowBackgroundColour = "#3cce5f";//Normal    
                    //ConvertEnable = false;
                }
                else
                {
                    //ConvertEnable = true;
                }
            }
        }
        
     
        public RawProduct RawProduct
        {
            get { return _rawProduct; }
            set
            {
                _rawProduct = value;
                RaisePropertyChanged(() => this.RawProduct);
                //if (RawProduct.RawProductID == 12)
                //{
                //    ConvertEnable = false;
                //}
                //else
                //{
                //    ConvertEnable = true;
                //}
            }
        }

        public string IsNotesVisible
        {
            get { return _isNotesVisible; }
            set
            {
                _isNotesVisible = value;
                RaisePropertyChanged(() => this.IsNotesVisible);
            }
        }

        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                RaisePropertyChanged(() => this.Notes);
                if (String.IsNullOrEmpty(Notes))
                {
                    IsNotesVisible = "Collapsed";
                }
                else
                {
                    IsNotesVisible = "Visible";
                }
            }
        }

        public string IsOrderActive
        {
            get { return _isOrderActive; }
            set
            {
                _isOrderActive = value;
                RaisePropertyChanged(() => this.IsOrderActive);
               
            }
        }

         public bool MoveEnabled
        {
            get { return _moveEnabled; }
            set
            {
                _moveEnabled = value;
                RaisePropertyChanged(() => this.MoveEnabled);
               
            }
        }

        public bool DeleteEnabled
        {
            get { return _deleteEnabled; }
            set
            {
                _deleteEnabled = value;
                RaisePropertyChanged(() => this.DeleteEnabled);
               
            }
        }

        public string ShiftText
        {
            get { return _shiftText; }
            set
            {
                _shiftText = value;
                RaisePropertyChanged(() => this.ShiftText);

            }
        }

        public RawProductMachine RawProductMachine
        {
            get { return _rawProductMachine; }
            set
            {
                _rawProductMachine = value;
                RaisePropertyChanged(() => this.RawProductMachine);

            }
        }

        public string BlockLogStockStr
        {
            get { return _blockLogStockStr; }
            set
            {
                _blockLogStockStr = value;
                RaisePropertyChanged(() => this.BlockLogStockStr);
            }
        }

        public string CustomerNameVisibility
        {
            get { return _customerNameVisibility; }
            set
            {
                _customerNameVisibility = value;
                RaisePropertyChanged(() => this.CustomerNameVisibility);
            }
        }

        public string RawBackgroundColour
        {
            get { return _rawBackgroundColour; }
            set
            {
                _rawBackgroundColour = value;
                RaisePropertyChanged(() => this.RawBackgroundColour);
            }
        }

        public string BottomRowVisibility
        {
            get { return _bottomRowVisibility; }
            set
            {
                _bottomRowVisibility = value;
                RaisePropertyChanged(() => this.BottomRowVisibility);
            }
        }

        #endregion

        #region COMMANDS
        //public ICommand AcceptCommand
        //{
        //    get
        //    {
        //        return _acceptCommand ?? (_acceptCommand = new LogOutCommandHandler(() => AcceptOrder(), canExecute));
        //    }
        //}
        public ICommand MoveCommand
        {
            get
            {
                return _moveCommand ?? (_moveCommand = new LogOutCommandHandler(() => MoveOrder(), canExecute));
            }
        }
        public ICommand DeleteCommand
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new LogOutCommandHandler(() => DeleteOrder(), canExecute));
            }
        }

        private ICommand _enDisCommand;
        public ICommand EnDisCommand
        {
            get
            {
                return _enDisCommand ?? (_enDisCommand = new LogOutCommandHandler(() => EnableDisableShift(), canExecute));
            }
        }

        public ICommand ConvertCommand
        {
            get
            {
                return _convertCommand ?? (_convertCommand = new LogOutCommandHandler(() => ConvertOrder(), canExecute));
            }
        }

        private ICommand _completeCommand;
        public ICommand CompleteCommand
        {
            get
            {
                return null;// _completeCommand ?? (_completeCommand = new LogOutCommandHandler(() => CompleteOrder(), canExecute));
            }
        }

        private ICommand _viewCommand;
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
