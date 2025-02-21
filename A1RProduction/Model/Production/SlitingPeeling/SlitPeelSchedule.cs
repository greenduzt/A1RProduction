using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Model.Products;
using A1QSystem.Model.Stock;
using A1QSystem.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Model.Production.SlitingPeeling
{
    public class SlitPeelSchedule : ViewModelBase
    {
        public SlitPeelProduction SlitPeelProduction { get; set; }
        public RawProduct RawProduct { get; set; }
        public decimal RawQty { get; set; }
        public decimal ReOrderRawQty { get;  set; }
        public SlitPeel SlitPeel { get; set; }
        public Product Product { get; set; }
        public RawStock RawStock { get; set; }
        public Customer Customer { get; set; }
        public string ShiftName { get; set; }
        private bool canExecute;
        private ICommand _moveCommand;
        public DateTime FreightArrDate { get; set; }
        
        public string FreightArrTime { get; set; }
        
        //public string FreightDescription { get; set; }
        private string _freightName;
        private bool _freightTimeAvailable;
        private bool _freightDateAvailable;
        public string FreightDateTimeVisibility { get; set; }
        public string SalesOrder { get; set; }
        public string FreightVisiblity { get; set; }
        public string ArrivalDateTime { get; set; }

        public SlitPeelSchedule()
        {            
            canExecute = true;
        }
        private void MoveOrder()
        {
            var childWindow = new ChildWindowView();
            childWindow.ShowShiftSlitPeelWindow(this);
        }

        public string FreightDescription
        {
            get { return _freightName; }
            set
            {
                _freightName = value;
                if (FreightDescription == "A1Rubber Stock")
                {
                    FreightVisiblity = "Hidden";
                    FreightDateTimeVisibility = "Hidden";
                }
                else
                {
                    FreightVisiblity = "Visible";
                    FreightDateTimeVisibility = "Visible";
                }
            }
        }

        public bool FreightTimeAvailable 
        {
            get { return _freightTimeAvailable; }
            set 
            { 
                _freightTimeAvailable = value;
                if (FreightTimeAvailable == true)
                {
                    FreightArrTime = string.Empty;
                    FreightDateTimeVisibility = "Hidden";
                }
                else
                {
                    FreightDateTimeVisibility = "Visible";
                }           
            }
        }

        public bool FreightDateAvailable 
        {
            get { return _freightDateAvailable; }
            set { 
                _freightDateAvailable = value;
                if (FreightDateAvailable == true)
                {
                    FreightDateTimeVisibility = "Hidden";
                }
                else
                {
                    FreightDateTimeVisibility = "Visible";
                }             
            } 
        }

        public ICommand MoveCommand
        {
            get
            {
                return _moveCommand ?? (_moveCommand = new LogOutCommandHandler(() => MoveOrder(), canExecute));
            }
        }
    }
}
