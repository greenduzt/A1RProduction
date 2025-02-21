using A1QSystem.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Vehicles
{
    public class VehicleWorkOrder : ViewModelBase
    {
        private ObservableCollection<VehicleMaintenanceInfo> _vehicleMaintenanceInfo;
        public User User { get; set; }
        public Int32 VehicleWorkOrderID { get; set; }
        public string WorkOrderType { get; set; }
        //public ObservableCollection<VehicleMaintenanceInfo> VehicleMaintenanceInfo { get; set; }
        public VehicleMaintenanceSequence VehicleMaintenanceSequence { get; set; }
        public int LargestSeqID { get; set; }
        public Vehicle Vehicle { get; set; }
        public DateTime? FirstServiceDate { get; set; }
        public DateTime? NextServiceDate { get; set; }
        public long LastOdometerReading { get; set; }
        public long OdometerReading { get; set; }
        public string MaintenanceFrequency { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string VehicleSearchString { get; set; }
        public string DaysToComplete { get; set; }
        public string DaysToCompleteBackgroundCol { get; set; }
        public string DaysToCompleteForeGroundCol { get; set; }
        public string Status { get; set; }
        public int Urgency { get; set; }
        public string UrgencyStr { get; set; }
        public string UrgencyBackgroundCol { get; set; }
        public string UrgencyForeGroundCol { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string CompletedBy { get; set; }
        //public bool IsViewed { get; set; }
        private string _repeatAnimation;
        private string _completeBackCol;
        private string _viewRepeatAnimation, _extraNotes;
        private bool _isViewed;
        private bool _completeBtnEnabled;

        public VehicleWorkOrder()
        {
            RepeatAnimation = "0x";
            ViewRepeatAnimation = "0x";    
        }

        public string ExtraNotes
        {
            get
            {
                return _extraNotes;
            }
            set
            {
                _extraNotes = value;
                RaisePropertyChanged(() => this.ExtraNotes);

            }
        }

        public ObservableCollection<VehicleMaintenanceInfo> VehicleMaintenanceInfo
        {
            get
            {
                return _vehicleMaintenanceInfo;
            }
            set
            {
                _vehicleMaintenanceInfo = value;
                RaisePropertyChanged(() => this.VehicleMaintenanceInfo);

            }
        }



        public bool CompleteBtnEnabled
        {
            get { return _completeBtnEnabled; }
            set
            {
                _completeBtnEnabled = value;
                RaisePropertyChanged(() => this.CompleteBtnEnabled);
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

        public string CompleteBackCol
        {
            get { return _completeBackCol; }
            set
            {
                _completeBackCol = value;
                RaisePropertyChanged(() => this.CompleteBackCol);
            }
        }

        public string ViewRepeatAnimation
        {
            get { return _viewRepeatAnimation; }
            set
            {
                _viewRepeatAnimation = value;
                RaisePropertyChanged(() => this.ViewRepeatAnimation);
            }
        }

        

        public bool IsViewed
        {
            get { return _isViewed; }
            set
            {
                _isViewed = value;
                RaisePropertyChanged(() => this.IsViewed);
            }
        }
        
    }
}
