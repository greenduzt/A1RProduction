using A1QSystem.Model.Products;
using A1QSystem.Model.RawMaterials;
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
    public class MixingProductionSchedulerNotifier : IDisposable
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
        public MixingProductionSchedulerNotifier()
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


            this.CurrentCommand = new SqlCommand("SELECT MixingProductionDetails.id,MixingProductionDetails.m_prod_raw_product_id,MixingProductionDetails.m_prod_blocklog_qty,MixingProductionDetails.m_prod_production_date,MixingProductionDetails.m_prod_shift, " +
                                                 "RawProducts.RawProductCode, RawProducts.Description,RawProducts.RawProductType " +
                                                 "FROM dbo.MixingProductionDetails " +
                                                 "INNER JOIN dbo.RawProducts ON MixingProductionDetails.m_prod_raw_product_id = RawProducts.RawProductID WHERE MixingProductionDetails.m_prod_status ='Mixing'", this.CurrentConnection);

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
                                RawProductID = Convert.ToInt16(dr["m_prod_raw_product_id"]),
                                RawProductCode = dr["RawProductCode"].ToString(),
                                Description = dr["Description"].ToString(),
                                RawProductType = dr["RawProductType"].ToString()
                            };
                            rpd.RawProDetailsID = Convert.ToInt16(dr["id"]);
                            rpd.BlockLogQty = Convert.ToDecimal(dr["m_prod_blocklog_qty"]);
                            rpd.ProductionDate = dr["m_prod_production_date"].ToString();
                            rpd.Shift = Convert.ToInt16(dr["m_prod_shift"]);
                            rpd.OriginType = "Mixing";


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
