using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;

using log4net;

namespace MigrationTool.Services.Helpers
{
    internal static class DbHelper
    {
        public static IEnumerable<TEntity> QuerySql<TEntity>(
            DbProviderFactory provider,
            string connectionString,
            string[] fields,
            string table,
            string where,
            Func<DbDataReader, Dictionary<string, int>, TEntity> create,
            ILog log)
        {
            var datasql = CreateSql(fields, table, where);

            return QuerySql(provider, connectionString, datasql, create, log);
        }

        private static IEnumerable<TEntity> QuerySql<TEntity>(
            DbProviderFactory provider,
            string connectionString,
            string sql,
            Func<DbDataReader, Dictionary<string, int>, TEntity> create,
            ILog log)
        {
            using (var connection = CreateConnection(provider, connectionString))
            {
                OpenConnection(connection);
                using (var command = CreateCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var columns = Enumerable.Range(0, reader.FieldCount).ToDictionary((index) => reader.GetName(index), StringComparer.InvariantCultureIgnoreCase);

                        while (reader.Read())
                        {
                            TEntity entity;
                            if (TryCreateEntity(() => create(reader, columns), log, out entity))
                            {
                                yield return entity;
                            }
                        }
                    }
                }
            }
        }

        private static string CreateSql(string[] fields, string table, string where)
        {
            return string.IsNullOrWhiteSpace(where)
                ? string.Format("select {0} from {1}", string.Join(",", fields), table)
                : string.Format("select {0} from {1} where {2}", string.Join(",", fields), table, where);
        }

        private static DbConnection CreateConnection(DbProviderFactory provider, string connectionString)
        {
            var connection = provider.CreateConnection();
            connection.ConnectionString = connectionString;

            return connection;
        }

        private static DbCommand CreateCommand(string sql, DbConnection conn)
        {
            var command = conn.CreateCommand();
            command.CommandText = sql;

            return command;
        }

        private static bool TryCreateEntity<TEntity>(Func<TEntity> create, ILog log, out TEntity entity)
        {
            try
            {
                entity = create();
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Failed to create entity.", ex);
                entity = default(TEntity);
                return false;
            }
        }

        private static void OpenConnection(IDbConnection connection)
        {
            while (true)
            {
                try
                {
                    connection.Open();
                    break;
                }
                catch (Exception)
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}