using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using A1QSystem.Core;

namespace A1QSystem.Models
{
    public class Expense : ObservableObject
    {
        private string _productCode;
        private string _productName;
        private double _mixes;
        private string _shift;


        public string ProductCode
        {
            get { return _productCode; }
            set { _productCode = value; RaisePropertyChanged(() => this.ProductCode); }
        }

        public string ProductName 
        {
            get { return _productName; }
            set { _productName = value; RaisePropertyChanged(() => this.ProductName); }  
        }

        public double Mixes
        {
            get { return _mixes; }
            set { _mixes = value; RaisePropertyChanged(() => this.Mixes); }
        }

        public string Shift
        {
            get { return _shift; }
            set { _shift = value;}
        }

    }
}
