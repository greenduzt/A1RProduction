using A1QSystem.Model;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Production.ReRoll;
using A1QSystem.Model.Products;
using A1QSystem.Model.Products.Tiles;
using A1QSystem.Model.Shifts;
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
    public class ReRollingOrdersNotifier : IDisposable
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
        public ReRollingOrdersNotifier()
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

        public ObservableCollection<ReRollingOrder> RegisterDependency()
        {

            this.CurrentCommand = new SqlCommand("SELECT Orders.order_id,Orders.sales_no,Orders.customer_id,Orders.required_date,Orders.rerolling_comments, " +
                                                 "ReRollingOrders.id AS slitting_id,ReRollingOrders.prod_timetable_id,ReRollingOrders.product_id,ReRollingOrders.raw_product_id,ReRollingOrders.qty,ReRollingOrders.rolls,ReRollingOrders.dollar_value,ReRollingOrders.order_type,ReRollingOrders.status, " +
                                                 "Products.product_code,Products.product_name,Products.product_description,Products.Unit,Products.Thickness,Products.max_yield,Products.min_yield,Products.price,Products.density,Products.width,Products.height,Products.mould_type,Products.logo_path, " +
	                                             "Customers.CompanyName, " +
	                                             "ProductionTimeTable.date, " +
	                                             "Machines.machine_id,Machines.machine_name, " +
                                                 "Shifts.shift_id,Shifts.shift_name, " +
                                                 "RawProducts.RawProductType " +
                                                 "FROM dbo.Orders " +
                                                 "INNER JOIN dbo.ReRollingOrders ON Orders.order_id = ReRollingOrders.order_no " +
                                                 "INNER JOIN dbo.Products ON ReRollingOrders.product_id = Products.id " +
                                                 "INNER JOIN dbo.Customers ON Orders.customer_id = Customers.CustomerID " +
                                                 "INNER JOIN dbo.ProductionTimeTable ON ReRollingOrders.prod_timetable_id = ProductionTimeTable.id " +
                                                 "INNER JOIN dbo.Machines ON ProductionTimeTable.machine_id = Machines.machine_id " +
                                                 "INNER JOIN dbo.Shifts ON ReRollingOrders.shift = Shifts.shift_id " +
                                                 "INNER JOIN dbo.RawProducts ON Products.raw_product_id = RawProducts.RawProductID " +
                                                 "WHERE ReRollingOrders.rolls > 0 AND ReRollingOrders.status ='ReRolling'", this.CurrentConnection);
            this.CurrentCommand.Notification = null;

            SqlDependency dependency = new SqlDependency(this.CurrentCommand);
            dependency.OnChange += this.dependency_OnChange;

            if (this.CurrentConnection.State == ConnectionState.Closed)
                this.CurrentConnection.Open();
            try
            {
                ObservableCollection<ReRollingOrder> reRollingProduction = new ObservableCollection<ReRollingOrder>();

                using (SqlDataReader dr = this.CurrentCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            ReRollingOrder rro = new ReRollingOrder();
                            rro.ID = Convert.ToInt32(dr["slitting_id"]);
                            rro.ProdTimetableID = Convert.ToInt32(dr["prod_timetable_id"]);
                            rro.Rolls = Convert.ToDecimal(dr["rolls"]);
                            rro.DollarValue = Convert.ToDecimal(dr["dollar_value"]);
                            rro.Shift = new Shift() { ShiftID = Convert.ToInt16(dr["shift_id"]), ShiftName = dr["shift_name"].ToString() };
                            rro.Status = dr["status"].ToString();
                            rro.ReRollingDate = Convert.ToDateTime(dr["date"]);
                            rro.Order = new Order()
                            {
                                OrderNo = Convert.ToInt32(dr["order_id"]),
                                SalesNo = dr["sales_no"].ToString(),
                                OrderType = Convert.ToInt16(dr["order_type"]),
                                RequiredDate = Convert.ToDateTime(dr["required_date"]),
                                Comments = dr["rerolling_comments"].ToString(),
                                Customer = new Customer()
                                {
                                    CustomerId = Convert.ToInt16(dr["customer_id"]),
                                    CompanyName = dr["CompanyName"].ToString()                                }
                            };
                            rro.Product = new Product()
                            {
                                ProductID = Convert.ToInt16(dr["product_id"]),
                                ProductCode = dr["product_code"].ToString(),
                                ProductName = dr["product_name"].ToString(),
                                ProductDescription = dr["product_description"].ToString(),
                                ProductUnit = dr["Unit"].ToString(),
                                Tile = new Tile()
                                {
                                    Thickness = Convert.ToDecimal(dr["Thickness"]),
                                    MaxYield = Convert.ToDecimal(dr["max_yield"]),
                                    MinYield = Convert.ToDecimal(dr["min_yield"]),
                                    Height = Convert.ToDecimal(dr["height"])
                                },                                
                                UnitPrice = Convert.ToDecimal(dr["price"]),
                                Density = dr["density"].ToString(),
                                Width = Convert.ToDecimal(dr["width"]),                                
                                MouldType = dr["mould_type"].ToString(),
                                LogoPath = dr["logo_path"].ToString(),
                                RawProduct = new RawProduct()
                                {
                                    RawProductID = Convert.ToInt16(dr["raw_product_id"]),
                                    RawProductType = dr["RawProductType"].ToString()
                                }
                            };
                            reRollingProduction.Add(rro);
                        }
                    }
                }

                return reRollingProduction;
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