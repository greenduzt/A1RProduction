using A1QSystem.DB;
using A1QSystem.Model.Production;
using A1QSystem.Model.Shifts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{
    public class ShiftIProcessor
    {

        private int CurrentProdTimeTableID { get; set; }
        private DateTime SelectedDate { get; set; }
        private DateTime CurrentDate { get; set; }
        private int CurentShift { get; set; }
        private int machineId;

        public ShiftIProcessor(DateTime sDate,int m)
        {
            CurrentDate = Convert.ToDateTime(NTPServer.GetNetworkTime().ToString("dd/MM/yyyy"));
            SelectedDate = sDate;
            machineId = m;
        }
        public int GetCurrentShift()
        {
            List<Shift> ShiftDetails = DBAccess.GetAllShifts();
            //Get the current shhift
            foreach (var item in ShiftDetails)
            {
                bool isShift = TimeBetween(DateTime.Now, item.StartTime, item.EndTime);

                if (isShift == true)
                {
                    CurentShift = item.ShiftID;
                }
            }

            return CurentShift;
        }
        public int GetCurrentProdTimeTableID()
        {
            List<ProductionTimeTable> pTimeTable = DBAccess.GetProductionTimeTableByID(machineId, CurrentDate);
            foreach (var item in pTimeTable)
            {
                if (item.MachineID == machineId)
                {
                    CurrentProdTimeTableID = item.ID;
                }
            }

            return CurrentProdTimeTableID;
        }



        public List<Shift> GetShiftDetails()
        {
            List<Shift> sList = new List<Shift>();
            List<ProductionTimeTable> prodTimeTable = DBAccess.GetProductionTimeTableByMachineID(machineId, SelectedDate);
            if (prodTimeTable.Count != 0)
            {
                foreach (var item in prodTimeTable)
                {
                    if (item.IsMachineActive == true)
                    {
                        sList.Add(new Shift() { ShiftID = 0, ShiftName = "Select" });
                        if (item.IsDayShiftActive == true)
                        {
                            if (CurrentProdTimeTableID == item.ID && CurentShift <= 1)
                            {
                                goto a;
                            }
                            else if (CurrentProdTimeTableID == item.ID && CurentShift > 1)
                            {
                                goto b;
                            }
                            else
                            {
                                goto a;
                            }
                        a: ;
                            sList.Add(new Shift() { ShiftID = 1, ShiftName = "Day" });
                        }
                    b: ;
                        if (item.IsEveningShiftActive == true)
                        {
                            if (CurrentProdTimeTableID == item.ID && CurentShift <= 2)
                            {
                                goto c;
                            }
                            else if (CurrentProdTimeTableID == item.ID && CurentShift > 2)
                            {
                                goto d;
                            }
                            else
                            {
                                goto c;
                            }
                        c: ;
                            sList.Add(new Shift() { ShiftID = 2, ShiftName = "Evening" });
                        }
                    d: ;
                        if (item.IsNightShiftActive == true)
                        {
                            if (CurrentProdTimeTableID == item.ID && CurentShift <= 3)
                            {
                                goto e;
                            }
                            else if (CurrentProdTimeTableID == item.ID && CurentShift > 3)
                            {
                                goto f;
                            }
                            else
                            {
                                goto e;
                            }
                        e: ;
                            sList.Add(new Shift() { ShiftID = 3, ShiftName = "Night" });
                        }
                    f: ;

                    }
                    else
                    {
                        sList.Add(new Shift() { ShiftID = 0, ShiftName = "Not Available" });
                    }
                }
            }

            if (sList.Count == 0)
            {
                sList.Add(new Shift() { ShiftID = 0, ShiftName = "Not Available" });
            }
            return sList;
        }

        bool TimeBetween(DateTime datetime, TimeSpan start, TimeSpan end)
        {
            // convert datetime to a TimeSpan
            TimeSpan now = datetime.TimeOfDay;
            // see if start comes before end
            if (start < end)
                return start <= now && now <= end;
            // start is after end, so do the inverse comparison
            return !(end < now && now < start);
        }
    }
}
