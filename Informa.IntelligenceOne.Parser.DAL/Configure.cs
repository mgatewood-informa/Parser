using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Informa.IntelligenceOne.Parser.DAL
{
    public static class Configure
    {
        public static void ConfigureServices(IServiceCollection services, string connectionString)
        {
            //context lifetime defaults to scoped
            services.AddDbContext<InformaContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IParserRepository, ParserRepository>();
        }
    }
}
