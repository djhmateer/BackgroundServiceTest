using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BackgroundServiceTest.BackgroundServices
{
    public class Test2Service : BackgroundService
    {
        private readonly Test2Channel _test2Channel;
        public Test2Service(Test2Channel test2Channel) => _test2Channel = test2Channel;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Started Test2Service");

            await foreach (var filePathAndName in _test2Channel.ReadAllAsync())
            {
                Log.Information($"got a message from the channel {filePathAndName}");
                try
                {
                    Log.Information("inside try");

                    // if an exception is thrown, then next in the channel will not be read
                    throw new ApplicationException("TEST exception.. ");
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "HS - Job failed - in overall catch");

                    // very bad as the background service will 'stall'
                    // the host will still be running
                    // but we are outside of the await foreach now
                    // so reader is stopped
                    throw new ApplicationException("maybe a db call fails");
                    Log.Information("unreachable");
                }
                finally
                {
                    Log.Information("inside finally");
                }

                Log.Information("unreachable");
            }
        }
    }
}
