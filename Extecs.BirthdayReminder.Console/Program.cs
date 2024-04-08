using Extecs.BirthdayReminder.CLI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static System.Net.Mime.MediaTypeNames;

namespace Extecs.BirthdayReminder.CLI
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Dependency Injection setup
            IHost _host = Host.CreateDefaultBuilder().ConfigureServices(
                services =>
                {
                    IServiceCollection serviceCollection = services.AddSingleton<IApplication, Services.Application>();
                })
            .Build();

            var app = _host.Services.GetRequiredService<IApplication>();
            app.Run();

        }
    }
}
