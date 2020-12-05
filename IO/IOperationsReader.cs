using System.Collections.Generic;
using System.Threading.Tasks;

namespace IO
{
    public interface IOperationsReader
    {
        Task<Dictionary<string, string>> OperationCodes(string operationsFilePath);
    }
}
