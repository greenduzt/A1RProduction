using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Shifts;
using A1QSystem.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Model.Production.Peeling
{
    public class PeelingOrder : ViewModelBase
    {
        public Int32 ID { get; set; }
        public Int32 ProdTimetableID { get; set; }
        public Product Product { get; set; }
        public Shift Shift { get; set; }
        public string NotesVisibility { get; set; }
        public decimal Qty { get; set; }
        public decimal Logs { get; set; }
        public decimal DollarValue { get; set; }       
        public string Status { get; set; }
        public DateTime PeelingDate { get; set; }
        public string ThicknessString { get; set; }
        public string SizeString { get; set; }
        //public bool IsReRollingReq { get; set; }
        public string ReRollingReqString { get; set; }

        private bool _isReRollingReq;
        private string _reRollingReqVisible;
        private string _rowBackgroundColour;
        private Order _order;
        private bool canExecute;
        private ICommand _completeCommand;

        public PeelingOrder()
        {
            canExecute = true;
            RowBackgroundColour = "#ffffff";
            NotesVisibility = "Collapsed";
            ReRollingReqVisible = "Collapsed";
        }

        public Order Order
        {
            get { return _order; }
            set
            {
                _order = value;
                RaisePropertyChanged(() => this.Order);


                if (Order.OrderType == 1)
                {
                    RowBackgroundColour = "#cc3700";//Urgent                   
                }
                if (Order.OrderType == 2)
                {
                    RowBackgroundColour = "#cc3700";//Urgent Graded                    
                }
                if (Order.OrderType == 3)
                {
                    RowBackgroundColour = "#ffffff";//Normal                   
                }
                if (Order.OrderType == 4)
                {
                    RowBackgroundColour = "#ffffff";//Normal    
                }

                if (!String.IsNullOrEmpty(Order.Comments))
                {
                    NotesVisibility = "Visible";
                }
                else
                {
                    NotesVisibility = "Collapsed";
                }
            }
        }


        public bool IsReRollingReq
        {
            get { return _isReRollingReq; }
            set
            {
                _isReRollingReq = value;
                RaisePropertyChanged(() => this.IsReRollingReq);
                if (IsReRollingReq != null)
                {
                    if (IsReRollingReq)
                    {
                        ReRollingReqVisible = "Visible";
                    }
                    else
                    {
                        ReRollingReqVisible = "Collapsed";
                    }
                }
            }
        }

        private void CompleteOrder()
        {
            var childWindow = new ChildWindowView();
            childWindow.ShowPeelingConfirmationView(this);
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

        public string ReRollingReqVisible
        {
            get { return _reRollingReqVisible; }
            set
            {
                _reRollingReqVisible = value;
                RaisePropertyChanged(() => this.ReRollingReqVisible);
            }
        }

        

        public ICommand CompleteCommand
        {
            get
            {
                return _completeCommand ?? (_completeCommand = new LogOutCommandHandler(() => CompleteOrder(), canExecute));
            }
        }
    }
}
