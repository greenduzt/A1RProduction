using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Formula;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Mixing;
using A1QSystem.Model.Products;
using A1QSystem.Model.RawMaterials;
using A1QSystem.Model.Shifts;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Productions
{
    public class MixingUncompletedListViewModel : ViewModelBase
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        private int _currentShift;
        private int _currentMixingTimeTable;
        private DateTime _currentDate;
        private DateTime _selectedDate;
        private string _headerResult;
        private ListCollectionView _collView = null;
        public List<ProductionHistory> ProductionHistory { get; set; }
        private Microsoft.Practices.Prism.Commands.DelegateCommand _closeCommand;
        public event Action Closed;
        private ICommand _searchMixingUnCompletedCommand;
        private ICommand _printMixingUnCompletedCommand;
        private ICommand _completeWorkOrderCommand;

        public MixingUncompletedListViewModel()
        {
            CurrentDate = DateTime.Now;
            SelectedDate = CurrentDate;
            _closeCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(CloseForm);
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        private void SearchMixingUncompleted()
        {
            ProductionHistory = DBAccess.GetMixingUnCompletedList(SelectedDate,false);
            if (ProductionHistory.Count != 0)
            {
                HeaderResult = "Mixing uncompleted list for the date " + SelectedDate.ToString("dd/MM/yyyy");
                CollView = new ListCollectionView(ProductionHistory);
            }
            else
            {
                HeaderResult = "Mixing uncompleted list not found for the date " + SelectedDate.ToString("dd/MM/yyyy");
                ProductionHistory.Clear();
                CollView = new ListCollectionView(ProductionHistory);
                //Msg.Show("No information found for the date " + SelectedDate.ToString("dd/MM/yyyy"), "Grading History Not Found", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
        }

        private void PrintMixingUncompleted()
        {
            SearchMixingUncompleted();
            if(ProductionHistory != null && ProductionHistory.Count > 0)
            {
                Exception exception = null;
                BackgroundWorker worker = new BackgroundWorker();
                ChildWindowView LoadingScreen = new ChildWindowView();
                LoadingScreen.ShowWaitingScreen("Processing");

                worker.DoWork += (_, __) =>
                {
                    PrintMixingUnCompletedListPDF pm = new PrintMixingUnCompletedListPDF(ProductionHistory, SelectedDate, HeaderResult);
                    exception=pm.CreateProductionPDF();
                };
                worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                {
                    LoadingScreen.CloseWaitingScreen();
                    if(exception != null)
                    {
                        Msg.Show("Could not print the document " + System.Environment.NewLine + exception.ToString(), "Could Not Print Mixing Uncompleted List", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                    }
                };
                worker.RunWorkerAsync();
            }
            else
            {
                Msg.Show("No information found for the date " + SelectedDate.ToString("dd/MM/yyyy"), "Mixing Uncompleted Details Not Found", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
        }

        private void CompleteOrder(Object parameter)
        {
            int index = ProductionHistory.IndexOf(parameter as ProductionHistory);
            if (index >= 0)
            {
                if (Msg.Show("Are you sure you want to complete this order?", "Order Completing Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {                    
                    //BackgroundWorker worker = new BackgroundWorker();
                    //ChildWindowView LoadingScreen = new ChildWindowView();
                    //LoadingScreen.ShowWaitingScreen("Processing");

                    //worker.DoWork += (_, __) =>
                    //{
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

                        List<RawProductMachine> rpm = DBAccess.GetMachineIdByRawProdId(ProductionHistory[index].RawProduct.RawProductID);
                        if (rpm.Count != 0)
                        {
                            _currentMixingTimeTable = GetProdTimeTableID(rpm[0].MixingMachineID);
                        }


                        /*****Process curing******/

                        Curing pendingCuring = new Curing();
                        string curDes = "Up";
                        GenerateCuringTime();
                        Tuple<Int32, int, bool> tuple = DBAccess.CheckToAddCuring(ProductionHistory[index].RawProduct.RawProductID);

                        if (tuple.Item1 == 0 && tuple.Item2 == 0 && tuple.Item3 == false || tuple.Item3 == true)
                        {
                            //Get BlockLog Curing
                            pendingCuring = DBAccess.GetTopCuringProduct(ProductionHistory[index].SalesOrder.ID, ProductionHistory[index].RawProduct.RawProductID);
                            pendingCuring.StartTime = StartTime;
                            pendingCuring.EndTime = EndTime;
                            pendingCuring.IsEnabled = true;
                            curDes = "Up";
                        }
                        else
                        {
                            pendingCuring.OrderNo = tuple.Item1;
                            pendingCuring.Product = new Product() { ProductID = tuple.Item2, RawProduct = new RawProduct() { RawProductID = ProductionHistory[index].RawProduct.RawProductID } };
                            pendingCuring.Qty = 1;
                            pendingCuring.StartTime = StartTime;
                            pendingCuring.EndTime = EndTime;
                            pendingCuring.IsCured = false;
                            pendingCuring.IsEnabled = true;
                            curDes = "Ins";
                        }

                        List<GradingCompleted> ggList = CalculateGradingRubber(ProductionHistory[index].RawProduct.RawProductID, _currentShift);
                        MixingProductionDetails mixingProductionDetails = new MixingProductionDetails();

                        decimal b = DBAccess.UpdateUnfinishedOrders(ProductionHistory[index], ggList, pendingCuring, curDes);
                        SearchMixingUncompleted();
                    //};

                    //worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                    //{
                    //    LoadingScreen.CloseWaitingScreen();
                    //};
                    //worker.RunWorkerAsync();
                }
            }
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

        private void GenerateCuringTime()
        {
            StartTime = DateTime.Now;
            EndTime = StartTime.AddHours(+7);            
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

        public string HeaderResult
        {
            get { return _headerResult; }
            set
            {
                _headerResult = value;
                RaisePropertyChanged(() => this.HeaderResult);
            }
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
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                RaisePropertyChanged(() => this.SelectedDate);
            }
        }

        public ListCollectionView CollView
        {
            get { return _collView; }
            set
            {
                _collView = value;
                RaisePropertyChanged(() => this.CollView);

            }
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        public Microsoft.Practices.Prism.Commands.DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public ICommand SearchMixingUnCompletedCommand
        {
            get
            {
                return _searchMixingUnCompletedCommand ?? (_searchMixingUnCompletedCommand = new LogOutCommandHandler(() => SearchMixingUncompleted(), true));
            }
        }

        public ICommand PrintMixingUnCompletedCommand
        {
            get
            {
                return _printMixingUnCompletedCommand ?? (_printMixingUnCompletedCommand = new LogOutCommandHandler(() => PrintMixingUncompleted(), true));
            }
        }

        public ICommand CompleteWorkOrderCommand
        {
            get
            {
                if (_completeWorkOrderCommand == null)
                {
                    _completeWorkOrderCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, CompleteOrder);
                }
                return _completeWorkOrderCommand;
            }
        }
    }
}
