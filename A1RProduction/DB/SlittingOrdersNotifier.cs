using A1QSystem.Model;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Mixing;
using A1QSystem.Model.Production.SlitingPeeling;
using A1QSystem.Model.Production.Slitting;
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
    public class SlittingOrdersNotifier: IDisposable
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
        public SlittingOrdersNotifier()
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

        public ObservableCollection<SlittingOrder> RegisterDependency(int macnineID)
        {
            this.CurrentCommand = new SqlCommand("SELECT Orders.order_id,Orders.sales_no,Orders.customer_id,Orders.required_date,Orders.slitting_comments, " +
                                                 "SlittingOrders.id AS slitting_id,SlittingOrders.prod_timetable_id,SlittingOrders.product_id,SlittingOrders.raw_product_id,SlittingOrders.qty,SlittingOrders.blocks,SlittingOrders.dollar_value,SlittingOrders.order_type,SlittingOrders.status, " +
                                                 "Products.product_name,Products.product_code,Products.product_description,Products.Unit,Products.unit_price, " +
	                                             "Customers.CompanyName, " +
	                                             "ProductionTimeTable.date, " +
	                                             "Machines.machine_id,Machines.machine_name, " +
                                                 "Shifts.shift_id,Shifts.shift_name, " +
                                                 "RawProducts.RawProductType,RawProducts.Description, " +
                                                 "ProductTiles.thickness,ProductTiles.max_yield " +
                                                 "FROM dbo.Orders " +
                                                 "INNER JOIN dbo.SlittingOrders ON Orders.order_id = SlittingOrders.order_no " +
                                                 "INNER JOIN dbo.Products ON SlittingOrders.product_id = Products.id " +
                                                 "INNER JOIN dbo.Customers ON Orders.customer_id = Customers.CustomerID " +
                                                 "INNER JOIN dbo.ProductionTimeTable ON SlittingOrders.prod_timetable_id = ProductionTimeTable.id " +
                                                 "INNER JOIN dbo.Machines ON ProductionTimeTable.machine_id = Machines.machine_id " +
                                                 "INNER JOIN dbo.Shifts ON SlittingOrders.shift = Shifts.shift_id " +
                                                 "INNER JOIN dbo.RawProducts ON SlittingOrders.raw_product_id = RawProducts.RawProductID " +
                                                 "INNER JOIN dbo.ProductTiles ON Products.id = ProductTiles.product_id " +
                                                 "WHERE SlittingOrders.blocks > 0 AND Machines.machine_id = @MachineID", this.CurrentConnection);

            this.CurrentCommand.Notification = null;


            SqlDependency dependency = new SqlDependency(this.CurrentCommand);
            dependency.OnChange += this.dependency_OnChange;

            if (this.CurrentConnection.State == ConnectionState.Closed)
                this.CurrentConnection.Open();
            try
            {
                ObservableCollection<SlittingOrder> slittingOrders = new ObservableCollection<SlittingOrder>();
                CurrentCommand.Parameters.AddWithValue("@MachineID", macnineID);
                using (SqlDataReader dr = this.CurrentCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            SlittingOrder so = new SlittingOrder();
                            so.ID = Convert.ToInt32(dr["slitting_id"]);
                            so.ProdTimetableID = Convert.ToInt32(dr["prod_timetable_id"]);
                            so.Qty = Convert.ToDecimal(dr["qty"]);
                            so.Blocks = Convert.ToDecimal(dr["blocks"]);
                            so.DollarValue = Convert.ToDecimal(dr["dollar_value"]);
                            so.Shift = new Shift() { ShiftID = Convert.ToInt16(dr["shift_id"]), ShiftName = dr["shift_name"].ToString() };
                            so.Status = dr["status"].ToString();
                            so.SlittingDate = Convert.ToDateTime(dr["date"]);    
                            so.Order = new Order()
                            {
                                OrderNo = Convert.ToInt32(dr["order_id"]),
                                SalesNo = dr["sales_no"].ToString(),
                                Comments = dr["slitting_comments"].ToString(),
                                OrderType = Convert.ToInt16(dr["order_type"]),
                                RequiredDate = Convert.ToDateTime(dr["required_date"]),
                                Customer = new Customer()
                                {
                                    CustomerId = Convert.ToInt16(dr["customer_id"]),
                                    CompanyName = dr["CompanyName"].ToString()
                                }
                            };
                            so.Product = new Product()
                            {
                                ProductID = Convert.ToInt16(dr["product_id"]),
                                ProductCode = dr["product_code"].ToString(),
                                ProductName = dr["product_name"].ToString(),
                                ProductDescription = dr["product_description"].ToString(),
                                ProductUnit = dr["Unit"].ToString(),
                                
                                Tile = new Tile(){Thickness = Convert.ToDecimal(dr["thickness"]),
                                MaxYield = Convert.ToDecimal(dr["max_yield"])},

                                UnitPrice = Convert.ToDecimal(dr["unit_price"]),
                                RawProduct = new RawProduct()
                                {
                                    RawProductID = Convert.ToInt16(dr["raw_product_id"]),
                                    RawProductType = dr["RawProductType"].ToString(),
                                    Description =  dr["Description"].ToString()
                                }
                            };
                            slittingOrders.Add(so);                                                        
                        }
                    }
                }

                return slittingOrders;
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
