using A1QSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model
{
    public class MachineSelectedEmployees : ObservableObject
    {

        private string _empID;
        private string _empName;
        private double _empMixes;


        public string EmpID
        {
            get { return _empID; }
            set { _empID = value; RaisePropertyChanged(() => this.EmpID); }
        }

        public string EmpName
        {
            get { return _empName; }
            set { _empName = value; RaisePropertyChanged(() => this.EmpName); }
        }

        public double EmpMixes
        {
            get { return _empMixes; }
            set { _empMixes = value; RaisePropertyChanged(() => this.EmpMixes); }
        }
    }
}
