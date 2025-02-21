using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Comments;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Comments
{
    public class CommentsViewModel : BaseViewModel, IDataErrorInfo
    {
        public int QuoteNo { get; set; }
        public int UserID { get; set; }
        public string State { get; set; }
        //public string NewComment { get; set; }
        public List<Comment> Comments { get; set; }
        public string CurrDate { get; set; }

        public event Action Closed;
        private DelegateCommand _closeCommand;
       // private DelegateCommand _addCommand;
        private ICommand _addCommand;


        public CommentsViewModel(int quoteNo, string userName, string state)
        {
           

            string[] ssize = userName.Split(new char[0]);
            List<User> user = DBAccess.GetUserByName(ssize[0], ssize[1]);
           
            QuoteNo = quoteNo;
            State = state;

            foreach(var x in user){
                UserID = x.ID;
            }

            DateTime today = DateTime.Now;
            CurrDate = String.Format("{0:dd/MM/yyyy HH:mm:ss}", today);

            GetComments();

            _closeCommand = new DelegateCommand(CloseForm);
        //    _addCommand = new DelegateCommand(AddComment);
        }

        private string _newComment;
        public string NewComment
        {
            get { return _newComment; }
            set
            {
                _newComment = value;
                RaisePropertyChanged("NewComment");
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        private void GetComments()
        {
            Comments = DBAccess.GetCommentsByQuoteNo(QuoteNo);

        }

        private void AddComment()
        {
            //if(!String.IsNullOrWhiteSpace(NewComment))
            //{

                int res = DBAccess.InsertComment(QuoteNo, UserID, NewComment, CurrDate);
                if (res > 0)
                {
                    Msg.Show("Comment has been added successfully", "Comment Added", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);
                    CloseForm();
                }
                else
                {
                    Msg.Show("An error has occured! Please try again later", "Comment Addition Failed", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                }        
   
           // }
        }

        public string QuoteNoString { get { return State+QuoteNo; } }


        string IDataErrorInfo.Error
        {
            get
            {
                return null;
            }
        }


        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                return GetValidationError(propertyName);
            }
        }
        static readonly string[] ValidatedProperies = 
        {
            "NewComment"
        };

        public bool IsValid
        {
            get
            {
                foreach (string property in ValidatedProperies)
                {
                    if (GetValidationError(property) != null)

                        return false;
                }
                return true;
            }
        }

        string GetValidationError(string propertyName)
        {
            string error = null;

            switch (propertyName)
            {
                case "NewComment":
                    error = ValidateCompanyName();
                    break;
                default:
                    error = null;
                    throw new Exception("Unexpected property being validated on Service");
            }
            return error;
        }

        private string ValidateCompanyName()
        {
            if (String.IsNullOrWhiteSpace(NewComment))
            {
                return "Comment required!";
            }

            return null;
        }


        #region COMMANDS
        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public ICommand AddCommand
        {
            get
            {
                if (_addCommand == null)
                    _addCommand = new A1QSystem.Commands.RelayCommand(param => this.AddComment(), param => this.CanSave);

                return _addCommand;
            }
        }

        protected bool CanSave
        {
            get
            {
                return IsValid;
            }
        }

        //public DelegateCommand AddCommand
        //{
        //    get { return _addCommand; }
        //}

        #endregion
    }
}
