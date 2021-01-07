using IO;
using IO.Dto;
using Moq;
using Serilog;
using System;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PiltuvelisSkirstymas.Tests
{
    public class ParsingTests
    {
        [Fact]
        public async Task BothDimFormatsAreParsedCorrectly() 
        {
            //Arrange
            var fileSystemMoq = new Mock<IFileSystem>();
            fileSystemMoq.Setup(f => f.File.ReadAllLinesAsync(It.IsAny<string>(), It.IsAny<Encoding>(), default))
                .Returns(() => Task.FromResult(EipWithTestData.Split(Environment.NewLine)));

            var logger = Mock.Of<ILogger>();
            var eipReader = new EipReader(fileSystemMoq.Object, logger);

            //Act
            var products = (await eipReader.GetParsedEipContentsAsync("dummy.txt")).ToArray();

            //Assert
            Assert.Equal(products[0].DimDateDateTime, products[1].DimDateDateTime);
            Assert.Equal(new DateTime(2021, 2, 22), products[0].DimDateDateTime);
            Assert.Equal(new DateTime(2021, 2, 22), products[1].DimDateDateTime);
        }

        [Fact]
        public async Task WhenXmlSerializing_OpenCloseTagsAreCreatedForEmptyFields()
        {
            //Arrange
            var fileSystemMoq = new Mock<IFileSystem>();
            fileSystemMoq.Setup(f => f.File.ReadAllLinesAsync(It.IsAny<string>(), It.IsAny<Encoding>(), default))
                .Returns(() => Task.FromResult(EipTestDataWithEmptyFields.Split(Environment.NewLine)));

            var logger = Mock.Of<ILogger>();
            var eipReader = new EipReader(fileSystemMoq.Object, logger);
            var products = (await eipReader.GetParsedEipContentsAsync("dummy.txt")).ToArray();

            var resultData = new I06Output(products.Select(p => new I07Output(p)).ToArray());

            //Act
            var xmlString = resultData.XmlSerialize();

            //Assert
            Assert.Contains("<DIM_01> </DIM_01>", xmlString);
            Assert.Contains("<I07_APRASYMAS1> </I07_APRASYMAS1>", xmlString);
            Assert.Contains("<I07_APRASYMAS2> </I07_APRASYMAS2>", xmlString);
            Assert.Contains("<I07_APRASYMAS3> </I07_APRASYMAS3>", xmlString);
        }

        private const string EipWithTestData =
            @"<I06>
                <I07>
                    <I07_EIL_NR>   1</I07_EIL_NR>
                    <I07_KODAS>kodas1  </I07_KODAS>
                    <I07_PAV>pav1</I07_PAV>
                    <I07_KODAS_IS>K3          </I07_KODAS_IS>
                    <I07_KIEKIS>         1</I07_KIEKIS>
                    <I07_GALIOJA_IKI>2020.03.01 00:00</I07_GALIOJA_IKI>
                    <I07_APRASYMAS1></I07_APRASYMAS1>
                    <I07_APRASYMAS2></I07_APRASYMAS2>
                    <I07_APRASYMAS3></I07_APRASYMAS3>
                    <PAP_2>1</PAP_2>
                    <DIM_01>2021-02-22</DIM_01>
                </I07>
                <I07>
                    <I07_EIL_NR>   2</I07_EIL_NR>
                    <I07_KODAS>kodas2</I07_KODAS>
                    <I07_PAV>pav2</I07_PAV>
                    <I07_KODAS_IS>K1          </I07_KODAS_IS>
                    <I07_KIEKIS>    2  </I07_KIEKIS>
                    <I07_GALIOJA_IKI>2020.03.02 00:00</I07_GALIOJA_IKI>
                    <I07_APRASYMAS1>1</I07_APRASYMAS1>
                    <I07_APRASYMAS2>2</I07_APRASYMAS2>
                    <I07_APRASYMAS3>3</I07_APRASYMAS3>
                    <PAP_2>2</PAP_2>
                    <DIM_01>2021.02.22</DIM_01>
                </I07>
            </I06>";

        private const string EipTestDataWithEmptyFields =
            @"<I06>
                <I07>
                    <I07_EIL_NR>   1</I07_EIL_NR>
                    <I07_KODAS>kodas1  </I07_KODAS>
                    <I07_PAV>pav1</I07_PAV>
                    <I07_KODAS_IS>K3          </I07_KODAS_IS>
                    <I07_KIEKIS>         1</I07_KIEKIS>
                    <I07_GALIOJA_IKI>2020.03.01 00:00</I07_GALIOJA_IKI>
                    <I07_APRASYMAS1></I07_APRASYMAS1>
                    <I07_APRASYMAS2></I07_APRASYMAS2>
                    <I07_APRASYMAS3></I07_APRASYMAS3>
                    <PAP_2>1</PAP_2>
                    <DIM_01></DIM_01>
                </I07>
                <I07>
                    <I07_EIL_NR>   2</I07_EIL_NR>
                    <I07_KODAS>kodas2</I07_KODAS>
                    <I07_PAV>pav2</I07_PAV>
                    <I07_KODAS_IS>K1          </I07_KODAS_IS>
                    <I07_KIEKIS>    2  </I07_KIEKIS>
                    <I07_GALIOJA_IKI>2020.03.02 00:00</I07_GALIOJA_IKI>
                    <I07_APRASYMAS1>1</I07_APRASYMAS1>
                    <I07_APRASYMAS2>2</I07_APRASYMAS2>
                    <I07_APRASYMAS3>3</I07_APRASYMAS3>
                    <PAP_2>2</PAP_2>
                    <DIM_01></DIM_01>
                </I07>
            </I06>";
    }
}
