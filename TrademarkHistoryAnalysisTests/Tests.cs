using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TrademarkHistoryAnalysis.DAO;
using TrademarkHistoryAnalysis.Models;
using TrademarkHistoryAnalysis.Services;

namespace TrademarkHistoryAnalysisTests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void ParseZippedXML()
        {
            List<CaseFile> caseFiles = Parser.ParseZippedXML("apc18840407-20181231-01.zip");
            Assert.AreEqual(caseFiles.Count, 146943);
        }
    }
}
