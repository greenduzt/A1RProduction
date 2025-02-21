using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Production.Slitting;
using A1QSystem.Model.Products;
using A1QSystem.Model.Stock;
using A1QSystem.PDFGeneration;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Productions.Slitting
{
    public class SlittingConfirmationViewModel : ViewModelBase
    {
        private CalculationModel calculation;
        private string lastOperation;
        private bool newDisplayRequired = false;
        private DelegateCommand<string> _digitButtonPressCommand;
        private string _qty;
        private string textBoxType;

        public event Action Closed;
        private DelegateCommand _closeCommand;
        private ICommand _printYieldCommand;
        private ICommand _printOffSpecCommand;
        private ICommand _submitCommand;
        private decimal _totalYieldCut;
        private decimal _offSpecTiles;
        //private decimal _shreddedTiles;
        //private string _shreddedReasonsVisiblity;
        //private string _shreddedLabelMargin;
        private string _shreddedTextBoxMargin;
        //private decimal _extraTiles;
        private string _tileForeGColor;
        //private string _shreddedGridMargin;
        private bool _isContaminated;
        private bool _isLiftedOffBoard;
        private bool _isUnevenThickness;
        private bool _isTooThick;
        private bool _isTooThin;
        private bool _isStonelines;
        private bool _isDamaged;
        private bool _isOther;
        private bool _isOperatorError;

        //private bool _isContaminatedShredded;
        //private bool _isLiftedOffBoardShredded;
        //private bool _isUnevenThicknessShredded;
        //private bool _isTooThickShredded;
        //private bool _isTooThinShredded;
        //private bool _isStonelinesShredded;
        //private bool _isDamagedShredded;
        //private bool _isOtherShredded;
        //private bool _isOperatorErrorShredded;

        private bool canExecute;
        private bool _totYieldCutPrint;
        private bool _offSpecPrint;
        private bool _offSpecEnabled;
        private bool _totYieldCutEnabled;
        //private bool _shreddedEnabled;
        private bool _submitEnabled;
        private List<int> _maxBlockCount;
        private int _noOfBlocksSlit;
        private decimal blocksToUpdate;
        private decimal tilesToUpdate;
        private decimal soTiles;
        private decimal soBlocks;
        private decimal soDollarVal;
        private decimal pspTiles;
        private decimal pspBlocks;
        private decimal pspDollarVal;
        public int currentShift;
        private decimal totMaxYieldCut;
        private string _offSpeccReasonsVisiblity;
        public SlittingOrder slittingProductionDetails { get; set; }
        public decimal blocksCompleted;
        public decimal dollarsCompleted;
        private string _txtOtherVisibility;
        private string _otherText;

        public SlittingConfirmationViewModel(SlittingOrder so)
        {
            slittingProductionDetails = so;
            _closeCommand = new DelegateCommand(CloseForm);

            canExecute = true;
            MaxBlockCount = new List<int>();
            OffSpeccReasonsVisiblity = "Collapsed";
            TxtOtherVisibility = "Collapsed";
            OtherText = string.Empty;
            //_shreddedReasonsVisiblity = "Collapsed";
            //ShreddedLabelMargin = "122,169,0,0";
            //ShreddedTextBoxMargin = "271,172,0,0";
            //ShreddedGridMargin = "5,189,-5,0";
            decimal maxB = slittingProductionDetails.Blocks;

            for (int i = 0; i <= maxB; i++)
            {
                MaxBlockCount.Add(i);
            }
            NoOfBlocksSlit = 0;
            OffSpecEnabled = false;
            TotYieldCutEnabled = false;
            //ShreddedEnabled = false;
            SubmitEnabled = false;

            this.calculation = new CalculationModel();
            this._qty = "0";
            this.FirstOperand = string.Empty;
            this.SecondOperand = string.Empty;
            this.Operation = string.Empty;
            this.lastOperation = string.Empty;
            this.fullExpression = string.Empty;
        }

        public void SubmitOrder()
        {
            totMaxYieldCut = Math.Floor(NoOfBlocksSlit * slittingProductionDetails.Product.Tile.MaxYield);

            if (NoOfBlocksSlit == 0)
            {
                Msg.Show("Select the no of blocks slit", "Selection Required", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (Convert.ToDecimal(OffSpecTilesStr)== 0 && Convert.ToDecimal(TotalYieldCutStr) == 0)
            {
                Msg.Show("Input required for either of yield cut, Off-Spec or Shredding tiles", "Input Required", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (Convert.ToDecimal(TotalYieldCutStr) > totMaxYieldCut)
            {
                Msg.Show("Total yield cut cannot be greater than " + totMaxYieldCut + " tiles", "Invalid Input", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if ((Convert.ToDecimal(TotalYieldCutStr) + Convert.ToDecimal(OffSpecTilesStr)) > totMaxYieldCut)
            {
                Msg.Show("Total for both yield cut and off-spec cannot be greater than " + totMaxYieldCut + " tiles", "Invalid Input", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (Convert.ToDecimal(TotalYieldCutStr) > totMaxYieldCut)
            {
                Msg.Show("Total for both yield cut and shredded cannot be greater than " + totMaxYieldCut + " tiles", "Invalid Input", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }            
            else if ((Convert.ToDecimal(OffSpecTilesStr) != 0) && (IsLiftedOffBoard == false && IsUnevenThickness == false && IsTooThick == false && IsTooThin == false && IsStonelines == false && IsDamaged == false && IsContaminated == false && IsOperatorError == false && IsOther == false))
            {
                Msg.Show("Tick reasons for adding tiles to Off-Spec", "Selection Required", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if(Convert.ToDecimal(OffSpecTilesStr) > 0 && IsOther == true && String.IsNullOrWhiteSpace(OtherText))
            {
                Msg.Show("Enter a reason for adding Off-Spec ", "Reason Required", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            //else if ((Convert.ToDecimal(ShredTilesStr) != 0) && (IsLiftedOffBoardShredded == false && IsUnevenThicknessShredded == false && IsTooThickShredded == false && IsTooThinShredded == false && IsStonelinesShredded == false && IsDamagedShredded == false && IsContaminatedShredded == false && IsOperatorErrorShredded == false && IsOtherShredded == false))
            //{
            //    Msg.Show("Tick reasons for adding tiles to Shredding", "Selection Required", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            //}           
            else
            {
                TotalYieldCut = Convert.ToDecimal(TotalYieldCutStr);
                OffSpecTiles = Convert.ToDecimal(OffSpecTilesStr);
                //ShreddedTiles = Convert.ToDecimal(ShredTilesStr);
                
                if(IsOther ==false)
                {
                    OtherText = string.Empty;
                }

                ShiftManager sm = new ShiftManager();
                SlittingOrder so1 = DBAccess.GetSlittingOrderByID(slittingProductionDetails.ID);
                PendingSlitPeel ps = DBAccess.GetPendingSlittingOrder(this);
                PendingOrder po = DBAccess.GetPendingOrder(slittingProductionDetails.Order.OrderNo, slittingProductionDetails.Product.RawProduct.RawProductID, slittingProductionDetails.Product.ProductID);
                currentShift = sm.GetCurrentShift();

                /**Updating SlittingOrder**/
                SlittingOrder soUpdate = new SlittingOrder();
                soUpdate.Blocks = so1.Blocks - NoOfBlocksSlit;
                soUpdate.Qty = (so1.Qty - Math.Floor((NoOfBlocksSlit * slittingProductionDetails.Product.Tile.MaxYield))) < 0 ? 0 : so1.Qty - Math.Floor((NoOfBlocksSlit * slittingProductionDetails.Product.Tile.MaxYield));
                soUpdate.DollarValue = so1.DollarValue - (Math.Floor((NoOfBlocksSlit * slittingProductionDetails.Product.Tile.MaxYield)) * slittingProductionDetails.Product.UnitPrice) < 0 ? 0 : so1.DollarValue - (Math.Floor((NoOfBlocksSlit * slittingProductionDetails.Product.Tile.MaxYield)) * slittingProductionDetails.Product.UnitPrice);
                //Update SlittingOrder
                int r = DBAccess.UpdateSlittingOrder(this, soUpdate);
                if (r > 0)
                {
                    /**Updating PendingSlitPeel**/
                    List<SlittingOrder> slittingOrderList = DBAccess.GetAllSlittingOrderByID(slittingProductionDetails.Order.OrderNo, slittingProductionDetails.Product.ProductID, slittingProductionDetails.Product.RawProduct.RawProductID);
                    decimal existingTiles = slittingOrderList.Sum(x=>x.Qty);
                    blocksCompleted = BlockLogCalculator(TotalYieldCut, slittingProductionDetails.Product.Tile.MaxYield);
                    dollarsCompleted = TotalYieldCut * slittingProductionDetails.Product.UnitPrice;

                    //Calculating difference to update PendingSlitPeel
                    decimal totTilesCompleted = 0;
                    Tuple<decimal,decimal> e1 = DBAccess.GetTotalSlittingCompleted(slittingProductionDetails.Order.OrderNo, slittingProductionDetails.Product.RawProduct.RawProductID, slittingProductionDetails.Product.ProductID);
                    totTilesCompleted = e1.Item1;
                    pspTiles = (po.Qty - (totTilesCompleted + existingTiles)) > 0 ? po.Qty - (totTilesCompleted + existingTiles) : 0;
                    
                        pspBlocks = BlockLogCalculator(pspTiles, slittingProductionDetails.Product.Tile.MaxYield);
                        //Now update PendingSlitPeel table
                        PendingSlitPeel psp = new PendingSlitPeel();
                        psp.Order = new Order(){OrderNo = slittingProductionDetails.Order.OrderNo};
                        psp.Product = new Product(){ProductID = slittingProductionDetails.Product.ProductID, RawProduct = new RawProduct(){RawProductID = slittingProductionDetails.Product.RawProduct.RawProductID}};
                        psp.BlockLogQty = BlockLogCalculator(pspTiles, slittingProductionDetails.Product.Tile.MaxYield);
                        psp.Qty = pspTiles;
                        DBAccess.UpdatePendingSlitPeel(psp,TotalYieldCut);

                    //Check if the order has been completed
                    Tuple<decimal, decimal> e2 = DBAccess.GetTotalSlittingCompleted(slittingProductionDetails.Order.OrderNo, slittingProductionDetails.Product.RawProduct.RawProductID, slittingProductionDetails.Product.ProductID);
                    totTilesCompleted = e2.Item1;
                    PendingOrder po2 = DBAccess.GetPendingOrder(slittingProductionDetails.Order.OrderNo, slittingProductionDetails.Product.RawProduct.RawProductID, slittingProductionDetails.Product.ProductID);
                    if(totTilesCompleted >=po2.Qty)
                    {
                        //Update PendingOrder status to SlittingCompleted
                        int s2 = DBAccess.UpdatePendingOrder(slittingProductionDetails.Order.OrderNo, slittingProductionDetails.Product.RawProduct.RawProductID, slittingProductionDetails.Product.ProductID,"SlittingCompleted");
                    }   
                    
                    //Display message if the ordered qty has not been met
                    if(po.BlockLogQty == e2.Item2 && po.Qty > e2.Item1)
                    {
                        //Update PendingSlitPeel active =false
                        DBAccess.UpdatePendingSlitPeelActive(psp,false);

                        Msg.Show("Total blocks of " + e2.Item2.ToString("G29") + " have been slit for the Order - " + slittingProductionDetails.Order.OrderNo + System.Environment.NewLine +
                                 "Total no of " + e2.Item1.ToString("G29") + " tiles were slit for " + slittingProductionDetails.Product.ProductDescription + System.Environment.NewLine + 
                                 "This order requires another " + (po.Qty - e2.Item1).ToString("G29") + " tiles" + System.Environment.NewLine +
                                 "Please contact administration", "Ordered qty has not been met for the order - " + slittingProductionDetails.Order.OrderNo, MsgBoxButtons.OK, MsgBoxImage.Information_Red);
                    }
                    else if (po.BlockLogQty == e2.Item2 && po.Qty <= e2.Item1)
                    {
                        //Update PendingSlitPeel active =false
                        DBAccess.UpdatePendingSlitPeelActive(psp, false);
                    }

                    //Add a transaction record
                    string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    if (String.IsNullOrEmpty(userName))
                    {
                        userName = "Unknown";
                    }

                    A1QSystem.Model.Transaction.Transaction Transaction = new A1QSystem.Model.Transaction.Transaction()
                    {
                        TransDateTime = DateTime.Now,
                        Transtype = "Completed(Slitting)",
                        SalesOrderID = slittingProductionDetails.Order.OrderNo,
                        Products = new List<RawStock>()
                        {
                            new RawStock(){RawProductID = slittingProductionDetails.Product.ProductID,Qty=Convert.ToDecimal(TotalYieldCutStr)},  
                        },
                        CreatedBy = userName
                    };
                    DBAccess.InsertTransaction(Transaction);
                }
                else
                {
                    Msg.Show("Problem occured while processing this request! Please close the application and re-open", "Problem Occured", MsgBoxButtons.OK, MsgBoxImage.Error);
                }               

                CloseForm();
            }
        }

        private decimal BlockLogCalculator(decimal qty, decimal yield)
        {
            decimal res = 0;
            if (yield != 0)
            {
                res = Math.Ceiling(qty / yield);
            }
            return res;
        }
        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }


        private void PrintYieldOrder()
        {
            decimal combinedVal = 0;
            totMaxYieldCut = Math.Floor(NoOfBlocksSlit * slittingProductionDetails.Product.Tile.MaxYield);
            combinedVal = Convert.ToDecimal(TotalYieldCutStr) + Convert.ToDecimal(OffSpecTilesStr);

            if (NoOfBlocksSlit == 0)
            {
                Msg.Show("Select the no of blocks slit", "Selection Required", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (Convert.ToDecimal(TotalYieldCutStr) == 0)
            {
                Msg.Show("Total yield required", "Input Required", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (Convert.ToDecimal(TotalYieldCutStr) > totMaxYieldCut)
            {
                Msg.Show("Total yield cut cannot be greater than " + totMaxYieldCut + " tiles", "Invalid Input", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (Convert.ToDecimal(OffSpecTilesStr) > totMaxYieldCut)
            {
                Msg.Show("Off - Spec tiles cannot be greater than " + totMaxYieldCut + " tiles", "Invalid Input", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (combinedVal > totMaxYieldCut)
            {
                Msg.Show("Total of both yield cut and Off-Spec cannot be greater than " + totMaxYieldCut + " tiles", "Invalid Input", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else
            {
                if (Msg.Show("Are you sure you want to print?", "Printing Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    bool isOffSpec = false;
                    PrintSlittingOrder printOrder = new PrintSlittingOrder(this, isOffSpec);
                }
            }
        }


        private void PrintOffSpecOrder()
        {
            decimal combinedVal = 0;
            combinedVal = Convert.ToDecimal(TotalYieldCutStr) + Convert.ToDecimal(OffSpecTilesStr);

            if (NoOfBlocksSlit == 0)
            {
                Msg.Show("Select the no of blocks slit", "Selection Required", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (Convert.ToDecimal(OffSpecTilesStr) == 0)
            {
                Msg.Show("Off-Spec tiles required", "Input Required", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (Convert.ToDecimal(TotalYieldCutStr) > totMaxYieldCut)
            {
                Msg.Show("Total yield cut cannot be greater than " + totMaxYieldCut + " tiles", "Invalid Input", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (Convert.ToDecimal(OffSpecTilesStr) > totMaxYieldCut)
            {
                Msg.Show("Off - Spec tiles cannot be greater than " + totMaxYieldCut + " tiles", "Invalid Input", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (combinedVal > totMaxYieldCut)
            {
                Msg.Show("Total of both yield cut and Off-Spec cannot be greater than " + totMaxYieldCut + " tiles", "Invalid Input", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else
            {
                if (Msg.Show("Are you sure you want to print Off-Spec slip?", "Printing Off-Spec Slip", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    bool isOffSpec = true;
                    PrintSlittingOrder printOrder = new PrintSlittingOrder(this, isOffSpec);
                }
            }
        }


        //private string GetTextBoxValue()
        //{
        //    string s = string.Empty;

        //    if (textBoxType == "TotYield")
        //    {
        //        s = TotalYieldCut.ToString();
        //    }
        //    else if (textBoxType == "OffSpec")
        //    {
        //        s = OffSpecTiles.ToString();
        //    }

        //    return s;
        //}

        private void AddToTextbox(string s)
        {

            if (textBoxType == "TotYield")
            {
                TotalYieldCutStr = s;
            }
            else if (textBoxType == "OffSpec")
            {
                OffSpecTilesStr = s;
            }
            //else if (textBoxType == "Shred")
            //{
            //    ShredTilesStr = s;
            //}
        }

        private void AddToTextbox2(string s)
        {

            if (textBoxType == "TotYield")
            {
                _totalYieldCutStr = s;
            }
            else if (textBoxType == "OffSpec")
            {
                _offSpecTilesStr = s;
            }
            //else if (textBoxType == "Shred")
            //{
            //    _shredTilesStr = s;
            //}            
        }

        private void TextBoxType(string type)
        {           
             
            textBoxType = type;
        }


        //deals with button inputs and sorts out the display accordingly
        public void DigitButtonPress(string button)
        {

            if (textBoxType == "TotYield")
            {
                _qty = TotalYieldCutStr;
            }
            else if (textBoxType == "OffSpec")
            {
                _qty = OffSpecTilesStr;
            }
            //else if (textBoxType == "Shred")
            //{
            //    _qty = ShredTilesStr;
            //}

           // _qty = textBoxType == "TotYield" ? TotalYieldCutStr : OffSpecTilesStr;
            switch (button)
            {
                case "C":
                    AddToTextbox("0");
                    FirstOperand = string.Empty;
                    SecondOperand = string.Empty;
                    Operation = string.Empty;
                    LastOperation = string.Empty;
                    FullExpression = string.Empty;
                    break;
                case "Del":
                    if (!string.IsNullOrWhiteSpace(_qty))
                    {
                        if (_qty.Length > 1)
                            AddToTextbox(_qty.Substring(0, _qty.Length - 1));
                        else AddToTextbox("0");
                    }
                    break;
                case "+/-":
                    if (_qty.Contains("-") || _qty == "0")
                    {
                        AddToTextbox(_qty.Remove(_qty.IndexOf("-"), 1));
                    }
                    else AddToTextbox("-" + _qty);
                    break;
                case ".":
                    if (newDisplayRequired)
                    {
                        AddToTextbox("0.");
                    }
                    else
                    {
                        if (!_qty.Contains("."))
                        {
                            AddToTextbox(_qty + ".");
                        }
                    }
                    break;
                default:
                    if (_qty == "0" || newDisplayRequired)
                        AddToTextbox(button);
                    else
                        AddToTextbox(_qty + button);
                    break;
            }

            //AddToTextbox(Qty);
            newDisplayRequired = false;
        }

        private void EnDisSubmit()
        {
            if (Convert.ToDecimal(TotalYieldCutStr) == 0 && Convert.ToDecimal(OffSpecTilesStr) == 0)
            {
                SubmitEnabled = false;
            }
            else
            {
                SubmitEnabled = true;
            }
        }

        #region PUBLIC PROPERTIES

        
        private static bool CanDigitButtonPress(string button)
        {
            return true;
        }

        public string Qty
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

        public bool TotYieldCutEnabled
        {
            get
            {
                return _totYieldCutEnabled;
            }
            set
            {
                _totYieldCutEnabled = value;
                RaisePropertyChanged(() => this.TotYieldCutEnabled);
            }
        }

        public bool OffSpecEnabled
        {
            get
            {
                return _offSpecEnabled;
            }
            set
            {
                _offSpecEnabled = value;
                RaisePropertyChanged(() => this.OffSpecEnabled);
            }
        }


        //public bool ShreddedEnabled
        //{
        //    get
        //    {
        //        return _shreddedEnabled;
        //    }
        //    set
        //    {
        //        _shreddedEnabled = value;
        //        RaisePropertyChanged(() => this.ShreddedEnabled);
        //    }
        //}


        public bool TotYieldCutPrint
        {
            get
            {
                return _totYieldCutPrint;
            }
            set
            {
                _totYieldCutPrint = value;
                RaisePropertyChanged(() => this.TotYieldCutPrint);
            }
        }

        public bool OffSpecPrint
        {
            get
            {
                return _offSpecPrint;
            }
            set
            {
                _offSpecPrint = value;
                RaisePropertyChanged(() => this.OffSpecPrint);
            }
        }




        //public string ShreddedLabelMargin
        //{
        //    get
        //    {
        //        return _shreddedLabelMargin;
        //    }
        //    set
        //    {
        //        _shreddedLabelMargin = value;
        //        RaisePropertyChanged(() => this.ShreddedLabelMargin);

        //    }
        //}

        public string ShreddedTextBoxMargin
        {
            get
            {
                return _shreddedTextBoxMargin;
            }
            set
            {
                _shreddedTextBoxMargin = value;
                RaisePropertyChanged(() => this.ShreddedTextBoxMargin);

            }
        }


        //public string ShreddedReasonsVisiblity
        //{
        //    get
        //    {
        //        return _shreddedReasonsVisiblity;
        //    }
        //    set
        //    {
        //        _shreddedReasonsVisiblity = value;
        //        RaisePropertyChanged(() => this.ShreddedReasonsVisiblity);

        //    }
        //}



        public string OffSpeccReasonsVisiblity
        {
            get
            {
                return _offSpeccReasonsVisiblity;
            }
            set
            {
                _offSpeccReasonsVisiblity = value;
                RaisePropertyChanged(() => this.OffSpeccReasonsVisiblity);

            }
        }

        public int NoOfBlocksSlit
        {
            get
            {
                return _noOfBlocksSlit;
            }
            set
            {
                _noOfBlocksSlit = value;
                RaisePropertyChanged(() => this.NoOfBlocksSlit);

                if (NoOfBlocksSlit > 0)
                {

                    OffSpecEnabled = true;
                    TotYieldCutEnabled = true;
                    //ShreddedEnabled = true;
                }
                else
                {
                    OffSpecEnabled = false;
                    TotYieldCutEnabled = false;
                    //ShreddedEnabled = false;
                    TotalYieldCut = 0;
                    OffSpecTiles = 0;
                    //ShreddedTiles = 0;
                }

            }
        }
        public List<int> MaxBlockCount
        {
            get
            {
                return _maxBlockCount;
            }
            set
            {
                _maxBlockCount = value;
                RaisePropertyChanged(() => this.MaxBlockCount);

            }
        }

        private string _totalYieldCutStr;
        public string TotalYieldCutStr
        {
            get
            {
                return _totalYieldCutStr;
            }
            set
            {
                _totalYieldCutStr = value;
                RaisePropertyChanged(() => this.TotalYieldCutStr);

                if (!String.IsNullOrWhiteSpace(TotalYieldCutStr))
                {
                    if (Convert.ToDecimal(TotalYieldCutStr) > 0)
                    {
                        TotYieldCutPrint = true;
                    }
                    else
                    {
                        TotYieldCutPrint = false;
                    }

                    textBoxType = "TotYield";

                    EnDisSubmit();
                }
            }
        }

        private string _offSpecTilesStr;
        public string OffSpecTilesStr
        {
            get
            {
                return _offSpecTilesStr;
            }
            set
            {
                _offSpecTilesStr = value;
                RaisePropertyChanged(() => this.OffSpecTilesStr);
                
                if (!String.IsNullOrWhiteSpace(OffSpecTilesStr))
                {
                    if (Convert.ToDecimal(OffSpecTilesStr) > 0)
                    {
                        OffSpecPrint = true;
                        OffSpeccReasonsVisiblity = "Visible";
                        //ShreddedLabelMargin = "121,264,0,0";
                        //ShreddedTextBoxMargin = "271,264,0,0";

                        //if (Convert.ToDecimal(ShredTilesStr) > 0)
                        //{
                        //    ShreddedGridMargin = "5,292,-5,0";
                        //}
                        //else
                        //{
                        //    ShreddedGridMargin = "5,193,-5,0";
                        //}
                    }
                    else
                    {
                        OffSpecPrint = false;
                        OffSpeccReasonsVisiblity = "Collapsed";
                        //ShreddedLabelMargin = "122,169,0,0";
                        //ShreddedTextBoxMargin = "271,172,0,0";
                        //ShreddedGridMargin = "5,193,-5,0";
                    }

                    EnDisSubmit();
                    textBoxType = "OffSpec";
                }                                
            }
        }


        //private string _shredTilesStr;
        //public string ShredTilesStr
        //{
        //    get
        //    {
        //        return _shredTilesStr;
        //    }
        //    set
        //    {
        //        _shredTilesStr = value;
        //        RaisePropertyChanged(() => this.ShredTilesStr);
                
        //        //if (Convert.ToDecimal(ShredTilesStr) > 0)
        //        //{
        //            //ShreddedReasonsVisiblity = "Visible";

        //            //if (Convert.ToDecimal(OffSpecTilesStr) > 0)
        //            //{
        //            //    ShreddedGridMargin = "5,292,-5,0";
        //            //}
        //            //else
        //            //{
        //            //    ShreddedGridMargin = "5,193,-5,0";
        //            //}
        //        //}
        //        //else
        //        //{
        //            //ShreddedReasonsVisiblity = "Collapsed";
        //        //}

        //        EnDisSubmit();
        //        textBoxType = "Shred";
        //    }
        //}

        public decimal TotalYieldCut
        {
            get
            {
                return _totalYieldCut;
            }
            set
            {
                _totalYieldCut = value;
                RaisePropertyChanged(() => this.TotalYieldCut);

                if (TotalYieldCut > 0)
                {
                    TotYieldCutPrint = true;
                }
                else
                {
                    TotYieldCutPrint = false;
                }

                textBoxType = "TotYield";

                EnDisSubmit();
            }
        }

        public decimal OffSpecTiles
        {
            get
            {
                return _offSpecTiles;
            }
            set
            {
                _offSpecTiles = value;
                RaisePropertyChanged(() => this.OffSpecTiles);
                if (OffSpecTiles > 0)
                {
                    OffSpecPrint = true;
                    OffSpeccReasonsVisiblity = "Visible";
                    //ShreddedLabelMargin = "121,264,0,0";
                    //ShreddedTextBoxMargin = "271,264,0,0";

                    //if (ShreddedTiles > 0)
                    //{
                    //    ShreddedGridMargin = "5,292,-5,0";
                    //}
                    //else
                    //{
                    //    ShreddedGridMargin = "5,193,-5,0";
                    //}
                }
                else
                {
                    OffSpecPrint = false;
                    OffSpeccReasonsVisiblity = "Collapsed";
                    //ShreddedLabelMargin = "122,169,0,0";
                    //ShreddedTextBoxMargin = "271,172,0,0";
                    //ShreddedGridMargin = "5,193,-5,0";
                }
                EnDisSubmit();

                textBoxType = "OffSpec";

            }
        }


        //public decimal ShreddedTiles
        //{
        //    get
        //    {
        //        return _shreddedTiles;
        //    }
        //    set
        //    {
        //        _shreddedTiles = value;
        //        RaisePropertyChanged(() => this.ShreddedTiles);

        //        //if (ShreddedTiles > 0)
        //        //{
        //        //    ShreddedReasonsVisiblity = "Visible";

        //            //if (OffSpecTiles > 0)
        //            //{
        //            //    ShreddedGridMargin = "5,292,-5,0";
        //            //}
        //            //else
        //            //{
        //            //    ShreddedGridMargin = "5,193,-5,0";
        //            //}
        //        //}
        //        //else
        //        //{
        //        //    ShreddedReasonsVisiblity = "Collapsed";
        //        //}
        //        EnDisSubmit();

        //        textBoxType = "Shred";
        //    }
        //}



        //public string ShreddedGridMargin
        //{
        //    get
        //    {
        //        return _shreddedGridMargin;
        //    }
        //    set
        //    {
        //        _shreddedGridMargin = value;
        //        RaisePropertyChanged(() => this.ShreddedGridMargin);
        //    }
        //}

        //public decimal ExtraTiles 
        //{
        //    get
        //    {
        //        return _extraTiles;
        //    }
        //    set
        //    {
        //        _extraTiles = value ;
        //        RaisePropertyChanged(() => this.ExtraTiles);            
        //    }
        //}



        public string TileForeGColor
        {
            get
            {
                return _tileForeGColor;
            }
            set
            {
                _tileForeGColor = value;
                RaisePropertyChanged(() => this.TileForeGColor);
            }
        }

        public bool IsContaminated
        {
            get
            {
                return _isContaminated;
            }
            set
            {
                _isContaminated = value;
                RaisePropertyChanged(() => this.IsContaminated);
            }
        }

        public bool IsLiftedOffBoard
        {
            get
            {
                return _isLiftedOffBoard;
            }
            set
            {
                _isLiftedOffBoard = value;
                RaisePropertyChanged(() => this.IsLiftedOffBoard);
            }
        }

        public bool IsUnevenThickness
        {
            get
            {
                return _isUnevenThickness;
            }
            set
            {
                _isUnevenThickness = value;
                RaisePropertyChanged(() => this.IsUnevenThickness);
            }
        }

        public bool IsTooThick
        {
            get
            {
                return _isTooThick;
            }
            set
            {
                _isTooThick = value;
                RaisePropertyChanged(() => this.IsTooThick);
            }
        }

        public bool IsTooThin
        {
            get
            {
                return _isTooThin;
            }
            set
            {
                _isTooThin = value;
                RaisePropertyChanged(() => this.IsTooThin);
            }
        }

        public bool IsStonelines
        {
            get
            {
                return _isStonelines;
            }
            set
            {
                _isStonelines = value;
                RaisePropertyChanged(() => this.IsStonelines);

            }
        }
        public bool IsDamaged
        {
            get
            {
                return _isDamaged;
            }
            set
            {
                _isDamaged = value;
                RaisePropertyChanged(() => this.IsDamaged);
            }
        }

        public bool IsOther
        {
            get
            {
                return _isOther;
            }
            set
            {
                _isOther = value;
                RaisePropertyChanged(() => this.IsOther);

                if(IsOther)
                {
                    TxtOtherVisibility = "Visible";
                }
                else
                {
                    TxtOtherVisibility = "Collapsed";
                }
            }
        }

        public bool IsOperatorError
        {
            get
            {
                return _isOperatorError;
            }
            set
            {
                _isOperatorError = value;
                RaisePropertyChanged(() => this.IsOperatorError);

            }
        }

        public string TxtOtherVisibility
        {
            get
            {
                return _txtOtherVisibility;
            }
            set
            {
                _txtOtherVisibility = value;
                RaisePropertyChanged(() => this.TxtOtherVisibility);
            }
        }

        public string OtherText
        {
            get
            {
                return _otherText;
            }
            set
            {
                _otherText = value;
                RaisePropertyChanged(() => this.OtherText);
            }
        }    
    
        

        //public bool IsContaminatedShredded
        //{
        //    get
        //    {
        //        return _isContaminatedShredded;
        //    }
        //    set
        //    {
        //        _isContaminatedShredded = value;
        //        RaisePropertyChanged(() => this.IsContaminatedShredded);
        //    }
        //}

        //public bool IsLiftedOffBoardShredded
        //{
        //    get
        //    {
        //        return _isLiftedOffBoardShredded;
        //    }
        //    set
        //    {
        //        _isLiftedOffBoardShredded = value;
        //        RaisePropertyChanged(() => this.IsLiftedOffBoardShredded);
        //    }
        //}

        //public bool IsUnevenThicknessShredded
        //{
        //    get
        //    {
        //        return _isUnevenThicknessShredded;
        //    }
        //    set
        //    {
        //        _isUnevenThicknessShredded = value;
        //        RaisePropertyChanged(() => this.IsUnevenThicknessShredded);
        //    }
        //}

        //public bool IsTooThickShredded
        //{
        //    get
        //    {
        //        return _isTooThickShredded;
        //    }
        //    set
        //    {
        //        _isTooThickShredded = value;
        //        RaisePropertyChanged(() => this.IsTooThickShredded);
        //    }
        //}

        //public bool IsTooThinShredded
        //{
        //    get
        //    {
        //        return _isTooThinShredded;
        //    }
        //    set
        //    {
        //        _isTooThinShredded = value;
        //        RaisePropertyChanged(() => this.IsTooThinShredded);
        //    }
        //}

        //public bool IsStonelinesShredded
        //{
        //    get
        //    {
        //        return _isStonelinesShredded;
        //    }
        //    set
        //    {
        //        _isStonelinesShredded = value;
        //        RaisePropertyChanged(() => this.IsStonelinesShredded);

        //    }
        //}
        //public bool IsDamagedShredded
        //{
        //    get
        //    {
        //        return _isDamagedShredded;
        //    }
        //    set
        //    {
        //        _isDamagedShredded = value;
        //        RaisePropertyChanged(() => this.IsDamagedShredded);
        //    }
        //}

        //public bool IsOtherShredded
        //{
        //    get
        //    {
        //        return _isOtherShredded;
        //    }
        //    set
        //    {
        //        _isOtherShredded = value;
        //        RaisePropertyChanged(() => this.IsOtherShredded);
        //    }
        //}

        //public bool IsOperatorErrorShredded
        //{
        //    get
        //    {
        //        return _isOperatorErrorShredded;
        //    }
        //    set
        //    {
        //        _isOperatorErrorShredded = value;
        //        RaisePropertyChanged(() => this.IsOperatorErrorShredded);

        //    }
        //}


        


        #endregion

        #region COMMANDS

        public ICommand SubmitCommand
        {
            get
            {
                return _submitCommand ?? (_submitCommand = new A1QSystem.Commands.LogOutCommandHandler(() => SubmitOrder(), canExecute));
            }
        }

        public ICommand PrintYieldCommand
        {
            get
            {
                return _printYieldCommand ?? (_printYieldCommand = new A1QSystem.Commands.LogOutCommandHandler(() => PrintYieldOrder(), canExecute));
            }
        }

        public ICommand PrintOffSpecCommand
        {
            get
            {
                return _printOffSpecCommand ?? (_printOffSpecCommand = new A1QSystem.Commands.LogOutCommandHandler(() => PrintOffSpecOrder(), canExecute));
            }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
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

        private ICommand _totalYieldClicked;
        public ICommand TotalYieldClicked
        {
            get
            {
                return _totalYieldClicked ?? (_totalYieldClicked = new A1QSystem.Commands.LogOutCommandHandler(() => TextBoxType("TotYield"), canExecute));
            }
        }

        private ICommand _offspecClicked;
        public ICommand OffspecClicked
        {
            get
            {
                return _offspecClicked ?? (_offspecClicked = new A1QSystem.Commands.LogOutCommandHandler(() => TextBoxType("OffSpec"), canExecute));
            }
        }

        private ICommand _shredTilesClicked;
        public ICommand ShredTilesClicked
        {
            get
            {
                return _shredTilesClicked ?? (_shredTilesClicked = new A1QSystem.Commands.LogOutCommandHandler(() => TextBoxType("Shred"), canExecute));
            }
        }

        

        #endregion
    }
}
