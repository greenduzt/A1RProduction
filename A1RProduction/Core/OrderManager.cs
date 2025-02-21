using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Grading;
using A1QSystem.Model.Production.Mixing;
using A1QSystem.Model.Production.Peeling;
using A1QSystem.Model.Production.Slitting;
using A1QSystem.Model.Products;
using A1QSystem.Model.RawMaterials;
using A1QSystem.Model.Stock;
using A1QSystem.ViewModel.Orders;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace A1QSystem.Core
{
    public class OrderManager
    {
        private AddProductionOrderViewModel addProductionOrderViewModel;

        public OrderManager(AddProductionOrderViewModel apovm)
        {
            addProductionOrderViewModel = apovm;
        }

        public OrderManager()
        {
            //addProductionOrderViewModel = apovm;
        }

        public int ProcessOrder(Order order)
        {
            int res = 0;
            Int32 orderID =0;
            Tuple<List<GradingOrder>, List<MixingOrder>> elem = null;
            Tuple<Order, Order> splitOrder = null;          
           
            //Create OrderID
            //orderID = DBAccess.GenerateNewOrderID();
            //if (orderID > 0)
            //{
                //Assign the order id
                //order.OrderNo = orderID;

                //Convert QTY to Blocks and Logs
                order = CalculateBlocksLogs(order);

                //List<Curing> toCuringList = ProcessCuringList(order);

                //if (toCuringList.Count > 0)
                //{
                    //Save to Orders/OrderDetails/PendingOrders/PendingSlitPeel/Curing
                    int resOP = DBAccess.AddToOrders(order);
                    if (resOP == 1)
                    {
                        //Check Blocks/Logs Stock
                        StockManager sm = new StockManager();
                        splitOrder = sm.CheckBlockLogStock(order);

                        //Add to Status tables
                        //int statusRes = DBAccess.AddToStatus(splitOrder, order);

                        //Production
                        if (splitOrder.Item1.OrderDetails.Count > 0)
                        {
                            //Seperate Mixing and Grading
                            elem = SeperateMixingAndGrading(splitOrder.Item1);
                            if (elem.Item1.Count > 0)
                            {
                                //Add to Grading
                                int r1 = ProductionManager.AddToGrading(elem.Item1);
                                res = r1 > 0 ? 10 : 0;
                            }
                            if (elem.Item2.Count > 0)
                            {
                                //Add to Mixing
                                GradingProductionDetails gpd = null;
                                List<GradingCompleted> gcl = null;
                                int r2 = ProductionManager.AddToMixing(elem.Item2, gpd, 0, 0, 0, gcl,"", 0, new DateTime(), true);
                                res = r2 > 0 ? 10 : 0;
                            }                           
                        }
                        //Slitting/Peeling
                        if (splitOrder.Item2.OrderDetails.Count > 0)
                        {
                            List<SlittingOrder> slittingOrderList = new List<SlittingOrder>();
                            List<PeelingOrder> peelingOrderList = new List<PeelingOrder>();

                            foreach (var item in splitOrder.Item2.OrderDetails)
                            {
                                //Decides whether to go to Slitting or Peeling
                                if (item.Product.Type == "Tile")
                                {
                                    SlittingOrder so = new SlittingOrder();
                                    so.Order = splitOrder.Item2;
                                    so.Product = item.Product;
                                    so.Qty = item.Quantity;
                                    so.Blocks = item.BlocksLogsToMake;
                                    so.DollarValue = item.Quantity * item.Product.UnitPrice;
                                    slittingOrderList.Add(so);
                                }
                                else if (item.Product.Type == "Bulk")
                                {
                                    PeelingOrder po = new PeelingOrder();
                                    po.Order = splitOrder.Item2;
                                    po.Product = item.Product;
                                    po.Logs = item.BlocksLogsToMake;
                                    po.DollarValue = item.Quantity * item.Product.UnitPrice;
                                    po.IsReRollingReq = false;
                                    peelingOrderList.Add(po);
                                }
                                else if (item.Product.Type == "Roll" || item.Product.Type == "Standard")
                                {
                                    PeelingOrder po = new PeelingOrder();
                                    po.Order = splitOrder.Item2;
                                    po.Product = item.Product;
                                    po.Logs = item.BlocksLogsToMake;
                                    po.DollarValue = item.Quantity * item.Product.UnitPrice;
                                    po.IsReRollingReq = true;
                                    peelingOrderList.Add(po);
                                }
                            }

                            if (slittingOrderList.Count > 0)
                            {
                                //Add to slitting
                                int r3 = ProductionManager.AddToSlitting(slittingOrderList);
                                res = r3 > 0 ? 10 : 0;
                            }

                            if (peelingOrderList.Count > 0)
                            {
                                //Add to peeling
                                int r4 = ProductionManager.AddToPeeling(peelingOrderList);
                                res = r4 > 0 ? 10 : 0;
                            }
                        }                       
                    }
                    else
                    {
                        res = 5;
                    }
                //}
                //else
                //{
                //    res = 6;
                //}
            //}
            //else
            //{
            //    res = 3;
            //}           
            return res;
        }



        public Order CalculateBlocksLogs(Order order)
        {           
            foreach (var item in order.OrderDetails)
            {             
              
                if(item.Product.Type == "Tile")
                {
                    item.Quantity = item.Quantity;
                    item.BlocksLogsToMake = Math.Ceiling(item.Quantity / item.Product.Tile.MaxYield);
                }
                else if (item.Product.Type == "Bulk" )
                {
                    item.Quantity = item.Quantity;
                    item.BlocksLogsToMake = Math.Ceiling(item.Quantity);
                }              
                else if (item.Product.Type == "Roll" || item.Product.Type == "Standard")
                {
                    ProductMeterage pm = new ProductMeterage();
                    pm.Thickness = item.Product.Tile.Thickness;
                    pm.MouldType = item.Product.MouldType;
                    pm.MouldSize = item.Product.Width;
                    List<ProductMeterage> productMeterageList = DBAccess.GetProductMeterageByValues(pm);
                    if(productMeterageList.Count > 0)
                    {
                        decimal maxRollsPerLog = Math.Floor(productMeterageList[0].ExpectedYield / item.Product.Tile.MaxYield);
                        decimal noOfLogsReq = Math.Ceiling(item.Quantity / maxRollsPerLog);
                        item.Quantity = item.Quantity;
                        item.BlocksLogsToMake = noOfLogsReq;
                    }
                }
                else if (item.Product.Type == "Block" || item.Product.Type == "Log" || item.Product.Type == "Curvedge" || item.Product.Type == "Box" || 
                    item.Product.Type == "BoxPallet" || item.Product.Type == "Pallet" || item.Product.Type == "Playform")
                {
                    item.Quantity = item.Quantity;
                    item.BlocksLogsToMake = item.Quantity;
                }
                else if (item.Product.Type == "Custom")//TODO
                {

                }                
            }         
            return order;
        }

        private List<Curing> ProcessCuringList(Order order)
        {
            List<Curing> toCuringList = new List<Curing>();

            foreach (var item in order.OrderDetails)
            {
                if (item.Product.Type != "Box" && item.Product.Type != "BoxPallet" && item.Product.Type != "Kg" && item.Product.Type != "Pallet")
                {

                    for (int i = 0; i < item.BlocksLogsToMake; i++)
                    {
                        toCuringList.Add(new Curing() { Product = new Product() { ProductID = item.Product.ProductID, Type = item.Product.Type, RawProduct = new RawProduct() { RawProductID = item.Product.RawProduct.RawProductID } }, OrderNo = order.OrderNo, Qty = 1, StartTime = new DateTime(2000, 01, 01), EndTime = new DateTime(2000, 01, 01), IsCured = false, IsEnabled = false });
                    }
                }
            }

            return toCuringList;
        }

        private Tuple<List<GradingOrder>, List<MixingOrder>> SeperateMixingAndGrading(Order order)
        {
            List<MixingOrder> mixingOrderList = new List<MixingOrder>();
            List<GradingOrder> gradingOrderList = new List<GradingOrder>();
            List<MixingOnly> mixingOnlyList = new List<MixingOnly>();
            mixingOnlyList = DBAccess.GetMixingOnly();

            if (mixingOnlyList != null)
            {
                foreach (var item in order.OrderDetails)
                {
                    if (mixingOnlyList.Select(x => x.RawProductID).Contains(item.Product.RawProduct.RawProductID))
                    {
                        MixingOrder mixingOrder = new MixingOrder()
                        {
                            MixingTimeTableID = 0,
                            Product = item.Product,
                            Shift = 0,
                            BlocksLogs = item.BlocksLogsToMake,
                            Qty = item.Quantity,
                            Order = new Order()
                            {
                                OrderNo = item.OrderNo,
                                RequiredDate = item.MixingDate,
                                MixingDate = item.MixingDate,
                                OrderType = order.OrderType,
                                SalesNo = order.SalesNo,
                                Comments = order.Comments,
                                Customer = order.Customer
                            }
                        };
                        mixingOrderList.Add(mixingOrder);
                    }
                    else
                    {
                        //Get grading date
                        DateTime GradingDate = item.MixingDate;
                        if (item.MixingDate.Date == DateTime.Now.Date)
                        {
                            GradingDate = item.MixingDate;
                        }
                        else
                        {
                            //BusinessDaysGenerator bdg = new BusinessDaysGenerator();
                            //GradingDate = item.MixingDate.AddDays(-1);
                            //GradingDate = bdg.SkipWeekendsGetFriday(GradingDate);

                            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
                            GradingDate = item.MixingDate.AddDays(-1);
                            GradingDate = bdg.SkipSundayGetSaturday(GradingDate);
                        }

                        GradingOrder gradingOrder = new GradingOrder()
                        {
                            GradingTimeTableID = 0,
                            Product = item.Product,
                            Shift = 0,
                            BlocksLogs = item.BlocksLogsToMake,
                            Qty = item.Quantity,
                            Order = new Order()
                            {
                                OrderNo = item.OrderNo,
                                RequiredDate = GradingDate,
                                MixingDate = item.MixingDate,
                                OrderType = order.OrderType,
                                SalesNo = order.SalesNo,
                                Comments = order.Comments,
                                Customer = order.Customer
                            }
                        };
                        gradingOrderList.Add(gradingOrder);
                    }
                }
            }
            else
            {
                Msg.Show("Cannot load mixing only list!!!", "Mixing Only List Failed To Load", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }

            Tuple<List<GradingOrder>, List<MixingOrder>> elem = new Tuple<List<GradingOrder>, List<MixingOrder>>(gradingOrderList, mixingOrderList);

            return elem;
           
        }
    }
}
