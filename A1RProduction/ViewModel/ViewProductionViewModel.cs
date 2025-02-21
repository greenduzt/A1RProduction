using A1QSystem.Commands;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.Production;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace A1QSystem.ViewModel
{
    public class ViewProductionViewModel : INotifyPropertyChanged
    {
        DispatcherTimer timer;
        Machines machLargeMixer;
        Machines highSpeedMixer;
        Machines smallMixer;
        Machines logPeel;
        Machines csbrMachine;
        Machines slitter;
        Machines bagging;
        Machines rollUp;
        Machines rubberGrading;
        Machines shredding;

        private double _largeMixerMixes;
        private double _largeMixerPeople;
        private double _largeMixerEfficiency;

        private double _highSpeedMixerMixes;
        private double _highSpeedMixerPeople;
        private double _highSpeedMixerEfficiency;

        private double _smallMixerMixes;
        private double _smallMixerPeople;
        private double _smallMixerEfficiency;

        private double _logMixerMixes;
        private double _logMixerPeople;
        private double _logMixerEfficiency;

        private double _csbrMixerMixes;
        private double _csbrMixerPeople;
        private double _csbrMixerEfficiency;

        private double _slitterMixerMixes;
        private double _slitterMixerPeople;
        private double _slitterMixerEfficiency;

        private double _baggingMixerMixes;
        private double _baggingMixerPeople;
        private double _baggingMixerEfficiency;

        private double _rollMixerMixes;
        private double _rollMixerPeople;
        private double _rollMixerEfficiency;

        private double _gradingMixerMixes;
        private double _gradingMixerPeople;
        private double _gradingMixerEfficiency;

        private double _shreddingMixerMixes;
        private double _shreddingMixerPeople;
        private double _shreddingMixerEfficiency;

        private double _factoryEfficiency;
        
        private string _userName;
        private string _state;
        private List<UserPrivilages> _privilages;
        private List<MetaData> metaData;
        private bool _canExecute;
        private ICommand _commandsBack;
        private ICommand _searchData;
        private ICommand _clearGauges;
        private ICommand navHomeCommand;
        private ICommand navProductionCommand;

        private DateTime _currentDate;

        double totEfficieny = 0;

        List<ProductionLine> pLine;
        List<Machines> list;

        public ViewProductionViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            _canExecute = true;
            metaData = md;
            machLargeMixer = new Machines(0);
            highSpeedMixer = new Machines(0);
            smallMixer = new Machines(0);
            logPeel = new Machines(0);
            csbrMachine = new Machines(0);
            slitter = new Machines(0);
            bagging = new Machines(0);
            rollUp = new Machines(0);
            rubberGrading = new Machines(0);
            shredding = new Machines(0);

            _userName = UserName;
            _state = State;
            _privilages = Privilages;

           _currentDate =Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"));

           timer = new DispatcherTimer();

          /*   List<ProductionLine> pLine = DBAccess.GetProductionDataByDate(_currentDate);

            List<Machines> list = DBAccess.GetProductionData(pLine);

            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine(list[i].MachineName + " " + list[i].Mixes);
            }

                 
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += delegate { timer_Tick(list); };
            timer.Start();*/
        }
        #region Geters and Setters

        /* User Name */

        public string UserName 
        {
            get { return _userName; }
            set 
            { 
                _userName = value;
             
            } 
        }
        /*Current Date*/
        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set
            {
                _currentDate = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CurrentDate"));                
            }
        }

        /*Factory Efficiency*/
        public double FactoryEfficiency
        {
            get { return _factoryEfficiency; }
            set
            {
                _factoryEfficiency = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FactoryEfficiency"));
                }
            }
        }

        /*Large Mixer*/
        public double LargeMixerMixes 
        {
            get 
            { 
                return _largeMixerMixes; 
            }
            set 
            { 
                _largeMixerMixes = value;
                if (PropertyChanged != null)
                {

                    PropertyChanged(this, new PropertyChangedEventArgs("LargeMixerMixes"));
                }
            }        
        }

        public double LargeMixerPeople
        {
            get
            {
                return _largeMixerEfficiency;
            }
            set
            {
                _largeMixerEfficiency = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LargeMixerPeople"));
                }
            }
        }

        public double LargeMixerEfficiency
        {
            get
            {
                return _largeMixerPeople;
            }
            set
            {
                _largeMixerPeople = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LargeMixerEfficiency"));
                }
            }
        }
        /*End of Large Mixer*/

        /*High Speed Mixer*/
        public double HighSpeedMixerMixes
        {
            get
            {
                return _highSpeedMixerMixes;
            }
            set
            {
                _highSpeedMixerMixes = value;
                if (PropertyChanged != null)
                {

                    PropertyChanged(this, new PropertyChangedEventArgs("HighSpeedMixerMixes"));
                }
            }
        }

        public double HighSpeedMixerPeople
        {
            get
            {
                return _highSpeedMixerPeople;
            }
            set
            {
                _highSpeedMixerPeople = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("HighSpeedMixerPeople"));
                }
            }
        }

        public double HighSpeedMixerEfficiency
        {
            get
            {
                return _highSpeedMixerEfficiency;
            }
            set
            {
                _highSpeedMixerEfficiency = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("HighSpeedMixerEfficiency"));
                }
            }
        }
        /*End of High Speed Mixer*/

        /*Small Mixer*/
        public double SmallMixerMixes
        {
            get
            {
                return _smallMixerMixes;
            }
            set
            {
                _smallMixerMixes = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SmallMixerMixes"));
                }
            }
        }

        public double SmallMixerPeople
        {
            get
            {
                return _smallMixerPeople;
            }
            set
            {
                _smallMixerPeople = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SmallMixerPeople"));
                }
            }
        }

        public double SmallMixerEfficiency
        {
            get
            {
                return _smallMixerEfficiency;
            }
            set
            {
                _smallMixerEfficiency = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SmallMixerEfficiency"));
                }
            }
        }
        /*End of High Speed Mixer*/

        /*Log and Peel*/
        public double LogMixerMixes
        {
            get
            {
                return _logMixerMixes;
            }
            set
            {
                _logMixerMixes = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LogMixerMixes"));

                }
            }
        }

        public double LogMixerPeople
        {
            get
            {
                return _logMixerPeople;
            }
            set
            {
                _logMixerPeople = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LogMixerPeople"));
                }
            }
        }

        public double LogMixerEfficiency
        {
            get
            {
                return _logMixerEfficiency;
            }
            set
            {
                _logMixerEfficiency = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LogMixerEfficiency"));
                }
            }
        }
        /*End of Log and Peel*/

        /*CSBR*/
        public double CSBRMixerMixes
        {
            get
            {
                
                return _csbrMixerMixes;
            }
            set
            {
                _csbrMixerMixes = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CSBRMixerMixes"));
                }
            }
        }

        public double CSBRMixerPeople
        {
            get
            {
                return _csbrMixerPeople;
            }
            set
            {
                _csbrMixerPeople = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CSBRMixerPeople"));
                }
            }
        }

        public double CSBRMixerEfficiency
        {
            get
            {
                return _csbrMixerEfficiency;
            }
            set
            {
                _csbrMixerEfficiency = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CSBRMixerEfficiency"));
                }
            }
        }
        /*End of CSBR*/

        /*Slitter*/
        public double SlitterMixerMixes
        {
            get
            {

                return _slitterMixerMixes;
            }
            set
            {
                _slitterMixerMixes = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SlitterMixerMixes"));
                }
            }
        }

        public double SlitterMixerPeople
        {
            get
            {
                return _slitterMixerPeople;
            }
            set
            {
                _slitterMixerPeople = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SlitterMixerPeople"));
                }
            }
        }

        public double SlitterMixerEfficiency
        {
            get
            {
                return _slitterMixerEfficiency;
            }
            set
            {
                _slitterMixerEfficiency = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SlitterMixerEfficiency"));
                }
            }
        }
        /*End of Slitter*/

        /*Slitter*/
        public double BaggingMixerMixes
        {
            get
            {

                return _baggingMixerMixes;
            }
            set
            {
                _baggingMixerMixes = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("BaggingMixerMixes"));
                }
            }
        }

        public double BaggingMixerPeople
        {
            get
            {
                return _baggingMixerPeople;
            }
            set
            {
                _baggingMixerPeople = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("BaggingMixerPeople"));
                }
            }
        }

        public double BaggingMixerEfficiency
        {
            get
            {
                return _baggingMixerEfficiency;
            }
            set
            {
                _baggingMixerEfficiency = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("BaggingMixerEfficiency"));
                }
            }
        }
        /*End of Slitter*/

        /*Roll Up Machine*/
        public double RollMixerMixes
        {
            get
            {

                return _rollMixerMixes;
            }
            set
            {
                _rollMixerMixes = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("RollMixerMixes"));
                }
            }
        }

        public double RollMixerPeople
        {
            get
            {
                return _rollMixerPeople;
            }
            set
            {
                _rollMixerPeople = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("RollMixerPeople"));
                }
            }
        }

        public double RollMixerEfficiency
        {
            get
            {
                return _rollMixerEfficiency;
            }
            set
            {
                _rollMixerEfficiency = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("RollMixerEfficiency"));
                }
            }
        }
        /*End of Roll Up Machine*/

        /*Rubber Grading*/
        public double GradingMixerMixes
        {
            get
            {

                return _gradingMixerMixes;
            }
            set
            {
                _gradingMixerMixes = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("GradingMixerMixes"));
                }
            }
        }

        public double GradingMixerPeople
        {
            get
            {
                return _gradingMixerPeople;
            }
            set
            {
                _gradingMixerPeople = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("GradingMixerPeople"));
                }
            }
        }

        public double GradingMixerEfficiency
        {
            get
            {
                return _gradingMixerEfficiency;
            }
            set
            {
                _gradingMixerEfficiency = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("GradingMixerEfficiency"));
                }
            }
        }
        /*End of Rubber Grading*/

        /*Shredding*/
        public double ShreddingMixerMixes
        {
            get
            {

                return _shreddingMixerMixes;
            }
            set
            {
                _shreddingMixerMixes = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ShreddingMixerMixes"));
                }
            }
        }

        public double ShreddingMixerPeople
        {
            get
            {
                return _shreddingMixerPeople;
            }
            set
            {
                _shreddingMixerPeople = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ShreddingMixerPeople"));
                }
            }
        }

        public double ShreddingMixerEfficiency
        {
            get
            {
                return _shreddingMixerEfficiency;
            }
            set
            {
                _shreddingMixerEfficiency = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ShreddingMixerEfficiency"));
                }
            }
        }
        /*End of Shredding*/

        #endregion

        #region Gauge Initializers

        //Large Mixer
        public Machines disLargeMixerData()
        {
            machLargeMixer = new Machines(0);
            machLargeMixer.Max = 13 ;
            machLargeMixer.Min = 0;
            machLargeMixer.MachineName = "Large Mixer";

            return machLargeMixer;
        }
        //High Speed Mixer
        public Machines disHighSpeedMixerData()
        {
            highSpeedMixer = new Machines(0);
            highSpeedMixer.Max = 60;
            highSpeedMixer.Min = 0;
            highSpeedMixer.MachineName = "High Speed Mixer";
            return highSpeedMixer;
        }

        //Small Mixer
        public Machines disSmallMixerData()
        {
            smallMixer = new Machines(0);
            smallMixer.Max = 18;
            smallMixer.Min = 0;
            smallMixer.MachineName = "Small Mixer";
            return smallMixer;
        }

        //Log and Peel
        public Machines disLogPeelData()
        {
            logPeel = new Machines(0);
            logPeel.Max = 40000;
            logPeel.Min = 0;
            logPeel.MachineName = "Log and Peel";
            return logPeel;
        }
        //CSBR Machine
        public Machines disCSBRData()
        {
            csbrMachine = new Machines(0);
            csbrMachine.Max = 20000;
            csbrMachine.Min = 0;
            csbrMachine.MachineName = "CSBR Machine";
            return csbrMachine;
        }

        //Slitter
        public Machines disSlitterData()
        {
            slitter = new Machines(0);
            slitter.Max = 40000;
            slitter.Min = 0;
            slitter.MachineName = "Slitter";
            return slitter;
        }

        //Bagging Machine
        public Machines disBaggingData()
        {
            bagging = new Machines(0);
            bagging.Max = 30000;
            bagging.Min = 0;
            bagging.MachineName = "Bagging Machine";
            return bagging;
        }

        //Rollup Machine
        public Machines disRollUpData()
        {
            rollUp = new Machines(0);
            rollUp.Max = 2000;
            rollUp.Min = 0;
            rollUp.MachineName = "Roll Up Machine";
            return rollUp;
        }

        //Rubber Grading
        public Machines disRubberGradingData()
        {
            rubberGrading = new Machines(0);
            rubberGrading.Max = 25000;
            rubberGrading.Min = 0;
            rubberGrading.MachineName = "Rubber Grading";
            return rubberGrading;
        }

        //Shredding
        public Machines disShreddingData()
        {
            shredding = new Machines(0);
            shredding.Max = 2000;
            shredding.Min = 0;
            shredding.MachineName = "Shredding";
            return shredding;
        }
        #endregion

        #region Timer

        void timer_Tick(List<ProductionLine> pLine)
        {

          
            Random r = new Random();
            totEfficieny = 0;

            list = new List<Machines>();          
            list = DBAccess.GetProductionData(pLine);

            for (int i = 0; i < list.Count; i++)
            {
             //   Console.WriteLine(list[i].MachineName + " " + list[i].Mixes);

                if (list[i].MachineName == "Large Mixer")
                {

                    double tot = 0;
                    for (int x = 0; x < list.Count; x++)
                    {
                        if (list[x].MachineName == "Large Mixer")
                        {
                            tot += list[x].Mixes;
                        }
                    }

                    LargeMixerMixes = tot;
                    machLargeMixer.Mixes = LargeMixerMixes;
                    LargeMixerPeople = 2;
                    
                }

                if (list[i].MachineName == "High Speed Mixer")
                {

                    double tot = 0;
                    for (int x = 0; x < list.Count; x++)
                    {
                         if (list[x].MachineName == "High Speed Mixer")
                         {
                             tot += list[x].Mixes;
                         }
                    }
                    HighSpeedMixerMixes = tot;
                    highSpeedMixer.Mixes = HighSpeedMixerMixes;
                    HighSpeedMixerPeople = 2;
                    
                }

                if (list[i].MachineName == "Small Mixer")
                {

                    double tot = 0;
                    for (int x = 0; x < list.Count; x++)
                    {
                        if (list[x].MachineName == "Small Mixer")
                        {
                            tot += list[x].Mixes;
                        }
                    }

                    SmallMixerMixes = tot;
                    smallMixer.Mixes = SmallMixerMixes;
                    SmallMixerPeople = 1;
                    
                }

                if (list[i].MachineName == "Log and Peel Machine")
                {
                    double tot = 0;
                    for (int x = 0; x < list.Count; x++)
                    {
                        if (list[x].MachineName == "Log and Peel Machine")
                        {
                            tot += list[x].Mixes;
                        }
                    }
                    LogMixerMixes = tot;
                    logPeel.Mixes = LogMixerMixes;
                    LogMixerPeople = 1;
                    
                }

                if (list[i].MachineName == "CSBR Machine")
                {
                    double tot = 0;
                    for (int x = 0; x < list.Count; x++)
                    {
                        if (list[x].MachineName == "CSBR Machine")
                        {
                            tot += list[x].Mixes;
                        }
                    }
                    CSBRMixerMixes = tot;
                    csbrMachine.Mixes = CSBRMixerMixes;
                    CSBRMixerPeople = 1;
                   
                }
                if (list[i].MachineName == "Slitter Machine")
                {
                    double tot = 0;
                    for (int x = 0; x < list.Count; x++)
                    {
                        if (list[x].MachineName == "Slitter Machine")
                        {
                            tot += list[x].Mixes;
                        }
                    }
                    SlitterMixerMixes = tot;
                    slitter.Mixes = SlitterMixerMixes;
                    SlitterMixerPeople = 1;
                    
                }

                if (list[i].MachineName == "Bagging Machine")
                {
                    double tot = 0;
                    for (int x = 0; x < list.Count; x++)
                    {
                        if (list[x].MachineName == "Bagging Machine")
                        {
                            tot += list[x].Mixes;
                        }
                    }
                    BaggingMixerMixes = tot;
                    bagging.Mixes = BaggingMixerMixes;
                    BaggingMixerPeople = 1;                    
                }

                if (list[i].MachineName == "Roll Up Machine")
                {
                    double tot = 0;
                    for (int x = 0; x < list.Count; x++)
                    {
                        if (list[x].MachineName == "Roll Up Machine")
                        {
                            tot += list[x].Mixes;
                        }
                    }

                    RollMixerMixes = tot;
                    rollUp.Mixes = RollMixerMixes;
                    RollMixerPeople = 1;                           
                }

                if (list[i].MachineName == "Rubber Grading Machine")
                {
                    double tot = 0;
                    for (int x = 0; x < list.Count; x++)
                    {
                        if (list[x].MachineName == "Rubber Grading Machine")
                        {
                            tot += list[x].Mixes;
                        }
                    }
                    GradingMixerMixes = tot;
                    rubberGrading.Mixes = tot;
                    GradingMixerPeople = 3;
                }

             
                if (list[i].MachineName == "Shredding Machine")
                {
                    double tot = 0;
                    for (int x = 0; x < list.Count; x++)
                    {
                        if (list[x].MachineName == "Shredding Machine")
                        {
                            tot += list[x].Mixes;
                        }
                    }
                    ShreddingMixerMixes = tot;
                    shredding.Mixes = ShreddingMixerMixes;
                    ShreddingMixerPeople = 1;      
                }
            }
            LargeMixerEfficiency = CalculateEffeciency(LargeMixerMixes, 20);
            HighSpeedMixerEfficiency = CalculateEffeciency(HighSpeedMixerMixes, 60);
            SmallMixerEfficiency = CalculateEffeciency(SmallMixerMixes, 25);
            LogMixerEfficiency = CalculateEffeciency(LogMixerMixes, 45000);
            CSBRMixerEfficiency = CalculateEffeciency(CSBRMixerMixes, 25000);
            SlitterMixerEfficiency = CalculateEffeciency(SlitterMixerMixes, 45000);
            BaggingMixerEfficiency = CalculateEffeciency(BaggingMixerMixes, 25000);
            RollMixerEfficiency = CalculateEffeciency(RollMixerMixes, 2000);             
            GradingMixerEfficiency = CalculateEffeciency(GradingMixerMixes, 35000);
            ShreddingMixerEfficiency = CalculateEffeciency(ShreddingMixerMixes, 3000);
        }

        #endregion

        public double CalculateEffeciency(double mixes, double maxMixes)
        {
            double efficiency = 0;

            efficiency = (mixes / maxMixes) * 100;
           
            CalcTotEfficiency(efficiency);

            return Math.Round(efficiency,0);            
        }
        public void CalcTotEfficiency(double eff)
        {
            
            totEfficieny = totEfficieny + eff;            

            FactoryEfficiency = Math.Round(totEfficieny / 10,0);

        }

        #region Commands
        public ICommand SearchData
        {
            get
            {
                return _searchData ?? (_searchData = new LogOutCommandHandler(() => ExecuteGauges(), _canExecute));
            }
        }
        
        public void ExecuteGauges()
        {
            ClearMixes();

            string selectedDate = CurrentDate.ToString("dd/MM/yyyy");

            pLine = new List<ProductionLine>();            
            pLine = DBAccess.GetProductionDataByDate(selectedDate);

            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += delegate { timer_Tick(pLine); };
            timer.Start();
        }

        public ICommand ClearGauges
        {
            get
            {
                return _clearGauges ?? (_clearGauges = new LogOutCommandHandler(() => ClearMixes(), _canExecute));
            }
        }


       

        public ICommand CommandsBack
        {
            get
            {
                return _commandsBack ?? (_commandsBack = new LogOutCommandHandler(() => GoBack(), _canExecute));
            }
        }
        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
        public ICommand NavProductionsCommand
        {
            get
            {
                return navProductionCommand ?? (navProductionCommand = new LogOutCommandHandler(() => Switcher.Switch(new ProductionMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
        public void ClearMixes()
        {

              
                timer.Stop();

                LargeMixerMixes = 0;
                machLargeMixer.Mixes = LargeMixerMixes;
                LargeMixerMixes = 0;
                LargeMixerPeople = 0;
                LargeMixerEfficiency = CalculateEffeciency(LargeMixerMixes, 20);

                HighSpeedMixerMixes = 0;
                highSpeedMixer.Mixes = HighSpeedMixerMixes;
                HighSpeedMixerMixes = 0;
                HighSpeedMixerPeople = 0;
                HighSpeedMixerEfficiency = CalculateEffeciency(HighSpeedMixerMixes, 60);

                SmallMixerMixes = 0;
                smallMixer.Mixes = SmallMixerMixes;
                SmallMixerMixes = 0;
                SmallMixerPeople = 1;
                SmallMixerEfficiency = CalculateEffeciency(SmallMixerMixes, 25);

                LogMixerMixes = 0;
                logPeel.Mixes = LogMixerMixes;
                LogMixerMixes = 0;
                LogMixerPeople = 0;
                LogMixerEfficiency = CalculateEffeciency(LogMixerMixes, 45000);

                CSBRMixerMixes = 0;
                csbrMachine.Mixes = CSBRMixerMixes;
                CSBRMixerMixes = 0;
                CSBRMixerPeople = 0;
                CSBRMixerEfficiency = CalculateEffeciency(CSBRMixerMixes, 25000);

                SlitterMixerMixes = 0;
                slitter.Mixes = SlitterMixerMixes;
                SlitterMixerMixes = 0;
                SlitterMixerPeople = 0;
                SlitterMixerEfficiency = CalculateEffeciency(SlitterMixerMixes, 45000);

                BaggingMixerMixes = 0;
                bagging.Mixes = BaggingMixerMixes;
                BaggingMixerMixes = 0;
                BaggingMixerPeople = 0;
                BaggingMixerEfficiency = CalculateEffeciency(BaggingMixerMixes, 25000);

                RollMixerMixes = 0;
                rollUp.Mixes = RollMixerMixes;
                RollMixerMixes = 0;
                RollMixerPeople = 0;
                RollMixerEfficiency = CalculateEffeciency(RollMixerMixes, 2000);

                GradingMixerMixes = 0;
                rubberGrading.Mixes = GradingMixerMixes;
                GradingMixerMixes = 0;
                GradingMixerPeople = 0;
                GradingMixerEfficiency = CalculateEffeciency(GradingMixerMixes, 35000);

                ShreddingMixerMixes = 0;
                shredding.Mixes = ShreddingMixerMixes;
                ShreddingMixerMixes = 0;
                ShreddingMixerPeople = 0;
                ShreddingMixerEfficiency = CalculateEffeciency(ShreddingMixerMixes, 3000);
          

        }

        public void GoBack()
        {
            Switcher.Switch(new ProductionMenuView(_userName, _state, _privilages, metaData));
        }


        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
