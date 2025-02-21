

using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Shreding;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Productions.Grading
{
    public class AddToRegrindStockViewModel : ViewModelBase
    {
        private CalculationModel calculation;
        private bool canExecute;
        private string _regrind;
        private string _shockpadCorkShred;
        private string _shockpadShred;
        private string lastOperation;
        private bool newDisplayRequired = false;
        private string _qty;
        private string textboxType;
        //private List<Shred> _shred;

        public event Action Closed;
        private DelegateCommand _closeCommand;
        private DelegateCommand _submitCommand;
        private ICommand _regrindCommand;
        private ICommand _shockpadCorkShredCommand;
        private ICommand _ShockpadShredCommand;
        private DelegateCommand<string> _digitButtonPressCommand;

        public AddToRegrindStockViewModel()
        {
            this.calculation = new CalculationModel();
            this.FirstOperand = string.Empty;
            this.SecondOperand = string.Empty;
            this.Operation = string.Empty;
            this.lastOperation = string.Empty;
            this.fullExpression = string.Empty;
            canExecute = true;
            //Shred = new List<Shred>();
            //Shred=DBAccess.GetShredingInfo(true);
            _closeCommand = new DelegateCommand(CloseForm);
            _submitCommand = new DelegateCommand(AddToRegrindStock);
        }

        private void AddToRegrindStock()
        {
            if(string.IsNullOrWhiteSpace(Regrind) && string.IsNullOrWhiteSpace(ShockpadShred) && string.IsNullOrWhiteSpace(ShockpadCorkShred))
            {
                Msg.Show("Quantity Required", "Qauntity Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else
            {
                decimal regrind = 0;
                decimal shockpadShred = 0;
                decimal shockpadCorkShred = 0;
                List<ShredStock> shredStock = new List<ShredStock>();
                ShiftManager sm = new ShiftManager();
                if(!string.IsNullOrWhiteSpace(Regrind)) 
                {
                    regrind = Decimal.Parse(Regrind);
                    if (regrind > 0)
                    {
                        shredStock.Add(new ShredStock() { Shred = new Shred() { ID = 6 }, Qty = regrind });
                    }
                }

                if (!string.IsNullOrWhiteSpace(ShockpadCorkShred))
                {
                    shockpadCorkShred = Decimal.Parse(ShockpadCorkShred);
                    if (shockpadCorkShred > 0)
                    {
                        shredStock.Add(new ShredStock() { Shred = new Shred() { ID = 7 }, Qty = shockpadCorkShred });
                    }
                }

                if (!string.IsNullOrWhiteSpace(ShockpadShred))
                {
                    shockpadShred = Decimal.Parse(ShockpadShred);
                    if (shockpadShred > 0)
                    {
                        shredStock.Add(new ShredStock() { Shred = new Shred() { ID = 8 }, Qty = shockpadShred });
                    }
                }
              

                int res = DBAccess.UpdateShredStock(shredStock, sm.GetCurrentShift());
                if (res != 0)
                {
                    CloseForm();
                }
                else
                {
                    Console.WriteLine("Stock was not updated");
                }
            }
        }

        private void RegrindClicked()
        {
            textboxType="RegrindClicked";
        }

        private void ShockpadCorkShredClicked()
        {
            textboxType="ShockpadCorkShredClicked";
        }

        private void ShockpadShredClicked()
        {
           textboxType="ShockpadShredClicked";
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        //deals with button inputs and sorts out the display accordingly
        public void DigitButtonPress(string button)
        {
            bool deleted = false;
            Qty = string.Empty;
            
            switch (button)
            {
                case "C":
                    Qty = "0";
                    FirstOperand = string.Empty;
                    SecondOperand = string.Empty;
                    Operation = string.Empty;
                    LastOperation = string.Empty;
                    FullExpression = string.Empty;
                    break;
                case "Del":
                    if (textboxType == "RegrindClicked")
                    {
                        _qty = Regrind;
                    }
                    else if (textboxType == "ShockpadCorkShredClicked")
                    {
                        _qty = ShockpadCorkShred;
                    }
                    else if (textboxType == "ShockpadShredClicked")
                    {
                        _qty = ShockpadShred;
                    }

                    if (_qty.Length > 1)
                    {
                        Qty = _qty.Substring(0, _qty.Length - 1);
                    }
                    else
                    {
                        Qty = "";
                    }

                    if (textboxType == "RegrindClicked")
                    {
                        Regrind = Qty;
                    }
                    else if (textboxType == "ShockpadCorkShredClicked")
                    {
                        ShockpadCorkShred = Qty;
                    }
                    else if (textboxType == "ShockpadShredClicked")
                    {
                        ShockpadShred = Qty;
                    }
                    deleted = true;
                    break;
                case "+/-":
                    if (_qty.Contains("-") || _qty == "0")
                    {
                        Qty = _qty.Remove(_qty.IndexOf("-"), 1);
                    }
                    else Qty = "-" + _qty;
                    break;
                case ".":
                    if (newDisplayRequired)
                    {
                        Qty = "0.";
                    }
                    else
                    {
                        if (!_qty.Contains("."))
                        {
                            Qty = _qty + ".";
                        }
                    }
                    break;
                default:
                    if (_qty == "0" || newDisplayRequired)
                        Qty = button;
                    else
                        Qty = _qty + button;
                    break;
            }

            if (deleted == false)
            {

                if (textboxType == "RegrindClicked")
                {
                    Regrind += Qty;
                }
                else if (textboxType == "ShockpadCorkShredClicked")
                {
                    ShockpadCorkShred += Qty;
                }
                else if (textboxType == "ShockpadShredClicked")
                {
                    ShockpadShred += Qty;
                }
            }
            
            newDisplayRequired = false;
        }

        #region PUBLIC PROPERTIES

        private static bool CanDigitButtonPress(string button)
        {
            return true;
        }

        public string Regrind
        {
            get { return _regrind; }
            set { _regrind = value; RaisePropertyChanged(() => this.Regrind); }
        }

        public string ShockpadCorkShred
        {
            get { return _shockpadCorkShred; }
            set { _shockpadCorkShred = value; RaisePropertyChanged(() => this.ShockpadCorkShred); }
        }

        public string ShockpadShred
        {
            get { return _shockpadShred; }
            set { _shockpadShred = value; RaisePropertyChanged(() => this.ShockpadShred); }
        }

        public string FirstOperand
        {
            get { return calculation.FirstOperand; }
            set { calculation.FirstOperand = value; }
        }

        public string SecondOperand
        {
            get { return calculation.SecondOperand; }
            set { calculation.SecondOperand = value; }
        }


        public string Operation
        {
            get { return calculation.Operation; }
            set { calculation.Operation = value; }
        }

        public string LastOperation
        {
            get { return lastOperation; }
            set { lastOperation = value; }
        }

        private string fullExpression;
        public string FullExpression
        {
            get { return fullExpression; }
            set
            {
                fullExpression = value;
                RaisePropertyChanged(() => this.FullExpression);
            }
        }

        public string Qty
        {
            get
            {
                return _qty;
            }
            set
            {
                _qty = value;
                RaisePropertyChanged(() => this.Qty);

            }
        }

        //public List<Shred> Shred
        //{
        //    get { return _shred; }
        //    set { _shred = value; RaisePropertyChanged(() => this.Shred); }
        //}

        #endregion

        #region COMMANDS

        public DelegateCommand SubmitCommand
        {
            get { return _submitCommand; }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public ICommand RegrindCommand
        {
            get
            {
                return _regrindCommand ?? (_regrindCommand = new A1QSystem.Commands.LogOutCommandHandler(() => RegrindClicked(), canExecute));
            }
        }

        public ICommand ShockpadCorkShredCommand
        {
            get
            {
                return _shockpadCorkShredCommand ?? (_shockpadCorkShredCommand = new A1QSystem.Commands.LogOutCommandHandler(() => ShockpadCorkShredClicked(), canExecute));
            }
        }

        public ICommand ShockpadShredCommand
        {
            get
            {
                return _ShockpadShredCommand ?? (_ShockpadShredCommand = new A1QSystem.Commands.LogOutCommandHandler(() => ShockpadShredClicked(), canExecute));
            }
        }
        

        public ICommand DigitButtonPressCommand
        {
            get
            {
                if (_digitButtonPressCommand == null)
                {
                    _digitButtonPressCommand = new DelegateCommand<string>(
                        DigitButtonPress, CanDigitButtonPress);
                }
                return _digitButtonPressCommand;
            }
        }

        #endregion
    }
}
