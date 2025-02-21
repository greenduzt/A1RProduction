using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Formula;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Grading;
using A1QSystem.Model.Products;
using A1QSystem.Model.RawMaterials;
using A1QSystem.Model.Shifts;
using A1QSystem.Model.Stock;
using A1QSystem.ViewModel.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{
    public class SlitPeelCapacityAllocation
    {
        //public BindingList<RawMaterialDetails> RawMaterialDetails;
        private DateTime CurrentDate;
        private TimeSpan CurrentTime { get; set; }
        //private List<SlitPeel> CurrentSlitPeelDetails;
        private List<ProductionTimeTable> ProdTimeTableDetails;
        private List<Machines> MachinesList;
        private int RawProductID;
        private int MachineID;
        private int SalesID;


        public SlitPeelCapacityAllocation(int salesId, List<Machines> machinesList)
        {                      

            SalesID = salesId;
            MachinesList = machinesList;
            CurrentDate = Convert.ToDateTime(NTPServer.GetNetworkTime().ToString("dd/MM/yyyy"));


            //foreach (var item in currentSlitPeelDetails)
            //{
            //    //foreach (var items in item.GradingProduction)
            //    //{
            //    //    RawProductID = items.Product.RawProductID;
            //    //}
            //    RawProductID = item.RawProductID;
                
            //}

            //List<RawProductMachine> rawProductMachine = DBAccess.GetMachineIdByRawProdId(RawProductID);
            //foreach (var item in rawProductMachine)
            //{
            //    MachineID = item.SlitPeelMachineID;
            //}           

            ////Convert the date in to ProdTimeTableId
            //ProdTimeTableDetails = DBAccess.GetProductionTimeTableDetails(MachineID, CurrentDate);

           
            //CurrentTime = DateTime.Now.TimeOfDay;

            //List<Shift> ShiftDetails = DBAccess.GetAllShifts();

            //foreach (var item in ShiftDetails)
            //{
            //    bool isShift = TimeBetween(DateTime.Now, item.StartTime, item.EndTime);

            //    if (isShift == true)
            //    {
            //        currShift = item.ShiftID;
            //    }
            //}

            //AddDollarValue(currentSlitPeelDetails);
            //List<DateShift> d = GetShift(CurrentDate);
        }



        public int AddDollarValue(List<SlitPeel> currentSlitPeelDetails)
        {
            int res = 0;
            DateTime fDate = CurrentDate;
            List<SlitPeel> SlitPeelList = new List<SlitPeel>();
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            DateTime lDate = CurrentDate;

            //if (currentSlitPeelDetails != null)
            //{
            //    foreach (var item in currentSlitPeelDetails)
            //    {
            //        decimal qtyCounter = 0;
            //        decimal qtyComp = 0;
            //        RawProductID = item.Product.RawProductID;
            //        lDate = fDate;

            //        List<RawProductMachine> rawProductMachine = DBAccess.GetMachineIdByRawProdId(RawProductID);
            //        foreach (var itemRPM in rawProductMachine)
            //        {
            //            MachineID = itemRPM.SlitPeelMachineID;
            //        }
            //        if (MachineID != 0)
            //        {
            //            while (item.DollarValue != 0)
            //            {
            //                //Convert the date in to ProdTimeTableId
            //                ProdTimeTableDetails = DBAccess.GetProductionTimeTableDetails(MachineID, CurrentDate);
            //                if (ProdTimeTableDetails.Count != 0)
            //                {
            //                    List<DateShift> ShiftDetails = GetShift(lDate);

            //                    foreach (var itemSD in ShiftDetails)
            //                    {
            //                        foreach (var itemSL in itemSD.ShiftList)
            //                        {
            //                            if (item.DollarValue > 0)
            //                            {
            //                                decimal curDollarVal = DBAccess.GetCurrentShiftDollarValue(itemSL.ProTimeTableID, itemSL.shift);
            //                                decimal maxCap = DBAccess.GetSlitPeelMaxCapacity(itemSL.ProTimeTableID, itemSL.shift);
            //                                decimal newMaxCap = decimal.Floor((maxCap / item.Product.ProductPrice)) * item.Product.ProductPrice;
            //                                decimal avaFreeSpace = newMaxCap - curDollarVal;
            //                                decimal avaFreeBL = GetQty(GetQty(avaFreeSpace, item.Product.ProductPrice, item.Product.ProductUnit), item.Product.MaxItemsPer, item.Product.ProductUnit);
            //                                decimal extra = 0;
            //                                decimal blkLogs = 0;
            //                                decimal x = 0;
            //                                decimal d = 0;

            //                                if (avaFreeSpace >= item.DollarValue && avaFreeBL > 0)
            //                                {
            //                                    //SlitPeel sp = new SlitPeel();
            //                                    decimal qtyToMake = GetQty(item.DollarValue, item.Product.ProductPrice, item.Product.ProductUnit);

            //                                    qtyCounter += qtyToMake;
            //                                    if (qtyCounter > item.OriginalQty)
            //                                    {
            //                                        extra = qtyCounter - item.OriginalQty;//Extra needs to be added to the stock
            //                                        x = qtyToMake - extra;
            //                                        d = x * item.Product.ProductPrice;
            //                                    }
            //                                    else
            //                                    {
            //                                        x = qtyToMake;
            //                                        d = item.DollarValue;
            //                                    }
            //                                    qtyComp += x;
            //                                    blkLogs = GetBlockLog(x, item.Product.MaxItemsPer, item.Product.ProductUnit);
            //                                    item.DollarValue = 0;
            //                                }
            //                                else if (avaFreeSpace < item.DollarValue && avaFreeBL > 0)
            //                                {
            //                                    item.DollarValue -= avaFreeSpace;
            //                                    //SlitPeel sp = new SlitPeel();
            //                                    decimal qtyToMake = GetQty(avaFreeSpace, item.Product.ProductPrice, item.Product.ProductUnit);
            //                                    qtyCounter += qtyToMake;
            //                                    if (qtyCounter > item.OriginalQty)
            //                                    {
            //                                        extra = qtyCounter - item.OriginalQty;//Extra needs to be added to the stock
            //                                        x = qtyToMake - extra;
            //                                        d = x * item.Product.ProductPrice;
            //                                    }
            //                                    else
            //                                    {
            //                                        x = GetQty(avaFreeSpace, item.Product.ProductPrice, item.Product.ProductUnit);
            //                                        d = avaFreeSpace;
            //                                    }

            //                                    qtyComp += x;
            //                                    blkLogs = GetBlockLog(x, item.Product.MaxItemsPer, item.Product.ProductUnit);
                                            
            //                                }
            //                                if(x > 0 && d > 0)
            //                                {
            //                                    SlitPeel sp = new SlitPeel();
            //                                    sp.ProdTimeTableID = itemSL.ProTimeTableID;
            //                                    sp.SalesOrderID = SalesID;
            //                                    sp.ProductID = item.Product.ProductID;
            //                                    sp.RawProductID = item.Product.RawProductID;
            //                                    sp.QtyToMake = x;
            //                                    sp.DollarValue = d;
            //                                    sp.OriginalBlockLogs = blkLogs;
            //                                    sp.Shift = itemSL.shift;
            //                                    sp.Type = item.Type;
            //                                    sp.OrdertypeID = item.OrdertypeID;
            //                                    SlitPeelList.Add(sp);
            //                                    fDate = lDate;//Next Starting date
            //                                }
            //                            }
            //                        }
            //                    }
            //                    lDate = bdg.AddBusinessDays(lDate, 1);
            //                }
            //                else
            //                {
            //                    bool r = AddNewDates(lDate);//Create a new date
            //                }
            //            }
            //        }                          
            //    }

            //    if (SlitPeelList.Count != 0) //Add to DB
            //    {
            //        res = DBAccess.AddToSlitPeel(SlitPeelList);
            //        if (res == 1)
            //        {
            //            SlitPeelList.Clear();
            //        }
            //    }            
            //}
            return res;          
        }

        private List<DateShift> GetShift(DateTime lDate)
        {
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            List<DateShift> dateShift = new List<DateShift>();
            List<ShiftDetails> subShifts = null;
            int shift = 0;

            if (ProdTimeTableDetails.Count != 0)
            {
                foreach (var item in ProdTimeTableDetails)
                {
                    if (item.ProductionDate == lDate)
                    {
                        subShifts = new List<ShiftDetails>();
                        if (item.IsMachineActive == true)
                        {
                            shift = 0;
                            if (item.IsDayShiftActive == true)
                            {
                                shift = 1;
                                subShifts.Add(new ShiftDetails() { ProTimeTableID = item.ID,shift =shift });
                            }
                            if (item.IsEveningShiftActive == true)
                            {
                                shift = 2;
                                subShifts.Add(new ShiftDetails() { ProTimeTableID = item.ID, shift = shift });
                            }
                            if (item.IsNightShiftActive == true)
                            {
                                shift = 3;
                                subShifts.Add(new ShiftDetails() { ProTimeTableID = item.ID, shift = shift });
                            }

                            if (item.IsDayShiftActive == false && item.IsEveningShiftActive == false && item.IsNightShiftActive == false)
                            {
                                Console.WriteLine("SLIT PEEL shifts not available - " + lDate);
                                lDate = bdg.AddBusinessDays(lDate, 1);
                                dateShift = GetShift(lDate);
                            }
                            

                            if (shift != 0)
                            {
                                DateShift ds = new DateShift();
                                ds.ProdDate = lDate;
                                ds.ShiftList = subShifts;
                                dateShift.Add(ds);
                            }
                            else
                            {
                                Console.WriteLine("No shifts found - " + lDate);
                                lDate = bdg.AddBusinessDays(lDate, 1);
                                dateShift = GetShift(lDate);
                            }
                        }
                        else
                        {
                            Console.WriteLine("SLIT PEEL Machine is switched off - " + lDate);
                            lDate = bdg.AddBusinessDays(lDate, 1);
                            dateShift = GetShift(lDate);
                        }
                    }

                }
            }

            return dateShift;
        }

        public bool CheckDateAvailable(DateTime cDate)
        {
            bool res = false;
            Production prod = new Production();
            List<ProductionTimeTable> ptt = DBAccess.GetProductionTimeTableByID(1, cDate);//Get Production TimeTable
            if (ptt.Count == 0)
            {
                res = prod.AddNewDates(cDate, false);
            }
            else
            {
                res = true;
                Console.WriteLine("date available in the current list");
            }
            return res;
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

        private decimal GetQty(decimal dolVal, decimal prodPrice, string prodUnit)
        {
            decimal amount =0;
            amount = dolVal / prodPrice;

            
            if(prodUnit == "EA" || prodUnit == "TILE")
            {
                amount = decimal.Floor(amount);
                
            }

            return amount;
        }

        private decimal GetBlockLog(decimal dolVal, decimal prodPrice, string prodUnit)
        {
            decimal amount = 0;
            amount = dolVal / prodPrice;


            if (prodUnit == "EA" || prodUnit == "TILE")
            {
                amount = Math.Ceiling(amount);
                if (amount == 0)
                {
                    amount = 1;
                }
            }

            return amount;
        }


      


    }
}
