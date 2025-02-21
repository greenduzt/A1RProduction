using A1QSystem.ViewModel.VehicleWorkOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Commands
{
    public class DeleteVehicleMaintenanceDes : ICommand
    {
        private readonly VehicleMaintenanceDescriptionsViewModel vehicleMaintenanceDescriptionsViewModel;


        public DeleteVehicleMaintenanceDes(VehicleMaintenanceDescriptionsViewModel n)
        {
            vehicleMaintenanceDescriptionsViewModel = n;
        }

        public bool CanExecute(object parameter)
        {
            return (vehicleMaintenanceDescriptionsViewModel.SelectedItem != null);
        }


        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Invokes this command to perform its intended task.
        /// </summary>
        public void Execute(object parameter)
        {
            var selectedItem = vehicleMaintenanceDescriptionsViewModel.SelectedItem;
            vehicleMaintenanceDescriptionsViewModel.VehicleMaintenanceInfo.Remove(selectedItem);
        }
    }
}
