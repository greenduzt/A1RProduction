using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Production.Slitting;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Productions.Slitting
{
    public class OffSpecReportDatePickerViewModel : ViewModelBase
    {
        private DateTime _toDate;
        private DateTime _fromDate;
        private DateTime _currentDate;
        private bool canExecute;
        private ICommand _printCommand;
        private DelegateCommand _closeCommand;

        public event Action Closed;

        public OffSpecReportDatePickerViewModel()
        {
            CurrentDate = DateTime.Now;
            FromDate = CurrentDate;
            ToDate = CurrentDate;
            canExecute = true;
            _closeCommand = new DelegateCommand(CloseForm);
        }

        private void PrintReport()
        {
            List<OffSpecDetails> OffSpecDetails = new List<OffSpecDetails>();

            OffSpecDetails=DBAccess.GetOffSpecDetails(FromDate,ToDate);

            if(OffSpecDetails.Count == 0)
            {
                Msg.Show("No information found for the specified dates", "No Information Availablle", MsgBoxButtons.OK, MsgBoxImage.Information_Orange);
            }
            else
            {
                Exception exception = null;
                BackgroundWorker worker = new BackgroundWorker();
                ChildWindowView LoadingScreen = new ChildWindowView();
                LoadingScreen.ShowWaitingScreen("Printing");

                worker.DoWork += (_, __) =>
                {
                    OffspecReportPDF ospdf = new OffspecReportPDF(OffSpecDetails,FromDate,ToDate);
                    exception = ospdf.createWorkOrderPDF();
                };

                worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                {
                    LoadingScreen.CloseWaitingScreen();
                    if (exception != null)
                    {
                        Msg.Show("A problem has occured while prining. Please try again later." + System.Environment.NewLine + exception, "Printing Error", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                    }
                    CloseForm();
                };
                worker.RunWorkerAsync();
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        public DateTime CurrentDate
        {
            get
            {
                return _currentDate;
            }
            set
            {
                _currentDate = value;
                RaisePropertyChanged(() => this.CurrentDate);
            }
        }

        public DateTime FromDate
        {
            get
            {
                return _fromDate;
            }
            set
            {
                _fromDate = value;
                RaisePropertyChanged(() => this.FromDate);
            }
        }

        public DateTime ToDate
        {
            get
            {
                return _toDate;
            }
            set
            {
                _toDate = value;
                RaisePropertyChanged(() => this.ToDate);
            }
        }

        public ICommand PrintCommand
        {
            get
            {
                return _printCommand ?? (_printCommand = new A1QSystem.Commands.LogOutCommandHandler(() => PrintReport(), canExecute));
            }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }
    }
}
