using A1QSystem.Model;
using A1QSystem.Model.Machine;
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
    public class MachineMaintenanceOrdersNotifier : IDisposable
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

        public MachineMaintenanceOrdersNotifier()
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


        public ObservableCollection<MachineWorkOrder> RegisterDependency(DateTime date)
        {
            string nDate = date.ToString("yyyy-MM-dd");
            string currDate = DateTime.Now.ToString("yyyy-MM-dd");
            string op = "=";

            if (date.Date == DateTime.Now.Date)
            {
                op = "<=";
            }

            this.CurrentCommand = new SqlCommand("SELECT MachineMaintenanceWorkOrder.id,MachineMaintenanceWorkOrder.machine_id,Machines.machine_name,Machines.type,MachineMaintenanceWorkOrder.user_id,MachineMaintenanceWorkOrder.urgency,MachineMaintenanceWorkOrder.work_order_type,MachineMaintenanceWorkOrder.first_service_date, " +
                                                           "MachineMaintenanceWorkOrder.maintenance_freq,MachineMaintenanceWorkOrder.maintenance_freq_str,MachineMaintenanceWorkOrder.next_service_date,MachineMaintenanceWorkOrder.created_date,MachineMaintenanceWorkOrder.created_by,MachineMaintenanceWorkOrder.is_completed, " +
                                                           "MachineMaintenanceWorkOrder.completed_date,MachineMaintenanceWorkOrder.completed_by,MachineMaintenanceWorkOrder.status " +
                                                           "FROM dbo.MachineMaintenanceWorkOrder " +
                                                           "INNER JOIN dbo.Machines ON MachineMaintenanceWorkOrder.machine_id = Machines.machine_id " +
                                                           "WHERE MachineMaintenanceWorkOrder.next_service_date " + op + " @DateSearch AND MachineMaintenanceWorkOrder.is_completed ='false' AND (MachineMaintenanceWorkOrder.status <> 'Cancelled' AND MachineMaintenanceWorkOrder.status <> 'Completed') " +
                                                           "ORDER BY MachineMaintenanceWorkOrder.next_service_date,MachineMaintenanceWorkOrder.urgency", this.CurrentConnection);

            this.CurrentCommand.Notification = null;


            SqlDependency dependency = new SqlDependency(this.CurrentCommand);
            dependency.OnChange += this.dependency_OnChange;

            if (this.CurrentConnection.State == ConnectionState.Closed)
                this.CurrentConnection.Open();
            try
            {
                ObservableCollection<MachineWorkOrder> machineWorkOrderList = new ObservableCollection<MachineWorkOrder>();
                CurrentCommand.Parameters.AddWithValue("@DateSearch", date.Date);
                using (SqlDataReader dr = this.CurrentCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            DateTime? dt = dr["completed_date"] as DateTime?;

                            MachineWorkOrder vwo = new MachineWorkOrder();
                            vwo.WorkOrderNo = Convert.ToInt32(dr["id"]);
                            vwo.FirstServiceDate = Convert.ToDateTime(dr["first_service_date"]);
                            vwo.Machine = new Machines(0) { MachineID = Convert.ToInt16(dr["machine_id"]), MachineName = dr["machine_name"].ToString(), MachineType = dr["type"].ToString() };
                            vwo.User = new User() { ID = Convert.ToInt16(dr["user_id"]) };
                            vwo.Urgency = Convert.ToInt16(dr["urgency"]);
                            vwo.WorkOrderType = dr["work_order_type"].ToString();
                            vwo.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = Convert.ToInt16(dr["maintenance_freq"]), Frequency = dr["maintenance_freq_str"].ToString() };
                            //vwo.Frequency = Convert.ToInt16(dr["maintenance_freq"]);
                            vwo.NextServiceDate = Convert.ToDateTime(dr["next_service_date"]);

                            vwo.CreatedDate = Convert.ToDateTime(dr["created_date"]);
                            vwo.CreatedBy = dr["created_by"].ToString();
                            vwo.IsCompleted = Convert.ToBoolean(dr["is_completed"]);
                            vwo.CompletedDate = dt;
                            vwo.UrgencyStr = Convert.ToInt16(dr["urgency"]) == 1 ? "Urgent" : "Normal";
                            machineWorkOrderList.Add(vwo);
                        }
                    }
                }

                return machineWorkOrderList;
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
