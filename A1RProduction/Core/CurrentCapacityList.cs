using A1QSystem.Model.Capacity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{
    public class CurrentCapacityList
    {
        public List<ProductCapacity> NewCurrentCapacityList { get; set; }

        public CurrentCapacityList(DateTime date)
        {
            NewCurrentCapacityList = new List<ProductCapacity>();

            if (date.DayOfWeek == DayOfWeek.Friday)
            {
                //Friday only

                //New capacities as at 25/05/2016
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 1, CapacityKG = 2560, GradedKG = 0 });//4Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 1, CapacityKG = 3040, GradedKG = 0 });//12Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 1, CapacityKG = 1280, GradedKG = 0 });//16Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 1, CapacityKG = 1120, GradedKG = 0 });//30Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 1, CapacityKG = 1120, GradedKG = 0 });//30/40Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 1, CapacityKG = 1000, GradedKG = 0 });//regrind unsure

                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 2, CapacityKG = 1600, GradedKG = 0 });//4Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 2, CapacityKG = 1900, GradedKG = 0 });//12Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 2, CapacityKG = 800, GradedKG = 0 });//16Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 2, CapacityKG = 700, GradedKG = 0 });//30Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 2, CapacityKG = 700, GradedKG = 0 });//30/40Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 2, CapacityKG = 1000, GradedKG = 0 });//regrind unsure

                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 3, CapacityKG = 1600, GradedKG = 0 });//4Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 3, CapacityKG = 1900, GradedKG = 0 });//12Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 3, CapacityKG = 800, GradedKG = 0 });//16Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 3, CapacityKG = 700, GradedKG = 0 });//30Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 3, CapacityKG = 700, GradedKG = 0 });//30/40Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 3, CapacityKG = 1000, GradedKG = 0 });//regrind

                ////New capacities as at 16/05/2016
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 1, CapacityKG = 3600, GradedKG = 0 });//4Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 1, CapacityKG = 2240, GradedKG = 0 });//12Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 1, CapacityKG = 1280, GradedKG = 0 });//16Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 1, CapacityKG = 880, GradedKG = 0 });//30Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 1, CapacityKG = 880, GradedKG = 0 });//30/40Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 1, CapacityKG = 1000, GradedKG = 0 });//regrind unsure

                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 2, CapacityKG = 2250, GradedKG = 0 });//4Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 2, CapacityKG = 1400, GradedKG = 0 });//12Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 2, CapacityKG = 800, GradedKG = 0 });//16Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 2, CapacityKG = 550, GradedKG = 0 });//30Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 2, CapacityKG = 550, GradedKG = 0 });//30/40Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 2, CapacityKG = 1000, GradedKG = 0 });//regrind unsure

                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 3, CapacityKG = 2250, GradedKG = 0 });//4Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 3, CapacityKG = 1400, GradedKG = 0 });//12Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 3, CapacityKG = 800, GradedKG = 0 });//16Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 3, CapacityKG = 550, GradedKG = 0 });//30Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 3, CapacityKG = 550, GradedKG = 0 });//30/40Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 3, CapacityKG = 1000, GradedKG = 0 });//regrind

                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 1, CapacityKG = 3100, GradedKG = 0 });//4Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 1, CapacityKG = 1612, GradedKG = 0 });//12Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 1, CapacityKG = 806, GradedKG = 0 });//16Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 1, CapacityKG = 682, GradedKG = 0 });//30Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 1, CapacityKG = 682, GradedKG = 0 });//30/40Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 1, CapacityKG = 600, GradedKG = 0 });//regrind unsure

                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 2, CapacityKG = 2750, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 2, CapacityKG = 1430, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 2, CapacityKG = 715, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 2, CapacityKG = 605, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 2, CapacityKG = 605, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 2, CapacityKG = 600, GradedKG = 0 });

                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 3, CapacityKG = 2750, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 3, CapacityKG = 1430, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 3, CapacityKG = 715, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 3, CapacityKG = 605, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 3, CapacityKG = 605, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 3, CapacityKG = 600, GradedKG = 0 });

                ////For testing 18,600
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 1, CapacityKG = 9300, GradedKG = 0 });//4Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 1, CapacityKG = 4836, GradedKG = 0 });//12Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 1, CapacityKG = 2418, GradedKG = 0 });//16Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 1, CapacityKG = 2046, GradedKG = 0 });//30Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 1, CapacityKG = 2046, GradedKG = 0 });//30/40Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 1, CapacityKG = 2000, GradedKG = 0 });//regrind unsure

                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 2, CapacityKG = 8250, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 2, CapacityKG = 4290, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 2, CapacityKG = 2145, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 2, CapacityKG = 1815, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 2, CapacityKG = 1815, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 2, CapacityKG = 1800, GradedKG = 0 });

                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 3, CapacityKG = 8250, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 3, CapacityKG = 4290, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 3, CapacityKG = 2145, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 3, CapacityKG = 1815, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 3, CapacityKG = 1815, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 3, CapacityKG = 1800, GradedKG = 0 });

                ////For testing 7,440
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 1, CapacityKG = 3720, GradedKG = 0 });//4Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 1, CapacityKG = 1935, GradedKG = 0 });//12Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 1, CapacityKG = 968, GradedKG = 0 });//16Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 1, CapacityKG = 819, GradedKG = 0 });//30Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 1, CapacityKG = 819, GradedKG = 0 });//30/40Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 1, CapacityKG = 800, GradedKG = 0 });//regrind unsure

                ////For testing 6,600
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 2, CapacityKG = 3300, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 2, CapacityKG = 1716, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 2, CapacityKG = 858, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 2, CapacityKG = 726, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 2, CapacityKG = 726, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 2, CapacityKG = 700, GradedKG = 0 });

                ////For testing 6,600
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 3, CapacityKG = 3300, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 3, CapacityKG = 1716, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 3, CapacityKG = 858, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 3, CapacityKG = 726, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 3, CapacityKG = 726, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 3, CapacityKG = 700, GradedKG = 0 });
            }
            else
            {
                //New capacities as at 25/05/2016
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 1, CapacityKG = 3520, GradedKG = 0 });//4Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 1, CapacityKG = 4180, GradedKG = 0 });//12Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 1, CapacityKG = 1760, GradedKG = 0 });//16Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 1, CapacityKG = 1540, GradedKG = 0 });//30Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 1, CapacityKG = 1540, GradedKG = 0 });//30/40Mesh
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 1, CapacityKG = 1500, GradedKG = 0 });//regrind

                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 2, CapacityKG = 2560, GradedKG = 0 });
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 2, CapacityKG = 3040, GradedKG = 0 });
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 2, CapacityKG = 1280, GradedKG = 0 });
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 2, CapacityKG = 1120, GradedKG = 0 });
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 2, CapacityKG = 1120, GradedKG = 0 });
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 2, CapacityKG = 1500, GradedKG = 0 });

                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 3, CapacityKG = 2240, GradedKG = 0 });
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 3, CapacityKG = 2660, GradedKG = 0 });
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 3, CapacityKG = 1120, GradedKG = 0 });
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 3, CapacityKG = 980, GradedKG = 0 });
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 3, CapacityKG = 980, GradedKG = 0 });
                NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 3, CapacityKG = 1500, GradedKG = 0 });  

                ////New capacities as at 16/05/2016
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 1, CapacityKG = 4950, GradedKG = 0 });//4Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 1, CapacityKG = 3080, GradedKG = 0 });//12Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 1, CapacityKG = 1760, GradedKG = 0 });//16Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 1, CapacityKG = 1210, GradedKG = 0 });//30Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 1, CapacityKG = 1210, GradedKG = 0 });//30/40Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 1, CapacityKG = 1500, GradedKG = 0 });//regrind

                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 2, CapacityKG = 3600, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 2, CapacityKG = 2240, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 2, CapacityKG = 1280, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 2, CapacityKG = 880, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 2, CapacityKG = 880, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 2, CapacityKG = 1500, GradedKG = 0 });

                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 3, CapacityKG = 3150, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 3, CapacityKG = 1960, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 3, CapacityKG = 1120, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 3, CapacityKG = 770, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 3, CapacityKG = 770, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 3, CapacityKG = 1500, GradedKG = 0 });  

                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 1, CapacityKG = 5200, GradedKG = 0 });//4Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 1, CapacityKG = 2704, GradedKG = 0 });//12Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 1, CapacityKG = 1352, GradedKG = 0 });//16Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 1, CapacityKG = 1144, GradedKG = 0 });//30Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 1, CapacityKG = 1144, GradedKG = 0 });//30/40Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 1, CapacityKG = 1000, GradedKG = 0 });//regrind

                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 2, CapacityKG = 4600, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 2, CapacityKG = 2392, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 2, CapacityKG = 1196, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 2, CapacityKG = 1012, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 2, CapacityKG = 1012, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 2, CapacityKG = 1000, GradedKG = 0 });

                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 3, CapacityKG = 4600, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 3, CapacityKG = 2392, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 3, CapacityKG = 1196, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 3, CapacityKG = 1012, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 3, CapacityKG = 1012, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 3, CapacityKG = 1000, GradedKG = 0 });  

                ////For testing 30,000
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 1, CapacityKG = 15600, GradedKG = 0 });//4Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 1, CapacityKG = 8112, GradedKG = 0 });//12Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 1, CapacityKG = 4056, GradedKG = 0 });//16Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 1, CapacityKG = 3432, GradedKG = 0 });//30Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 1, CapacityKG = 3432, GradedKG = 0 });//30/40Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 1, CapacityKG = 3000, GradedKG = 0 });//regrind

                ////For testing 27,600
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 2, CapacityKG = 13800, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 2, CapacityKG = 7176, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 2, CapacityKG = 3588, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 2, CapacityKG = 3036, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 2, CapacityKG = 3036, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 2, CapacityKG = 3000, GradedKG = 0 });

                ////For testing 27,600
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 3, CapacityKG = 13800, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 3, CapacityKG = 7176, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 3, CapacityKG = 3588, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 3, CapacityKG = 3036, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 3, CapacityKG = 3036, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 3, CapacityKG = 3000, GradedKG = 0 });

                ////For testing 12,000
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 1, CapacityKG = 6000, GradedKG = 0 });//4Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 1, CapacityKG = 3120, GradedKG = 0 });//12Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 1, CapacityKG = 1560, GradedKG = 0 });//16Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 1, CapacityKG = 1320, GradedKG = 0 });//30Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 1, CapacityKG = 1320, GradedKG = 0 });//30/40Mesh
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 1, CapacityKG = 1300, GradedKG = 0 });//regrind

                ////For testing 9,600
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 2, CapacityKG = 4800, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 2, CapacityKG = 2496, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 2, CapacityKG = 1248, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 2, CapacityKG = 1056, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 2, CapacityKG = 1056, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 2, CapacityKG = 1000, GradedKG = 0 });

                ////For testing 9,600
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 1, Shift = 3, CapacityKG = 4800, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 2, Shift = 3, CapacityKG = 2496, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 3, Shift = 3, CapacityKG = 1248, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 4, Shift = 3, CapacityKG = 1056, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 5, Shift = 3, CapacityKG = 1056, GradedKG = 0 });
                //NewCurrentCapacityList.Add(new ProductCapacity() { ProductionTimeTableID = 0, RubberGradingID = 6, Shift = 3, CapacityKG = 1000, GradedKG = 0 });
            }
        }
    }
}
