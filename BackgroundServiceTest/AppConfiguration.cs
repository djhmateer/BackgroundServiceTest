using System;
using Microsoft.Extensions.Configuration;

namespace BackgroundServiceTest
{
    public class AppConfiguration
    {
        public string ConnectionString { get; }

        private AppConfiguration( string connectionString )
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public static AppConfiguration LoadFromEnvironment()
        {
            // This reads the ASPNETCORE_ENVIRONMENT flag from the system

            // set on production server via the dot net run command
            // set on development via the launchSettings.json file
            // set on Unit test projects via the TestBase
            var aspnetcore = "ASPNETCORE_ENVIRONMENT";
            var env = Environment.GetEnvironmentVariable(aspnetcore);

            string connectionString;
            switch (env)
            {
                case "Development":
                case "Test":
                    connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json")
                        .Build().GetConnectionString("Default");
                    break;
                case "Production":
                    connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.Production.json")
                        .Build().GetConnectionString("Default");
                    break;
                default:
                    throw new ArgumentException($"Expected {nameof(aspnetcore)} to be Development, Test or Production and it is {env}");
            }


         
            return new AppConfiguration(connectionString);
        }
    }

}
