using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Machine;
using A1QSystem.Model.Users;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Machine
{
    public class SetMechanicGroupComplettionViewModel : ViewModelBase
    {
        private MachineWorkOrder machineWorkOrder;
        private List<UserPosition> _userPositions;
        private string _selectedMechanic;
        private bool canExecute;
        public bool isSubmiited;
        private UserPosition _selectedUserPosition;
        public event Action<UserPosition> Closed;
        private DelegateCommand _closeCommand;
        private ICommand _submitCommand;


        public SetMechanicGroupComplettionViewModel()
        {
            UserPositions = new List<UserPosition>();
            _closeCommand = new DelegateCommand(CloseForm);
            canExecute = true;
            LoadUserPositions();
            isSubmiited = false;
            //machineWorkOrder = mwo;
            //if (!string.IsNullOrWhiteSpace(machineWorkOrder.User.FullName))
            //{
            //    SelectedMechanic = machineWorkOrder.User.FullName;
            //}
            //else
            //{
                SelectedMechanic = "Select";
            //}
        }

        private void LoadUserPositions()
        {
            UserPositions = DBAccess.GetAllUserPositions("MachineMechanic");
            UserPositions.Add(new UserPosition() { FullName = "Select" });
        }

        private void SubmitDetails()
        {
            if (SelectedMechanic == "Select")
            {
                Msg.Show("Please select mechanic's name", "Select Mechanic Name", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else
            {
                SelectedUserPosition = UserPositions.Single(s => s.FullName == SelectedMechanic);

                isSubmiited = true;
                CloseForm();
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed(SelectedUserPosition);
            }
        }

        #region PUBLIC_PROPERTIES
        public string SelectedMechanic
        {
            get { return _selectedMechanic; }
            set
            {
                _selectedMechanic = value;
                RaisePropertyChanged(() => this.SelectedMechanic);
            }
        }

        public List<UserPosition> UserPositions
        {
            get
            {
                return _userPositions;
            }
            set
            {
                _userPositions = value;
                RaisePropertyChanged(() => this.UserPositions);
            }
        }

        public UserPosition SelectedUserPosition
        {
            get
            {
                return _selectedUserPosition;
            }
            set
            {
                _selectedUserPosition = value;
                RaisePropertyChanged(() => this.SelectedUserPosition);
            }
        }

        #endregion

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public ICommand SubmitCommand
        {
            get
            {
                return _submitCommand ?? (_submitCommand = new A1QSystem.Commands.LogOutCommandHandler(() => SubmitDetails(), canExecute));
            }
        }
    }
}
