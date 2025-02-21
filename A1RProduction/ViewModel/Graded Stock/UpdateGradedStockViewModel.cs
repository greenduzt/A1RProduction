using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Stock;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.ViewModel.Graded_Stock
{
    public class UpdateGradedStockViewModel : ViewModelBase
    {
        private decimal _mesh4;
        private decimal _mesh12;
        private decimal _mesh16;
        private decimal _mesh30;
        private decimal _regrind;
        private decimal _boxed;
        public event Action Closed;
        private DelegateCommand _closeCommand;
        private DelegateCommand _submitCommand;

        public UpdateGradedStockViewModel()
        {
            _closeCommand = new DelegateCommand(CloseForm);
            _submitCommand = new DelegateCommand(UpdateGradedStock);

            List<GradedStock> gradedStock = DBAccess.GetGradedStock();

            foreach (var item in gradedStock)
            {
                if (item.ID == 1)
                {
                    Mesh4 = Math.Ceiling(item.Qty);
                }
                if (item.ID == 2)
                {
                    Mesh12 = Math.Ceiling(item.Qty);
                }
                if (item.ID == 3)
                {
                    Mesh16 = Math.Ceiling(item.Qty);
                }
                if (item.ID == 4)
                {
                    Mesh30 = Math.Ceiling(item.Qty);
                }
                if (item.ID == 6)
                {
                    Regrind = Math.Ceiling(item.Qty);
                }
            }

        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        private void UpdateGradedStock()
        {
          if (Msg.Show("Are you sure you want to update graded stock?", "Updating Graded Stock Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Information, MsgBoxResult.Yes) == MsgBoxResult.Yes)
          {
              List<GradedStock> gradedStockList = new List<GradedStock>();
             
                  GradedStock gradedStock1 = new GradedStock();
                  gradedStock1.ID = 1;
                  gradedStock1.Qty = Mesh4;
                  gradedStockList.Add(gradedStock1);
              
                  GradedStock gradedStock2 = new GradedStock();
                  gradedStock2.ID = 2;
                  gradedStock2.Qty = Mesh12;
                  gradedStockList.Add(gradedStock2);
             
                  GradedStock gradedStock3 = new GradedStock();
                  gradedStock3.ID = 3;
                  gradedStock3.Qty = Mesh16;
                  gradedStockList.Add(gradedStock3);
             
                  GradedStock gradedStock4 = new GradedStock();
                  gradedStock4.ID = 4;
                  gradedStock4.Qty = Mesh30;
                  gradedStockList.Add(gradedStock4);
             
                  GradedStock gradedStock5 = new GradedStock();
                  gradedStock5.ID = 6;
                  gradedStock5.Qty = Regrind;
                  gradedStockList.Add(gradedStock5);
              
                  GradedStock gradedStock6 = new GradedStock();
                  gradedStock6.ID = 7;
                  gradedStock6.Qty = Boxed;
                  gradedStockList.Add(gradedStock6);             
             
              if(gradedStockList.Count > 0)
              {
                  int res = DBAccess.UpdateGradedStock2(gradedStockList);
                  if(res > 0)
                  {
                      Msg.Show("Graded stock was updated successfully ", "Graded Stock Updated", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);
                  }
                  else
                  {
                      Msg.Show("Something went wrong and the Graded Stock wasn't updated successfully ", "Failed To Update Graded Stock", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                  }
                  CloseForm();
              }
          }
        }

        #region PUBLIC PROPERTIES

        public decimal Mesh4
        {
            get { return _mesh4; }
            set { _mesh4 = value; RaisePropertyChanged(() => this.Mesh4); }
        }

        public decimal Mesh12
        {
            get { return _mesh12; }
            set { _mesh12 = value; RaisePropertyChanged(() => this.Mesh12); }
        }

        public decimal Mesh16
        {
            get { return _mesh16; }
            set { _mesh16 = value; RaisePropertyChanged(() => this.Mesh16); }
        }

        public decimal Mesh30
        {
            get { return _mesh30; }
            set { _mesh30 = value; RaisePropertyChanged(() => this.Mesh30); }
        }

        public decimal Regrind
        {
            get { return _regrind; }
            set { _regrind = value; RaisePropertyChanged(() => this.Regrind); }
        }

        public decimal Boxed
        {
            get { return _boxed; }
            set { _boxed = value; RaisePropertyChanged(() => this.Boxed); }
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

        #endregion
    }
}
