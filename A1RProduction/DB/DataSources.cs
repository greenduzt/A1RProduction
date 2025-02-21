using A1QSystem.Commands;
using A1QSystem.View;
using A1QSystem.View.Quoting;
using A1QSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.DB
{

    public class DataSources
    {

      //  public ObservableCollection<Product> Products { get; set; }

       

        public DataSources()
        {
        //    Products = new ObservableCollection<Product>();
        //    LoadDummyData();

        }    
        public DataView GetProducts()
        {
            DataView pdv = new DataView();
            pdv = DBAccess.GetAllProducts().Tables["Products"].DefaultView;
            pdv.Sort = "ProductCode ASC";
            return pdv;
        }
        
    }
    /* */

   


   /* public class Freights
    {
        public DataView GetFreightNames()
        {
            return DBAccess.GetFreightNames().Tables["Freight"].DefaultView;
        }
    }
    */
    public class ForkLifts
    {

        public DataView GetForklifts()
        {
            return DBAccess.GetAllForkLifts().Tables["ForkLifts"].DefaultView;
        }
    }

    public class Vehicles
    {
        public DataView GetVehicles()
        {
            return null;//DBAccess.GetAllVehicles().Tables["Vehicles"].DefaultView;
        }
    }   

    
}
