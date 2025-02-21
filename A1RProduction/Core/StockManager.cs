using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.DeliveryDetails;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Products;
using A1QSystem.Model.Stock;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{
    public class StockManager
    {
        private List<ProductMeterage> prodMeterageList;

        public Tuple<Order, Order> CheckBlockLogStock(Order order)
        {
            Tuple<Order, Order> splitOrder = null;
            prodMeterageList = new List<ProductMeterage>();

            Order prodOrder = new Order();
            Order slitPeelOrder = new Order();

            prodOrder = CopyOrder(order);
            slitPeelOrder = CopyOrder(order);
            prodOrder.OrderDetails = new ObservableCollection<OrderDetails>();
            slitPeelOrder.OrderDetails = new ObservableCollection<OrderDetails>();

            //prodOrder.OrderDetails.Clear();
            //slitPeelOrder.OrderDetails.Clear();
            prodMeterageList = DBAccess.GetProductMeterage();

            
                if (prodMeterageList.Count > 0 || prodMeterageList != null)
                {
                    foreach (var itemOD in order.OrderDetails)
                    {
                        if (itemOD.BlocksLogsToMake > 0)
                        {
                            PendingSlitPeel psp = new PendingSlitPeel() { Product = itemOD.Product};
                            RawStock rawStock = DBAccess.GetBlockLogStockByID(psp);

                            if (itemOD.Product.RawProduct.RawProductID == rawStock.RawProductID)
                            {
                                decimal toMakeBL = 0;
                                decimal toSlitPeelBL = 0;
                                decimal qtyToDeductBL = 0;
                                decimal toMakeQty = 0;
                                decimal toSlitPeelQty = 0;
                                OrderDetails odProd = new OrderDetails();
                                OrderDetails odSlitPeel = new OrderDetails();

                                if (order.OrderPriority == 1)
                                {
                                    //Check block/log stock and slit/peel or produce
                                    if (itemOD.Product.Type != "Block" || itemOD.Product.Type != "Log" || itemOD.Product.Type != "Box")
                                    {
                                        //Block/log checking
                                        if (itemOD.BlocksLogsToMake <= rawStock.Qty && rawStock.Qty > 0)//Full stock available
                                        {
                                            toMakeBL = 0;
                                            toSlitPeelBL = itemOD.BlocksLogsToMake;
                                            qtyToDeductBL = itemOD.BlocksLogsToMake;
                                            toMakeQty = 0;
                                            toSlitPeelQty = itemOD.Quantity;
                                        }
                                        else if (itemOD.BlocksLogsToMake > rawStock.Qty && rawStock.Qty > 0)//Partial stock available
                                        {
                                            toMakeBL = itemOD.BlocksLogsToMake - rawStock.Qty;
                                            toSlitPeelBL = rawStock.Qty;
                                            qtyToDeductBL = rawStock.Qty;
                                            toSlitPeelQty = CalculateQty(itemOD.Product, toSlitPeelBL);
                                            toMakeQty = itemOD.Quantity - toSlitPeelQty;
                                        }
                                        else if (rawStock.Qty <= 0)
                                        {
                                            toMakeBL = itemOD.BlocksLogsToMake;//No stock available
                                            toSlitPeelBL = 0;
                                            qtyToDeductBL = 0;
                                            toMakeQty = itemOD.Quantity;
                                            toSlitPeelQty = 0;
                                        }

                                        //Production
                                        if (toMakeBL > 0)
                                        {
                                            odProd.Product = itemOD.Product;
                                            odProd.Quantity = toMakeQty;
                                            odProd.BlocksLogsToMake = toMakeBL;
                                            prodOrder.OrderDetails.Add(odProd);
                                        }                                        

                                        //Deduct from block/log stock and PendingSlitPeel
                                        if (qtyToDeductBL > 0 && toSlitPeelBL > 0)
                                        {
                                            //For Slitting and Peeling
                                            odSlitPeel.Product = itemOD.Product;
                                            odSlitPeel.Quantity = toSlitPeelQty;
                                            odSlitPeel.BlocksLogsToMake = toSlitPeelBL;
                                            odSlitPeel.OrderNo = itemOD.OrderNo;
                                            slitPeelOrder.OrderDetails.Add(odSlitPeel);

                                            //For Block/Log Stock deduction
                                            RawStock rs = new RawStock();
                                            rs.RawProductID = itemOD.Product.RawProduct.RawProductID;
                                            rs.Qty = (rawStock.Qty - qtyToDeductBL) < 0 ? 0 : (rawStock.Qty - qtyToDeductBL);

                                            //For PendingSlitpleel deduction
                                            PendingSlitPeel ps = new PendingSlitPeel();
                                            ps.Order = order;
                                            ps.Product = itemOD.Product;
                                            ps.Qty = toSlitPeelQty;
                                            ps.BlockLogQty = toSlitPeelBL;

                                            //Update Block/Log Stock and PendingSlitPeel
                                            int res = DBAccess.UpdateBlockLogPendingSlitPeel(rs, ps);
                                            Console.WriteLine(res > 0 ? "Block/Log and SlitPeel Updated" : "Block/Log and SlitPeel Failed");
                                        }
                                    }
                                }
                                else if (order.OrderPriority == 2)
                                {
                                    //No block/log stock checking. Add to production and no slit/peel
                                    //Production
                                    odProd.Product = itemOD.Product;
                                    odProd.Quantity = itemOD.Quantity;
                                    odProd.BlocksLogsToMake = itemOD.BlocksLogsToMake;
                                    odProd.OrderNo = itemOD.OrderNo;
                                    odProd.MixingDate = itemOD.MixingDate;
                                    odProd.MixingComment = itemOD.MixingComment;
                                    prodOrder.OrderDetails.Add(odProd);
                                }
                            }                            
                        }
                    }
                }
                else
                {
                    //Msg.Show("Cannot load the product meterage data ", "Failed To Load Product Meterage", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                }
            

            splitOrder = new Tuple<Order, Order>(prodOrder, slitPeelOrder);

            return splitOrder;
        }

        private Order CopyOrder(Order order)
        {
            Order o = new Order();
            o.OrderNo = order.OrderNo;
            o.OrderType = order.OrderType;
            o.OrderPriority = order.OrderPriority;
            o.RequiredDate = order.RequiredDate;
            o.SalesNo = order.SalesNo;
            o.Comments = order.Comments;
            o.DeliveryDetails = new List<Delivery>() { new Delivery() { FreightID = order.DeliveryDetails[0].FreightID } };
            o.IsRequiredDateSelected = order.IsRequiredDateSelected;
            o.OrderCreatedDate = order.OrderCreatedDate;
            o.Customer = new Customer() { CustomerId = order.Customer.CustomerId };

            return o;
        }

        private decimal CalculateQty(Product product, decimal blockLog)
        {
            decimal qty = 0;

            if (product.Type == "Tile")
            {
                qty = Math.Floor(blockLog * product.Tile.MaxYield);                   
            }
            else if (product.Type == "Bulk" || product.Type == "Box" || product.Type == "BoxPallet")
            {
                qty = blockLog;
            }
            else if (product.Type == "Roll" || product.Type == "Standard")
            {                
                if (prodMeterageList.Count > 0)
                {
                    var data = prodMeterageList.Single(c => c.Thickness == product.Tile.Thickness && c.MouldType == product.MouldType && c.MouldSize == product.Width);
                    decimal maxRollsPerLog = Math.Floor(data.ExpectedYield / product.Tile.MaxYield);
                    qty = maxRollsPerLog * blockLog;
                }
            }
            else if (product.Type == "Block" || product.Type == "Log" || product.Type == "Curvedge")
            {
                qty = blockLog;     
            }
            else if (product.Type == "Custom")//TODO
            {

            }

            return qty;
        }


        private List<RawStock> LoadRawStock()
        {            
            return DBAccess.GetAllBlockLogStock();            
        }
    }
}
