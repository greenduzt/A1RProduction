using A1QSystem.Model.Products;
using A1QSystem.Model.Stock;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.DB
{
    public class ReadingRawStockNotifier :  IDisposable
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
        public ReadingRawStockNotifier()
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

        public BindingList<StockMaintenanceDetails> RegisterDependency()
        {


            this.CurrentCommand = new SqlCommand("SELECT RawProducts.RawProductID, RawProducts.RawProductCode,RawProducts.Description,RawProducts.RawProductType, " +
                                                 "RawStock.rs_qty,RawStock.rs_re_order_qty " +
                                                 "FROM dbo.RawProducts " +
                                                 "INNER JOIN dbo.RawStock ON RawStock.rs_raw_product_id = RawProducts.RawProductID order by RawProducts.RawProductCode", this.CurrentConnection);

            this.CurrentCommand.Notification = null;


            SqlDependency dependency = new SqlDependency(this.CurrentCommand);
            dependency.OnChange += this.dependency_OnChange;

            if (this.CurrentConnection.State == ConnectionState.Closed)
                this.CurrentConnection.Open();
            try
            {
                BindingList<StockMaintenanceDetails> stockMaintenanceDetailsList = new BindingList<StockMaintenanceDetails>();
                using (SqlDataReader dr = this.CurrentCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            StockMaintenanceDetails smd = new StockMaintenanceDetails();

                            smd.RawProduct = new RawProduct()
                            {
                                RawProductID = Convert.ToInt16(dr["RawProductID"]),
                                RawProductCode = dr["RawProductCode"].ToString(),
                                Description = dr["Description"].ToString(),
                                RawProductType = dr["RawProductType"].ToString()
                            };
                            smd.RawStock = new RawStock()
                            {
                                Qty = Math.Round(Convert.ToDecimal(dr["rs_qty"]),2),
                                ReOrderQty =  Math.Round(Convert.ToDecimal(dr["rs_re_order_qty"]),2)
                            };
                            stockMaintenanceDetailsList.Add(smd);      
                        }
                    }
                }

                return stockMaintenanceDetailsList;
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
