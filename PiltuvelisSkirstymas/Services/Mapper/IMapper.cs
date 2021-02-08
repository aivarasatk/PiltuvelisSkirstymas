using IO.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiltuvelisSkirstymas.Services.Mapper
{
    public interface IMapper
    {
        IEnumerable<I07Output> MapToOutput(IEnumerable<I07Input> input);
    }
}
