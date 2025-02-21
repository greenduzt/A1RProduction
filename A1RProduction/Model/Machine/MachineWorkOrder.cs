using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Other;
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

namespace A1QSystem.Model.Machine
{
    public class MachineWorkOrder : ViewModelBase
    {

        public Int32 WorkOrderNo { get; set; }
        public Machines Machine { get; set; }
        public User User { get; set; }
        public int Urgency { get; set; }
        public string WorkOrderType { get; set; }
        public MachineMaintenanceFrequency MachineMaintenanceFrequency { get; set; }
        //public int Frequency { get; set; }
        public DateTime FirstServiceDate { get; set; }
        public DateTime NextServiceDate { get; set; }
        public ObservableCollection<MachineMaintenanceInfo> MachineMaintenanceInfo { get; set; }
        private List<MachineWorkOrder> dbWorkOrders;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }        
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }

        public DateTime? ShowOrderDate { get; set; }
        public string CompletedBy { get; set; }
        public string Status { get; set; }
        public string UrgencyStr { get; set; }
        public string DaysToComplete { get; set; }
        public string DaysToCompleteBackgroundCol { get; set; }
        public string DaysToCompleteForeGroundCol { get; set; }
        public string UrgencyBackgroundCol { get; set; }
        public string UrgencyForeGroundCol { get; set; }

        public string OrderTypeBackgroundCol { get; set; }
        public string OrderTypeForeGroundCol { get; set; }
        public MachineProvider MachineProvider { get; set; }

        private Machines _machine;
        private string _buttonsVisibility;
        private string _repeatAnimation;
        private string _orderType;
        private bool canExecute;
        private string _fontWeight, _fontSize;
        private ObservableCollection<FileUpload> _fileUploadList;
        private string _reason;
        private string _review;
        private string _externalMechanicName;
        private DateTime? _reviewAddedDateTime;
        private string _reviewAddedBy;

        public MachineWorkOrder()
        {
            canExecute = true;
            RepeatAnimation = "0x";
            MachineMaintenanceInfo = new ObservableCollection<MachineMaintenanceInfo>();
            Machines = new Machines(0);
        }

        public string Review
        {
            get { return _review; }
            set
            {
                _review = value;
                RaisePropertyChanged(() => this.Review);
            }
        }

        public string ReviewAddedBy
        {
            get { return _reviewAddedBy; }
            set
            {
                _reviewAddedBy = value;
                RaisePropertyChanged(() => this.ReviewAddedBy);
            }
        }

        public DateTime? ReviewAddedDateTime
        {
            get { return _reviewAddedDateTime; }
            set
            {
                _reviewAddedDateTime = value;
                RaisePropertyChanged(() => this.ReviewAddedDateTime);
            }
        }

        public string ExternalMechanicName
        {
            get { return _externalMechanicName; }
            set
            {
                _externalMechanicName = value;
                RaisePropertyChanged(() => this.ExternalMechanicName);
            }
        }

        public string Reason
        {
            get { return _reason; }
            set
            {
                _reason = value;
                RaisePropertyChanged(() => this.Reason);
            }
        }

        public ObservableCollection<FileUpload> FileUploadList
        {
            get { return _fileUploadList; }
            set
            {
                _fileUploadList = value;
                RaisePropertyChanged(() => this.FileUploadList);
            }
        }

        public string OrderType
        {
            get { return _orderType; }
            set
            {
                _orderType = value;
                RaisePropertyChanged(() => this.OrderType);
            }
        }
               

        public Machines Machines
        {
            get { return _machine; }
            set
            {
                _machine = value;
                RaisePropertyChanged(() => this.Machines);              
            }
        }

        public string RepeatAnimation
        {
            get { return _repeatAnimation; }
            set
            {
                _repeatAnimation = value;
                RaisePropertyChanged(() => this.RepeatAnimation);
            }
        }

        public string ButtonsVisibility
        {
            get { return _buttonsVisibility; }
            set
            {
                _buttonsVisibility = value;
                RaisePropertyChanged(() => this.ButtonsVisibility);
            }
        }

        public string FontWeight
        {
            get { return _fontWeight; }
            set
            {
                _fontWeight = value;
                RaisePropertyChanged(() => this.FontWeight);
            }
        }

        public string FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                RaisePropertyChanged(() => this.FontSize);
            }
        }
        

    }
}
