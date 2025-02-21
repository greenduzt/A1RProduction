using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core.Enumerations
{
    public enum OrderStatus
    {
        None = 0,
        Completed = 1,
        Canceled = 2,
        Pending = 3,
        Grading = 4,
        Mixing = 5,
        Curing = 6,
        Slitting = 7,
        Peeling = 8,
        Dispatched = 9,
        Bagging= 10,
        Allocated = 11,
        SlitPeel =12
    }

    public enum StockStatus
    {
        In,
        Out,
        NotAvailable
    }

    public enum ShiftType
    {
        Day = 1,
        Afternoon = 2,
        Night = 3
    }

    public enum VehicleWorkOrderEnum
    {
        Pending = 1,
        Completed=2,
        Cancelled=3
    }
    public enum VehicleWorkOrderTypesEnum
    {
        Maintenance = 1,
        Repair = 2
    }

    //public enum MachineMaintenanceFreq
    //{
    //    Daily = 1,
    //    Weekly = 2,
    //    OneMonth = 3,
    //    SixMonths = 4,
    //    OneYear = 5,
    //    TwoYears = 6,
    //    Daily_Weekly = 7,
    //    Daily_OneMonth = 8,
    //    Daily_SixMonths = 9,
    //    Daily_OneYear = 10,
    //    Daily_TwoYears = 11,
    //    Daily_Weekly_OneMonth = 12,
    //    Daily_Weekly_SixMonths = 13,
    //    Daily_Weekly_OneYear = 14,
    //    Daily_Weekly_TwoYears = 15,
    //    Daily_OneMonth_SixMonths = 16,
    //    Daily_OneMonth_OneYear = 17,
    //    Daily_OneMonth_TwoYears = 18,
    //    Daily_SixMonths_OneYear = 19,
    //    Daily_SixMonths_TwoYears = 20,
    //    Daily_OneYear_TwoYears = 21,
    //    Weekly_OneMonth = 22,
    //    Weekly_SixMonths = 23,
    //    Weekly_OneYear = 24,
    //    Weekly_TwoYears = 25,
    //    Weekly_OneMonth_SixMonths = 26,
    //    Weekly_OneMonth_OneYear = 27,
    //    Weekly_OneMonth_TwoYears = 28,
    //    Weekly_SixMonths_OneYear = 29,
    //    Weekly_SixMonths_TwoYears = 30,
    //    Weekly_OneYear_TwoYears = 31,
    //    OneMonth_SixMonths = 32,
    //    OneMonth_OneYear = 33,
    //    OneMonth_TwoYears = 34,
    //    OneMonth_SixMonths_OneYear = 35,
    //    OneMonth_SixMonths_TwoYears = 36,
    //    SixMonths_OneYear = 37,
    //    SixMonths_TwoYears = 38,
    //    OneYear_TwoYears = 39,
    //    OneOff = 40
    //}

    public enum MachineMaintenanceFreq
    {
        Daily = 1,
        Weekly = 2,
        OneMonth = 3,
        SixMonths = 4,
        OneYear = 5,
        TwoYears = 6,
        OneOff = 100
    }
}
