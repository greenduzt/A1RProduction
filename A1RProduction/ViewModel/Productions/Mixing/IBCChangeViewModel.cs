using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Production.Mixing;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.ViewModel.Productions.Mixing
{
    public class IBCChangeViewModel : ViewModelBase
    {
        private List<BinderType> _binderType;
        private string _selectedBinderType;
        private string _batchNo;
        private DateTime _dateTime;
        public event Action Closed;
        private DelegateCommand _closeCommand;
        private DelegateCommand _submitCommand;

        public IBCChangeViewModel()
        {
            DateTime = DateTime.Now;
            SelectedBinderType = "Select";
            BinderType = new List<BinderType>();
            BinderType = DBAccess.GetBinderTypes();
            _closeCommand = new DelegateCommand(CloseForm);
            _submitCommand = new DelegateCommand(ChangeIBC);
        }

        private void ChangeIBC()
        {
            if (SelectedBinderType == "Select")
            {
                Msg.Show("Please select the Binder Type.", "Binder Type Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else if (string.IsNullOrWhiteSpace(BatchNo))
            {
                Msg.Show("Please enter batch no.", "Batch No Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else
            {
                IBCChangeOver ibcc = new IBCChangeOver();
                ibcc.DateTime = DateTime;
                ibcc.BinderType = SelectedBinderType;
                ibcc.BatchNo = BatchNo;

                int res = DBAccess.InsertIBCChangeOverDetails(ibcc);
                if (res == 0)
                {
                    Msg.Show("There has been a problem while changing IBC details. Please try again later.", "Data Did Not Saved", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }

                CloseForm();
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }



        public List<BinderType> BinderType
        {
            get
            {
                return _binderType;
            }
            set
            {
                _binderType = value;
                RaisePropertyChanged(() => this.BinderType);
            }
        }

        public DateTime DateTime
        {
            get
            {
                return _dateTime;
            }
            set
            {
                _dateTime = value;
                RaisePropertyChanged(() => this.DateTime);
            }
        }
        public string SelectedBinderType
        {
            get
            {
                return _selectedBinderType;
            }
            set
            {
                _selectedBinderType = value;
                RaisePropertyChanged(() => this.SelectedBinderType);
            }
        }

        public string BatchNo
        {
            get
            {
                return _batchNo;
            }
            set
            {
                _batchNo = value;
                RaisePropertyChanged(() => this.BatchNo);
            }
        }



        #region COMMANDS

        public DelegateCommand SubmitCommand
        {
            get { return _submitCommand; }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        #endregion
    }
}
