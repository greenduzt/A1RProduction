using A1QSystem.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Vehicles
{
    public class VehicleWorkDescription : ViewModelBase
    {
        public Int32 ID { get; set; }
        public int VehicleMaintenanceSequenceID { get; set; }
        public VehicleWorkOrder VehicleWorkOrder { get; set; }
        public VehicleMaintenanceInfo VehicleMaintenanceInfo { get; set; }
        
        public string Description { get; set; }
        //public bool IsCompleted { get; set; }
        public bool IsActive { get; set; }
        public string Comment { get; set; }
        private Int32 _vehicleRepairWorkOrderID;
        private ObservableCollection<VehicleRepairDescription> _vehicleRepairDescription;
        private string _completedVisibility;
        private string _repairOrderVisibility;
        private string _repairVisibility;
        private string _normalVisibility;
        private string _rootVisibility;
        private bool _partsOrdered;
        private bool _problemBtnEnabled;
        private bool _isCompleted;
        private bool _result;
        private bool _itemDone;
        private bool _itemRepair;
        private bool _itemDoneEnabled;
        private bool _itemRepairEnabled;

        public VehicleWorkDescription()
        {
            ProblemBtnEnabled = false;
            VehicleRepairDescription = new ObservableCollection<VehicleRepairDescription>();
            
            
        }

        public Int32 VehicleRepairWorkOrderID
        {
            get { return _vehicleRepairWorkOrderID; }
            set
            {
                _vehicleRepairWorkOrderID = value;
                RaisePropertyChanged(() => this.VehicleRepairWorkOrderID);
                //if (VehicleRepairWorkOrderID > 0)
                //{
                //    CompletedVisibility= "Collapsed";
                //    RepairOrderVisibility = "Visible";
                //}
                //else
                //{
                //    CompletedVisibility = "Visible";
                //    RepairOrderVisibility = "Collapsed";
                //}
            }
        }



        public string RootVisibility
        {
            get { return _rootVisibility; }
            set
            {
                _rootVisibility = value;
                RaisePropertyChanged(() => this.RootVisibility);
            }
        }

        public string CompletedVisibility
        {
            get { return _completedVisibility; }
            set
            {
                _completedVisibility = value;
                RaisePropertyChanged(() => this.CompletedVisibility);
            }
        }

        public string RepairOrderVisibility
        {
            get { return _repairOrderVisibility; }
            set
            {
                _repairOrderVisibility = value;
                RaisePropertyChanged(() => this.RepairOrderVisibility);
            }
        }
        

        public bool PartsOrdered
        {
            get { return _partsOrdered; }
            set
            {
                _partsOrdered = value;
                RaisePropertyChanged(() => this.PartsOrdered);                
            }
        }

        public bool IsCompleted
        {
            get { return _isCompleted; }
            set
            {
                _isCompleted = value;
                RaisePropertyChanged(() => this.IsCompleted);       
                if(IsCompleted==true)
                {
                    ItemDone = true;
                }
                else
                {
                    ItemDone = false;
                }
            }
        }


        public bool ProblemBtnEnabled
        {
            get
            {
                return _problemBtnEnabled;
            }
            set
            {
                _problemBtnEnabled = value;
                RaisePropertyChanged(() => this.ProblemBtnEnabled);
            }
        }

        public bool Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
                RaisePropertyChanged(() => this.Result);

                //if (Result)
                //{
                //    ProblemBtnEnabled = false;
                //}
                //else
                //{
                //    ProblemBtnEnabled = true;
                //}
            }
        }

        public bool ItemDone
        {
            get
            {
                return _itemDone;
            }
            set
            {
                _itemDone = value;
                RaisePropertyChanged(() => this.ItemDone);

                if (ItemDone == true)
                {
                    ProblemBtnEnabled = false;
                }
                else if (ItemDone == false && VehicleRepairWorkOrderID == 0)
                {
                    ProblemBtnEnabled = true;
                }
            }
        }

        public bool ItemRepair
        {
            get
            {
                return _itemRepair;
            }
            set
            {
                _itemRepair = value;
                RaisePropertyChanged(() => this.ItemRepair);
            }
        }

        public bool ItemDoneEnabled
        {
            get
            {
                return _itemDoneEnabled;
            }
            set
            {
                _itemDoneEnabled = value;
                RaisePropertyChanged(() => this.ItemDoneEnabled);
            }
        }

        public bool ItemRepairEnabled
        {
            get
            {
                return _itemRepairEnabled;
            }
            set
            {
                _itemRepairEnabled = value;
                RaisePropertyChanged(() => this.ItemRepairEnabled);
            }
        }

        public string RepairVisibility
        {
            get
            {
                return _repairVisibility;
            }
            set
            {
                _repairVisibility = value;
                RaisePropertyChanged(() => this.RepairVisibility);
            }
        }

        public string NormalVisibility
        {
            get
            {
                return _normalVisibility;
            }
            set
            {
                _normalVisibility = value;
                RaisePropertyChanged(() => this.NormalVisibility);
            }
        }

        public ObservableCollection<VehicleRepairDescription> VehicleRepairDescription
        {
            get
            {
                return _vehicleRepairDescription;
            }
            set
            {
                _vehicleRepairDescription = value;
                RaisePropertyChanged(() => this.VehicleRepairDescription);               
            }
        }    

        
    }
}
