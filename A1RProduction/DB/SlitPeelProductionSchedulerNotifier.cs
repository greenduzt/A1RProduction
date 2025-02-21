using A1QSystem.Model;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.SlitingPeeling;
using A1QSystem.Model.Products;
using A1QSystem.Model.RawMaterials;
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
    public class SlitPeelProductionSchedulerNotifier : IDisposable
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
        public SlitPeelProductionSchedulerNotifier()
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

        public ObservableCollection<SlitPeelSchedule> RegisterDependency()
        {

            this.CurrentCommand = new SqlCommand("SELECT SalesOrder.sales_order_no,SalesOrder.required_date,SalesOrder.freight_arr_time,SalesOrder.freight_time_available, " +
                                                 "SlitPeel.prod_time_table_id AS ProdTimeTableID,SlitPeel.sales_order_id, SlitPeel.id,SlitPeel.qty_to_make,SlitPeel.blocks_logs,SlitPeel.shift,SlitPeel.blocks_logs,SlitPeel.product_id,SlitPeel.raw_product_id,SlitPeel.type,SlitPeel.order_type,SlitPeel.status,SlitPeel.qty_made,SlitPeel.dollar_value, " +
                                                 "Products.ProductCode,Products.ProductUnit,Products.max_items_per,Products.ProductPrice, " +
                                                 "Customers.CustomerID,Customers.CompanyName, " +
                                                 "RawStock.rs_qty, " +
												 "ProductionTimeTable.date, " +
                                                 "ProductionOrderInfo.description, " + 
                                                 "Freight.FreightDescription " +
                                                 "FROM dbo.SalesOrder " +
                                                 "INNER JOIN dbo.SlitPeel ON SalesOrder.id = SlitPeel.sales_order_id " +
                                                 "INNER JOIN dbo.Products ON SlitPeel.product_id = Products.ProductID " +
                                                 "INNER JOIN dbo.Customers ON SalesOrder.customer_id = Customers.CustomerID " +                                             
                                                 "INNER JOIN dbo.RawStock ON Products.RawProductID = RawStock.rs_raw_product_id " +
												 "INNER JOIN dbo.ProductionTimeTable ON SlitPeel.prod_time_table_id = ProductionTimeTable.id " +
                                                 "INNER JOIN dbo.ProductionOrderInfo ON Products.ProductID = ProductionOrderInfo.product_id AND SalesOrder.id = ProductionOrderInfo.sales_order_id " +
                                                 "INNER JOIN dbo.Freight ON SalesOrder.freight_id = Freight.ID", this.CurrentConnection);

            this.CurrentCommand.Notification = null;


            SqlDependency dependency = new SqlDependency(this.CurrentCommand);
            dependency.OnChange += this.dependency_OnChange;           

            if (this.CurrentConnection.State == ConnectionState.Closed)
                this.CurrentConnection.Open();
            try
            {
                ObservableCollection<SlitPeelSchedule> rawProductionDetails = new ObservableCollection<SlitPeelSchedule>();
                using (SqlDataReader dr = this.CurrentCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            SlitPeelSchedule sps = new SlitPeelSchedule();

                            //sps.Product = new Product()
                            //{
                            //    ProductID = Convert.ToInt16(dr["product_id"]),
                            //    ProductCode = dr["ProductCode"].ToString(),
                            //    ProductDescription = dr["description"].ToString(),
                            //    ProductUnit = dr["ProductUnit"].ToString(),
                            //    MaxItemsPer = Convert.ToDecimal(dr["max_items_per"]),
                            //    RawProductID = Convert.ToInt16(dr["raw_product_id"]),
                            //    ProductPrice = Convert.ToInt16(dr["ProductPrice"])
                            //};
                            //sps.Customer = new Customer()
                            //{
                            //    CompanyName = dr["CompanyName"].ToString()
                            //};
                            //sps.SlitPeel = new SlitPeel()
                            //{
                            //    ProdTimeTableID = Convert.ToInt32(dr["ProdTimeTableID"]),
                            //    ID = Convert.ToInt16(dr["id"]),
                            //    QtyToMake = Convert.ToDecimal(dr["qty_to_make"]),
                            //    QtyMade = Convert.ToDecimal(dr["qty_made"]),
                            //    DollarValue = Convert.ToDecimal(dr["dollar_value"]),
                            //    Shift = Convert.ToInt16(dr["shift"]),
                            //    ProductionDate = dr["date"].ToString(),
                            //    OriginalBlockLogs = Convert.ToDecimal(dr["blocks_logs"]),
                            //    SalesOrderID = Convert.ToInt32(dr["sales_order_id"]),
                            //    Type = dr["type"].ToString(),
                            //    OrdertypeID = Convert.ToInt16(dr["order_type"]),
                            //    Status = dr["status"].ToString()
                            //};
                            //sps.RawStock = new RawStock()
                            //{
                            //    Qty = Convert.ToDecimal(dr["rs_qty"])
                            //};
                            
                            //sps.SalesOrder = dr["sales_order_no"].ToString();
                            ////sps.FreightArrDate = Convert.ToDateTime(dr["freight_arr_date"]);
                            ////sps.FreightDateAvailable = Convert.ToBoolean(dr["freight_date_available"]);
                            //sps.FreightArrTime = dr["freight_arr_time"].ToString();
                            //sps.FreightTimeAvailable = Convert.ToBoolean(dr["freight_time_available"]);
                            //sps.FreightDescription = dr["FreightDescription"].ToString();
                            
                            rawProductionDetails.Add(sps);
                           
                        }
                    }
                }

                return rawProductionDetails;
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
