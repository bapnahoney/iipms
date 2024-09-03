using Abp.AspNetCore.Dependency;
using Abp.Dependency;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace HIPMS.Web.Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //Honey
                    webBuilder.UseIIS();
                    webBuilder.UseIISIntegration();
                    webBuilder.UseStartup<Startup>();
                }).ConfigureServices(services =>
                {
                    // Disable insecure cipher suites
                    System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true; System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; System.Net.ServicePointManager.CheckCertificateRevocationList = false;
                })
                .UseCastleWindsor(IocManager.Instance.IocContainer);

    }
}
