using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Sales;
using A1QSystem.View;
using A1QSystem.View.Sales;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace A1QSystem.ViewModel.Sales
{
    public class QuoteToSaleViewModel : ViewModelBase
    {
        private string _userName;
        private string _state;
        private List<UserPrivilages> _privilages;
        private bool _canExecute;
        public ObservableCollection<QuoteToOrder> quoteDetails { get; set; }
        private ListCollectionView _quoteDetailsListColl;
        private List<MetaData> metaData;
        private ICommand _backCommand;
        private ICommand _homeCommand;
        private ICommand _salesCommand;

        public Dispatcher UIDispatcher { get; set; }
        public QuoteToOrderNotifier Notifier { get; set; }

        public QuoteToSaleViewModel(string UserName, string State, List<UserPrivilages> uPriv, Dispatcher uidispatcher, List<MetaData> md)
        {
            _userName = UserName;
            _state = State;
            _privilages = uPriv;
            metaData = md;
            quoteDetails = new ObservableCollection<QuoteToOrder>();

            this.UIDispatcher = uidispatcher;
            this.Notifier = new QuoteToOrderNotifier();

            this.Notifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_NewMessage);
            quoteDetails = this.Notifier.RegisterDependency(UserName, State);

            this.LoadData(quoteDetails);
            
            _canExecute = true;           
          
        }

        private void LoadData(ObservableCollection<QuoteToOrder> op)
        {
            this.UIDispatcher.BeginInvoke((Action)delegate()
            {
                if (op != null)
                {
                    quoteDetails = op;

                    ObservableCollection<FreightDetails> freightDetails = DBAccess.GetFreightDetailsQuoteToSale();
                    foreach (var x in freightDetails)
                    {
                        if (x.QuoteID != 0)
                        {
                            QuoteDetails qd = new QuoteDetails();
                            QuoteToOrder qs = new QuoteToOrder(UserName, State);

                            qd.ID = Convert.ToInt16(x.QuoteID);
                            qd.ProductCode = x.FreightName;
                            qd.Quantity = Convert.ToDecimal(x.FreightPallets);
                            qd.ProductDescription = x.FreightDescription;
                            qd.ProductPrice = x.FreightPrice;
                            qd.ProductUnit = x.FreightUnit;
                            qd.Discount = x.FreightDiscount;
                            qd.Total = x.FreightTotal;

                            qs.quoteDetails = qd;
                            quoteDetails.Add(qs);
                        }
                    }
                    
                    QuoteDetailsListColl = new ListCollectionView(quoteDetails);
                    QuoteDetailsListColl.GroupDescriptions.Add(new PropertyGroupDescription("quoteDetails.ID"));
                }
            });
        }

        void notifier_NewMessage(object sender, SqlNotificationEventArgs e)
        {
            this.LoadData(this.Notifier.RegisterDependency(UserName, State));
        }

        #region PUBLIC PROPERTIES


        public ListCollectionView QuoteDetailsListColl
        {
            get { return _quoteDetailsListColl; }
            set
            {
                _quoteDetailsListColl = value;
                RaisePropertyChanged(() => this.QuoteDetailsListColl);

            }
        }


        public string UserName 
        {
            get { return _userName;}
            set 
            { 
                _userName = value;
            }
        }

        public string State
        {
            get { return _state; }
            set
            {
                _state = value;
            }
        }

        #endregion

        #region COMMANDS

        public ICommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new LogOutCommandHandler(() => Switcher.Switch(new SalesMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
        public ICommand SalesCommand
        {
            get
            {
                return _salesCommand ?? (_salesCommand = new LogOutCommandHandler(() => Switcher.Switch(new SalesMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        #endregion
    }
}
