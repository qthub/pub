using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace qtee.ServerApp
{
    public class Config
    {
        private static IConfiguration Configuration { get; set; }

        public static string DbConnectionForApplication => Configuration["DbConnectionForApplication"];

        public static void Init()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }
    }
}
