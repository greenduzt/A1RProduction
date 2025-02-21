using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Stock;
using A1QSystem.View;
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
    
    public class AddToGradedStockViewModel : ViewModelBase
    {
        private CalculationModel calculation;
        private string _qty;
        private string _mesh4;
        private string _mesh12;
        private string _mesh16;
        private string _mesh30;
        private string _mesh4Red;
        private string _mesh12Red;
        private string _redFines;
        private string lastOperation;
        private string textboxType;
        private bool newDisplayRequired = false;
        public event Action Closed;
        private DelegateCommand _closeCommand;
        private DelegateCommand _submitCommand;
        private DelegateCommand<string> _digitButtonPressCommand;
        private DelegateCommand<string> _gradedStockCommand;

        public AddToGradedStockViewModel()
        {
            this.calculation = new CalculationModel();
            this.FirstOperand = string.Empty;
            this.SecondOperand = string.Empty;
            this.Operation = string.Empty;
            this.lastOperation = string.Empty;
            this.fullExpression = string.Empty;
            _closeCommand = new DelegateCommand(CloseForm);
            _submitCommand = new DelegateCommand(AddToGradedStock);
           
        }

        private void AddToGradedStock()
        {
            if (string.IsNullOrWhiteSpace(Mesh4) && string.IsNullOrWhiteSpace(Mesh12) && string.IsNullOrWhiteSpace(Mesh16) && string.IsNullOrWhiteSpace(Mesh30) && string.IsNullOrWhiteSpace(Mesh4Red) && string.IsNullOrWhiteSpace(Mesh12Red) && string.IsNullOrWhiteSpace(RedFines))
            {
                Msg.Show("Quantity Required", "Qauntity Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else
            {
                ShiftManager sm = new ShiftManager();
                List<GradedStock> gradedStockList = new List<GradedStock>();
                decimal mesh4 = 0;
                decimal mesh12 = 0;
                decimal mesh16 = 0;
                decimal mesh30 = 0;
                decimal mesh4Red = 0;
                decimal mesh12Red = 0;
                decimal redFines = 0;

                if (!string.IsNullOrWhiteSpace(Mesh4))
                {
                    mesh4 = Decimal.Parse(Mesh4);
                    if(mesh4 > 0)
                    {
                        gradedStockList.Add(new GradedStock() { ID=1,Qty=mesh4});
                    }
                }

                if (!string.IsNullOrWhiteSpace(Mesh12))
                {
                    mesh12 = Decimal.Parse(Mesh12);
                    if (mesh12 > 0)
                    {
                        gradedStockList.Add(new GradedStock() { ID = 2, Qty = mesh12 });
                    }
                }

                if (!string.IsNullOrWhiteSpace(Mesh16))
                {
                    mesh16 = Decimal.Parse(Mesh16);
                    if (mesh16 > 0)
                    {
                        gradedStockList.Add(new GradedStock() { ID = 3, Qty = mesh16 });
                    }
                }

                if (!string.IsNullOrWhiteSpace(Mesh30))
                {
                    mesh30 = Decimal.Parse(Mesh30);
                    if (mesh30 > 0)
                    {
                        gradedStockList.Add(new GradedStock() { ID = 4, Qty = mesh30 });
                    }
                }

                if (!string.IsNullOrWhiteSpace(Mesh4Red))
                {
                    mesh4Red = Decimal.Parse(Mesh4Red);
                    if (mesh4Red > 0)
                    {
                        gradedStockList.Add(new GradedStock() { ID = 9, Qty = mesh4Red });
                    }
                }

                if (!string.IsNullOrWhiteSpace(Mesh12Red))
                {                                                                                                                                                                                                                                                                                                                                                                                                     
                    mesh12Red = Decimal.Parse(Mesh12Red);
                    if (mesh12Red > 0)
                    {
                        gradedStockList.Add(new GradedStock() { ID = 10, Qty = mesh12Red });
                    }
                }

                if (!string.IsNullOrWhiteSpace(RedFines))
                {
                    redFines = Decimal.Parse(RedFines);
                    if (redFines > 0)
                    {
                        gradedStockList.Add(new GradedStock() { ID = 11, Qty = redFines });
                    }
                }

                int res = DBAccess.InsertGradedStock(gradedStockList, sm.GetCurrentShift(), sm.GetCurrentDate(), true);
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

        //deals with button inputs and sorts out the display accordingly
        public void DigitButtonPress(string button)
        {
            Qty = string.Empty;
            bool deleted = false;

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
                    if (textboxType == "4mesh")
                    {
                        _qty = Mesh4;
                    }
                    else if (textboxType == "12mesh")
                    {
                        _qty = Mesh12;
                    }
                    else if (textboxType == "16mesh")
                    {
                        _qty = Mesh16;
                    }
                    else if (textboxType == "30mesh")
                    {
                        _qty = Mesh30;
                    }
                    else if (textboxType == "Red4mesh")
                    {
                        _qty = Mesh4Red;
                    }
                    else if (textboxType == "Red12mesh")
                    {
                        _qty = Mesh12Red;
                    }
                    else if (textboxType == "RedFines")
                    {
                        _qty = RedFines;
                    }
                    
                    if (_qty.Length > 1)
                    {
                        Qty = _qty.Substring(0, _qty.Length - 1);
                    }
                    else
                    {
                        Qty = "";
                    }

                    if (textboxType == "4mesh")
                    {
                        Mesh4 = Qty;
                    }
                    else if (textboxType == "12mesh")
                    {
                        Mesh12 = Qty;
                    }
                    else if (textboxType == "16mesh")
                    {
                        Mesh16 = Qty;
                    }
                    else if (textboxType == "30mesh")
                    {
                        Mesh30 = Qty;
                    }
                    else if (textboxType == "Red4mesh")
                    {
                        Mesh4Red = Qty;
                    }
                    else if (textboxType == "Red12mesh")
                    {
                        Mesh12Red = Qty;
                    }
                    else if (textboxType == "RedFines")
                    {
                        RedFines = Qty;
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
                if (textboxType == "4mesh")
                {
                    Mesh4 += Qty;
                }
                else if (textboxType == "12mesh")
                {
                    Mesh12 += Qty;
                }
                else if (textboxType == "16mesh")
                {
                    Mesh16 += Qty;
                }
                else if (textboxType == "30mesh")
                {
                    Mesh30 += Qty;
                }
                else if (textboxType == "Red4mesh")
                {
                    Mesh4Red += Qty;
                }
                else if (textboxType == "Red12mesh")
                {
                    Mesh12Red += Qty;
                }
                else if (textboxType == "RedFines")
                {
                    RedFines += Qty;
                }
            }

            newDisplayRequired = false;
        }
        
        public void GetWhichTextBox(string textbox)
        {
            switch (textbox)
            {
                case "4mesh":
                    textboxType = "4mesh";
                    break;
                case "12mesh":
                    textboxType = "12mesh";
                    break;
                case "16mesh":
                    textboxType = "16mesh";
                    break;
                case "30mesh":
                    textboxType = "30mesh";
                    break;
                case "Red4mesh":
                    textboxType = "Red4mesh";
                    break;
                case "Red12mesh":
                    textboxType = "Red12mesh";
                    break;
                case "RedFines":
                    textboxType = "RedFines";
                    break;
                default:
              
                    break;
            }
        }       

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        #region PUBLIC PROPERTIES

        private static bool CanDigitButtonPress(string button)
        {
            return true;
        }

        private static bool CanGradedButtonPress(string button)
        {
            return true;
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


        public string Mesh4
        {
            get { return _mesh4; }
            set { _mesh4 = value; RaisePropertyChanged(() => this.Mesh4); }
        }

        public string Mesh12
        {
            get { return _mesh12; }
            set { _mesh12 = value; RaisePropertyChanged(() => this.Mesh12); }
        }

        public string Mesh16
        {
            get { return _mesh16; }
            set { _mesh16 = value; RaisePropertyChanged(() => this.Mesh16); }
        }

        public string Mesh30
        {
            get { return _mesh30; }
            set { _mesh30 = value; RaisePropertyChanged(() => this.Mesh30); }
        }

        public string Mesh4Red
        {
            get { return _mesh4Red; }
            set { _mesh4Red = value; RaisePropertyChanged(() => this.Mesh4Red); }
        }

        public string Mesh12Red
        {
            get { return _mesh12Red; }
            set { _mesh12Red = value; RaisePropertyChanged(() => this.Mesh12Red); }
        }

        public string RedFines
        {
            get { return _redFines; }
            set { _redFines = value; RaisePropertyChanged(() => this.RedFines); }
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

        public ICommand GradedStockCommand
        {
            get
            {
                if (_gradedStockCommand == null)
                {
                    _gradedStockCommand = new DelegateCommand<string>(
                        GetWhichTextBox, CanGradedButtonPress);
                }
                return _gradedStockCommand;
            }
        }

        #endregion

    }

}
