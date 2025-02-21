using A1QSystem.Commands;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.Orders;
using A1QSystem.View.Production;
using A1QSystem.View.Quoting;
using A1QSystem.View.StockMaintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel
{
    public class QuotingMainMenuViewModel : CommonBase
    {
        private ICommand _newQuoteCommand;
        private ICommand _updateQCommand;        
        private ICommand _backCommand;
        private ICommand navHomeCommand;
        private ICommand _slitPeelCommand;
        private ICommand _stockMaintenanceCommand;
        private bool _canExecute;
        private List<UserPrivilages> _privilages;
        private List<MetaData> metaData;
        private string _userName;
        private string _state;
        private string _btnNewQuote;
        private string _btnUpdateQuote;

        public QuotingMainMenuViewModel(string UserName, string Password, List<UserPrivilages> uPriv, List<MetaData> md)
        {
            _userName = UserName;
            _state = Password;
            _privilages = uPriv;
            metaData = md;
            _canExecute = true;

            //for (int i = 0; i < uPriv.Count; i++)
            //{
            //    BtnNewQuote = uPriv[i].AddQuote;
            //    BtnUpdateQuote = uPriv[i].UpdateQuote;
            //}
        }

        public string BtnNewQuote
        {
            get { return _btnNewQuote; }
            set
            {
                if (value != _btnNewQuote)
                {
                    _btnNewQuote = value;
                    RaisePropertyChanged("BtnNewQuote");
                }
            }
        }

       
        public string BtnUpdateQuote
        {
            get { return _btnUpdateQuote; }
            set
            {
                if (value != _btnUpdateQuote)
                {
                    _btnUpdateQuote = value;
                    RaisePropertyChanged("BtnUpdateQuote");
                }
            }
        }

        public ICommand NewQuoteCommand
        {
            get
            {
                return _newQuoteCommand ?? (_newQuoteCommand = new LogOutCommandHandler(() => Switcher.Switch(new ProductionSchedulerView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
       
        public ICommand UpdateQCommand
        {
            get
            {
                return _updateQCommand ?? (_updateQCommand = new LogOutCommandHandler(() => Switcher.Switch(new AddProductionOrderView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
        public ICommand BackCommand
        {
            get
            {

                return _backCommand ?? (_backCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand SlitPeelCommand
        {
            get
            {
                return _slitPeelCommand ?? (_slitPeelCommand = new LogOutCommandHandler(() => Switcher.Switch(new SlitPeelProductionSchedulerView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand StockMaintenanceCommand
        {
            get
            {
                return null;// _stockMaintenanceCommand ?? (_stockMaintenanceCommand = new LogOutCommandHandler(() => Switcher.Switch(new StockMaintenanceView(_userName, _state, _privilages)), _canExecute));
            }
        }

        
       
    }
}
