using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography.X509Certificates;

namespace HomeNetAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var certificate = new X509Certificate2("Certificates/homenet.pfx", "Okuhle*1994");
            var host = new WebHostBuilder()
                .UseKestrel(options => options.UseHttps(certificate))
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}
