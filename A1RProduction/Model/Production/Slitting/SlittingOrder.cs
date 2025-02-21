using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Other;
using A1QSystem.Model.Shifts;
using A1QSystem.Model.Stock;
using A1QSystem.View;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Model.Production.Slitting
{
    public class SlittingOrder : ViewModelBase
    {
        public Int32 ID { get; set; }
        public Int32 ProdTimetableID { get; set; }
        //public Order Order { get; set; }
        public Product Product { get; set; }
        public Shift Shift { get; set; }
        public decimal Qty { get; set; }
        public decimal Blocks { get; set; }
        public decimal DollarValue { get; set; }
        public string Status { get; set; }
        public DateTime SlittingDate { get; set; }
        public string NotesVisibility { get; set; }
        public A1QSystem.Model.Transaction.Transaction Transaction { get; set; }
        private string _rowBackgroundColour;
        private ICommand _completeCommand;
        private bool canExecute;     
        private Order _order;
        private string _salesOrderNoVisibility;
        private string _isNotesVisible;
        private string _itemBackgroundColour;
        private bool _isExpanded;
        private string _shiftText;
        private string _shiftBtnBackColour;
        private string _itemPresenterVivibility;
        private Machines _machine;
        private string _bottomRowVisibility;
        //private string _notes;

        public SlittingOrder()
        {
            canExecute = true;
            RowBackgroundColour = "#ffffff";
            NotesVisibility = "Collapsed";
            ShiftText = "Disable";

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
             

                if(!String.IsNullOrEmpty(Order.Comments))
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

        public string IsNotesVisible
        {
            get { return _isNotesVisible; }
            set
            {
                _isNotesVisible = value;
                RaisePropertyChanged(() => this.IsNotesVisible);
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

        public string BottomRowVisibility
        {
            get { return _bottomRowVisibility; }
            set
            {
                _bottomRowVisibility = value;
                RaisePropertyChanged(() => this.BottomRowVisibility);
            }
        }

        

        
        //public string Notes
        //{
        //    get { return _notes; }
        //    set
        //    {
        //        _notes = value;
        //        RaisePropertyChanged(() => this.Notes);

        //        if (!String.IsNullOrEmpty(Notes))
        //        {
        //            NotesVisibility = "Visible";
        //        }
        //        else
        //        {
        //            NotesVisibility = "Collapsed";
        //        }

        //    }
        //}

        public string RowBackgroundColour
        {
            get { return _rowBackgroundColour; }
            set
            {
                _rowBackgroundColour = value;
                RaisePropertyChanged(() => this.RowBackgroundColour);
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

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                RaisePropertyChanged(() => this.IsExpanded);
            }
        }

        public string   ShiftText
        {
            get { return _shiftText; }
            set
            {
                _shiftText = value;
                RaisePropertyChanged(() => this.ShiftText);
            }
        }

        public string ItemPresenterVivibility
        {
            get { return _itemPresenterVivibility; }
            set
            {
                _itemPresenterVivibility = value;
                RaisePropertyChanged(() => this.ItemPresenterVivibility);
            }
        }

        public Machines Machine
        {
            get { return _machine; }
            set
            {
                _machine = value;
                RaisePropertyChanged(() => this.Machine);
            }
        }
        
        

        private void CompleteSlitting()
        {
            var childWindow = new ChildWindowView();
            childWindow.ShowSlittingConfirmationView(this);
        }  

        public ICommand CompleteCommand
        {
            get
            {
                return _completeCommand ?? (_completeCommand = new LogOutCommandHandler(() => CompleteSlitting(), canExecute));
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
                if (Msg.Show("Are you sure, you want to delete this order?", "Order Delete Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Information_Red, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    int x = DBAccess.DeleteSlittingOrder(this);
                    if (x > 0)
                    {
                        string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                        if (String.IsNullOrEmpty(userName))
                        {
                            userName = "Unknown";
                        }
                        Transaction = new A1QSystem.Model.Transaction.Transaction()
                        {
                            TransDateTime = DateTime.Now,
                            Transtype = "Deleted Slitting",
                            SalesOrderID = Order.OrderNo,
                            Products = new List<RawStock>()
                        {
                            new RawStock(){RawProductID = Product.ProductID,Qty=Blocks},  
                        },
                            CreatedBy = userName
                        };
                        int r = DBAccess.InsertTransaction(Transaction);
                        Msg.Show("The selected order has been successfully deleted!", "Order Deleted", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);
                    }
                }
            }
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
                //int sp1 = DBAccess.UpdateSystemParameter("MoveOrder-Slitting", true);
                //if (sp1 > 0)
                //{
                    var childWindow = new ChildWindowView();
                    childWindow.ShowSlittingShiftWindow(this);
                //    int sp2 = DBAccess.UpdateSystemParameter("MoveOrder", false);
                //}
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
                    SlittingManager sm = new SlittingManager();

                    if (ShiftText == "Enable")
                    {
                        if (Msg.Show("Are you sure, you want to enable this shift?", "Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                        {
                            int r = DBAccess.EnableDisableSingleShift(Convert.ToDateTime(SlittingDate), Machine.MachineID, true, Shift.ShiftID);
                            if (r > 0)
                            {
                                if (Msg.Show("Do you want to allocate existing orders to this shift?", "Shift Enabling Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                                {
                                    List<SlittingOrder> dd = DBAccess.GetSlittingOrdersByMachine(Machine.MachineID);
                                    sm.BackupDeleteAdd(dd);
                                    ProductionManager.AddToSlitting(dd);      
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
                            int r = DBAccess.EnableDisableSingleShift(Convert.ToDateTime(SlittingDate), Machine.MachineID, false, Shift.ShiftID);
                            if (r > 0)
                            {
                                List<SlittingOrder> slittingOrderList = new List<SlittingOrder>();                                
                                ObservableCollection<SlittingOrder> tempList = DBAccess.GetSlittingOrdersByDateByShift(ProdTimetableID, Shift.ShiftID);
                                slittingOrderList = sm.SeparateReleventSlittingOrders(tempList, Shift.ShiftID, Machine.MachineID);
                                sm.BackupDeleteAdd(slittingOrderList);                                
                                ShiftText = "Enable";
                                ShiftBtnBackColour = "#FFCC3700";
                            }
                        }
                    }

                    int sp2 = DBAccess.UpdateSystemParameter("DisableEnableDates", false);
                }
            }
        }

        

        #region COMMANDS

        private ICommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new LogOutCommandHandler(() => DeleteOrder(), true));
            }
        }

        private ICommand _moveCommand;
        public ICommand MoveCommand
        {
            get
            {
                return _moveCommand ?? (_moveCommand = new LogOutCommandHandler(() => MoveOrder(), canExecute));
            }
        }


        private ICommand _enDisShiftCommand;
        public ICommand EnDisShiftCommand
        {
            get
            {
                return _enDisShiftCommand ?? (_enDisShiftCommand = new LogOutCommandHandler(() => EnableDisableShift(), canExecute));
            }
        }

        #endregion
    }
}
