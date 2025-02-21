using A1QSystem.ViewModel.VehicleWorkOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Commands
{
    public class DeleteIVehicleRepairItemCommand : ICommand
    {
        private readonly InnerVehicleRepairWorkOrderViewModel innerVehicleRepairWorkOrderViewModel;

        public DeleteIVehicleRepairItemCommand(InnerVehicleRepairWorkOrderViewModel ivrwovm)
        {
            innerVehicleRepairWorkOrderViewModel = ivrwovm;
        }


        public bool CanExecute(object parameter)
        {
            return (innerVehicleRepairWorkOrderViewModel.SelectedItem != null);
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
            var selectedItem = innerVehicleRepairWorkOrderViewModel.SelectedItem;
            innerVehicleRepairWorkOrderViewModel.VehicleRepairWorkOrder.VehicleRepairDescription.Remove(selectedItem);
        }
    }
}
