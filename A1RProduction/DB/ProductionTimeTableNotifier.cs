using A1QSystem.Model.Production;
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
    public class ProductionTimeTableNotifier : IDisposable
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
        public ProductionTimeTableNotifier()
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


        public ObservableCollection<ProductionTimeTable> RegisterDependency()
        {
            //string neDate = DateTime.Now.ToString("yyy-MM-dd");
            //string neDate = DateTime.Now.Date;

            this.CurrentCommand = new SqlCommand("SELECT ProductionTimeTable.id,ProductionTimeTable.machine_id,ProductionTimeTable.date,ProductionTimeTable.machine_active, " +
                                                 "ProductionTimeTable.day_shift,ProductionTimeTable.evening_shift,ProductionTimeTable.night_shift " +
                                                 "FROM dbo.ProductionTimeTable WHERE ProductionTimeTable.date >= @currDate ORDER BY ProductionTimeTable.date ASC", this.CurrentConnection);


            this.CurrentCommand.Notification = null;

            SqlDependency dependency = new SqlDependency(this.CurrentCommand);
            dependency.OnChange += this.dependency_OnChange;

            if (this.CurrentConnection.State == ConnectionState.Closed)
                this.CurrentConnection.Open();
            try
            {
                ObservableCollection<ProductionTimeTable> pttList = new ObservableCollection<ProductionTimeTable>();
                CurrentCommand.Parameters.AddWithValue("@currDate", DateTime.Now.Date);
                using (SqlDataReader dr = this.CurrentCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            ProductionTimeTable ptt = new ProductionTimeTable();

                            ptt.ID = Convert.ToInt32(dr["id"]);
                            ptt.MachineID = Convert.ToInt16(dr["machine_id"]);
                            ptt.ProductionDate = Convert.ToDateTime(dr["date"]);
                            ptt.IsMachineActive = Convert.ToBoolean(dr["machine_active"]);
                            ptt.IsDayShiftActive = Convert.ToBoolean(dr["day_shift"]);
                            ptt.IsEveningShiftActive = Convert.ToBoolean(dr["evening_shift"]);
                            ptt.IsNightShiftActive = Convert.ToBoolean(dr["night_shift"]);                          
                            pttList.Add(ptt);
                        }
                    }
                }

                return pttList;
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
