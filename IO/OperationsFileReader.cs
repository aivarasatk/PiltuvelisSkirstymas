using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IO
{
    public class OperationsFileReader : IOperationsReader
    {
        public Task<Dictionary<string, string>> OperationCodes(string operationsFilePath)
        {
            return Task.FromResult(new Dictionary<string,string>
            {
                ["GEN_PILSTYMAS"] = "010000046984",
                ["GEN_BLISTER"] = "010000047566",
                ["GEN"] = "TODO",
                ["GEN_ISORINIAI"] = "002000017166",
                ["GEN_PERKOLIACIJA"] = "002000017164",
                ["GEN_MAISTAS"] = "002000017165",
                ["GEN_TABLETYNAS"] = "002000017096"
            });
        }
    }
}
