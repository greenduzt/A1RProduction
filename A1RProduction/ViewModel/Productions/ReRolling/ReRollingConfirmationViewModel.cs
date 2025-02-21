using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Other;
using A1QSystem.Model.Production.ReRoll;
using A1QSystem.Model.Products;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Productions.ReRolling
{
    public class ReRollingConfirmationViewModel : ViewModelBase
    {
        public ReRollingOrder reRollingProduction { get; set; }
        public List<int> _maxLogCount { get; set; }
        public int _noOfLogsReRolled { get; set; }
        public decimal width;
        public decimal complDollarVal;
        public int currentShift;
        private string _productName { get; set; }
        private string _productSize { get; set; }
        private string _productDensity { get; set; }
        private string _productThickness { get; set; }
        private string _selectedShortRolls;
        private string _selectedOffSpec;
        private string _shortRollGridVisible;
        private string _offSpecGridVisible;
        private string _reRollQtyAvailable;
        private string _labelOffSpecMargin;
        private string _comboOffSpecMargin;
        private string _offSpecRollLabelMargin;
        private string _offSpecGridMargin;
        private bool _submitEnabled;
        private List<ProductMeterage> prodMeterageList;
        private ObservableCollection<ShortRoll> _shortRollCollection;
        private ObservableCollection<OffSpec> _offSpecCollection;

        public event Action Closed;
        private ICommand _closeCommand;
        private ICommand _submitCommand;


        public ReRollingConfirmationViewModel(ReRollingOrder reRollingProd)
        {
            reRollingProduction = reRollingProd;
            MaxLogCount = new List<int>();
            decimal maxLogs = reRollingProduction.Rolls;
            for (int i = 0; i <= maxLogs; i++)
            {
                MaxLogCount.Add(i);
            }
            complDollarVal = 0;

            ProductName = reRollingProduction.Product.ProductName;
            ProductDensity = reRollingProduction.Product.Density;
            ProductSize = reRollingProduction.Product.Width.ToString("G29") + " x " + reRollingProduction.Product.Tile.MaxYield.ToString("G29");
            ProductThickness = reRollingProduction.Product.Tile.Thickness + "mm";
            SubmitEnabled = false;
            SelectedShortRolls = "0";
            SelectedOffSpec = "0";
            ShortRollCollection = new ObservableCollection<ShortRoll>();
            OffSpecCollection = new ObservableCollection<OffSpec>();
            ShortRollGridVisible = "Collapsed";
            OffSpecGridVisible = "Collapsed";
            ReRollQtyAvailable = "Collapsed";
            LabelOffSpecMargin = "8,244,0,0";
            ComboOffSpecMargin = "147,244,0,0";
            OffSpecRollLabelMargin = "8,273,0,0";
            OffSpecGridMargin = "147,280,0,0";
            width = Convert.ToDecimal(reRollingProduction.Product.Width.ToString("G29"));

            ProductMeterage pm = new ProductMeterage();
            pm.Thickness = reRollingProduction.Product.Tile.Thickness;
            pm.MouldType = reRollingProduction.Product.MouldType;
            pm.MouldSize = reRollingProduction.Product.Width;
            prodMeterageList = DBAccess.GetProductMeterageByValues(pm);
        }

        public void SubmitOrder()
        {
            //Check if it safe to complete
            List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
            bool has = systemParameters.Any(x => x.Value == true && (x.ParameterCode == "ChangingShiftForReRolling" || x.ParameterCode == "PeelingCompletion"));
            if (has == true)
            {
                Msg.Show("System is performing some updates. Please try again in few minutes ", "System Busy", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                //Short Rolls
                int sr = Convert.ToInt16(SelectedShortRolls);
                int osr = Convert.ToInt16(SelectedOffSpec);
                bool hasError = false;
                string error = "Please correct the following errors";
                if (sr > 0)
                {
                    error += System.Environment.NewLine + System.Environment.NewLine;

                    foreach (var item in ShortRollCollection)
                    {
                        if (item.LM == 0)
                        {
                            error += "- Short Roll " + item.RollNo + " requires a valid value in L/m" + System.Environment.NewLine;
                            hasError = true;
                        }
                    }
                }

                //Off-spec
                if (osr > 0)
                {
                    if (hasError == false)
                    {
                        error += System.Environment.NewLine + System.Environment.NewLine;
                    }

                    foreach (var item in OffSpecCollection)
                    {
                        if (item.LM == 0)
                        {
                            error += "- Off-spec Roll " + item.RollNo + " requires a valid value in L/m" + System.Environment.NewLine;
                            hasError = true;
                        }
                    }

                    foreach (var item in OffSpecCollection)
                    {
                        if (item.IsContaminated == false && item.IsOperatorError == false && item.IsOther == false && item.IsTooThick == false && item.IsTooThin == false)
                        {
                            error += "- Off-spec Roll " + item.RollNo + " requires a reason" + System.Environment.NewLine;
                            hasError = true;
                        }
                    }
                }

                if (hasError)
                {
                    Msg.Show(error, "Please Input Correct Values", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }
                else
                {
                    if (Msg.Show("Have you re-rolled the whole log?" + System.Environment.NewLine + "The whole log must be re-rolled to complete the order", "Order Completion", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                    {
                        //Get the current shift
                        ShiftManager sm = new ShiftManager();
                        currentShift = sm.GetCurrentShift();

                        //Calculate dollar value for the completed rolls
                        complDollarVal = reRollingProduction.Product.UnitPrice * NoOfLogsReRolled;
                        //Go to database
                        int res = DBAccess.ReRollingCompleted(this);
                        if (res > 0)
                        {
                            //Update PendingSlitPeel if requires
                            decimal rollSCompleted = 0;
                            PendingOrder orderDetails = DBAccess.GetPendingOrder(reRollingProduction.Order.OrderNo, reRollingProduction.Product.RawProduct.RawProductID, reRollingProduction.Product.ProductID);
                            List<ReRollingOrder> reRollingList = DBAccess.GetReRollingOrdersByID(reRollingProduction.Order.OrderNo, reRollingProduction.Product.ProductID, reRollingProduction.Product.RawProduct.RawProductID);
                            rollSCompleted = DBAccess.GetTotalReRollingCompleted(reRollingProduction.Order.OrderNo, reRollingProduction.Product.ProductID);
                            decimal rollsToComplete = reRollingList.Sum(x => x.Rolls);

                            decimal rollsToUpdate = orderDetails.Qty - (rollSCompleted + rollsToComplete);
                            PendingSlitPeel ps = new PendingSlitPeel();
                            ps.Product = new Product() { ProductID = reRollingProduction.Product.ProductID, RawProduct = new RawProduct() { RawProductID = reRollingProduction.Product.RawProduct.RawProductID } };
                            ps.Order = reRollingProduction.Order;
                            ps.Qty = rollsToUpdate;

                            if (rollsToUpdate > 0)
                            {
                                decimal perLog = Math.Floor(prodMeterageList[0].ExpectedYield / reRollingProduction.Product.Tile.MaxYield);
                                ps.Qty = rollsToUpdate;
                                ps.BlockLogQty = Math.Ceiling(rollsToUpdate / perLog);
                            }
                            else
                            {
                                ps.Qty = 0;
                                ps.BlockLogQty = 0;
                            }

                            int res2 = DBAccess.UpdatePendingSlitPeel(ps,0);
                            if (res2 > 0)
                            {
                                Console.WriteLine("PendingSlitPeel has been updated back from Re-Rolling");

                            }
                            //Check if the order has been completed
                            rollSCompleted = DBAccess.GetTotalReRollingCompleted(reRollingProduction.Order.OrderNo, reRollingProduction.Product.ProductID);
                            PendingOrder po2 = DBAccess.GetPendingOrder(reRollingProduction.Order.OrderNo, reRollingProduction.Product.RawProduct.RawProductID, reRollingProduction.Product.ProductID);
                            if (rollSCompleted >= po2.Qty)
                            {
                                //Update PendingOrder status to ReRolling Completed

                                int s2 = DBAccess.UpdatePendingOrder(reRollingProduction.Order.OrderNo, reRollingProduction.Product.RawProduct.RawProductID, reRollingProduction.Product.ProductID, "ReRollingCompleted");
                            }

                            //Update the status of the completed re-rolling order
                            int upRes = DBAccess.UpdateReRollingStatus(reRollingProduction.ID, "ReRollingCompleted");

                            decimal rollsPending = DBAccess.GetSumOfReRollingOrders(reRollingProduction.Order.OrderNo, reRollingProduction.Product.ProductID, reRollingProduction.Product.RawProduct.RawProductID, "ReRolling");
                            if (rollsPending <= 0)//If all the rolls are completed in ReRolling
                            {
                                if (po2.Qty > rollSCompleted)//still rolls left in pending orders
                                {
                                    //Update PendingSlitPeel active =false
                                    DBAccess.UpdatePendingSlitPeelActive(ps, false);

                                    Msg.Show("Total no of " + rollSCompleted.ToString("G29") + " rolls have been re-rolled for the Order - " + reRollingProduction.Order.OrderNo + System.Environment.NewLine +
                                             "But, the order requires another " + (po2.Qty - rollSCompleted).ToString("G29") + " rolls for " + reRollingProduction.Product.ProductDescription + System.Environment.NewLine +
                                             "Please contact administration", "Ordered qty has not been met for the order - " + reRollingProduction.Order.OrderNo, MsgBoxButtons.OK, MsgBoxImage.Information_Red);
                                }
                            }
                            //DBAccess.InsertErrorLog(DateTime.Now + " Re-Rolling error | Sales ID " + reRollingProduction.Order.OrderNo + " | ProductID " + reRollingProduction.Product.ProductID.ToString());
                        }
                    }

                    CloseForm();
                }
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        #region PUBLIC_PROPERTIES

        public string OffSpecRollLabelMargin
        {
            get { return _offSpecRollLabelMargin; }
            set { _offSpecRollLabelMargin = value; RaisePropertyChanged(() => this.OffSpecRollLabelMargin); }
        }

        public string OffSpecGridMargin
        {
            get { return _offSpecGridMargin; }
            set { _offSpecGridMargin = value; RaisePropertyChanged(() => this.OffSpecGridMargin); }
        }

        public string LabelOffSpecMargin
        {
            get { return _labelOffSpecMargin; }
            set { _labelOffSpecMargin = value; RaisePropertyChanged(() => this.LabelOffSpecMargin); }
        }

        public string ComboOffSpecMargin
        {
            get { return _comboOffSpecMargin; }
            set { _comboOffSpecMargin = value; RaisePropertyChanged(() => this.ComboOffSpecMargin); }
        }

        public string ReRollQtyAvailable
        {
            get { return _reRollQtyAvailable; }
            set { _reRollQtyAvailable = value; RaisePropertyChanged(() => this.ReRollQtyAvailable); }
        }

        public string ShortRollGridVisible
        {
            get { return _shortRollGridVisible; }
            set { _shortRollGridVisible = value; RaisePropertyChanged(() => this.ShortRollGridVisible); }
        }

        public string OffSpecGridVisible
        {
            get { return _offSpecGridVisible; }
            set { _offSpecGridVisible = value; RaisePropertyChanged(() => this.OffSpecGridVisible); }
        }

        public string ProductName
        {
            get { return _productName; }
            set { _productName = value; RaisePropertyChanged(() => this.ProductName); }
        }

        public string ProductSize
        {
            get { return _productSize; }
            set { _productSize = value; RaisePropertyChanged(() => this.ProductSize); }
        }

        public string ProductDensity
        {
            get { return _productDensity; }
            set { _productDensity = value; RaisePropertyChanged(() => this.ProductDensity); }
        }

        public string ProductThickness
        {
            get { return _productThickness; }
            set { _productThickness = value; RaisePropertyChanged(() => this.ProductThickness); }
        }

        public string SelectedShortRolls
        {
            get { return _selectedShortRolls; }
            set
            {
                _selectedShortRolls = value; RaisePropertyChanged(() => this.SelectedShortRolls);
                if (!String.IsNullOrEmpty(SelectedShortRolls))
                {
                    int x = Convert.ToInt16(SelectedShortRolls);
                    if (x > 0)
                    {
                        ShortRollGridVisible = "Visible";
                        LabelOffSpecMargin = "8,431,0,0";
                        ComboOffSpecMargin = "147,432,0,0";
                        OffSpecRollLabelMargin = "8,462,0,0";
                        OffSpecGridMargin = "147,460,0,0";
                        if (ShortRollCollection != null)
                        {
                            ShortRollCollection.Clear();
                        }

                        for (int i = 1; i <= x; i++)
                        {
                            ShortRollCollection.Add(new ShortRoll() { RollNo = i, LM = 0 });
                        }
                    }
                    else
                    {
                        ShortRollGridVisible = "Collapsed";
                        LabelOffSpecMargin = "8,244,0,0";
                        ComboOffSpecMargin = "147,244,0,0";
                        OffSpecRollLabelMargin = "8,273,0,0";
                        OffSpecGridMargin = "147,280,0,0";
                    }
                }
                else
                {
                    ShortRollGridVisible = "Collapsed";
                }
            }
        }

        public ObservableCollection<ShortRoll> ShortRollCollection
        {
            get { return _shortRollCollection; }
            set
            {
                _shortRollCollection = value; RaisePropertyChanged(() => this.ShortRollCollection);
            }
        }


        public ObservableCollection<OffSpec> OffSpecCollection
        {
            get { return _offSpecCollection; }
            set
            {
                _offSpecCollection = value; RaisePropertyChanged(() => this.OffSpecCollection);
            }
        }


        public string SelectedOffSpec
        {
            get { return _selectedOffSpec; }
            set
            {
                _selectedOffSpec = value; RaisePropertyChanged(() => this.SelectedOffSpec);
                if (!String.IsNullOrEmpty(SelectedOffSpec))
                {
                    int x = Convert.ToInt16(SelectedOffSpec);
                    if (x > 0)
                    {
                        OffSpecGridVisible = "Visible";
                        if (OffSpecCollection != null)
                        {
                            OffSpecCollection.Clear();
                        }

                        for (int i = 1; i <= x; i++)
                        {
                            OffSpecCollection.Add(new OffSpec() { RollNo = i, LM = 0 });
                        }
                    }
                    else
                    {
                        OffSpecGridVisible = "Collapsed";
                    }
                }
                else
                {
                    OffSpecGridVisible = "Collapsed";
                }
            }
        }

        public List<int> MaxLogCount
        {
            get
            {
                return _maxLogCount;
            }
            set
            {
                _maxLogCount = value;
                RaisePropertyChanged(() => this.MaxLogCount);
            }
        }

        public int NoOfLogsReRolled
        {
            get
            {
                return _noOfLogsReRolled;
            }
            set
            {
                _noOfLogsReRolled = value;
                RaisePropertyChanged(() => this.NoOfLogsReRolled);
                if (NoOfLogsReRolled > 0)
                {
                    ReRollQtyAvailable = "Visible";
                    SubmitEnabled = true;
                }
                else
                {
                    SubmitEnabled = false;
                    SelectedShortRolls = "0";
                    SelectedOffSpec = "0";
                    ReRollQtyAvailable = "Collapsed";
                    ShortRollGridVisible = "Collapsed";
                    OffSpecGridVisible = "Collapsed";

                }
            }
        }


        public bool SubmitEnabled
        {
            get
            {
                return _submitEnabled;
            }
            set
            {
                _submitEnabled = value;
                RaisePropertyChanged(() => this.SubmitEnabled);
            }
        }

        #endregion

        #region COMMANDS
        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new LogOutCommandHandler(() => CloseForm(), true));
            }
        }

        public ICommand SubmitCommand
        {
            get
            {
                return _submitCommand ?? (_submitCommand = new LogOutCommandHandler(() => SubmitOrder(), true));
            }
        }
        #endregion


    }
}