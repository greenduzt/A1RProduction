using A1QSystem.Core;
using A1QSystem.Model.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.ViewModel.Customers
{
    public class CustomerDetailsViewModel : ViewModelBase
    {
        private string userName;
        private List<MetaData> metaData;
        private string _version;

        public CustomerDetailsViewModel(string un, List<MetaData> md)
        {
            userName = un;
            metaData = md;

            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
        }


        #region Public_Properties
        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
                RaisePropertyChanged(() => this.Version);
            }
        }
        #endregion
    }
}
