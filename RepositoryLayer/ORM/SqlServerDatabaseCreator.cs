using System;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace RepositoryLayer.Orm
{
    public static class SqlServerDatabaseCreator
    {
        private static readonly Regex SafeDatabaseNameRegex = new Regex("^[A-Za-z0-9_]+$", RegexOptions.Compiled);

        public static void EnsureDatabaseExists(string baseConnectionString, string databaseName)
        {
            if (string.IsNullOrWhiteSpace(baseConnectionString))
            {
                throw new ArgumentNullException(nameof(baseConnectionString));
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentNullException(nameof(databaseName));
            }

            if (!SafeDatabaseNameRegex.IsMatch(databaseName))
            {
                throw new ArgumentException(
                    "Database name contains unsupported characters. Allowed: letters, digits, underscore.",
                    nameof(databaseName));
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(baseConnectionString)
            {
                InitialCatalog = "master"
            };

            using SqlConnection sqlConnection = new SqlConnection(builder.ConnectionString);
            sqlConnection.Open();

            using SqlCommand sqlCommand = new SqlCommand(@"
                IF DB_ID(@DatabaseName) IS NULL
                BEGIN
                    DECLARE @Sql NVARCHAR(MAX) = N'CREATE DATABASE ' + QUOTENAME(@DatabaseName) + N';';
                    EXEC (@Sql);
                END
                ", sqlConnection);

            sqlCommand.Parameters.AddWithValue("@DatabaseName", databaseName);

            _ = sqlCommand.ExecuteNonQuery();
        }
    }
}