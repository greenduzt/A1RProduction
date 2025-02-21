using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Machine;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Production.Peeling;
using A1QSystem.Model.Production.ReRoll;
using A1QSystem.Model.Production.SlitingPeeling;
using A1QSystem.Model.Production.Slitting;
using A1QSystem.Model.RawMaterials;
using A1QSystem.Model.Stock;
using A1QSystem.Model.Users;
using A1QSystem.Model.Vehicles;
using A1QSystem.View.Comments;
using A1QSystem.View.Customers;
using A1QSystem.View.Formula;
using A1QSystem.View.Graded_Stock;
using A1QSystem.View.Loading;
using A1QSystem.View.Machine;
using A1QSystem.View.Machine.MachineHistory;
using A1QSystem.View.Machine.MachineWorkOrders;
using A1QSystem.View.Orders;
using A1QSystem.View.Production;
using A1QSystem.View.Production.Grading;
using A1QSystem.View.Production.Mixing;
using A1QSystem.View.Production.Peeling;
using A1QSystem.View.Production.ReRolling;
using A1QSystem.View.Production.SlitPeel;
using A1QSystem.View.Production.Slitting;
using A1QSystem.View.Sales;
using A1QSystem.View.StockMaintenance;
using A1QSystem.View.Vehicles;
using A1QSystem.View.VehicleWorkOrders;
using A1QSystem.View.VehicleWorkOrders.History;
using A1QSystem.View.WorkOrders;
using A1QSystem.ViewModel;
using A1QSystem.ViewModel.Comments;
using A1QSystem.ViewModel.Formula;
using A1QSystem.ViewModel.Graded_Stock;
using A1QSystem.ViewModel.Loading;
using A1QSystem.ViewModel.Machine;
using A1QSystem.ViewModel.Machine.MachineHistory;
using A1QSystem.ViewModel.Machine.MachineWorkOrders;
using A1QSystem.ViewModel.Orders;
using A1QSystem.ViewModel.Productions;
using A1QSystem.ViewModel.Productions.Grading;
using A1QSystem.ViewModel.Productions.Mixing;
using A1QSystem.ViewModel.Productions.Peeling;
using A1QSystem.ViewModel.Productions.ReRolling;
using A1QSystem.ViewModel.Productions.SlitPeel;
using A1QSystem.ViewModel.Productions.Slitting;
using A1QSystem.ViewModel.Sales;
using A1QSystem.ViewModel.StockMaintenance;
using A1QSystem.ViewModel.Vehicles;
using A1QSystem.ViewModel.VehicleWorkOrders;
using A1QSystem.ViewModel.VehicleWorkOrders.History;
using A1QSystem.ViewModel.WorkOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.View
{
    public class ChildWindowView : BaseViewModel
    {
        public event Action<Customer> customer_Closed;
        public event Action<A1QSystem.Model.Freight> freight_Closed;
        public event Action<OrderSchedule> orderProduction_Closed;
        public event Action<QuoteDetails> quoteDetails_Closed;
        public event Action quoteApproval_Closed;
        public event Action comments_Closed;
        public event Action shiftOrder_Closed;
        public event Action conOrder_Closed;
        public event Action shiftSlitPeel_Closed;
        public event Action<int> editRawStock_Closed;
        public event Action slitPeelConfirmation_Closed;
        public event Action peelingConfirmation_Closed;
        public event Action addToGradedStock_Closed;
        public event Action mixingWeeklySchedule_Closed;
        public event Action mixingUnfinished_Closed;
        public event Action openWaitingScreen_Closed;
        public event Action formulaScreen_Closed;
        public event Action printLabels_Closed;
        public event Action reRollingConfirmation_Closed;
        public event Action updateGradedStock_Closed;
        public event Action<int> newVehicleWorkOrder_Closed;
        public event Action<int> updateVehicleWorkOrder_Closed;
        public event Action vehicleWorkOrderItems_Closed;
        public event Action<int> completeWorkOrder_Closed;
        public event Action<MaintenanceAcceptanceViewModel> maintenanceAcceptance_Closed;
        public event Action<Int32> innerVehicleRepairWorkOrder_Closed;
        public event Action vehicleWorkOrderHistoryItems_Closed;
        public event Action addNewVehicle_Closed;
        public event Action updateVehicle_Closed;
        public event Action<int> vehicleMaintDes_Closed;
        public event Action<int> machineMaintDes_Closed;
        public event Action<int> machineWorkOrderItems_Closed;
        public event Action<SetMechanicViewModel> setMechanicViewModel_Closed;
        public event Action<UserPosition> setMechanicGroupViewModel_Closed;
        public event Action<int> completeMachineWorkOrder_Closed;
        public event Action<int> machineWorkOrderHistoryItems_Closed;
        public event Action addNewMachine_Closed;
        public event Action updateMachine_Closed;
        public event Action<MaintenanceDescriptionsViewModel> viewVehicleMaintenanceDesc_Closed;
        public event Action viewVehicleRepairDesc_Closed;
        public event Action<int> completeVehicleRepairDesc_Closed;
        public event Action<Int64> odometerAcceptance_Closed;
        public event Action addToRegrindStock_Closed;
        public event Action offSpecReport_Closed;
        public event Action shiftSlittingOrder_Closed;
        public event Action IBCChangeView_Closed;
        public event Action providerScreenClosed;
        public event Action<int> fileUploadScreenClosed;
        public event Action<int> uploadedFilesClosed;

        public ChildWindowView() { }

        public void Show(int personId)
        {
            AddCustomerViewModel vm = new AddCustomerViewModel(personId);
            vm.Closed += ChildWindow_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new AddCustomer() { DataContext = vm });
        }

        public void ShowFreight(int personId)
        {
            AddFreightViewModel vm = new AddFreightViewModel(personId);
            vm.Closed += ShowFreightWindow_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new A1QSystem.View.Freight.AddFreight() { DataContext = vm });
        }

        void ChildWindow_Closed(Customer customer)
        {
            if (customer_Closed != null)
                customer_Closed(customer);
            ChildWindowManager.Instance.CloseChildWindow();
        }

        void ShowFreightWindow_Closed(A1QSystem.Model.Freight freight)
        {
            if (freight_Closed != null)
                freight_Closed(freight);
            ChildWindowManager.Instance.CloseChildWindow();
        }


        /********************Edit Production Order Details********************/
        public void ShowOrderProductionDetails(int OrderProductionID, int orderNo)
        {
            OrderProductionEditViewModel opevm = new OrderProductionEditViewModel(OrderProductionID, orderNo);
            opevm.Closed += ShowOrderProductionDetails_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new OrderProductionEditView(OrderProductionID, orderNo) { DataContext = opevm });
        }
        void ShowOrderProductionDetails_Closed(OrderSchedule orderProduction)
        {
            if (orderProduction_Closed != null)
                orderProduction_Closed(orderProduction);
            ChildWindowManager.Instance.CloseChildWindow();
        }

        /********************Edit Quote Details********************/
        public void ShowEditQuoteDetails(int QuoteNo, string UserName, string State)
        {
            EditQuoteDetailsPopUpViewModel eqdpvm = new EditQuoteDetailsPopUpViewModel(QuoteNo, UserName, State);
            eqdpvm.Closed += ShowEditQuoteDetails_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new EditQuoteDetailsPopUpView(QuoteNo, UserName, State) { DataContext = eqdpvm });
        }
        void ShowEditQuoteDetails_Closed(QuoteDetails quoteDetails)
        {
            if (quoteDetails_Closed != null)
                quoteDetails_Closed(quoteDetails);
            ChildWindowManager.Instance.CloseChildWindow();
        }

        /*********************Quote to sale *********************/
        public void ShowQuoteToSaleApproval(int QuoteNo, string State)
        {
            QuoteToSaleApprovalPopUpViewModel qtspvm = new QuoteToSaleApprovalPopUpViewModel(QuoteNo, State);
            qtspvm.Closed += QuoteToSaleApproval_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new QuoteToSaleApprovalPopUpView(QuoteNo, State) { DataContext = qtspvm });
        }
        void QuoteToSaleApproval_Closed()
        {
            if (quoteApproval_Closed != null)
                quoteApproval_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        /*********************Comments *********************/
        public void ShowComments(int QuoteNo, string userName, string State)
        {
            CommentsViewModel comm = new CommentsViewModel(QuoteNo, userName, State);
            comm.Closed += CommentsWindow_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new CommentsView(QuoteNo, userName, State) { DataContext = comm });
        }
        void CommentsWindow_Closed()
        {
            if (comments_Closed != null)
                comments_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        /*****************Shifting Orders******************/

        public void ShowShiftOrderWindow(RawProductionDetails rawProductionDetails, string type)
        {
            ShiftProductionViewModel shiftOrderVM = new ShiftProductionViewModel(rawProductionDetails, type);
            shiftOrderVM.Closed += shiftOrderVM_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new ShiftProductionView(rawProductionDetails, type) { DataContext = shiftOrderVM });
        }

        void shiftOrderVM_Closed()
        {
            if (shiftOrder_Closed != null)
                shiftOrder_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        /*****************Shifting Slits/Peels******************/

        public void ShowShiftSlitPeelWindow(SlitPeelSchedule slitPeelSchedule)
        {
            ShiftSlitPeelViewModel shiftSlitPeelVM = new ShiftSlitPeelViewModel(slitPeelSchedule);
            shiftSlitPeelVM.Closed += shiftSlitPeelVM_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new ShiftSlitPeelView(slitPeelSchedule) { DataContext = shiftSlitPeelVM });
        }

        void shiftSlitPeelVM_Closed()
        {
            if (shiftSlitPeel_Closed != null)
                shiftSlitPeel_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }
        /*****************Editing Raw Stock******************/
        public void ShowEditRawStockWindow(StockMaintenanceDetails stockMaintenanceDetails)
        {
            EditStockMaintenanceViewModel esmvm = new EditStockMaintenanceViewModel(stockMaintenanceDetails);
            esmvm.Closed += editRawStockVM_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new EditStockMaintenance(stockMaintenanceDetails) { DataContext = esmvm });
        }

        void editRawStockVM_Closed()
        {
            if (editRawStock_Closed != null)
                editRawStock_Closed(1);
            ChildWindowManager.Instance.CloseChildWindow();
        }

        /*****************Slitting Confirmation******************/
        public void ShowSlittingConfirmationView(SlittingOrder slittingProductionDetails)
        {
            SlittingConfirmationViewModel spcvm = new SlittingConfirmationViewModel(slittingProductionDetails);
            spcvm.Closed += slitPeelConfirmationVM_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new SlittingConfirmationView(slittingProductionDetails) { DataContext = spcvm });
        }

        void slitPeelConfirmationVM_Closed()
        {
            if (slitPeelConfirmation_Closed != null)
                slitPeelConfirmation_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }
        ///*****************Peeling Confirmation******************/
        //public void ShowPeelingEditRawStockWindow(PeelingProductionDetails peelingProductionDetails)
        //{
        //    PeelingConfirmationViewModel pcvm = new PeelingConfirmationViewModel(peelingProductionDetails);
        //    pcvm.Closed += PeelConfirmationVM_Closed;
        //    ChildWindowManager.Instance.ShowChildWindow(new PeelingConfirmationView(peelingProductionDetails) { DataContext = pcvm });
        //}

        //void PeelConfirmationVM_Closed()
        //{
        //    if (peelingConfirmation_Closed != null)
        //        peelingConfirmation_Closed();
        //    ChildWindowManager.Instance.CloseChildWindow();
        //}
        /*****************Adding to graded stock******************/

        public void ShowAddGradedStock()
        {
            AddToGradedStockViewModel agsvm = new AddToGradedStockViewModel();
            agsvm.Closed += ShowAddGradedStockVM_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new AddToGradedStockView() { DataContext = agsvm });
        }
        void ShowAddGradedStockVM_Closed()
        {
            if (addToGradedStock_Closed != null)
                addToGradedStock_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        /*****************View Mixing Weekly Schedule******************/
        public void ShowMixingWeeklySchedule()
        {
            WeeklyScheduleViewModel agsvm = new WeeklyScheduleViewModel();
            agsvm.Closed += MixingWeeklySchedule_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new WeeklyScheduleView() { DataContext = agsvm });
        }
        void MixingWeeklySchedule_Closed()
        {
            if (mixingWeeklySchedule_Closed != null)
                mixingWeeklySchedule_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

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

        /*****************Converting Order To Graded******************/
        public void ShowConvertOrderWindow(RawProductionDetails rawProductionDetails)
        {
            ConvertOrderViewModel convertOrderVM = new ConvertOrderViewModel(rawProductionDetails);
            convertOrderVM.Closed += convertOrderVM_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new ConvertOrderView(rawProductionDetails) { DataContext = convertOrderVM });
        }

        void convertOrderVM_Closed()
        {
            if (conOrder_Closed != null)
                conOrder_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        /*****************Order Result******************/
        public void ShowOrderResultWindow(List<Tuple<DateTime, string, int, string>> data)
        {
            OrderResultViewModel ORVM = new OrderResultViewModel(data);
            ORVM.Closed += OrderResultVM_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new OrderResultView(data) { DataContext = ORVM });
        }

        void OrderResultVM_Closed()
        {
            if (conOrder_Closed != null)
                conOrder_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        /*****************Peeling Confirmation******************/
        public void ShowPeelingConfirmationView(PeelingOrder peelingOrder)
        {
            PeelingConfirmationViewModel pcvm = new PeelingConfirmationViewModel(peelingOrder);
            pcvm.Closed += PeelConfirmationVM_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new PeelingConfirmationView(peelingOrder) { DataContext = pcvm });
        }

        void PeelConfirmationVM_Closed()
        {
            if (peelingConfirmation_Closed != null)
                peelingConfirmation_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///*****************Printing Labels***************************/
        public void PrintingLabelsPopUp(ReRollingOrder rrp)
        {
            PrintLabelViewModel plvm = new PrintLabelViewModel(rrp);
            plvm.Closed += ClosePrintLabelPopUp;
            ChildWindowManager.Instance.ShowChildWindow(new PrintLabelView(rrp) { DataContext = plvm });
        }
        public void ClosePrintLabelPopUp()
        {
            if (printLabels_Closed != null)
                printLabels_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///*****************ReRolling Popup***************************/
        public void ShowReRollingConfirmationView(ReRollingOrder reRollingProd)
        {
            ReRollingConfirmationViewModel rrcvm = new ReRollingConfirmationViewModel(reRollingProd);
            rrcvm.Closed += ReRollingConfirmationVM_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new ReRollingConfirmationView(reRollingProd) { DataContext = rrcvm });
        }

        void ReRollingConfirmationVM_Closed()
        {
            if (reRollingConfirmation_Closed != null)
                reRollingConfirmation_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///*****************Update Graded Stock***************************/

        public void ShowAddGradedStockInside()
        {
            UpdateGradedStockViewModel ugsvm = new UpdateGradedStockViewModel();
            ugsvm.Closed += UpdateGradedStock_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new UpdateGradedStockView() { DataContext = ugsvm });
        }
        void UpdateGradedStock_Closed()
        {
            if (updateGradedStock_Closed != null)
                updateGradedStock_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///*****************New Vehicle WorkOrder***************************/
        public void ShowNewVehicleWorkOrder(string userName)
        {
            NewVehicleWorkOrderViewModel nvwovm = new NewVehicleWorkOrderViewModel(userName);
            nvwovm.Closed += NewVehicleWorkOrder_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new NewVehicleWorkOrderView(userName) { DataContext = nvwovm });
        }
        void NewVehicleWorkOrder_Closed(int n)
        {
            if (newVehicleWorkOrder_Closed != null)
                newVehicleWorkOrder_Closed(n);
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///*****************Update Vehicle WorkOrder***************************/
        public void ShowUpdateVehicleWorkOrder(VehicleWorkOrder vwo, string userName)
        {
            UpdateVehicleWorkOrderViewModel nvwovm = new UpdateVehicleWorkOrderViewModel(vwo, userName);
            nvwovm.Closed += UpdateVehicleWorkOrder_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new UpdateVehicleWorkOrderView(vwo, userName) { DataContext = nvwovm });
        }
        void UpdateVehicleWorkOrder_Closed(int n)
        {
            if (updateVehicleWorkOrder_Closed != null)
                updateVehicleWorkOrder_Closed(n);
            ChildWindowManager.Instance.CloseChildWindow();
        }


        ///*****************Vehicle WorkOrder Items***************************/
        public void ShowVehicleWorkOrderItems(VehicleWorkOrder vw)
        {
            ViewVehicleWorkOrderItemsViewModel nvwovm = new ViewVehicleWorkOrderItemsViewModel(vw);
            nvwovm.Closed += VehicleWorkOrderItems_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new ViewVehicleWorkOrderItemsView(vw) { DataContext = nvwovm });
        }
        void VehicleWorkOrderItems_Closed()
        {
            if (vehicleWorkOrderItems_Closed != null)
                vehicleWorkOrderItems_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///*****************Complete Vehicle WorkOrder***************************/
        public void ShowCompleteVehicleWorkOrder(VehicleWorkOrder vwo)
        {
            CompleteVehicleWorkOrderViewModel nvwovm = new CompleteVehicleWorkOrderViewModel(vwo);
            nvwovm.Closed += CompleteVehicleWorkOrder_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new CompleteVehicleWorkOrderView(vwo) { DataContext = nvwovm });
        }
        void CompleteVehicleWorkOrder_Closed(int n)
        {
            if (completeWorkOrder_Closed != null)
                completeWorkOrder_Closed(n);
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///*****************Maintenance Acceptance Form***************************/
        public void ShowMaintenanceAcceptance(VehicleWorkOrder vwo)
        {
            MaintenanceAcceptanceViewModel nvwovm = new MaintenanceAcceptanceViewModel(vwo);
            nvwovm.Closed += MaintenanceAcceptance_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new MaintenanceAcceptanceView(vwo) { DataContext = nvwovm });
        }
        void MaintenanceAcceptance_Closed(MaintenanceAcceptanceViewModel mavm)
        {
            if (maintenanceAcceptance_Closed != null)
                maintenanceAcceptance_Closed(mavm);
            ChildWindowManager.Instance.CloseChildWindow();
        }


        ///*****************Inner Vehicle Repair Workorder***************************/
        public void ShowInnerRepairVehicleWorkOrder(VehicleWorkDescription vwo, string un)
        {
            InnerVehicleRepairWorkOrderViewModel nvwovm = new InnerVehicleRepairWorkOrderViewModel(vwo, un);
            nvwovm.Closed += InnerVehicleRepairWorkOrder_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new InnerVehicleRepairWorkOrderView(vwo, un) { DataContext = nvwovm });
        }
        void InnerVehicleRepairWorkOrder_Closed(Int32 id)
        {
            if (innerVehicleRepairWorkOrder_Closed != null)
                innerVehicleRepairWorkOrder_Closed(id);
            ChildWindowManager.Instance.CloseChildWindow();
        }


        ///*****************Vehicle WorkOrder History Items***************************/
        public void ShowVehicleWorkOrderHistoryItems(VehicleWorkOrderHistory vwoh)
        {
            VehicleWorkOrderHistoryItemsViewModel nvwovm = new VehicleWorkOrderHistoryItemsViewModel(vwoh);
            nvwovm.Closed += VehicleWorkOrderHistoryItems_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new VehicleWorkOrderHistoryItemsView(vwoh) { DataContext = nvwovm });
        }
        void VehicleWorkOrderHistoryItems_Closed()
        {
            if (vehicleWorkOrderHistoryItems_Closed != null)
                vehicleWorkOrderHistoryItems_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///************************Add New Vehicle ***********************************/
        public void ShowAddNewVehicle()
        {
            AddNewVehicleViewModel nvwovm = new AddNewVehicleViewModel();
            nvwovm.Closed += ShowAddNewVehicle_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new AddNewVehicleView() { DataContext = nvwovm });
        }
        void ShowAddNewVehicle_Closed()
        {
            if (addNewVehicle_Closed != null)
                addNewVehicle_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///************************Update Vehicle ***********************************/
        public void ShowUpdateVehicle()
        {
            UpdateVehicleViewModel uvvm = new UpdateVehicleViewModel();
            uvvm.Closed += UpdateVehicle_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new UpdateVehicleView() { DataContext = uvvm });
        }
        void UpdateVehicle_Closed()
        {
            if (updateVehicle_Closed != null)
                updateVehicle_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }


        ///*****************Vehicle Maintenance Description***************************/
        public void ShowVehicleMainDes(int location, int vehicleType, string userName)
        {
            VehicleMaintenanceDescriptionsViewModel nvwovm = new VehicleMaintenanceDescriptionsViewModel(location, vehicleType, userName);
            nvwovm.Closed += VehicleMaintDes_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new VehicleMaintenanceDescriptionsView(location, vehicleType, userName) { DataContext = nvwovm });
        }
        void VehicleMaintDes_Closed(int n)
        {
            if (vehicleMaintDes_Closed != null)
                vehicleMaintDes_Closed(n);
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///*****************Machine Maintenance Description***************************/
        public void ShowMachineMainDes(int location, string userName, int machineID, string ot, int pid, string noOfDays)
        {
            MachineMaintenanceDescriptionViewModel nvwovm = new MachineMaintenanceDescriptionViewModel(location, userName, machineID,ot,pid,noOfDays);
            nvwovm.Closed += MachineMaintDes_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new MachineMaintenanceDescriptionView(location, userName, machineID, ot, pid, noOfDays) { DataContext = nvwovm });
        }
        void MachineMaintDes_Closed(int n)
        {
            if (machineMaintDes_Closed != null)
                machineMaintDes_Closed(n);
            ChildWindowManager.Instance.CloseChildWindow();
        }


        ///*****************Machine WorkOrder Items***************************/
        public void ShowMachineWorkOrderItems(MachineWorkOrder mwo)
        {
            MachineWorkOrderItemsViewModel nvwovm = new MachineWorkOrderItemsViewModel(mwo);
            nvwovm.Closed += MachineWorkOrderItems_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new MachineWorkOrderItemsView(mwo) { DataContext = nvwovm });
        }
        void MachineWorkOrderItems_Closed(int a)
        {
            if (machineWorkOrderItems_Closed != null)
                machineWorkOrderItems_Closed(a);
            ChildWindowManager.Instance.CloseChildWindow();
        }


        ///*****************Set Mechanic Name***************************/
        public void ShowSetMechanicName(MachineWorkOrder mwo)
        {
            SetMechanicViewModel nvwovm = new SetMechanicViewModel(mwo);
            nvwovm.Closed += SetMechanicClose_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new SetMechanicView(mwo) { DataContext = nvwovm });
        }
        void SetMechanicClose_Closed(SetMechanicViewModel mavm)
        {
            if (setMechanicViewModel_Closed != null)
                setMechanicViewModel_Closed(mavm);
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///*****************Complete Machine WorkOrder***************************/
        public void ShowCompleteMachineWorkOrder(MachineWorkOrder vwo, string uN)
        {
            CompleteMachineWorkOrderViewModel nvwovm = new CompleteMachineWorkOrderViewModel(vwo, uN);
            nvwovm.Closed += CompleteMachineWorkOrder_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new CompleteMachineWorkOrderView(vwo, uN) { DataContext = nvwovm });
        }
        void CompleteMachineWorkOrder_Closed(int n)
        {
            if (completeMachineWorkOrder_Closed != null)
                completeMachineWorkOrder_Closed(n);
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///*****************Machine WorkOrder History Items***************************/
        public void ShowMachineWorkOrderHistoryItems(MachineWorkOrderHistory vwoh)
        {
            MachineWorkOrderHistoryItemsViewModel nvwovm = new MachineWorkOrderHistoryItemsViewModel(vwoh);
            nvwovm.Closed += MachineWorkOrderHistoryItems_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new MachineWorkOrderHistoryItemsView(vwoh) { DataContext = nvwovm });
        }
        void MachineWorkOrderHistoryItems_Closed(int r)
        {
            if (machineWorkOrderHistoryItems_Closed != null)
                machineWorkOrderHistoryItems_Closed(r);
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///************************Add New Machine ***********************************/
        public void ShowAddNewMachine()
        {
            AddNewMachineViewModel nvwovm = new AddNewMachineViewModel();
            nvwovm.Closed += ShowAddNewMachine_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new AddNewMachineView() { DataContext = nvwovm });
        }
        void ShowAddNewMachine_Closed()
        {
            if (addNewMachine_Closed != null)
                addNewMachine_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///************************Update Machine ***********************************/
        public void ShowUpdateMachine()
        {
            UpdateMachineViewModel uvvm = new UpdateMachineViewModel();
            uvvm.Closed += UpdateMachine_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new UpdateMachineView() { DataContext = uvvm });
        }
        void UpdateMachine_Closed()
        {
            if (updateMachine_Closed != null)
                updateMachine_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }


        ///*****************Set Mechanic Name for groups***************************/
        public void ShowSetMechanicNameGroup()
        {
            SetMechanicGroupComplettionViewModel nvwovm = new SetMechanicGroupComplettionViewModel();
            nvwovm.Closed += SetMechanicGroupClose_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new SetMechanicGroupComplettionView() { DataContext = nvwovm });
        }
        void SetMechanicGroupClose_Closed(UserPosition mavm)
        {
            if (setMechanicGroupViewModel_Closed != null)
                setMechanicGroupViewModel_Closed(mavm);
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///*****************Set Mechanic Name for groups***************************/
        public void ShowVehicleMaintenanceDescription(VehicleWorkOrder vwo)
        {
            MaintenanceDescriptionsViewModel mdvm = new MaintenanceDescriptionsViewModel(vwo);
            mdvm.Closed += VehicleMaintenanceDescription_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new MaintenanceDescriptionsView(vwo) { DataContext = mdvm });
        }
        void VehicleMaintenanceDescription_Closed(MaintenanceDescriptionsViewModel m)
        {
            if (viewVehicleMaintenanceDesc_Closed != null)
                viewVehicleMaintenanceDesc_Closed(m);
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///*****************View Vehicle Repair Descriptions*************************/
        public void ShowVehicleRepairDescription(VehicleWorkOrder vwo)
        {
            RepairDescriptionsViewModel mdvm = new RepairDescriptionsViewModel(vwo);
            mdvm.Closed += ViewVehicleRepairDesc_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new RepairDescriptionsView(vwo) { DataContext = mdvm });
        }
        void ViewVehicleRepairDesc_Closed()
        {
            if (viewVehicleRepairDesc_Closed != null)
                viewVehicleRepairDesc_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///*****************Complete Vehicle Repair Descriptions*************************/
        public void ShowCompleteVehicleRepairDescription(VehicleWorkOrder vwo, string un)
        {
            CompleteRepairVehicleWorkOrderViewModel mdvm = new CompleteRepairVehicleWorkOrderViewModel(vwo, un);
            mdvm.Closed += CompleteVehicleRepairDesc_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new CompleteRepairVehicleWorkOrderView(vwo, un) { DataContext = mdvm });
        }
        void CompleteVehicleRepairDesc_Closed(int r)
        {
            if (completeVehicleRepairDesc_Closed != null)
                completeVehicleRepairDesc_Closed(r);
            ChildWindowManager.Instance.CloseChildWindow();
        }

        ///*****************Complete Vehicle Repair Descriptions*************************/
        public void ShowOdometerAcceptanceView(VehicleWorkOrder vwo)
        {
            OdometerAccepatnceViewModel mdvm = new OdometerAccepatnceViewModel(vwo);
            mdvm.Closed += OdometerAcceptance_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new OdometerAccepatnceView(vwo) { DataContext = mdvm });
        }
        void OdometerAcceptance_Closed(Int64 odo)
        {
            if (odometerAcceptance_Closed != null)
                odometerAcceptance_Closed(odo);
            ChildWindowManager.Instance.CloseChildWindow();
        }

        /*****************Adding to regrind stock******************/

        public void ShowAddRegrindStock()
        {
            AddToRegrindStockViewModel agsvm = new AddToRegrindStockViewModel();
            agsvm.Closed += ShowAddRegrindStockVM_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new AddToRegrindStockView() { DataContext = agsvm });
        }
        void ShowAddRegrindStockVM_Closed()
        {
            if (addToRegrindStock_Closed != null)
                addToRegrindStock_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }


        /*****************Off-Spec Report******************/

        public void ShowOffSpecReport()
        {
            OffSpecReportDatePickerViewModel agsvm = new OffSpecReportDatePickerViewModel();
            agsvm.Closed += OffSpecReport_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new OffSpecReportDatePickerView() { DataContext = agsvm });
        }
        void OffSpecReport_Closed()
        {
            if (offSpecReport_Closed != null)
                offSpecReport_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        /*****************Shifting Slitting Orders******************/
        public void ShowSlittingShiftWindow(SlittingOrder so)
        {
            ShiftSlittingOrderViewModel shiftOrderVM = new ShiftSlittingOrderViewModel(so);
            shiftOrderVM.Closed += SlittingShiftWindow_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new ShiftSlittingOrderView(so) { DataContext = shiftOrderVM });
        }

        void SlittingShiftWindow_Closed()
        {
            if (shiftSlittingOrder_Closed != null)
                shiftSlittingOrder_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }


        /*****************IBCChange******************/
        public void ShowIBCChangeWindow()
        {
            IBCChangeViewModel shiftOrderVM = new IBCChangeViewModel();
            shiftOrderVM.Closed += IBCChangeWindow_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new IBCChangeView() { DataContext = shiftOrderVM });
        }

        void IBCChangeWindow_Closed()
        {
            if (IBCChangeView_Closed != null)
                IBCChangeView_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        /*****************View Mixing Uncompleted List***************/
        public void ShowMixingUnfinishedList()
        {
            MixingUncompletedListViewModel agsvm = new MixingUncompletedListViewModel();
            agsvm.Closed += MixingUnfinished_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new MixingUncompletedListView() { DataContext = agsvm });
        }
        void MixingUnfinished_Closed()
        {
            if (mixingUnfinished_Closed != null)
                mixingUnfinished_Closed();
            ChildWindowManager.Instance.CloseChildWindow();
        }


        /**********************Provider Screen Popup***************************/

        public void ShowProvider()
        {
            AddNewProviderViewModel anpvm = new AddNewProviderViewModel();
            anpvm.Closed += AddNewProvider_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new AddProviderView() { DataContext = anpvm });
        }
        void AddNewProvider_Closed()
        {
            if (providerScreenClosed != null)
                providerScreenClosed();
            ChildWindowManager.Instance.CloseChildWindow();
        }

        /**********************File Upload Screen***************************/

        public void ShowFileUploadScreen(string userName,int machineID, Int64 wod)
        {
            FileUploadViewModel anpvm = new FileUploadViewModel(userName,machineID, wod);
            anpvm.Closed += FileUploadScreen_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new FileUploadView(userName,machineID, wod) { DataContext = anpvm });
        }

        void FileUploadScreen_Closed(int a)
        {
            if (fileUploadScreenClosed != null)
                fileUploadScreenClosed(a);
            ChildWindowManager.Instance.CloseChildWindow();
        }


        /**********************Open File Uploaded Screen***************************/
        public void ShowOpenFileUploadedScreen(Int64 workOrderNo, string machineName)
        {
            OpenUploadedFilesViewModel anpvm = new OpenUploadedFilesViewModel(workOrderNo, machineName);
            anpvm.Closed += OpenFileUploadedScreen_Closed;
            ChildWindowManager.Instance.ShowChildWindow(new OpenUploadedFilesView(workOrderNo, machineName) { DataContext = anpvm });
        }

        void OpenFileUploadedScreen_Closed(int t)
        {
            if (uploadedFilesClosed != null)
                uploadedFilesClosed(t);
            ChildWindowManager.Instance.CloseChildWindow();
        }

        
    }
}
