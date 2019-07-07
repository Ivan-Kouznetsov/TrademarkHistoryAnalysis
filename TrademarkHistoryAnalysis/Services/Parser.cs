using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using TrademarkHistoryAnalysis.Models;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace TrademarkHistoryAnalysis.Services
{
    public static class Parser
    {
        const string TempDirectory = "TradeMarkHistoryAnalysisXml";
        private static string TempDirectoryFullPath { get; set; }

        static Parser() {
            TempDirectoryFullPath = Path.GetTempPath() + Path.DirectorySeparatorChar + TempDirectory;
            IEnumerable<string> existingTempFiles = Directory.EnumerateFiles(TempDirectoryFullPath);

            foreach (string f in existingTempFiles)
            {
                File.Delete(f);
            }
        }

        /// <summary>
        /// Returns true if the XML element e is a USPTO case file, in which the attribute "supplemental-register-in" is equal to F. F is short for false.
        /// </summary>
        /// <param name="element">XElement representing a case file</param>
        /// <returns>A bool indicating whether the element has a the attribute "supplemental-register-in" is equal to F</returns>
        private static bool IsOnPrincipalRegister(XElement element)
        {
            string supplemental = (string)element.Element("supplemental-register-in");
            return supplemental == "F"; // F is code for false
        }

        /// <summary>
        /// Attempts to parse a date from an XML element's child element 
        /// </summary>
        /// <param name="element">XElement representing a case file</param>
        /// <param name="datename">name of child element containing a date string</param>
        /// <returns>A Nullable DateTime</returns>
        private static DateTime? GetCaseFileDate(XElement element, string datename)
        {
            const string xmlDateFormat = "yyyyMMdd";
            if (element.Element("case-file-header").Element(datename) != null)
            {
                return DateTime.ParseExact((string)element.Element("case-file-header").Element(datename), xmlDateFormat, CultureInfo.InvariantCulture);
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Compiles a list of classifications which includes international codes and identifications of goods and services
        /// </summary>
        /// <param name="classifications">XML elements the child elemnts of which are classification nodes </param>
        /// <param name="GoodsAndServices">a list of strings that match the classification nodes</param>
        /// <returns>A list of Classifications</returns>
        private static List<Classification> GetClasses(IEnumerable<XElement> classifications, List<string> GoodsAndServices)
        {
            List<Classification> result = new List<Classification>();
            int index = 0;

            foreach (XElement c in classifications)
            {
                if (c.Element("international-code") != null)
                {
                    if (int.TryParse((string)c.Element("international-code"), out int code))
                    {
                        result.Add(new Classification(code, GoodsAndServices.ElementAtOrDefault(index)));
                    }
                }
                else
                {
                    result.Add(new Classification(0, GoodsAndServices.ElementAtOrDefault(index)));
                }
                index++;
            }

            return result;
        }

        /// <summary>
        /// Parses goods and services text elements and concatanates them together into a semicolon separated string
        /// </summary>
        /// <param name="caseFileStatements">an IEnumerable XElement containing "case-file-statements" child elements</param>
        /// <returns></returns>
        private static List<string> GetGoodsAndServices(IEnumerable<XElement> caseFileStatements)
        {
            List<string> texts = new List<string>();

            foreach (XElement s in caseFileStatements)
            {
                if (((string)s.Element("type-code")).StartsWith("GS"))
                {
                    texts.Add((string)s.Element("text")); //GS is code for Goods and Services
                }
            }

            return texts;
        }
        /// <summary>
        /// Parses an individual XElement containing a case file
        /// </summary>
        /// <param name="e">an XElement from a USPTO annual archive XML file</param>
        /// <returns>A CaseFile</returns>
        private static CaseFile ParseElement(XElement e) {
            XElement header = e.Element("case-file-header");
            XElement owner = e.Element("case-file-owners").Element("case-file-owner");

            string state = null;
            string country = "US";

            if (owner.Element("nationality") != null)
            {
                if (owner.Element("nationality").Element("state") != null)
                {
                    state = (string)owner.Element("nationality").Element("state");
                }
                else
                {
                    country = (string)owner.Element("nationality").Element("country");
                }
            }

            CaseFile result = new CaseFile((DateTime)GetCaseFileDate(e, "filing-date"),
                                (int)e.Element("serial-number"),
                                GetCaseFileDate(e, "registration-date"),
                                (int?)e.Element("registration-number"),
                                (string)owner.Element("party-name"),
                                (int)owner.Element("legal-entity-type-code"),
                                state,
                                country,
                                (string)header.Element("attorney-name"),
                                (int)header.Element("status-code"),
                                (string)header.Element("mark-identification"),
                                     GetClasses(e.Elements("classifications").Elements("classification"),
                                                GetGoodsAndServices(e.Elements("case-file-statements").Elements("case-file-statement")))
                                );
            return result;
        }

        /// <summary>
        /// Parses USPTO annual data and outputs CaseFile objects to a stream
        /// </summary>
        /// <param name="xmlData">a stream from which xml data is read</param>
        /// <param name="output">a stream to which results are written</param>
        public static void ParseXML(Stream xmlData, Stream output)
        {   
            XElement xElement = XElement.Load(xmlData);           
            IEnumerable<XElement> searchResults = from case_file in xElement.Descendants("case-file")
                                                  where IsOnPrincipalRegister(case_file.Element("case-file-header")) &&
                                                       (case_file.Element("case-file-header") != null) &&
                                                       (case_file.Element("case-file-owners") != null) &&
                                                       (case_file.Element("serial-number") != null) &&
                                                       (case_file.Elements("case-file-statements") != null) &&
                                                       (case_file.Elements("classifications") != null)
                                                  select case_file;

            IFormatter formatter = new BinaryFormatter();
            foreach (XElement e in searchResults)
            {               
                // save to outout stream                
                formatter.Serialize(output, ParseElement(e) );
            }
        }
        
        private static IEnumerable<XElement> LazyGetXElements(string filename)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            settings.DtdProcessing = DtdProcessing.Parse;
            using (XmlReader reader = XmlReader.Create(filename, settings))
            {
                reader.MoveToContent();

                while (reader.Read())
                {
                    while (reader.NodeType == XmlNodeType.Element
                           && reader.Name.Equals("case-file", StringComparison.InvariantCulture))
                    {
                        yield return XNode.ReadFrom(reader) as XElement;
                    }
                }

            }
        }
        /// <summary>
        /// Parses an XML USPTO annual archive file without loading the whole file into memory
        /// </summary>
        /// <param name="filename">path of the file</param>
        /// <returns>List of CaseFiles</returns>
        public static List<CaseFile> LowMemoryParse(string filename)
        {
            List<CaseFile> caseFiles = new List<CaseFile>();

            foreach (XElement e in LazyGetXElements(filename))
            {
                if (IsOnPrincipalRegister(e.Element("case-file-header")) &&
                            (e.Element("case-file-header") != null) &&
                            (e.Element("case-file-owners") != null) &&
                            (e.Element("serial-number") != null) &&
                            (e.Elements("case-file-statements") != null) &&
                            (e.Elements("classifications") != null))
                {
                    caseFiles.Add(ParseElement(e));
                }
            }

            return caseFiles;
        }
        /// <summary>
        /// Unzips and parses an XML USPTO annual archive file without loading the whole file into memory
        /// </summary>
        /// <param name="filename">path of the file</param>
        /// <returns>List of CaseFiles</returns>
        public static List<CaseFile> LowMemoryParseZippedXML(string filename) {
            ZipFile.ExtractToDirectory(filename, TempDirectoryFullPath);
            string newFilepath = TempDirectoryFullPath + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(filename) + ".xml";        
            List<CaseFile> caseFiles = LowMemoryParse(newFilepath);
            File.Delete(newFilepath);

            return caseFiles;   
        }

        /// <summary>
        /// Parses a zipped USPTO annual archive file
        /// </summary>
        /// <param name="filename">name of zip file</param>
        /// <param name="output">a stream to which the output will be written</param>
        public static void ParseZippedXML(string filename, Stream output)
        {
            
            using (Stream zipFileStream = File.Open(filename, FileMode.Open))
            {
                ZipArchive zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Read);
                using (Stream xmlStream = zipArchive.Entries[0].Open())
                {
                    ParseXML(xmlStream, output);
                }                    
            }            
        }

        /// <summary>
        /// Parses a USPTO annual case file collection
        /// </summary>
        /// <param name="filename">filename of a zip file containing a single XML file containing a USPTO annual case file collection</param>
        /// <returns>A list of CaseFile objects</returns>
        public static List<CaseFile> ParseZippedXML(string filename)
        {
            List<CaseFile> caseFiles = new List<CaseFile>();

            using (MemoryStream memoryStream = new MemoryStream()) {
                ParseZippedXML(filename, memoryStream);

                IFormatter formatter = new BinaryFormatter();

                memoryStream.Position = 0;
                while (memoryStream.Position < memoryStream.Length) {
                    caseFiles.Add((CaseFile)formatter.Deserialize(memoryStream));
                }

                return caseFiles;
            }
        }
        /// <summary>
        /// Parses USPTO annual data in zipped XML files using parallel processing. The method uses temporary files. 
        /// </summary>
        /// <param name="filenames">a list of USPTO annual data in zipped XML files</param>
        /// <returns>A list of CaseFile objects</returns>
        public static List<CaseFile> ParseInParallel(IEnumerable<string> filenames)
        {
            ConcurrentBag<string> concurrentFilenames = new ConcurrentBag<string>(filenames);
            ConcurrentBag<CaseFile> concurrentCaseFiles = new ConcurrentBag<CaseFile>();
           

            Parallel.ForEach(concurrentFilenames,
                new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                filename =>
                {
                    concurrentCaseFiles.AddRange(LowMemoryParseZippedXML(filename));
                }
                );
      
            return new List<CaseFile>(concurrentCaseFiles);
        }

        public delegate void CaseFileWriter(List<CaseFile> caseFiles);
        /// <summary>
        /// Takes a list of zipped USPTO annual archive file locations and parses them in parallel
        /// while sending batches of completed results to a CaseFileWriter
        /// </summary>
        /// <param name="filenames">zipped XML file locations</param>
        /// <param name="writer">a deligate pointing to a method that takes a List of CaseFile objects and saves them somewhere</param>
        /// <param name="writeSynchronously">if true processes will wait and call the CaseFileWriter one at a time, if false the CaseFileWriter will be called as soon as a batch of results is ready</param>
        public static void ParseInParallelWriteOnTheFly(IEnumerable<string> filenames,
                                                        CaseFileWriter writer,
                                                        bool writeSynchronously = true
                                                        )
        {
            ConcurrentBag<string> concurrentFilenames = new ConcurrentBag<string>(filenames);

            object writeLock = new object();

            Parallel.ForEach(concurrentFilenames,
                new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                filename =>
                {
                    List<CaseFile> caseFiles = LowMemoryParseZippedXML(filename);

                    if (writeSynchronously)
                    {
                        lock (writeLock)
                        {
                            writer(caseFiles);
                        }
                    }
                    else
                    {
                        writer(caseFiles);
                    }
                }
                );
        }
        
        #region Helpers
        public static void AddRange<T>(this ConcurrentBag<T> @this, IEnumerable<T> toAdd)
        {
            foreach (var element in toAdd)
            {
                @this.Add(element);
            }
        }
        #endregion
    }
}
