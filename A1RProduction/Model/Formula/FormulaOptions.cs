using A1QSystem.Core;
using A1QSystem.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Formula
{
    public class FormulaOptions : ViewModelBase
    {
        public int ID { get; set; }
        public int GradingSchedulingID { get; set; }
        public int GroupID { get; set; }
        public RawProduct RawProduct { get; set; }
        //public Formulas Formula { get; set; }

        private Formulas _formula;
        public Formulas Formula
        {
            get
            {
                return _formula;
            }
            set
            {
                _formula = value;
                RaisePropertyChanged(() => this.Formula);
            }
        }

        private bool _checked;
        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
                RaisePropertyChanged(() => this.Checked);
                if(Checked)
                {
                    IsEnabled = false;
                }
                else
                {
                    IsEnabled = true;
                }
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                RaisePropertyChanged(() => this.IsEnabled);
            }
        }
    }
}
