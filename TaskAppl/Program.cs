// using NLog;
// using NLog.Web;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace TaskAppl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Возможно подключение логирования
            //Logger logger = LogManager
            //    .Setup()
            //    .LoadConfigurationFromXml("nlog.config")
            //    .GetCurrentClassLogger();

            try
            {
                // logger.Debug("init main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                // logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // LogManager.Shutdown();
            }
        }


        /// <summary>
        /// Program / CreateHostBuilder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    string envName = env.EnvironmentName;
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config
                        .AddJsonFile(Path.Combine("Configurations", "appsettings.json"), optional: true, reloadOnChange: true)
                        .AddJsonFile(Path.Combine("Configurations", $"appsettings.{envName}.json"), optional: true, reloadOnChange: true);
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = long.MaxValue;
                });
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            });
            // .UseNLog();
    }
}
