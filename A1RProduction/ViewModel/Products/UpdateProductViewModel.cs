using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.Products;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Products
{
    public class UpdateProductViewModel : ViewModelBase
    {

        private string _userName;
        private string state;
        private List<UserPrivilages> _privilages;
        private List<MetaData> metaData;
        private ICommand _backCommand;
        private ICommand navHomeCommand;
        private ICommand navProductCommand;
        private bool _canExecute;

        public UpdateProductViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            _userName = UserName;
            state = State;
            _privilages = Privilages;
            _canExecute = true;
            metaData = md;
            LoadProductDetails();
        }


        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = UserName;
            }
        }

        private ObservableCollection<string> _productCodes;
        public ObservableCollection<string> ProductCodes
        {
            get { return _productCodes; }
            set
            {
                _productCodes = value;
                RaisePropertyChanged(() => this.ProductCodes);
            }
        }

        private ObservableCollection<string> _productDescriptions;
        public ObservableCollection<string> ProductDescriptions
        {
            get { return _productDescriptions; }
            set
            {
                _productDescriptions = value;
                RaisePropertyChanged(() => this.ProductDescriptions);
            }
        }

        private string _selectedProductCode;
        public string SelectedProductCode
        {
            get
            {
                return _selectedProductCode;
            }
            set
            {
                _selectedProductCode = value;
                RaisePropertyChanged(() => this.SelectedProductCode);              
            }
        }

        private string _selectedProductDescription;
        public string SelectedProductDescription
        {
            get
            {
                return _selectedProductDescription;
            }
            set
            {
                _selectedProductDescription = value;
                RaisePropertyChanged(() => this.SelectedProductDescription);
            }
        }


        private void LoadProductDetails()
        {
            DataView pdv = new DataView();
            pdv = DBAccess.GetAllProducts().Tables["Products"].DefaultView;
            pdv.Sort = "ProductCode ASC";

            var proCodes = new ObservableCollection<string>();
            var proDescriptions = new ObservableCollection<string>();

            for (int x = 0; x < pdv.Count; x++)
            {
                proCodes.Add(pdv[x]["ProductCode"].ToString());
            }
            ProductCodes = proCodes;

            pdv.Sort = "ProductDescription ASC";
            for (int x = 0; x < pdv.Count; x++)
            {
                proDescriptions.Add(pdv[x]["ProductDescription"].ToString());
            }
            ProductDescriptions = proDescriptions;

        }

        private void NavigateHome()
        {
         //   if (!String.IsNullOrWhiteSpace(ProductName))
         //   {
         //       if (Msg.Show("Are you sure you want to navigate to HOME screen?" + System.Environment.NewLine + "You haven't finished filling up fields!", "Naivgation Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
         //       {
         //           Switcher.Switch(new MainMenu(_userName, state, _privilages));
         //       }
         //   }
         //   else
         //   {
            Switcher.Switch(new MainMenu(_userName, state, _privilages, metaData));
         //   }
        }

        private void NavigateProducts()
        {
         //   if (!String.IsNullOrWhiteSpace(ProductName))
         //   {
         //       if (Msg.Show("Are you sure you want to navigate to PRODUCTS screen?" + System.Environment.NewLine + "You haven't finished filling up fields!", "Naivgation Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
         //       {
         //           Switcher.Switch(new ProductsMenu(_userName, state, _privilages));
         //       }
         //   }
         //   else
         //   {
            Switcher.Switch(new ProductsMenu(_userName, state, _privilages, metaData));
         //   }
        }

        #region Commands

        public ICommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new LogOutCommandHandler(() => Switcher.Switch(new ProductsMenu(_userName, state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => NavigateHome(), _canExecute));
            }
        }

        public ICommand NavProductsCommand
        {
            get
            {
                return navProductCommand ?? (navProductCommand = new LogOutCommandHandler(() => NavigateProducts(), _canExecute));
            }
        }

        #endregion
    }
}
