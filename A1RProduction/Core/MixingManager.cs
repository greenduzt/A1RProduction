using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Grading;
using A1QSystem.Model.Production.Mixing;
using A1QSystem.Model.Products;
using A1QSystem.ViewModel.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{
    public class MixingManager
    {
        private List<ProductionTimeTable> ProdTimeTableDetails;
        private List<Machines> MachinesList;
        private List<MixingOnly> mixingOnlyList;
        private DateTime reqDate;

        public MixingManager()
        {
            MachinesList = new List<Machines>();
            MachinesList = DBAccess.GetNumberOfMachines();
            reqDate = DateTime.Now.Date;
            mixingOnlyList = DBAccess.GetMixingOnly();
        }

        public int ProcessMixingOrder(List<MixingOrder> mixingOrders, GradingProductionDetails gradingProductionDetails, int orderType, int currShift, Int64 currentProdTimeTable,
            List<GradingCompleted> ggList, string pcName, Int32 realProdTimeTableID, DateTime realDateTime, bool gradingCompleted)
        {
            int result = 0;

            ProdTimeTableDetails = new List<ProductionTimeTable>();

            int machineId = 0;
            List<MixingCapacity> mixingCapacityList = new List<MixingCapacity>();
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            DateTime lDate = reqDate;
            DateTime fDate = reqDate;
            //CHECK DATE IS THERE in the produciton time table otherwise create it

            //DBAccess.GetMixingCapacityDetails

            foreach (var item in mixingOrders)
            {
                lDate = fDate;

                List<RawProductMachine> rawProductMachine = DBAccess.GetMachineIdByRawProdId(item.Product.RawProduct.RawProductID);
                foreach (var itemRPM in rawProductMachine)
                {
                    machineId = itemRPM.MixingMachineID;
                }

                if (machineId != 0)
                {

                    bool isMixingOnly = mixingOnlyList.Select(x => x.RawProductID).Contains(item.Product.RawProduct.RawProductID);

                    if (isMixingOnly)
                    {
                        lDate = item.Order.MixingDate.Date;
                    }

                    while (item.BlocksLogs > 0)
                    {
                        //Convert the date in to ProdTimeTableId
                        ProdTimeTableDetails = DBAccess.GetProductionTimeTableDetails(machineId, lDate);

                        if (ProdTimeTableDetails.Count != 0)
                        {
                            List<DateShift> ShiftDetails = GetShift(lDate);
                            foreach (var itemSD in ShiftDetails)
                            {
                                foreach (var itemSL in itemSD.ShiftList)
                                {
                                    if (item.BlocksLogs > 0)
                                    {
                                        List<MixingCapacity> mixingCapList = DBAccess.GetMixingCapacity(itemSL.ProTimeTableID);//Mixing TimeTableID
                                        if (mixingCapList.Count != 0)
                                        {
                                            int prodTimeTableId = 0;
                                            decimal maxMix = 0;
                                            decimal curCap = 0;
                                            decimal avaCap = 0;
                                            decimal b = 0;

                                            foreach (var itemMCL in mixingCapList)
                                            {
                                                prodTimeTableId = itemMCL.ProdTimeTableID;
                                                maxMix = itemMCL.MaxMixes;
                                            }

                                            curCap = DBAccess.GetCurrentMixingCapacity(itemSL.ProTimeTableID);//Mixing TimeTableID
                                            avaCap = maxMix - curCap;
                                            if (avaCap >= item.BlocksLogs && item.BlocksLogs > 0)
                                            {
                                                b = item.BlocksLogs;
                                                item.BlocksLogs = 0;
                                            }
                                            else if (avaCap < item.BlocksLogs && avaCap > 0 && item.BlocksLogs > 0)
                                            {
                                                b = avaCap;
                                                item.BlocksLogs -= avaCap;
                                            }
                                            if (b > 0)
                                            {
                                                MixingCapacity mc = new MixingCapacity();
                                                mc.ProdTimeTableID = prodTimeTableId;
                                                mc.MixingTimeTableID = itemSL.ProTimeTableID;//Mixing TimeTableID
                                                mc.SalesId = item.Order.OrderNo;
                                                mc.RawProductID = item.Product.RawProduct.RawProductID;
                                                mc.BlockLogQty = b;
                                                mc.OrderType = item.Order.OrderType;
                                                mixingCapacityList.Add(mc);
                                                fDate = lDate;//Next Starting date
                                            }
                                        }
                                        else
                                        {
                                            //Create a new date
                                            bool res = CheckDateAvailable(lDate, machineId);
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            lDate = bdg.AddBusinessDays(lDate, 1);
                        }
                        else
                        {
                            //Create a new date
                            bool res = CheckDateAvailable(lDate, machineId);
                        }
                    }
                }

                if (mixingCapacityList.Count != 0)
                {
                    int x = DBAccess.AddToMixing(mixingCapacityList, gradingProductionDetails, orderType, currShift, currentProdTimeTable, ggList, pcName, realProdTimeTableID, realDateTime, gradingCompleted);
                    if (x != 0)
                    {
                        mixingCapacityList.Clear();
                    }
                    result = x;
                }
            }

            return result;
        }

        public List<DateShift> GetShift(DateTime lDate)
        {
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            List<DateShift> dateShift = new List<DateShift>();
            List<ShiftDetails> subShifts = null;

            if (ProdTimeTableDetails.Count != 0)
            {
                foreach (var item in ProdTimeTableDetails)
                {
                    if (dateShift.Count == 0)
                    {
                        if (item.ProductionDate == lDate)
                        {
                            subShifts = new List<ShiftDetails>();
                            if (item.IsMachineActive == true)
                            {
                                subShifts.Add(new ShiftDetails() { ProTimeTableID = item.ID });

                                DateShift ds = new DateShift();
                                ds.ProdDate = lDate;
                                ds.ShiftList = subShifts;
                                dateShift.Add(ds);
                            }
                            else
                            {
                                Console.WriteLine("Mixing Machine is switched off - " + lDate);
                                lDate = bdg.AddBusinessDays(lDate, 1);
                                dateShift = GetShift(lDate);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (dateShift.Count == 0)
                {
                    //Create a new date

                    lDate = bdg.AddBusinessDays(lDate, 1);
                    dateShift = GetShift(lDate);
                }
            }
            return dateShift;
        }

        public bool CheckDateAvailable(DateTime cDate, int machineId)
        {
            bool res = false;
            Production prod = new Production();
            List<ProductionTimeTable> ptt = DBAccess.GetProductionTimeTableByID(machineId, cDate);//Get Production TimeTable
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
    }
}
