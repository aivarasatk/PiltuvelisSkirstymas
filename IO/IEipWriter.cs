using IO.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO
{
    public interface IEipWriter
    {
        Task WriteAsync(I06Output output);
    }
}
