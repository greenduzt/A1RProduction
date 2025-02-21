using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Formula;
using A1QSystem.Model.RawMaterials;
using A1QSystem.Model.Stock;
using A1QSystem.Model.Transaction;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.ViewModel.Orders
{
    public class ConvertOrderViewModel : ViewModelBase
    {
        public string RawProductCode { get; set; }
        private decimal _qty;
        public RawProductionDetails RawProductionDetails { get; set; }
        public List<GradedStock> GradedStock { get; set; }
        public event Action Closed;
        private DelegateCommand _closeCommand;
        private DelegateCommand _convertCommand;

        public ConvertOrderViewModel(RawProductionDetails rawProductionDetails)
        {
            RawProductionDetails = rawProductionDetails;
            Qty = RawProductionDetails.BlockLogQty;

            RawProductCode = RawProductionDetails.RawProduct.RawProductCode;

            _closeCommand = new DelegateCommand(CloseForm);
            _convertCommand = new DelegateCommand(ConvertOrder);

            List<GradedStock> gsl = DBAccess.GetGradedStock();
            GradedStock = new List<GradedStock>();
            foreach (var item in gsl)
            {
                if (item.ID != 5)
                {
                    GradedStock gs = new GradedStock();
                    gs.GradeName = item.GradeName;
                    gs.Qty = Math.Round(item.Qty);
                    GradedStock.Add(gs);
                }
            }
        }

        private void ConvertOrder()
        { 
            if (Qty > RawProductionDetails.BlockLogQty)
            {
                Msg.Show("Quantity must be less than or equal to " + RawProductionDetails.BlockLogQty, "Enter Correct Quantity", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (Qty <= 0)
            {
                Msg.Show("Invalid quantity ", "Enter Correct Quantity", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (RawProductionDetails.OrderType == 4)
            {
                Msg.Show("This order is already a Graded Order", "Cannot Update to Graded Order", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
             else if (RawProductionDetails.ActiveOrder == true)
             {
                 Msg.Show("This order is being currently processed and cannot be shifted ", "Cannot Update to Graded Order", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
             }
             else
             {
                 decimal kg1 = 0;
                 decimal kg2 = 0;                
                 decimal blkPossible1 = 0;
                 decimal blkPossible2 = 0;
                 decimal kgReq1 = 0;
                 decimal kgReq2 = 0;
                 bool convertNow = false;

                 List<Formulas> fList = DBAccess.GetFormulaDetailsByRawProdID(RawProductionDetails.RawProduct.RawProductID);
                 List<GradedStock> gsList = new List<GradedStock>();
                
                 if (fList.Count > 0)
                 {
                     if (fList[0].ProductCapacity1 > 0 )
                     {
                         kg1 = Qty * fList[0].GradingWeight1;
                         gsList.Add(new GradedStock() { ID = fList[0].ProductCapacity1,Qty = kg1});
                         if(fList[0].ProductCapacity2 > 0)
                         {
                             kg2 = Qty * fList[0].GradingWeight2;
                             gsList.Add(new GradedStock() { ID = fList[0].ProductCapacity2,Qty = kg2 });
                         }

                         List<GradedStock> gradedStock = DBAccess.GetGradedStockByID(gsList);

                         var grading1 = gradedStock.SingleOrDefault(c => c.ID == fList[0].ProductCapacity1);//ProductCapacity1
                         var grading2 = gradedStock.SingleOrDefault(c => c.ID == fList[0].ProductCapacity2);//ProductCapacity2  
        
                         if(grading1 != null)
                         {
                             if( kg1 <= grading1.Qty)
                             {
                                 blkPossible1 = decimal.Floor(kg1 / fList[0].GradingWeight1);
                             }
                             else
                             {
                                 blkPossible1 = decimal.Floor(grading1.Qty / fList[0].GradingWeight1);
                             }

                             if (grading2 != null)
                             {
                                 if (kg2 <= grading2.Qty)
                                 {
                                     blkPossible2 = decimal.Floor(kg2 / fList[0].GradingWeight2);
                                 }
                                 else
                                 {
                                     blkPossible2 = decimal.Floor(grading2.Qty / fList[0].GradingWeight2);
                                 }

                                     if(blkPossible1 > blkPossible2)
                                     {
                                         blkPossible1 = blkPossible2;
                                     }
                                     else if (blkPossible1 <= blkPossible2)
                                     {
                                         blkPossible2 = blkPossible1;
                                     }                                
                             }

                             if(blkPossible1 >0)
                             {
                                 kgReq1 = blkPossible1 * fList[0].GradingWeight1;
                             }

                             if (blkPossible2 > 0)
                             {
                                 kgReq2 = blkPossible2 * fList[0].GradingWeight2;
                             }

                             if (blkPossible1 < Qty && blkPossible1 != 0)
                             {
                                 if (Msg.Show("The graded stock is Insufficient to complete all the " + Qty + " " + RawProductionDetails.RawProduct.RawProductType + PluSin(Qty) + System.Environment.NewLine + "But, " + blkPossible1 + " " + RawProductionDetails.RawProduct.RawProductType + PluSin(blkPossible1) + " can be converted!" + System.Environment.NewLine + "Do you want to make this " + blkPossible1 + " " + RawProductionDetails.RawProduct.RawProductType + PluSin(blkPossible1) + " from graded stock?", "Insufficient Graded Stock", MsgBoxButtons.YesNo, MsgBoxImage.Error, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                                 {
                                     Qty = blkPossible1;
                                     convertNow = true;
                                     CloseForm();
                                 }
                             }
                             else if (blkPossible1 == 0)
                             {
                                 Msg.Show("The graded stock is insufficient to complete this order!", "Insufficient Graded Stock", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);                                 
                             }
                             else
                             {
                                 convertNow = true;
                                 CloseForm();
                             }
                             if (convertNow)
                             {

                                 List<GradedStock> gradedStockList = new List<GradedStock>();
                                 if (blkPossible1 > 0)
                                 {
                                     gradedStockList.Add(new GradedStock() { ID = fList[0].ProductCapacity1, Qty = blkPossible1 * fList[0].GradingWeight1 });
                                 }

                                 if (blkPossible2 > 0)
                                 {
                                     gradedStockList.Add(new GradedStock() { ID = fList[0].ProductCapacity2, Qty = blkPossible2 * fList[0].GradingWeight2 });
                                 }

                                int z= DBAccess.ConvertOrder(this,gradedStockList);
                                if(z>0)
                                {
                                    /*********TRANSACTION********/
                                    string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                    if (String.IsNullOrEmpty(userName))
                                    {
                                        userName = "Unknown";
                                    }

                                    Transaction transaction = null;
                                    transaction = new Transaction()
                                    {
                                        TransDateTime = DateTime.Now,
                                        Transtype = "Converted To Graded",
                                        SalesOrderID = RawProductionDetails.SalesOrderId,
                                        Products = new List<RawStock>()
                                            {
                                                new RawStock(){RawProductID = RawProductionDetails.RawProduct.RawProductID,Qty=Qty},  
                                            },
                                        CreatedBy = userName
                                    };
                                    DBAccess.InsertTransaction(transaction);
                                 }
                             }
                         }
                     }
                 }
             }
        }

        private string PluSin(decimal q)
        {
            string str = string.Empty;

            if (q > 1)
            {
                str = "s";
            }else
            {
                str = string.Empty;
            }
            return str;
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        #region PUBLIC PROPERTIES

        public decimal Qty
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

        public DelegateCommand ConvertCommand
        {
            get { return _convertCommand; }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        #endregion

    }
}
