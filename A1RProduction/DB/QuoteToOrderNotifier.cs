using A1QSystem.Model;
using A1QSystem.Model.Sales;
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
    public class QuoteToOrderNotifier : IDisposable
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

        public QuoteToOrderNotifier()
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

        public ObservableCollection<QuoteToOrder> RegisterDependency(string UserName, string State)
        {
            DateTime now = DateTime.Now;

            string currentDate = now.ToString("dd/MM/yyyy");

            this.CurrentCommand = new SqlCommand("SELECT Quotes.ID, Quotes.Prefix,Quotes.QuoteDate,Quotes.CustomerName,Quotes.ProName,Quotes.tax,Quotes.ListPriceTot,Quotes.SubTotal,Quotes.TotAmount,Quotes.SalesPerson, " + 
                                                 "QuoteDetails.QuoteID, QuoteDetails.ProductCode, QuoteDetails.ProductDescription, QuoteDetails.ProductPrice, QuoteDetails.ProductUnit,QuoteDetails.Quantity,QuoteDetails.Discount,QuoteDetails.Total, " +
                                                 "FreightDetails.FreightName, FreightDetails.ShipToAddress, FreightDetails.FreightTotal " +
                                                 "FROM dbo.Quotes INNER JOIN dbo.QuoteDetails ON Quotes.ID = QuoteDetails.QuoteID " +
                                                 "INNER JOIN dbo.FreightDetails ON Quotes.ID = FreightDetails.QuoteID " +
                                                 "WHERE Quotes.QuoteApproved = '" + false + "' ORDER BY Quotes.ID DESC", this.CurrentConnection);           
            this.CurrentCommand.Notification = null;
            
            SqlDependency dependency = new SqlDependency(this.CurrentCommand);
            dependency.OnChange += this.dependency_OnChange;
            
            if (this.CurrentConnection.State == ConnectionState.Closed)
                this.CurrentConnection.Open();
            try
            {
                ObservableCollection<QuoteToOrder> quoteToOrder = new ObservableCollection<QuoteToOrder>();
                using (SqlDataReader dr = this.CurrentCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            QuoteToOrder qo = new QuoteToOrder(UserName, State);
                            QuoteDetails qd = new QuoteDetails();
                            Quote q = new Quote();
                            FreightDetails fd = new FreightDetails();

                           

                            q.ID = Convert.ToInt16(dr["ID"]);
                            q.Prefix = dr["Prefix"].ToString();
                            q.QuoteDate = dr["QuoteDate"].ToString();
                            q.CustomerName = dr["CustomerName"].ToString();
                            q.ProName = dr["ProName"].ToString();
                            q.Tax = Convert.ToDecimal(dr["tax"]);
                            q.ListPriceTot = Convert.ToDecimal(dr["ListPriceTot"]);
                            q.SubTotal = Convert.ToDecimal(dr["SubTotal"]);
                            q.TotAmount = Convert.ToDecimal(dr["TotAmount"]);
                            q.SalesPerson = dr["SalesPerson"].ToString();

                            qd.ID = Convert.ToInt16(dr["QuoteID"]);
                            qd.ProductCode = dr["ProductCode"].ToString();
                            qd.ProductDescription = dr["ProductDescription"].ToString();
                            qd.ProductPrice = Convert.ToDecimal(dr["ProductPrice"]);
                            qd.ProductUnit = dr["ProductUnit"].ToString();
                            qd.Quantity = Convert.ToDecimal(dr["Quantity"]);
                            qd.Discount = Convert.ToDecimal(dr["Discount"]);
                            qd.Total = Convert.ToDecimal(dr["Total"]);

                            fd.FreightName = dr["FreightName"].ToString();
                            fd.FreightTotal = Convert.ToDecimal(dr["FreightTotal"]);
                            fd.ShipToAddress = dr["ShipToAddress"].ToString();

                            if (q.QuoteDate == currentDate)
                            {
                                qo.CanvasNew = "Visible";
                            }
                            else
                            {
                                qo.CanvasNew = "Hidden";
                            }

                            

                            qo.quote = q;
                            qo.quoteDetails = qd;
                            qo.freightDetails = fd;
                            quoteToOrder.Add(qo);
                        }
                    }
                }
                return quoteToOrder;
            }
            catch { return null; }
        }

        void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {

          //  Console.WriteLine(e.Type);
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
