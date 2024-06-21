using System.IO;
using Microsoft.Extensions.Configuration;

namespace DipScooper
{
    internal static class Program
    {
        public static IConfiguration Configuration { get; private set; }

        static void Main()
        {
            InitializeConfiguration();
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        static void InitializeConfiguration()
        {
            // Bygg konfigurasjonen fra appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }
    }
}