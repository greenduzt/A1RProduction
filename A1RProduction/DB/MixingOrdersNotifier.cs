using A1QSystem.Model;
using A1QSystem.Model.Production.Mixing;
using A1QSystem.Model.Products;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.DB
{
    public class MixingOrdersNotifier : IDisposable
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
        public MixingOrdersNotifier()
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

        public ObservableCollection<MixingProductionDetails> RegisterDependency()
        {
            ObservableCollection<MixingProductionDetails> rawProductionDetails = new ObservableCollection<MixingProductionDetails>();

            this.CurrentCommand = new SqlCommand("SELECT MixingCurrentCapacity.id,MixingCurrentCapacity.prod_time_table_id,MixingCurrentCapacity.mixing_time_table_id,MixingCurrentCapacity.sales_id,MixingCurrentCapacity.raw_product_id,MixingCurrentCapacity.blockLog_qty,MixingCurrentCapacity.order_type,MixingCurrentCapacity.rank,MixingCurrentCapacity.active_order, " +
                                                 "RawProducts.RawProductID,RawProducts.RawProductCode, RawProducts.Description,RawProducts.RawProductType, " +
                                                 "Formulas.mixing, " +
                                                 "ProductionTimeTable.date, " +
                                                 "Orders.order_id,Orders.sales_no,Orders.mixing_date,Orders.mixing_comments,Orders.required_date_selected,ISNULL(Orders.mixing_shift,'') AS mixing_shift, " +
                                                 "Customers.CompanyName, RawProductsActive.day_shift, RawProductsActive.evening_shift,RawProductsActive.night_shift, " +
                                                 "RawProductMachine.mixing_machine_id " + 
                                                 "FROM dbo.MixingCurrentCapacity " +
                                                 "INNER JOIN dbo.RawProducts ON MixingCurrentCapacity.raw_product_id = RawProducts.RawProductID " +
                                                 "INNER JOIN dbo.Formulas ON RawProducts.RawProductID = Formulas.raw_product_id " +
                                                 "INNER JOIN dbo.Orders ON MixingCurrentCapacity.sales_id = Orders.order_id " +
                                                 "INNER JOIN dbo.ProductionTimeTable ON MixingCurrentCapacity.mixing_time_table_id = ProductionTimeTable.id " +
                                                 "INNER JOIN dbo.RawProductsActive ON RawProducts.RawProductID = RawProductsActive.raw_product_id " +
                                                 "INNER JOIN dbo.Customers ON Orders.customer_id = Customers.CustomerID " +
                                                 "INNER JOIN dbo.RawProductMachine ON RawProducts.RawProductID = RawProductMachine.raw_product_id " +
                                                 "WHERE MixingCurrentCapacity.blockLog_qty > 0 AND MixingCurrentCapacity.status = 'Mixing' AND RawProductsActive.type = 'mixing' " +
                                                 "ORDER BY MixingCurrentCapacity.rank,Orders.mixing_date,Orders.mixing_shift desc", this.CurrentConnection);

            this.CurrentCommand.Notification = null;

            SqlDependency dependency = new SqlDependency(this.CurrentCommand);
            dependency.OnChange += this.dependency_OnChange;

            if (this.CurrentConnection.State == ConnectionState.Closed)
                this.CurrentConnection.Open();
            try
            {
                using (SqlDataReader dr = this.CurrentCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            MixingProductionDetails rpd = new MixingProductionDetails();
                            
                            rpd.RawProduct = new RawProduct()
                            {
                                RawProductID = Convert.ToInt16(dr["RawProductID"]),
                                RawProductCode = dr["RawProductCode"].ToString(),
                                Description = dr["Description"].ToString(),
                                RawProductType = dr["RawProductType"].ToString()
                            };
                            rpd.Customer = new Customer()
                            {
                                CompanyName = dr["CompanyName"].ToString()
                            };
                            rpd.RawProductsActive = new RawProductsActive()
                            {
                                DayShift = Convert.ToBoolean(dr["day_shift"]),
                                EveningShift = Convert.ToBoolean(dr["evening_shift"]),
                                NightShift = Convert.ToBoolean(dr["night_shift"])
                            };
                            rpd.MixingCurrentCapacityID = Convert.ToInt32(dr["id"]);
                            rpd.ProdTimeTableID = Convert.ToInt32(dr["prod_time_table_id"]);
                            rpd.MixingTimeTableID = Convert.ToInt32(dr["mixing_time_table_id"]); 
                            rpd.SalesOrder = dr["sales_no"].ToString();
                            rpd.SalesOrderId = Convert.ToInt32(dr["order_id"]);
                            rpd.RequiredDate = Convert.ToDateTime(dr["mixing_date"]).ToString("dd/MM/yyyy");
                            rpd.ReqDateSelected = Convert.ToBoolean(dr["required_date_selected"]);
                            rpd.Comment = dr["mixing_comments"].ToString();
                            rpd.MixingFormula = dr["mixing"].ToString();
                            rpd.BlockLogQty = Convert.ToDecimal(dr["blockLog_qty"]);
                            rpd.OrderType = Convert.ToInt32(dr["order_type"]);
                            rpd.ProductionDate = dr["date"].ToString();
                            rpd.Rank = Convert.ToInt16(dr["rank"]); 
                            rpd.ActiveOrder = Convert.ToBoolean(dr["active_order"]);
                            rpd.MixingShift = dr["mixing_shift"].ToString();
                            rpd.RawProductMachine = new RawProductMachine() { MixingMachineID = Convert.ToInt16(dr["mixing_machine_id"]) };
                            rpd.MixingDate = Convert.ToDateTime(dr["mixing_date"]);
                            rawProductionDetails.Add(rpd);

                        }
                    }
                }

                
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error reading Product Capacity: " + e);
            }

            return rawProductionDetails;
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
