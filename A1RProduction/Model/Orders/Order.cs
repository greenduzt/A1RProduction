using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.Model.DeliveryDetails;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Orders
{
    public class Order : ViewModelBase
    {
        public Int64 OrderNo { get; set; }
        public int OrderPriority { get; set; }
        public int OrderType { get; set; }       
        //public ObservableCollection<OrderDetails> OrderDetails { get; set; }
        public List<Delivery> DeliveryDetails { get; set; }
        public Customer Customer { get; set; }
        public MasterOrder MasterOrder { get; set; }
        public DateTime RequiredDate { get; set; }//Grading date
        public DateTime MixingDate { get; set; }//Mixing date
        public DateTime OrderCreatedDate { get; set; }
        public bool IsRequiredDateSelected { get; set; }
        public string Comments { get; set; }
        public string MixingComments { get; set; }
        public string SlittingComments { get; set; }
        public string PeelingComments { get; set; }
        public string ReRollingComments { get; set; }
        public string SalesNo { get; set; }
        public decimal ListPriceTotal { get; set; }
        public decimal GST { get; set; }
        public decimal TotalAmount { get; set; }
        public string SearchString { get; set; }
        public string MixingShift { get; set; }

        private ObservableCollection<OrderDetails> _orderDetails;


        public ObservableCollection<OrderDetails> OrderDetails
        {
            get { return _orderDetails; }
            set
            {
                _orderDetails = value;
                RaisePropertyChanged(() => this.OrderDetails);
            }
        }
    }
}
