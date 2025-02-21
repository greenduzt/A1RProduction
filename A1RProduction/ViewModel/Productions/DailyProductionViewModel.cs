using A1QSystem.Commands;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.Production;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel
{
    public class DailyProductionViewModel
    {
        private ICommand _commandLargeMixer;
        private ICommand _commandHighSpeedMixer;
        private ICommand _commandSmallMixer;
        private ICommand _commandSCBR;
        private ICommand _commandsBagging;
        private ICommand _commandsSlitter;
        private ICommand _commandsLogPeel;
        private ICommand _commandsRollUp;
        private ICommand _commandsGradding;
        private ICommand _commandsShredding;
        private ICommand _commandsBack;
        private ICommand navHomeCommand;
        private ICommand navProductionsCommand;

       // private string _UserName;

        private string _userName;
        private string _state;
        private List<UserPrivilages> _privilages;
        private List<MetaData> metaData;
        private bool _canExecute;

        public DailyProductionViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {

            _canExecute = true;
            metaData = md;
            _userName = UserName;
            _state = State;
            _privilages = Privilages;

        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = UserName;          
            }
        }

      

        #region Commands

        public ICommand CommandLargeMixer
        {
            get
            {
                return _commandLargeMixer ?? (_commandLargeMixer = new LogOutCommandHandler(() => Switcher.Switch(new MachineLargeMixerView(_userName, _state, "Large Mixer", _privilages, metaData)), _canExecute));
            }
        }

        public ICommand CommandHighSpeedMixer
        {
            get
            {
                return _commandHighSpeedMixer ?? (_commandHighSpeedMixer = new LogOutCommandHandler(() => Switcher.Switch(new MachineLargeMixerView(_userName, _state, "High Speed Mixer", _privilages, metaData)), _canExecute));
            }
        }

        public ICommand CommandSmalldMixer
        {
            get
            {
                return _commandSmallMixer ?? (_commandSmallMixer = new LogOutCommandHandler(() => Switcher.Switch(new MachineLargeMixerView(_userName, _state, "Small Mixer", _privilages, metaData)), _canExecute));
            }
        }
        public ICommand CommandSCBR
        {
            get
            {
                return _commandSCBR ?? (_commandSCBR = new LogOutCommandHandler(() => Switcher.Switch(new MachineLargeMixerView(_userName, _state, "CSBR Machine", _privilages, metaData)), _canExecute));
            }
        }
        public ICommand CommandsBagging
        {
            get
            {
                return _commandsBagging ?? (_commandsBagging = new LogOutCommandHandler(() => Switcher.Switch(new MachineLargeMixerView(_userName, _state, "Bagging Machine", _privilages, metaData)), _canExecute));
            }
        }
        public ICommand CommandsSlitter
        {
            get
            {
                return _commandsSlitter ?? (_commandsSlitter = new LogOutCommandHandler(() => Switcher.Switch(new MachineLargeMixerView(_userName, _state, "Slitter Machine", _privilages, metaData)), _canExecute));
            }
        }

        public ICommand CommandsLogPeel
        {
            get
            {
                return _commandsLogPeel ?? (_commandsLogPeel = new LogOutCommandHandler(() => Switcher.Switch(new MachineLargeMixerView(_userName, _state, "Log and Peel Machine", _privilages, metaData)), _canExecute));
            }
        }

        public ICommand CommandsRollUp
        {
            get
            {
                return _commandsRollUp ?? (_commandsRollUp = new LogOutCommandHandler(() => Switcher.Switch(new MachineLargeMixerView(_userName, _state, "Roll Up Machine", _privilages, metaData)), _canExecute));
            }
        }
        public ICommand CommandsGradding
        {
            get
            {
                return _commandsGradding ?? (_commandsGradding = new LogOutCommandHandler(() => Switcher.Switch(new MachineLargeMixerView(_userName, _state, "Rubber Grading Machine", _privilages, metaData)), _canExecute));
            }
        }
        public ICommand CommandsShredding
        {
            get
            {
                return _commandsShredding ?? (_commandsShredding = new LogOutCommandHandler(() => Switcher.Switch(new MachineLargeMixerView(_userName, _state, "Shredding Machine", _privilages, metaData)), _canExecute));
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return _commandsBack ?? (_commandsBack = new LogOutCommandHandler(() => Switcher.Switch(new ProductionMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

       
        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
        public ICommand NavProductionsCommand
        {
            get
            {
                return navProductionsCommand ?? (navProductionsCommand = new LogOutCommandHandler(() => Switcher.Switch(new ProductionMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
        
        #endregion

    }
}
