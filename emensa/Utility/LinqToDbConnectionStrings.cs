using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using LinqToDB.Configuration;

namespace emensa.Utility
{
    public class ConnectionStringSettings : IConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public bool IsGlobal => false;
    }

    public class LinqToDbSettings : ILinqToDBSettings
    {
        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

        public string DefaultConfiguration => "MySql.Data.MySqlClient";
        public string DefaultDataProvider => "MySql.Data.MySqlClient";

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return
                    new ConnectionStringSettings
                    {
                        Name = "emensa",
                        ProviderName = "MySql.Data.MySqlClient",
                        ConnectionString = @"Server=localhost;Database=emensa;Uid=root;Pwd=password;"
                    };
            }
        }
    }
    
    
}