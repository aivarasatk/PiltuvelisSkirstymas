using System.Collections.Generic;

namespace PiltuvelisSkirstymas.Services.Config
{
    public interface IConfig
    {
        IEnumerable<Maker> Makers { get; init; }
    }

    public record Maker(string Key, string OperationCode);
}
