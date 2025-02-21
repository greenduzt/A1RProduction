﻿using A1QSystem.Model.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Capacity
{
    public class CapacityInfo
    {
        public ProductionTimeTable ProductionTimeTable { get; set; }
        public List<GradingDefaultCapacity> GradingCapacityList { get; set; }
        public List<MixingDefaultCapacity> MixingCapacityList { get; set; }
        public List<SlittingDefaultCapacity> SlittingCapacityList { get; set; }
        public List<PeelingDefaultCapacity> PeelingCapacityList { get; set; }
        public List<ReRollingDefaultCapacity> ReRollingCapacityList { get; set; }
    }
}
