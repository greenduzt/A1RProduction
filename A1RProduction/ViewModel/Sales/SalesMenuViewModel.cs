using A1QSystem.Commands;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Sales
{
    public class SalesMenuViewModel
    {
        private string _userName;
        private string _state;
        private List<UserPrivilages> _privilages;
        private List<MetaData> metaData;
        private ICommand navHomeCommand;
        private ICommand _backCommand;
        private ICommand _quoteToSaleCommand;

        private bool _canExecute;


        public SalesMenuViewModel(string UserName, string Password, List<UserPrivilages> uPriv, List<MetaData> md)
        {
            _userName = UserName;
            _state = Password;
            _privilages = uPriv;
            metaData = md;
            _canExecute = true;
        }

        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand BackCommand
        {
            get
            {

                return _backCommand ?? (_backCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand QuoteToSaleCommand
        {
            get
            {

                return _quoteToSaleCommand ?? (_quoteToSaleCommand = new LogOutCommandHandler(() => Switcher.Switch(new QuoteToSaleView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
    }
}
