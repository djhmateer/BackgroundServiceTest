using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace BackgroundServiceTest
{
    public static class Db
    {
        public static IDbConnection GetOpenConnection(string connectionString)
        {
            if (connectionString == null) throw new ArgumentException("connectionString can't be null");
            DbConnection cnn = new SqlConnection(connectionString);
            return cnn;
        }

        public static async Task<(DateTime lastRunEnd, DateTime runEveryDayAfter)> GetTaskById(string connectionString, int taskId)
        {
            using var conn = GetOpenConnection(connectionString);

            var result = await conn.QueryAsync<(DateTime, DateTime)>(@"
                select LastRunEnd, CONVERT(DATETIME, RunEveryDayAfter) as RunEveryDayAfter
                from BackgroundTask
                where BackgroundTaskId = @TaskId
                ", new { taskId });

            // 01-01-1900 if RunEveryDayAfter DateTime date part
            // C# getting a time only representation in C#10
            return result.Single();
        }

        public static async Task UpdateTaskLastRunStartAndStatusIdToRunning(string connectionString, int taskId)
        {
            using var conn = GetOpenConnection(connectionString);

            var taskStatusRunning = TaskStatus.Running;

            await conn.ExecuteAsync(@"
                update BackgroundTask
                set LastRunStart = @LastRunStart,
                    LastStatusId = @TaskStatusRunning
                where BackgroundTaskId = @TaskID
                ", new { lastRunStart = DateTime.Now, taskId, taskStatusRunning });
        }


        public static async Task UpdateTaskLastRunEndAndStatusIdToCompletedOrException(string connectionString, int taskId, bool success)
        {
            using var conn = GetOpenConnection(connectionString);

            var taskStatus = success ? TaskStatus.Completed : TaskStatus.Error;

            await conn.ExecuteAsync(@"
                update BackgroundTask
                set LastRunEnd = @LastRunEnd,
                    LastStatusId = @TaskStatus
                where BackgroundTaskId = @TaskId
                ", new { lastRunEnd = DateTime.Now, taskId, taskStatus });
        }

        public static class TaskStatus
        {
            public const int NeverRan = 1;
            public const int Running = 2;
            public const int Completed = 3;
            public const int Error = 9;
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
