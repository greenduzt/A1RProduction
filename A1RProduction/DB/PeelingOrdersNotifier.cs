using A1QSystem.Model;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Peeling;
using A1QSystem.Model.Production.SlitingPeeling;
using A1QSystem.Model.Products;
using A1QSystem.Model.Products.Tiles;
using A1QSystem.Model.Shifts;
using A1QSystem.Model.Stock;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.DB
{
    public class PeelingOrdersNotifier : IDisposable
    {
        private event EventHandler<SqlNotificationEventArgs> _newMessage;
        private SqlConnection connection;

        public SqlCommand CurrentCommand { get; set; }

        public SqlConnection CurrentConnection
        {
            get
            {
                this.connection = this.connection ?? new SqlConnection(DBConfiguration.DbConnectionString);
                return this.connection;
            }
        }
        public PeelingOrdersNotifier()
        {
            SqlDependency.Start(DBConfiguration.DbConnectionString);

        }

        public event EventHandler<SqlNotificationEventArgs> NewMessage
        {
            add
            {
                this._newMessage += value;
            }
            remove
            {
                this._newMessage -= value;
            }
        }

        public virtual void OnNewMessage(SqlNotificationEventArgs notification)
        {
            if (this._newMessage != null)
                this._newMessage(this, notification);
        }

        public ObservableCollection<PeelingOrder> RegisterDependency()
        {


            this.CurrentCommand = new SqlCommand("SELECT Orders.order_id,Orders.sales_no,Orders.customer_id,Orders.required_date,Orders.peeling_comments, " +
                                                 "PeelingOrders.id AS slitting_id,PeelingOrders.prod_timetable_id,PeelingOrders.product_id,PeelingOrders.raw_product_id,PeelingOrders.qty,PeelingOrders.logs,PeelingOrders.dollar_value,PeelingOrders.order_type,PeelingOrders.status,PeelingOrders.is_re_rolling_req, " +
                                                 "Products.product_code,Products.product_name,Products.product_description,Products.Unit,Products.Thickness,Products.max_yield,Products.min_yield,Products.price,Products.density,Products.width,Products.height,Products.mould_type,Products.type, " +
	                                             "Customers.CompanyName, " +
	                                             "ProductionTimeTable.date, " +
	                                             "Machines.machine_id,Machines.machine_name, " +
                                                 "Shifts.shift_id,Shifts.shift_name, " +
                                                 "RawProducts.RawProductType " +
                                                 "FROM dbo.Orders " +
                                                 "INNER JOIN dbo.PeelingOrders ON Orders.order_id = PeelingOrders.order_no " +
                                                 "INNER JOIN dbo.Products ON PeelingOrders.product_id = Products.id " +
                                                 "INNER JOIN dbo.Customers ON Orders.customer_id = Customers.CustomerID " +
                                                 "INNER JOIN dbo.ProductionTimeTable ON PeelingOrders.prod_timetable_id = ProductionTimeTable.id " +
                                                 "INNER JOIN dbo.Machines ON ProductionTimeTable.machine_id = Machines.machine_id " +
                                                 "INNER JOIN dbo.Shifts ON PeelingOrders.shift = Shifts.shift_id " +
                                                 "INNER JOIN dbo.RawProducts ON Products.raw_product_id = RawProducts.RawProductID " +
                                                 "WHERE PeelingOrders.logs > 0 ", this.CurrentConnection);

            this.CurrentCommand.Notification = null;


            SqlDependency dependency = new SqlDependency(this.CurrentCommand);
            dependency.OnChange += this.dependency_OnChange;

            if (this.CurrentConnection.State == ConnectionState.Closed)
                this.CurrentConnection.Open();
            try
            {
                ObservableCollection<PeelingOrder> poeelingOrders = new ObservableCollection<PeelingOrder>();              

                using (SqlDataReader dr = this.CurrentCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {

                            PeelingOrder po = new PeelingOrder();
                            po.ID = Convert.ToInt32(dr["slitting_id"]);
                            po.ProdTimetableID = Convert.ToInt32(dr["prod_timetable_id"]);
                            po.Logs = Convert.ToDecimal(dr["logs"]);
                            po.Qty = Convert.ToDecimal(dr["qty"]);
                            po.DollarValue = Convert.ToDecimal(dr["dollar_value"]);
                            po.Shift = new Shift() { ShiftID = Convert.ToInt16(dr["shift_id"]), ShiftName = dr["shift_name"].ToString() };
                            po.Status = dr["status"].ToString();
                            po.PeelingDate = Convert.ToDateTime(dr["date"]);
                            po.IsReRollingReq = Convert.ToBoolean(dr["is_re_rolling_req"]);
                            po.Order = new Order()
                            {
                                OrderNo = Convert.ToInt32(dr["order_id"]),
                                SalesNo = dr["sales_no"].ToString(),
                                Comments = dr["peeling_comments"].ToString(),
                                OrderType = Convert.ToInt16(dr["order_type"]),
                                RequiredDate = Convert.ToDateTime(dr["required_date"]),
                                Customer = new Customer()
                                {
                                    CustomerId = Convert.ToInt16(dr["customer_id"]),
                                    CompanyName = dr["CompanyName"].ToString()
                                }
                            };
                            po.Product = new Product()
                            {
                                ProductID = Convert.ToInt16(dr["product_id"]),
                                ProductCode = dr["product_code"].ToString(),
                                ProductName = dr["product_name"].ToString(),
                                ProductDescription = dr["product_description"].ToString(),
                                ProductUnit = dr["Unit"].ToString(),
                                Tile = new Tile(){Thickness = Convert.ToDecimal(dr["Thickness"]),
                                MaxYield = Convert.ToDecimal(dr["max_yield"]),
                                MinYield = Convert.ToDecimal(dr["min_yield"]),Height = Convert.ToDecimal(dr["height"])},
                                
                                UnitPrice = Convert.ToDecimal(dr["price"]),
                                Density = dr["density"].ToString(),
                                Width = Convert.ToDecimal(dr["width"]),
                                
                                MouldType = dr["mould_type"].ToString(),
                                Type = dr["type"].ToString(),
                                RawProduct = new RawProduct()
                                {
                                    RawProductID = Convert.ToInt16(dr["raw_product_id"]),
                                    RawProductType = dr["RawProductType"].ToString()
                                }
                            };

                            poeelingOrders.Add(po);                                                        
                        }
                    }
                }

                return poeelingOrders;
            }
            catch { return null; }

        }

        void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            // DBAccess.RescheduleOrdersByDate(Convert.ToDateTime("21/08/2015"));

            SqlDependency dependency = sender as SqlDependency;

            dependency.OnChange -= new OnChangeEventHandler(dependency_OnChange);

            this.OnNewMessage(e);
        }

        #region IDisposable Members

        public void Dispose()
        {
            SqlDependency.Stop(DBConfiguration.DbConnectionString);
        }

        #endregion
    }
}
