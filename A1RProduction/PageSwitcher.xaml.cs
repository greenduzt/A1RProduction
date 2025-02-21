using A1QSystem;
using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Machine;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Production.Slitting;
using A1QSystem.View;
using A1QSystem.View.CuringProducts;
using A1QSystem.View.Customers;
using A1QSystem.View.Dashboard;
using A1QSystem.View.Graded_Stock;
using A1QSystem.View.Loading;
using A1QSystem.View.Machine.MachineHistory;
using A1QSystem.View.Machine.MachineWorkOrders;
using A1QSystem.View.Maintenance;
using A1QSystem.View.Manufacturing;
using A1QSystem.View.Manufacturing.RubberFormula;
using A1QSystem.View.Orders;
using A1QSystem.View.Orders.Status;
using A1QSystem.View.Production;
using A1QSystem.View.Production.Grading;
using A1QSystem.View.Production.Mixing;
using A1QSystem.View.Production.Peeling;
using A1QSystem.View.Production.ReRolling;
using A1QSystem.View.Production.SlitPeel;
using A1QSystem.View.Production.Slitting;
using A1QSystem.View.Production.WeeklyScheduleFull;
using A1QSystem.View.Products;
using A1QSystem.View.Quoting;
using A1QSystem.View.Sales;
using A1QSystem.View.Stock.BlockLogStock;
using A1QSystem.View.StockMaintenance;
using A1QSystem.View.WorkOrders;
using A1QSystem.ViewModel.Orders;
using A1QSystem.ViewModel.PageSwitcher;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace A1QSystem
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PageSwitcher : Window
    {

        public PageSwitcher()
        {
            InitializeComponent();

          
            TopContentViewModel tcvm = new TopContentViewModel();
            DataContext = tcvm;
            Switcher.pageSwitcher = this;

            string userName=string.Empty;
            string state=string.Empty;
            List<UserPrivilages> priv = null ;
            List<MetaData> metaData = new List<MetaData>();
            metaData.Add(new MetaData() { ID = 1, KeyName = "version", Description = "A1 Rubber Console V1.7.3 2017 TEST" });
            metaData.Add(new MetaData() { ID = 2, KeyName = "grades_no_show", Description = "69,105" });

            //Switcher.Switch(new VehicleWorkOrdersView(userName, state, priv, metaData));
            //Switcher.Switch(new GradingScheduleView("Chamara Walaliyadde", "QLD", priv, metaData));
            //Switcher.Switch(new MixingScheduleView(userName, state, priv, metaData));
            //Switcher.Switch(new MixingHistoryView(userName, state, priv, metaData));
            //Switcher.Switch(new MachineWorkOrdersView(userName, state, priv, metaData));
            //Switcher.Switch(new MachineWorkOrderHistoryView(userName, state, priv, metaData));

            //Switcher.Switch(new GradingHistoryView("Chamara Walaliyadde", "QLD", priv, metaData));
            //Switcher.Switch(new StockMaintenanceView(userName, state, priv));
            //Switcher.Switch(new PeelingSchedulerView(userName, state, priv));
            //Switcher.Switch(new SlitingScheduleView(userName, state, priv));
            //Switcher.Switch(new SlitPeelProductionSchedulerView(userName, state, priv));
            //Switcher.Switch(new ProductionSchedulerView(userName, state, priv, metaData));
            //Switcher.Switch(new MixingProductionSchedulerView(userName, state, priv));

            //Switcher.Switch(new OrderPendingView("Chamara Walaliyadde", "QLD", priv));
            //Switcher.Switch(new QuoteToSaleView("Chamara Walaliyadde", "QLD", priv));
            //Switcher.Switch(new SalesMenuView("Chamara Walaliyadde", "QLD", priv));
            //Switcher.Switch(new ProductionMaintenanceView("Chamara Walaliyadde", "QLD", priv, metaData));
            //Switcher.Switch(new MainMenu("Chamara Walaliyadde", "QLD", priv, metaData));
            //Switcher.Switch(new ScehduleMachineWorkOrdersView("Chamara Walaliyadde", "QLD", priv, metaData));
            //Switcher.Switch(new DayShiftView("Large Mixer","Chamara Walaliyadde", "QLD"));
            //Switcher.Switch(new QuoteV("Chamara Walaliyadde", "QLD"));
            //Switcher.Switch(new OrdersMainMenuView("Chamara Walaliyadde", "QLD", priv));
            //Switcher.Switch(new PickingOrderView("Chamara Walaliyadde", "QLD", priv));
            //Switcher.Switch(new AddProductionOrderView("Chamara Walaliyadde", "QLD", priv, metaData));           
            //Switcher.Switch(new ViewGradedStockView("Chamara Walaliyadde", "QLD", priv));  
            //Switcher.Switch(new FlatBenchSlitterView("Chamara Walaliyadde", "QLD", priv));  
            //Switcher.Switch(new CuringView("Chamara Walaliyadde", "QLD", priv, metaData));  
            //Switcher.Switch(new CarouselSlitterView("Chamara Walaliyadde", "QLD", priv));  
            //Switcher.Switch(new PeelingOrdersView("Chamara Walaliyadde", "QLD", priv));
            //Switcher.Switch(new PeelingHistoryView("Chamara Walaliyadde", "QLD", priv));
            //Switcher.Switch(new ReRollingOrdersView("Chamara Walaliyadde", "QLD", priv));  
            //Switcher.Switch(new AmendOrderView("Chamara Walaliyadde", "QLD", priv));
            //Switcher.Switch(new BlockLogStockView("Chamara Walaliyadde", "QLD", priv));  
            //Switcher.Switch(new OrderStatusView("Chamara Walaliyadde", "QLD", priv));  
            //Switcher.Switch(new ReRollingHistoryView("Chamara Walaliyadde", "QLD", priv));  
            //Switcher.Switch(new SlittingHistoryView(userName, state, priv));
            // SearchProductByName cw = new SearchProductByName();
            // cw.Show();

            // Switcher.Switch(new QuotingMainMenu("Chamara Walaliyadde", "QLD"));  

            //Switcher.Switch(new OrderProductionSchedularView());
            //Switcher.Switch(new TempOrderProductionSchedulerView()); 
            //Switcher.Switch(new NewQuoteView("Chamara Walaliyadde", "QLD", priv));  
            //Switcher.Switch(new UpdateQuoteView("Chamara Walaliyadde", "QLD", priv));  
            //Switcher.Switch(new UpdateProductView("Chamara Walaliyadde", "QLD", priv));
            //Switcher.Switch(new RubberFormulaView("Chamara Walaliyadde", "QLD", priv));  
            //Switcher.Switch(new WorkStationsView("Chamara Walaliyadde", "QLD", priv, metaData));  
            //Switcher.Switch(new WeeklyScheduleFullView("Chamara Walaliyadde", "QLD", priv, metaData));  
            //Switcher.Switch(new LoadingScreen());
            
            Switcher.Switch(new LoginView(TopContent));
            childWindow.DataContext = ChildWindowManager.Instance;
        }

        public void Navigate(UserControl nextPage)
        {
            this.MainContent.Content = nextPage;         
        }


        public void Navigate(UserControl nextPage, object state)
        {
            this.Content = nextPage;
            ISwitchable s = nextPage as ISwitchable;

            if (s != null)
                s.UtilizeState(state);
            else
                throw new ArgumentException("NextPage is not ISwitchable! "
                  + nextPage.Name.ToString());
        }


        /// <summary>
        /// TitleBar_MouseDown - Drag if single-click, resize if double-click
        /// </summary>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                if (e.ClickCount == 2)
                {
                    AdjustWindowSize();
                }
                else
                {
                    Application.Current.MainWindow.DragMove();
                }
        }

       

        /// <summary>
        /// MaximizedButton_Clicked
        /// </summary>
        private void Maximize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AdjustWindowSize();
        }

        /// <summary>
        /// Minimized Button_Clicked
        /// </summary>
        private void Minimize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Adjusts the WindowSize to correct parameters when Maximize button is clicked
        /// </summary>
        private void AdjustWindowSize()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                //MaximizeButton.Content = "1";
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                //MaximizeButton.Content = "2";
            }

        }
    }
}
