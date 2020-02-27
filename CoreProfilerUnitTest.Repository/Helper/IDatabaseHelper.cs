using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CoreProfilerUnitTest.Repository2.Helper
{
    public interface IDatabaseHelper
    {
        /// <summary>
        /// WLDO連線字串
        /// </summary>
        string WLDOConnectionString { get; }

        /// <summary>
        /// MySQL
        /// </summary>
        string MySQLConnectionString { get; }

        IDbConnection GetConnection(string connectionString);

        IDbConnection GetMySQLConnection(string connectionString);
    }
}