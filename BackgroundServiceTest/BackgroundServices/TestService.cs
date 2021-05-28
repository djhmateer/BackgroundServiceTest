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
            var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

            while (!stoppingToken.IsCancellationRequested)
            {
                // Do we need to run BudgetAlert
                var (lastRun, nextRun, frequencyInMinutes) = await Db.GetBudgetAlertLastRunAndFrequencyInMinutes(connectionString);

                // **HERE**

                bool shouldRun = false;
                if (DateTime.Now > nextRun) shouldRun = true;



                // task has never been run
                if (lastRun == null) shouldRun = true;
                else
                {
                    // nextRun will be not null
                    if (DateTime.Now > nextRun) shouldRun = true;
                }

                if (shouldRun)
                {
                    Log.Information("Running task");
                    await Task.Delay(2000, stoppingToken); // doing work, maybe sending emails

                    if (lastRun == null)
                    {
                        lastRun = DateTime.Now;
                        nextRun = DateTime.Now.AddMinutes(frequencyInMinutes);
                    }
                    else
                    {
                        // lastrun is not null
                        nextRun = ((DateTime) lastRun).AddMinutes(frequencyInMinutes);

                        if (nextRun < DateTime.Now)
                        {
                            //something strange has happened and the next run time is in the past
                            // so skip those, and go x in the future now.
                            nextRun = DateTime.Now.AddMinutes(frequencyInMinutes);
                        }

                    }
                    await Db.UpdateBudgetAlertLastSuccessfulRunToNow(connectionString, (DateTime)nextRun);
                }

                //Log.Information($"connectionstring {connectionString}");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
