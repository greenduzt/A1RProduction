using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Products;
using A1QSystem.Model.Stock;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Model.Production.SlitingPeeling
{
    public class SlittingProductionDetails : ViewModelBase
    {
        public Product Product { get; set; }
        public SlitPeel SlitPeel { get; set; }
        public String SlittingFormula { get; set; }
        public RawStock RawStock { get; set; }
        public SlitPeelProduction SlitPeelProduction { get; set; }
        public Customer Customer { get; set; }
        public ObservableCollection<SlitPeelDetails> InnerGrid { get; set; }
        public RawProduct RawProduct { get; set; }
        public DateTime SlittingStartTime { get; set; }
        public string ShiftName { get; set; }
        public string TileNames { get; set; }
        public decimal TotTiles { get; set; }
        public decimal BlocksDone { get; set; }
        public string FreightVisibility { get; set; }
        private ICommand _completedCommand;
        private ICommand _printCommand;
        private bool canExecute;
        public DateTime FreightArrDate { get; set; }
        public string FreightArrTime { get; set; }
        public string FreightDescription { get; set; }
        private string _freightName;
        private bool _freightTimeAvailable;
        private bool _freightDateAvailable;
        public string FreightDateTimeVisibility { get; set; }
        public int salesOrderID { get; set; }
        public string SalesOrder { get; set; }
        public string ArrivalDateTime { get; set; }
        private int _ordertype;
        private string _rowBackgroundColour;

        public SlittingProductionDetails()
        {
            canExecute = true;
            RowBackgroundColour = "#ffffff";
        }

        private void CompleteOrder()
        {



            //var childWindow = new ChildWindowView();
            //childWindow.ShowEditRawStockWindow(this);    

            //if (Msg.Show("Have you completed all the " + SlitPeelProduction.BlockLogQty + " " + Product.ProductUnit + " of " + Product.ProductDescription + "?" + System.Environment.NewLine + "Click yes if you have only completed all the " + SlitPeelProduction.BlockLogQty + " " + Product.ProductUnit + " of " + Product.ProductDescription, "Slitting Completion Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
            //{

            //   int r = DBAccess.UpdateSlittingQty(this);
            //   if (r == 0)
            //   {
            //       Msg.Show("Problem occured while processing this request! Please close the application and re-open", "Problem Occured", MsgBoxButtons.OK, MsgBoxImage.Error);
                   
            //   }
            //}            
        }

        private decimal BlockLogCalculator(decimal qty, decimal yield)
        {
            decimal res = 0;

            res = Math.Ceiling(qty / yield);

            return res;
        }

        private void PrintOrder()
        {
            if (Msg.Show("Do you want to PRINT SLITTING ORDER for " + Product.ProductDescription + "?", "Printing Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
            {
                //PrintSlittingOrder printOrder = new PrintSlittingOrder(this);
            }
        }

        public string FreightName
        {
            get { return _freightName; }
            set
            {
                _freightName = value;
                if (FreightName == "No Freight")
                {

                    FreightVisibility = "Hidden";
                    FreightDateTimeVisibility = "Hidden";
                }
                else
                {
                    FreightVisibility = "Visible";
                    FreightDateTimeVisibility = "Visible";
                }
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
                    FreightDateTimeVisibility = "Hidden";
                }
                else
                {
                    FreightDateTimeVisibility = "Visible";
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

        public string RowBackgroundColour
        {
            get { return _rowBackgroundColour; }
            set
            {
                _rowBackgroundColour = value;
                RaisePropertyChanged(() => this.RowBackgroundColour);
            }
        }

        public int OrderType
        {
            get { return _ordertype; }
            set
            {
                _ordertype = value;
                RaisePropertyChanged(() => this.OrderType);

                if (OrderType == 1 || OrderType == 2)
                {
                    RowBackgroundColour = "#e62d00";//Urgent
                }

                if (OrderType == 3 || OrderType == 4)
                {
                    RowBackgroundColour = "#ffffff";//Normal
                }
              
            }
        }

        #region COMMANDS

        public ICommand CompletedCommand
        {
            get
            {
                return _completedCommand ?? (_completedCommand = new LogOutCommandHandler(() => CompleteOrder(), canExecute));
            }
        }
        public ICommand PrintCommand
        {
            get
            {
                return _printCommand ?? (_printCommand = new LogOutCommandHandler(() => PrintOrder(), canExecute));
            }
        }

        #endregion

    }
}
