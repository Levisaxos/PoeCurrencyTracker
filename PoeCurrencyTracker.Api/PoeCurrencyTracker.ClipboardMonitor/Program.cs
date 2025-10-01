using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using PoeCurrencyTracker.ClipboardMonitor.Interface;
using PoeCurrencyTracker.ClipboardMonitor.Model;
using PoeCurrencyTracker.ClipboardMonitor.UseCases;
using System.Reflection;
using static System.Formats.Asn1.AsnWriter;

namespace PoeCurrencyTracker.ClipboardMonitor
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            // Configuration
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            // Register configuration
            builder.Services.Configure<ClipboardMonitorOptions>(
                builder.Configuration.GetSection(ClipboardMonitorOptions.SectionName));

            // Register use cases
            builder.Services.AddUseCases();

            var host = builder.Build();

            using var scope = host.Services.CreateScope();
            var clipboardMonitorUseCase = scope.ServiceProvider.GetRequiredService<IClipboardMonitorUseCase>();
            // Your clipboard monitoring logic here            
            clipboardMonitorUseCase.Handle();
        }

    }
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var interfaces = assembly.GetTypes()
                .Where(t => t.IsInterface && t.Name.EndsWith("UseCase"))
                .ToList();

            foreach (var interfaceType in interfaces)
            {
                var implementationType = assembly.GetTypes()
                    .FirstOrDefault(t => t.IsClass &&
                                       !t.IsAbstract &&
                                       interfaceType.IsAssignableFrom(t));

                if (implementationType != null)
                {
                                       
                        services.AddScoped(interfaceType, implementationType);
                        Console.WriteLine($"Registered as Scoped: {interfaceType.Name} -> {implementationType.Name}");                    
                }
            }

            return services;
        }
    }
}