using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Machine;
using A1QSystem.Model.Meta;
using A1QSystem.PDFGeneration;
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

namespace A1QSystem.ViewModel.Machine.MachinePart
{
    public class PrintMachinePartsViewModel : ViewModelBase
    {
        private ObservableCollection<Int32> _workOrderNo;
        private ObservableCollection<Machines> _machine;
        private ObservableCollection<MachinePartDescription> _machinePartDescription;
        private List<UserPrivilages> privilages;
        private string userName;
        private string state;
        private string _searchButtonBackground;
        private List<MetaData> metaData;
        private int _selectedMachine;
        private Int32 _selectedWorkOrderNo;
        private bool _searchEnabled;
        private bool _printEnabled;
        private string _version;
        private string _printButtonBackground;
        private bool canExecute;
        private ObservableCollection<MachinePartDescription> vpdList;

        private ICommand _homeCommand;
        private ICommand _searchCommand;
        private ICommand _printCommand;

        public PrintMachinePartsViewModel(string UserName, string State, List<UserPrivilages> UserPrivilages, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = UserPrivilages;
            metaData = md;
            MachinePartDescription = new ObservableCollection<MachinePartDescription>();
            WorkOrderNo = new ObservableCollection<Int32>();
            SearchEnabled = false;
            LoadMachines();
            canExecute = true;    
        }

        private void LoadMachineParts()
        {
            PrintEnabled = false;
            PrintButtonBackground = "#FFDEDEDE";
            MachinePartDescription.Clear();
            vpdList = DBAccess.GetMachineParts(SelectedMachine);
            WorkOrderNo.Clear();
            if (vpdList.Count > 0)
            {
                WorkOrderNo.Add(0);
                foreach (var item in vpdList)
                {
                    if (item.Machine.MachineID == SelectedMachine)
                    {
                        var data = WorkOrderNo.SingleOrDefault(x => x == item.ID);
                        if (data == 0)
                        {
                            WorkOrderNo.Add(item.ID);
                        }
                    }
                }
                SelectedWorkOrderNo = 0;
            }
            else
            {
                WorkOrderNo.Clear();
                WorkOrderNo.Add(0);
                SelectedWorkOrderNo = 0;
            }
        }

        private void LoadMachines()
        {
            Machine = DBAccess.GetMachinesByLocation(1);
            Machine.Add(new Machines(0) { MachineID = 0, MachineString = "Select" });
            SelectedMachine = 0;
        }

        private void SearchPart()
        {
            if (vpdList.Count > 0)
            {
                //MachinePartDescription = new ObservableCollection<MachinePartDescription>();
                //foreach (var item in vpdList)
                //{
                //    if (item.ID == SelectedWorkOrderNo)
                //    {
                //        MachinePartDescription.Add(item);
                //    }
                //}
                MachinePartDescription = vpdList;
            }
        }

        private void EnableSearch()
        {
            PrintEnabled = false;
            PrintButtonBackground = "#FFDEDEDE";

            if (SelectedMachine > 0 && SelectedWorkOrderNo > 0)
            {
                SearchEnabled = true;
                SearchButtonBackground = "#FF787C7A";
            }
            else
            {
                SearchEnabled = false;
                SearchButtonBackground = "#FFDEDEDE";
            }
        }

        private void PrintPart()
        {
            if (Msg.Show("Would you like to print selected machine parts ?", "Printing Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Information_Orange, MsgBoxResult.Yes) == MsgBoxResult.Yes)
            {
                            
                Exception exception = null;
                BackgroundWorker worker = new BackgroundWorker();
                ChildWindowView LoadingScreen;
                LoadingScreen = new ChildWindowView();
                LoadingScreen.ShowWaitingScreen("Printing");

                worker.DoWork += (_, __) =>
                {
                    Machines machine = Machine.SingleOrDefault(x => x.MachineID == SelectedMachine);

                    PrintMachinePartPDF ppdf = new PrintMachinePartPDF(machine, SelectedWorkOrderNo, MachinePartDescription);
                    exception = ppdf.createWorkOrderPDF();
                };

                worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                {
                    LoadingScreen.CloseWaitingScreen();
                    if (exception != null)
                    {
                        Msg.Show("A problem has occured while prining and did not print properly. Please try again later.", "Printing Error", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                    }

                };
                worker.RunWorkerAsync();
              }
        }


        public ObservableCollection<MachinePartDescription> MachinePartDescription
        {
            get
            {
                return _machinePartDescription;
            }
            set
            {
                _machinePartDescription = value;
                RaisePropertyChanged(() => this.MachinePartDescription);
                if (MachinePartDescription.Count > 0)
                {
                    PrintEnabled = true;
                    PrintButtonBackground = "#FF787C7A";
                }
                else
                {
                    PrintEnabled = false;
                    PrintButtonBackground = "#FFDEDEDE";
                }
            }
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

        public ObservableCollection<Machines> Machine
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
                LoadMachineParts();
            }
        }

        public ObservableCollection<Int32> WorkOrderNo
        {
            get
            {
                return _workOrderNo;
            }
            set
            {
                _workOrderNo = value;
                RaisePropertyChanged(() => this.WorkOrderNo);
            }
        }

        public Int32 SelectedWorkOrderNo
        {
            get
            {
                return _selectedWorkOrderNo;
            }
            set
            {
                _selectedWorkOrderNo = value;
                RaisePropertyChanged(() => this.SelectedWorkOrderNo);
                EnableSearch();
            }
        }

        public bool SearchEnabled
        {
            get
            {
                return _searchEnabled;
            }
            set
            {
                _searchEnabled = value;
                RaisePropertyChanged(() => this.SearchEnabled);
            }
        }

        public string SearchButtonBackground
        {
            get
            {
                return _searchButtonBackground;
            }
            set
            {
                _searchButtonBackground = value;
                RaisePropertyChanged(() => this.SearchButtonBackground);
            }
        }

        public bool PrintEnabled
        {
            get
            {
                return _printEnabled;
            }
            set
            {
                _printEnabled = value;
                RaisePropertyChanged(() => this.PrintEnabled);
            }
        }

        public string PrintButtonBackground
        {
            get
            {
                return _printButtonBackground;
            }
            set
            {
                _printButtonBackground = value;
                RaisePropertyChanged(() => this.PrintButtonBackground);
            }
        }



        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new A1QSystem.Commands.LogOutCommandHandler(() => SearchPart(), canExecute));
            }
        }

        public ICommand PrintCommand
        {
            get
            {
                return _printCommand ?? (_printCommand = new A1QSystem.Commands.LogOutCommandHandler(() => PrintPart(), canExecute));
            }
        }

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, privilages, metaData)), canExecute));
            }
        }
    }
}
