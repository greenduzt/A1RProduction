using A1QSystem.Model.Production.Grading;
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
    public class GradingWeeklyScheduleNotifier : IDisposable
    {
        private event EventHandler<SqlNotificationEventArgs> _newMessage;
        private SqlConnection connection;

        public SqlCommand CurrentCommand { get; set; }

        public GradingWeeklyScheduleNotifier()
        {
            SqlDependency.Start(DBConfiguration.DbConnectionString);
        }

        public SqlConnection CurrentConnection
        {
            get
            {
                this.connection = this.connection ?? new SqlConnection(DBConfiguration.DbConnectionString);
                return this.connection;
            }
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

        public ObservableCollection<GradingWeeklySchedule> RegisterDependency()
        {
            ObservableCollection<GradingWeeklySchedule> rawProductionDetails = new ObservableCollection<GradingWeeklySchedule>();

            this.CurrentCommand = new SqlCommand("SELECT Orders.order_id,RawProducts.RawProductID,RawProducts.Description,RawProducts.RawProductType, ISNULL(GradingScheduling.blocklog_qty,0) AS g_blockLogs,Orders.mixing_date,Orders.mixing_shift,Orders.comments " +
                                                 "FROM dbo.GradingScheduling " +
                                                 "INNER JOIN dbo.Orders ON GradingScheduling.sales_id  = Orders.order_id " +
                                                 "INNER JOIN dbo.RawProducts ON GradingScheduling.raw_product_id = RawProducts.RawProductID " +
                                                 "WHERE dbo.GradingScheduling.sales_id = GradingScheduling.sales_id AND GradingScheduling.raw_product_id <> 61 AND GradingScheduling.blocklog_qty > 0 " +
                                                 "ORDER BY dbo.Orders.mixing_date,Orders.mixing_shift DESC", this.CurrentConnection);

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
                            GradingWeeklySchedule rpd = new GradingWeeklySchedule();
                            rpd.OrderNo = Convert.ToInt64(dr["order_id"]);
                            rpd.RawProduct = new RawProduct()
                            {
                                RawProductID = Convert.ToInt16(dr["RawProductID"]),
                                Description = dr["Description"].ToString(),
                                RawProductType = dr["RawProductType"].ToString()
                            };

                            rpd.GradingBlockLogs = Convert.ToInt64(dr["g_blockLogs"]);
                            rpd.MixingDate = Convert.ToDateTime(dr["mixing_date"]);
                            rpd.MixingShift = dr["mixing_shift"].ToString();
                            rpd.Comments = dr["comments"].ToString();
                            rpd.IsCommentsVisible = string.IsNullOrWhiteSpace(rpd.Comments) ? "Collapsed" : "Visible";
                            rawProductionDetails.Add(rpd);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error reading dispatch details: " + e);
            }

            return rawProductionDetails;
        }

        void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
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
