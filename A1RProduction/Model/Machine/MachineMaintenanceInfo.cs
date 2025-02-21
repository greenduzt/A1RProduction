using A1QSystem.Core;
using A1QSystem.Interfaces;
using A1QSystem.Model.Vehicles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Machine
{
    public class MachineMaintenanceInfo : ViewModelBase, ISequencedObject
    {
        public Int32 ID { get; set; }
        public Machines Machine { get; set; }
        public MachineMaintenanceFrequency MachineMaintenanceFrequency { get; set; }
        //public string MachineCode { get; set; }
        public string MachineDescription { get; set; }
        //public string Repetition { get; set; }
        public DateTime CreatedDate { get; set; }
        public string  CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public DateTime FirstDate { get; set; }
        public DateTime PreferredDate { get; set; }
        public DateTime LastDate { get; set; }

        private string _machineCode;
        private string _workItemVisible;
        private string _repetition;
        private bool _monChecked;
        private bool _tueChecked;
        private bool _wedChecked;
        private bool _thusChecked;
        private bool _friChecked;
        private int p_SequenceNumber;
        private string _sequenceStr;
        private string _orderType;
        private int _providerID;
        private int _showOrderBeforeDays;


        public string OrderType
        {
            get { return _orderType; }

            set
            {
                _orderType = value;
                base.RaisePropertyChanged(() => this.OrderType);
            }
        }

        public int ProviderID
        {
            get { return _providerID; }

            set
            {
                _providerID = value;
                base.RaisePropertyChanged(() => this.ProviderID);
            }
        }

        public int ShowOrderBeforeDays
        {
            get { return _showOrderBeforeDays; }

            set
            {
                _showOrderBeforeDays = value;
                base.RaisePropertyChanged(() => this.ShowOrderBeforeDays);
            }
        }

        public string SequenceStr
        {
            get { return _sequenceStr; }

            set
            {
                _sequenceStr = value;
                base.RaisePropertyChanged(() => this.SequenceStr);
            }
        }

        public int SequenceNumber
        {
            get { return p_SequenceNumber; }

            set
            {
                p_SequenceNumber = value;
                base.RaisePropertyChanged(() => this.SequenceNumber);

                SequenceStr = "M" + SequenceNumber;
            }
        }

        public bool MonChecked
        {
            get { return _monChecked; }

            set
            {
                _monChecked = value;
                base.RaisePropertyChanged(() => this.MonChecked);
            }
        }

        public bool TueChecked
        {
            get { return _tueChecked; }

            set
            {
                _tueChecked = value;
                base.RaisePropertyChanged(() => this.TueChecked);
            }
        }

        public bool WedChecked
        {
            get { return _wedChecked; }

            set
            {
                _wedChecked = value;
                base.RaisePropertyChanged(() => this.WedChecked);
            }
        }

        public bool ThusChecked
        {
            get { return _thusChecked; }

            set
            {
                _thusChecked = value;
                base.RaisePropertyChanged(() => this.ThusChecked);
            }
        }

        public bool FriChecked
        {
            get { return _friChecked; }

            set
            {
                _friChecked = value;
                base.RaisePropertyChanged(() => this.FriChecked);
            }
        }

        public string WorkItemVisible
        {
            get { return _workItemVisible; }

            set
            {
                _workItemVisible = value;
                base.RaisePropertyChanged(() => this.WorkItemVisible);
            }
        }

        public string MachineCode
        {
            get { return _machineCode; }

            set
            {
                _machineCode = value;
                base.RaisePropertyChanged(() => this.MachineCode);
                if (!string.IsNullOrWhiteSpace(MachineCode))
                {
                    if (MachineCode == "Select")
                    {
                        WorkItemVisible = "Collapsed";
                    }
                    else
                    {
                        WorkItemVisible = "Visible";
                    }
                }
            }
        }

        public string Repetition
        {
            get { return _repetition; }

            set
            {
                _repetition = value;
                base.RaisePropertyChanged(() => this.Repetition);

                if(!String.IsNullOrWhiteSpace(Repetition))
                {                
                    string[] tokens = Repetition.Split(',');
                    if (tokens.Count() > 0)
                    {
                        foreach (var item in tokens)
                        {
                            if (item == "mon")
                            {
                                MonChecked = true;
                            }
                            else if (item == "tue")
                            {
                                TueChecked = true;
                            }
                            else if (item == "wed")
                            {
                                WedChecked = true;
                            }
                            else if (item == "thus")
                            {
                                ThusChecked = true;
                            }
                            else if (item == "fri")
                            {
                                FriChecked = true;
                            }
                        }
                    }
                }
            }
        }

        
    }
}
