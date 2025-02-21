using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class DBConfiguration
    {
        private static string dbConnectionString;
        private static string dbProviderName;

        static DBConfiguration()
        {
            dbConnectionString = DBConnection.Properties.Settings.Default.A1RubberConnection;
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
