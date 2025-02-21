using A1QSystem.Model;
using A1QSystem.Model.Production.Grading;
using A1QSystem.Model.Products;
using A1QSystem.Model.Shifts;
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
    public class GradingOrdersNotifier : IDisposable
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
        public GradingOrdersNotifier()
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

        public ObservableCollection<GradingProductionDetails> RegisterDependency()
        {
            //Get the current shift
            int curShift = 0;
            DateTime curDate = DateTime.Now.Date;
            DateTime date = curDate;
            List<Shift> ShiftDetails = DBAccess.GetAllShifts();
            foreach (var item in ShiftDetails)
            {
                bool isShift = TimeBetween(DateTime.Now, item.StartTime, item.EndTime);

                if (isShift == true)
                {
                    curShift = item.ShiftID;
                }
            }

            if(curShift == 3)
            {
                curDate=date.AddDays(-1);
            }

            

            this.CurrentCommand = new SqlCommand("SELECT GradingScheduling.id AS g_id,GradingScheduling.production_time_table_id,GradingScheduling.sales_id,GradingScheduling.raw_product_id,GradingScheduling.blocklog_qty,GradingScheduling.shift,GradingScheduling.status, GradingScheduling.order_type,GradingScheduling.active_order,GradingScheduling.print_counter, " +
		                                         "RawProducts.RawProductCode, RawProducts.Description, RawProducts.RawProductType, " +
		                                         "Formulas.grading, " +
                                                 "ProductionTimeTable.id AS PID,ProductionTimeTable.date, " +
                                                 "Orders.order_id,Orders.required_date,Orders.comments,Orders.required_date_selected,Orders.sales_no,Orders.mixing_date,ISNULL(Orders.mixing_shift,'') AS mixing_shift, " +
                                                 "Customers.CompanyName, " +
                                                 "RawProductMachine.grading_machine_id " +
		                                         "FROM dbo.GradingScheduling " +
		                                         "INNER JOIN dbo.RawProducts ON GradingScheduling.raw_product_id = RawProducts.RawProductID " +
		                                         "INNER JOIN dbo.Formulas ON GradingScheduling.raw_product_id = Formulas.raw_product_id " +
                                                 "INNER JOIN dbo.ProductionTimeTable ON GradingScheduling.production_time_table_id = ProductionTimeTable.id " +
                                                 "INNER JOIN dbo.Orders ON GradingScheduling.sales_id = Orders.order_id " +
                                                 "INNER JOIN dbo.Customers ON Orders.customer_id = Customers.CustomerID " +
                                                 "INNER JOIN dbo.RawProductMachine ON RawProducts.RawProductID = RawProductMachine.raw_product_id " +
                                                 "WHERE GradingScheduling.status='Grading' AND GradingScheduling.blocklog_qty > 0 AND ProductionTimeTable.date >= @currDate " +
                                                 "ORDER BY Orders.order_type,Orders.mixing_date,Orders.mixing_shift desc", this.CurrentConnection);

            this.CurrentCommand.Notification = null;

            SqlDependency dependency = new SqlDependency(this.CurrentCommand);
            dependency.OnChange += this.dependency_OnChange;

            if (this.CurrentConnection.State == ConnectionState.Closed)
                this.CurrentConnection.Open();
            try
            {
                ObservableCollection<GradingProductionDetails> rawProductionDetails = new ObservableCollection<GradingProductionDetails>();
                CurrentCommand.Parameters.AddWithValue("@currDate", curDate);
                using (SqlDataReader dr = this.CurrentCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            GradingProductionDetails rpd = new GradingProductionDetails();

                            rpd.RawProduct = new RawProduct()
                            {
                                RawProductID = Convert.ToInt16(dr["raw_product_id"]),
                                RawProductCode = dr["RawProductCode"].ToString(),
                                Description = dr["Description"].ToString(),
                                RawProductType = dr["RawProductType"].ToString()                               
                            };
                            rpd.Customer = new Customer()
                            {
                                CompanyName = dr["CompanyName"].ToString()
                            };
                            rpd.GradingSchedulingID = Convert.ToInt32(dr["g_id"]);
                            rpd.ProdTimeTableID = Convert.ToInt32(dr["PID"]);
                            rpd.RequiredDate = dr["required_date"].ToString();
                            rpd.MixingDate = Convert.ToDateTime(dr["mixing_date"]);
                            rpd.MixingShift = dr["mixing_shift"].ToString();
                            rpd.OrderType = Convert.ToInt16(dr["order_type"]);
                            rpd.SalesOrder = dr["sales_no"].ToString();
                            rpd.RawProDetailsID = Convert.ToInt32(dr["production_time_table_id"]);
                            rpd.SalesOrderId = Convert.ToInt32(dr["order_id"]);
                            rpd.GradingFormula = dr["grading"].ToString();
                            rpd.BlockLogQty = Convert.ToDecimal(dr["blocklog_qty"]);
                            rpd.ProductionDate = dr["date"].ToString();
                            rpd.PDate = Convert.ToDateTime(dr["date"]);
                            rpd.Shift = Convert.ToInt16(dr["shift"]);
                            rpd.GradingStatus = dr["status"].ToString();
                            rpd.Notes = dr["comments"].ToString();
                            rpd.GradingActive = Convert.ToBoolean(dr["active_order"]);
                            rpd.PrintCounter = Convert.ToInt16(dr["print_counter"]);
                            rpd.ReqDateSelected = Convert.ToBoolean(dr["required_date_selected"]);
                            rpd.RawProductMachine = new RawProductMachine() { GradingMachineID = Convert.ToInt16(dr["grading_machine_id"]) };
                            
                            rawProductionDetails.Add(rpd);
                        }
                    }
                }

                return rawProductionDetails;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return null; 
            }

        }

        void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            // DBAccess.RescheduleOrdersByDate(Convert.ToDateTime("21/08/2015"));

            SqlDependency dependency = sender as SqlDependency;

            dependency.OnChange -= new OnChangeEventHandler(dependency_OnChange);

            this.OnNewMessage(e);
        }

        bool TimeBetween(DateTime datetime, TimeSpan start, TimeSpan end)
        {
            // convert datetime to a TimeSpan
            TimeSpan now = datetime.TimeOfDay;
            // see if start comes before end
            if (start < end)
                return start <= now && now <= end;
            // start is after end, so do the inverse comparison
            return !(end < now && now < start);
        }

        #region IDisposable Members

        public void Dispose()
        {
            SqlDependency.Stop(DBConfiguration.DbConnectionString);
        }

        #endregion
    }
}
