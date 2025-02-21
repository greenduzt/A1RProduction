using A1QSystem.View.Formula;
using A1QSystem.View.Loading;
using A1QSystem.ViewModel;
using A1QSystem.ViewModel.Formula;
using A1QSystem.ViewModel.Loading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{
    public class ChildWindow : BaseViewModel
    {
        public event Action openWaitingScreen_Closed;
        public event Action formulaScreen_Closed;
      

        ///*****************Showing Loading Screen******************/
        public void ShowWaitingScreen(string msg)
        {
            LoadingScreenViewModel lsvm = new LoadingScreenViewModel(msg);
            lsvm.Closed += CloseWaitingScreen;
            ChildWindowManager.Instance.ShowChildWindow(new LoadingScreen(msg) { DataContext = lsvm });
        }

        public void CloseWaitingScreen()
        {
            if (openWaitingScreen_Closed != null)
                openWaitingScreen_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }
       

        ///*****************Show Formula******************/
        public void ShowFormula(string formula)
        {
            ShowFormulaViewModel sfvm = new ShowFormulaViewModel(formula);
            sfvm.Closed += CloseFormulaScreen;
            ChildWindowManager.Instance.ShowChildWindow(new ShowFormulaView(formula) { DataContext = sfvm });
        }

        public void CloseFormulaScreen()
        {
            if (formulaScreen_Closed != null)
                formulaScreen_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

     
    }
}