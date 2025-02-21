using A1QSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production.ReRoll
{
    public class OffSpec : ViewModelBase
    {
        public int RollNo { get; set; }
        public decimal LM { get; set; }
        private bool _isTooThick;
        private bool _isTooThin;
        private bool _isContaminated;
        private bool _isOther;
        private bool _isOperatorError;


        public bool IsTooThick
        {
            get { return _isTooThick; }
            set { _isTooThick = value; RaisePropertyChanged(() => this.IsTooThick); }
        }

        public bool IsTooThin
        {
            get { return _isTooThin; }
            set { _isTooThin = value; RaisePropertyChanged(() => this.IsTooThin); }
        }

        public bool IsContaminated
        {
            get { return _isContaminated; }
            set { _isContaminated = value; RaisePropertyChanged(() => this.IsContaminated); }
        }

        public bool IsOther
        {
            get { return _isOther; }
            set { _isOther = value; RaisePropertyChanged(() => this.IsOther); }
        }

        public bool IsOperatorError
        {
            get { return _isOperatorError; }
            set { _isOperatorError = value; RaisePropertyChanged(() => this.IsOperatorError); }
        }
    }
}
