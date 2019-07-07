using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using TrademarkHistoryAnalysis.Models;

namespace TrademarkHistoryAnalysis.DAO
{
    public class CaseFilesDAO
    {
        private readonly string connectionString = "Data Source=";

        const string SelectCaseFiles =
@"SELECT FilingDate, SerialNumber, RegistrationDate,
RegistrationNumber, Owner, OwnerTypeId,
State, Country, Attorney, StatusCode, MarkLiteralElements, 
group_concat(CaseFileClass.InternationalCode,',') as InternationalCodes,
group_concat(CaseFileClass.GoodsAndServices ,'$') as GoodsAndServices from CaseFiles 
join CaseFileClass on CaseFileClass.CaseFileId = CaseFiles.CaseFileId GROUP BY CaseFiles.CaseFileId";
        const string SelectCaseFilesWithNoClasses =
@"SELECT FilingDate, SerialNumber, RegistrationDate,
RegistrationNumber, Owner, OwnerTypeId,
State, Country, Attorney, StatusCode, MarkLiteralElements, 
null as InternationalCodes,
null as GoodsAndServices from CaseFiles 
WHERE CaseFileId not in (select CaseFileId from CaseFileClass)";

        public CaseFilesDAO(string filename)
        {
            connectionString = connectionString + filename;
            CreateDataBase();
        }

        private bool CreateDataBase()
        {

            const string CreateCaseFiles = "CREATE TABLE CaseFiles (CaseFileId INTEGER PRIMARY KEY, FilingDate DATE, SerialNumber INTEGER, RegistrationDate DATE, RegistrationNumber INTEGER, Owner TEXT, OwnerTypeId INTEGER, State TEXT, Country TEXT, Attorney TEXT, StatusCode INTEGER, MarkLiteralElements TEXT);";
            const string CreateCaseFileClass = "CREATE TABLE CaseFileClass (CaseFileClassId INTEGER PRIMARY KEY, CaseFileId INTEGER, InternationalCode INTEGER, GoodsAndServices TEXT) ;";
            try
            {
                using (SqliteConnection sqliteConnection = new SqliteConnection(connectionString))
                {
                    sqliteConnection.Open();
                    using (SqliteCommand sqliteCommand = new SqliteCommand(CreateCaseFiles, sqliteConnection))
                    {
                        sqliteCommand.ExecuteNonQuery();
                        sqliteCommand.CommandText = CreateCaseFileClass;
                        sqliteCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public void SaveCaseFileList(List<CaseFile> caseFiles)
        {
            const string InsertCaseFile = "INSERT INTO CaseFiles (FilingDate, SerialNumber, RegistrationDate, RegistrationNumber, Owner, OwnerTypeId, State, Country, Attorney, StatusCode, MarkLiteralElements) VALUES (@FilingDate, @SerialNumber, @RegistrationDate, @RegistrationNumber, @Owner, @OwnerTypeId, @State, @Country, @Attorney, @StatusCode, @MarkLiteralElements)";
            const string SelectLastInsertId = "SELECT last_insert_rowid() as 'ID';";
            const string InsertClasses = "INSERT INTO CaseFileClass (CaseFileId, InternationalCode, GoodsAndServices) VALUES (@CaseFileId, @InternationalCode, @GoodsAndServices);";

            using (SqliteConnection sqliteConnection = new SqliteConnection(connectionString))
            {
                sqliteConnection.Open();
                using (SqliteCommand sqliteCommand = new SqliteCommand(InsertCaseFile, sqliteConnection))
                {
                    using (sqliteCommand.Transaction = sqliteConnection.BeginTransaction())
                    {
                        foreach (CaseFile file in caseFiles)
                        {
                            sqliteCommand.CommandText = InsertCaseFile;
                            sqliteCommand.Parameters.Clear();

                            sqliteCommand.Parameters.AddWithValue("@FilingDate", file.FilingDate);
                            sqliteCommand.Parameters.AddWithValue("@SerialNumber", file.SerialNumber);
                            sqliteCommand.Parameters.AddWithValue("@RegistrationNumber", DbNullIfNull(file.RegistrationNumber));
                            sqliteCommand.Parameters.AddWithValue("@RegistrationDate", DbNullIfNull(file.RegistrationDate));
                            sqliteCommand.Parameters.AddWithValue("@Owner", DbNullIfNull(file.Owner));
                            sqliteCommand.Parameters.AddWithValue("@OwnerTypeId", file.OwnerTypeId);
                            sqliteCommand.Parameters.AddWithValue("@State", DbNullIfNull(file.State));
                            sqliteCommand.Parameters.AddWithValue("@Country", file.Country ?? "US");
                            sqliteCommand.Parameters.AddWithValue("@Attorney", DbNullIfNull(file.Attorney));
                            sqliteCommand.Parameters.AddWithValue("@StatusCode", file.StatusCode);
                            sqliteCommand.Parameters.AddWithValue("@MarkLiteralElements", DbNullIfNull(file.MarkLiteralElements));

                            sqliteCommand.ExecuteNonQuery();

                            sqliteCommand.CommandText = SelectLastInsertId;
                            sqliteCommand.Parameters.Clear();

                            Int64 lastId = (Int64)sqliteCommand.ExecuteScalar();

                            sqliteCommand.CommandText = InsertClasses;

                            foreach (Classification c in file.Classes)
                            {
                                sqliteCommand.Parameters.Clear();
                                sqliteCommand.Parameters.AddWithValue("@CaseFileId", lastId);
                                sqliteCommand.Parameters.AddWithValue("@InternationalCode", c.InternationalCode);
                                sqliteCommand.Parameters.AddWithValue("@GoodsAndServices", DbNullIfNull(c.GoodsAndServices));
                                sqliteCommand.ExecuteNonQuery();
                            }
                        }
                        sqliteCommand.Transaction.Commit();
                    }
                }
            }
        }

        static private object DbNullIfNull(object obj)
        {
            if (obj == null)
            {
                return DBNull.Value;
            }

            return obj;
        }

        public List<CaseFile> GetAllCaseFiles()
        {
            List<CaseFile> caseFiles = new List<CaseFile>();

            using (SqliteConnection sqliteConnection = new SqliteConnection(connectionString))
            {
                sqliteConnection.Open();
                string[] queries = new string[] { SelectCaseFiles, SelectCaseFilesWithNoClasses };
                foreach (string q in queries) {
                    using (SqliteCommand sqliteCommand = new SqliteCommand(q, sqliteConnection))
                    {
                        SqliteDataReader reader = sqliteCommand.ExecuteReader();

                        while (reader.Read())
                        {
                            DateTime filingDate = DateTime.Parse((string)reader["FilingDate"]);
                            int serialNumber = (int)(Int64)reader["SerialNumber"];
                            DateTime? registrationDate = NullableDatabaseStringToNullableDateTime(reader["RegistrationDate"]);
                            int? registrationNumber = (int?)(Int64?)DatabaseNullableToNullable(reader["RegistrationNumber"]);
                            string owner = (string)reader["Owner"];
                            int ownerTypeId = (int)(Int64)reader["OwnerTypeId"];
                            string state = (string)DatabaseNullableToNullable(reader["State"]);
                            string country = (string)reader["Country"];
                            string attorney = (string)DatabaseNullableToNullable(reader["Attorney"]);
                            int statusCode = (int)(Int64)reader["StatusCode"];
                            string markLiteralElements = (string)DatabaseNullableToNullable(reader["MarkLiteralElements"]);
                            List<Classification> classifications = ClassificationsFromDatabaseData(reader["InternationalCodes"],
                                                                                      ',',
                                                                                      reader["GoodsAndServices"],
                                                                                      '$');

                            caseFiles.Add(new CaseFile(filingDate, serialNumber, registrationDate, registrationNumber, owner, ownerTypeId,
                                                       state, country, attorney, statusCode, markLiteralElements, classifications));
                        }
                    }
                }
            }

            return caseFiles;
        }

        private static List<Classification> ClassificationsFromDatabaseData(object internationalCodes, char internationalCodesSeparator, object goodsAndServices, char goodsAndServicesSeparator)
        {
            List<Classification> result = new List<Classification>();

            if (goodsAndServices == DBNull.Value) return result;

            string[] goods = ((string)goodsAndServices).Split(goodsAndServicesSeparator);
            int goodsCount = goods.Length;
            string[] codes = new string[goodsCount];

            if (internationalCodes != DBNull.Value)
            {
                codes = ((string)internationalCodes).Split(internationalCodesSeparator);
            }
          
            for (int i = 0; i < codes.Length; i++)
            {
                bool parsed = int.TryParse(codes[i], out int thisCode);
                result.Add(new Classification( parsed ? (int?)thisCode : null, goods.ElementAtOrDefault(i)));
            }      

            return result;
        }



        private static DateTime? NullableDatabaseStringToNullableDateTime(object date)
        {
            if (DBNull.Value == date)
            {
                return null;
            }

            return DateTime.Parse((string)date);
        }

        private static object DatabaseNullableToNullable(object obj)
        {
            if (DBNull.Value == obj)
            {
                return null;
            }

            return obj;
        }
    }
}
