using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TrademarkHistoryAnalysis.DAO;
using TrademarkHistoryAnalysis.Models;
using TrademarkHistoryAnalysis.Services;

namespace TrademarkHistoryAnalysis
{
    public enum PathType
    {
        File,
        Directory,
        DoesNotExist
    }

    class Program
    {
        static void Main(string[] args)
        {         
            Console.WriteLine(Properties.Resources.Copyright.Replace(@"\n",Environment.NewLine));

            if (args.Length == 2)
            {
                string path = args[0];
                string databaseFilename = args[1];

                if (File.Exists(databaseFilename))
                {
                    File.Delete(databaseFilename);                   
                }

                DAO.CaseFilesDAO caseFilesDAO = new CaseFilesDAO(databaseFilename);

                PathType pathType = DeterminePathType(path);

                if (pathType == PathType.File)
                {
                    Console.WriteLine(Properties.Resources.StartedParsing, path);
                    List<CaseFile> caseFiles = Parser.ParseZippedXML(path);

                    GC.Collect();
                    caseFilesDAO.SaveCaseFileList(caseFiles);
                    Console.WriteLine(Properties.Resources.EndedParsing);
                }
                else if (pathType == PathType.Directory)
                {
                    Console.WriteLine("Started at " + DateTime.Now.ToString("HH:mm:ss"));
                    IEnumerable<string> files = Directory.EnumerateFiles(path, "*.zip", SearchOption.TopDirectoryOnly).Where(f => f.EndsWith(".zip", StringComparison.OrdinalIgnoreCase));

                    Console.WriteLine("Found {0} files", files.Count());

                    try
                    {
                        Parser.ParseInParallelWriteOnTheFly(files, new Parser.CaseFileWriter(caseFilesDAO.SaveCaseFileList));
                        Console.WriteLine("Finished at " + DateTime.Now.ToString("HH:mm:ss"));
                    }
                    catch (AggregateException /*aggregateException*/)
                    {
                        Console.WriteLine(Properties.Resources.IOErrorMessage);
                    }
                    catch (Exception otherException)
                    {
                        Console.WriteLine(Properties.Resources.GenericErrorMessage + otherException.Message);
                    }
                   
                }
                else
                {
                    Console.WriteLine(Properties.Resources.FileNotFound);
                }
            }
            else
            {
                Console.WriteLine(Properties.Resources.Instructions.Replace(@"\n", Environment.NewLine));
            }
            PressAnyKey();
        }
        
        private static PathType DeterminePathType(string path)
        {
            if (File.Exists(path))
            {
                return PathType.File;
            }

            if (Directory.Exists(path))
            {
                return PathType.Directory;
            }
            return PathType.DoesNotExist;
        }

        #region UI Help
        private static void PressAnyKey()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
        #endregion
    }
}
