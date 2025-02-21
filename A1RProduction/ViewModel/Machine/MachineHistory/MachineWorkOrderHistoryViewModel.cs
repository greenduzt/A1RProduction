using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Machine;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Orders;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using MsgBox;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Machine.MachineHistory
{
    public class MachineWorkOrderHistoryViewModel : ViewModelBase
    {
        private ObservableCollection<Machines> _machines;
        private ObservableCollection<MachineWorkOrderHistory> _machineWorkOrderHistory;
        // private ICollectionView _itemsView;
        private int _selectedMachine;
        private Machines _machine;
        private string userName;
        private string state;
        private string _selectedOrderStatus;
        private List<UserPrivilages> userPrivilages;
        private ChildWindowView myChildWindow;
        private ChildWindowView LoadingScreen;
        private string _version;
        private DateTime _currentDate, _selectedFromDate, _selectedToDate;
        private bool canExecute;
        private ICommand _homeCommand;
        private ICommand _viewCommand;
        private ICommand _printCommand;
        private ICommand _fileUploadCommand, _exportCommand, _searchCommand, _recordCommand;
        private List<MetaData> metaData;

        public MachineWorkOrderHistoryViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {
            SelectedOrderStatus = "All";
            CurrentDate = DateTime.Now;
            SelectedFromDate = DateTime.Now;
            SelectedToDate = DateTime.Now;
            userName = UserName;
            state = State;
            userPrivilages = up;
            canExecute = true;
            metaData = md;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
            MachineWorkOrderHistory = new ObservableCollection<MachineWorkOrderHistory>();
            BackgroundWorker worker = new BackgroundWorker();
            LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Loading");

            worker.DoWork += (_, __) =>
            {
                LoadData();
                LoadMachines();
                SelectedMachine = 0;
            };

            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();
               // _itemsView = CollectionViewSource.GetDefaultView(MachineWorkOrderHistory);
               // _itemsView.Filter = x => Filter(x as MachineWorkOrderHistory);

            };
            worker.RunWorkerAsync();
        }

        private void LoadData()
        {
            MachineWorkOrderHistory = DBAccess.GetMachineWorkOrderHistory(SelectedMachine, SelectedFromDate, SelectedToDate, SelectedOrderStatus);
            //ObservableCollection<MachineWorkOrderHistory> machineWorkOrderHistoryTemp = DBAccess.GetMiscelaniousWorkOrderHistory();
            //foreach (var item in machineWorkOrderHistoryTemp)
            //{
            //    MachineWorkOrderHistory.Add(item);
            //}

            //MachineWorkOrderHistory = new ObservableCollection<MachineWorkOrderHistory>(MachineWorkOrderHistory.OrderBy(i => i.IsCompleted).OrderBy(i=>i.Review).OrderByDescending(i => i.CompletedDate).Select(i=>i));
        }

        private void ExportToExcel()
        {          

            BackgroundWorker worker = new BackgroundWorker();
            ChildWindow LoadingScreen = new ChildWindow();            

            LoadingScreen.ShowWaitingScreen("Working..");

            worker.DoWork += (_, __) =>
            {
                LoadData();

                if (MachineWorkOrderHistory != null && MachineWorkOrderHistory.Count > 0)
                {
                    Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                    Microsoft.Office.Interop.Excel.Workbook worKbooK = excel.Workbooks.Add(Type.Missing);
                    Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)worKbooK.ActiveSheet;

                    excel.Visible = false;
                    excel.DisplayAlerts = false;
                    worksheet.Name = "WorkOrders";
                    int row = 1;
                    worksheet.Range[worksheet.Cells[row, 1], worksheet.Cells[row, 8]].Merge();

                    try
                    {
                        excel = new Microsoft.Office.Interop.Excel.Application();
                        excel.Visible = false;
                        excel.DisplayAlerts = false;
                        worKbooK = excel.Workbooks.Add(Type.Missing);

                        worksheet = (Microsoft.Office.Interop.Excel.Worksheet)worKbooK.ActiveSheet;
                        worksheet.Name = "WorkOrders";

                        row++;
                        worksheet.Range[worksheet.Cells[row, 1], worksheet.Cells[row, 8]].Merge();
                        worksheet.Cells[row, 1] = "MACHINE WORK ORDERS";
                        worksheet.Cells[row, 1].Cells.Font.Bold = true;
                        worksheet.Cells[row, 1].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        row++;

                        worksheet.Range[worksheet.Cells[row, 1], worksheet.Cells[row, 8]].Merge();
                        worksheet.Cells[row, 1] = "Printed Date Time : " + DateTime.Now;
                        worksheet.Cells[row, 1].Font.Bold = true;
                        worksheet.Cells.Font.Size = 12;
                        row += 2;

                        worksheet.Rows[row].Font.Bold = true;
                        worksheet.Rows[row].Font.Size = 14;
                        worksheet.Cells[row, 1] = "Work Order No";
                        worksheet.Cells[row, 1].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        worksheet.Cells[row, 2] = "Work Order Type";
                        worksheet.Cells[row, 2].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        worksheet.Cells[row, 3] = "Frequency";
                        worksheet.Cells[row, 3].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        worksheet.Cells[row, 4] = "Machine ID";
                        worksheet.Cells[row, 5] = "Machine Name";
                        worksheet.Cells[row, 5].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        worksheet.Cells[row, 6] = "Completed By";
                        worksheet.Cells[row, 6].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        worksheet.Cells[row, 7] = "Completed Date";
                        worksheet.Cells[row, 8] = "Status";
                        worksheet.Cells[row, 8].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                        worksheet.Range["A" + row].RowHeight = 30;
                        worksheet.Range["A" + row].ColumnWidth = 100;

                        worksheet.Range["B" + row].RowHeight = 30;
                        worksheet.Range["B" + row].ColumnWidth = 100;

                        worksheet.Range["C" + row].RowHeight = 30;
                        worksheet.Range["C" + row].ColumnWidth = 100;

                        worksheet.Range["D" + row].RowHeight = 30;
                        worksheet.Range["D" + row].ColumnWidth = 100;

                        worksheet.Range["A" + row].Style.HorizontalAlignment = HorizontalAlignment.Center;
                        worksheet.Range["B" + row].Style.HorizontalAlignment = HorizontalAlignment.Center;
                        worksheet.Range["C" + row].Style.HorizontalAlignment = HorizontalAlignment.Center;
                        worksheet.Range["D" + row].Style.HorizontalAlignment = HorizontalAlignment.Center;
                        worksheet.Range["G" + row].Style.HorizontalAlignment = HorizontalAlignment.Center;

                        row++;

                        foreach (var item in MachineWorkOrderHistory)
                        {
                            Range rg = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[row, 7];
                            rg.EntireColumn.NumberFormat = "MM/DD/YYYY";

                            worksheet.Cells[row, 1] = item.WorkOrderNo;
                            worksheet.Cells[row, 2] = item.WorkOrderType;
                            worksheet.Cells[row, 3] = item.MachineMaintenanceFrequency.Frequency;
                            worksheet.Cells[row, 4] = item.Machine.MachineID;
                            worksheet.Cells[row, 5] = item.Machine.MachineName;
                            worksheet.Cells[row, 6] = item.User.FullName;
                            worksheet.Cells[row, 7] = Convert.ToDateTime(item.CompletedDate).ToString("dd/MM/yyyy");
                            worksheet.Cells[row, 8] = item.Status;
                            worksheet.Cells[row, 7].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                            worksheet.Cells[row, 8].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                            if (item.Status.Equals("Not Completed"))
                            {
                                worksheet.Rows[row].Font.Bold = true;
                            }

                            row++;
                        }
                        worksheet.Columns.AutoFit();

                        string fileName = "WorkOrders" + DateTime.Now.ToString("dd-MM-yyyy-HH.mm.ss");

                        excel.Visible = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);

                    }
                    finally
                    {
                        worksheet = null;
                        worKbooK = null;
                    }
                }
                };
                worker.RunWorkerCompleted += delegate (object s, RunWorkerCompletedEventArgs args)
                {
                    LoadingScreen.CloseWaitingScreen();

                };
                worker.RunWorkerAsync();

           
        }
        private void SearchRecords()
        {
            LoadData();
        }

        private void LoadMachines()
        {
            Machines = DBAccess.GetMachinesByLocation(1);
            Machines.Insert(0,new Machines(0) { MachineID = 0, MachineName = "All Machines" });
           
        }        

        private void ViewItems(Object parameter)
        {
            int index = MachineWorkOrderHistory.IndexOf(parameter as MachineWorkOrderHistory);
            if (index >= 0)
            {
                myChildWindow = new ChildWindowView();
                myChildWindow.ShowMachineWorkOrderHistoryItems(MachineWorkOrderHistory[index]);
                myChildWindow.machineWorkOrderHistoryItems_Closed += (r =>
                {
                    if (r > 0)
                    {
                        LoadData();
                    }
                });
            }
        }

        private void ShowUploadFiles(Object parameter)
        {
            int index = MachineWorkOrderHistory.IndexOf(parameter as MachineWorkOrderHistory);
            if (index >= 0)
            {
                ChildWindowView myChildWindow = new ChildWindowView();
                myChildWindow.ShowFileUploadScreen(userName, MachineWorkOrderHistory[index].Machine.MachineID, MachineWorkOrderHistory[index].WorkOrderNo);
                myChildWindow.fileUploadScreenClosed += (r =>
                {
                    if (r > 0)
                    {
                        LoadData();
                    }
                });
            }
        }

        private void Record(Object parameter)
        {
            int index = MachineWorkOrderHistory.IndexOf(parameter as MachineWorkOrderHistory);
            if (index >= 0)
            {
                ChildWindowView myChildWindow = new ChildWindowView();
                myChildWindow.ShowOpenFileUploadedScreen( MachineWorkOrderHistory[index].WorkOrderNo, MachineWorkOrderHistory[index].Machine.MachineName );
                myChildWindow.uploadedFilesClosed += (r =>
                {
                    if (r > 0)
                    {
                        LoadData();
                    }
                });
            }
        }


        private void PrintOrders(Object parameter)
        {
            
            int index = MachineWorkOrderHistory.IndexOf(parameter as MachineWorkOrderHistory);
            if (index >= 0)
            {
                if (Msg.Show("Are you sure you want to print history for work order " + MachineWorkOrderHistory[index].WorkOrderNo + "?", "Printing Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Information_Orange, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {

                    BackgroundWorker worker = new BackgroundWorker();
                    ChildWindowView LoadingScreen;
                    LoadingScreen = new ChildWindowView();
                    LoadingScreen.ShowWaitingScreen("Printing");

                    worker.DoWork += (_, __) =>
                    {

                        if (MachineWorkOrderHistory[index].WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
                        {
                            ObservableCollection<MachineRepairDescription> machineRepairDescriptionList = DBAccess.GetMachineRepairDescriptionByID2(MachineWorkOrderHistory[index].WorkOrderNo);
                            ObservableCollection<MachineParts> vehiclePartsList = DBAccess.GetMachinePartsDescriptionByID(machineRepairDescriptionList);
                            Int32 id = 0;
                            foreach (var item in machineRepairDescriptionList)
                            {
                                id = item.MachineWorkDescriptionID;
                                break;
                            }

                            MachineWorkOrderHistory[index].MahcineWorkDescription = DBAccess.GetMachineWorkDescriptionForRepair(id);

                            foreach (var item in machineRepairDescriptionList)
                            {
                                item.MachineParts = new ObservableCollection<MachineParts>();
                                foreach (var items in vehiclePartsList)
                                {
                                    if (item.ID == items.MachineRepairID)
                                    {
                                        item.MachineParts.Add(items);
                                    }
                                }
                            }
                            if (machineRepairDescriptionList.Count > 0 && MachineWorkOrderHistory[index].MahcineWorkDescription.Count > 0)
                            {
                                MachineWorkOrderHistory[index].MahcineWorkDescription[0].MachineRepairDescription = machineRepairDescriptionList;
                            }
                            else
                            {
                                MachineWorkOrderHistory[index].MahcineWorkDescription.Add(new MachineWorkDescription() { MachineMaintenanceInfo = new MachineMaintenanceInfo() { MachineDescription = "Repair order for " + MachineWorkOrderHistory[index].Machine.MachineName }, MachineRepairDescription = machineRepairDescriptionList });
                            }
                        }
                        if (MachineWorkOrderHistory[index].WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
                        {
                            MachineWorkOrderHistory[index].MahcineWorkDescription = DBAccess.GetMachineWorkDescriptionByID(MachineWorkOrderHistory[index].WorkOrderNo, MachineWorkOrderHistory[index].IsCompleted);
                            if (MachineWorkOrderHistory[index].MahcineWorkDescription.Count > 0)
                            {
                                ObservableCollection<MachineRepairDescription> machineRepairDescriptionList = DBAccess.GetMachineRepairDescriptionByID(MachineWorkOrderHistory[index].MahcineWorkDescription);
                                ObservableCollection<MachineParts> vehiclePartsList = DBAccess.GetMachinePartsDescriptionByID(machineRepairDescriptionList);

                                foreach (var item in MachineWorkOrderHistory[index].MahcineWorkDescription)
                                {
                                    item.MachineRepairDescription = new ObservableCollection<MachineRepairDescription>();
                                    foreach (var items in machineRepairDescriptionList)
                                    {
                                        if (item.ID == items.MachineWorkDescriptionID)
                                        {
                                            item.MachineRepairDescription.Add(items);
                                            items.MachineParts = new ObservableCollection<MachineParts>();
                                            foreach (var itemz in vehiclePartsList)
                                            {
                                                if (items.ID == itemz.MachineRepairID)
                                                {
                                                    items.MachineParts.Add(itemz);
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            //VehicleWorkOrderHistory[index].VehicleWorkOrderDetailsHistory = DBAccess.GetVehicleWorkDescriptionCompleted(VehicleWorkOrderHistory[index].VehicleWorkOrderID);
                        }

                        MachineWorkOrderHistoryPDF vwoh = new MachineWorkOrderHistoryPDF(MachineWorkOrderHistory[index]);
                        vwoh.createWorkOrderPDF();
                    };

                    worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                    {
                        LoadingScreen.CloseWaitingScreen();

                    };
                    worker.RunWorkerAsync();
                }
            }
            
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }


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

        //public ICollectionView ItemsView
        //{
        //    get { return _itemsView; }
        //}

        public ObservableCollection<MachineWorkOrderHistory> MachineWorkOrderHistory
        {
            get { return _machineWorkOrderHistory; }
            set
            {
                _machineWorkOrderHistory = value;
                RaisePropertyChanged(() => this.MachineWorkOrderHistory);
            }
        }             

        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set
            {
                _currentDate = value;
                RaisePropertyChanged(() => this.CurrentDate);
            }
        }

        public DateTime SelectedFromDate
        {
            get { return _selectedFromDate; }
            set
            {
                _selectedFromDate = value;
                RaisePropertyChanged(() => this.SelectedFromDate);
            }
        }

        public DateTime SelectedToDate
        {
            get { return _selectedToDate; }
            set
            {
                _selectedToDate = value;
                RaisePropertyChanged(() => this.SelectedToDate);
            }
        }
        public Machines Machine
        {
            get
            {
                return _machine;
            }
            set
            {
                _machine = value;
                RaisePropertyChanged(() => this.Machine);
            }
        }
        public ObservableCollection<Machines> Machines
        {
            get
            {
                return _machines;
            }
            set
            {
                _machines = value;
                RaisePropertyChanged(() => this.Machines);
            }
        }

        public int SelectedMachine
        {
            get
            {
                return _selectedMachine;
            }
            set
            {
                _selectedMachine = value;
                RaisePropertyChanged(() => this.SelectedMachine);
            }
        }

        public string SelectedOrderStatus
        {
            get
            {
                return _selectedOrderStatus;
            }
            set
            {
                _selectedOrderStatus = value;
                RaisePropertyChanged(() => this.SelectedOrderStatus);
            }
        }        

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand ViewCommand
        {
            get
            {
                if (_viewCommand == null)
                {
                    _viewCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, ViewItems);
                }
                return _viewCommand;
            }
        }

        public ICommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                {
                    _printCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, PrintOrders);
                }
                return _printCommand;
            }
        }


        public ICommand FileUploadCommand
        {
            get
            {
                if (_fileUploadCommand == null)
                {
                    _fileUploadCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, ShowUploadFiles);
                }
                return _fileUploadCommand;
            }
        }
                

        public ICommand ExportCommand
        {
            get
            {
                return _exportCommand ?? (_exportCommand = new LogOutCommandHandler(() => ExportToExcel(), canExecute));
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new LogOutCommandHandler(() => SearchRecords(), canExecute));
            }
        }


        public ICommand RecordCommand
        {
            get
            {
                if (_recordCommand == null)
                {
                    _recordCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, Record);
                }
                return _recordCommand;
            }
        }
      


    }
}
