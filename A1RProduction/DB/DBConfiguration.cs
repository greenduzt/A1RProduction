using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.DB
{
    public class DBConfiguration
    {
        private static string dbConnectionString;
        private static string dbProviderName;

        static DBConfiguration()
        {
            dbConnectionString = A1QSystem.Properties.Settings.Default.Northwind;
        }

        public static string DbConnectionString
        {
            get { return dbConnectionString; }
        }

        public static string DbProviderName
        {
            get { return dbProviderName; }
        }

    }
}
