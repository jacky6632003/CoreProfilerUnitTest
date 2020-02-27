using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoreProfilerUnitTest.RepositoryTests.Misc
{
    public class DatabaseCommands
    {
        /// <summary>
        /// 建立 Database.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="database">The database.</param>
        public static void CreateDatabase(string connectionString, string database)
        {
            var exists = DatabaseExists(connectionString, database);
            if (exists)
            {
                return;
            }

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sqlCommand = $"CREATE DATABASE [{database}];";
                conn.Execute(sqlCommand);
            }
        }

        /// <summary>
        /// Destroys the database (for localdb).
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dataBaseName">Name of the data base.</param>
        internal static void DestroyDatabase(string connectionString, string dataBaseName)
        {
            var queryCommand = $@"
                SELECT [physical_name] FROM [sys].[master_files]
                WHERE [database_id] = DB_ID('{dataBaseName}')";

            var fileNames = ExecuteSqlQuery
            (
                connectionString,
                string.Format(queryCommand, dataBaseName),
                row => (string)row["physical_name"]
            );

            var executeCommand = $@"
                ALTER DATABASE {dataBaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                EXEC sp_detach_db '{dataBaseName}', 'true'";

            if (fileNames.Any())
            {
                ExecuteSqlCommand
                (
                    connectionString,
                    string.Format(executeCommand, dataBaseName)
                );

                fileNames.ForEach(File.Delete);
            }

            var fileName = DatabaseFilePath(dataBaseName);
            try
            {
                var mdfPath = string.Concat(fileName, ".mdf");
                var ldfPath = string.Concat(fileName, "_log.ldf");

                var mdfExists = File.Exists(mdfPath);
                var ldfExists = File.Exists(ldfPath);

                if (mdfExists)
                {
                    File.Delete(mdfPath);
                }

                if (ldfExists)
                {
                    File.Delete(ldfPath);
                }
            }
            catch
            {
                Console.WriteLine("Could not delete the files (open in Visual Studio?)");
            }
        }

        /// <summary>
        /// 檢查指定的 Database 是否存在.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="database">The database.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool DatabaseExists(string connectionString, string database)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sqlCommand = new StringBuilder();
                sqlCommand.AppendLine($"if exists(select * from sys.databases where name = '{database}')");
                sqlCommand.AppendLine("select 'true'");
                sqlCommand.AppendLine("else ");
                sqlCommand.AppendLine("select 'false'");

                var result = conn.QueryFirstOrDefault<string>(sqlCommand.ToString());
                return result.Equals("true", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Databases the file path (for localdb).
        /// </summary>
        /// <param name="dataBaseName">Name of the data base.</param>
        /// <returns></returns>
        private static string DatabaseFilePath(string dataBaseName)
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = Path.Combine(directoryName, dataBaseName);
            return filePath;
        }

        /// <summary>
        /// Executes the SQL command (for localdb).
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandText">The command text.</param>
        private static void ExecuteSqlCommand(string connectionString, string commandText)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Executes the SQL query (for localdb).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="queryText">The query text.</param>
        /// <param name="read">The read.</param>
        /// <returns></returns>
        private static List<T> ExecuteSqlQuery<T>(string connectionString,
                                                         string queryText,
                                                         Func<SqlDataReader, T> read)
        {
            var result = new List<T>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryText;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(read(reader));
                        }
                    }
                }
            }

            return result;
        }
    }
}