﻿using IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PiltuvelisSkirstymas.Services.Config;
using PiltuvelisSkirstymas.Services.Mapper;
using PiltuvelisSkirstymas.ViewModels;
using Serilog;
using System.IO;
using System.IO.Abstractions;
using System.Windows;

namespace PiltuvelisSkirstymas
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
        private void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();

            var logger = new LoggerConfiguration()
                        .WriteTo.Async(conf =>
                            conf.File(
                                path: "Logs/log.txt",
                                rollingInterval: RollingInterval.Day,
                                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}]:[{Level:u3}]: {Message:lj}{NewLine}{Exception}"))
                        .CreateLogger();

            services.AddSingleton<MainViewModel>();

            services.AddSingleton<ILogger>(l => logger);

            services.AddSingleton<IEipReader, EipReader>();
            services.AddSingleton<IEipWriter, EipWriter>();

            services.AddSingleton<IConfig, Configuration>();
            services.AddSingleton<IMapper, Mapper>();
            services.AddSingleton<IConfiguration>(s => config);
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<MainWindow>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
