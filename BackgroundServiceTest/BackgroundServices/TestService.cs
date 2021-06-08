using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace WebApplication8.BackgroundServices
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
                    // Do we need to run BudgetAlert
                    // lets assume it needs to run once every day
                    //  and we start trying at a certain time.

                    // if webserver is down, it will pick up where left off, and try and do only 1 run


                    // testing task1
                    // want it to run once every 5 minutes starting immediately
                    // starting at 1330
                    var (lastRunStart, lastRunEnd) = await Db.GetBudgetAlert(connectionString);

                    bool shouldRun = false;
                    var frequencyInSeconds = 10;

                    // happy path
                    if (DateTime.Now > lastRunEnd.AddSeconds(frequencyInSeconds)) shouldRun = true;

                    if (shouldRun)
                    {
                        Log.Information("Start task 1");
                        await Db.UpdateBudgetAlertLastRunStart(connectionString);
                        //await Task.Delay(10000, stoppingToken); // doing work, maybe sending emails
                        await Foo.Bar(stoppingToken);

                        Log.Information("End task 1");
                        await Db.UpdateBudgetAlertLastRunEnd(connectionString);
                    }

                    // testing task2
                    // **HERE** maybe we want this to start at 14:45:00 and run every minute at 00 seconds
                    var (_, lastRunEndB) = await Db.GetBudgetAlertB(connectionString);

                    bool shouldRunB = false;
                    var frequencyInSecondsB = 10;

                    if (DateTime.Now > lastRunEndB.AddSeconds(frequencyInSecondsB)) shouldRunB = true;

                    if (shouldRunB)
                    {
                        Log.Information("Start task 2");
                        await Db.UpdateBudgetAlertLastRunStartB(connectionString);
                        await Foo.BarB(stoppingToken);

                        Log.Information("End task 2");
                        await Db.UpdateBudgetAlertLastRunEndB(connectionString);
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
        public static async Task Bar(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Delay(5000, stoppingToken);
                throw new ApplicationException("blow up - our system should be able to handle this and retry");
                //throw new InvalidOperationException("Operation is not valid due to Bar being in a bad state");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Bar task threw an exception, but we want it to retry the next time it is due to run");
            }
        }

        public static async Task BarB(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Delay(5000, stoppingToken);
                throw new ApplicationException("blow upB - our system should be able to handle this and retry");
                //throw new InvalidOperationException("Operation is not valid due to Bar being in a bad state");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "BarB task threw an exception, but we want it to retry the next time it is due to run");
            }
        }
    }
}
