using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Core;
using A1QSystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using A1QSystem.Commands;
using System.Windows;
using A1QSystem.View.Production;
using MsgBox;
using A1QSystem.Model.Meta;

namespace A1QSystem.ViewModel
{
    public class MachineViewModel : ViewModelBase
    {
            
        private BindingList<Expense> _dayShiftData;
        private BindingList<Expense> _eveningShiftData;
        private BindingList<Expense> _nightShiftData;

        private BindingList<MachineSelectedEmployees> _dayShiftPeopleData;

        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Employee> Employees { get; set; }

        private double _totalExpense;
        private double _eveningShiftMixes;
        private double _nightShiftMixes;
        private double _totalMixes;
        private string _noOfPeopleDay;
        private string _noOfPeopleEve;
        private string _noOfPeopleNight;

        private string _machineName;
        private string _userName;
        private string _state;
        private List<UserPrivilages> _privilages;
        private int _productionID;
        private List<MetaData> metaData;
    
        private bool _canExecute;

        private string _productionDate = DateTime.Now.ToString("dd/MM/yyyy");
        
        private ICommand _addProdCommand;
        private ICommand _commandUserLogout;
        private ICommand _commandsBack;

        public MachineViewModel(string MachineName, string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {

            _productionID = DBAccess.GetProductionID();
            metaData = md;
            _machineName = MachineName;
            _userName = UserName;
            _state = State;
            _privilages = Privilages;

            _canExecute = true;

            DataView pdv = new DataView();
            pdv = DBAccess.GetAllProducts().Tables["Products"].DefaultView;
            pdv.Sort = "ProductName ASC";
            Products = new ObservableCollection<Product>();

            for (int x = 0; x < pdv.Count; x++)
            {
                Products.Add(new Product() {ProductCode = pdv[x]["ProductCode"].ToString(), ProductName = pdv[x]["ProductName"].ToString() });
            }

            

            pdv = DBAccess.GetAllEmployees().Tables["Employees"].DefaultView;
            pdv.Sort = "EmpName ASC";
            Employees = new ObservableCollection<Employee>();

            for (int x = 0; x < pdv.Count; x++)
            {
                Employees.Add(new Employee() { EmpID = Convert.ToInt32(pdv[x]["EmpID"]), EmpName = pdv[x]["EmpName"].ToString() });
            }


            DayShiftData = new BindingList<Expense>();
            NightShiftData = new BindingList<Expense>();
            EveningShiftData = new BindingList<Expense>();

            DayShiftPeopleData = new BindingList<MachineSelectedEmployees>();

          //  _addShiftBtnStatus = false;
            _addProductionData = true;

        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string MachineName
        {
            get 
            {
                return _machineName; 
            }
            set{_machineName = value;}
        }

        public int ProductionID
        {
            get { return _productionID; }
            set { _productionID = value; }
        }


        public double TotalExpense
        {
            get
            {
                _totalExpense = _dayShiftData.Sum(x => x.Mixes);
                return _totalExpense;  
            }
            set 
            {
                _totalExpense = value;
                
            }
        }

        public double EveningShiftMixes
        {
            get { return _eveningShiftMixes = _eveningShiftData.Sum(x => x.Mixes); }
            set { _eveningShiftMixes = value; }
        }

        public double NightShiftMixes
        {
            get { return _nightShiftMixes = _nightShiftData.Sum(x => x.Mixes); }
            set { _nightShiftMixes = value; }
        }

        public double TotalMixes
        {
            get { return _totalMixes = TotalExpense + EveningShiftMixes + NightShiftMixes; }
            set { _totalMixes = value; }
        }
        
        public BindingList<Expense> DayShiftData
        {
            get { return _dayShiftData; }
            set
            {
                _dayShiftData = value;
                if (_dayShiftData != null)
                {
                   
                    _dayShiftData.ListChanged += (o, e) => RaisePropertyChanged(() => this.TotalExpense);
                    _dayShiftData.ListChanged += (o, e) => RaisePropertyChanged(() => this.TotalMixes);                    

                }
                RaisePropertyChanged(() => this.DayShiftData);
              
            }
        }

        public BindingList<Expense> EveningShiftData
        {
            get { return _eveningShiftData; }
            set
            {
                _eveningShiftData = value;
                if (_eveningShiftData != null)
                {
                    _eveningShiftData.ListChanged += (o, e) => RaisePropertyChanged(() => this.EveningShiftMixes);
                    _eveningShiftData.ListChanged += (o, e) => RaisePropertyChanged(() => this.TotalMixes);
                }
                RaisePropertyChanged(() => this._eveningShiftData);
            }
        }

        public BindingList<Expense> NightShiftData
        {
            get { return _nightShiftData; }
            set
            {
                _nightShiftData = value;
                if (_nightShiftData != null)
                {
                    _nightShiftData.ListChanged += (o, e) => RaisePropertyChanged(() => this.NightShiftMixes);
                    _nightShiftData.ListChanged += (o, e) => RaisePropertyChanged(() => this.TotalMixes);
                }
                RaisePropertyChanged(() => this.NightShiftData);
            }
        }

        /* Dayshift People Data */
        public BindingList<MachineSelectedEmployees> DayShiftPeopleData
        {
            get { return _dayShiftPeopleData; }
            set
            {
                _dayShiftPeopleData = value;
                if (_dayShiftData != null)
                {

                 //   _dayShiftPeopleData.ListChanged += (o, e) => RaisePropertyChanged(() => this.TotalExpense);
                 //   _dayShiftPeopleData.ListChanged += (o, e) => RaisePropertyChanged(() => this.TotalMixes);

                }
                RaisePropertyChanged(() => this.DayShiftPeopleData);

            }
        }

        public string ProductionDate
        {
            get { return _productionDate; }
            set 
            { 
                _productionDate = value; 
                RaisePropertyChanged(() => this.ProductionDate);
            }
        }

  
        public string NoOfPeopleDay
        {
            get { return _noOfPeopleDay; }
            set
            {
                _noOfPeopleDay = value;
                RaisePropertyChanged(() => this.NoOfPeopleDay);            

            }
        }

        public string NoOfPeopleEve
        {
            get { return _noOfPeopleEve; }
            set
            {
                _noOfPeopleEve = value;
                RaisePropertyChanged(() => this.NoOfPeopleEve);               
            }
        }

        public string NoOfPeopleNight
        {
            get { return _noOfPeopleNight; }
            set
            {
                _noOfPeopleNight = value;
                RaisePropertyChanged(() => this.NoOfPeopleNight);                
            }
        }

        public ICommand AddProdCommand
        {
            get
            {
                return _addProdCommand ?? (_addProdCommand = new LogOutCommandHandler(() => AddProductionData(), _addProductionData));
            }
        }

        private bool _addProductionData;
        public void AddProductionData()
        {
         
               int result = DBAccess.InsertProductionData(DayShiftData, EveningShiftData, NightShiftData,NoOfPeopleDay, NoOfPeopleEve, NoOfPeopleNight, ProductionDate, MachineName);

               if (result > 0)
               {
                  Msg.Show("Productions were added successfully!", "Production Added",MsgBoxButtons.OK,MsgBoxImage.OK,MsgBoxResult.Yes);
                  Switcher.Switch(new DailyProductionView(_userName, _state, _privilages, metaData));
               }            
          
        }

       

        public ICommand CommandsBack
        {
            get
            {
                return _commandsBack ?? (_commandsBack = new LogOutCommandHandler(() => Switcher.Switch(new DailyProductionView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }


              

     
     

    }
}
