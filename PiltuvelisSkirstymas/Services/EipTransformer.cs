using IO.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PiltuvelisSkirstymas.Services
{
    public static class EipTransformer
    {
        private static Dictionary<string, string> _codeDictionary = new()
        {
            ["V1"] = "010000046984",
            ["V2"] = "010000047566",
            ["V3"] = "TODO",
            ["K1"] = "002000017166",
            ["K2"] = "002000017164",
            ["K3"] = "002000017165",
            ["T3"] = "002000017096"
        };

        /// <summary>
        /// Transforms input objects into output ready list.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws if any of the input values does contain valid Maker (xml I07_KODAS_IS) keys </exception>
        public static IEnumerable<I07Output> ToOutput(IEnumerable<I07Input> input, int lineStart)
        {
            var output = new List<I07Output>();
            foreach(var product in input)
            {
                if (product.LineNr < lineStart)
                    continue;

                var explodedProducts = Explode(product);

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
        private static IEnumerable<I07Output> Explode(I07Input product)
        {
            foreach(var (key, value) in _codeDictionary)
            {
                if (!product.Maker.Contains(key))
                    continue;

                var output = new I07Output(product);
                output.ReferenceNumber = value;

                yield return output;
            }
        }
    }
}
