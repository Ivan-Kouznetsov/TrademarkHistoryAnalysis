using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TrademarkHistoryAnalysis.DAO;
using TrademarkHistoryAnalysis.Models;
using TrademarkHistoryAnalysis.Services;
using System.IO;
using System;

namespace TrademarkHistoryAnalysisTests
{
    [TestClass]
    public class Tests
    {
        readonly string[] ZippedXMLFiles = new string[] { "FilesToParse" + Path.DirectorySeparatorChar + "apc18840407-20181231-01.zip",
                                                          "FilesToParse" + Path.DirectorySeparatorChar + "apc18840407-20181231-01Copy1.zip",
                                                          "FilesToParse" + Path.DirectorySeparatorChar + "apc18840407-20181231-01Copy2.zip",
                                                          "FilesToParse" + Path.DirectorySeparatorChar + "apc18840407-20181231-01Copy3.zip",
                                                          "FilesToParse" + Path.DirectorySeparatorChar + "apc18840407-20181231-01Copy4.zip"};
        
        const int ZippedXMLFileCases = 146943;        

        [TestMethod]
        public void ParseZippedXML()
        {
            List<CaseFile> caseFiles = Parser.ParseZippedXML(ZippedXMLFiles[0]);
            Assert.AreEqual(ZippedXMLFileCases, caseFiles.Count);
        }

        [TestMethod]
        public void LowMemoryParseZippedXML()
        {
            List<CaseFile> caseFiles = Parser.LowMemoryParseZippedXML(ZippedXMLFiles[0]);
            Assert.AreEqual(ZippedXMLFileCases, caseFiles.Count);
        }

        [TestMethod]
        public void ParseZippedXMLInParallel()
        {
            int fileCopyCount = ZippedXMLFiles.Length;   
            List<CaseFile> caseFiles = Parser.ParseInParallel(ZippedXMLFiles);
           
            Assert.AreEqual(ZippedXMLFileCases * fileCopyCount, caseFiles.Count);
        }

        [TestMethod]
        public void ParseInParallelWriteOnTheFly()
        {
            int fileCopyCount = ZippedXMLFiles.Length;
            const string TestDbName = "test.db";

            CaseFilesDAO caseFilesDAO = new CaseFilesDAO(TestDbName);
            Parser.ParseInParallelWriteOnTheFly(ZippedXMLFiles, new Parser.CaseFileWriter(caseFilesDAO.SaveCaseFileList));

            List<CaseFile> caseFiles = caseFilesDAO.GetAllCaseFiles();
            //File.Delete(TestDbName);

            Assert.AreEqual(ZippedXMLFileCases * fileCopyCount, caseFiles.Count);
        }
    }
}
