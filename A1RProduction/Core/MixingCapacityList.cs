using A1QSystem.Model.Capacity;
using A1QSystem.Model.Production.Mixing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{
    public class MixingCapacityList
    {
        public List<MixingCapacity> HighSpeedMixingCapacityList { get; set; }
        public List<MixingCapacity> LargeMixingCapacityList { get; set; }

        public List<MixingCapacity> PopulateLargeMixer()
        {
            HighSpeedMixingCapacityList = new List<MixingCapacity>();
            HighSpeedMixingCapacityList.Add(new MixingCapacity() { ProdTimeTableID = 0, MaxMixes = 100 });
           
            return HighSpeedMixingCapacityList;
        }

        public List<MixingCapacity> PopulateHighSpeedMixer()
        {
            LargeMixingCapacityList = new List<MixingCapacity>();
            LargeMixingCapacityList.Add(new MixingCapacity() { ProdTimeTableID = 0, MaxMixes = 13 });
          
            return LargeMixingCapacityList;
        }
       
    }
}
