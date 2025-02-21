
using A1QSystem.DB;
using A1QSystem.Model.Stock;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;

namespace A1QSystem.ViewModel.StockMaintenance
{
    public class EditStockMaintenanceViewModel
    {
        public int RawProductID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public decimal AQty { get; set; }
        public string Header { get; set; }
        public StockMaintenanceDetails StockMaintenanceDetails { get; set; }
        public event Action Closed;
        private DelegateCommand _closeCommand;
        private DelegateCommand _updateCommand;

        public EditStockMaintenanceViewModel(StockMaintenanceDetails stockMaintenanceDetails)
        {
            StockMaintenanceDetails = stockMaintenanceDetails;
            _closeCommand = new DelegateCommand(CloseForm);
            _updateCommand = new DelegateCommand(EditRawStock);           

            RawProductID = StockMaintenanceDetails.RawProduct.RawProductID;
            Code = StockMaintenanceDetails.RawProduct.RawProductCode;
            Description = StockMaintenanceDetails.RawProduct.Description;
            Type = StockMaintenanceDetails.RawProduct.RawProductType;
            AQty = StockMaintenanceDetails.RawStock.Qty;
            Header = StockMaintenanceDetails.RawProduct.Description;
        }

        public void EditRawStock()
        {
                int res = DBAccess.UpdateRawStock(this);
                if (res == 0)
                {
                    Msg.Show("There is a problem updating the record. Please try again later", "Update Failed", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                }

                CloseForm();
        }

       

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        #region COMMANDS

        public DelegateCommand UpdateCommand
        {
            get { return _updateCommand; }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        #endregion
    }
}
