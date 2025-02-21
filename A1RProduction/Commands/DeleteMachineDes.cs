using A1QSystem.ViewModel.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Commands
{
    public class DeleteMachineDes : ICommand
    {
        private readonly MachineMaintenanceDescriptionViewModel machineMaintenanceDescriptionViewModel;


        public DeleteMachineDes(MachineMaintenanceDescriptionViewModel n)
        {
            machineMaintenanceDescriptionViewModel = n;
        }

        public bool CanExecute(object parameter)
        {
            return (machineMaintenanceDescriptionViewModel.SelectedItem != null);
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
            var selectedItem = machineMaintenanceDescriptionViewModel.SelectedItem;
            machineMaintenanceDescriptionViewModel.MachineMaintenanceInfo.Remove(selectedItem);
        }
    }
}
