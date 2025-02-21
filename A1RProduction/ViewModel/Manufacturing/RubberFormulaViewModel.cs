using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MsgBox;
using A1QSystem.Commands;
using A1QSystem.PDFGeneration;
using A1QSystem.Model.FormulaGeneration;

namespace A1QSystem.ViewModel.Manufacturing
{
    public class RubberFormulaViewModel : ViewModelBase
    {
        private string _userName;
        private string state;
        private List<UserPrivilages> _privilages;
        

        private ICommand _searchProductCommand;
        private ICommand _command;
        private ICommand _printCommand;
        private ICommand _clearCommand;
        private bool _canExecute;

        public RubberFormulaViewModel(string UserName, string State, List<UserPrivilages> Privilages)
        {
            _userName = UserName;
            state = State;
            _privilages = Privilages;
            _canExecute = true;
            _productColourDetails = new BindingList<ProductColourDetails>();
            LoadProductDetails();
            LoadColours();
          //  SelectedProductName = "Bunnings UTE Mat";

           // _productColourDetails.Add(new ProductColourDetails(){ColourName="XXX",BagSize1= 10,BagSize2= 10});
        }

        private void LoadProductDetails()
        {
            ManuProductList = DBAccess.GetAllMaufacturingProducts();

            var proNames = new ObservableCollection<string>();
            foreach(var data in ManuProductList){
                proNames.Add(data.ProductName);
            }
            proNames = new ObservableCollection<string>(proNames.OrderBy(a => a));
            ProductNames=proNames;
        }

        private void SearchProduct()
        {
            ClearFields();

            var searchedData = new ObservableCollection <RubberProduction>();
            searchedData = DBAccess.SearchProductFormula(getProductIDbyName());
            if (searchedData.Count > 0)
            {
                foreach (var x in searchedData)
                {
                    NoOfBins = x.NoOfBins;
                    MouldType = x.MouldType;
                    Mesh4 = x.GradingSize4;
                    Mesh12 = x.GradingSize12;
                    Mesh16 = x.GradingSize16;
                    Mesh16To30 = x.GradingSize1620;
                    Mesh30 = x.GradingSize30;
                    Mesh12mg = x.GradingSize12mg;
                    MeshRegrind = x.GradingSizeRegrind;                   
                    Binder = x.Binder;
                    MixingMinutes = x.Minutes;
                    SpecialInstructions = x.SpecialInstructions;
                    ColourInstructions = x.ColourInstructions;
                    MethodPS = x.MethodPS;
                }
                var searchedColourData = new BindingList<ProductColourDetails>();
                searchedColourData = DBAccess.GetFormulaColours(getProductIDbyName());

                if (searchedColourData.Count > 0)
                {
                    ColourDetailsActive = true;
                  
                    ProductColourDetails = searchedColourData;

                   
                }
            }
            else
            {
                Msg.Show("Information not available for the searched product name", "Information Not Availabale", MsgBoxButtons.OK, MsgBoxImage.Information);
            }
        }

        private void ClearFields()
        {
            MouldType = string.Empty;
            Mesh4 = 0;
            Mesh12 = 0;
            Mesh16 = 0;
            Mesh16To30 = 0;
            Mesh30 = 0;
            Mesh12mg = 0;
            MeshRegrind = 0;
            NoOfBins = 0;
            SpecialInstructions = string.Empty;
            Binder = 0;
            MixingMinutes = 0;
            ColourInstructions = string.Empty;
            ProductColourDetails.Clear();
        }

        private int getProductIDbyName()
        {
            int id=0;

            foreach (var data in ManuProductList)
            {
                if (data.ProductName == SelectedProductName)
                {
                    id=data.ProductID;
                }
            }
            return id; 
        }

        private void Print()
        {
            PrintFormulaPDF pfpdf = new PrintFormulaPDF();
            bool result = pfpdf.CreateFormula(ProductColourDetails,FormulaColourTable, Binder, BinderType, SelectedProductName, MixingMinutes, MouldType, NoOfBins, Mesh4, Mesh12, Mesh16, Mesh16To30, Mesh30, Mesh3040, Mesh12mg, MeshRegrind, SpecialInstructions, ColourInstructions, MethodPS, HeaderColours, HeaderFontSize, TopicFontSize, SpecialInsHeight, SpecialInsTextPosHeight, Enable, Lift1, Lift2, MixingNotes, FormulaType);
            Console.WriteLine(result);
        }

        //Loading Colours to Datagrid
        private void LoadColours()
        {
            var productColourList = new BindingList<ProductColours>();
            productColourList=DBAccess.GetAllProductColours();

            ColourList = productColourList;
        }


        private bool CanExecute(object parameter)
        {
            return true;
        }

        private void Execute(object parameter)
        {
            int index = ProductColourDetails.IndexOf(parameter as ProductColourDetails);
            if (index > -1 && index < ProductColourDetails.Count)
            {
                ProductColourDetails.RemoveAt(index);
            }
            if (ProductColourDetails.Count == 0)
            {
                ProductColourDetails = new BindingList<ProductColourDetails>();
            }
        }

        #region Public Properties

        private BindingList<ProductColours> _colourList;
        public BindingList<ProductColours> ColourList
        {
            get { return _colourList; }
            set
            {
                _colourList = value;
                
                RaisePropertyChanged(() => this.ColourList);
            }
        }

        private BindingList<ProductColourDetails> _productColourDetails;
        public BindingList<ProductColourDetails> ProductColourDetails
        {
            get { return _productColourDetails; }
            set
            {
                _productColourDetails = value;

                if (_productColourDetails != null)
                {
                    _productColourDetails.ListChanged += (o, e) => RaisePropertyChanged(() => this.ColourName);
                    _productColourDetails.ListChanged += (o, e) => RaisePropertyChanged(() => this.BagSize1);
                    _productColourDetails.ListChanged += (o, e) => RaisePropertyChanged(() => this.BagSize2);
                }

                RaisePropertyChanged(() => this.ProductColourDetails);
            }
        }

        private ObservableCollection<FormulaColourTableHeaders> _formulaColourTable;
        public ObservableCollection<FormulaColourTableHeaders> FormulaColourTable
        {
            get { return _formulaColourTable; }
            set
            {
                _formulaColourTable = value;


                RaisePropertyChanged(() => this.FormulaColourTable);
            }
        }

        private string _colourName;
        public string ColourName
        {
            get
            {
                return _colourName;
            }
            set
            {
                _colourName = value;
                RaisePropertyChanged(() => this.ColourName);
            }
        }

        private string _formulaType;
        public string FormulaType
        {
            get
            {
                return _formulaType;
            }
            set
            {
                _formulaType = value;
                RaisePropertyChanged(() => this.FormulaType);
            }
        }

        private int _bagSize1;
        public int BagSize1
        {
            get
            {
                return _bagSize1;
            }
            set
            {
                _bagSize1 = value;
                RaisePropertyChanged(() => this.BagSize1);
            }
        }

        private int _bagSize2;
        public int BagSize2
        {
            get
            {
                return _bagSize2;
            }
            set
            {
                _bagSize2 = value;
                RaisePropertyChanged(() => this.BagSize2);
            }
        }

        private ObservableCollection<string> _productNames;
        public ObservableCollection<string> ProductNames
        {
            get
            {
                return _productNames;
            }
            set
            {
                _productNames = value;
                RaisePropertyChanged(() => this.ProductNames);
            }
        }
        private ObservableCollection<ManufacturingProduct> _manuProductList;
        public ObservableCollection<ManufacturingProduct> ManuProductList
        {
            get
            {
                return _manuProductList;
            }
            set
            {
                _manuProductList = value;
                RaisePropertyChanged(() => this.ManuProductList);
            }
        }
        private string _selectedProductName;
        public string SelectedProductName
        {
            get
            {
                return _selectedProductName;
            }
            set
            {
                _selectedProductName = value;
                RaisePropertyChanged(() => this.SelectedProductName);
            }
        }


        private string _mouldType;
        public string MouldType
        {
            get
            {
                return _mouldType;
            }
            set
            {
                _mouldType = value;
                RaisePropertyChanged(() => this.MouldType);
            }
        }

        private int _noOfBins;
        public int NoOfBins
        {
            get
            {
                return _noOfBins;
            }
            set
            {
                _noOfBins = value;
                RaisePropertyChanged(() => this.NoOfBins);
            }
        }


        private int _mesh4;
        public int Mesh4
        {
            get
            {
                return _mesh4;
            }
            set
            {
                _mesh4 = value;
                RaisePropertyChanged(() => this.Mesh4);
            }
        }

        private int _mesh12;
        public int Mesh12
        {
            get
            {
                return _mesh12;
            }
            set
            {
                _mesh12 = value;
                RaisePropertyChanged(() => this.Mesh12);
            }
        }
        private int _mesh16;
        public int Mesh16
        {
            get
            {
                return _mesh16;
            }
            set
            {
                _mesh16 = value;
                RaisePropertyChanged(() => this.Mesh16);
            }
        }

        private int _mesh16To30;
        public int Mesh16To30
        {
            get
            {
                return _mesh16To30;
            }
            set
            {
                _mesh16To30 = value;
                RaisePropertyChanged(() => this.Mesh16To30);
            }
        }
        private int _mesh30;
        public int Mesh30
        {
            get
            {
                return _mesh30;
            }
            set
            {
                _mesh30 = value;
                RaisePropertyChanged(() => this.Mesh30);
            }
        }

        private int _mesh3040;
         public int Mesh3040
        {
            get
            {
                return _mesh3040;
            }
            set
            {
                _mesh3040 = value;
                RaisePropertyChanged(() => this.Mesh3040);
            }
        }

        
        private int _mesh12mg;
        public int Mesh12mg
        {
            get
            {
                return _mesh12mg;
            }
            set
            {
                _mesh12mg = value;
                RaisePropertyChanged(() => this.Mesh12mg);
            }
        }
        private int _meshRegrind;
        public int MeshRegrind
        {
            get
            {
                return _meshRegrind;
            }
            set
            {
                _meshRegrind = value;
                RaisePropertyChanged(() => this.MeshRegrind);
            }
        }

       
        private int _binder;
        public int Binder
        {
            get
            {
                return _binder;
            }
            set
            {
                _binder = value;
                RaisePropertyChanged(() => this.Binder);
            }
        }
        private string _binderType;
        public string BinderType
        {
            get
            {
                return _binderType;
            }
            set
            {
                _binderType = value;
                RaisePropertyChanged(() => this.BinderType);
            }
        }

        

        private int _mixingMinutes;
        public int MixingMinutes
        {
            get
            {
                return _mixingMinutes;
            }
            set
            {
                _mixingMinutes = value;
                RaisePropertyChanged(() => this.MixingMinutes);
            }
        }

        private bool _colourDetailsActive;
        public bool ColourDetailsActive
        {
            get { return _colourDetailsActive; }
            set
            {
                _colourDetailsActive = value;
                RaisePropertyChanged(() => this.ColourDetailsActive);
            }
        }
        private string _specialInstructions;
        public string SpecialInstructions
        {
            get { return _specialInstructions; }
            set
            {
                _specialInstructions = value;
                RaisePropertyChanged(() => this.SpecialInstructions);
            }
        }

        private string _colourInstructions;
        public string ColourInstructions
        {
            get { return _colourInstructions; }
            set
            {
                _colourInstructions = value;
                RaisePropertyChanged(() => this.ColourInstructions);
            }
        }

        private string _methodPS;
        public string MethodPS
        {
            get { return _methodPS; }
            set
            {
                _methodPS = value;
                RaisePropertyChanged(() => this.MethodPS);
            }
        }
        private string _headerColours;
        public string HeaderColours
        {
            get { return _headerColours; }
            set
            {
                _headerColours = value;
                RaisePropertyChanged(() => this.HeaderColours);
            }
        }

        private decimal _headerFontSize;
        public decimal HeaderFontSize
        {
            get { return _headerFontSize; }
            set
            {
                _headerFontSize = value;
                RaisePropertyChanged(() => this.HeaderFontSize);
            }
        }

        private decimal _topicFontSize;
        public decimal TopicFontSize
        {
            get { return _topicFontSize; }
            set
            {
                _topicFontSize = value;
                RaisePropertyChanged(() => this.TopicFontSize);
            }
        }

        private decimal _specialInsHeight;
        public decimal SpecialInsHeight
        {
            get { return _specialInsHeight; }
            set
            {
                _specialInsHeight = value;
                RaisePropertyChanged(() => this.SpecialInsHeight);
            }
        }

        private decimal _specialInsTextPosHeight;
         public decimal SpecialInsTextPosHeight
        {
            get { return _specialInsTextPosHeight; }
            set
            {
                _specialInsTextPosHeight = value;
                RaisePropertyChanged(() => this.SpecialInsTextPosHeight);
            }
        }
        
        

        private bool _enable;
        public bool Enable
        {
            get { return _enable; }
            set
            {
                _enable = value;
                RaisePropertyChanged(() => this.Enable);
            }
        }

        private string _lift1;
        public string Lift1
        {
            get
            {
                return _lift1;
            }
            set
            {
                _lift1 = value;
                RaisePropertyChanged(() => this.Lift1);
            }
        }

        private string _lift2;
        public string Lift2
        {
            get
            {
                return _lift2;
            }
            set
            {
                _lift2 = value;
                RaisePropertyChanged(() => this.Lift2);
            }
        }
        private string _mixingNotes;
        public string MixingNotes
        {
            get
            {
                return _mixingNotes;
            }
            set
            {
                _mixingNotes = value;
                RaisePropertyChanged(() => this.MixingNotes);
            }
        }
        
        
        #endregion

        /*
        string IDataErrorInfo.Error
        {
            get
            {
                return null;
            }
        }


        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                return GetValidationError(propertyName);
            }
        }

        static readonly string[] ValidatedProperies = 
        {
            "ProductName",
            "ProductCode",
            "ProductDescription",
            "ProductUnit",
            "ProductPrice"
        };

        protected bool CanSave
        {
            get
            {
                return IsValid;
            }
        }
        public bool IsValid
        {
            get
            {
                foreach (string property in ValidatedProperies)
                {
                    if (GetValidationError(property) != null)

                        return false;
                }
                return true;
            }
        }

        string GetValidationError(string propertyName)
        {
            string error = null;

            switch (propertyName)
            {
                case "SelectedProductName":
                    error = ValidateSelectedProductName();
                    break;
                default:
                    error = null;
                    throw new Exception("Unexpected property being validated on Service");
            }

            return error;
        }

        private string ValidateSelectedProductName()
        {
            throw new NotImplementedException();
        }
        */
        #region Commands

        public ICommand SearchProductCommand
        {
            get
            {
                if (_searchProductCommand == null)
                    _searchProductCommand = new A1QSystem.Commands.RelayCommand(param => this.SearchProduct(), param => this._canExecute);

                return _searchProductCommand;
            }
        }

        public ICommand PrintCommand
        {
            get
            {
                return _printCommand ?? (_printCommand = new LogOutCommandHandler(() => Print(), _canExecute));
            }
        }

        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new LogOutCommandHandler(() => ClearFields(), _canExecute));
            }
        }

      

        public ICommand RemoveCommand
        {
            get
            {
                if (_command == null)
                {
                    _command = new A1QSystem.Commands.DelegateCommand(CanExecute, Execute);
                }
                return _command;
            }
        }

        #endregion
    }
}
