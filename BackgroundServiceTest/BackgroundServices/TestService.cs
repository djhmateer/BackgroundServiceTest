using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BackgroundServiceTest.BackgroundServices
{
    public class TestService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(2000, stoppingToken);
            Log.Information("Started Background Service");
            var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    //{
                    //    // Task1 - Budget Alert
                    //    // want it to run once per day at 0950 
                    //    var taskId = 1;
                    //    var (lastRunEnd, runEveryDayAfter) = await Db.GetTaskById(connectionString, taskId);

                    //    var today = DateTime.Now.Date;

                    //    // has the task run today already?
                    //    if (lastRunEnd.Date == today) { }
                    //    else
                    //    {
                    //        var todayWhenToRunTaskAfter = new DateTime(today.Year, today.Month, today.Day,
                    //            runEveryDayAfter.Hour, runEveryDayAfter.Minute, runEveryDayAfter.Second);

                    //        // have we gone past the time when it should run today?
                    //        bool shouldRun = DateTime.Now > todayWhenToRunTaskAfter;

                    //        if (shouldRun)
                    //        {
                    //            await Db.UpdateTaskLastRunStartAndStatusIdToRunning(connectionString, taskId);
                    //            var success = await Foo.Task1(stoppingToken);

                    //            await Db.UpdateTaskLastRunEndAndStatusIdToCompletedOrException(connectionString, taskId, success);
                    //        }
                    //    }
                    //}

                    {
                        // Task 2 - continuous every 1 minute
                        var taskId = 2;
                        var (lastRunEnd, _) = await Db.GetTaskById(connectionString, taskId);

                        var frequencyInSeconds = 60;

                        bool shouldRun = DateTime.Now > lastRunEnd.AddSeconds(frequencyInSeconds);

                        if (shouldRun)
                        {
                            await Db.UpdateTaskLastRunStartAndStatusIdToRunning(connectionString, taskId);
                            var success = await Foo.Task2(stoppingToken);

                            await Db.UpdateTaskLastRunEndAndStatusIdToCompletedOrException(connectionString, taskId, success);
                        }
                    }

                    Log.Information("ping");
                    await Task.Delay(5000, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                Log.Warning("Operation cancelled - can happen when app is shutting down gracefully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
            }
            Log.Warning("Background Service stopped");
        }
    }

    public static class Foo
    {
        public static async Task<bool> Task1(CancellationToken stoppingToken)
        {
            try
            {
                Log.Information("Start task 1");
                await Task.Delay(5000, stoppingToken);
                Log.Information("End task 1");
                //throw new ApplicationException("blow up - our system should be able to handle this and retry");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Task1 task threw an exception, but we want it to retry the next time it is due to run");
                return false;
            }
            return true;
        }

        public static async Task<bool> Task2(CancellationToken stoppingToken)
        {
            try
            {
                Log.Information("Start task 2");
                await Task.Delay(5000, stoppingToken);
                Log.Information("End task 2");
                //throw new ApplicationException("blow upB - our system should be able to handle this and retry");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Task2 task threw an exception, but we want it to retry the next time it is due to run");
                return false;
            }
            return true;
        }
    }
}
