using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace WebApplication8
{
    public static class Db
    {
        public static async Task<(DateTime? lastRun, DateTime? nextRun, int frequency)> 
            GetBudgetAlertLastRunAndFrequencyInMinutes(string connectionString)
        {
            using var conn = GetOpenConnection(connectionString);

            var result = await conn.QueryAsync<(DateTime?, DateTime?, int)>(@"
                select LastRun, NextRun, FrequencyInMinutes
                from BackgroundTask
                where BackgroundTaskId = 1 
                ");

            return result.Single();
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

        public static IDbConnection GetOpenConnection(string connectionString)
        {
            if (connectionString == null) throw new ArgumentException("connectionString can't be null");
            DbConnection cnn = new SqlConnection(connectionString);
            return cnn;
        }

        public static async Task UpdateBudgetAlertLastSuccessfulRunToNow(string connectionString, DateTime nextRun)
        {
            using var conn = GetOpenConnection(connectionString);

            var lastRun = DateTime.Now;

            await conn.ExecuteAsync(@"
                update BackgroundTask
                set LastRun = @LastRun
                NextRun = @NextRun
                ", new { lastRun, nextRun });
        }
    }

    public class BackgroundTask
    {
        public int BackgroundTaskId { get; set; }
        public string Name { get; set; }
        public DateTime? LastRunDateTime { get; set; }
        public int FrequencyInMinutes { get; set; }
    }
}
