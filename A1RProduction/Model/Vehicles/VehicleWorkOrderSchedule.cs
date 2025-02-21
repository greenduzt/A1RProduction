using A1QSystem.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Vehicles
{
    public class VehicleWorkOrderSchedule : ViewModelBase
    {
        private int _selectedVehicleCategory;
        private DateTime _startDate;
        private bool _oneMonthChecked;
        private bool _sixMonthChecked;
        private bool _oneYearChecked;
        private bool _twoYearsChecked;
        private int _daysBetweenVehicles;
        
        public int VehicleID { get; set; }
        private ObservableCollection<ScheduledVehicle> _scheduledVehicle;

        public int DaysBetweenVehicles
        {
            get
            {
                return _daysBetweenVehicles;
            }
            set
            {
                _daysBetweenVehicles = value;
                RaisePropertyChanged(() => this.DaysBetweenVehicles);
            }
        }

        public int SelectedVehicleCategory
        {
            get
            {
                return _selectedVehicleCategory;
            }
            set
            {
                _selectedVehicleCategory = value;
                RaisePropertyChanged(() => this.SelectedVehicleCategory);
            }
        }

        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                RaisePropertyChanged(() => this.StartDate);
            }
        }

        public ObservableCollection<ScheduledVehicle> ScheduledVehicle
        {
            get
            {
                return _scheduledVehicle;
            }
            set
            {
                _scheduledVehicle = value;
                RaisePropertyChanged(() => this.ScheduledVehicle);
            }
        }

        public bool OneMonthChecked
        {
            get
            {
                return _oneMonthChecked;
            }
            set
            {
                _oneMonthChecked = value;
                RaisePropertyChanged(() => this.OneMonthChecked);
            }
        }

        public bool SixMonthChecked
        {
            get
            {
                return _sixMonthChecked;
            }
            set
            {
                _sixMonthChecked = value;
                RaisePropertyChanged(() => this.SixMonthChecked);
            }
        }

        public bool OneYearChecked
        {
            get
            {
                return _oneYearChecked;
            }
            set
            {
                _oneYearChecked = value;
                RaisePropertyChanged(() => this.OneYearChecked);
            }
        }

        public bool TwoYearsChecked
        {
            get
            {
                return _twoYearsChecked;
            }
            set
            {
                _twoYearsChecked = value;
                RaisePropertyChanged(() => this.TwoYearsChecked);
            }
        }
    }
}
