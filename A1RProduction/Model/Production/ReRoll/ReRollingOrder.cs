﻿using A1QSystem.Commands;
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

namespace A1QSystem.Model.Production.ReRoll
{
    public class ReRollingOrder : ViewModelBase
    {
        public Int32 ID { get; set; }
        public Int32 ProdTimetableID { get; set; }
        public Product Product { get; set; }
        public decimal Rolls { get; set; }
        public decimal DollarValue { get; set; }
        public decimal YieldToReRoll { get; set; }
        public decimal Qty { get; set; }
        public Shift Shift { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public DateTime ReRollingDate { get; set; }
        public string RowBackgroundColour { get; set; }
        public string ThicknessString { get; set; }
        public string SizeString { get; set; }
        public string NotesVisibility { get; set; }

        private Order _order;
        private string _salesOrderNoVisibility;

        private bool canExecute;
        private ICommand _completeCommand;
        private ICommand _printLabelCommand;


        public ReRollingOrder()
        {
            canExecute = true;
            RowBackgroundColour = "#ffffff";

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


                if (String.IsNullOrEmpty(Order.SalesNo))
                {
                    SalesOrderNoVisibility = "Collapsed";
                }
                else
                {
                    SalesOrderNoVisibility = "Visible";
                }
           
            }
        }

        public string SalesOrderNoVisibility
        {
            get { return _salesOrderNoVisibility; }
            set
            {
                _salesOrderNoVisibility = value;
                RaisePropertyChanged(() => this.SalesOrderNoVisibility);


            }
        }

        

        private void CompleteOrder()
        {
            var childWindow = new ChildWindowView();
            childWindow.ShowReRollingConfirmationView(this);
        }

        private void PrintLabels()
        {
            var childWindow = new ChildWindowView();
            childWindow.PrintingLabelsPopUp(this);
        }


        #region COMMANDS



        public ICommand CompleteCommand
        {
            get
            {
                return _completeCommand ?? (_completeCommand = new LogOutCommandHandler(() => CompleteOrder(), canExecute));
            }
        }
        public ICommand PrintLabelCommand
        {
            get
            {
                return _printLabelCommand ?? (_printLabelCommand = new LogOutCommandHandler(() => PrintLabels(), canExecute));
            }
        }

        #endregion
    }
}
