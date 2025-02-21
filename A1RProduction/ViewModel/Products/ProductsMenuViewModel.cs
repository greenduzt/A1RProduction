using A1QSystem.Commands;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.Products;
using A1QSystem.View.Quoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Products
{
    public class ProductsMenuViewModel :CommonBase
    {
        private string _userName;
        private string _state;
        private List<UserPrivilages> _userPrivilages;
        private string _btnUpdateProduct;
        private string _btnNewProduct;
        private List<MetaData> metaData;
        private ICommand _productsCommand;
        private ICommand _commandsBack;
        private ICommand _updateCommand;
        private ICommand navHomeCommand;
        private bool _canExecute;

        public ProductsMenuViewModel(string UserName, string State, List<UserPrivilages> UserPrivilages, List<MetaData> md)
        {
            _userName = UserName;
            _state = State;
            _userPrivilages = UserPrivilages;
            metaData = md;
            _canExecute = true;

            //for (int i = 0; i < _userPrivilages.Count; i++)
            //{
            //    BtnUpdateProduct = _userPrivilages[i].UpdateProduct;
            //    if (BtnUpdateProduct == "visible")
            //    {
            //        _btnUpdateProduct = "visible";
            //    }
            //    else
            //    {
            //        _btnUpdateProduct = "hidden";
            //    }

            //    BtnNewProduct = _userPrivilages[i].AddProduct;
            //    if (BtnNewProduct == "visible")
            //    {
            //        _btnNewProduct = "visible";
            //    }
            //    else
            //    {
            //        _btnNewProduct = "hidden";
            //    }
            //}
        }

        #region Public Properties

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = UserName;
            }
        }

        public string BtnUpdateProduct
        {
            get { return _btnUpdateProduct; }
            set
            {
                _btnUpdateProduct = value;
                RaisePropertyChanged("BtnUpdateProduct");
            }
        }

        public string BtnNewProduct
        {
            get { return _btnNewProduct; }
            set
            {
                _btnNewProduct = value;
                RaisePropertyChanged("BtnNewProduct");
            }
        }

        #endregion

        #region Commands

        public ICommand ProductsCommand
        {
            get
            {
                return _productsCommand ?? (_productsCommand = new LogOutCommandHandler(() => Switcher.Switch(new AddProductView(_userName, _state, _userPrivilages, metaData)), _canExecute));
            }
        }

        public ICommand UpdateCommand
        {
            get
            {
                return _updateCommand ?? (_updateCommand = new LogOutCommandHandler(() => Switcher.Switch(new UpdateProductView(_userName, _state, _userPrivilages, metaData)), _canExecute));
            }
        }        

        public ICommand CommandsBack
        {
            get
            {
                return _commandsBack ?? (_commandsBack = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _userPrivilages, metaData)), _canExecute));
            }
        }
     
        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _userPrivilages, metaData)), _canExecute));
            }
        }

        #endregion
    }
}
