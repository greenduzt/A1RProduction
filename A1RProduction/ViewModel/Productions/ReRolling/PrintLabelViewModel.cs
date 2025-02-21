using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Production.ReRoll;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using MsgBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Productions.ReRolling
{
    public class PrintLabelViewModel : ViewModelBase
    {
        private string _selectedNoOfPrints;
        private ReRollingOrder reRollingProduction;
        private bool _printEnable;
        private ChildWindowView loadingScreen;
        public event Action Closed;
        private ICommand _closeCommand;
        private ICommand _printLabelCommand;

        public PrintLabelViewModel(ReRollingOrder rrp)
        {
            reRollingProduction = rrp;
            SelectedNoOfPrints = "0";
            PrintEnable = false;
        }

        private void PrintLabel()
        {
            if (Convert.ToInt16(SelectedNoOfPrints) > 0)
            {
                Exception res = null;
                CloseForm();
                BackgroundWorker worker = new BackgroundWorker();
                loadingScreen = new ChildWindowView();
                loadingScreen.ShowWaitingScreen("Printing");

                worker.DoWork += (_, __) =>
                {
                    PrintLabelsPDF pl = new PrintLabelsPDF(reRollingProduction, Convert.ToInt16(SelectedNoOfPrints));
                    res = pl.CreateLabel();
                };
                worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                {
                    loadingScreen.CloseWaitingScreen();
                    if (res != null)
                    {
                        //DBAccess.InsertErrorLog(DateTime.Now + " Printing label error " + res.ToString());
                        Msg.Show("A problem has occured and the label did not create properly. Please try again later.", "Printing Error", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                    }
                };
                worker.RunWorkerAsync();
            }
            else
            {
                Msg.Show("Please select the number of print outs from the list ", "Selection Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }
        #region PUBLIC_PROPERTIES



        public bool PrintEnable
        {
            get { return _printEnable; }
            set { _printEnable = value; RaisePropertyChanged(() => this.PrintEnable); }
        }
        public string SelectedNoOfPrints
        {
            get { return _selectedNoOfPrints; }
            set
            {
                _selectedNoOfPrints = value; RaisePropertyChanged(() => this.SelectedNoOfPrints);
                if (Convert.ToInt16(SelectedNoOfPrints) > 0)
                {
                    PrintEnable = true;
                }
                else
                {
                    PrintEnable = false;
                }
            }
        }

        #endregion

        #region COMMANDS

        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new LogOutCommandHandler(() => CloseForm(), true));
            }
        }

        public ICommand PrintLabelCommand
        {
            get
            {
                return _printLabelCommand ?? (_printLabelCommand = new LogOutCommandHandler(() => PrintLabel(), true));
            }
        }

        #endregion
    }
}
