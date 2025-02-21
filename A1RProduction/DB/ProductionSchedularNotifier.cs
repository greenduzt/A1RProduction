using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Production;
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
    public class ProductionSchedularNotifier : IDisposable
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
        public ProductionSchedularNotifier()
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

        public ObservableCollection<RawProductionDetails> RegisterDependency()
        {
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();

            this.CurrentCommand = new SqlCommand("SELECT Orders.id,Orders.order_id,Orders.sales_no,Orders.customer_id,Orders.required_date,Orders.required_date_selected,Orders.comments,Orders.order_status,Orders.mixing_date,ISNULL(Orders.mixing_shift,'') AS mixing_shift, " +
                                                "GradingScheduling.id AS GradingSchedulingID,GradingScheduling.active_order, " +
                                                "GradingScheduling.production_time_table_id,GradingScheduling.sales_id,GradingScheduling.raw_product_id,GradingScheduling.blocklog_qty,GradingScheduling.shift,GradingScheduling.order_type,GradingScheduling.print_counter, " +
                                                "RawProducts.RawProductCode, RawProducts.Description,RawProducts.RawProductType, " +
                                                "Freight.FreightDescription, " +
                                                "Customers.CompanyName, " +
                                                "ProductionTimeTable.date,ProductionTimeTable.machine_active,ProductionTimeTable.day_shift,ProductionTimeTable.evening_shift,ProductionTimeTable.night_shift,  " +
                                                "Formulas.grading_weight1,Formulas.grading_weight2,Formulas.grading_weight3, " +
                                                "RawProductMachine.grading_machine_id " +
                                                "FROM dbo.Orders " +
                                                "INNER JOIN dbo.GradingScheduling ON Orders.order_id = GradingScheduling.sales_id " +
                                                "INNER JOIN dbo.RawProducts ON GradingScheduling.raw_product_id = RawProducts.RawProductID " +
                                                "INNER JOIN dbo.Freight ON Orders.freight_id = Freight.ID " +
                                                "INNER JOIN dbo.ProductionTimeTable ON GradingScheduling.production_time_table_id = ProductionTimeTable.id " +
                                                "INNER JOIN dbo.Formulas ON GradingScheduling.raw_product_id = Formulas.raw_product_id " +
                                                "INNER JOIN dbo.RawProductMachine ON RawProducts.RawProductID = RawProductMachine.raw_product_id " +
                                                "INNER JOIN dbo.Customers ON Orders.customer_id = Customers.CustomerID WHERE GradingScheduling.status ='Grading' AND GradingScheduling.blocklog_qty > 0 ORDER BY Orders.mixing_date,Orders.mixing_shift", this.CurrentConnection);


            this.CurrentCommand.Notification = null;

            SqlDependency dependency = new SqlDependency(this.CurrentCommand);
            dependency.OnChange += this.dependency_OnChange;

            if (this.CurrentConnection.State == ConnectionState.Closed)
                this.CurrentConnection.Open();
            try
            {
                ObservableCollection<RawProductionDetails> rawProductionDetails = new ObservableCollection<RawProductionDetails>();
                using (SqlDataReader dr = this.CurrentCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            RawProductionDetails rpd = new RawProductionDetails();

                            rpd.RawProduct = new RawProduct()
                            {
                                RawProductID = Convert.ToInt16(dr["raw_product_id"]),
                                SalesID = Convert.ToInt16(dr["sales_id"]),
                                RawProductCode = dr["RawProductCode"].ToString(),
                                Description = dr["Description"].ToString(),
                                RawProductType = dr["RawProductType"].ToString()
                            };
                            rpd.Customer = new Customer()
                            {
                                CompanyName = dr["CompanyName"].ToString(),
                                CustomerId = Convert.ToInt16(dr["customer_id"])
                            };
                            rpd.GradingSchedulingID = Convert.ToInt32(dr["GradingSchedulingID"]);
                            //rpd.CurrentCapacityID = dr["CurrentCapacityID"].ToString();
                            rpd.ProdTimeTableID = Convert.ToInt32(dr["production_time_table_id"]);
                            rpd.SalesOrderId = Convert.ToInt32(dr["order_id"]);
                            rpd.RawProDetailsID = Convert.ToInt16(dr["id"]);
                            rpd.BlockLogQty = Convert.ToDecimal(dr["blocklog_qty"]);
                            rpd.ProductionDate = dr["date"].ToString();
                            rpd.PDate = Convert.ToDateTime(dr["date"]);
                            rpd.MachineActive = Convert.ToBoolean(dr["machine_active"]);
                            rpd.DayActive = Convert.ToBoolean(dr["day_shift"]);
                            rpd.EveningActive = Convert.ToBoolean(dr["evening_shift"]);
                            rpd.NightActive = Convert.ToBoolean(dr["night_shift"]);
                            rpd.Shift = Convert.ToInt16(dr["shift"]);
                            rpd.OriginType = "Grading";
                            rpd.OrderType = Convert.ToInt16(dr["order_type"]);
                            rpd.SalesOrder = dr["sales_no"].ToString();
                            rpd.OrderRequiredDate = Convert.ToDateTime(dr["required_date"]);
                            rpd.MixingDate = Convert.ToDateTime(dr["mixing_date"]);
                            rpd.MixingShift = dr["mixing_shift"].ToString();
                            rpd.FreightDescription = dr["FreightDescription"].ToString();
                            rpd.Notes = dr["comments"].ToString();
                            rpd.ActiveOrder = Convert.ToBoolean(dr["active_order"]);
                            rpd.PrintCounter = Convert.ToInt32(dr["print_counter"]);
                            rpd.ReqDateSelected = Convert.ToBoolean(dr["required_date_selected"]);
                            rpd.RawProductMachine = new RawProductMachine() { GradingMachineID = Convert.ToInt16(dr["grading_machine_id"]) };
                            rawProductionDetails.Add(rpd);
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
