using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PiltuvelisSkirstymas.Services.Config
{
    public class Configuration : IConfig
    {
        private readonly IConfiguration _configuration;

        public IEnumerable<Maker> Makers { get; init; }

        public Configuration(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            Makers = InitMakers();
        }

        private IEnumerable<Maker> InitMakers()
        {
            return _configuration.GetSection("Makers")
                .GetChildren()
                .ToArray()
                .Select(m => new Maker(m.GetValue<string>("Key"), m.GetValue<string>("Value")));
        }

        
    }
}
