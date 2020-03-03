using Informa.IntelligenceOne.Parser;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Parser
{
    public class Program
    {
        static void Main(string[] args)
        {
            
            FileParser parser = new FileParser();
            parser.Parse(@"C:\My.Net\Informa\Parser\Spreadsheets\_intranet_rates_Template.xlsx");

            //CreateHostBuilder(args).Build().Run();
 
        }

        /*public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }*/
    }
}
