using A1QSystem.ViewModel.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Commands
{
    public class DeleteNewVehicleRepairItem : ICommand
    {
        private readonly NewVehicleRepairWorkOrderViewModel newVehicleRepairWorkOrderViewModel;

        public DeleteNewVehicleRepairItem(NewVehicleRepairWorkOrderViewModel n)
        {
            newVehicleRepairWorkOrderViewModel = n;
        }

        public bool CanExecute(object parameter)
        {
            return (newVehicleRepairWorkOrderViewModel.SelectedItem != null);
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
            var selectedItem = newVehicleRepairWorkOrderViewModel.SelectedItem;
            newVehicleRepairWorkOrderViewModel.VehicleRepairWorkOrder.VehicleRepairDescription.Remove(selectedItem);
        }
    }
}
