using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace WebApplication8
{
    public static class Db
    {
        public static IDbConnection GetOpenConnection(string connectionString)
        {
            if (connectionString == null) throw new ArgumentException("connectionString can't be null");
            DbConnection cnn = new SqlConnection(connectionString);
            return cnn;
        }

        public static async Task<(DateTime lastRunStart, DateTime lastRunEnd)> GetBudgetAlert(string connectionString)
        {
            using var conn = GetOpenConnection(connectionString);

            var result = await conn.QueryAsync<(DateTime, DateTime)>(@"
                select LastRunStart, LastRunEnd
                from BackgroundTask
                where BackgroundTaskId = 1 
                ");

            return result.Single();
        }

        public static async Task UpdateBudgetAlertLastRunStart(string connectionString)
        {
            using var conn = GetOpenConnection(connectionString);

            await conn.ExecuteAsync(@"
                update BackgroundTask
                set LastRunStart = @LastRunStart
                where BackgroundTaskId = 1 
                ", new { lastRunStart = DateTime.Now });
        }


        public static async Task UpdateBudgetAlertLastRunEnd(string connectionString)
        {
            using var conn = GetOpenConnection(connectionString);

            await conn.ExecuteAsync(@"
                update BackgroundTask
                set LastRunEnd = @LastRunEnd
                where BackgroundTaskId = 1 
                ", new { lastRunEnd = DateTime.Now });
        }




        public static async Task<(DateTime lastRunStart, DateTime lastRunEnd)> GetBudgetAlertB(string connectionString)
        {
            using var conn = GetOpenConnection(connectionString);

            var result = await conn.QueryAsync<(DateTime, DateTime)>(@"
                select LastRunStart, LastRunEnd
                from BackgroundTask
                where BackgroundTaskId = 2 
                ");

            return result.Single();
        }

        public static async Task UpdateBudgetAlertLastRunStartB(string connectionString)
        {
            using var conn = GetOpenConnection(connectionString);

            await conn.ExecuteAsync(@"
                update BackgroundTask
                set LastRunStart = @LastRunStart
                where BackgroundTaskId = 2 
                ", new { lastRunStart = DateTime.Now });
        }


        public static async Task UpdateBudgetAlertLastRunEndB(string connectionString)
        {
            using var conn = GetOpenConnection(connectionString);

            await conn.ExecuteAsync(@"
                update BackgroundTask
                set LastRunEnd = @LastRunEnd
                where BackgroundTaskId = 2 
                ", new { lastRunEnd = DateTime.Now });
        }



        public static async Task<List<BackgroundTask?>> GetAllBackgroundTasks(string connectionString)
        {
            using var conn = GetOpenConnection(connectionString);

            var result = await conn.QueryAsync<BackgroundTask?>(@"
                select *
                from BackgroundTask
                ");

            return result.ToList();
        }


    }

    public class BackgroundTask
    {
        public int BackgroundTaskId { get; set; }
        public string Name { get; set; }
        public DateTime LastRunStart { get; set; }
        public DateTime LastRunEnd { get; set; }
    }
}
