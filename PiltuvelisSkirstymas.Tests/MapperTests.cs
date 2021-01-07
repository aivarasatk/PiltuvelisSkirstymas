using Moq;
using Xunit;
using PiltuvelisSkirstymas.Services.Config;
using PiltuvelisSkirstymas.Services.Mapper;
using System;
using IO.Dto;
using System.Collections.Generic;
using System.Linq;

namespace PiltuvelisSkirstymas.Tests
{
    public class MapperTests
    {
        [Fact]
        public void When_OperationCodesDontMap_Result_ThrowsArgumentException()
        {
            //Arrange
            var configMock = new Mock<IConfig>();
            configMock.Setup(c => c.Makers).Returns(new[] { new Maker("T1", "V1"), new Maker("T2", "V2") });

            var mapper = new Mapper(configMock.Object);

            var input = new[]
            {
                new I07Input
                {
                    Code = new [] { "sdasad" },
                    LineNr = 1,
                    Name = "pav1",
                    Amount = 1,
                    Maker = "T1",
                    DateDateTime = new DateTime(2020,3,1),
                    Details1 = "",
                    Details2 = "",
                    Details3 = "",
                    _pap2 = "1",
                    DimDateDateTime = default
                }
            };

            var operationCodes = new Dictionary<string, string>
            {
                ["K1"] = "125458",
                ["K2"] = "127475458",
            };

            //Act & Assert
            Assert.Throws<ArgumentException>(() => mapper.MapToOutput(input, operationCodes));
        }

        [Fact]
        public void When_InputCannotBeExploded_Result_ThrowsArgumentException()
        {
            //Arrange
            var configMock = new Mock<IConfig>();
            configMock.Setup(c => c.Makers).Returns(new[] { new Maker("T1", "V1"), new Maker("T2", "V2") });

            var mapper = new Mapper(configMock.Object);

            var input = new[]
            {
                new I07Input
                {
                    Code = new [] { "sdasad" },
                    LineNr = 1,
                    Name = "pav1",
                    Amount = 1,
                    Maker = "BB",
                    DateDateTime = new DateTime(2020,3,1),
                    Details1 = "",
                    Details2 = "",
                    Details3 = "",
                    _pap2 = "1",
                    DimDateDateTime = default
                }
            };

            var operationCodes = new Dictionary<string, string>
            {
                ["V1"] = "125458",
                ["V2"] = "127475458",
            };

            //Act & Assert
            Assert.Throws<ArgumentException>(() => mapper.MapToOutput(input, operationCodes));
        }

        [Theory]
        [InlineData("T1", 1)]
        [InlineData("T1T2", 2)]
        [InlineData("K1T1", 2)]
        [InlineData("K1T1T2", 3)]
        public void When_InputIsValidAndOperationCodesMap_Result_IsMappedOutput(string maker, int excpectedMappedCount)
        {
            //Arrange
            var configMock = new Mock<IConfig>();
            configMock.Setup(c => c.Makers).Returns(new[] { new Maker("T1", "V1"), new Maker("T2", "V2"), new Maker("K1", "V3") });

            var mapper = new Mapper(configMock.Object);

            var input = new[]
            {
                new I07Input
                {
                    Code = new [] { "sdasad" },
                    LineNr = 1,
                    Name = "pav1",
                    Amount = 1,
                    Maker = maker,
                    DateDateTime = new DateTime(2020,3,1),
                    Details1 = "",
                    Details2 = "",
                    Details3 = "",
                    _pap2 = "1",
                    DimDateDateTime = default
                }
            };

            var operationCodes = new Dictionary<string, string>
            {
                ["V1"] = "125458",
                ["V2"] = "127475458",
                ["V3"] = "7439547",
            };

            //Act 
            var output = mapper.MapToOutput(input, operationCodes).ToArray();

            //Assert
            Assert.True(output.Length == excpectedMappedCount);
        }

        [Theory]
        [InlineData("T1")]
        [InlineData("T1T2")]
        [InlineData("T1T2K1")]
        public void When_InputIsValidAndOperationCodesMap_Result_DataIsEqual(string maker)
        {
            //Arrange
            var configMock = new Mock<IConfig>();
            configMock.Setup(c => c.Makers).Returns(new[] { new Maker("T1", "V1"), new Maker("T2", "V2"), new Maker("K1", "V3") });

            var mapper = new Mapper(configMock.Object);

            var input = new[]
            {
                new I07Input
                {
                    Code = new [] { "sdasad" },
                    LineNr = 1,
                    Name = "pav1",
                    Amount = 1,
                    Maker = maker,
                    DateDateTime = new DateTime(2020,3,1),
                    Details1 = "a",
                    Details2 = "b",
                    Details3 = "c",
                    _pap2 = "1",
                    DimDateDateTime = new DateTime(2020,3,2)
                }
            };

            var operationCodes = new Dictionary<string, string>
            {
                ["V1"] = "125458",
                ["V2"] = "127475458",
                ["V3"] = "7439547",
            };

            //Act 
            var output = mapper.MapToOutput(input, operationCodes);

            //Assert
            Assert.True(output.All(o =>
            {
                return o.Code == "sdasad"
                    && o.Amount == 1
                    && o.DateString == "2020.03.01"
                    && o.Maker == maker
                    && o.Details1 == "a"
                    && o.Details2 == "b"
                    && o.Details3 == "c"
                    && o.Pap2 == 1
                    && o.DimDate == "2020.03.02";
            }));
        }
    }
}
