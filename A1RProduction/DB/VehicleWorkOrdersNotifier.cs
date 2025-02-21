using A1QSystem.Model;
using A1QSystem.Model.Vehicles;
using A1QSystem.ViewModel.Stock;
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
    public class VehicleWorkOrdersNotifier
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

        public VehicleWorkOrdersNotifier()
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

        public ObservableCollection<VehicleWorkOrder> RegisterDependency(DateTime searchDate, string state)
        {
            string nDate = searchDate.ToString("yyyy-MM-dd");
            string currDate = DateTime.Now.ToString("yyyy-MM-dd");
            string op = "=";

            if (nDate == currDate)
            {
                op = "<=";
            }
            else
            {
                op = "=";
            }

            this.CurrentCommand = new SqlCommand("SELECT VehicleWorkOrder.id AS VehicleWorkOrderNo,VehicleWorkOrder.work_order_type,StockLocation.stock_name,Vehicles.id AS VehicleID,Vehicles.serial_number,vehicles.vehicle_code,Vehicles.vehicle_brand,Vehicles.vehicle_description,VehicleWorkOrder.first_service_date,VehicleWorkOrder.next_service_date,VehicleWorkOrder.created_date,VehicleWorkOrder.last_odometer_reading,VehicleWorkOrder.odometer_reading,VehicleWorkOrder.urgency,VehicleWorkOrder.is_viewed,VehicleWorkOrder.largest_seq_id, " +
                                                           "VehicleCategory.id AS VehicleCategoryID,VehicleCategory.vehicle_type " +
                                                           "FROM dbo.VehicleWorkOrder " +
                                                           "INNER JOIN dbo.Vehicles ON VehicleWorkOrder.vehicle_id = Vehicles.id " +
                                                           "INNER JOIN dbo.StockLocation ON Vehicles.stock_location_id = StockLocation.id " +
                                                           "INNER JOIN dbo.VehicleCategory ON Vehicles.vehicle_category_id = VehicleCategory.id " +
                                                           "WHERE dbo.VehicleWorkOrder.next_service_date" + op + "@DateSearch AND VehicleWorkOrder.is_completed = 'false' AND (VehicleWorkOrder.status <> 'Cancelled' AND VehicleWorkOrder.status <> 'Completed') AND StockLocation.stock_name = @State " +
                                                           "ORDER BY VehicleWorkOrder.urgency ASC,dbo.VehicleWorkOrder.next_service_date ASC", this.CurrentConnection);

            this.CurrentCommand.Notification = null;


            SqlDependency dependency = new SqlDependency(this.CurrentCommand);
            dependency.OnChange += this.dependency_OnChange;

            if (this.CurrentConnection.State == ConnectionState.Closed)
                this.CurrentConnection.Open();
            try
            {
                ObservableCollection<VehicleWorkOrder> vehicleWorkOrderList = new ObservableCollection<VehicleWorkOrder>();
                CurrentCommand.Parameters.AddWithValue("@DateSearch", searchDate.Date);
                CurrentCommand.Parameters.AddWithValue("@State", string.IsNullOrWhiteSpace(state) ? "QLD" : state);
                using (SqlDataReader dr = this.CurrentCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            VehicleWorkOrder vwo = new VehicleWorkOrder();
                            vwo.VehicleWorkOrderID = Convert.ToInt32(dr["VehicleWorkOrderNo"]);
                            //vwo.User = new User() { FullName = dr["FirstName"].ToString() + " " + dr["LastName"].ToString() };
                            vwo.LastOdometerReading = Convert.ToInt64(dr["last_odometer_reading"]);
                            vwo.OdometerReading = Convert.ToInt64(dr["odometer_reading"]);
                            vwo.WorkOrderType = dr["work_order_type"].ToString();
                            vwo.LargestSeqID = Convert.ToInt16(dr["largest_seq_id"]);
                            vwo.Vehicle = new Vehicle() { ID = Convert.ToInt16(dr["VehicleID"]), VehicleCode = dr["vehicle_code"].ToString(), SerialNumber = dr["serial_number"].ToString(), VehicleBrand = dr["vehicle_brand"].ToString(), VehicleDescription = dr["vehicle_description"].ToString(), StockLocation = new StockLocation() { StockName = dr["stock_name"].ToString() }, VehicleCategory = new VehicleCategory() { ID = Convert.ToInt16(dr["VehicleCategoryID"]), VehicleType = dr["vehicle_type"].ToString() } };
                            vwo.FirstServiceDate = Convert.ToDateTime(dr["first_service_date"]);
                            vwo.NextServiceDate = Convert.ToDateTime(dr["next_service_date"]);
                            vwo.CreatedDate = Convert.ToDateTime(dr["created_date"]);
                            vwo.VehicleSearchString = Convert.ToInt16(dr["VehicleID"]) + " " + dr["serial_number"].ToString() + " " + dr["vehicle_brand"].ToString() + " " + dr["vehicle_description"].ToString();
                            vwo.Urgency = Convert.ToInt16(dr["urgency"]);
                            vwo.UrgencyStr = Convert.ToInt16(dr["urgency"]) == 1 ? "Urgent" : "Normal";
                            vwo.IsViewed = Convert.ToBoolean(dr["is_viewed"]);
                            vwo.CompleteBackCol = (Convert.ToBoolean(dr["is_viewed"]) == true ? "#2E8856" : "#81b799");
                            vwo.CompleteBtnEnabled = (Convert.ToBoolean(dr["is_viewed"]) == true ? true : false);
                            vehicleWorkOrderList.Add(vwo);
                        }
                    }
                }

                return vehicleWorkOrderList;
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
