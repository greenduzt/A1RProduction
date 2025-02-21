
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Stock;
using A1QSystem.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Graded_Stock
{
    public class ViewGradedStockViewModel : ViewModelBase
    {
        public string Mesh4 { get; set; }
        public string Mesh12 { get; set; }
        public string Mesh16 { get; set; }
        public string Mesh30 { get; set; }
        public string Regrind { get; set; }
        public string Red4Mesh { get; set; }
        public string Red12Mesh { get; set; }
        public string RedFines { get; set; }
        private List<MetaData> metaData;
        private List<UserPrivilages> privilages;
        private string userName;
        private string state;
        private bool canExecute;
        private string _version;
        private ICommand _backCommand;


        public ViewGradedStockViewModel(string UserName, string State, List<UserPrivilages> UserPrivilages, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = UserPrivilages;
            canExecute = true;
            metaData = md;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
            List<GradedStock> gradedStock = DBAccess.GetGradedStock();
            if(gradedStock == null || gradedStock.Count ==0)
            {
                Mesh4 = "Not found";
                Mesh12= "Not found";
                Mesh16= "Not found";
                Mesh30= "Not found";
                Regrind = "Not found";
                Red4Mesh = "Not found";
                Red12Mesh = "Not found";
                RedFines = "Not found";
            }
            else
            {
                foreach (var item in gradedStock)
                {
                    if (item.ID == 1)
                    {
                        Mesh4 = Math.Ceiling(item.Qty).ToString();
                    }
                    if (item.ID == 2)
                    {
                        Mesh12 = Math.Ceiling(item.Qty).ToString();
                    }
                    if (item.ID == 3)
                    {
                        Mesh16 = Math.Ceiling(item.Qty).ToString();
                    }
                    if (item.ID == 4)
                    {
                        Mesh30 = Math.Ceiling(item.Qty).ToString();
                    }
                    if (item.ID == 6)
                    {
                        Regrind = Math.Ceiling(item.Qty).ToString();
                    }
                    if (item.ID == 9)
                    {
                        Red4Mesh = Math.Ceiling(item.Qty).ToString();
                    }
                    if (item.ID == 10)
                    {
                        Red12Mesh = Math.Ceiling(item.Qty).ToString();
                    }

                    if (item.ID == 11)
                    {
                        RedFines = Math.Ceiling(item.Qty).ToString();
                    }
                }
            }

        }

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

        public ICommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, privilages, metaData)), canExecute));
            }
        }
    }
}
