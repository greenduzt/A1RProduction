using A1QSystem.Commands;
using A1QSystem.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Model.Production
{
    public class ProductionSchedulingDetails
    {
        public OrderProduction OrderProduction { get; set; }
        public OrderProductionDetails OrderProdDetails { get; set; }
        public RawProduct RawProduct { get; set; }
        public decimal QtyToMake { get; set; }
        public int Shift { get; set; }
        public string ShiftName { get; set; }
        public string DisplayDate { get; set; } 
        private ICommand _acceptCommand;
        private ICommand _moveCommand;
        private ICommand _deleteCommand;
        private bool canExecute;

        public ProductionSchedulingDetails()
        {
            canExecute = true;
        }

        private void AcceptOrder()
        {
            
        }

        private void MoveOrder()
        {
            Console.WriteLine(OrderProdDetails.Product.ProductCode + " " + RawProduct.RawProductCode);
        }

        private void DeleteOrder()
        {
            Console.WriteLine(OrderProdDetails.Product.ProductCode + " " + RawProduct.RawProductCode);
        }

        public ICommand AcceptCommand
        {
            get
            {
                return _acceptCommand ?? (_acceptCommand = new LogOutCommandHandler(() => AcceptOrder(), canExecute));
            }
        }
        public ICommand MoveCommand
        {
            get
            {
                return _moveCommand ?? (_moveCommand = new LogOutCommandHandler(() => MoveOrder(), canExecute));
            }
        }
        public ICommand DeleteCommand
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new LogOutCommandHandler(() => DeleteOrder(), canExecute));
            }
        }
    }
}
