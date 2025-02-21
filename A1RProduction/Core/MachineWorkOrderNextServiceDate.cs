using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{
    public static class MachineWorkOrderNextServiceDate
    {
        public static List<Tuple<Int32,int, string, DateTime, DateTime, DateTime>> CalculateNextServiceDate(int mid, int num)
        {
            List<Tuple<Int32, int, string, DateTime, DateTime, DateTime>> dates = new List<Tuple<Int32,int, string, DateTime, DateTime, DateTime>>();
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            DateTime currentDate = DateTime.Now;
            DateTime nextDate = currentDate.AddDays(1);
            DateTime d = currentDate;
            List<int> listOfInts = new List<int>();

            while (num > 0)
            {
                listOfInts.Add(num % 10);
                num = num / 10;
            }
            listOfInts.Reverse();

            foreach (var item in listOfInts)
            {
                switch (item)
                {
                   
                    case 1: dates.Add(Tuple.Create(mid, item, "Daily", nextDate, bdg.SkipWeekends(currentDate.AddDays(1)), bdg.SkipWeekends(currentDate.AddDays(1))));//Daily
                        break;
                    case 2: List<Tuple<DateTime, DateTime>> tup= GetNextTwoWeeks();                         
                            dates.Add(Tuple.Create(mid, item, "Weekly", tup[0].Item1, bdg.SkipWeekends(currentDate.AddDays(7)), tup[0].Item2));//Weekly
                        break;
                    case 3: dates.Add(Tuple.Create(mid, item, "Monthly", nextDate, bdg.SkipWeekends(currentDate.AddMonths(1)), bdg.SkipWeekends(currentDate.AddMonths(2))));//Monthly
                        break;
                    case 4: dates.Add(Tuple.Create(mid, item, "SixMonths", nextDate, bdg.SkipWeekends(currentDate.AddMonths(6)), bdg.SkipWeekends(currentDate.AddMonths(12))));//Six Months
                        break;
                    case 5: dates.Add(Tuple.Create(mid, item, "OneYear", nextDate, bdg.SkipWeekends(currentDate.AddYears(1)), bdg.SkipWeekends(currentDate.AddYears(2))));//One Year
                        break;
                    case 6: dates.Add(Tuple.Create(mid, item, "TwoYears", nextDate, bdg.SkipWeekends(currentDate.AddYears(2)), bdg.SkipWeekends(currentDate.AddYears(4))));//Two Years
                        break;
                }
            }

            return dates;
        }

        public static List<Tuple<DateTime, DateTime>> GetNextTwoWeeks()
        {
            List<Tuple<DateTime, DateTime>> tup = new List<Tuple<DateTime, DateTime>>();
            DateTime[] fiveDates = new DateTime[5];
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            DateTime nextMonday = bdg.GetNextDateByDay(DayOfWeek.Monday);
            for (int i = 0; i < 5; i++)
            {
                fiveDates[i] = nextMonday;
                nextMonday = bdg.SkipWeekends(nextMonday.AddDays(1));
            }


            DateTime maxDate = fiveDates.Max(r => r);
            DateTime minDate = fiveDates.Min(r => r);

            tup.Add(Tuple.Create(minDate,maxDate));

            return tup;
        }
    }
}
