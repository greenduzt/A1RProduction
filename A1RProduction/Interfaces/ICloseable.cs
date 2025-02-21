using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Interfaces
{
    interface ICloseable
    {
        event EventHandler<EventArgs> RequestClose;
    }
}
