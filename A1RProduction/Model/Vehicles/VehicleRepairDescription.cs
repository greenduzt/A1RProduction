using A1QSystem.Core;
using A1QSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Vehicles
{
    public class VehicleRepairDescription : ObservableObject, ISequencedObject
    {
        public Int32 ID { get; set; }
        public Int32 VehicleWorkDescriptionID { get; set; }
        public Int32 VehicleWorkOrderID { get; set; }
        public string RepairDescription { get; set; }
        public bool IsActive { get; set; }
        
        //public ObservableCollection<VehicleParts> Vehicleparts { get; set; }
        private bool _isCompleted;
        private bool _partsOrdered;
        private int p_SequenceNumber;
        private string _strSequenceNumber;
        private string _repairVisiblity;
        private string _repairCompletedVisibility;
        private ObservableCollection<VehicleParts> _vehicleparts;        

        public VehicleRepairDescription()
        {
            Vehicleparts = new ObservableCollection<VehicleParts>();
        }

        public int SequenceNumber
        {
            get { return p_SequenceNumber; }

            set
            {
                p_SequenceNumber = value;
                base.RaisePropertyChanged(() => this.SequenceNumber);
                StrSequenceNumber = "R" + SequenceNumber.ToString();
            }
        }

        public string StrSequenceNumber
        {
            get { return _strSequenceNumber; }

            set
            {
                _strSequenceNumber = value;
                base.RaisePropertyChanged(() => this.StrSequenceNumber);
                if (StrSequenceNumber == "Select")
                {
                    RepairVisiblity = "Collapsed";
                }
                else
                {
                    RepairVisiblity = "Visible";
                }
            }
        }

        public bool PartsOrdered
        {
            get { return _partsOrdered; }

            set
            {
                _partsOrdered = value;
                base.RaisePropertyChanged(() => this.PartsOrdered);
            }
        }

        public string RepairVisiblity
        {
            get { return _repairVisiblity; }

            set
            {
                _repairVisiblity = value;
                RaisePropertyChanged(() => this.RepairVisiblity);
            }
        }
        
        

        public ObservableCollection<VehicleParts> Vehicleparts
        {
            get { return _vehicleparts; }

            set
            {
                _vehicleparts = value;
                RaisePropertyChanged(() => this.Vehicleparts);
            }
        }



        public string RepairCompletedVisibility
        {
            get { return _repairCompletedVisibility; }

            set
            {
                _repairCompletedVisibility = value;
                RaisePropertyChanged(() => this.RepairCompletedVisibility);
            }
        }

        public bool IsCompleted
        {
            get { return _isCompleted; }

            set
            {
                _isCompleted = value;
                RaisePropertyChanged(() => this.IsCompleted);
            }
        }

      
        //public VehicleRepairDescription(int itemIndex)
        //{
        //    p_SequenceNumber = itemIndex;
        //}
    }
}
