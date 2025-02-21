
using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Machine;
using A1QSystem.Model.Meta;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace A1QSystem.ViewModel.Machine
{
    public class AddNewProviderViewModel : ViewModelBase
    {
        private string _newProviderName, _providerNameVisibility, _providerNameListVisibility, _btnName;
        private int _selectedProvider;
        private List<MachineProvider> _machineProviders;
        private MachineProvider _machineProvider;
        private bool canExecute;
        public event Action Closed;
        private ICommand _addProviderCommand;
        private ICommand _addNewProviderCommand;
        private ICommand _closeNewProviderCommand;
        private Microsoft.Practices.Prism.Commands.DelegateCommand _closeCommand;

        public AddNewProviderViewModel()
        {
            MachineProvider = new MachineProvider();
            canExecute = true;
            MachineProvider.State = "Select";
            ProviderNameVisibility = "Collapsed";
            ProviderNameListVisibility = "Visible";
            BtnName = "UPDATE";

            LoadProviders();
            _closeCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(CloseForm);
        }

        private void LoadProviders()
        {
            MachineProviders = DBAccess.GetAllProviders("All");
            MachineProviders.Insert(0, new MachineProvider() { ProviderID = 0, ProviderName = "Select"});
            SelectedProvider = 0;
            MachineProvider.State = "Select";
            MachineProvider.Active = "Select";
        }

        private void CloseBtnNewProvider()
        {
            ProviderNameVisibility = "Collapsed";
            ProviderNameListVisibility = "Visible";
            SelectedProvider = 0;
            MachineProvider = new MachineProvider();
            MachineProvider.ProviderName = String.Empty;
            MachineProvider.Address = String.Empty;
            MachineProvider.Suburb = String.Empty;
            MachineProvider.PostCode = String.Empty;
            MachineProvider.State = "Select";
            NewProviderName = string.Empty;
            BtnName = "UPDATE";
            MachineProvider.Active = "Select";
            LoadProviders();
        }

        private void AddNewProviderDB()
        {
            if(string.IsNullOrWhiteSpace(MachineProvider.ProviderName) && string.IsNullOrWhiteSpace(NewProviderName))
            {
                Msg.Show("Please enter provider name", "", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.OK);
            }
            else if (MachineProvider.State.Equals("Select"))
            {
                Msg.Show("Please enter state", "", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.OK);
            }
            else if (string.IsNullOrWhiteSpace(MachineProvider.ContactName))
            {
                Msg.Show("Please enter contact name", "", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.OK);
            }
            else
            {
                int result =  DBAccess.AddNewProviderToDB(MachineProvider, NewProviderName);
                if (result > 0)
                {
                    Msg.Show("Provider added successfully", "", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.OK);
                    CloseForm();
                }
                else if (result == -1)
                {
                    Msg.Show("Provider name exists!", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
                }
                else
                {
                    Msg.Show("Failed to add provider", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
                }
            }
        }


        private void AddNewProvider()
        {
            ProviderNameVisibility = "Visible";
            ProviderNameListVisibility = "Collapsed";
            MachineProvider.ProviderName = String.Empty;
            MachineProvider.Address = String.Empty;
            MachineProvider.Suburb = String.Empty;
            MachineProvider.PostCode = String.Empty;
            MachineProvider.State = "Select";
            NewProviderName = string.Empty;
            BtnName = "ADD";
            MachineProvider.Active = "Select";
            MachineProvider.ContactName = String.Empty;
            MachineProvider.Email = String.Empty;
            MachineProvider.Phone = String.Empty;
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        
        public string NewProviderName
        {
            get
            {
                return _newProviderName;
            }
            set
            {
                _newProviderName = value;
                RaisePropertyChanged(() => this.NewProviderName);
            }
        }

        public string ProviderNameVisibility
        {
            get
            {
                return _providerNameVisibility;
            }
            set
            {
                _providerNameVisibility = value;
                RaisePropertyChanged(() => this.ProviderNameVisibility);
            }
        }

        public string ProviderNameListVisibility
        {
            get
            {
                return _providerNameListVisibility;
            }
            set
            {
                _providerNameListVisibility = value;
                RaisePropertyChanged(() => this.ProviderNameListVisibility);
            }
        }

        public int SelectedProvider
        {
            get
            {
                return _selectedProvider;
            }
            set
            {
                _selectedProvider = value;
                RaisePropertyChanged(() => this.SelectedProvider);
                if(SelectedProvider > 0)
                {
                    MachineProvider mp = MachineProviders.FirstOrDefault(x=>x.ProviderID == SelectedProvider);
                    if(mp != null)
                    {
                        MachineProvider = mp;
                    }
                }
                else
                {
                    MachineProvider = new MachineProvider();
                }
            }
        }
        public List<MachineProvider> MachineProviders
        {
            get
            {
                return _machineProviders;
            }
            set
            {
                _machineProviders = value;
                RaisePropertyChanged(() => this.MachineProviders);
            }
        }

        public MachineProvider MachineProvider
        {
            get
            {
                return _machineProvider;
            }
            set
            {
                _machineProvider = value;
                RaisePropertyChanged(() => this.MachineProvider);
            }
        }

        public string BtnName
        {
            get
            {
                return _btnName;
            }
            set
            {
                _btnName = value;
                RaisePropertyChanged(() => this.BtnName);
            }
        }

        
        public ICommand CloseNewProviderCommand
        {
            get
            {
                return _closeNewProviderCommand ?? (_closeNewProviderCommand = new A1QSystem.Commands.LogOutCommandHandler(() => CloseBtnNewProvider(), canExecute));
            }
        }

        public ICommand AddNewProviderCommand
        {
            get
            {
                return _addNewProviderCommand ?? (_addNewProviderCommand = new LogOutCommandHandler(() => AddNewProvider(), canExecute));
            }
        }

        public ICommand AddProviderCommand
        {
            get
            {
                return _addProviderCommand ?? (_addProviderCommand = new LogOutCommandHandler(() => AddNewProviderDB(), canExecute));
            }
        }

        public Microsoft.Practices.Prism.Commands.DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }
    }
}
