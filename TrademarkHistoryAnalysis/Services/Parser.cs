using System;
using System.Collections.Generic;
using TrademarkHistoryAnalysis.Models;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Globalization;

namespace TrademarkHistoryAnalysis.Services
{
    public static class Parser
    {
        /// <summary>
        /// Returns true if the XML element e is a USPTO case file, in which the attribute "supplemental-register-in" is equal to F. F is short for false.
        /// </summary>
        /// <param name="element">XElement representing a case file</param>
        /// <returns>A bool indicating whether the element has a the attribute "supplemental-register-in" is equal to F</returns>
        private static bool IsOnPrincipalRegister(XElement element)
        {
            string supplemental = (string)element.Element("supplemental-register-in");
            return supplemental == "F"; //F is code for false
        }

        /// <summary>
        /// Attempts to parse a date from an XML element's child element 
        /// </summary>
        /// <param name="element">XElement representing a case file</param>
        /// <param name="datename">Name of child element containing a date string</param>
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
        /// Gets a list of class IDs from an XML "classfifications" element
        /// </summary>
        /// <param name="classifications">An IEnumerable XElement containing "classification" child elements</param>
        /// <returns>A list of class IDs</returns>
        private static List<int> GetClasses(IEnumerable<XElement> classifications)
        {
            List<int> result = new List<int>();

            foreach (XElement c in classifications)
            {
                if (int.TryParse((string)c.Element("primary-code"), out int code))
                {
                    result.Add(code);
                }
            }

            return result;
        }

        /// <summary>
        /// Parses goods and services text elements and concatanates them together into a semicolon separated string
        /// </summary>
        /// <param name="caseFileStatements">An IEnumerable XElement containing "case-file-statements" child elements</param>
        /// <returns></returns>
        private static string GetGoodsAndServices(IEnumerable<XElement> caseFileStatements)
        {
            List<string> texts = new List<string>();

            foreach (XElement s in caseFileStatements)
            {
                if (((string)s.Element("type-code")).StartsWith("GS")) texts.Add((string)s.Element("text")); //GS is code for Goods and Services
            }

            return String.Join(';', texts);
        }

        /// <summary>
        /// Parses a USPTO annual case file collection
        /// </summary>
        /// <param name="stream">A stream containing a USPTO annual case file collection encoded as XML</param>
        /// <returns>A list of CaseFile objects</returns>
        public static List<CaseFile> ParseXML(Stream stream)
        {
            
            XElement xElement = XElement.Load(stream);

            IEnumerable<XElement> searchResults = from case_file in xElement.Descendants("case-file")
                                                  where IsOnPrincipalRegister(case_file.Element("case-file-header")) &&
                                                       (case_file.Element("case-file-header") != null) &&
                                                       (case_file.Element("case-file-owners") != null) &&
                                                       (case_file.Element("serial-number") != null) &&
                                                       (case_file.Elements("case-file-statements") != null) &&
                                                       (case_file.Elements("classifications") != null)
                                                  select case_file;

            List<CaseFile> results = new List<CaseFile>();
            
            foreach (XElement e in searchResults)
            {
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

                results.Add(new CaseFile((DateTime)GetCaseFileDate(e, "filing-date"),
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
                            GetGoodsAndServices(e.Elements("case-file-statements").Elements("case-file-statement")),
                            GetClasses(e.Elements("classifications").Elements("classification"))
                            ));
            }

            stream.Dispose();

            return results;
        }

        /// <summary>
        /// Parses a USPTO annual case file collection
        /// </summary>
        /// <param name="filename">Filename of a zip file containing a single XML file containing a USPTO annual case file collection</param>
        /// <returns>A list of CaseFile objects</returns>
        public static List<CaseFile> ParseZippedXML(string filename)
        {
            List<CaseFile> caseFiles = new List<CaseFile>();

            using (Stream zipFileStream = File.Open(filename, FileMode.Open))
            {
                ZipArchive zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Read);
                caseFiles = ParseXML(zipArchive.Entries[0].Open());
            }
            return caseFiles;
        }
    }
}
