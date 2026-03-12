using System;
using Microsoft.Data.SqlClient;

namespace RepositoryLayer.Orm
{
    public static class QuantityMeasurementOrmConnectionString
    {
        public static string BuildOrmConnectionString(string baseConnectionString, string ormDatabaseName)
        {
            if (string.IsNullOrWhiteSpace(baseConnectionString))
            {
                throw new ArgumentNullException(nameof(baseConnectionString));
            }

            if (string.IsNullOrWhiteSpace(ormDatabaseName))
            {
                throw new ArgumentNullException(nameof(ormDatabaseName));
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(baseConnectionString)
            {
                InitialCatalog = ormDatabaseName
            };

            return builder.ConnectionString;
        }
    }
}