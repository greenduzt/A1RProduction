using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Production.Peeling;
using A1QSystem.Model.Production.Slitting;
using A1QSystem.Model.Products;
using A1QSystem.Model.Products.Rolls;
using A1QSystem.Model.RawMaterials;
using A1QSystem.Model.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{   

    public class PendingOrderManager
    {
        private List<PendingSlitPeel> pendingSlitPeel;
        private Curing curing;
        
        public PendingOrderManager(Curing c)
        {
            curing = c;           
        }

        public PendingOrderManager()
        {

        }
     
        public int ProcessPendingOrders()
        {
            int res=0;
            pendingSlitPeel = new List<PendingSlitPeel>();

            pendingSlitPeel = DBAccess.GetPendingOrdersByID(curing);
            if (pendingSlitPeel.Count > 0)
            {
                //Check if it is available in SimilarBlockLogs
                int x = DBAccess.GetSimilarBlockLogOtherProduct(curing);
                if (x > 0)
                {
                    pendingSlitPeel = DBAccess.GetPendingOrdersByID(curing);

                }                
            }
            else
            {
                int x = DBAccess.GetSimilarBlockLogOtherProduct(curing);
                if (x > 0)
                {
                    pendingSlitPeel = DBAccess.GetPendingOrdersByID(curing);
                }  
            }

            res = UpdatekBlockLogStockAgainstOrders(pendingSlitPeel);
            //else
            //{
            //    //Check if it is available in SimilarBlockLogs
            //    int x = DBAccess.GetSimilarBlockLogOtherProduct(curing);
            //    pendingSlitPeel = DBAccess.GetPendingOrdersByID(curing);
            //    res = UpdatekBlockLogStockAgainstOrders(pendingSlitPeel);
            //}
           

            //pendingSlitPeel = DBAccess.GetPendingOrders(); 
            //if (pendingSlitPeel.Count > 0)
            //{
            //    res = UpdatekBlockLogStockAgainstOrders(pendingSlitPeel);
            //}
            return res;
        }

        public int ProcessPendingOrdersByMultipleProducts(Order order)
        {
            int res = 0;
            pendingSlitPeel = new List<PendingSlitPeel>();
            pendingSlitPeel = DBAccess.GetMultiplePendingOrders(order);
            if (pendingSlitPeel.Count > 0)
            {
                res = UpdatekBlockLogStockAgainstOrders(pendingSlitPeel);
            }
            return res;
        }

        private int UpdatekBlockLogStockAgainstOrders(List<PendingSlitPeel> pendingOrdersCuring)
        {
            int result = 0;
            List<SlittingOrder> slittingOrderList = new List<SlittingOrder>();
            List<PeelingOrder> peelingOrderList = new List<PeelingOrder>();
            foreach (var itemPOC in pendingOrdersCuring)
            {
                if (itemPOC.BlockLogQty > 0)
                {
                    //Check RawStock for blocks/logs
                    RawStock rawStock = DBAccess.GetBlockLogStockByID(itemPOC);
                    if (rawStock.Qty > 0)
                    {
                        if (rawStock.RawProductID == itemPOC.Product.RawProduct.RawProductID)
                        {
                            decimal blAva = 0;
                            decimal qtyAva = 0;

                            if (rawStock.Qty >= itemPOC.BlockLogQty)
                            {
                                //Convert to blockslogs
                                blAva = itemPOC.BlockLogQty;
                                qtyAva = ConvertToQty(itemPOC.Product, blAva);
                            }
                            else if (rawStock.Qty < itemPOC.BlockLogQty)
                            {
                                //Convert to blockslogs
                                blAva = rawStock.Qty;
                                qtyAva = ConvertToQty(itemPOC.Product, blAva);
                            }

                            //BlockLog Stock deduct
                            RawStock rs = new RawStock();
                            rs.RawProductID = itemPOC.Product.RawProduct.RawProductID;
                            rs.Qty = rawStock.Qty - blAva;//deduct from rawstock to add to PendingSlitPeel

                            //PendingSlitPeel deduct
                            PendingSlitPeel ps = new PendingSlitPeel();
                            ps.ID = itemPOC.ID;
                            ps.Order = itemPOC.Order;
                            ps.Product = itemPOC.Product;
                            ps.Qty = (itemPOC.Qty - qtyAva) < 0 ? 0 : (itemPOC.Qty - qtyAva);
                            ps.BlockLogQty = itemPOC.BlockLogQty - blAva;

                            //Decides whether to go to Slitting or Peeling
                            if (itemPOC.Product.Type == "Tile")
                            {
                                SlittingOrder so = new SlittingOrder();
                                so.Order = itemPOC.Order;
                                so.Product = itemPOC.Product;
                                so.Qty = qtyAva;
                                so.Blocks = blAva;
                                so.DollarValue = qtyAva * itemPOC.Product.UnitPrice;
                                slittingOrderList.Add(so);
                            }
                            else if (itemPOC.Product.Type == "Bulk")
                            {
                                PeelingOrder po = new PeelingOrder();
                                po.Order = itemPOC.Order;
                                po.Product = itemPOC.Product;
                                po.Logs = blAva;
                                po.DollarValue = qtyAva * itemPOC.Product.UnitPrice;
                                po.IsReRollingReq = false;
                                peelingOrderList.Add(po);
                            }                          
                            else if (itemPOC.Product.Type == "Roll" || itemPOC.Product.Type == "Standard")
                            {
                                PeelingOrder po = new PeelingOrder();
                                po.Order = itemPOC.Order;
                                po.Product = itemPOC.Product;
                                po.Logs = blAva;
                                po.DollarValue = qtyAva * itemPOC.Product.UnitPrice;
                                po.IsReRollingReq = true;
                                peelingOrderList.Add(po);
                            }
                            else if (itemPOC.Product.Type == "Custom")
                            {

                            }

                            if (blAva > 0 && qtyAva > 0)
                            {
                                //Update RawStock and PendingSlitPeel
                                int res = DBAccess.UpdateBlockLogStock(Tuple.Create(ps, rs));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Logs not available for product " + itemPOC.Product.ProductDescription);
                    }
                }
                else
                {
                    Console.WriteLine("No logs available in the PendingSlitPeel for " + itemPOC.Product.ProductDescription);
                }
            }
            if(slittingOrderList.Count > 0)
            {
                //Add to slitting
                ProductionManager.AddToSlitting(slittingOrderList);
                Console.WriteLine("Added to slitting");
                result = 1;
            }
            if(peelingOrderList.Count > 0)
            {
                //Add to peeling
                ProductionManager.AddToPeeling(peelingOrderList);
                Console.WriteLine("Added to peeling");
                result = 2;
            }
            return result;
        }
        private decimal ConvertToBlocksLogs(decimal qty,Product product)
        {
            decimal bl = 0;

            if (product.Type == "Tile")
            {
                bl = Math.Ceiling(qty / product.Tile.MaxYield);
            }
            else if (product.Type == "Bulk")
            {
                bl = qty;
            }
            else if (product.Type == "Standard")
            {
                BulkRoll bulkRoll = new BulkRoll();
                List<BulkRoll> peelingReRollingList = DBAccess.GetPeelingReRollingList();
                var reRollExist = peelingReRollingList.FirstOrDefault(x => (x.StandardRoll.ProductID == product.ProductID));
                bulkRoll = DBAccess.GetPeelingReRollingProducts(reRollExist);
                decimal bLPerBulkRoll = Math.Floor(bulkRoll.MaxYield / bulkRoll.StandardRoll.MaxYield);
                bl = Math.Ceiling(qty / bLPerBulkRoll);
            }
            else if (product.Type == "Custom")
            {

            }

            return bl;
        }


        private decimal ConvertToQty(Product product,decimal blcokslogs)
        {
            decimal qty = 0;


            if (product.Type == "Tile")
            {
                qty = Math.Floor(blcokslogs * product.Tile.MaxYield);
            }
            else if (product.Type == "Bulk")
            {
                qty = Math.Ceiling(blcokslogs);
            }
            //else if (product.Type == "Standard")
            //{
            //    BulkRoll bulkRoll = new BulkRoll();
            //    List<BulkRoll> peelingReRollingList = DBAccess.GetPeelingReRollingList();
            //    var reRollExist = peelingReRollingList.FirstOrDefault(x => (x.StandardRoll.ProductID == product.ProductID));
            //    bulkRoll = DBAccess.GetPeelingReRollingProducts(reRollExist);
            //    decimal bLPerBulkRoll = Math.Floor(bulkRoll.MaxYield / bulkRoll.StandardRoll.MaxYield);
            //    qty = Math.Ceiling(blcokslogs * bLPerBulkRoll);
            //}
            else if (product.Type == "Roll" || product.Type == "Standard")
            {
                 ProductMeterage pm = new ProductMeterage();
                 pm.Thickness = product.Tile.Thickness;
                 pm.MouldType = product.MouldType;
                 pm.MouldSize = product.Width;
                 List<ProductMeterage> productMeterageList = DBAccess.GetProductMeterageByValues(pm);
                 if (productMeterageList.Count > 0)
                 {
                     decimal maxYield = blcokslogs * productMeterageList[0].ExpectedYield;
                     qty = Math.Floor(maxYield / product.Tile.MaxYield);
                 }                
            }
            else if (product.Type == "Custom")//TODO
            {

            }

            return qty;
        }

        private decimal CalculateQtyByProduct(Product product, decimal blockslogs)
        {

            decimal qty = 0;

            if (product.Type == "Tile")
            {
                qty = Math.Ceiling(blockslogs * product.Tile.MaxYield);
            }
            else if (product.Type == "Bulk")
            {
                qty = Math.Ceiling(blockslogs);
            }
            else if (product.Type == "Standard")
            {
                BulkRoll bulkRoll = new BulkRoll();
                List<BulkRoll> peelingReRollingList = DBAccess.GetPeelingReRollingList();
                var reRollExist = peelingReRollingList.FirstOrDefault(x => (x.StandardRoll.ProductID == product.ProductID));
                bulkRoll = DBAccess.GetPeelingReRollingProducts(reRollExist);
                decimal bLPerBulkRoll = Math.Floor(bulkRoll.MaxYield / bulkRoll.StandardRoll.MaxYield);
                qty = Math.Ceiling(blockslogs * bLPerBulkRoll);
            }
            else if (product.Type == "Custom")//TODO
            {

            }

            return qty;
        }

        public void UpdatePendingOrder(Int32 orderNo,int rawProductId,int productId, decimal bl,decimal q)
        {
            //Get PendingOrder
            PendingOrder po = DBAccess.GetPendingOrder(orderNo, rawProductId, productId);

            if (po.BlockLogQty > 0)
            {
                po.BlockLogQty -= bl;
                po.Qty -= q;         

                DBAccess.UpdatePendingOrderDetails(po);
            }

            if (po.BlockLogQty <= 0)//Update status if zero
            {
                DBAccess.UpdatePendingOrder(orderNo, rawProductId, productId, "Completed");
            }
        }

    }
}
