using IO.Dto;
using PiltuvelisSkirstymas.Services.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PiltuvelisSkirstymas.Services.Mapper
{
    public class Mapper : IMapper
    {
        private readonly IConfig _config;
        public Mapper(IConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Transforms input objects into output ready list by exploding each product based on maker count.
        /// </summary>
        /// <param name="input">Product list from GEN file</param>
        /// <param name="operationCodes">Operation code list containing verbose maker keys and reference code values</param>
        /// <returns>List of exploded products ready for import</returns>
        /// <exception cref="ArgumentException">Throws if any of the input values does contain valid Maker (xml I07_KODAS_IS) keys </exception>
        public IEnumerable<I07Output> MapToOutput(IEnumerable<I07Input> input)
        {
            return ToOutput(input, _config.Makers);
        }

        private IEnumerable<I07Output> ToOutput(IEnumerable<I07Input> input, 
            IEnumerable<Maker> mappedOperationCodes)
        {
            var output = new List<I07Output>();
            foreach(var product in input)
            {
                var explodedProducts = Explode(product, mappedOperationCodes);

                if (!explodedProducts.Any())
                    throw new ArgumentException($"Product {product.Code.First()} does not contain any of the valid <I07_KODAS_IS> values");

                output.AddRange(explodedProducts);
            }
            return output;
        }

        /// <summary>
        /// "Explodes" one product containing several maker departments into many products for each department
        /// </summary>
        /// <param name="product"></param>
        /// <returns>Enumerable of I07 based on department count</returns>
        private IEnumerable<I07Output> Explode(I07Input product, IEnumerable<Maker> mappedOperationCodes)
        {
            foreach(var (key, operationCode) in mappedOperationCodes)
            {
                if (!product.Maker.Contains(key))
                    continue;

                var output = new I07Output(product);
                output.ReferenceNumber = operationCode;

                yield return output;
            }
        }
    }
}
