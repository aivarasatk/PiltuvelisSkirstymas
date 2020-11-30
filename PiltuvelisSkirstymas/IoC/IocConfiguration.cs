using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiltuvelisSkirstymas.ViewModels;
using PiltuvelisSkirstymas.Services.Logger;

namespace PiltuvelisSkirstymas.IoC
{
    public static class IocConfiguration
    {
        public static void Configure()
        {
            Ioc.Default.ConfigureServices(new ServiceCollection()
                .AddSingleton<MainViewModel>()
                .AddSingleton<ILogService, LogService>()
                .AddSingleton<IEipReader, EipReader>()
                .AddSingleton<IEipWriter, EipWriter>()
                .BuildServiceProvider());
        }
    }
}
