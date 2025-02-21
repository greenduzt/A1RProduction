using A1QSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model
{
    public class GridItem : ViewModelBase1
    {
     //   public string ProductName { get; set; }
     //   public int MixesCompleted { get; set; }      


      


        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set
            {
                _productName = value;
                OnPropertyChanged("ProductName");             
                Console.WriteLine("Product Name " + _productName);

            }
        }        
       
        private int _mixesCompleted;
        public int MixesCompleted
        {
            get { return _mixesCompleted; }
            set
            {
                _mixesCompleted = value;               

                OnPropertyChanged("MixesCompleted");
                OnPropertyChanged("DayShiftMixes");
                Console.WriteLine("Mixes Completed " + MixesCompleted);

            }
        }

      

       

      
 /**/
    }
}
