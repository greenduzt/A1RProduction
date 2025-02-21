using A1QSystem.Model.Production;
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
    public class GradingMixingNotifier : IDisposable
    {
        private event EventHandler<SqlNotificationEventArgs> _newMessage;
        private SqlConnection connection;
        public SqlCommand CurrentCommand { get; set; }

        public GradingMixingNotifier()
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


        public ObservableCollection<ProductionTotals> RegisterDependency()
        {
            ObservableCollection<ProductionTotals> psList = new ObservableCollection<ProductionTotals>();

            this.CurrentCommand = new SqlCommand("SELECT Orders.required_date as grading_date,ISNULL(GradingScheduling.raw_product_id,0) as grading_raw_product_id,ISNULL(GradingScheduling.shift,0) as grading_shift, ISNULL(GradingScheduling.blocklog_qty,0) as grading_blocklog_qty, ISNULL(g.RawProductType,'') as grading_unit, " +
                                                 "Orders.mixing_date,ISNULL(MixingCurrentCapacity.raw_product_id,0) as raw_product_id,Orders.mixing_shift,ISNULL(MixingCurrentCapacity.blockLog_qty,0) as mixing_block_log, ISNULL(m.RawProductType,'') as mixing_unit " +
                                                 "FROM dbo.Orders " +
                                                 "LEFT JOIN dbo.GradingScheduling ON Orders.order_id = GradingScheduling.sales_id " +
                                                 "LEFT JOIN dbo.MixingCurrentCapacity ON Orders.order_id = MixingCurrentCapacity.sales_id " +
                                                 "LEFT JOIN dbo.RawProducts g ON GradingScheduling.raw_product_id = g.RawProductID " +
                                                 "LEFT JOIN dbo.RawProducts m ON MixingCurrentCapacity.raw_product_id = m.RawProductID", this.CurrentConnection);
                       

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
                            ProductionTotals pt = new ProductionTotals();
                            pt.GradingDate = Convert.ToDateTime(dr["grading_date"]);

                            string shiftName = string.Empty;
                            if (Convert.ToInt16(dr["grading_shift"]) == 1)
                            {
                                shiftName = "Morning";
                            }
                            else if (Convert.ToInt16(dr["grading_shift"]) == 2)
                            {
                                shiftName = "Arvo";
                            }
                            else if (Convert.ToInt16(dr["grading_shift"]) == 3)
                            {
                                shiftName = "Night";
                            }

                            pt.GradingShift = shiftName;
                            pt.GradingUnit = dr["grading_unit"].ToString();
                            pt.GradingQty = Convert.ToDecimal(dr["grading_blocklog_qty"]);

                            pt.MixingDate = Convert.ToDateTime(dr["mixing_date"]);
                            pt.MixingingUnit = dr["mixing_unit"].ToString();
                            pt.MixingShift = dr["mixing_shift"].ToString();
                            pt.MixingQty = Convert.ToDecimal(dr["mixing_block_log"]);

                            psList.Add(pt);
                        }
                    }
                }


            }
            catch (Exception e)
            {

                Debug.WriteLine("Error reading dispatch details: " + e);
            }
            return psList;
        }

        void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            // DBAccess.RescheduleOrdersByDate(Convert.ToDateTime("21/08/2015"));

            SqlDependency dependency = sender as SqlDependency;

            dependency.OnChange -= new OnChangeEventHandler(dependency_OnChange);

            this.OnNewMessage(e);
        }

        public static T CheckNull<T>(object obj)
        {
            return (obj == DBNull.Value ? default(T) : (T)obj);

        }

        #region IDisposable Members

        public void Dispose()
        {
            SqlDependency.Stop(DBConfiguration.DbConnectionString);
        }

        #endregion

    }
}

