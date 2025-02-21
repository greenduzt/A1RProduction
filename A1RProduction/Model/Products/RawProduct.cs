using A1QSystem.Core;
using A1QSystem.Model.Formula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Products
{
    public class RawProduct : ViewModelBase
    {

        public int RawProductID { get; set; }
        public int SalesID { get; set; }
        public string RawProductCode { get; set; }  
        public string RawProductName { get; set; }
        public string RawProductType { get; set; }
        public decimal Cost { get; set; }
        public bool Active { get; set; }

        private string _description;
        private Formulas _formula;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged(() => this.Description);
            }
        }

        public Formulas Formula
        {
            get { return _formula; }
            set
            {
                _formula = value;
                RaisePropertyChanged(() => this.Formula);
            }
        }

    }
}
