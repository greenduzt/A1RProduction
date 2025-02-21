using A1QSystem.Commands;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.Orders;
using A1QSystem.View.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel
{
    public class ProductionMenuViewModel : CommonBase
    {
        private string _userName;
        private string _state;
        private List<UserPrivilages> _privilages;
        private string _btnNewDailyProduction;
        private string _btnViewProduction;
        private List<MetaData> metaData;
        private bool _canExecute;
        private ICommand _commandViewProductionView;
        private ICommand _newDailyProdCommand;
        private ICommand _commandsBack;
        private ICommand navHomeCommand;
        private ICommand _prodOverviewCommand;

        public ProductionMenuViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            _canExecute = true;
            _userName = UserName;
            _state = State;
            _privilages = Privilages;
            metaData = md;
            //for (int i = 0; i < _privilages.Count; i++)
            //{
            //    BtnNewDailyProduction = _privilages[i].AddDailyProduction;
            //    if (BtnNewDailyProduction == "visible")
            //    {
            //        _btnNewDailyProduction = "visible";
            //    }
            //    else
            //    {
            //        _btnNewDailyProduction = "hidden";
            //    }

            //    BtnViewProduction = _privilages[i].ViewProduction;
            //    if (BtnViewProduction == "visible")
            //    {
            //        _btnViewProduction = "visible";
            //    }
            //    else
            //    {
            //        _btnViewProduction = "hidden";
            //    }
            //}

        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
            }
        }

      
        public string BtnNewDailyProduction
        {
            get { return _btnNewDailyProduction; }
            set
            {
                _btnNewDailyProduction = value;
                RaisePropertyChanged("BtnNewDailyProduction");
            }
        }

        
        public string BtnViewProduction
        {
            get { return _btnViewProduction; }
            set
            {
                _btnViewProduction = value;
                RaisePropertyChanged("BtnViewProduction");
            }
        }

        #region Commands

        public ICommand CommandViewProductionView
        {
            get
            {
                return _commandViewProductionView ?? (_commandViewProductionView = new LogOutCommandHandler(() => Switcher.Switch(new ProductionView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand NewDailyProdCommand
        {
            get
            {
                return _newDailyProdCommand ?? (_newDailyProdCommand = new LogOutCommandHandler(() => Switcher.Switch(new DailyProductionView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand CommandsBack
        {
            get
            {
                return _commandsBack ?? (_commandsBack = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }


      
        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand ProdOverviewCommand
        {
            get
            {
                return null;// _prodOverviewCommand ?? (_prodOverviewCommand = new LogOutCommandHandler(() => Switcher.Switch(new OrderProductionSchedularView()), _canExecute));
            }
        }

        #endregion

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
