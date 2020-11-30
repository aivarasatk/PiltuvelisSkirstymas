using IO.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IO
{
    public interface IEipReader
    {
        Task<IEnumerable<I07Input>> GetParsedEipContentsAsync(string filePath);
    }
}
