using A1QSystem.ViewModel.Machine.MachineWorkOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Commands
{
    public class DeleteMachineRepairItem : ICommand
    {
        private readonly NewMachineRepairWorkOrderViewModel newMachineRepairWorkOrderViewModel;

        public DeleteMachineRepairItem(NewMachineRepairWorkOrderViewModel n)
        {
            newMachineRepairWorkOrderViewModel = n;
        }

        public bool CanExecute(object parameter)
        {
            return (newMachineRepairWorkOrderViewModel.SelectedItem != null);
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
            var selectedItem = newMachineRepairWorkOrderViewModel.SelectedItem;
            newMachineRepairWorkOrderViewModel.MachineRepairWorkOrder.MachineRepairDescription.Remove(selectedItem);
        }
    }
}
