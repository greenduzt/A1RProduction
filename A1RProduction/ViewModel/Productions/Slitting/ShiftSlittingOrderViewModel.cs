
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Slitting;
using A1QSystem.Model.Shifts;
using A1QSystem.Model.Stock;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.ViewModel.Productions.Slitting
{
    public class ShiftSlittingOrderViewModel : ViewModelBase
    {
        private SlittingOrder _slittingOrder;
        private decimal _qty;
        private DateTime _currentDate;
        private DateTime _selectedDate;
        private List<Shift> _shiftList;
        public int CurentShift { get; set; }
        public int CurrentProdTimeTableID { get; set; }
        private int _selectedShift;
        public event Action Closed;
        private DelegateCommand _closeCommand;
        private DelegateCommand _shiftOrderCommand;

        public ShiftSlittingOrderViewModel(SlittingOrder so)
        {
            SlittingOrder = so;
            Qty = SlittingOrder.Blocks;
            CurrentDate = Convert.ToDateTime(NTPServer.GetNetworkTime().ToString("dd/MM/yyyy"));
            SelectedDate = Convert.ToDateTime(SlittingOrder.SlittingDate);
            _closeCommand = new DelegateCommand(CloseForm);
            _shiftOrderCommand = new DelegateCommand(ShiftOrder);
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        private void ShiftOrder()
        {
            if (SelectedShift == 0)
            {
                Msg.Show("Please select a shift to move ", "Select Shift", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (SelectedShift == SlittingOrder.Shift.ShiftID && SelectedDate == Convert.ToDateTime(SlittingOrder.SlittingDate))
            {
                Msg.Show("Cannot shift order to " + GetShiftNameByID(SelectedShift.ToString()) + " shift. Please select a different shift", "Select A Different Shift", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                Qty = SlittingOrder.Blocks;
            }
            else
            {
                SlittingManager sm = new SlittingManager();
                Tuple<decimal, decimal, ProductionTimeTable,decimal,decimal> elements2 = null;
                elements2 = sm.CalculateDestinationRemainingDollarValue(SlittingOrder, SelectedDate, SelectedShift, Qty);
                string updateType = string.Empty;
                Tuple<decimal, decimal, decimal> elements = new Tuple<decimal,decimal,decimal>(0,0,0);  
                 
                
                if(SlittingOrder.Blocks == Qty)
                {
                    updateType = "Del";                    
                }
                else
                {
                    updateType = "Upd";
                    elements = sm.CalculateOriginQuantities(SlittingOrder, Qty);
                }

                if (elements2.Item1 <= elements2.Item2)
                {
                    //can move
                    int res=DBAccess.MoveSlittingOrder(elements2.Item3.ID, SlittingOrder.Order.OrderNo, SlittingOrder.Product, SelectedShift, elements2.Item4, elements2.Item5, elements2.Item1, SlittingOrder, updateType, elements.Item1, elements.Item2, elements.Item3);
                    if (res > 0)
                    {
                        //Add a transaction record
                        string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                        if (String.IsNullOrEmpty(userName))
                        {
                            userName = "Unknown";
                        }
                        A1QSystem.Model.Transaction.Transaction Transaction = new A1QSystem.Model.Transaction.Transaction()
                        {
                            TransDateTime = DateTime.Now,
                            Transtype = "Moving(Slitting)",
                            SalesOrderID = SlittingOrder.Order.OrderNo,
                            Products = new List<RawStock>()
                        {
                            new RawStock(){RawProductID = SlittingOrder.Product.ProductID,Qty=Convert.ToDecimal(SlittingOrder.Blocks)},  
                        },
                            CreatedBy = userName
                        };
                        DBAccess.InsertTransaction(Transaction);
                    }
                }
                else
                {
                    //can't move, display how much can move
                    //decimal amountMove = elements2.Item1 - elements2.Item2;
                    Msg.Show("The capacity is not enough to move " + Qty + " blocks to " + SelectedDate.ToString("dd/MM/yyyy") + ", " + GetShiftNameByID(SelectedShift.ToString()) + " shift", "Inadequate Capacity", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);

                    //if (Msg.Show("Cannot move " + Qty + " blocks to " + SelectedDate.ToString("dd/MM/yyyy") + " " + GetShiftNameByID(SelectedShift.ToString()) +
                    //    System.Environment.NewLine + "Only " + amountMove + " can move", "Select Shift", MsgBoxButtons.YesNo, MsgBoxImage.Error, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                    //{

                    //}
                }

                CloseForm();
            }   
        }

        private void LoadShiftData()
        {
            ShiftIProcessor si = new ShiftIProcessor(SelectedDate, SlittingOrder.Machine.MachineID);
            CurentShift = si.GetCurrentShift();
            CurrentProdTimeTableID = si.GetCurrentProdTimeTableID();
            ShiftList = si.GetShiftDetails();
            SelectedShift = 0;
        }

        private string GetShiftNameByID(string shiftId)
        {

            string result = string.Empty;

            switch (shiftId)
            {
                case "1": result = "Day";
                    break;
                case "2": result = "Afternoon";
                    break;
                case "3": result = "Night";
                    break;
                default: result = "Unspecified";
                    break;
            }

            return result;
        }

        public DateTime CurrentDate
        {
            get
            {
                return _currentDate;
            }
            set
            {
                _currentDate = value;
                RaisePropertyChanged(() => this.CurrentDate);
            }
        }

        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                RaisePropertyChanged(() => this.SelectedDate);

                LoadShiftData();
            }
        }

        public SlittingOrder SlittingOrder
        {
            get
            {
                return _slittingOrder;
            }
            set
            {
                _slittingOrder = value;
                RaisePropertyChanged(() => this.SlittingOrder);
            }
        }

        public decimal Qty
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

        public List<Shift> ShiftList
        {
            get
            {
                return _shiftList;
            }
            set
            {
                _shiftList = value;
                RaisePropertyChanged(() => this.ShiftList);
            }
        }

        public int SelectedShift
        {
            get
            {
                return _selectedShift;
            }
            set
            {
                _selectedShift = value;
                RaisePropertyChanged(() => this.SelectedShift);
            }
        }

        #region COMMANDS

        public DelegateCommand ShiftOrderCommand
        {
            get { return _shiftOrderCommand; }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        #endregion
    }
}
