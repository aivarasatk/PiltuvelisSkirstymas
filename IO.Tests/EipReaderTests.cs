using Moq;
using Serilog;
using System;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IO.Tests
{
    public class EipReaderTests
    {
        [Fact]
        public async Task When_FulyQualifiedEipIsUsed_Result_OutputsValidI07Array()
        {
            //Arrange
            IEipReader eipEipReader = ArrangedEipReaderWithDifferentFileContents(FullyQualifiedEip);

            //Act
            var products = await eipEipReader.GetParsedEipContentsAsync("dummy.txt");

            //Assert
            Assert.NotEmpty(products);
            Assert.True(products.All(p => !string.IsNullOrWhiteSpace(p.Code.First())));
        }

        [Fact]
        public async Task When_TrimmedEipIsUsed_Result_OutputsValidI07Array()
        {
            //Arrange
            IEipReader eipEipReader = ArrangedEipReaderWithDifferentFileContents(TrimmedQualifiedEip);

            //Act
            var products = await eipEipReader.GetParsedEipContentsAsync("dummy.txt");

            //Assert
            Assert.NotEmpty(products);
            Assert.True(products.All(p => !string.IsNullOrWhiteSpace(p.Code.First())));
        }

        [Fact]
        public async Task When_EipIsMissingRequiredFields_Result_ThrowsInvalidOperation()
        {
            //Arrange
            IEipReader eipEipReader = ArrangedEipReaderWithDifferentFileContents(EipWithoutRequiredFields);

            //Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>  eipEipReader.GetParsedEipContentsAsync("dummy.txt"));
        }

        [Fact]
        public async Task When_ValidEipIsProvided_Result_IsSerializedCorrectly()
        {
            //Arrange
            IEipReader eipEipReader = ArrangedEipReaderWithDifferentFileContents(EipWithTestData);

            //Act
            var products = await eipEipReader.GetParsedEipContentsAsync("dummy.txt");

            //Assert
            Assert.True(products.Count() == 2);

            var first = products.First();
            var second = products.Last();

            Assert.True(
                first.Code.First() == "kodas1"
                && first.LineNr == 1
                && first.Name == "pav1"
                && first.Maker == "K3"
                && first.Amount == 1
                && first.DateDateTime == new DateTime(2020,3,1)
                && first.Details1 == ""
                && first.Details2 == ""
                && first.Details3 == ""
                && first.Pap2 == 1
                && first.DimDateDateTime == default);

            Assert.True(
                second.Code.First() == "kodas2"
                && second.LineNr == 2
                && second.Name == "pav2"
                && second.Maker == "K1"
                && second.Amount == 2
                && second.DateDateTime == new DateTime(2020, 3, 2)
                && second.Details1 == "1"
                && second.Details2 == "2"
                && second.Details3 == "3"
                && second.Pap2 == 2
                && second.DimDate == "2020.10.10"
                &&second.DimDateDateTime == new DateTime(2020,10,10));
        }

        private IEipReader ArrangedEipReaderWithDifferentFileContents(string eipFileContents)
        {
            var fileSystemMoq = new Mock<IFileSystem>();
            fileSystemMoq.Setup(f => f.File.ReadAllLinesAsync(It.IsAny<string>(), It.IsAny<Encoding>(), default))
                .Returns(() => Task.FromResult(EipStringSplit(eipFileContents)));

            var logger = Mock.Of<ILogger>();
            return new EipReader(fileSystemMoq.Object, logger);
        }


        private string[] EipStringSplit(string eipString) => eipString.Split(Environment.NewLine);

        private const string EipWithTestData=
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
                    <DIM_01>2020.10.10</DIM_01>
                </I07>
            </I06>";

        private const string EipWithoutRequiredFields =
            @"000001     <I06>
            000060          <I07>
            000105               <I07_PERKELTA>1</I07_PERKELTA>
            000126               <I07_APRASYMAS1>Pagaminta 2018-08mŠn. - 14924but. Atideta neaðkiam terminui.                                                                                          </I07_APRASYMAS1>
            000127               <I07_APRASYMAS2>Pagaminta 2018-08mën. - 14924but. Atideta neaðkiam terminui.                                                                                          </I07_APRASYMAS2>
            000128               <I07_APRASYMAS3>(17890)  (1234590)                                                                                                                            </I07_APRASYMAS3>
            000131               <T_KIEKIS>       35000                  </T_KIEKIS>
            000132               <PAP_1>01054</PAP_1>
            000133               <PAP_2>6</PAP_2>
            000134               <DIM_01></DIM_01>
            000135          </I07>
            08648          <I07>
            008720               <DI07_BAR_KODAS>            </DI07_BAR_KODAS>
            008650               <I07_EIL_NR>  3331</I07_EIL_NR>
            008651               <I07_TIPAS>1</I07_TIPAS>
            008652               <I07_KODAS>024FI  </I07_KODAS>
            008653               <I07_KODAS>0240FI  </I07_KODAS>
            008654               <I07_KODAS>02430FI  </I07_KODAS>
            008655               <I07_KODAS>02030FI  </I07_KODAS>
            008656               <I07_KODAS>0230FI  </I07_KODAS>
            008657               <I07_PAV>Trigen tabl. N30          </I07_PAV>
            008723          </I07>
            014552</I06>";

        private const string TrimmedQualifiedEip =
            @"000001     <I06>
            000046          <I06_WEB_ATAS>             </I06_WEB_ATAS>
            000047          <I06_WEB_PERKELTA>1</I06_WEB_PERKELTA>
            000048          <I06_WEB_PERKELTA_I>1</I06_WEB_PERKELTA_I>
            000049          <I06_KODAS_ZN></I06_KODAS_ZN>
            000050          <I06_BUSENA>  1</I06_BUSENA>
            000051          <I06_APRASYMAS1></I06_APRASYMAS1>
            000052          <I06_APRASYMAS2></I06_APRASYMAS2>
            000053          <I06_APRASYMAS3></I06_APRASYMAS3>
            000054          <I06_ISAF></I06_ISAF>
            000055          <I06_PVM_SKOL>1</I06_PVM_SKOL>
            000056          <WEB_GLN>                     </WEB_GLN>
            000057          <WEB_GLN_KS>                     </WEB_GLN_KS>
            000058          <PAP_1></PAP_1>
            000059          <PAP_2></PAP_2>
            000060          <I07>
            000062               <I07_EIL_NR>   843</I07_EIL_NR>
            000063               <I07_TIPAS>1</I07_TIPAS>
            000064               <I07_KODAS>02510017SE  </I07_KODAS>
            000069               <I07_PAV>A ml SE              </I07_PAV>
            000070               <I07_KODAS_TR>            </I07_KODAS_TR>
            000071               <I07_KODAS_IS>K3          </I07_KODAS_IS>
            000072               <I07_KODAS_OS>SE </I07_KODAS_OS>
            000073               <I07_KODAS_OS_C>            </I07_KODAS_OS_C>
            000074               <I07_SERIJA>            </I07_SERIJA>
            000075               <I07_KODAS_US>BUT         </I07_KODAS_US>
            000076               <I07_KIEKIS>         35000</I07_KIEKIS>
            000077               <I07_FRAKCIJA>   1</I07_FRAKCIJA>
            000104               <I07_GALIOJA_IKI>2020.03.01 00:00</I07_GALIOJA_IKI>
            000105               <I07_PERKELTA>1</I07_PERKELTA>
            000126               <I07_APRASYMAS1>ðkiam terminui.                                                                                          </I07_APRASYMAS1>
            000127               <I07_APRASYMAS2>Pagaminta 2018 neaðkiam terminui.                                                                                          </I07_APRASYMAS2>
            000128               <I07_APRASYMAS3>(123)  (12345                                                                                                                           </I07_APRASYMAS3>
            000131               <T_KIEKIS>       35000                  </T_KIEKIS>
            000132               <PAP_1>014</PAP_1>
            000133               <PAP_2>6</PAP_2>
            000134               <DIM_01></DIM_01>
            000135          </I07>
            08648          <I07>
            008649               <DI07_BAR_KODAS>            </DI07_BAR_KODAS>
            008650               <I07_EIL_NR>  3331</I07_EIL_NR>
            008651               <I07_TIPAS>1</I07_TIPAS>
            008652               <I07_KODAS>0244  </I07_KODAS>
            008653               <I07_KODAS>0244I  </I07_KODAS>
            008654               <I07_KODAS>0244FI  </I07_KODAS>
            008655               <I07_KODAS>0244FI  </I07_KODAS>
            008656               <I07_KODAS>0244FI  </I07_KODAS>
            008657               <I07_PAV>Tn tabl. N30          </I07_PAV>
            008658               <I07_KODAS_TR>            </I07_KODAS_TR>
            008659               <I07_KODAS_IS>T3V2        </I07_KODAS_IS>
            008664               <I07_KIEKIS>          5000</I07_KIEKIS>
            008692               <I07_GALIOJA_IKI>2020.09.11 00:00</I07_GALIOJA_IKI>
            008714               <I07_APRASYMAS1>U5                                                                                                                                         </I07_APRASYMAS1>
            008715               <I07_APRASYMAS2>U5                                                                                                                                           </I07_APRASYMAS2>
            008716               <I07_APRASYMAS3></I07_APRASYMAS3>
            008717               <I07_KODAS_KL></I07_KODAS_KL>
            008718               <I07_PVM_SKOL>1</I07_PVM_SKOL>
            008719               <T_KIEKIS>        5000                  </T_KIEKIS>
            008720               <PAP_1>0100</PAP_1>
            008721               <PAP_2>4</PAP_2>
            008722               <DIM_01></DIM_01>
            008723          </I07>
            014552</I06>";

        private const string FullyQualifiedEip =
            @"000001     <I06>
            000002          <I06_OP_TIP> 4</I06_OP_TIP>
            000003          <I06_VAL_POZ>0</I06_VAL_POZ>
            000004          <I06_PVM_TIP>0</I06_PVM_TIP>
            000005          <I06_OP_STORNO>0</I06_OP_STORNO>
            000006          <I06_DOK_NR>06-09      </I06_DOK_NR>
            000007          <I06_OP_DATA>2020.01.01 00:00</I06_OP_DATA>
            000008          <I06_DOK_DATA>2020.01.01 00:00</I06_DOK_DATA>
            000009          <I06_KODAS_MS>86          </I06_KODAS_MS>
            000010          <I06_KODAS_KS>PLANAS      </I06_KODAS_KS>
            000011          <I06_KODAS_SS>            </I06_KODAS_SS>
            000012          <I06_PAV>Planas                                                                </I06_PAV>
            000013          <I06_ADR>                                        </I06_ADR>
            000014          <I06_ATSTOVAS>                                        </I06_ATSTOVAS>
            000015          <I06_KODAS_VS>            </I06_KODAS_VS>
            000016          <I06_PAV2>                                                                      </I06_PAV2>
            000017          <I06_ADR2>                                        </I06_ADR2>
            000018          <I06_ADR3>                                        </I06_ADR3>
            000019          <I06_KODAS_VL>            </I06_KODAS_VL>
            000020          <I06_KODAS_XS>PVM         </I06_KODAS_XS>
            000021          <I06_KODAS_SS_P>            </I06_KODAS_SS_P>
            000022          <I06_PASTABOS>                                        </I06_PASTABOS>
            000023          <I06_MOK_DOK>            </I06_MOK_DOK>
            000024          <I06_MOK_SUMA>        0.00</I06_MOK_SUMA>
            000025          <I06_KODAS_SS_M>            </I06_KODAS_SS_M>
            000026          <I06_SUMA_VAL>           0.00</I06_SUMA_VAL>
            000027          <I06_SUMA> .00</I06_SUMA>
            000028          <I06_SUMA_PVM> .00</I06_SUMA_PVM>
            000029          <I06_KURSAS>   0.000000000000000</I06_KURSAS>
            000030          <I06_PERKELTA>1</I06_PERKELTA>
            000031          <I06_ADDUSR>    </I06_ADDUSR>
            000032          <I06_R_DATE>2020.07.16 12:07</I06_R_DATE>
            000033          <I06_USERIS>   </I06_USERIS>
            000034          <I06_KODAS_AU>            </I06_KODAS_AU>
            000035          <I06_KODAS_SM>            </I06_KODAS_SM>
            000036          <I06_INTRASTAT>1</I06_INTRASTAT>
            000037          <I06_DOK_REG></I06_DOK_REG>
            000038          <I06_KODAS_AK>            </I06_KODAS_AK>
            000039          <I06_SUMA_WK></I06_SUMA_WK>
            000040          <I06_KODAS_LS_1></I06_KODAS_LS_1>
            000041          <I06_KODAS_LS_2></I06_KODAS_LS_2>
            000042          <I06_KODAS_LS_3></I06_KODAS_LS_3>
            000043          <I06_KODAS_LS_4></I06_KODAS_LS_4>
            000044          <I06_PVM_VAL>              0.00</I06_PVM_VAL>
            000045          <I06_WEB_POZ>0</I06_WEB_POZ>
            000046          <I06_WEB_ATAS>             </I06_WEB_ATAS>
            000047          <I06_WEB_PERKELTA>1</I06_WEB_PERKELTA>
            000048          <I06_WEB_PERKELTA_I>1</I06_WEB_PERKELTA_I>
            000049          <I06_KODAS_ZN></I06_KODAS_ZN>
            000050          <I06_BUSENA>  1</I06_BUSENA>
            000051          <I06_APRASYMAS1></I06_APRASYMAS1>
            000052          <I06_APRASYMAS2></I06_APRASYMAS2>
            000053          <I06_APRASYMAS3></I06_APRASYMAS3>
            000054          <I06_ISAF></I06_ISAF>
            000055          <I06_PVM_SKOL>1</I06_PVM_SKOL>
            000056          <WEB_GLN>                     </WEB_GLN>
            000057          <WEB_GLN_KS>                     </WEB_GLN_KS>
            000058          <PAP_1></PAP_1>
            000059          <PAP_2></PAP_2>
            000060          <I07>
            000061               <DI07_BAR_KODAS>            </DI07_BAR_KODAS>
            000062               <I07_EIL_NR>   843</I07_EIL_NR>
            000063               <I07_TIPAS>1</I07_TIPAS>
            000064               <I07_KODAS>02  </I07_KODAS>
            000065               <I07_KODAS>02  </I07_KODAS>
            000066               <I07_KODAS>02  </I07_KODAS>
            000067               <I07_KODAS>02  </I07_KODAS>
            000068               <I07_KODAS>02  </I07_KODAS>
            000069               <I07_PAV>A 15 ml SE              </I07_PAV>
            000070               <I07_KODAS_TR>            </I07_KODAS_TR>
            000071               <I07_KODAS_IS>K3          </I07_KODAS_IS>
            000072               <I07_KODAS_OS>R02E </I07_KODAS_OS>
            000073               <I07_KODAS_OS_C>            </I07_KODAS_OS_C>
            000074               <I07_SERIJA>            </I07_SERIJA>
            000075               <I07_KODAS_US>BUT         </I07_KODAS_US>
            000076               <I07_KIEKIS>         35000</I07_KIEKIS>
            000077               <I07_FRAKCIJA>   1</I07_FRAKCIJA>
            000078               <I07_KODAS_US_P>BUT         </I07_KODAS_US_P>
            000079               <I07_KODAS_US_A>BUT         </I07_KODAS_US_A>
            000080               <I07_ALT_KIEKIS>         35000</I07_ALT_KIEKIS>
            000081               <I07_ALT_FRAK>   1</I07_ALT_FRAK>
            000082               <I07_VAL_KAINA>            0.0000</I07_VAL_KAINA>
            000083               <I07_SUMA_VAL>              0.00</I07_SUMA_VAL>
            000084               <I07_KAINA_BE>      0.0000</I07_KAINA_BE>
            000085               <I07_KAINA_SU>      0.0000</I07_KAINA_SU>
            000086               <I07_NUOLAIDA>  0.00</I07_NUOLAIDA>
            000087               <I07_ISLAIDU_M>0</I07_ISLAIDU_M>
            000088               <I07_ISLAIDOS>        0.00</I07_ISLAIDOS>
            000089               <I07_ISLAIDOS_PVM>        0.00</I07_ISLAIDOS_PVM>
            000090               <I07_MUITAS_M>0</I07_MUITAS_M>
            000091               <I07_MUITAS>        0.00</I07_MUITAS>
            000092               <I07_MUITAS_PVM>        0.00</I07_MUITAS_PVM>
            000093               <I07_AKCIZAS_M>0</I07_AKCIZAS_M>
            000094               <I07_AKCIZAS>        0.00</I07_AKCIZAS>
            000095               <I07_AKCIZAS_PVM>        0.00</I07_AKCIZAS_PVM>
            000096               <I07_MOKESTIS>0</I07_MOKESTIS>
            000097               <I07_MOKESTIS_P> 21.00</I07_MOKESTIS_P>
            000098               <I07_PVM>        0.00</I07_PVM>
            000099               <I07_SUMA>        0.00</I07_SUMA>
            000100               <I07_PAR_KAINA>      0.0000</I07_PAR_KAINA>
            000101               <I07_PAR_KAINA_N>      0.0000</I07_PAR_KAINA_N>
            000102               <I07_MOK_SUMA>       20.00</I07_MOK_SUMA>
            000103               <I07_SAVIKAINA>        0.00</I07_SAVIKAINA>
            000104               <I07_GALIOJA_IKI>2020.03.01 00:00</I07_GALIOJA_IKI>
            000105               <I07_PERKELTA>1</I07_PERKELTA>
            000106               <I07_ADDUSR>A    </I07_ADDUSR>
            000107               <I07_USERIS>    </I07_USERIS>
            000108               <I07_R_DATE>2020.06.25 08:35</I07_R_DATE>
            000109               <I07_SERTIFIKATAS>            </I07_SERTIFIKATAS>
            000110               <I07_KODAS_KT>            </I07_KODAS_KT>
            000111               <I07_KODAS_K0>            </I07_KODAS_K0>
            000112               <I07_KODAS_KV>            </I07_KODAS_KV>
            000113               <I07_KODAS_VZ>            </I07_KODAS_VZ>
            000114               <I07_ADD_DATE>2018.02.02 10:28</I07_ADD_DATE>
            000115               <I07_APSKRITIS></I07_APSKRITIS>
            000116               <I07_SANDORIS></I07_SANDORIS>
            000117               <I07_SALYGOS></I07_SALYGOS>
            000118               <I07_RUSIS></I07_RUSIS>
            000119               <I07_SALIS></I07_SALIS>
            000120               <I07_MATAS></I07_MATAS>
            000121               <I07_SALIS_K></I07_SALIS_K>
            000122               <I07_MASE>         0.000</I07_MASE>
            000123               <I07_INT_KIEKIS>         0.000</I07_INT_KIEKIS>
            000124               <I07_PVM_VAL>              0.00</I07_PVM_VAL>
            000125               <I07_KODAS_KS>            </I07_KODAS_KS>
            000126               <I07_APRASYMAS1>Paðkiam terminui.                                                                                          </I07_APRASYMAS1>
            000127               <I07_APRASYMAS2>Pagaminkiam terminui.                                                                                          </I07_APRASYMAS2>
            000128               <I07_APRASYMAS3>(123                                                                                                                            </I07_APRASYMAS3>
            000129               <I07_KODAS_KL></I07_KODAS_KL>
            000130               <I07_PVM_SKOL>1</I07_PVM_SKOL>
            000131               <T_KIEKIS>       35000                  </T_KIEKIS>
            000132               <PAP_1>010000</PAP_1>
            000133               <PAP_2>6</PAP_2>
            000134               <DIM_01></DIM_01>
            000135          </I07>
            08648          <I07>
            008649               <DI07_BAR_KODAS>            </DI07_BAR_KODAS>
            008650               <I07_EIL_NR>  3331</I07_EIL_NR>
            008651               <I07_TIPAS>1</I07_TIPAS>
            008652               <I07_KODAS>024  </I07_KODAS>
            008653               <I07_KODAS>024  </I07_KODAS>
            008654               <I07_KODAS>024  </I07_KODAS>
            008655               <I07_KODAS>024  </I07_KODAS>
            008656               <I07_KODAS>024  </I07_KODAS>
            008657               <I07_PAV>T. N30          </I07_PAV>
            008658               <I07_KODAS_TR>            </I07_KODAS_TR>
            008659               <I07_KODAS_IS>T3V2        </I07_KODAS_IS>
            008660               <I07_KODAS_OS>            </I07_KODAS_OS>
            008661               <I07_KODAS_OS_C>            </I07_KODAS_OS_C>
            008662               <I07_SERIJA>            </I07_SERIJA>
            008663               <I07_KODAS_US>DËÞ         </I07_KODAS_US>
            008664               <I07_KIEKIS>          5000</I07_KIEKIS>
            008665               <I07_FRAKCIJA>   1</I07_FRAKCIJA>
            008666               <I07_KODAS_US_P>DËÞ         </I07_KODAS_US_P>
            008667               <I07_KODAS_US_A>DËÞ         </I07_KODAS_US_A>
            008668               <I07_ALT_KIEKIS>          5000</I07_ALT_KIEKIS>
            008669               <I07_ALT_FRAK>   1</I07_ALT_FRAK>
            008670               <I07_VAL_KAINA>            0.0000</I07_VAL_KAINA>
            008671               <I07_SUMA_VAL>              0.00</I07_SUMA_VAL>
            008672               <I07_KAINA_BE>      0.0000</I07_KAINA_BE>
            008673               <I07_KAINA_SU>      0.0000</I07_KAINA_SU>
            008674               <I07_NUOLAIDA>  0.00</I07_NUOLAIDA>
            008675               <I07_ISLAIDU_M>1</I07_ISLAIDU_M>
            008676               <I07_ISLAIDOS>        0.00</I07_ISLAIDOS>
            008677               <I07_ISLAIDOS_PVM>        0.00</I07_ISLAIDOS_PVM>
            008678               <I07_MUITAS_M>1</I07_MUITAS_M>
            008679               <I07_MUITAS>        0.00</I07_MUITAS>
            008680               <I07_MUITAS_PVM>        0.00</I07_MUITAS_PVM>
            008681               <I07_AKCIZAS_M>1</I07_AKCIZAS_M>
            008682               <I07_AKCIZAS>        0.00</I07_AKCIZAS>
            008683               <I07_AKCIZAS_PVM>        0.00</I07_AKCIZAS_PVM>
            008684               <I07_MOKESTIS>1</I07_MOKESTIS>
            008685               <I07_MOKESTIS_P> 21.00</I07_MOKESTIS_P>
            008686               <I07_PVM>        0.00</I07_PVM>
            008687               <I07_SUMA>        0.00</I07_SUMA>
            008688               <I07_PAR_KAINA>      0.0000</I07_PAR_KAINA>
            008689               <I07_PAR_KAINA_N>      0.0000</I07_PAR_KAINA_N>
            008690               <I07_MOK_SUMA>       18.00</I07_MOK_SUMA>
            008691               <I07_SAVIKAINA>        0.00</I07_SAVIKAINA>
            008692               <I07_GALIOJA_IKI>2020.09.11 00:00</I07_GALIOJA_IKI>
            008693               <I07_PERKELTA>1</I07_PERKELTA>
            008694               <I07_ADDUSR>    </I07_ADDUSR>
            008695               <I07_USERIS>    </I07_USERIS>
            008696               <I07_R_DATE>2020.05.22 15:32</I07_R_DATE>
            008697               <I07_SERTIFIKATAS>            </I07_SERTIFIKATAS>
            008698               <I07_KODAS_KT>            </I07_KODAS_KT>
            008699               <I07_KODAS_K0>            </I07_KODAS_K0>
            008700               <I07_KODAS_KV>            </I07_KODAS_KV>
            008701               <I07_KODAS_VZ>            </I07_KODAS_VZ>
            008702               <I07_ADD_DATE>2020.05.22 15:32</I07_ADD_DATE>
            008703               <I07_APSKRITIS></I07_APSKRITIS>
            008704               <I07_SANDORIS>11 </I07_SANDORIS>
            008705               <I07_SALYGOS></I07_SALYGOS>
            008706               <I07_RUSIS></I07_RUSIS>
            008707               <I07_SALIS></I07_SALIS>
            008708               <I07_MATAS>NAR</I07_MATAS>
            008709               <I07_SALIS_K></I07_SALIS_K>
            008710               <I07_MASE>    180000.000</I07_MASE>
            008711               <I07_INT_KIEKIS>       180.000</I07_INT_KIEKIS>
            008712               <I07_PVM_VAL>              0.00</I07_PVM_VAL>
            008713               <I07_KODAS_KS>            </I07_KODAS_KS>
            008714               <I07_APRASYMAS1>U584_TK025                                                                                                                                            </I07_APRASYMAS1>
            008715               <I07_APRASYMAS2>U584_TK025                                                                                                                                            </I07_APRASYMAS2>
            008716               <I07_APRASYMAS3></I07_APRASYMAS3>
            008717               <I07_KODAS_KL></I07_KODAS_KL>
            008718               <I07_PVM_SKOL>1</I07_PVM_SKOL>
            008719               <T_KIEKIS>        5000                  </T_KIEKIS>
            008720               <PAP_1>010</PAP_1>
            008721               <PAP_2>4</PAP_2>
            008722               <DIM_01></DIM_01>
            008723          </I07>
            014552</I06>


            ";
    }
}
