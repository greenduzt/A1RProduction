using A1QSystem.DB;
using A1QSystem.Model.Vehicles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{
    public class VehicleWorkOrderManager
    {
        public VehicleWorkOrderManager() { }

        public Tuple<int, List<Tuple<string, string>>> CreateVehicleWorkOrder(VehicleWorkOrder v, decimal Odometer)
        {
            int y = 0;
            int id = 0;
            List<Tuple<string,string>> tup = new List<Tuple<string,string>>();

            List<VehicleMaintenanceSequence> VehicleMaintenanceSequenceList = DBAccess.GetVehicleMaintenanceSequence(v.Vehicle.VehicleCategory.ID);
            ObservableCollection<VehicleWorkOrder> TopVehicleWorkOrders = DBAccess.GetTopVehicleWorkOrdersByVehicleID(v.Vehicle.ID);
            int count = VehicleMaintenanceSequenceList.Count;
            //Get last odometer raeding
            VehicleWorkOrder CompletedVehicleWorkOrder = DBAccess.GetLastCompletedWorkOrder(v.Vehicle.ID);
            bool found = false;

            foreach (var item in TopVehicleWorkOrders)
            {
                var d = TopVehicleWorkOrders.SingleOrDefault(x => x.VehicleMaintenanceSequence.ID == item.VehicleMaintenanceSequence.ID);
                decimal gap = Odometer - d.OdometerReading;
                for (int i = 0; i < VehicleMaintenanceSequenceList.Count; i++)
                {
                    if (d.VehicleMaintenanceSequence.ID == VehicleMaintenanceSequenceList[i].ID && !VehicleMaintenanceSequenceList[i].Unit.Equals("monthly") && found ==false)
                    {
                        if (d.VehicleMaintenanceSequence.ID == 35 || d.VehicleMaintenanceSequence.ID == 37)
                        {
                            y = i;
                        }
                        else
                        {
                            y = i + 1;
                        }

                        if (gap >= VehicleMaintenanceSequenceList[i].Kmhrs && gap < VehicleMaintenanceSequenceList[y].Kmhrs)
                        {
                            id = VehicleMaintenanceSequenceList[i].ID;
                            tup.Add(Tuple.Create("MaintenanceDesHeader", VehicleMaintenanceSequenceList[i].Kmhrs.ToString() == "500" ? "500Hr (Oil Change) Maintenance Description Schedule" : (VehicleMaintenanceSequenceList[i].Kmhrs == 0 ? "" : VehicleMaintenanceSequenceList[i].Kmhrs.ToString()) + "" + VehicleMaintenanceSequenceList[i].Unit.First().ToString().ToUpper() + VehicleMaintenanceSequenceList[i].Unit.Substring(1) + " Maintenance Description Schedule"));
                            tup.Add(Tuple.Create("frequency", VehicleMaintenanceSequenceList[i].Kmhrs.ToString()));
                            tup.Add(Tuple.Create("unit",VehicleMaintenanceSequenceList[i].Unit.First().ToString().ToUpper() + VehicleMaintenanceSequenceList[i].Unit.Substring(1)));

                            found = true;
                        }
                    }
                }
                if(found == true)
                {
                    break;
                }
            }

            //Since last odometer reading how many KM/HR has been driven
            decimal diff = Odometer - CompletedVehicleWorkOrder.OdometerReading;
            if (id == 0 && diff >= 0)
            {
                for (int i = 0; i < VehicleMaintenanceSequenceList.Count; i++)
                {
                    y = i + 1;

                    if (y != count)
                    {
                        if (diff >= VehicleMaintenanceSequenceList[i].Kmhrs && diff < VehicleMaintenanceSequenceList[y].Kmhrs)
                        {
                            id = VehicleMaintenanceSequenceList[i].ID;
                            tup.Add(Tuple.Create("MaintenanceDesHeader", VehicleMaintenanceSequenceList[i].Kmhrs.ToString() == "500" ? "500Hr (Oil Change) Maintenance Description Schedule" : (VehicleMaintenanceSequenceList[i].Kmhrs == 0 ? "" : VehicleMaintenanceSequenceList[i].Kmhrs.ToString()) + "" + VehicleMaintenanceSequenceList[i].Unit.First().ToString().ToUpper() + VehicleMaintenanceSequenceList[i].Unit.Substring(1) + " Maintenance Description Schedule"));
                            tup.Add(Tuple.Create("frequency", VehicleMaintenanceSequenceList[i].Kmhrs.ToString()));
                            tup.Add(Tuple.Create("unit", VehicleMaintenanceSequenceList[i].Unit.First().ToString().ToUpper() + VehicleMaintenanceSequenceList[i].Unit.Substring(1)));
                        }
                    }
                    else if (diff > VehicleMaintenanceSequenceList[i].Kmhrs)
                    {
                        id = VehicleMaintenanceSequenceList[i].ID;
                        tup.Add(Tuple.Create("MaintenanceDesHeader", VehicleMaintenanceSequenceList[i].Kmhrs.ToString() == "500" ? "500Hr (Oil Change) Maintenance Description Schedule" : (VehicleMaintenanceSequenceList[i].Kmhrs == 0 ? "" : VehicleMaintenanceSequenceList[i].Kmhrs.ToString()) + "" + VehicleMaintenanceSequenceList[i].Unit.First().ToString().ToUpper() + VehicleMaintenanceSequenceList[i].Unit.Substring(1) + " Maintenance Description Schedule"));
                        tup.Add(Tuple.Create("frequency", VehicleMaintenanceSequenceList[i].Kmhrs.ToString()));
                        tup.Add(Tuple.Create("unit", VehicleMaintenanceSequenceList[i].Unit.First().ToString().ToUpper() + VehicleMaintenanceSequenceList[i].Unit.Substring(1)));
                    }
                    else if (diff == 0 && VehicleMaintenanceSequenceList[i].Kmhrs == 0)
                    {
                        id = VehicleMaintenanceSequenceList[i].ID;
                        tup.Add(Tuple.Create("MaintenanceDesHeader", VehicleMaintenanceSequenceList[i].Kmhrs.ToString() == "500" ? "500Hr (Oil Change) Maintenance Description Schedule" : (VehicleMaintenanceSequenceList[i].Kmhrs == 0 ? "" : VehicleMaintenanceSequenceList[i].Kmhrs.ToString()) + "" + VehicleMaintenanceSequenceList[i].Unit.First().ToString().ToUpper() + VehicleMaintenanceSequenceList[i].Unit.Substring(1) + " Maintenance Description Schedule"));
                        tup.Add(Tuple.Create("frequency", VehicleMaintenanceSequenceList[i].Kmhrs.ToString()));
                        tup.Add(Tuple.Create("unit", VehicleMaintenanceSequenceList[i].Unit.First().ToString().ToUpper() + VehicleMaintenanceSequenceList[i].Unit.Substring(1)));
                    }
                    else if (diff >= VehicleMaintenanceSequenceList[i].Kmhrs)
                    {
                        id = VehicleMaintenanceSequenceList[i].ID;
                        tup.Add(Tuple.Create("MaintenanceDesHeader", VehicleMaintenanceSequenceList[i].Kmhrs.ToString() == "500" ? "500Hr (Oil Change) Maintenance Description Schedule" : (VehicleMaintenanceSequenceList[i].Kmhrs == 0 ? "" : VehicleMaintenanceSequenceList[i].Kmhrs.ToString()) + "" + VehicleMaintenanceSequenceList[i].Unit.First().ToString().ToUpper() + VehicleMaintenanceSequenceList[i].Unit.Substring(1) + " Maintenance Description Schedule"));
                        tup.Add(Tuple.Create("frequency", VehicleMaintenanceSequenceList[i].Kmhrs.ToString()));
                        tup.Add(Tuple.Create("unit", VehicleMaintenanceSequenceList[i].Unit.First().ToString().ToUpper() + VehicleMaintenanceSequenceList[i].Unit.Substring(1)));
                    }
                }
            }

            return Tuple.Create(id, tup);
        }
    }
}
