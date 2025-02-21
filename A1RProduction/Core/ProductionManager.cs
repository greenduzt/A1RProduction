using A1QSystem.DB;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Grading;
using A1QSystem.Model.Production.Mixing;
using A1QSystem.Model.Production.Peeling;
using A1QSystem.Model.Production.ReRoll;
using A1QSystem.Model.Production.Slitting;
using A1QSystem.Model.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{
    public static class ProductionManager
    {
        

        //GRADING
        public static int AddToGrading(List<GradingOrder> gradingOrder)
        {
            GradingManager gm = new GradingManager();
            return gm.AddToGrading(gradingOrder);
        }

        //MIXING
        public static int AddToMixing(List<MixingOrder> mixingOrders, GradingProductionDetails gradingProductionDetails, int orderType, int currShift, Int64 currentProdTimeTable,
            List<GradingCompleted> ggList, string pcName, Int32 realProdTimeTableID, DateTime realDateTime, bool gradingCompleted)
        {
            MixingManager mm = new MixingManager();
            return mm.ProcessMixingOrder(mixingOrders, gradingProductionDetails, orderType, currShift, currentProdTimeTable, ggList, pcName, realProdTimeTableID, realDateTime, gradingCompleted);
        }

        //SLITTING
        public static int AddToSlitting(List<SlittingOrder> slittingOrder)
        {
            SlittingManager sm = new SlittingManager();
            return sm.ProcessSlittingOrder(slittingOrder);
            
        }
        //PEELING
        public static int AddToPeeling(List<PeelingOrder> peelingOrder)
        {
            PeelingManager pm = new PeelingManager();
            return pm.ProcessPeelingOrder(peelingOrder);
            
        }

        //RE-ROLLING
        public static int AddToReRolling(ReRollingOrder reRollingOrder)
        {
            ReRollingManager rrm = new ReRollingManager();
            return rrm.ProcessReRollingOrder(reRollingOrder);
        }

        //GRADED STOCK
        public static int AddToGradedStock(List<GradedStock> gradedStock)
        {
            return DBAccess.InsertGradedStock(gradedStock,0,DateTime.Now,false);
        }
       
    }
}
