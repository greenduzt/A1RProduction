using A1QSystem.Model.Formula;
using A1QSystem.Model.Orders;
using System.Collections.Generic;

namespace A1QSystem.Core
{
    public class CapacityChecker
    {
        public OrderDetails RawMaterialDetails;
        public List<Formulas> Formulas;

    //    public CapacityChecker(RawMaterialDetails rawMaterialDetails)
    //    {
    //        RawMaterialDetails = rawMaterialDetails;

    //        Formulas = DBAccess.GetFormulaDetailsByRawProdID(RawMaterialDetails.Product.RawProductID);

    //        //Console.WriteLine(RawMaterialDetails.RawID);
        
    //    }


    //    public void CheckGradedStock()
    //    {

    //        //Before doing anything else, delete all the records related to the row
    //        int res = DBAccess.ReAllocateToGradedStock(RawMaterialDetails.RawID);

    //        List<FormulaDetails> fdList = new List<FormulaDetails>();
    //        List<GradedStock> gsList = new List<GradedStock>();
    //        List<GradedStock> prodWeight = new List<GradedStock>();
    //        foreach (var item in Formulas)
    //        {
    //            fdList = GetGradingWeightDependency(item.RawProductID);

    //            foreach (var items in fdList)
    //            {
    //                gsList.Add(new GradedStock() { ID = items.CapacityID });
    //            }
    //        }
    //        if (gsList != null)
    //        {
    //            List<GradedStock> gradedStockCapacity = DBAccess.GetGradedStockByID(gsList);
                            
    //            foreach (var itemFDL in fdList)
    //            {
    //                foreach (var item in gradedStockCapacity)
    //                {
    //                    if (item.ID == itemFDL.CapacityID)
    //                    {
    //                        prodWeight.Add(new GradedStock() { RowID = RawMaterialDetails.RawID, ID = item.ID, Qty = CalReqKg(RawMaterialDetails.BlockLogQty, itemFDL.GradingWeight) });
    //                    }

    //                }
    //            }

    //            bool result = false;
    //            int r = 1;

    //            foreach (var itemGSC in gradedStockCapacity)
    //            {
    //                foreach (var itemPW in prodWeight)
    //                {
    //                    if (itemGSC.ID == itemPW.ID)
    //                    {
    //                        if (itemGSC.Qty >= itemPW.Qty)
    //                        {
    //                            if (r == 1)
    //                            {
    //                                result = true;
    //                            }
    //                            else
    //                            {
    //                                result = false;
    //                            }
    //                        }
    //                        else
    //                        {
    //                            result = false;
    //                            r = 0;
    //                        }
    //                    }
    //                }
    //            }

    //            if (result == true)//Grading capacity available
    //            {
    //                DBAccess.UpdateGradedStock(prodWeight);
    //            }
    //            else//No Grading capacity available
    //            {
    //                Console.WriteLine("No Grading capacity available");
    //            }
    //        }
    //    }

    //    public void ReAllocateToGradedStock(string rawId)
    //    {
    //        int res = DBAccess.ReAllocateToGradedStock(rawId);
    //        if (res != 0)
    //        {
    //            Console.WriteLine("Re Alocated to Graded Stock");
    //        }
    //    }

    //    private decimal CalReqKg(decimal amount, decimal gradingWeight)
    //    {
    //        return decimal.Floor(amount * gradingWeight);
    //    }

    //    public void CalculateMixingCapacity()
    //    {

    //    }


    //    private List<FormulaDetails> GetGradingWeightDependency(int id)
    //    {

    //        List<FormulaDetails> depList = new List<FormulaDetails>();

    //        List<Formulas> fList = DBAccess.GetFormulaDetailsByRawProdID(id);
    //        foreach (var item in fList)
    //        {
    //            if (item.ProductCapacity1 != 0 && item.GradingWeight1 != 0)
    //            {
    //                depList.Add(new FormulaDetails() { CapacityID = item.ProductCapacity1, GradingWeight = item.GradingWeight1 });
    //            }

    //            if (item.ProductCapacity2 != 0 && item.GradingWeight2 != 0)
    //            {
    //                depList.Add(new FormulaDetails() { CapacityID = item.ProductCapacity2, GradingWeight = item.GradingWeight2 });
    //            }
    //        }
    //        return depList;
    //    }
    }
}
