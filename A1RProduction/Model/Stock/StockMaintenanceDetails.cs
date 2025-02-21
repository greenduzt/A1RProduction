using A1QSystem.Commands;
using A1QSystem.Model.Products;
using A1QSystem.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Model.Stock
{
    public class StockMaintenanceDetails
    {
        public RawStock RawStock { get; set; }
        public RawProduct RawProduct { get; set; }

        private ICommand _editCommand;
        private bool canExecute;

        public StockMaintenanceDetails()
        {
            canExecute = true;
        }

        private void OpenEditRawStock()
        {
            var childWindow = new ChildWindowView();
            childWindow.ShowEditRawStockWindow(this);       

        }

        public ICommand EditCommand
        {
            get
            {
                return _editCommand ?? (_editCommand = new LogOutCommandHandler(() => OpenEditRawStock(), canExecute));
            }
        }
    }
}
