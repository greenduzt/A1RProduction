using A1QSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model
{
    public class ProductColourDetails : ObservableObject
    {

        private string _colourName;
        public string ColourName
        {
            get
            {
                return _colourName;
            }
            set
            {
                _colourName = value;
                RaisePropertyChanged(() => this.ColourName);
            }
        }

        private string _bagSize1;
        public string BagSize1
        {
            get
            {
                return _bagSize1;
            }
            set
            {
                _bagSize1 = value;

                RaisePropertyChanged(() => this.BagSize1);
            }
        }
        private string _bagSize2;
        public string BagSize2
        {
            get
            {
                return _bagSize2;
            }
            set
            {
                _bagSize2 = value;

                RaisePropertyChanged(() => this.BagSize2);
            }
        }

        private string _other;
        public string Other
        {
            get
            {
                return _other;
            }
            set
            {
                _other = value;

                RaisePropertyChanged(() => this.Other);
            }
        }
        private int _bagWeight;
        public int BagWeight
        {
            get
            {
                return _bagWeight;
            }
            set
            {
                _bagWeight = value;

                RaisePropertyChanged(() => this.BagWeight);
            }
        }

        

        public ProductColours _CurrentProduct;
        public ProductColours CurrentProduct
        {
            get { return _CurrentProduct; }
            set
            {
                _CurrentProduct = value;
                RaisePropertyChanged(() => this.CurrentProduct);              

            }
        }            

        
    }
}
