using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Data.Common;
using System.Data;
using DeconTools.Backend.Core;
using DeconTools.Backend;
using DeconTools.Backend.DTO;
using GetPeaks_DLL.DataFIFO;
using MemoryOverloadProfilierX86;
using GetPeaks_DLL.SQLite.DataTransferObjects;
using YAFMS_DB.GetPeaks;


//1.  create simple object class outside.  This is a database transfer object with .columns that was used to write the database.  This also gives us a return type


//http://www.csharphacker.com/technicalblog/index.php/2009/06/28/sqlite-for-c-%e2%80%93-part-3-%e2%80%93-my-first-c-app-using-sqlite-aka-hello-world/
namespace GetPeaks_DLL.SQLite
{
    public class DatabaseLayer
    {
        private Object databaseLock = new Object();

        public bool IncrementTableInformationByOne(string fileName)
        {
            bool didThisWork = false;

            #region get existing count
            List<string> columnListInfo = new List<string>();
            columnListInfo.Add("Count");
            
            string commandTextStringGetCount = "";

            #region set string
            //m_GetFeatureCommmndString = "SELECT ID, Abundance, ChargeState, DriftTime, MonoisotopicMass, RetentionTime, Score FROM FeatureLiteTable WHERE rowid = ? ";//(ID double, Abundance double, ChargeState double, DriftTime double, myMassMonoisotopicColumn double, myRetentionTimeColumn double , myScoreColumn double);";
            commandTextStringGetCount = "SELECT ";
            for (int i = 0; i < columnListInfo.Count - 1; i++)
            {
                commandTextStringGetCount += columnListInfo[i] + ",";
            }
            commandTextStringGetCount += columnListInfo[columnListInfo.Count - 1] + " FROM TableInfo WHERE rowid = ? ";//last point
            #endregion
            string GetCountCommmndString = commandTextStringGetCount;

            //int doestheTableExists = Convert.ToInt32(getFeatureCommand.ExecuteScalar()); 

            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            int count = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getFeatureCommand = connection.CreateCommand())
                {
                    getFeatureCommand.CommandText = GetCountCommmndString;
                    getFeatureCommand.Prepare();

                    SQLiteParameter newSqliteParamater = new SQLiteParameter("rowid", 1);
                    getFeatureCommand.Parameters.Add(newSqliteParamater);
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getFeatureCommand.ExecuteReader();
                            databaseReader.Read();

                            count = Convert.ToInt32(databaseReader[columnListInfo[0]]);

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();
            }

            #endregion

            #region write new number
            List<string> columnList = new List<string>();
            columnList.Add("Count");
            

            string commandTextString = "";
            connectionString = "Data Source=" + fileName;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;

                    #region set command for column names
                    //command.CommandText = "CREATE TABLE IF NOT EXISTS FeatureLiteTable (ID double, Abundance double, ChargeState double, DriftTime double, MonoisotopicMass double, RetentionTime double , Score double);";
                    commandTextString = "CREATE TABLE IF NOT EXISTS TableInfo (";
                    for (int i = 0; i < columnList.Count - 1; i++)
                    {
                        commandTextString += columnList[i] + " double,";
                    }
                    commandTextString += columnList[columnList.Count - 1] + " double);";//last point
                    #endregion
                    command.CommandText = commandTextString;

                    try
                    {
                        command.ExecuteNonQuery();
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            #region set command string for inserts
                            //command.CommandText = "INSERT INTO FeatureLiteTable values (@ID, @Abundance, @ChargeState, @DriftTime, @MonoisotopicMass, @RetentionTime, @Score)";
                            int newCount = count+1;
                            commandTextString = "UPDATE TableInfo SET Count = " + newCount.ToString() + " WHERE rowid = 1 ";

                            #endregion
                            command.CommandText = commandTextString;

                            #region setup columns and add data to each column
                      //      DbParameter parameterNumber0 = command.CreateParameter();
                      //      parameterNumber0.ParameterName = columnList[0];
                      //      parameterNumber0.DbType = DbType.Int32;
                      //      command.Parameters.Add(parameterNumber0);

                           
                      //      parameterNumber0.Value = count+1;
                                
                            command.ExecuteNonQuery();

                            #endregion

                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();

            }
            #endregion

            return didThisWork;
        }

        public bool IncrementTableInformation(string fileName, string tableName, List<string> columnNames, List<int> amounts)
        {
            bool didThisWork = false;

            #region get existing count

            List<string> columnListInfo = columnNames;
            //columnListInfo.Add(ColumnName);

            string commandTextStringGetCount = "";

            //#region set string
            ////m_GetFeatureCommmndString = "SELECT ID, Abundance, ChargeState, DriftTime, MonoisotopicMass, RetentionTime, Score FROM FeatureLiteTable WHERE rowid = ? ";//(ID double, Abundance double, ChargeState double, DriftTime double, myMassMonoisotopicColumn double, myRetentionTimeColumn double , myScoreColumn double);";
            //commandTextStringGetCount = "SELECT ";
            //for (int i = 0; i < columnListInfo.Count - 1; i++)
            //{
            //    commandTextStringGetCount += columnListInfo[i] + ",";
            //}
            //commandTextStringGetCount += columnListInfo[columnListInfo.Count - 1] + " FROM " + tableName + " WHERE rowid = ? ";//last point
            //#endregion
            //string GetCountCommmndString = commandTextStringGetCount;

            //int doestheTableExists = Convert.ToInt32(getFeatureCommand.ExecuteScalar()); 

            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            List<int> counts = new List<int>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                for (int i = 0; i < columnNames.Count; i++)
                {

                    using (SQLiteCommand getFeatureCommand = connection.CreateCommand())
                    {

                        commandTextStringGetCount = "SELECT ";
                        commandTextStringGetCount += columnNames[i] + " FROM " + tableName + " WHERE rowid = ? ";
                            //last point

                        getFeatureCommand.CommandText = commandTextStringGetCount;
                        //getFeatureCommand.CommandText = GetCountCommmndString;
                        getFeatureCommand.Prepare();

                        SQLiteParameter newSqliteParamater = new SQLiteParameter("rowid", 1);
                        getFeatureCommand.Parameters.Add(newSqliteParamater);
                        try
                        {
                            using (SQLiteTransaction transaction = connection.BeginTransaction())
                            {
                                SQLiteDataReader databaseReader = getFeatureCommand.ExecuteReader();
                                databaseReader.Read();

                                counts.Add(Convert.ToInt32(databaseReader[columnNames[i]]));

                                databaseReader.Close();
                                transaction.Commit();

                                didThisWork = true;
                            }
                        }

                        catch (SQLiteException exception)
                        {
                            didThisWork = false;
                            Console.WriteLine("Failed :" + exception.Message);
                        }
                        finally
                        {
                            if (i == columnNames.Count)
                            {
                                connection.Close();//this use to be here for a single transaction.  this may not be necessary and only affect the last read
                            }
                        }
                    }
                }
                connection.Close();

            }

            #endregion

            #region write new number
            //List<string> columnList = new List<string>();
            //columnList.Add(columnNames[0]);


            List<string> columnList = columnNames;
            //columnList.Add(columnNames[0]);
            string commandTextString = "";
            connectionString = "Data Source=" + fileName;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;

                    #region set command for column names
                    //command.CommandText = "CREATE TABLE IF NOT EXISTS FeatureLiteTable (ID double, Abundance double, ChargeState double, DriftTime double, MonoisotopicMass double, RetentionTime double , Score double);";
                    //commandTextString = "CREATE TABLE IF NOT EXISTS "+ tableName + " (";
                    //for (int i = 0; i < columnList.Count - 1; i++)
                    //{
                    //    commandTextString += columnList[i] + " double,";
                    //}
                    //commandTextString += columnList[columnList.Count - 1] + " double);";//last point
                    #endregion
                    //command.CommandText = commandTextString;

                    for (int j = 0; j < columnNames.Count; j++)
                    {
                        try
                        {
                            commandTextString = "CREATE TABLE IF NOT EXISTS " + tableName + " (";
                            for (int i = 0; i < columnList.Count - 1; i++)
                            {
                                commandTextString += columnList[i] + " double,";
                            }
                            commandTextString += columnList[columnList.Count - 1] + " double);"; //last point
                            command.CommandText = commandTextString;
                            command.ExecuteNonQuery();


                            using (SQLiteTransaction transaction = connection.BeginTransaction())
                            {
                                #region set command string for inserts

                                //command.CommandText = "INSERT INTO FeatureLiteTable values (@ID, @Abundance, @ChargeState, @DriftTime, @MonoisotopicMass, @RetentionTime, @Score)";
                                int newCount = counts[j] + amounts[j];
                                //commandTextString = "INSERT " + tableName + " SET " + columnNames[j] + " = " + newCount.ToString() + " WHERE rowid = 1 ";
                                //commandTextString = "UPDATE " + tableName + " SET " + columnNames[j] + " = " + newCount.ToString() + " WHERE rowid = 1 ";
                                //commandTextString = "UPDATE " + tableName + " SET " + columnNames[j] + " = " + newCount.ToString() + " WHERE rowid = 1 LIMIT 1";
                                commandTextString = "UPDATE " + tableName + " SET " + columnNames[j] + " = " + newCount.ToString() + " WHERE rowid = 1";
                                //commandTextString = "INSERT OR REPLACE " + tableName + " SET " + columnNames[j] + " = " + newCount.ToString() + " WHERE rowid = 1 ";
                                //commandTextString = "INSERT OR REPLACE " + tableName + " SET " + columnNames[j] + " = " + newCount.ToString() + " WHERE id = 1 ";
                                #endregion

                                command.CommandText = commandTextString;

                                #region setup columns and add data to each column

                                //      DbParameter parameterNumber0 = command.CreateParameter();
                                //      parameterNumber0.ParameterName = columnList[0];
                                //      parameterNumber0.DbType = DbType.Int32;
                                //      command.Parameters.Add(parameterNumber0);


                                //      parameterNumber0.Value = count+1;

                                command.ExecuteNonQuery();

                                #endregion

                                transaction.Commit();

                                didThisWork = true;
                            }
                        }

                        catch (SQLiteException exception)
                        {
                            didThisWork = false;
                            Console.WriteLine("Failed :" + exception.Message);
                        }
                        finally
                        {
                            //connection.Close();
                        }
                    }
                }
                connection.Close();

            }
            #endregion

            return didThisWork;
        }

        public bool IncrementTableInformation(string fileName, string tableName, string columnNames, int amounts)
        {
            bool didThisWork = false;

            #region get existing count

            
            //columnListInfo.Add(ColumnName);

            string commandTextStringGetCount = "";

            //#region set string
            ////m_GetFeatureCommmndString = "SELECT ID, Abundance, ChargeState, DriftTime, MonoisotopicMass, RetentionTime, Score FROM FeatureLiteTable WHERE rowid = ? ";//(ID double, Abundance double, ChargeState double, DriftTime double, myMassMonoisotopicColumn double, myRetentionTimeColumn double , myScoreColumn double);";
            //commandTextStringGetCount = "SELECT ";
            //for (int i = 0; i < columnListInfo.Count - 1; i++)
            //{
            //    commandTextStringGetCount += columnListInfo[i] + ",";
            //}
            //commandTextStringGetCount += columnListInfo[columnListInfo.Count - 1] + " FROM " + tableName + " WHERE rowid = ? ";//last point
            //#endregion
            //string GetCountCommmndString = commandTextStringGetCount;

            //int doestheTableExists = Convert.ToInt32(getFeatureCommand.ExecuteScalar()); 

            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            int counts = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                for (int i = 0; i < 1; i++)
                {

                    using (SQLiteCommand getFeatureCommand = connection.CreateCommand())
                    {

                        commandTextStringGetCount = "SELECT ";
                        commandTextStringGetCount += columnNames + " FROM " + tableName + " WHERE rowid = ? ";
                        //last point

                        getFeatureCommand.CommandText = commandTextStringGetCount;
                        //getFeatureCommand.CommandText = GetCountCommmndString;
                        getFeatureCommand.Prepare();

                        SQLiteParameter newSqliteParamater = new SQLiteParameter("rowid", 1);
                        getFeatureCommand.Parameters.Add(newSqliteParamater);
                        try
                        {
                            using (SQLiteTransaction transaction = connection.BeginTransaction())
                            {
                                SQLiteDataReader databaseReader = getFeatureCommand.ExecuteReader();
                                databaseReader.Read();

                                counts = Convert.ToInt32(databaseReader[columnNames]);

                                databaseReader.Close();
                                transaction.Commit();

                                didThisWork = true;
                            }
                        }

                        catch (SQLiteException exception)
                        {
                            didThisWork = false;
                            Console.WriteLine("Failed :" + exception.Message);
                        }
                        finally
                        {
                            if (i == 1)
                            {
                                connection.Close();//this use to be here for a single transaction.  this may not be necessary and only affect the last read
                            }
                        }
                    }
                }
                connection.Close();

            }

            #endregion

            #region write new number
            //List<string> columnList = new List<string>();
            //columnList.Add(columnNames[0]);


            
            //columnList.Add(columnNames[0]);
            string commandTextString = "";
            connectionString = "Data Source=" + fileName;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;

                    #region set command for column names
                    //command.CommandText = "CREATE TABLE IF NOT EXISTS FeatureLiteTable (ID double, Abundance double, ChargeState double, DriftTime double, MonoisotopicMass double, RetentionTime double , Score double);";
                    //commandTextString = "CREATE TABLE IF NOT EXISTS "+ tableName + " (";
                    //for (int i = 0; i < columnList.Count - 1; i++)
                    //{
                    //    commandTextString += columnList[i] + " double,";
                    //}
                    //commandTextString += columnList[columnList.Count - 1] + " double);";//last point
                    #endregion
                    //command.CommandText = commandTextString;

                    for (int j = 0; j < 1; j++)
                    {
                        try
                        {
                            commandTextString = "CREATE TABLE IF NOT EXISTS " + tableName + " (";
                            commandTextString += columnNames + " double);"; //last point
                            command.CommandText = commandTextString;
                            command.ExecuteNonQuery();


                            using (SQLiteTransaction transaction = connection.BeginTransaction())
                            {
                                #region set command string for inserts

                                //command.CommandText = "INSERT INTO FeatureLiteTable values (@ID, @Abundance, @ChargeState, @DriftTime, @MonoisotopicMass, @RetentionTime, @Score)";
                                int newCount = counts + amounts;
                                //commandTextString = "INSERT " + tableName + " SET " + columnNames[j] + " = " + newCount.ToString() + " WHERE rowid = 1 ";
                                //commandTextString = "UPDATE " + tableName + " SET " + columnNames[j] + " = " + newCount.ToString() + " WHERE rowid = 1 ";
                                //commandTextString = "UPDATE " + tableName + " SET " + columnNames[j] + " = " + newCount.ToString() + " WHERE rowid = 1 LIMIT 1";
                                commandTextString = "UPDATE " + tableName + " SET " + columnNames + " = " + newCount.ToString() + " WHERE rowid = 1";
                                //commandTextString = "INSERT OR REPLACE " + tableName + " SET " + columnNames[j] + " = " + newCount.ToString() + " WHERE rowid = 1 ";
                                //commandTextString = "INSERT OR REPLACE " + tableName + " SET " + columnNames[j] + " = " + newCount.ToString() + " WHERE id = 1 ";
                                #endregion

                                command.CommandText = commandTextString;

                                #region setup columns and add data to each column

                                //      DbParameter parameterNumber0 = command.CreateParameter();
                                //      parameterNumber0.ParameterName = columnList[0];
                                //      parameterNumber0.DbType = DbType.Int32;
                                //      command.Parameters.Add(parameterNumber0);


                                //      parameterNumber0.Value = count+1;

                                command.ExecuteNonQuery();

                                #endregion

                                transaction.Commit();

                                didThisWork = true;
                            }
                        }

                        catch (SQLiteException exception)
                        {
                            didThisWork = false;
                            Console.WriteLine("Failed :" + exception.Message);
                        }
                        finally
                        {
                            //connection.Close();
                        }
                    }
                }
                connection.Close();

            }
            #endregion

            return didThisWork;
        }

        public int ReadDatabaseSize(string fileName, string tableName, string columnName)
        {
            bool didThisWork = false;

            #region get existing count
            List<string> columnListInfo = new List<string>();
            columnListInfo.Add(columnName);

            string commandTextStringGetCount = "";

            #region set string
            //m_GetFeatureCommmndString = "SELECT ID, Abundance, ChargeState, DriftTime, MonoisotopicMass, RetentionTime, Score FROM FeatureLiteTable WHERE rowid = ? ";//(ID double, Abundance double, ChargeState double, DriftTime double, myMassMonoisotopicColumn double, myRetentionTimeColumn double , myScoreColumn double);";
            commandTextStringGetCount = "SELECT ";
            for (int i = 0; i < columnListInfo.Count - 1; i++)
            {
                commandTextStringGetCount += columnListInfo[i] + ",";
            }
            commandTextStringGetCount += columnListInfo[columnListInfo.Count - 1] + " FROM " + tableName + " WHERE rowid = ? ";//last point
            #endregion
            string GetCountCommmndString = commandTextStringGetCount;

            //int doestheTableExists = Convert.ToInt32(getFeatureCommand.ExecuteScalar()); 

            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            int count = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getFeatureCommand = connection.CreateCommand())
                {
                    getFeatureCommand.CommandText = GetCountCommmndString;
                    getFeatureCommand.Prepare();

                    SQLiteParameter newSqliteParamater = new SQLiteParameter("rowid", 1);
                    getFeatureCommand.Parameters.Add(newSqliteParamater);
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getFeatureCommand.ExecuteReader();
                            databaseReader.Read();

                            count = Convert.ToInt32(databaseReader[columnListInfo[0]]);

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();
            }

            #endregion

            if (didThisWork)
            {
                Console.WriteLine("     Database Size Obtained");
            }
            else
            {
                Console.WriteLine("     Could not read database size");
            }
            return count;
        }

        public PrecursorAndPeaksObject SK_SelectPrecursorAndMonoPeaks(string fileName, string tableNamePrecursor, string tableNamePeaks, int scanNumber, DatabaseTransferObject sampleObjectPrecursor, DatabaseTransferObject sampleObjectPeaks)
        {
            string aliasTable1Precursor = "SPP";//tableNamePrecursor
            string aliasTable2MonoPeaks = "SM";//tableNamePeaks

            //converts column names to field names so we can extract the information back out
            DatabasePeakProcessedWithMZObject currentSampleObjectPrecursor = (DatabasePeakProcessedWithMZObject)sampleObjectPrecursor;
            List<string> fieldNamePrecursor = new List<string>();
            foreach (string column in currentSampleObjectPrecursor.Columns)
            {
                //fieldNamePrecursor.Add(variableTable1 + "." + column);
                fieldNamePrecursor.Add(aliasTable1Precursor + "." + column + " AS '" + aliasTable1Precursor + "_" + column + "'");
            }

            DatabasePeakProcessedObject currentSampleObjectPeaks = (DatabasePeakProcessedObject)sampleObjectPeaks;
            List<string> fieldNamePeaks = new List<string>();
            foreach (string column in currentSampleObjectPeaks.Columns)
            {
                //fieldNamePeaks.Add(variableTable2 + "." + column);
                fieldNamePeaks.Add(aliasTable2MonoPeaks + "." + column + " AS '" + aliasTable2MonoPeaks + "_" + column + "'");
            }

            //this is where the results will end up
            PrecursorAndPeaksObject results = new PrecursorAndPeaksObject();
            bool didThisWork = false;

            //set up connection string to database
            string connectionString = "Data Source=" + fileName;

            //string variable1 = "T_Scan_MonoPeaks.Scan";//column matches to input scanNumber
            //string variable1 = aliasTable2 + ".Scan";//column matches to input scanNumber  //DOES NOT WORK.  leads to over run
            string variable1 = aliasTable1Precursor + ".TandemScan";//column matches to input scanNumber.  

            //Set up SQLite statement
            string commandTextStringGetCount;
            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNamePrecursor) + "";
            commandTextStringGetCount += ", ";
            commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNamePeaks) + " ";
            commandTextStringGetCount += "FROM " + tableNamePrecursor + " " + aliasTable1Precursor + " ";
            commandTextStringGetCount += "INNER JOIN " + tableNamePeaks + " " + aliasTable2MonoPeaks + " ";
            commandTextStringGetCount += "ON (" + aliasTable1Precursor + ".TandemScan = " + aliasTable2MonoPeaks + ".Scan) ";
            commandTextStringGetCount += "WHERE " + variable1 + "= ? ";

            #region querry as Text
            //SELECT SPP.TandemScan AS 'SPP_TandemScan', SPP.PrecursorScan AS 'SPP_PrecursorScan', SPP.PeakNumber AS 'SPP_PeakNumber', SPP.XValue AS 'SPP_XValue', SPP.XValueRaw AS 'SPP_XValueRaw', SPP.Height AS 'SPP_Height', SPP.LocalSignalToNoise AS 'SPP_LocalSignalToNoise', SPP.Background AS 'SPP_Background', SPP.Width AS 'SPP_Width', SPP.LocalLowestMinimaHeight AS 'SPP_LocalLowestMinimaHeight', SPP.SignalToBackground AS 'SPP_SignalToBackground', SPP.SignalToNoiseGlobal AS 'SPP_SignalToNoiseGlobal', SPP.SignalToNoiseLocalMinima AS 'SPP_SignalToNoiseLocalMinima' ,
            //       SM.Scan AS 'SM_Scan', SM.PeakNumber AS 'SM_PeakNumber', SM.XValue AS 'SM_XValue', SM.Height AS 'SM_Height', SM.LocalSignalToNoise AS 'SM_LocalSignalToNoise', SM.Background AS 'SM_Background', SM.Width AS 'SM_Width', SM.LocalLowestMinimaHeight AS 'SM_LocalLowestMinimaHeight', SM.SignalToBackground AS 'SM_SignalToBackground', SM.SignalToNoiseGlobal AS 'SM_SignalToNoiseGlobal', SM.SignalToNoiseLocalMinima AS 'SM_SignalToNoiseLocalMinima'
            //
            //FROM T_Scans_Precursor_Peaks SPP 
            //    INNER JOIN T_Scan_MonoPeaks SM
            //        ON (SPP.TandemScan = SM.Scan)
            //WHERE SPP.TandemScan= ?
            #endregion

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;

                    //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list
                    SQLiteParameter newSqliteParamater = new SQLiteParameter(variable1, scanNumber);

                    getIsotopesCommmnd.CreateParameter();
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    getIsotopesCommmnd.Prepare();

                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();
                            int counter = 0;
                            while (databaseReader.Read())
                            {
                                DatabasePeakProcessedWithMZObject loadedMonoIsotopeObject = new DatabasePeakProcessedWithMZObject();
                                
                                //foreach (DbDataRecord VARIABLE in databaseReader)
                                //{

                                //    loadedMonoIsotopeObject.ScanNum = Convert.ToInt32(VARIABLE.GetValue(0));
                                //    loadedMonoIsotopeObject.PeakNumber = Convert.ToInt32(VARIABLE.GetValue(1));

                                //}

                                //Console.WriteLine(counter);
                                //counter++;

                                //one
                                results.PrecursorPeak.ScanNumberTandem = Convert.ToInt32(databaseReader["SPP_TandemScan"]);
                                results.PrecursorPeak.ScanNumberPrecursor = Convert.ToInt32(databaseReader["SPP_PrecursorScan"]);
                                results.PrecursorPeak.PeakNumber = Convert.ToInt32(databaseReader["SPP_PeakNumber"]);
                                results.PrecursorPeak.XValue = Convert.ToDouble(databaseReader["SPP_XValue"]);
                                results.PrecursorPeak.XValueRaw = Convert.ToDouble(databaseReader["SPP_XValueRaw"]);
                                results.PrecursorPeak.Charge = Convert.ToInt32(databaseReader["SPP_Charge"]);
                                results.PrecursorPeak.Height = Convert.ToDouble(databaseReader["SPP_Height"]);
                                results.PrecursorPeak.LocalSignalToNoise = Convert.ToDouble(databaseReader["SPP_LocalSignalToNoise"]);
                                results.PrecursorPeak.Background = Convert.ToDouble(databaseReader["SPP_Background"]);
                                results.PrecursorPeak.Width = Convert.ToDouble(databaseReader["SPP_Width"]);
                                results.PrecursorPeak.LocalLowestMinimaHeight = Convert.ToDouble(databaseReader["SPP_LocalLowestMinimaHeight"]);
                                results.PrecursorPeak.SignalToBackground = Convert.ToDouble(databaseReader["SPP_SignalToBackground"]);
                                results.PrecursorPeak.SignalToNoiseGlobal = Convert.ToDouble(databaseReader["SPP_SignalToNoiseGlobal"]);
                                results.PrecursorPeak.SignalToNoiseLocalMinima = Convert.ToDouble(databaseReader["SPP_SignalToNoiseLocalMinima"]);

                                //many
                                loadedMonoIsotopeObject.ScanNumberTandem = Convert.ToInt32(databaseReader["SM_Scan"]);//scan num
                                loadedMonoIsotopeObject.PeakNumber = Convert.ToInt32(databaseReader["SM_PeakNumber"]);
                                loadedMonoIsotopeObject.XValue = Convert.ToDouble(databaseReader["SM_XValue"]);
                                loadedMonoIsotopeObject.Height = Convert.ToDouble(databaseReader["SM_Height"]);
                                loadedMonoIsotopeObject.LocalSignalToNoise = Convert.ToDouble(databaseReader["SM_LocalSignalToNoise"]);
                                loadedMonoIsotopeObject.Background = Convert.ToDouble(databaseReader["SM_Background"]);
                                loadedMonoIsotopeObject.Width = Convert.ToDouble(databaseReader["SM_Width"]);
                                loadedMonoIsotopeObject.LocalLowestMinimaHeight = Convert.ToDouble(databaseReader["SM_LocalLowestMinimaHeight"]);
                                loadedMonoIsotopeObject.SignalToBackground = Convert.ToDouble(databaseReader["SM_SignalToBackground"]);
                                loadedMonoIsotopeObject.SignalToNoiseGlobal = Convert.ToDouble(databaseReader["SM_SignalToNoiseGlobal"]);
                                loadedMonoIsotopeObject.SignalToNoiseLocalMinima = Convert.ToDouble(databaseReader["SM_SignalToNoiseLocalMinima"]);

                                results.TandemMonoPeakList.Add(loadedMonoIsotopeObject);

                                results.ScanNum = results.PrecursorPeak.ScanNumberTandem;
                                results.XValue = results.PrecursorPeak.XValueRaw;
                                results.Charge = results.PrecursorPeak.Charge;
                            }

                            //results.PeakNumber = results.TandemPeakList.Count;

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();

                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }

            return results;
            //return didThisWork;
        }

        public PrecursorAndPeaksObject SK_SelectPrecursorAndPeaks(string fileName, string tableNamePrecursor, string tableNamePeaks, int scanNumber, DatabaseTransferObject sampleObjectPrecursor, DatabaseTransferObject sampleObjectPeaks)
        {
            string aliasTable1Precursor = "SPP";//tableNamePrecursor
            string aliasTable2MonoPeaks = "SP";//tableNamePeaks

            //converts column names to field names so we can extract the information back out
            DatabasePeakProcessedWithMZObject currentSampleObjectPrecursor = (DatabasePeakProcessedWithMZObject)sampleObjectPrecursor;
            List<string> fieldNamePrecursor = new List<string>();
            foreach (string column in currentSampleObjectPrecursor.Columns)
            {
                //fieldNamePrecursor.Add(variableTable1 + "." + column);
                fieldNamePrecursor.Add(aliasTable1Precursor + "." + column + " AS '" + aliasTable1Precursor + "_" + column + "'");
            }

            DatabasePeakProcessedObject currentSampleObjectPeaks = (DatabasePeakProcessedObject)sampleObjectPeaks;
            List<string> fieldNamePeaks = new List<string>();
            foreach (string column in currentSampleObjectPeaks.Columns)
            {
                //fieldNamePeaks.Add(variableTable2 + "." + column);
                fieldNamePeaks.Add(aliasTable2MonoPeaks + "." + column + " AS '" + aliasTable2MonoPeaks + "_" + column + "'");
            }

            //this is where the results will end up
            PrecursorAndPeaksObject results = new PrecursorAndPeaksObject();
            bool didThisWork = false;

            //set up connection string to database
            string connectionString = "Data Source=" + fileName;

            //string variable1 = "T_Scan_MonoPeaks.Scan";//column matches to input scanNumber
            //string variable1 = aliasTable2 + ".Scan";//column matches to input scanNumber  //DOES NOT WORK.  leads to over run
            string variable1 = aliasTable1Precursor + ".TandemScan";//column matches to input scanNumber.  

            //Set up SQLite statement
            string commandTextStringGetCount;
            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNamePrecursor) + "";
            commandTextStringGetCount += ", ";
            commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNamePeaks) + " ";
            commandTextStringGetCount += "FROM " + tableNamePrecursor + " " + aliasTable1Precursor + " ";
            commandTextStringGetCount += "INNER JOIN " + tableNamePeaks + " " + aliasTable2MonoPeaks + " ";
            commandTextStringGetCount += "ON (" + aliasTable1Precursor + ".TandemScan = " + aliasTable2MonoPeaks + ".Scan) ";
            commandTextStringGetCount += "WHERE " + variable1 + "= ? ";

            #region querry as Text
            //SELECT SPP.TandemScan AS 'SPP_TandemScan', SPP.PrecursorScan AS 'SPP_PrecursorScan', SPP.PeakNumber AS 'SPP_PeakNumber', SPP.XValue AS 'SPP_XValue', SPP.XValueRaw AS 'SPP_XValueRaw', SPP.Height AS 'SPP_Height', SPP.LocalSignalToNoise AS 'SPP_LocalSignalToNoise', SPP.Background AS 'SPP_Background', SPP.Width AS 'SPP_Width', SPP.LocalLowestMinimaHeight AS 'SPP_LocalLowestMinimaHeight', SPP.SignalToBackground AS 'SPP_SignalToBackground', SPP.SignalToNoiseGlobal AS 'SPP_SignalToNoiseGlobal', SPP.SignalToNoiseLocalMinima AS 'SPP_SignalToNoiseLocalMinima' ,
            //       SM.Scan AS 'SM_Scan', SM.PeakNumber AS 'SM_PeakNumber', SM.XValue AS 'SM_XValue', SM.Height AS 'SM_Height', SM.LocalSignalToNoise AS 'SM_LocalSignalToNoise', SM.Background AS 'SM_Background', SM.Width AS 'SM_Width', SM.LocalLowestMinimaHeight AS 'SM_LocalLowestMinimaHeight', SM.SignalToBackground AS 'SM_SignalToBackground', SM.SignalToNoiseGlobal AS 'SM_SignalToNoiseGlobal', SM.SignalToNoiseLocalMinima AS 'SM_SignalToNoiseLocalMinima'
            //
            //FROM T_Scans_Precursor_Peaks SPP 
            //    INNER JOIN T_Scan_MonoPeaks SM
            //        ON (SPP.TandemScan = SM.Scan)
            //WHERE SPP.TandemScan= ?
            #endregion

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;

                    //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list
                    SQLiteParameter newSqliteParamater = new SQLiteParameter(variable1, scanNumber);

                    getIsotopesCommmnd.CreateParameter();
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    getIsotopesCommmnd.Prepare();

                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();
                            int counter = 0;
                            while (databaseReader.Read())
                            {
                                DatabasePeakProcessedObject loadedMonoIsotopeObject = new DatabasePeakProcessedObject();

                                //foreach (DbDataRecord VARIABLE in databaseReader)
                                //{

                                //    loadedMonoIsotopeObject.ScanNum = Convert.ToInt32(VARIABLE.GetValue(0));
                                //    loadedMonoIsotopeObject.PeakNumber = Convert.ToInt32(VARIABLE.GetValue(1));

                                //}

                                //Console.WriteLine(counter);
                                //counter++;

                                results.PrecursorPeak.ScanNumberTandem = Convert.ToInt32(databaseReader["SPP_TandemScan"]);
                                results.PrecursorPeak.ScanNumberPrecursor = Convert.ToInt32(databaseReader["SPP_PrecursorScan"]);
                                results.PrecursorPeak.PeakNumber = Convert.ToInt32(databaseReader["SPP_PeakNumber"]);
                                results.PrecursorPeak.XValue = Convert.ToDouble(databaseReader["SPP_XValue"]);
                                results.PrecursorPeak.XValueRaw = Convert.ToDouble(databaseReader["SPP_XValueRaw"]);
                                results.PrecursorPeak.Charge = Convert.ToInt32(databaseReader["SPP_Charge"]);
                                results.PrecursorPeak.Height = Convert.ToDouble(databaseReader["SPP_Height"]);
                                results.PrecursorPeak.LocalSignalToNoise = Convert.ToDouble(databaseReader["SPP_LocalSignalToNoise"]);
                                results.PrecursorPeak.Background = Convert.ToDouble(databaseReader["SPP_Background"]);
                                results.PrecursorPeak.Width = Convert.ToDouble(databaseReader["SPP_Width"]);
                                results.PrecursorPeak.LocalLowestMinimaHeight = Convert.ToDouble(databaseReader["SPP_LocalLowestMinimaHeight"]);
                                results.PrecursorPeak.SignalToBackground = Convert.ToDouble(databaseReader["SPP_SignalToBackground"]);
                                results.PrecursorPeak.SignalToNoiseGlobal = Convert.ToDouble(databaseReader["SPP_SignalToNoiseGlobal"]);
                                results.PrecursorPeak.SignalToNoiseLocalMinima = Convert.ToDouble(databaseReader["SPP_SignalToNoiseLocalMinima"]);

                                loadedMonoIsotopeObject.ScanNum = Convert.ToInt32(databaseReader["SP_Scan"]);
                                loadedMonoIsotopeObject.PeakNumber = Convert.ToInt32(databaseReader["SP_PeakNumber"]);
                                loadedMonoIsotopeObject.XValue = Convert.ToDouble(databaseReader["SP_XValue"]);
                                loadedMonoIsotopeObject.Height = Convert.ToDouble(databaseReader["SP_Height"]);
                                loadedMonoIsotopeObject.LocalSignalToNoise = Convert.ToDouble(databaseReader["SP_LocalSignalToNoise"]);
                                loadedMonoIsotopeObject.Background = Convert.ToDouble(databaseReader["SP_Background"]);
                                loadedMonoIsotopeObject.Width = Convert.ToDouble(databaseReader["SP_Width"]);
                                loadedMonoIsotopeObject.LocalLowestMinimaHeight = Convert.ToDouble(databaseReader["SP_LocalLowestMinimaHeight"]);
                                loadedMonoIsotopeObject.SignalToBackground = Convert.ToDouble(databaseReader["SP_SignalToBackground"]);
                                loadedMonoIsotopeObject.SignalToNoiseGlobal = Convert.ToDouble(databaseReader["SP_SignalToNoiseGlobal"]);
                                loadedMonoIsotopeObject.SignalToNoiseLocalMinima = Convert.ToDouble(databaseReader["SP_SignalToNoiseLocalMinima"]);

                                results.TandemPeakList.Add(loadedMonoIsotopeObject);

                                results.ScanNum = results.PrecursorPeak.ScanNumberTandem;
                                results.XValue = results.PrecursorPeak.XValueRaw;
                                results.Charge = results.PrecursorPeak.Charge;
                            }

                            //results.PeakNumber = results.TandemPeakList.Count;

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();

                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }

            return results;
            //return didThisWork;
        }

        //public void SK2_Final_SelectPeaksWithAttributes(string fileName, string tablenamePeaks, string tableNameAttribute, int scanNumber, DatabaseTransferObject sampleObjectAttributeCentric, DatabaseTransferObject sampleObjectPeakCentric, out DatabasePeakCentricObjectList resultPeaks, out DatabaseAttributeCentricObjectList resultsAttributes, bool isMono)
        public void SK2_Final_SelectPeaksWithAttributes(string fileName, string tablenamePeaks, int scanNumber, DatabaseTransferObject sampleObjectPeakCentric, out DatabasePeakCentricObjectList resultPeaks, bool isMono)
        {
            string aliasTable1Peaks = "TP";//tableNamepeaks
//            string aliasTableAtributes = "TA";//tableNameAtributes

            //converts column names to field names so we can extract the information back out
            //DatabaseAttributeCentricObject currentSampleObjectAttribute = (DatabaseAttributeCentricObject)sampleObjectAttributeCentric;
            //List<string> fieldNameAttributes = new List<string>();
            //foreach (string column in currentSampleObjectAttribute.Columns)
            //{
            //    //fieldNamePrecursor.Add(variableTable1 + "." + column);
            //    fieldNameAttributes.Add(aliasTableAtributes + "." + column + " AS '" + aliasTableAtributes + "_" + column + "'");
            //}

            DatabasePeakCentricObject currentSampleObjectPeaks = (DatabasePeakCentricObject)sampleObjectPeakCentric;
            List<string> fieldNamePeaks = new List<string>();
            foreach (string column in currentSampleObjectPeaks.Columns)
            {
                //fieldNamePeaks.Add(variableTable2 + "." + column);
                fieldNamePeaks.Add(aliasTable1Peaks + "." + column + " AS '" + aliasTable1Peaks + "_" + column + "'");
                
            }

            //this is where the results will end up
           
            bool didThisWork = false;

            //set up connection string to database
            string connectionString = "Data Source=" + fileName;

            //string variable1 = "T_Scan_MonoPeaks.Scan";//column matches to input scanNumber
            //string variable1 = aliasTable2 + ".Scan";//column matches to input scanNumber  //DOES NOT WORK.  leads to over run
            string variable1 = aliasTable1Peaks + ".ScanID";//column matches to input scanNumber. 
            

            //Set up SQLite statement
            string commandTextStringGetCount;
            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
 //           commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNameAttributes) + "";
 //           commandTextStringGetCount += ", ";
            commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNamePeaks) + " ";
            commandTextStringGetCount += "FROM " + tablenamePeaks + " " + aliasTable1Peaks + " ";//where alias is connected
 //           commandTextStringGetCount += "INNER JOIN " + tableNameAttribute + " " + aliasTableAtributes + " ";
 //           commandTextStringGetCount += "ON (" + aliasTable1Peaks + ".PeakID = " + aliasTableAtributes + ".PeakID) ";
            commandTextStringGetCount += "WHERE " + variable1 + "= ? ";
            if (isMono)
            {
//                commandTextStringGetCount += "AND " + aliasTableAtributes + ".isMonoisotopic= 1 ";
                commandTextStringGetCount += "AND " + aliasTable1Peaks + ".isMonoisotopic= 1 ";
            }

            #region querry as Text
            //SELECT SPP.TandemScan AS 'SPP_TandemScan', SPP.PrecursorScan AS 'SPP_PrecursorScan', SPP.PeakNumber AS 'SPP_PeakNumber', SPP.XValue AS 'SPP_XValue', SPP.XValueRaw AS 'SPP_XValueRaw', SPP.Height AS 'SPP_Height', SPP.LocalSignalToNoise AS 'SPP_LocalSignalToNoise', SPP.Background AS 'SPP_Background', SPP.Width AS 'SPP_Width', SPP.LocalLowestMinimaHeight AS 'SPP_LocalLowestMinimaHeight', SPP.SignalToBackground AS 'SPP_SignalToBackground', SPP.SignalToNoiseGlobal AS 'SPP_SignalToNoiseGlobal', SPP.SignalToNoiseLocalMinima AS 'SPP_SignalToNoiseLocalMinima' ,
            //       SM.Scan AS 'SM_Scan', SM.PeakNumber AS 'SM_PeakNumber', SM.XValue AS 'SM_XValue', SM.Height AS 'SM_Height', SM.LocalSignalToNoise AS 'SM_LocalSignalToNoise', SM.Background AS 'SM_Background', SM.Width AS 'SM_Width', SM.LocalLowestMinimaHeight AS 'SM_LocalLowestMinimaHeight', SM.SignalToBackground AS 'SM_SignalToBackground', SM.SignalToNoiseGlobal AS 'SM_SignalToNoiseGlobal', SM.SignalToNoiseLocalMinima AS 'SM_SignalToNoiseLocalMinima'
            //
            //FROM T_Scans_Precursor_Peaks SPP 
            //    INNER JOIN T_Scan_MonoPeaks SM
            //        ON (SPP.TandemScan = SM.Scan)
            //WHERE SPP.TandemScan= ?
            #endregion

            resultPeaks = new DatabasePeakCentricObjectList();
 //           resultsAttributes = new DatabaseAttributeCentricObjectList();


            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;

                    //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list
                    SQLiteParameter newSqliteParamater = new SQLiteParameter(variable1, scanNumber);

                    getIsotopesCommmnd.CreateParameter();
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    getIsotopesCommmnd.Prepare();

                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();
                            while (databaseReader.Read())
                            {
                                DatabasePeakCentricObject loadedPeakCentric = new DatabasePeakCentricObject();
                                //DatabaseAttributeCentricObject loadedAtributeCentric = new DatabaseAttributeCentricObject();

                                
                                //foreach (DbDataRecord VARIABLE in databaseReader)
                                //{

                                //    loadedMonoIsotopeObject.ScanNum = Convert.ToInt32(VARIABLE.GetValue(0));
                                //    loadedMonoIsotopeObject.PeakNumber = Convert.ToInt32(VARIABLE.GetValue(1));

                                //}

                                //loadedAtributeCentric.AttributeCentricData.PeakID = Convert.ToInt32(databaseReader["TA_PeakID"]);
                                //loadedAtributeCentric.AttributeCentricData.isSignal = Convert.ToBoolean(databaseReader["TA_isSignal"]);
                                //loadedAtributeCentric.AttributeCentricData.isCentroided = Convert.ToBoolean(databaseReader["TA_isCentroided"]);
                                //loadedAtributeCentric.AttributeCentricData.isMonoisotopic = Convert.ToBoolean(databaseReader["TA_isMonoisotopic"]);
                                //loadedAtributeCentric.AttributeCentricData.isIsotope = Convert.ToBoolean(databaseReader["TA_isIsotope"]);
                                //loadedAtributeCentric.AttributeCentricData.isMostAbundant = Convert.ToBoolean(databaseReader["TA_isMostAbundant"]);
                                //loadedAtributeCentric.AttributeCentricData.isCharged = Convert.ToBoolean(databaseReader["TA_isCharged"]);
                                //loadedAtributeCentric.AttributeCentricData.isCorrected = Convert.ToBoolean(databaseReader["TA_isCorrected"]);
                                //loadedAtributeCentric.AttributeCentricData.isPrecursorMass = Convert.ToBoolean(databaseReader["TA_isPrecursorMass"]);
                                loadedPeakCentric.PeakID = Convert.ToInt32(databaseReader["TP_PeakID"]);
                                loadedPeakCentric.ScanID = Convert.ToInt32(databaseReader["TP_ScanID"]);
                                loadedPeakCentric.GroupID = Convert.ToInt32(databaseReader["TP_GroupID"]);
                                loadedPeakCentric.MonoisotopicClusterID = Convert.ToInt32(databaseReader["TP_MonoisotopicClusterID"]);
                                loadedPeakCentric.FeatureClusterID = Convert.ToInt32(databaseReader["TP_FeatureClusterID"]);
                                loadedPeakCentric.Mz = Convert.ToDouble(databaseReader["TP_Mz"]);
                                loadedPeakCentric.ChargeState = Convert.ToInt32(databaseReader["TP_Charge"]);
                                loadedPeakCentric.Height = Convert.ToDouble(databaseReader["TP_Height"]);
                                loadedPeakCentric.Width = Convert.ToDouble(databaseReader["TP_Width"]);
                                loadedPeakCentric.Background = Convert.ToDouble(databaseReader["TP_Background"]);
                                loadedPeakCentric.LocalSignalToNoise = Convert.ToDouble(databaseReader["TP_LocalSignalToNoise"]);
                                loadedPeakCentric.MassMonoisotopic = Convert.ToDouble(databaseReader["TP_MassMonoisotopic"]);
                                loadedPeakCentric.Score = Convert.ToDouble(databaseReader["TP_Score"]);
                                loadedPeakCentric.AmbiguityScore = Convert.ToDouble(databaseReader["TP_AmbiguityScore"]);

                                //loadedPeakCentric.PeakCentricData.PeakID = Convert.ToInt32(databaseReader["TP_PeakID"]);
                                loadedPeakCentric.isSignal = Convert.ToBoolean(databaseReader["TP_isSignal"]);
                                loadedPeakCentric.isCentroided = Convert.ToBoolean(databaseReader["TP_isCentroided"]);
                                loadedPeakCentric.isMonoisotopic = Convert.ToBoolean(databaseReader["TP_isMonoisotopic"]);
                                loadedPeakCentric.isIsotope = Convert.ToBoolean(databaseReader["TP_isIsotope"]);
                                loadedPeakCentric.isMostAbundant = Convert.ToBoolean(databaseReader["TP_isMostAbundant"]);
                                loadedPeakCentric.isCharged = Convert.ToBoolean(databaseReader["TP_isCharged"]);
                                loadedPeakCentric.isCorrected = Convert.ToBoolean(databaseReader["TP_isCorrected"]);
                                //loadedPeakCentric.PeakCentricData.isPrecursorMass = Convert.ToBoolean(databaseReader["TP_isPrecursorMass"]);
                                

                                //resultsAttributes.DatabaseTransferObjects.Add(loadedAtributeCentric);
                                resultPeaks.DatabaseTransferObjects.Add(loadedPeakCentric);
                            }

                            //results.PeakNumber = results.TandemPeakList.Count;

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();

                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }

            //return results;
            //return didThisWork;
        }




        public void SK2_Final_SelectSimplePeaks(string fileName, string tablenamePeaks, string tablenameScans, int scanNumber, DatabaseTransferObject sampleObjectPeakCentric, DatabaseTransferObject sampleObjectScanCentric, out DatabasePeakCentricLiteObjectList resultPeaks, bool isMono)
        {
            string aliasTable1Peaks = "TP";//tableNamepeaks
            string aliasTable2Scans = "TS";//tableNamepeaks

            DatabasePeakCentricLiteObject currentSampleObjectPeaks = (DatabasePeakCentricLiteObject)sampleObjectPeakCentric;
            List<string> fieldNamePeaks = new List<string>();
            foreach (string column in currentSampleObjectPeaks.Columns)
            {
                fieldNamePeaks.Add(aliasTable1Peaks + "." + column + " AS '" + aliasTable1Peaks + "_" + column + "'");
            }

            DatabaseScanCentricObject currentSampleObjectScans = (DatabaseScanCentricObject)sampleObjectScanCentric;
            List<string> fieldNameScans = new List<string>();
            foreach (string column in currentSampleObjectScans.Columns)
            {
                fieldNameScans.Add(aliasTable2Scans + "." + column + " AS '" + aliasTable2Scans + "_" + column + "'");
            }

            bool didThisWork = false;

           

            //set up connection string to database
            string connectionString = "Data Source=" + fileName;

            string variable1 = aliasTable2Scans + ".ScanNumLc";//column matches to input scanNumber. 


            //Set up SQLite statement

            

            string commandTextStringGetCount;
            
            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNamePeaks) + " ";
            commandTextStringGetCount += "FROM " + tablenamePeaks + " " + aliasTable1Peaks + " ";//where alias is connected
            commandTextStringGetCount += "WHERE " + aliasTable1Peaks + "." + currentSampleObjectPeaks.Columns[1] + " IN ";//TP.ScanID
            commandTextStringGetCount += "(";
            commandTextStringGetCount += "SELECT ";
            commandTextStringGetCount += fieldNameScans[0] + " ";//TS.ScanID AS 'TS_ScanID'
            commandTextStringGetCount += "FROM " + tablenameScans + " " + aliasTable2Scans + " ";//where alias is connected
            commandTextStringGetCount += "WHERE " + variable1 + "= ? ";
            commandTextStringGetCount += ")";
            
            
            //if (isMono)
            //{
            //    //                commandTextStringGetCount += "AND " + aliasTableAtributes + ".isMonoisotopic= 1 ";
            //    commandTextStringGetCount += "AND " + aliasTable1Peaks + ".isMonoisotopic= 1 ";
            //}

            #region querry as Text
            //...new
            //SELECT TP.PeakID AS 'TP_PeakID', TP.ScanID AS 'TP_ScanID', TP.Mz AS 'TP_Mz', TP.Height AS 'TP_Height', TP.Width AS 'TP_Width' 
            //FROM T_Peak_Centric TP 
            //WHERE TP.ScanID  IN
            //(
            //SELECT TS.ScanID AS 'TS_ScanID'
            //FROM T_Scan_Centric TS 
            //WHERE TS.ScanNumLc  = 135
            //)

            //...old
            //SELECT SPP.TandemScan AS 'SPP_TandemScan', SPP.PrecursorScan AS 'SPP_PrecursorScan', SPP.PeakNumber AS 'SPP_PeakNumber', SPP.XValue AS 'SPP_XValue', SPP.XValueRaw AS 'SPP_XValueRaw', SPP.Height AS 'SPP_Height', SPP.LocalSignalToNoise AS 'SPP_LocalSignalToNoise', SPP.Background AS 'SPP_Background', SPP.Width AS 'SPP_Width', SPP.LocalLowestMinimaHeight AS 'SPP_LocalLowestMinimaHeight', SPP.SignalToBackground AS 'SPP_SignalToBackground', SPP.SignalToNoiseGlobal AS 'SPP_SignalToNoiseGlobal', SPP.SignalToNoiseLocalMinima AS 'SPP_SignalToNoiseLocalMinima' ,
            //       SM.Scan AS 'SM_Scan', SM.PeakNumber AS 'SM_PeakNumber', SM.XValue AS 'SM_XValue', SM.Height AS 'SM_Height', SM.LocalSignalToNoise AS 'SM_LocalSignalToNoise', SM.Background AS 'SM_Background', SM.Width AS 'SM_Width', SM.LocalLowestMinimaHeight AS 'SM_LocalLowestMinimaHeight', SM.SignalToBackground AS 'SM_SignalToBackground', SM.SignalToNoiseGlobal AS 'SM_SignalToNoiseGlobal', SM.SignalToNoiseLocalMinima AS 'SM_SignalToNoiseLocalMinima'
            //
            //FROM T_Scans_Precursor_Peaks SPP 
            //    INNER JOIN T_Scan_MonoPeaks SM
            //        ON (SPP.TandemScan = SM.Scan)
            //WHERE SPP.TandemScan= ? 
            #endregion

            resultPeaks = new DatabasePeakCentricLiteObjectList();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;

                    //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list
                    SQLiteParameter newSqliteParamater = new SQLiteParameter(variable1, scanNumber);

                    getIsotopesCommmnd.CreateParameter();
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    getIsotopesCommmnd.Prepare();

                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();
                            while (databaseReader.Read())
                            {
                                DatabasePeakCentricLiteObject loadedPeakCentric = new DatabasePeakCentricLiteObject();
                               
                                loadedPeakCentric.PeakData.ScanNumber = Convert.ToInt32(databaseReader["TP_ScanID"]);
                                loadedPeakCentric.PeakData.XValue = Convert.ToDouble(databaseReader["TP_Mz"]);
                                loadedPeakCentric.PeakData.Height = Convert.ToDouble(databaseReader["TP_Height"]);
                                loadedPeakCentric.PeakData.Width = (float)Convert.ToDouble(databaseReader["TP_Width"]);
                                resultPeaks.DatabaseTransferObjects.Add(loadedPeakCentric);
                            }

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                        //connection.Dispose();
                    }
                }
                connection.Close();
                
                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }

            //return results;
            //return didThisWork;
        }





        public void SK2_Final_SelectSimplePeaksCopy(string fileName, string tablenamePeaks, string tablenameScans, int scanNumber, DatabaseTransferObject sampleObjectPeakCentric, DatabaseTransferObject sampleObjectScanCentric, out DatabasePeakCentricLiteObjectList resultPeaks, bool isMono)
        {
            string aliasTable1Peaks = "TP";//tableNamepeaks
            string aliasTable2Scans = "TS";//tableNamepeaks

            DatabasePeakCentricLiteObject currentSampleObjectPeaks = (DatabasePeakCentricLiteObject)sampleObjectPeakCentric;
            List<string> fieldNamePeaks = new List<string>();
            foreach (string column in currentSampleObjectPeaks.Columns)
            {
                fieldNamePeaks.Add(aliasTable1Peaks + "." + column + " AS '" + aliasTable1Peaks + "_" + column + "'");
            }

            DatabaseScanCentricObject currentSampleObjectScans = (DatabaseScanCentricObject)sampleObjectScanCentric;
            List<string> fieldNameScans = new List<string>();
            foreach (string column in currentSampleObjectScans.Columns)
            {
                fieldNameScans.Add(aliasTable2Scans + "." + column + " AS '" + aliasTable2Scans + "_" + column + "'");
            }

            bool didThisWork = false;



            //set up connection string to database
            string connectionString = "Data Source=" + fileName;

            string variable1 = aliasTable1Peaks + ".ScanID";//column matches to input scanNumber. 


            //Set up SQLite statement
            string commandTextStringGetCount;
            //commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            //commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNamePeaks) + " ";
            //commandTextStringGetCount += "FROM " + tablenamePeaks + " " + aliasTable1Peaks + " ";//where alias is connected
            //commandTextStringGetCount += "WHERE " + variable1 + "= ? ";

            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNamePeaks) + " ";
            commandTextStringGetCount += "FROM " + tablenamePeaks + " " + aliasTable1Peaks + " ";//where alias is connected
            commandTextStringGetCount += "WHERE " + variable1 + "= ? ";
            if (isMono)
            {
                //                commandTextStringGetCount += "AND " + aliasTableAtributes + ".isMonoisotopic= 1 ";
                commandTextStringGetCount += "AND " + aliasTable1Peaks + ".isMonoisotopic= 1 ";
            }

            #region querry as Text
            //SELECT SPP.TandemScan AS 'SPP_TandemScan', SPP.PrecursorScan AS 'SPP_PrecursorScan', SPP.PeakNumber AS 'SPP_PeakNumber', SPP.XValue AS 'SPP_XValue', SPP.XValueRaw AS 'SPP_XValueRaw', SPP.Height AS 'SPP_Height', SPP.LocalSignalToNoise AS 'SPP_LocalSignalToNoise', SPP.Background AS 'SPP_Background', SPP.Width AS 'SPP_Width', SPP.LocalLowestMinimaHeight AS 'SPP_LocalLowestMinimaHeight', SPP.SignalToBackground AS 'SPP_SignalToBackground', SPP.SignalToNoiseGlobal AS 'SPP_SignalToNoiseGlobal', SPP.SignalToNoiseLocalMinima AS 'SPP_SignalToNoiseLocalMinima' ,
            //       SM.Scan AS 'SM_Scan', SM.PeakNumber AS 'SM_PeakNumber', SM.XValue AS 'SM_XValue', SM.Height AS 'SM_Height', SM.LocalSignalToNoise AS 'SM_LocalSignalToNoise', SM.Background AS 'SM_Background', SM.Width AS 'SM_Width', SM.LocalLowestMinimaHeight AS 'SM_LocalLowestMinimaHeight', SM.SignalToBackground AS 'SM_SignalToBackground', SM.SignalToNoiseGlobal AS 'SM_SignalToNoiseGlobal', SM.SignalToNoiseLocalMinima AS 'SM_SignalToNoiseLocalMinima'
            //
            //FROM T_Scans_Precursor_Peaks SPP 
            //    INNER JOIN T_Scan_MonoPeaks SM
            //        ON (SPP.TandemScan = SM.Scan)
            //WHERE SPP.TandemScan= ?
            #endregion

            resultPeaks = new DatabasePeakCentricLiteObjectList();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;

                    //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list
                    SQLiteParameter newSqliteParamater = new SQLiteParameter(variable1, scanNumber);

                    getIsotopesCommmnd.CreateParameter();
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    getIsotopesCommmnd.Prepare();

                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();
                            while (databaseReader.Read())
                            {
                                DatabasePeakCentricLiteObject loadedPeakCentric = new DatabasePeakCentricLiteObject();

                                loadedPeakCentric.PeakData.ScanNumber = Convert.ToInt32(databaseReader["TP_ScanID"]);
                                loadedPeakCentric.PeakData.XValue = Convert.ToDouble(databaseReader["TP_Mz"]);
                                loadedPeakCentric.PeakData.Height = Convert.ToDouble(databaseReader["TP_Height"]);
                                //loadedPeakCentric.PeakData.Width = (float)Convert.ToDouble(databaseReader["TP_Width"]));
                                resultPeaks.DatabaseTransferObjects.Add(loadedPeakCentric);
                            }

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                        //connection.Dispose();
                    }
                }
                connection.Close();

                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }

            //return results;
            //return didThisWork;
        }



        //public void SK2_Final_SelectPeaksWithAttributes(string fileName, string tablenamePeaks, string tableNameAttribute, int scanNumber, DatabaseTransferObject sampleObjectAttributeCentric, DatabaseTransferObject sampleObjectPeakCentric, out DatabasePeakCentricObjectList resultPeaks, out DatabaseAttributeCentricObjectList resultsAttributes, bool isMono)
        public void SK2_Final_SelectPeak(string fileName, string tablenamePeaks, int peakNumber, DatabaseTransferObject sampleObjectPeakCentric, out DatabasePeakCentricObjectList resultPeaks, bool isMono)
        {
            string aliasTable1Peaks = "TP";//tableNamepeaks
            //            string aliasTableAtributes = "TA";//tableNameAtributes

            //converts column names to field names so we can extract the information back out
            //DatabaseAttributeCentricObject currentSampleObjectAttribute = (DatabaseAttributeCentricObject)sampleObjectAttributeCentric;
            //List<string> fieldNameAttributes = new List<string>();
            //foreach (string column in currentSampleObjectAttribute.Columns)
            //{
            //    //fieldNamePrecursor.Add(variableTable1 + "." + column);
            //    fieldNameAttributes.Add(aliasTableAtributes + "." + column + " AS '" + aliasTableAtributes + "_" + column + "'");
            //}

            DatabasePeakCentricObject currentSampleObjectPeaks = (DatabasePeakCentricObject)sampleObjectPeakCentric;
            List<string> fieldNamePeaks = new List<string>();
            foreach (string column in currentSampleObjectPeaks.Columns)
            {
                //fieldNamePeaks.Add(variableTable2 + "." + column);
                fieldNamePeaks.Add(aliasTable1Peaks + "." + column + " AS '" + aliasTable1Peaks + "_" + column + "'");

            }

            //this is where the results will end up

            bool didThisWork = false;

            //set up connection string to database
            string connectionString = "Data Source=" + fileName;

            //string variable1 = "T_Scan_MonoPeaks.Scan";//column matches to input scanNumber
            //string variable1 = aliasTable2 + ".Scan";//column matches to input scanNumber  //DOES NOT WORK.  leads to over run
            string variable1 = aliasTable1Peaks + ".PeakID";//column matches to input scanNumber. 


            //Set up SQLite statement
            string commandTextStringGetCount;
            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            //           commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNameAttributes) + "";
            //           commandTextStringGetCount += ", ";
            commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNamePeaks) + " ";
            commandTextStringGetCount += "FROM " + tablenamePeaks + " " + aliasTable1Peaks + " ";//where alias is connected
            //           commandTextStringGetCount += "INNER JOIN " + tableNameAttribute + " " + aliasTableAtributes + " ";
            //           commandTextStringGetCount += "ON (" + aliasTable1Peaks + ".PeakID = " + aliasTableAtributes + ".PeakID) ";
            commandTextStringGetCount += "WHERE " + variable1 + "= ? ";
            if (isMono)
            {
                //                commandTextStringGetCount += "AND " + aliasTableAtributes + ".isMonoisotopic= 1 ";
                commandTextStringGetCount += "AND " + aliasTable1Peaks + ".isMonoisotopic= 1 ";
            }

            #region querry as Text
            //SELECT SPP.TandemScan AS 'SPP_TandemScan', SPP.PrecursorScan AS 'SPP_PrecursorScan', SPP.PeakNumber AS 'SPP_PeakNumber', SPP.XValue AS 'SPP_XValue', SPP.XValueRaw AS 'SPP_XValueRaw', SPP.Height AS 'SPP_Height', SPP.LocalSignalToNoise AS 'SPP_LocalSignalToNoise', SPP.Background AS 'SPP_Background', SPP.Width AS 'SPP_Width', SPP.LocalLowestMinimaHeight AS 'SPP_LocalLowestMinimaHeight', SPP.SignalToBackground AS 'SPP_SignalToBackground', SPP.SignalToNoiseGlobal AS 'SPP_SignalToNoiseGlobal', SPP.SignalToNoiseLocalMinima AS 'SPP_SignalToNoiseLocalMinima' ,
            //       SM.Scan AS 'SM_Scan', SM.PeakNumber AS 'SM_PeakNumber', SM.XValue AS 'SM_XValue', SM.Height AS 'SM_Height', SM.LocalSignalToNoise AS 'SM_LocalSignalToNoise', SM.Background AS 'SM_Background', SM.Width AS 'SM_Width', SM.LocalLowestMinimaHeight AS 'SM_LocalLowestMinimaHeight', SM.SignalToBackground AS 'SM_SignalToBackground', SM.SignalToNoiseGlobal AS 'SM_SignalToNoiseGlobal', SM.SignalToNoiseLocalMinima AS 'SM_SignalToNoiseLocalMinima'
            //
            //FROM T_Scans_Precursor_Peaks SPP 
            //    INNER JOIN T_Scan_MonoPeaks SM
            //        ON (SPP.TandemScan = SM.Scan)
            //WHERE SPP.TandemScan= ?
            #endregion

            resultPeaks = new DatabasePeakCentricObjectList();
            //           resultsAttributes = new DatabaseAttributeCentricObjectList();


            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;

                    //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list
                    SQLiteParameter newSqliteParamater = new SQLiteParameter(variable1, peakNumber);

                    getIsotopesCommmnd.CreateParameter();
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    getIsotopesCommmnd.Prepare();

                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();
                            while (databaseReader.Read())
                            {
                                DatabasePeakCentricObject loadedPeakCentric = new DatabasePeakCentricObject();
                                //DatabaseAttributeCentricObject loadedAtributeCentric = new DatabaseAttributeCentricObject();


                                //foreach (DbDataRecord VARIABLE in databaseReader)
                                //{

                                //    loadedMonoIsotopeObject.ScanNum = Convert.ToInt32(VARIABLE.GetValue(0));
                                //    loadedMonoIsotopeObject.PeakNumber = Convert.ToInt32(VARIABLE.GetValue(1));

                                //}

                                //loadedAtributeCentric.AttributeCentricData.PeakID = Convert.ToInt32(databaseReader["TA_PeakID"]);
                                //loadedAtributeCentric.AttributeCentricData.isSignal = Convert.ToBoolean(databaseReader["TA_isSignal"]);
                                //loadedAtributeCentric.AttributeCentricData.isCentroided = Convert.ToBoolean(databaseReader["TA_isCentroided"]);
                                //loadedAtributeCentric.AttributeCentricData.isMonoisotopic = Convert.ToBoolean(databaseReader["TA_isMonoisotopic"]);
                                //loadedAtributeCentric.AttributeCentricData.isIsotope = Convert.ToBoolean(databaseReader["TA_isIsotope"]);
                                //loadedAtributeCentric.AttributeCentricData.isMostAbundant = Convert.ToBoolean(databaseReader["TA_isMostAbundant"]);
                                //loadedAtributeCentric.AttributeCentricData.isCharged = Convert.ToBoolean(databaseReader["TA_isCharged"]);
                                //loadedAtributeCentric.AttributeCentricData.isCorrected = Convert.ToBoolean(databaseReader["TA_isCorrected"]);
                                //loadedAtributeCentric.AttributeCentricData.isPrecursorMass = Convert.ToBoolean(databaseReader["TA_isPrecursorMass"]);
                                loadedPeakCentric.PeakID = Convert.ToInt32(databaseReader["TP_PeakID"]);
                                loadedPeakCentric.ScanID = Convert.ToInt32(databaseReader["TP_ScanID"]);
                                loadedPeakCentric.GroupID = Convert.ToInt32(databaseReader["TP_GroupID"]);
                                loadedPeakCentric.MonoisotopicClusterID = Convert.ToInt32(databaseReader["TP_MonoisotopicClusterID"]);
                                loadedPeakCentric.FeatureClusterID = Convert.ToInt32(databaseReader["TP_FeatureClusterID"]);
                                loadedPeakCentric.Mz = Convert.ToDouble(databaseReader["TP_Mz"]);
                                loadedPeakCentric.ChargeState = Convert.ToInt32(databaseReader["TP_Charge"]);
                                loadedPeakCentric.Height = Convert.ToDouble(databaseReader["TP_Height"]);
                                loadedPeakCentric.Width = Convert.ToDouble(databaseReader["TP_Width"]);
                                loadedPeakCentric.Background = Convert.ToDouble(databaseReader["TP_Background"]);
                                loadedPeakCentric.LocalSignalToNoise = Convert.ToDouble(databaseReader["TP_LocalSignalToNoise"]);
                                loadedPeakCentric.MassMonoisotopic = Convert.ToDouble(databaseReader["TP_MassMonoisotopic"]);
                                loadedPeakCentric.Score = Convert.ToDouble(databaseReader["TP_Score"]);
                                loadedPeakCentric.AmbiguityScore = Convert.ToDouble(databaseReader["TP_AmbiguityScore"]);

                                //loadedPeakCentric.PeakCentricData.PeakID = Convert.ToInt32(databaseReader["TP_PeakID"]);
                                loadedPeakCentric.isSignal = Convert.ToBoolean(databaseReader["TP_isSignal"]);
                                loadedPeakCentric.isCentroided = Convert.ToBoolean(databaseReader["TP_isCentroided"]);
                                loadedPeakCentric.isMonoisotopic = Convert.ToBoolean(databaseReader["TP_isMonoisotopic"]);
                                loadedPeakCentric.isIsotope = Convert.ToBoolean(databaseReader["TP_isIsotope"]);
                                loadedPeakCentric.isMostAbundant = Convert.ToBoolean(databaseReader["TP_isMostAbundant"]);
                                loadedPeakCentric.isCharged = Convert.ToBoolean(databaseReader["TP_isCharged"]);
                                loadedPeakCentric.isCorrected = Convert.ToBoolean(databaseReader["TP_isCorrected"]);
                                //loadedPeakCentric.PeakCentricData.isPrecursorMass = Convert.ToBoolean(databaseReader["TP_isPrecursorMass"]);


                                //resultsAttributes.DatabaseTransferObjects.Add(loadedAtributeCentric);
                                resultPeaks.DatabaseTransferObjects.Add(loadedPeakCentric);
                            }

                            //results.PeakNumber = results.TandemPeakList.Count;

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();

                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }

            //return results;
            //return didThisWork;
        }

        //public void SK2_PrecursorPeakWithAttributes(string fileName, string tablenamePeaks, string tableNameAttribute, string tableNameScan, int scanNumber, DatabaseTransferObject sampleObjectAttributeCentric, DatabaseTransferObject sampleObjectPeakCentric, DatabaseTransferObject sampleObjectScanCentric, out List<DatabasePeakCentricObject> resultPeaks, out DatabaseAttributeCentricObjectList resultsAttributes, out DatabaseScanCentricObjectList resultsScans)
        public void SK2_PrecursorPeakWithAttributes(string fileName, string tablenamePeaks, string tableNameScan, int scanNumber, DatabaseTransferObject sampleObjectPeakCentric, DatabaseTransferObject sampleObjectScanCentric, out List<DatabasePeakCentricObject> resultPeaks, out DatabaseScanCentricObjectList resultsScans)
        {
            string aliasTablePeaks = "TP";//tableNamepeaks
 //           string aliasTableAttributes = "TA";//tableNameAtributes
            string aliasTableScans = "TS";//tableNameScans

            //converts column names to field names so we can extract the information back out
//            DatabaseAttributeCentricObject currentSampleObjectAttribute = (DatabaseAttributeCentricObject)sampleObjectAttributeCentric;
//            List<string> fieldNameAttributes = new List<string>();
//            foreach (string column in currentSampleObjectAttribute.Columns)
//            {
//                //fieldNamePrecursor.Add(variableTable1 + "." + column);
//                fieldNameAttributes.Add(aliasTableAttributes + "." + column + " AS '" + aliasTableAttributes + "_" + column + "'");
//            }

            DatabasePeakCentricObject currentSampleObjectPeaks = (DatabasePeakCentricObject)sampleObjectPeakCentric;
            List<string> fieldNamePeaks = new List<string>();
            foreach (string column in currentSampleObjectPeaks.Columns)
            {
                //fieldNamePeaks.Add(variableTable2 + "." + column);
                fieldNamePeaks.Add(aliasTablePeaks + "." + column + " AS '" + aliasTablePeaks + "_" + column + "'");

            }

            DatabaseScanCentricObject currentSampleObjectScans = (DatabaseScanCentricObject)sampleObjectScanCentric;
            List<string> fieldNameScans = new List<string>();
            foreach (string column in currentSampleObjectScans.Columns)
            {
                //fieldNamePeaks.Add(variableTable2 + "." + column);
                fieldNamePeaks.Add(aliasTableScans + "." + column + " AS '" + aliasTableScans + "_" + column + "'");

            }

            //this is where the results will end up
           
            bool didThisWork = false;

            //set up connection string to database
            string connectionString = "Data Source=" + fileName;

            //string variable1 = "T_Scan_MonoPeaks.Scan";//column matches to input scanNumber
            //string variable1 = aliasTable2 + ".Scan";//column matches to input scanNumber  //DOES NOT WORK.  leads to over run
            string variable1 = aliasTablePeaks + ".ScanID";//column matches to input scanNumber. 
            string variable2 = aliasTableScans + ".ScanID";//column matches to input scanNumber. 


            //Set up SQLite statement
            //string commandTextStringGetCount;
            //commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            //commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNameAttributes) + "";
            //commandTextStringGetCount += ", ";
            //commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNamePeaks) + " ";
            //commandTextStringGetCount += "FROM " + tablenamePeaks + " " + aliasTablePeaks + " ";//where alias is connected
            //commandTextStringGetCount += "INNER JOIN " + tableNameAttribute + " " + aliasTableAttributes + " ";
            //commandTextStringGetCount += "ON (" + aliasTablePeaks + ".PeakID = " + aliasTableAttributes + ".PeakID) ";
            //commandTextStringGetCount += "INNER JOIN " + tableNameScan + " " + aliasTableScans + " ";
            //commandTextStringGetCount += "ON (" + aliasTablePeaks + ".ScanID = " + aliasTableScans + ".ScanID) ";
            //commandTextStringGetCount += "WHERE " + variable1 + "= ? ";
            //commandTextStringGetCount += "AND " + variable2 + "= ? ";

            //string commandTextStringGetCount;
            //commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            //commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNameAttributes) + "";
            //commandTextStringGetCount += ", ";
            //commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNamePeaks) + " ";
            //commandTextStringGetCount += "FROM " + tablenamePeaks + " " + aliasTablePeaks + " ";//where alias is connected
            //commandTextStringGetCount += "INNER JOIN " + tableNameAttribute + " " + aliasTableAttributes + " ";
            //commandTextStringGetCount += "USING " + "(PeakID) ";
            //commandTextStringGetCount += "INNER JOIN " + tableNameScan + " " + aliasTableScans + " ";
            //commandTextStringGetCount += "USING " + "(PeakID)";
            //commandTextStringGetCount += "WHERE " + variable1 + "= ? ";
            ////commandTextStringGetCount += "AND " + variable2 + "= ? ";

            string commandTextStringGetCount;
            //commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            //commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNameAttributes) + "";
            //commandTextStringGetCount += ", ";
            //commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNamePeaks) + " ";
            //commandTextStringGetCount += "FROM " + tablenamePeaks + " " + aliasTablePeaks + " ";//where alias is connected
            //commandTextStringGetCount += "INNER JOIN " + tableNameScan + " " + aliasTableScans + " ";
            ////commandTextStringGetCount += "USING " + "(ScanID) ";
            //commandTextStringGetCount += "ON (" + aliasTablePeaks + ".ScanID = " + aliasTableScans + ".ScanID) ";
            //commandTextStringGetCount += "INNER JOIN " + tableNameAttribute + " " + aliasTableAttributes + " ";
            ////commandTextStringGetCount += "USING " + "(PeakID) ";
            //commandTextStringGetCount += "ON (" + aliasTablePeaks + ".PeakID = " + aliasTableAttributes + ".PeakID) ";
            //commandTextStringGetCount += "WHERE " + variable2 + "= ? ";
            //commandTextStringGetCount += "AND " + aliasTablePeaks + ".PeakID = " + aliasTableAttributes + ".PeakID";

            //commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            //commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNameAttributes) + "";
            //commandTextStringGetCount += ", ";
            //commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNamePeaks) + " ";
            //commandTextStringGetCount += "FROM " + tablenamePeaks + " " + aliasTablePeaks + " ";//where alias is connected
            //commandTextStringGetCount += "INNER JOIN (";
            //commandTextStringGetCount += "SELECT " + "PeakID ";
            //commandTextStringGetCount += "FROM " + tableNameScan + " " + aliasTableScans + " ";//where alias is connected
            //commandTextStringGetCount += "WHERE " + aliasTableScans + ".MSLevel = 2 AND " + variable2 + "= ?) ParentIonPeaks ";
            //commandTextStringGetCount += "ON (" + aliasTablePeaks + ".PeakID = " + "ParentIonPeaks.PeakID) ";
            //commandTextStringGetCount += "INNER JOIN " + tableNameAttribute + " " + aliasTableAttributes + " ";
            ////commandTextStringGetCount += "USING " + "(PeakID) ";
            //commandTextStringGetCount += "ON (" + aliasTablePeaks + ".PeakID = " + aliasTableAttributes + ".PeakID); ";

            //commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            //commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNameAttributes) + "";
            //commandTextStringGetCount += ", ";
            //commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNamePeaks) + " ";
            //commandTextStringGetCount += "FROM " + tablenamePeaks + " " + aliasTablePeaks + " ";//where alias is connected
            //commandTextStringGetCount += "INNER JOIN (";
            //commandTextStringGetCount += "SELECT " + "PeakID ";
            //commandTextStringGetCount += "FROM " + tableNameScan + " " + aliasTableScans + " ";//where alias is connected
            //commandTextStringGetCount += "WHERE " + aliasTableScans + ".MSLevel = 2 AND " + variable2 + "= ?) ParentIonPeaks ";
            //commandTextStringGetCount += "ON (" + aliasTablePeaks + ".PeakID = " + "ParentIonPeaks.PeakID) ";
            //commandTextStringGetCount += "INNER JOIN " + tableNameAttribute + " " + aliasTableAttributes + " ";
            //commandTextStringGetCount += "ON (" + aliasTablePeaks + ".PeakID = " + aliasTableAttributes + ".PeakID); ";

            commandTextStringGetCount = "SELECT " + "PeakID ";
            commandTextStringGetCount += "FROM " + tableNameScan + " " + aliasTableScans + " ";//where alias is connected
            //commandTextStringGetCount += "WHERE " + aliasTableScans + ".MSLevel = 2 AND " + variable2 + "= ?";// " ParentIonPeaks; ";

            commandTextStringGetCount += "WHERE " + aliasTableScans + ".PeakID = 2 AND " + variable2 + "= ?";// " ParentIonPeaks; ";

            #region querry as Text
            //SELECT SPP.TandemScan AS 'SPP_TandemScan', SPP.PrecursorScan AS 'SPP_PrecursorScan', SPP.PeakNumber AS 'SPP_PeakNumber', SPP.XValue AS 'SPP_XValue', SPP.XValueRaw AS 'SPP_XValueRaw', SPP.Height AS 'SPP_Height', SPP.LocalSignalToNoise AS 'SPP_LocalSignalToNoise', SPP.Background AS 'SPP_Background', SPP.Width AS 'SPP_Width', SPP.LocalLowestMinimaHeight AS 'SPP_LocalLowestMinimaHeight', SPP.SignalToBackground AS 'SPP_SignalToBackground', SPP.SignalToNoiseGlobal AS 'SPP_SignalToNoiseGlobal', SPP.SignalToNoiseLocalMinima AS 'SPP_SignalToNoiseLocalMinima' ,
            //       SM.Scan AS 'SM_Scan', SM.PeakNumber AS 'SM_PeakNumber', SM.XValue AS 'SM_XValue', SM.Height AS 'SM_Height', SM.LocalSignalToNoise AS 'SM_LocalSignalToNoise', SM.Background AS 'SM_Background', SM.Width AS 'SM_Width', SM.LocalLowestMinimaHeight AS 'SM_LocalLowestMinimaHeight', SM.SignalToBackground AS 'SM_SignalToBackground', SM.SignalToNoiseGlobal AS 'SM_SignalToNoiseGlobal', SM.SignalToNoiseLocalMinima AS 'SM_SignalToNoiseLocalMinima'
            //
            //FROM T_Scans_Precursor_Peaks SPP 
            //    INNER JOIN T_Scan_MonoPeaks SM
            //        ON (SPP.TandemScan = SM.Scan)
            //WHERE SPP.TandemScan= ?


//            SELECT T_Peak_Centric.*
//              FROM T_Peak_Centric
//              INNER JOIN ( SELECT PeakID
//                  FROM T_Scan_Centric S
//                  WHERE S.MSLevel = 2 AND S.ScanID = ?) ParentIonPeaks
//                  ON T_Peak_Centric.PeakID = ParentIonPeaks.PeakID
//              INNER JOIN T_Attribute_Centric
//                  ON T_Peak_Centric.PeakID = T_Attribute_Centric.PeakID

            //stored procedures?
            //embedded subquery
            //nesting sub queries
            //http://www8.silversand.net/techdoc/teachsql/ch07.htm

            #endregion

            //resultPeaks = new DatabasePeakCentricObjectList();
            resultPeaks = new List<DatabasePeakCentricObject>();
            //resultsAttributes = new DatabaseAttributeCentricObjectList();
            resultsScans = new DatabaseScanCentricObjectList();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;

                    //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list
                    //SQLiteParameter newSqliteParamater = new SQLiteParameter(variable1, scanNumber);

                    //getIsotopesCommmnd.CreateParameter();
                    //getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    //getIsotopesCommmnd.Prepare();

                    SQLiteParameter newSqliteParamater2 = new SQLiteParameter(variable2, scanNumber);

                    getIsotopesCommmnd.CreateParameter();
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamater2);

                    getIsotopesCommmnd.Prepare();
                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();
                            while (databaseReader.Read())
                            {
                                DatabasePeakCentricObject loadedPeakCentric = new DatabasePeakCentricObject();
                                //DatabaseAttributeCentricObject loadedAtributeCentric = new DatabaseAttributeCentricObject();
                                DatabaseScanCentricObject loadedScanCentric = new DatabaseScanCentricObject();

                                //foreach (DbDataRecord VARIABLE in databaseReader)
                                //{

                                //    loadedMonoIsotopeObject.ScanNum = Convert.ToInt32(VARIABLE.GetValue(0));
                                //    loadedMonoIsotopeObject.PeakNumber = Convert.ToInt32(VARIABLE.GetValue(1));

                                //}

                                //loadedAtributeCentric.AttributeCentricData.PeakID = Convert.ToInt32(databaseReader["TA_PeakID"]);
                                //loadedAtributeCentric.AttributeCentricData.isSignal = Convert.ToBoolean(databaseReader["TA_isSignal"]);
                                //loadedAtributeCentric.AttributeCentricData.isCentroided = Convert.ToBoolean(databaseReader["TA_isCentroided"]);
                                //loadedAtributeCentric.AttributeCentricData.isMonoisotopic = Convert.ToBoolean(databaseReader["TA_isMonoisotopic"]);
                                //loadedAtributeCentric.AttributeCentricData.isIsotope = Convert.ToBoolean(databaseReader["TA_isIsotope"]);
                                //loadedAtributeCentric.AttributeCentricData.isMostAbundant = Convert.ToBoolean(databaseReader["TA_isMostAbundant"]);
                                //loadedAtributeCentric.AttributeCentricData.isCharged = Convert.ToBoolean(databaseReader["TA_isCharged"]);
                                //loadedAtributeCentric.AttributeCentricData.isCorrected = Convert.ToBoolean(databaseReader["TA_isCorrected"]);
                                //loadedAtributeCentric.AttributeCentricData.isPrecursorMass = Convert.ToBoolean(databaseReader["TA_isPrecursorMass"]);
                                //loadedPeakCentric.PeakCentricData.PeakID = Convert.ToInt32(databaseReader["TP_PeakID"]);
                                loadedPeakCentric.ScanID = Convert.ToInt32(databaseReader["TP_ScanID"]);
                                loadedPeakCentric.GroupID = Convert.ToInt32(databaseReader["TP_GroupID"]);
                                loadedPeakCentric.MonoisotopicClusterID = Convert.ToInt32(databaseReader["TP_MonoisotopicClusterID"]);
                                loadedPeakCentric.FeatureClusterID = Convert.ToInt32(databaseReader["TP_FeatureClusterID"]);
                                loadedPeakCentric.Mz = Convert.ToDouble(databaseReader["TP_Mz"]);
                                loadedPeakCentric.ChargeState = Convert.ToInt32(databaseReader["TP_Charge"]);
                                loadedPeakCentric.Height = Convert.ToDouble(databaseReader["TP_Height"]);
                                loadedPeakCentric.Width = Convert.ToDouble(databaseReader["TP_Width"]);
                                loadedPeakCentric.Background = Convert.ToDouble(databaseReader["TP_Background"]);
                                loadedPeakCentric.LocalSignalToNoise = Convert.ToDouble(databaseReader["TP_LocalSignalToNoise"]);
                                loadedPeakCentric.MassMonoisotopic = Convert.ToDouble(databaseReader["TP_MassMonoisotopic"]);
                                loadedPeakCentric.Score = Convert.ToDouble(databaseReader["TP_Score"]);
                                loadedPeakCentric.AmbiguityScore = Convert.ToDouble(databaseReader["TP_AmbiguityScore"]);

                                loadedPeakCentric.isSignal = Convert.ToBoolean(databaseReader["TP_isSignal"]);
                                loadedPeakCentric.isCentroided = Convert.ToBoolean(databaseReader["TP_isCentroided"]);
                                loadedPeakCentric.isMonoisotopic = Convert.ToBoolean(databaseReader["TP_isMonoisotopic"]);
                                loadedPeakCentric.isIsotope = Convert.ToBoolean(databaseReader["TP_isIsotope"]);
                                loadedPeakCentric.isMostAbundant = Convert.ToBoolean(databaseReader["TP_isMostAbundant"]);
                                loadedPeakCentric.isCharged = Convert.ToBoolean(databaseReader["TP_isCharged"]);
                                loadedPeakCentric.isCorrected = Convert.ToBoolean(databaseReader["TP_isCorrected"]);
                                //loadedPeakCentric.PeakCentricData.isPrecursorMass = Convert.ToBoolean(databaseReader["TP_isPrecursorMass"]);
                                
                                
                                loadedScanCentric.ScanCentricData.ScanID = Convert.ToInt32(databaseReader["TS_ScanID"]);
                                loadedScanCentric.ScanCentricData.PeakID = Convert.ToInt32(databaseReader["TS_PeakID"]);

                                //fragment info


                                //resultsAttributes.DatabaseTransferObjects.Add(loadedAtributeCentric);
                                resultPeaks.Add(loadedPeakCentric);
                                resultsScans.DatabaseTransferObjects.Add(loadedScanCentric);
                            }

                            //results.PeakNumber = results.TandemPeakList.Count;

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();

                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }

            //return results;
            //return didThisWork;
        }

        public PrecursorAndPeaksObject SK_SelectPrecursorAndPeaksAndMonoPeaks(string fileName, string tableNamePrecursor, string tableNameMonoPeaks, string tableNamePeaks, int scanNumber, DatabaseTransferObject sampleObjectPrecursor, DatabaseTransferObject sampleObjectPeaks)
        {
            string aliasTable1Precursor = "SPP";//tableNamePrecursor
            string aliasTable2MonoPeaks = "SM";//tableNameMonoPeaks
            string aliasTable3Peaks = "SP";//tableNamePeaks

            //converts column names to field names so we can extract the information back out
            DatabasePeakProcessedWithMZObject currentSampleObjectPrecursor = (DatabasePeakProcessedWithMZObject)sampleObjectPrecursor;
            List<string> fieldNamePrecursor = new List<string>();
            foreach (string column in currentSampleObjectPrecursor.Columns)
            {
                //fieldNamePrecursor.Add(variableTable1 + "." + column);
                fieldNamePrecursor.Add(aliasTable1Precursor + "." + column + " AS '" + aliasTable1Precursor + "_" + column + "'");
            }

            DatabasePeakProcessedObject currentSampleObjectPeaks = (DatabasePeakProcessedObject)sampleObjectPeaks;
            List<string> fieldNameMonoPeaks = new List<string>();
            foreach (string column in currentSampleObjectPeaks.Columns)
            {
                //fieldNamePeaks.Add(variableTable2 + "." + column);
                fieldNameMonoPeaks.Add(aliasTable2MonoPeaks + "." + column + " AS '" + aliasTable2MonoPeaks + "_" + column + "'");
            }

            List<string> fieldNamePeaks = new List<string>();
            foreach (string column in currentSampleObjectPeaks.Columns)
            {
                //fieldNamePeaks.Add(variableTable2 + "." + column);
                fieldNamePeaks.Add(aliasTable3Peaks + "." + column + " AS '" + aliasTable3Peaks + "_" + column + "'");
            }

            //this is where the results will end up
            PrecursorAndPeaksObject results = new PrecursorAndPeaksObject();
            bool didThisWork = false;

            //set up connection string to database
            string connectionString = "Data Source=" + fileName;

            //string variable1 = "T_Scan_MonoPeaks.Scan";//column matches to input scanNumber
            //string variable1 = aliasTable2 + ".Scan";//column matches to input scanNumber  //DOES NOT WORK.  leads to over run
            string variable1 = aliasTable1Precursor + ".TandemScan";//column matches to input scanNumber.  

            //Set up SQLite statement
            string commandTextStringGetCount;
            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNamePrecursor) + "";
            commandTextStringGetCount += ", ";
            commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNameMonoPeaks) + "";
            commandTextStringGetCount += ", ";
            commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldNamePeaks) + " ";
            commandTextStringGetCount += "FROM " + tableNamePrecursor + " " + aliasTable1Precursor + " ";
            commandTextStringGetCount += "INNER JOIN " + tableNameMonoPeaks + " " + aliasTable2MonoPeaks + " ";
            commandTextStringGetCount += "ON " + aliasTable1Precursor + ".TandemScan = " + aliasTable2MonoPeaks + ".Scan ";
            commandTextStringGetCount += "INNER JOIN " + tableNamePeaks + " " + aliasTable3Peaks + " ";
            //commandTextStringGetCount += "ON " + aliasTable1Precursor + ".TandemScan = " + aliasTable3Peaks + ".Scan ";
            commandTextStringGetCount += "ON " + aliasTable3Peaks + ".Scan = " + aliasTable1Precursor + ".TandemScan ";
            commandTextStringGetCount += "WHERE " + variable1 + "= ? ";

            #region querry as Text
            //SELECT SPP.TandemScan AS 'SPP_TandemScan', SPP.PrecursorScan AS 'SPP_PrecursorScan', SPP.PeakNumber AS 'SPP_PeakNumber', SPP.XValue AS 'SPP_XValue', SPP.XValueRaw AS 'SPP_XValueRaw', SPP.Height AS 'SPP_Height', SPP.LocalSignalToNoise AS 'SPP_LocalSignalToNoise', SPP.Background AS 'SPP_Background', SPP.Width AS 'SPP_Width', SPP.LocalLowestMinimaHeight AS 'SPP_LocalLowestMinimaHeight', SPP.SignalToBackground AS 'SPP_SignalToBackground', SPP.SignalToNoiseGlobal AS 'SPP_SignalToNoiseGlobal', SPP.SignalToNoiseLocalMinima AS 'SPP_SignalToNoiseLocalMinima' ,
            //       SM.Scan AS 'SM_Scan', SM.PeakNumber AS 'SM_PeakNumber', SM.XValue AS 'SM_XValue', SM.Height AS 'SM_Height', SM.LocalSignalToNoise AS 'SM_LocalSignalToNoise', SM.Background AS 'SM_Background', SM.Width AS 'SM_Width', SM.LocalLowestMinimaHeight AS 'SM_LocalLowestMinimaHeight', SM.SignalToBackground AS 'SM_SignalToBackground', SM.SignalToNoiseGlobal AS 'SM_SignalToNoiseGlobal', SM.SignalToNoiseLocalMinima AS 'SM_SignalToNoiseLocalMinima'
            //
            //FROM T_Scans_Precursor_Peaks SPP 
            //    INNER JOIN T_Scan_MonoPeaks SM
            //        ON (SPP.TandemScan = SM.Scan)
            //WHERE SPP.TandemScan= ?
            #endregion

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;

                    //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list
                    SQLiteParameter newSqliteParamater = new SQLiteParameter(variable1, scanNumber);

                    getIsotopesCommmnd.CreateParameter();
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    getIsotopesCommmnd.Prepare();

                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();
                            int counter = 0;
                            while (databaseReader.Read())
                            {
                                DatabasePeakProcessedWithMZObject loadedMonoIsotopeObject = new DatabasePeakProcessedWithMZObject();
                                DatabasePeakProcessedObject loadedPeakObject = new DatabasePeakProcessedObject();
                                //foreach (DbDataRecord VARIABLE in databaseReader)
                                //{

                                //    loadedMonoIsotopeObject.ScanNum = Convert.ToInt32(VARIABLE.GetValue(0));
                                //    loadedMonoIsotopeObject.PeakNumber = Convert.ToInt32(VARIABLE.GetValue(1));

                                //}

                                //Console.WriteLine(counter);
                                //counter++;

                                results.PrecursorPeak.ScanNumberTandem = Convert.ToInt32(databaseReader["SPP_TandemScan"]);
                                results.PrecursorPeak.ScanNumberPrecursor = Convert.ToInt32(databaseReader["SPP_PrecursorScan"]);
                                results.PrecursorPeak.PeakNumber = Convert.ToInt32(databaseReader["SPP_PeakNumber"]);
                                results.PrecursorPeak.XValue = Convert.ToDouble(databaseReader["SPP_XValue"]);
                                results.PrecursorPeak.XValueRaw = Convert.ToDouble(databaseReader["SPP_XValueRaw"]);
                                results.PrecursorPeak.Charge = Convert.ToInt32(databaseReader["SPP_Charge"]);
                                results.PrecursorPeak.Height = Convert.ToDouble(databaseReader["SPP_Height"]);
                                results.PrecursorPeak.LocalSignalToNoise = Convert.ToDouble(databaseReader["SPP_LocalSignalToNoise"]);
                                results.PrecursorPeak.Background = Convert.ToDouble(databaseReader["SPP_Background"]);
                                results.PrecursorPeak.Width = Convert.ToDouble(databaseReader["SPP_Width"]);
                                results.PrecursorPeak.LocalLowestMinimaHeight = Convert.ToDouble(databaseReader["SPP_LocalLowestMinimaHeight"]);
                                results.PrecursorPeak.SignalToBackground = Convert.ToDouble(databaseReader["SPP_SignalToBackground"]);
                                results.PrecursorPeak.SignalToNoiseGlobal = Convert.ToDouble(databaseReader["SPP_SignalToNoiseGlobal"]);
                                results.PrecursorPeak.SignalToNoiseLocalMinima = Convert.ToDouble(databaseReader["SPP_SignalToNoiseLocalMinima"]);

                                loadedMonoIsotopeObject.ScanNumberTandem = Convert.ToInt32(databaseReader["SM_Scan"]);
                                loadedMonoIsotopeObject.PeakNumber = Convert.ToInt32(databaseReader["SM_PeakNumber"]);
                                loadedMonoIsotopeObject.XValue = Convert.ToDouble(databaseReader["SM_XValue"]);
                                loadedMonoIsotopeObject.Height = Convert.ToDouble(databaseReader["SM_Height"]);
                                loadedMonoIsotopeObject.LocalSignalToNoise = Convert.ToDouble(databaseReader["SM_LocalSignalToNoise"]);
                                loadedMonoIsotopeObject.Background = Convert.ToDouble(databaseReader["SM_Background"]);
                                loadedMonoIsotopeObject.Width = Convert.ToDouble(databaseReader["SM_Width"]);
                                loadedMonoIsotopeObject.LocalLowestMinimaHeight = Convert.ToDouble(databaseReader["SM_LocalLowestMinimaHeight"]);
                                loadedMonoIsotopeObject.SignalToBackground = Convert.ToDouble(databaseReader["SM_SignalToBackground"]);
                                loadedMonoIsotopeObject.SignalToNoiseGlobal = Convert.ToDouble(databaseReader["SM_SignalToNoiseGlobal"]);
                                loadedMonoIsotopeObject.SignalToNoiseLocalMinima = Convert.ToDouble(databaseReader["SM_SignalToNoiseLocalMinima"]);

                                loadedPeakObject.ScanNum = Convert.ToInt32(databaseReader["SP_Scan"]);
                                loadedPeakObject.PeakNumber = Convert.ToInt32(databaseReader["SP_PeakNumber"]);
                                loadedPeakObject.XValue = Convert.ToDouble(databaseReader["SP_XValue"]);
                                loadedPeakObject.Height = Convert.ToDouble(databaseReader["SP_Height"]);
                                loadedPeakObject.LocalSignalToNoise = Convert.ToDouble(databaseReader["SP_LocalSignalToNoise"]);
                                loadedPeakObject.Background = Convert.ToDouble(databaseReader["SP_Background"]);
                                loadedPeakObject.Width = Convert.ToDouble(databaseReader["SP_Width"]);
                                loadedPeakObject.LocalLowestMinimaHeight = Convert.ToDouble(databaseReader["SP_LocalLowestMinimaHeight"]);
                                loadedPeakObject.SignalToBackground = Convert.ToDouble(databaseReader["SP_SignalToBackground"]);
                                loadedPeakObject.SignalToNoiseGlobal = Convert.ToDouble(databaseReader["SP_SignalToNoiseGlobal"]);
                                loadedPeakObject.SignalToNoiseLocalMinima = Convert.ToDouble(databaseReader["SP_SignalToNoiseLocalMinima"]);

                                //this switching does nothing
                                if (loadedMonoIsotopeObject.ScanNumberTandem > 0)
                                {
                                    results.TandemMonoPeakList.Add(loadedMonoIsotopeObject);
                                }

                                if (loadedPeakObject.ScanNum > 0)
                                {
                                    results.TandemPeakList.Add(loadedPeakObject);
                                }
                                results.ScanNum = results.PrecursorPeak.ScanNumberTandem;
                                results.XValue = results.PrecursorPeak.XValueRaw;
                                results.Charge = results.PrecursorPeak.Charge;
                            }

                            //results.PeakNumber = results.TandemPeakList.Count;

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();

                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }

            return results;
            //return didThisWork;
        }

        //this is helpfull in checking which columns are around
        //foreach (DbDataRecord VARIABLE in databaseReader)
        //{

        //    loadedMonoIsotopeObject.ScanNum = Convert.ToInt32(VARIABLE.GetValue(0));
        //    loadedMonoIsotopeObject.PeakNumber = Convert.ToInt32(VARIABLE.GetValue(1));

        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="scanNumber">Scan</param>
        /// <param name="tableName">T_Scans</param>
        /// <param name="sampleObject">DatabaseScanObject</param>
        /// <returns></returns>
        public List<DatabaseScanObject> SK_SelectScansByScan(string fileName, int scanNumber, string tableName, DatabaseTransferObject sampleObject)
        {
            //FrameType frameType
            int startFrameNumber = 0;
            int endFrameNumber = 1000;
            int startScan;
            int endScan;

            //converts column names to field names so we can extract the information back out
            DatabaseScanObject currentSampleObject = (DatabaseScanObject)sampleObject;

            List<string> fieldName = new List<string>();
            foreach (string column in currentSampleObject.Columns)
            {
                fieldName.Add(column);
            }

            //this is where the results will end up
            List<DatabaseScanObject> results = new List<DatabaseScanObject>();
            bool didThisWork = false;

            //set up connection string to database
            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            //List<DatabaseIsotopeObject> loadedMonoIsotopeObjectList = new List<DatabaseIsotopeObject>();

            //Set up SQLite statement
            string commandTextStringGetCount;
            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldName) + " ";
            commandTextStringGetCount += "FROM " + tableName + " ";
            //commandTextStringGetCount += "WHERE " + "Scan In (SELECT Scan FROM T_Scans WHERE ParentScan = 8)";
            //commandTextStringGetCount += "WHERE " + "ParentScan = ?";
            commandTextStringGetCount += "WHERE " + "Scan = ?";
            

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;

                    //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list

                    //SQLiteParameter newSqliteParamater = new SQLiteParameter("Scan", 1);
                    //for (int index = 1; index < 4; index++)
                    //{
                    //int index = 8;

                    //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("minScan", 1));
                    //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("maxScan", sizeOfFile));

                    SQLiteParameter newSqliteParamater = new SQLiteParameter("Scan", scanNumber);

                    getIsotopesCommmnd.CreateParameter();
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    getIsotopesCommmnd.Prepare();
                    //}

                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();

                            while (databaseReader.Read())
                            {
                                //populate results here

                                DatabaseScanObject loadedMonoIsotopeObject = new DatabaseScanObject();

                                loadedMonoIsotopeObject.IndexId = Convert.ToInt32(databaseReader[fieldName[0]]);
                                loadedMonoIsotopeObject.Scan = Convert.ToInt32(databaseReader[fieldName[1]]);
                                loadedMonoIsotopeObject.MSLevel = Convert.ToInt32(databaseReader[fieldName[2]]);
                                loadedMonoIsotopeObject.ParentScan = Convert.ToInt32(databaseReader[fieldName[3]]);
                                loadedMonoIsotopeObject.Peaks = Convert.ToInt32(databaseReader[fieldName[4]]);
                                loadedMonoIsotopeObject.PeaksProcessed = Convert.ToInt32(databaseReader[fieldName[5]]);
                                loadedMonoIsotopeObject.PeakProcessingLevel = Convert.ToString(databaseReader[fieldName[6]]);

                                results.Add(loadedMonoIsotopeObject);
                            }

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();


                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }


            return results;
            //return didThisWork;
        }

        #region off
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="fileName"></param>
        ///// <param name="scanNumber">Scan</param>
        ///// <param name="tableName">T_Scans</param>
        ///// <param name="sampleObject">DatabaseScanObject</param>
        ///// <returns></returns>
        //public List<DatabaseFragmentCentricObject> SK_Centric_SelectScansByScan(string fileName, int scanNumber, string tableName, DatabaseTransferObject sampleObject)
        //{
        //    //FrameType frameType
        //    int startFrameNumber = 0;
        //    int endFrameNumber = 1000;
        //    int startScan;
        //    int endScan;

        //    //converts column names to field names so we can extract the information back out
        //    DatabaseFragmentCentricObject currentSampleObject = (DatabaseFragmentCentricObject)sampleObject;

        //    List<string> fieldName = new List<string>();
        //    foreach (string column in currentSampleObject.Columns)
        //    {
        //        fieldName.Add(column);
        //    }

        //    //this is where the results will end up
        //    List<DatabaseFragmentCentricObject> results = new List<DatabaseFragmentCentricObject>();
        //    bool didThisWork = false;

        //    //set up connection string to database
        //    string connectionString = "";
        //    connectionString = "Data Source=" + fileName;

        //    //List<DatabaseIsotopeObject> loadedMonoIsotopeObjectList = new List<DatabaseIsotopeObject>();

        //    //Set up SQLite statement
        //    string commandTextStringGetCount;
        //    commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
        //    commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldName) + " ";
        //    commandTextStringGetCount += "FROM " + tableName + " ";
        //    //commandTextStringGetCount += "WHERE " + "Scan In (SELECT Scan FROM T_Scans WHERE ParentScan = 8)";
        //    //commandTextStringGetCount += "WHERE " + "ParentScan = ?";
        //    commandTextStringGetCount += "WHERE " + "ScanID = ?";


        //    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        //    {
        //        connection.Open();

        //        using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
        //        {
        //            //load command up with SQLStatement
        //            getIsotopesCommmnd.CommandText = commandTextStringGetCount;

        //            //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list

        //            //SQLiteParameter newSqliteParamater = new SQLiteParameter("Scan", 1);
        //            //for (int index = 1; index < 4; index++)
        //            //{
        //            //int index = 8;

        //            //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("minScan", 1));
        //            //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("maxScan", sizeOfFile));

        //            SQLiteParameter newSqliteParamater = new SQLiteParameter("ScanID", scanNumber);

        //            getIsotopesCommmnd.CreateParameter();
        //            getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

        //            getIsotopesCommmnd.Prepare();
        //            //}

        //            //GO Get Data
        //            try
        //            {
        //                using (SQLiteTransaction transaction = connection.BeginTransaction())
        //                {
        //                    SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();

        //                    while (databaseReader.Read())
        //                    {
        //                        //populate results here

        //                        DatabaseFragmentCentricObject loadedMonoIsotopeObject = new DatabaseFragmentCentricObject();

        //                        loadedMonoIsotopeObject.FragmentCentricData.ScanID = Convert.ToInt32(databaseReader[fieldName[0]]);
        //                        loadedMonoIsotopeObject.FragmentCentricData.MsLevel = Convert.ToInt32(databaseReader[fieldName[1]]);
        //                        loadedMonoIsotopeObject.FragmentCentricData.ParentScanNumber = Convert.ToInt32(databaseReader[fieldName[2]]);
        //                        loadedMonoIsotopeObject.FragmentCentricData.TandemScanNumber = Convert.ToInt32(databaseReader[fieldName[3]]);
                                
        //                        results.Add(loadedMonoIsotopeObject);
        //                    }

        //                    databaseReader.Close();
        //                    transaction.Commit();

        //                    didThisWork = true;
        //                }
        //            }

        //            catch (SQLiteException exception)
        //            {
        //                didThisWork = false;
        //                Console.WriteLine("Failed :" + exception.Message);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //            }
        //        }
        //        connection.Close();


        //        //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
        //        //cmd.ExecuteNonQuery(); 
        //    }


        //    return results;
        //    //return didThisWork;
        //}

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="scanNumber">Scan</param>
        /// <param name="tableName">T_Scans</param>
        /// <param name="sampleObject">DatabaseScanObject</param>
        /// <returns></returns>
        public List<DatabaseScanCentricObject> SK2_SelectScansByScan(string fileName, int scanNumber, string tableName, DatabaseTransferObject sampleObject)
        {
            //FrameType frameType
            int startFrameNumber = 0;
            int endFrameNumber = 1000;
            int startScan;
            int endScan;

            //converts column names to field names so we can extract the information back out
            DatabaseScanCentricObject currentSampleObject = (DatabaseScanCentricObject)sampleObject;

            List<string> fieldName = new List<string>();
            foreach (string column in currentSampleObject.Columns)
            {
                fieldName.Add(column);
            }

            //this is where the results will end up
            List<DatabaseScanCentricObject> results = new List<DatabaseScanCentricObject>();
            bool didThisWork = false;

            //set up connection string to database
            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            //List<DatabaseIsotopeObject> loadedMonoIsotopeObjectList = new List<DatabaseIsotopeObject>();

            //Set up SQLite statement
            string commandTextStringGetCount;
            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldName) + " ";
            commandTextStringGetCount += "FROM " + tableName + " ";
            //commandTextStringGetCount += "WHERE " + "Scan In (SELECT Scan FROM T_Scans WHERE ParentScan = 8)";
            //commandTextStringGetCount += "WHERE " + "ParentScan = ?";
            commandTextStringGetCount += "WHERE " + "ScanNumLc = ?";


            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;

                    //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list

                    //SQLiteParameter newSqliteParamater = new SQLiteParameter("Scan", 1);
                    //for (int index = 1; index < 4; index++)
                    //{
                    //int index = 8;

                    //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("minScan", 1));
                    //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("maxScan", sizeOfFile));

                    SQLiteParameter newSqliteParamater = new SQLiteParameter("ScanNumLc", scanNumber);

                    getIsotopesCommmnd.CreateParameter();
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    getIsotopesCommmnd.Prepare();
                    //}

                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();

                            while (databaseReader.Read())
                            {
                                //populate results here

                                DatabaseScanCentricObject loadedMonoIsotopeObject = new DatabaseScanCentricObject();

                                loadedMonoIsotopeObject.ScanCentricData.ScanID = Convert.ToInt32(databaseReader[fieldName[0]]);
                                loadedMonoIsotopeObject.ScanCentricData.ScanNumLc = Convert.ToInt32(databaseReader[fieldName[1]]);
                                loadedMonoIsotopeObject.ScanCentricData.ElutionTime = Convert.ToDouble(databaseReader[fieldName[2]]);
                                loadedMonoIsotopeObject.ScanCentricData.FrameNumberDt = Convert.ToInt32(databaseReader[fieldName[3]]);
                                loadedMonoIsotopeObject.ScanCentricData.ScanNumDt = Convert.ToInt32(databaseReader[fieldName[4]]);
                                loadedMonoIsotopeObject.ScanCentricData.DriftTime = Convert.ToDouble(databaseReader[fieldName[5]]);
                                
                                results.Add(loadedMonoIsotopeObject);
                            }

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();


                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }


            return results;
            //return didThisWork;
        }

        #region off
        //public List<DatabaseFragmentCentricObject> SK2_SelectFragment(string fileName, string tableName, DatabaseTransferObject sampleObject)
        //{
        //    //FrameType frameType
        //    int startFrameNumber = 0;
        //    int endFrameNumber = 1000;
        //    int startScan;
        //    int endScan;

        //    //converts column names to field names so we can extract the information back out
        //    DatabaseFragmentCentricObject currentSampleObject = (DatabaseFragmentCentricObject)sampleObject;

        //    List<string> fieldName = new List<string>();
        //    foreach (string column in currentSampleObject.Columns)
        //    {
        //        fieldName.Add(column);
        //    }

        //    //this is where the results will end up
        //    List<DatabaseFragmentCentricObject> results = new List<DatabaseFragmentCentricObject>();
        //    bool didThisWork = false;

        //    //set up connection string to database
        //    string connectionString = "";
        //    connectionString = "Data Source=" + fileName;

        //    //List<DatabaseIsotopeObject> loadedMonoIsotopeObjectList = new List<DatabaseIsotopeObject>();

        //    //Set up SQLite statement
        //    string commandTextStringGetCount;
        //    commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
        //    commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldName) + " ";
        //    commandTextStringGetCount += "FROM " + tableName + " ";
        //    //commandTextStringGetCount += "WHERE " + "Scan In (SELECT Scan FROM T_Scans WHERE ParentScan = 8)";
        //    //commandTextStringGetCount += "WHERE " + "ParentScan = ?";
        //    commandTextStringGetCount += "WHERE " + "ParentScanNumber > 0;";


        //    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        //    {
        //        connection.Open();

        //        using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
        //        {
        //            //load command up with SQLStatement
        //            getIsotopesCommmnd.CommandText = commandTextStringGetCount;

        //            //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list

        //            //SQLiteParameter newSqliteParamater = new SQLiteParameter("Scan", 1);
        //            //for (int index = 1; index < 4; index++)
        //            //{
        //            //int index = 8;

        //            //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("minScan", 1));
        //            //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("maxScan", sizeOfFile));

        ////           SQLiteParameter newSqliteParamater = new SQLiteParameter("ID", scanNumber);

        ////           getIsotopesCommmnd.CreateParameter();
        ////           getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

        //            getIsotopesCommmnd.Prepare();
        //            //}

        //            //GO Get Data
        //            try
        //            {
        //                using (SQLiteTransaction transaction = connection.BeginTransaction())
        //                {
        //                    SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();

        //                    while (databaseReader.Read())
        //                    {
        //                        //populate results here

        //                        DatabaseFragmentCentricObject loadedMonoIsotopeObject = new DatabaseFragmentCentricObject();

        //                        loadedMonoIsotopeObject.FragmentCentricData.ScanID = Convert.ToInt32(databaseReader[fieldName[0]]);
        //                        loadedMonoIsotopeObject.FragmentCentricData.MsLevel = Convert.ToInt32(databaseReader[fieldName[1]]);
        //                        loadedMonoIsotopeObject.FragmentCentricData.ParentScanNumber = Convert.ToInt32(databaseReader[fieldName[2]]);
        //                        loadedMonoIsotopeObject.FragmentCentricData.TandemScanNumber = Convert.ToInt32(databaseReader[fieldName[3]]);
                                
        //                        results.Add(loadedMonoIsotopeObject);
        //                    }

        //                    databaseReader.Close();
        //                    transaction.Commit();

        //                    didThisWork = true;
        //                }
        //            }

        //            catch (SQLiteException exception)
        //            {
        //                didThisWork = false;
        //                Console.WriteLine("Failed :" + exception.Message);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //            }
        //        }
        //        connection.Close();


        //        //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
        //        //cmd.ExecuteNonQuery(); 
        //    }


        //    return results;
        //    //return didThisWork;
        //}

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="scanNumber">Parent Scan</param>
        /// <param name="tableName">T_Scans</param>
        /// <param name="sampleObject">DatabaseScanObject</param>
        /// <returns></returns>
        public List<DatabaseScanObject> SK_SelectScansByParentScan(string fileName, int scanNumber, string tableName, DatabaseTransferObject sampleObject)
        {
            //FrameType frameType
            int startFrameNumber = 0;
            int endFrameNumber = 1000;
            int startScan;
            int endScan;

            //converts column names to field names so we can extract the information back out
            DatabaseScanObject currentSampleObject = (DatabaseScanObject)sampleObject;

            List<string> fieldName = new List<string>();
            foreach (string column in currentSampleObject.Columns)
            {
                fieldName.Add(column);
            }

            //this is where the results will end up
            List<DatabaseScanObject> results = new List<DatabaseScanObject>();
            bool didThisWork = false;

            //set up connection string to database
            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            //List<DatabaseIsotopeObject> loadedMonoIsotopeObjectList = new List<DatabaseIsotopeObject>();

            //Set up SQLite statement
            string variable1 = "ParentScan";

            string commandTextStringGetCount;
            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldName) + " ";
            commandTextStringGetCount += "FROM " + tableName + " ";
            //commandTextStringGetCount += "WHERE " + "Scan In (SELECT Scan FROM T_Scans WHERE ParentScan = 8)";
            //commandTextStringGetCount += "WHERE " + "ParentScan = ?";
            commandTextStringGetCount += "WHERE " + variable1 + " = ?";

            

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;

                    //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list

                    //SQLiteParameter newSqliteParamater = new SQLiteParameter("Scan", 1);
                    //for (int index = 1; index < 4; index++)
                    //{
                    //int index = 8;

                    //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("minScan", 1));
                    //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("maxScan", sizeOfFile));

                    SQLiteParameter newSqliteParamater = new SQLiteParameter(variable1, scanNumber);

                    getIsotopesCommmnd.CreateParameter();
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    getIsotopesCommmnd.Prepare();
                    //}

                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();

                            while (databaseReader.Read())
                            {
                                //populate results here

                                DatabaseScanObject loadedMonoIsotopeObject = new DatabaseScanObject();

                                loadedMonoIsotopeObject.IndexId = Convert.ToInt32(databaseReader[fieldName[0]]);
                                loadedMonoIsotopeObject.Scan = Convert.ToInt32(databaseReader[fieldName[1]]);
                                loadedMonoIsotopeObject.MSLevel = Convert.ToInt32(databaseReader[fieldName[2]]);
                                loadedMonoIsotopeObject.ParentScan = Convert.ToInt32(databaseReader[fieldName[3]]);
                                loadedMonoIsotopeObject.Peaks = Convert.ToInt32(databaseReader[fieldName[4]]);
                                loadedMonoIsotopeObject.PeaksProcessed = Convert.ToInt32(databaseReader[fieldName[5]]);
                                loadedMonoIsotopeObject.PeakProcessingLevel = Convert.ToString(databaseReader[fieldName[6]]);

                                results.Add(loadedMonoIsotopeObject);
                            }

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();


                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }


            return results;
            //return didThisWork;
        }

        public List<DatabasePeakProcessedObject> SK_ReadProcessedPeak(string fileName, int scanNumber, string tableName, DatabaseTransferObject sampleObject)
        {
            //FrameType frameType
            int startFrameNumber = 0;
            int endFrameNumber = 1000;
            int startScan;
            int endScan;

            //converts column names to field names so we can extract the information back out
            DatabasePeakProcessedObject currentSampleObject = (DatabasePeakProcessedObject) sampleObject;
            List<string> fieldName = new List<string>();
            foreach (string column in currentSampleObject.Columns)
            {
                fieldName.Add(column);
            }

            //this is where the results will end up
            List<DatabasePeakProcessedObject> results = new List<DatabasePeakProcessedObject>();
            bool didThisWork = false;

            //set up connection string to database
            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            //List<DatabaseIsotopeObject> loadedMonoIsotopeObjectList = new List<DatabaseIsotopeObject>();
            string variable1 = "Scan";

            //Set up SQLite statement
            string commandTextStringGetCount;
            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldName) + " ";
            commandTextStringGetCount += "FROM " + tableName + " ";
            //commandTextStringGetCount += "WHERE " + "Scan In (SELECT Scan FROM T_Scans WHERE ParentScan = 8)";
            //commandTextStringGetCount += "WHERE " + "ParentScan = ?";
            commandTextStringGetCount += "WHERE " + variable1 + "= ?";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;
                    
                    //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list
                    
                    //SQLiteParameter newSqliteParamater = new SQLiteParameter("Scan", 1);
                    //for (int index = 1; index < 4; index++)
                    //{
                    //int index = 8;
                   
                    //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("minScan", 1));
                    //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("maxScan", sizeOfFile));

                    SQLiteParameter newSqliteParamater = new SQLiteParameter(variable1, scanNumber);

                    getIsotopesCommmnd.CreateParameter();
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    getIsotopesCommmnd.Prepare();
                    //}

                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();

                            while (databaseReader.Read())
                            {
                                //populate results here

                                DatabasePeakProcessedObject loadedMonoIsotopeObject = new DatabasePeakProcessedObject();

                                loadedMonoIsotopeObject.ScanNum = Convert.ToInt32(databaseReader[fieldName[0]]);
                                loadedMonoIsotopeObject.PeakNumber = Convert.ToInt32(databaseReader[fieldName[1]]);
                                loadedMonoIsotopeObject.XValue = Convert.ToDouble(databaseReader[fieldName[2]]);
                                loadedMonoIsotopeObject.Height = Convert.ToDouble(databaseReader[fieldName[3]]);
                                loadedMonoIsotopeObject.Charge = Convert.ToInt32(databaseReader[fieldName[4]]);
                                loadedMonoIsotopeObject.LocalSignalToNoise = Convert.ToDouble(databaseReader[fieldName[5]]);
                                loadedMonoIsotopeObject.Background = Convert.ToDouble(databaseReader[fieldName[6]]);
                                loadedMonoIsotopeObject.Width = Convert.ToDouble(databaseReader[fieldName[7]]);
                                loadedMonoIsotopeObject.LocalLowestMinimaHeight = Convert.ToDouble(databaseReader[fieldName[8]]);
                                loadedMonoIsotopeObject.SignalToBackground = Convert.ToDouble(databaseReader[fieldName[9]]);
                                loadedMonoIsotopeObject.SignalToNoiseGlobal = Convert.ToDouble(databaseReader[fieldName[10]]);
                                loadedMonoIsotopeObject.SignalToNoiseLocalMinima = Convert.ToDouble(databaseReader[fieldName[11]]);


                                results.Add(loadedMonoIsotopeObject);
                            }

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();


                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }


            return results;
            //return didThisWork;
        }

        public List<int> SK_ReadAllFragmentationScans(string fileName, string tableName)
        {
            
            //converts column names to field names so we can extract the information back out
            

            //this is where the results will end up
            List<int> results = new List<int>();
            bool didThisWork = false;

            //set up connection string to database
            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            //List<DatabaseIsotopeObject> loadedMonoIsotopeObjectList = new List<DatabaseIsotopeObject>();
            string variable1 = "Scan";

            //Set up SQLite statement
            string commandTextStringGetCount;
            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            commandTextStringGetCount += "TandemScan" + " ";
            commandTextStringGetCount += "FROM " + tableName + " ";
            //commandTextStringGetCount += "WHERE " + "Scan In (SELECT Scan FROM T_Scans WHERE ParentScan = 8)";
            //commandTextStringGetCount += "WHERE " + "ParentScan = ?";
            //commandTextStringGetCount += "WHERE " + variable1 + "= ?";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;

                    //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list

                    //SQLiteParameter newSqliteParamater = new SQLiteParameter("Scan", 1);
                    //for (int index = 1; index < 4; index++)
                    //{
                    //int index = 8;

                    //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("minScan", 1));
                    //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("maxScan", sizeOfFile));

                    //SQLiteParameter newSqliteParamater = new SQLiteParameter(variable1, scanNumber);

                    //getIsotopesCommmnd.CreateParameter();
                    //getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    //getIsotopesCommmnd.Prepare();
                    //}
                    
                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();

                            while (databaseReader.Read())
                            {
                                //populate results here

                                DatabasePeakProcessedObject loadedMonoIsotopeObject = new DatabasePeakProcessedObject();

                                results.Add(Convert.ToInt32(databaseReader["TandemScan"]));



                                
                            }

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();


                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }


            return results;
            //return didThisWork;
        }

        public List<int> SK_ReadAllScans(string fileName, string tableName)
        {

            //converts column names to field names so we can extract the information back out


            //this is where the results will end up
            List<int> results = new List<int>();
            bool didThisWork = false;

            //set up connection string to database
            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            //List<DatabaseIsotopeObject> loadedMonoIsotopeObjectList = new List<DatabaseIsotopeObject>();
            string variable1 = "Scan";

            //Set up SQLite statement
            string commandTextStringGetCount;
            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            commandTextStringGetCount += "Scan" + " ";
            commandTextStringGetCount += "FROM " + tableName + " ";
            //commandTextStringGetCount += "WHERE " + "Scan In (SELECT Scan FROM T_Scans WHERE ParentScan = 8)";
            //commandTextStringGetCount += "WHERE " + "ParentScan = ?";
            //commandTextStringGetCount += "WHERE " + variable1 + "= ?";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;

                    //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list

                    //SQLiteParameter newSqliteParamater = new SQLiteParameter("Scan", 1);
                    //for (int index = 1; index < 4; index++)
                    //{
                    //int index = 8;

                    //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("minScan", 1));
                    //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("maxScan", sizeOfFile));

                    //SQLiteParameter newSqliteParamater = new SQLiteParameter(variable1, scanNumber);

                    //getIsotopesCommmnd.CreateParameter();
                    //getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    //getIsotopesCommmnd.Prepare();
                    //}

                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();

                            while (databaseReader.Read())
                            {
                                //populate results here

                                DatabasePeakProcessedObject loadedMonoIsotopeObject = new DatabasePeakProcessedObject();

                                results.Add(Convert.ToInt32(databaseReader["Scan"]));




                            }

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();


                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }


            return results;
            //return didThisWork;
        }

        public List<int> SK2_ReadAllScans(string fileName, string tableName)
        {

            //converts column names to field names so we can extract the information back out


            //this is where the results will end up
            List<int> results = new List<int>();
            bool didThisWork = false;

            //set up connection string to database
            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            //List<DatabaseIsotopeObject> loadedMonoIsotopeObjectList = new List<DatabaseIsotopeObject>();
            string variable1 = "Scan";

            //Set up SQLite statement
            string commandTextStringGetCount;
            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            commandTextStringGetCount += "ScanNumLc" + " ";
            commandTextStringGetCount += "FROM " + tableName + " ";
            //commandTextStringGetCount += "WHERE " + "Scan In (SELECT Scan FROM T_Scans WHERE ParentScan = 8)";
            //commandTextStringGetCount += "WHERE " + "ParentScan = ?";
            //commandTextStringGetCount += "WHERE " + variable1 + "= ?";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;

                    //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list

                    //SQLiteParameter newSqliteParamater = new SQLiteParameter("Scan", 1);
                    //for (int index = 1; index < 4; index++)
                    //{
                    //int index = 8;

                    //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("minScan", 1));
                    //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("maxScan", sizeOfFile));

                    //SQLiteParameter newSqliteParamater = new SQLiteParameter(variable1, scanNumber);

                    //getIsotopesCommmnd.CreateParameter();
                    //getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    //getIsotopesCommmnd.Prepare();
                    //}

                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();

                            while (databaseReader.Read())
                            {
                                //populate results here

                                DatabasePeakProcessedObject loadedMonoIsotopeObject = new DatabasePeakProcessedObject();

                                results.Add(Convert.ToInt32(databaseReader["ScanNumLc"]));




                            }

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();


                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }


            return results;
            //return didThisWork;
        }


        public List<DatabaseScanCentricObject> SK2_ReadAllScanCentric(string fileName, string tableName)
        {

            //converts column names to field names so we can extract the information back out


            //this is where the results will end up
            List<DatabaseScanCentricObject> results = new List<DatabaseScanCentricObject>();
            bool didThisWork = false;

            //set up connection string to database
            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            //List<DatabaseIsotopeObject> loadedMonoIsotopeObjectList = new List<DatabaseIsotopeObject>();
            string variable1 = "Scan";

            //Set up SQLite statement
            string commandTextStringGetCount;
            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            commandTextStringGetCount += "*" + " ";//select all columns
            commandTextStringGetCount += "FROM " + tableName + " ";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;

                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();

                            while (databaseReader.Read())
                            {
                                //populate results here
                                DatabaseScanCentricObject loadedDataObject = new DatabaseScanCentricObject();
                                loadedDataObject.ScanCentricData.ScanID = Convert.ToInt32(databaseReader["ScanID"]);
                                loadedDataObject.ScanCentricData.PeakID = Convert.ToInt32(databaseReader["PeakID"]);

                                loadedDataObject.ScanCentricData.ScanNumLc = Convert.ToInt32(databaseReader["ScanNumLc"]);
                                loadedDataObject.ScanCentricData.ElutionTime = Convert.ToInt32(databaseReader["ElutionTime"]);

                                loadedDataObject.ScanCentricData.FrameNumberDt = Convert.ToInt32(databaseReader["FrameNumberDt"]);
                                loadedDataObject.ScanCentricData.ScanNumDt = Convert.ToInt32(databaseReader["ScanNumDt"]);
                                loadedDataObject.ScanCentricData.DriftTime = Convert.ToInt32(databaseReader["DriftTime"]);

                                loadedDataObject.ScanCentricData.MsLevel = Convert.ToInt32(databaseReader["MsLevel"]);
                                loadedDataObject.ScanCentricData.ParentScanNumber = Convert.ToInt32(databaseReader["ParentScanNumber"]);
                                loadedDataObject.ScanCentricData.TandemScanNumber = Convert.ToInt32(databaseReader["TandemScanNumber"]);

                                results.Add(loadedDataObject);
                            }

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();


                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }


            return results;
            //return didThisWork;
        }    

        public List<DatabasePeakProcessedWithMZObject> SK_ReadProcessedPeakWithMZ(string fileName, int scanNumber, string tableName, DatabaseTransferObject sampleObject)
        {
            //FrameType frameType
            int startFrameNumber = 0;
            int endFrameNumber = 1000;
            int startScan;
            int endScan;

            //converts column names to field names so we can extract the information back out
            DatabasePeakProcessedWithMZObject currentSampleObject = (DatabasePeakProcessedWithMZObject)sampleObject;
            List<string> fieldName = new List<string>();
            foreach (string column in currentSampleObject.Columns)
            {
                fieldName.Add(column);
            }

            //this is where the results will end up
            List<DatabasePeakProcessedWithMZObject> results = new List<DatabasePeakProcessedWithMZObject>();
            bool didThisWork = false;

            //set up connection string to database
            string connectionString = "";
            connectionString = "Data Source=" + fileName;


            //List<DatabaseIsotopeObject> loadedMonoIsotopeObjectList = new List<DatabaseIsotopeObject>();

            string variable1 = "TandemScan";

            //Set up SQLite statement
            string commandTextStringGetCount;
            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";
            commandTextStringGetCount += ConvertColumnNamesToCommaString(fieldName) + " ";
            commandTextStringGetCount += "FROM " + tableName + " ";
            //commandTextStringGetCount += "WHERE " + "Scan In (SELECT Scan FROM T_Scans WHERE ParentScan = 8)";
            //commandTextStringGetCount += "WHERE " + "ParentScan = ?";
            commandTextStringGetCount += "WHERE " + variable1 + " = ?";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    //load command up with SQLStatement
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;

                    //set up parameters for filling ? in statement.  The parameters are need for iterating through the table and returning a list

                    //SQLiteParameter newSqliteParamater = new SQLiteParameter("Scan", 1);
                    //for (int index = 1; index < 4; index++)
                    //{
                    //int index = 8;

                    //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("minScan", 1));
                    //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("maxScan", sizeOfFile));

                    SQLiteParameter newSqliteParamater = new SQLiteParameter(variable1, scanNumber);

                    getIsotopesCommmnd.CreateParameter();
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    getIsotopesCommmnd.Prepare();
                    //}

                    //GO Get Data
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();

                            while (databaseReader.Read())
                            {
                                //populate results here

                                DatabasePeakProcessedWithMZObject loadedMonoIsotopeObject = new DatabasePeakProcessedWithMZObject();

                                loadedMonoIsotopeObject.ScanNumberTandem = Convert.ToInt32(databaseReader[fieldName[0]]);
                                loadedMonoIsotopeObject.ScanNumberPrecursor = Convert.ToInt32(databaseReader[fieldName[1]]);
                                loadedMonoIsotopeObject.PeakNumber = Convert.ToInt32(databaseReader[fieldName[2]]);
                                loadedMonoIsotopeObject.XValue = Convert.ToDouble(databaseReader[fieldName[3]]);
                                loadedMonoIsotopeObject.XValueRaw = Convert.ToDouble(databaseReader[fieldName[4]]);
                                loadedMonoIsotopeObject.Height = Convert.ToDouble(databaseReader[fieldName[5]]);
                                loadedMonoIsotopeObject.LocalSignalToNoise = Convert.ToDouble(databaseReader[fieldName[6]]);
                                loadedMonoIsotopeObject.Background = Convert.ToDouble(databaseReader[fieldName[7]]);
                                loadedMonoIsotopeObject.Width = Convert.ToDouble(databaseReader[fieldName[8]]);
                                loadedMonoIsotopeObject.LocalLowestMinimaHeight = Convert.ToDouble(databaseReader[fieldName[9]]);
                                loadedMonoIsotopeObject.SignalToBackground = Convert.ToDouble(databaseReader[fieldName[10]]);
                                loadedMonoIsotopeObject.SignalToNoiseGlobal = Convert.ToDouble(databaseReader[fieldName[11]]);
                                loadedMonoIsotopeObject.SignalToNoiseLocalMinima = Convert.ToDouble(databaseReader[fieldName[12]]);


                                results.Add(loadedMonoIsotopeObject);
                            }

                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();


                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }


            return results;
            //return didThisWork;
        }

        public int ReadAllPrecursorPeaksFromScan(string fileName, int scanNumber)
        {
            //FrameType frameType
            int startFrameNumber = 0;
            int endFrameNumber = 1000;
            int startScan;
            int endScan;
            List<string> fieldName = new List<string>();

            fieldName.Add("Scan");
            fieldName.Add("PeakNumber");
            fieldName.Add("XValue");

            int nframes = endFrameNumber - startFrameNumber + 1;

            bool didThisWork = false;

            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            List<DatabaseIsotopeObject> loadedMonoIsotopeObjectList = new List<DatabaseIsotopeObject>();


            string commandTextStringGetCount = "SELECT " + "Scan, PeakNumber, XValue" + " ";

            commandTextStringGetCount += "FROM " + "T_Scan_Peaks" + " ";

            //commandTextStringGetCount += "WHERE " + "Scan In (SELECT Scan FROM T_Scans WHERE ParentScan = 8)";
            //commandTextStringGetCount += "WHERE " + "ParentScan = ?";
            commandTextStringGetCount += "WHERE " + "Scan = ?";

            
            //string arg = Select;
            //string arg2 = sql;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    getIsotopesCommmnd.CommandText = commandTextStringGetCount;
                    getIsotopesCommmnd.Prepare();
                    SQLiteParameter newSqliteParamater = new SQLiteParameter("Scan", 1);
                    //for (int index = 1; index < 4; index++)
                    //{
                    int index = 8;
                    newSqliteParamater = new SQLiteParameter("Scan", index);
      //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("minScan", 1));
      //              getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("maxScan", sizeOfFile));

                    getIsotopesCommmnd.CreateParameter();
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    //}

                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();

                            while (databaseReader.Read())
                            {

                                DatabaseIsotopeObject loadedMonoIsotopeObject = new DatabaseIsotopeObject();

                                loadedMonoIsotopeObject.MonoIsotopicMass = Convert.ToDouble(databaseReader[fieldName[0]]);
                                loadedMonoIsotopeObject.ExperimentMass = Convert.ToDouble(databaseReader[fieldName[1]]);
                                loadedMonoIsotopeObject.IsotopeMassesCSV = Convert.ToString(databaseReader[fieldName[2]]);
                      

                                loadedMonoIsotopeObjectList.Add(loadedMonoIsotopeObject);
                            }

                            //Console.WriteLine(ID + " " + abundance + " " + chargeState + " " + driftTime + " " + monoisotopicMass + " " + retentionTime + " " + score);
                            databaseReader.Close();
                            transaction.Commit();

                            didThisWork = true;
                        }
                    }

                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();


                //cmd.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
                //cmd.ExecuteNonQuery(); 
            }


            return nframes;
            //return didThisWork;
        }

        public bool CreateFeatrueLiteTableInformation(string fileName)
        {
            List<string> columnList = new List<string>();
            columnList.Add("Count");


            bool didThisWork = false;

            string connectionString = "";
            string commandTextString = "";
            connectionString = "Data Source=" + fileName;
            int counter=0;
            //Profiler newProfiler = new Profiler();
            int threadName = System.Threading.Thread.CurrentThread.ManagedThreadId;
            //newProfiler.printMemory("Before Lock " + threadName);



            //System.Threading.Monitor.TryEnter(databaseLock, 15);
            lock (databaseLock)
            {
                //newProfiler.printMemory(counter.ToString()); counter++;
                //newProfiler.printMemory(counter.ToString()); counter++;
                try
                {
                    //newProfiler.printMemory(counter.ToString()); counter++;
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        //newProfiler.printMemory(counter.ToString()); counter++;
                        try
                        {
                            //newProfiler.printMemory(counter.ToString()); counter++;
                            connection.Open();
                            //newProfiler.printMemory(counter.ToString()); counter++;
                        }
                        catch (SQLiteException exception)
                        {
                            #region inside catch
                            if (exception.Message == "The database file is locked\r\ndatabase is locked")
                            {
                                Console.WriteLine("Failed :" + exception.Message + "  Now what do we do");
                                Console.ReadKey();
                            }
                            didThisWork = false;
                            Console.WriteLine("Failed :" + exception.Message);
                            Console.ReadKey();
                            #endregion
                        }
                        catch (Exception anything)
                        {
                            Console.WriteLine("Failed :" + anything.Message + "  Now what do we do");
                        }
                        //newProfiler.printMemory(counter.ToString()); counter++;
                        using (SQLiteCommand command = new SQLiteCommand())
                        {
                            //newProfiler.printMemory(counter.ToString()); counter++;
                            command.Connection = connection;
                            //newProfiler.printMemory(counter.ToString()); counter++;
                            #region set command for column names
                            //command.CommandText = "CREATE TABLE IF NOT EXISTS FeatureLiteTable (ID double, Abundance double, ChargeState double, DriftTime double, MonoisotopicMass double, RetentionTime double , Score double);";
                            commandTextString = "CREATE TABLE IF NOT EXISTS TableInfo (";
                            for (int i = 0; i < columnList.Count - 1; i++)
                            {
                                commandTextString += columnList[i] + " double,";
                            }
                            commandTextString += columnList[columnList.Count - 1] + " double);";//last point
                            #endregion
                            command.CommandText = commandTextString;

                            try
                            {
                                #region inside try
                                command.ExecuteNonQuery();
                                //newProfiler.printMemory(counter.ToString()); counter++;
                                using (SQLiteTransaction transaction = connection.BeginTransaction())
                                {
                                    //newProfiler.printMemory(counter.ToString()+"L"); counter++;
                                    #region set command string for inserts
                                    //command.CommandText = "INSERT INTO FeatureLiteTable values (@ID, @Abundance, @ChargeState, @DriftTime, @MonoisotopicMass, @RetentionTime, @Score)";
                                    commandTextString = "INSERT INTO TableInfo values (";
                                    for (int i = 0; i < columnList.Count - 1; i++)
                                    {
                                        commandTextString += "@" + columnList[i] + ",";
                                    }
                                    commandTextString += "@" + columnList[columnList.Count - 1] + ")";

                                    #endregion

                                    command.CommandText = commandTextString;

                                    #region setup columns and add data to each column.  In this case initialize count
                                    DbParameter parameterNumber0 = command.CreateParameter();
                                    parameterNumber0.ParameterName = columnList[0];
                                    parameterNumber0.DbType = DbType.Int32;
                                    parameterNumber0.Value = 0;//sets the count to zero

                                    command.Parameters.Add(parameterNumber0);
                                    command.ExecuteNonQuery();
                                    #endregion

                                    #region commit try catch
                                    try
                                    {
                                        //newProfiler.printMemory(counter.ToString() + "y"); counter++;
                                        transaction.Commit();
                                        //newProfiler.printMemory(counter.ToString() + "z"); counter++;
                                        //newProfiler.printMemory(counter.ToString() + "z2"); counter++;
                                    }
                                    catch (SQLiteException exception)
                                    {
                                        #region inside catch
                                        if (exception.Message == "The database file is locked\r\ndatabase is locked")
                                        {
                                            Console.WriteLine("Failed :" + exception.Message + "  Now what do we do");
                                            Console.ReadKey();
                                        }
                                        didThisWork = false;
                                        Console.WriteLine("Failed :" + exception.Message);
                                        Console.ReadKey();
                                        #endregion
                                    }
                                    finally
                                    {
                                        //newProfiler.printMemory(counter.ToString() + "O"); counter++;
                                        connection.Close();
                                    }
                                    #endregion

                                    didThisWork = true;//if the Commit worked we will set the toggle to true here
                                }//this using cleans up nicely 3-20-11
                                #endregion
                            }
                            catch (SQLiteException exception)
                            {
                                #region inside catch
                                if (exception.Message == "The database file is locked\r\ndatabase is locked")
                                {
                                    Console.WriteLine("Failed :" + exception.Message + "  Now what do we do" + threadName);
                                    Console.ReadKey();
                                    int counter2=0;
                                    while (didThisWork == false)
                                    {
                                        try
                                        {
                                            counter++;
                                            Console.WriteLine("loop " + counter2);
                                            #region inside try
                                            command.ExecuteNonQuery();
                                            using (SQLiteTransaction transaction = connection.BeginTransaction())
                                            {
                                                #region set command string for inserts
                                                //command.CommandText = "INSERT INTO FeatureLiteTable values (@ID, @Abundance, @ChargeState, @DriftTime, @MonoisotopicMass, @RetentionTime, @Score)";
                                                commandTextString = "INSERT INTO TableInfo values (";
                                                for (int i = 0; i < columnList.Count - 1; i++)
                                                {
                                                    commandTextString += "@" + columnList[i] + ",";
                                                }
                                                commandTextString += "@" + columnList[columnList.Count - 1] + ")";

                                                #endregion

                                                command.CommandText = commandTextString;

                                                #region setup columns and add data to each column.  In this case initialize count
                                                DbParameter parameterNumber0 = command.CreateParameter();
                                                parameterNumber0.ParameterName = columnList[0];
                                                parameterNumber0.DbType = DbType.Int32;
                                                parameterNumber0.Value = 0;//sets the count to zero

                                                command.Parameters.Add(parameterNumber0);
                                                command.ExecuteNonQuery();
                                                #endregion

                                                #region commit try catch
                                                try
                                                {
                                                    transaction.Commit();
                                                }
                                                catch (SQLiteException exception2)
                                                {
                                                    #region inside catch
                                                    if (exception.Message == "The database file is locked\r\ndatabase is locked")
                                                    {
                                                        Console.WriteLine("Failed :" + exception2.Message + "  Now what do we do");
                                                        Console.ReadKey();
                                                    }
                                                    didThisWork = false;
                                                    Console.WriteLine("Failed :" + exception.Message);
                                                    Console.ReadKey();
                                                    #endregion
                                                }
                                                finally
                                                {
                                                    connection.Close();
                                                }
                                                #endregion

                                                didThisWork = true;//if the Commit worked we will set the toggle to true here
                                            }//this using cleans up nicely 3-20-11
                                            #endregion
                                        }
                                        catch 
                                        {
                                            Console.WriteLine("Failed :" + exception.Message);
                                        }
                                    }
                                }
                                didThisWork = false;
                                Console.WriteLine("Failed :" + exception.Message);
                                Console.ReadKey();
                                #endregion
                            }
                           
                            finally
                            {
                                //newProfiler.printMemory(counter.ToString()); counter++;
                                connection.Close();
                                //newProfiler.printMemory(counter.ToString()+"k"); counter++;
                            }
                            //newProfiler.printMemory(counter.ToString()); counter++;
                            command.Parameters.Clear();
                            //newProfiler.printMemory(counter.ToString()); counter++;
                        }//this using cleans up nicely 3-20-11

                        //connection.Close();//already closed in the finally 
                    }//this using cleans up nicely 3-20-11
                }
                catch (Exception anything)
                {
                    Console.WriteLine("Failed :" + anything.Message + "  Now what do we do");
                }
                
                
            }//endlock

            //newProfiler.printMemory("After lock Using " + threadName);
            int text = 4; text = 5; int text3 = text;

            columnList = null;
            return didThisWork;
        }

        #region SQL writers

        #region old stuff (off)
        //public bool WriteElutingPeakdata(DatabaseElutingPeakObject newEntry, string fileName)
        //{
        //    bool didThisWork = false;
        //    string connectionString = "";
        //    string commandTextString = "";
        //    connectionString = "Data Source=" + fileName;

        //    List<string> columnList = new List<string>();
        //    columnList.Add("ElutingPeakID");
        //    columnList.Add("ElutingPeakMass");
        //    columnList.Add("ElutingPeakScanStart");
        //    columnList.Add("ElutingPeakScanEnd");
        //    columnList.Add("ElutingPeakScanMaxIntensity");
        //    columnList.Add("ElutingPeakNumberofPeaks");
        //    columnList.Add("ElutingPeakNumberOfPeaksFlag");
        //    columnList.Add("ElutingPeakNumberOfPeaksMode");
        //    columnList.Add("ElutingPeakSummedIntensity");
        //    columnList.Add("ElutingPeakIntensityAggregate");

        //    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        //    {
        //        connection.Open();
        //        using (SQLiteCommand command = new SQLiteCommand())
        //        {
        //            int limit = 1;//one at a time
        //            command.Connection = connection;

        //            #region set command for column names
        //            command.CommandText = "CREATE TABLE IF NOT EXISTS ElutingPeakTable (ElutingPeakID double, ElutingPeakMass double, ElutingPeakScanStart double, ElutingPeakScanEnd double, ElutingPeakScanMaxIntensity double, ElutingPeakNumberofPeaks double, ElutingPeakNumberOfPeaksFlag double, ElutingPeakNumberOfPeaksMode double, ElutingPeakSummedIntensity double, ElutingPeakIsosResultIntensityAggregate double);";
        //            commandTextString = "CREATE TABLE IF NOT EXISTS ElutingPeakTable (";
        //            for (int i = 0; i < columnList.Count - 1; i++)
        //            {
        //                commandTextString += columnList[i] + " double,";
        //            }
        //            commandTextString += columnList[columnList.Count - 1] + " double);";//last point
        //            #endregion
        //            command.CommandText = commandTextString;

        //            try
        //            {
        //                command.ExecuteNonQuery();
        //                using (SQLiteTransaction transaction = connection.BeginTransaction())
        //                {

        //                    #region set command string for inserts
        //                    command.CommandText = "INSERT INTO ElutingPeakTable values (@ElutingPeakID, @ElutingPeakMass, @ElutingPeakScanStart, @ElutingPeakScanEnd, @ElutingPeakScanMaxIntensity, @ElutingPeakNumberofPeaks, @ElutingPeakNumberOfPeaksFlag, @ElutingPeakNumberOfPeaksMode, @ElutingPeakSummedIntensity, @ElutingPeakIsosResultIntensityAggregate)";

        //                    commandTextString = "INSERT INTO ElutingPeakTable values (";
        //                    for (int i = 0; i < columnList.Count - 1; i++)
        //                    {
        //                        commandTextString += "@" + columnList[i] + ",";
        //                    }
        //                    commandTextString += "@" + columnList[columnList.Count - 1] + ")";

        //                    #endregion
        //                    command.CommandText = commandTextString;

        //                    #region setup columns and add data to each column

        //                    DbParameter parameterNumber0 = command.CreateParameter();//ID
        //                    parameterNumber0.ParameterName = columnList[0];
        //                    parameterNumber0.DbType = DbType.Int32;
        //                    command.Parameters.Add(parameterNumber0);

        //                    DbParameter parameterNumber1 = command.CreateParameter();//Mass
        //                    parameterNumber1.ParameterName = columnList[1];
        //                    parameterNumber1.DbType = DbType.Double;
        //                    command.Parameters.Add(parameterNumber1);

        //                    DbParameter parameterNumber2 = command.CreateParameter();//ScanStart
        //                    parameterNumber2.ParameterName = columnList[2];
        //                    parameterNumber2.DbType = DbType.Int32;
        //                    command.Parameters.Add(parameterNumber2);

        //                    DbParameter parameterNumber3 = command.CreateParameter();//ScanEnd
        //                    parameterNumber3.ParameterName = columnList[3];
        //                    parameterNumber3.DbType = DbType.Int32;
        //                    command.Parameters.Add(parameterNumber3);

        //                    DbParameter parameterNumber4 = command.CreateParameter();//ScanMaxIntensity
        //                    parameterNumber4.ParameterName = columnList[4];
        //                    parameterNumber4.DbType = DbType.Int32;
        //                    command.Parameters.Add(parameterNumber4);

        //                    DbParameter parameterNumber5 = command.CreateParameter();//number of peaks
        //                    parameterNumber5.ParameterName = columnList[5];
        //                    parameterNumber5.DbType = DbType.Int32;
        //                    command.Parameters.Add(parameterNumber5);

        //                    DbParameter parameterNumber6 = command.CreateParameter();//peaks flag
        //                    parameterNumber6.ParameterName = columnList[6];
        //                    parameterNumber6.DbType = DbType.Int32;
        //                    command.Parameters.Add(parameterNumber6);

        //                    DbParameter parameterNumber7 = command.CreateParameter();//peaks method
        //                    parameterNumber7.ParameterName = columnList[7];
        //                    parameterNumber7.DbType = DbType.String;
        //                    command.Parameters.Add(parameterNumber7);

        //                    DbParameter parameterNumber8 = command.CreateParameter();//summed intensity
        //                    parameterNumber8.ParameterName = columnList[8];
        //                    parameterNumber8.DbType = DbType.Single;
        //                    command.Parameters.Add(parameterNumber8);

        //                    DbParameter parameterNumber9 = command.CreateParameter();//intensity agregate
        //                    parameterNumber9.ParameterName = columnList[9];
        //                    parameterNumber9.DbType = DbType.Double;
        //                    command.Parameters.Add(parameterNumber9);

        //                    for (int i = 0; i < limit; i++)
        //                    {
        //                        parameterNumber0.Value = newEntry.ElutingPeakID;
        //                        parameterNumber1.Value = newEntry.ElutingPeakMass;

        //                        parameterNumber2.Value = newEntry.ElutingPeakScanStart;
        //                        parameterNumber3.Value = newEntry.ElutingPeakScanEnd;
        //                        parameterNumber4.Value = newEntry.ElutingPeakScanMaxIntensity;

        //                        parameterNumber5.Value = newEntry.ElutingPeakNumberofPeaks;
        //                        parameterNumber6.Value = newEntry.ElutingPeakNumberOfPeaksFlag;
        //                        parameterNumber7.Value = newEntry.ElutingPeakNumberOfPeaksMode;

        //                        parameterNumber8.Value = newEntry.ElutingPeakSummedIntensity;
        //                        parameterNumber9.Value = newEntry.ElutingPeakIntensityAggregate;

        //                        command.ExecuteNonQuery();
        //                    }

        //                    #endregion
        //                    transaction.Commit();

        //                    didThisWork = true;
        //                }
        //            }

        //            catch (SQLiteException exception)
        //            {
        //                didThisWork = false;
        //                Console.WriteLine("Failed :" + exception.Message);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //            }
        //        }
        //        connection.Close();
        //    }
        //    return didThisWork;
        //}

        //public bool WriteIsotopeStorageData(DatabaseIsotopeObject newEntry, string fileName)
        //{
        //    bool didThisWork = false;
        //    string connectionString = "";
        //    string commandTextString = "";
        //    connectionString = "Data Source=" + fileName;

        //    List<string> columnList = new List<string>();
        //    columnList.Add("MonoisotopicMass");
        //    columnList.Add("ExperimentMass");
        //    columnList.Add("IsotopeMasses");
        //    columnList.Add("IsotopeIntensities");

        //    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        //    {
        //        connection.Open();
        //        using (SQLiteCommand command = new SQLiteCommand())
        //        {
        //            int limit = 1;//one at a time
        //            command.Connection = connection;

        //            #region set command for column names
        //            command.CommandText = "CREATE TABLE IF NOT EXISTS ElutingPeakTable (ElutingPeakID double, ElutingPeakMass double, ElutingPeakScanStart double, ElutingPeakScanEnd double, ElutingPeakScanMaxIntensity double, ElutingPeakNumberofPeaks double, ElutingPeakNumberOfPeaksFlag double, ElutingPeakNumberOfPeaksMode double, ElutingPeakSummedIntensity double, ElutingPeakIsosResultIntensityAggregate double);";
        //            commandTextString = "CREATE TABLE IF NOT EXISTS IsotopeTable (";
        //            for (int i = 0; i < columnList.Count - 2; i++)
        //            {
        //                commandTextString += columnList[i] + " double,";
        //            }
        //            commandTextString += columnList[columnList.Count - 2] + " string,";//last point
        //            commandTextString += columnList[columnList.Count - 1] + " string);";//last point
        //            #endregion
        //            command.CommandText = commandTextString;

        //            try
        //            {
        //                command.ExecuteNonQuery();
        //                using (SQLiteTransaction transaction = connection.BeginTransaction())
        //                {

        //                    #region set command string for inserts
        //                    command.CommandText = "INSERT INTO ElutingPeakTable values (@ElutingPeakID, @ElutingPeakMass, @ElutingPeakScanStart, @ElutingPeakScanEnd, @ElutingPeakScanMaxIntensity, @ElutingPeakNumberofPeaks, @ElutingPeakNumberOfPeaksFlag, @ElutingPeakNumberOfPeaksMode, @ElutingPeakSummedIntensity, @ElutingPeakIsosResultIntensityAggregate)";

        //                    commandTextString = "INSERT INTO IsotopeTable values (";
        //                    for (int i = 0; i < columnList.Count - 1; i++)
        //                    {
        //                        commandTextString += "@" + columnList[i] + ",";
        //                    }
        //                    commandTextString += "@" + columnList[columnList.Count - 1] + ")";

        //                    #endregion
        //                    command.CommandText = commandTextString;

        //                    #region setup columns and add data to each column

        //                    DbParameter parameterNumber0 = command.CreateParameter();//Monoisotopic Mass
        //                    parameterNumber0.ParameterName = columnList[0];
        //                    parameterNumber0.DbType = DbType.Double;
        //                    command.Parameters.Add(parameterNumber0);

        //                    DbParameter parameterNumber1 = command.CreateParameter();//Experimental Mass
        //                    parameterNumber1.ParameterName = columnList[1];
        //                    parameterNumber1.DbType = DbType.Double;
        //                    command.Parameters.Add(parameterNumber1);

        //                    DbParameter parameterNumber2 = command.CreateParameter();//Isotope Mass string
        //                    parameterNumber2.ParameterName = columnList[2];
        //                    parameterNumber2.DbType = DbType.String;
        //                    command.Parameters.Add(parameterNumber2);

        //                    DbParameter parameterNumber3 = command.CreateParameter();//Isotope Intensity string
        //                    parameterNumber3.ParameterName = columnList[3];
        //                    parameterNumber3.DbType = DbType.String;
        //                    command.Parameters.Add(parameterNumber3);

        //                    for (int i = 0; i < limit; i++)
        //                    {
        //                        parameterNumber0.Value = newEntry.MonoIsotopicMass;
        //                        parameterNumber1.Value = newEntry.ExperimentMass;

        //                        parameterNumber2.Value = newEntry.IsotopeMassesCSV;
        //                        parameterNumber3.Value = newEntry.IsotoepIntensitiesCSV;

        //                        command.ExecuteNonQuery();
        //                    }

        //                    #endregion
        //                    transaction.Commit();

        //                    didThisWork = true;
        //                }
        //            }

        //            catch (SQLiteException exception)
        //            {
        //                didThisWork = false;
        //                Console.WriteLine("Failed :" + exception.Message);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //            }
        //        }
        //        connection.Close();
        //    }
        //    return didThisWork;
        //}

        //public bool WriteFeatrueLitedata(DatabaseFeatureLiteObject newEntry, string fileName)
        //{
        //    List<string> columnList = new List<string>();
        //    columnList.Add("ID");
        //    columnList.Add("Abundance");
        //    columnList.Add("ChargeState");
        //    columnList.Add("DriftTime");
        //    columnList.Add("MonoisotopicMass");
        //    columnList.Add("RetentionTime");
        //    columnList.Add("Score");

        //    bool didThisWork = false;

        //    string connectionString = "";
        //    string commandTextString = "";
        //    connectionString = "Data Source=" + fileName;

        //    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        //    {
        //        connection.Open();
        //        using (SQLiteCommand command = new SQLiteCommand())
        //        {
        //            int limit = 1;//one at a time

        //            command.Connection = connection;

        //            #region set command for column names
        //            command.CommandText = "CREATE TABLE IF NOT EXISTS FeatureLiteTable (ID double, Abundance double, ChargeState double, DriftTime double, MonoisotopicMass double, RetentionTime double , Score double);";
        //            commandTextString = "CREATE TABLE IF NOT EXISTS FeatureLiteTable (";
        //            for (int i = 0; i < columnList.Count - 1; i++)
        //            {
        //                commandTextString += columnList[i] + " double,";
        //            }
        //            commandTextString += columnList[columnList.Count - 1] + " double);";//last point
        //            #endregion
        //            command.CommandText = commandTextString;

        //            try
        //            {
        //                command.ExecuteNonQuery();
        //                using (SQLiteTransaction transaction = connection.BeginTransaction())
        //                {
        //                    #region set command string for inserts
        //                    command.CommandText = "INSERT INTO FeatureLiteTable values (@ID, @Abundance, @ChargeState, @DriftTime, @MonoisotopicMass, @RetentionTime, @Score)";
        //                    commandTextString = "INSERT INTO FeatureLiteTable values (";
        //                    for (int i = 0; i < columnList.Count - 1; i++)
        //                    {
        //                        commandTextString += "@" + columnList[i] + ",";
        //                    }
        //                    commandTextString += "@" + columnList[columnList.Count - 1] + ")";

        //                    #endregion
        //                    command.CommandText = commandTextString;

        //                    #region setup columns and add data to each column
        //                    DbParameter parameterNumber0 = command.CreateParameter();
        //                    parameterNumber0.ParameterName = columnList[0];
        //                    parameterNumber0.DbType = DbType.Int32;
        //                    command.Parameters.Add(parameterNumber0);

        //                    DbParameter parameterNumber1 = command.CreateParameter();
        //                    parameterNumber1.ParameterName = columnList[1];
        //                    parameterNumber1.DbType = DbType.Single;
        //                    command.Parameters.Add(parameterNumber1);

        //                    DbParameter parameterNumber2 = command.CreateParameter();
        //                    parameterNumber2.ParameterName = columnList[2];
        //                    parameterNumber2.DbType = DbType.Int32;
        //                    command.Parameters.Add(parameterNumber2);

        //                    DbParameter parameterNumber3 = command.CreateParameter();
        //                    parameterNumber3.ParameterName = columnList[3];
        //                    parameterNumber3.DbType = DbType.Int32;
        //                    command.Parameters.Add(parameterNumber3);

        //                    DbParameter parameterNumber4 = command.CreateParameter();
        //                    parameterNumber4.ParameterName = columnList[4];
        //                    parameterNumber4.DbType = DbType.Double;
        //                    command.Parameters.Add(parameterNumber4);

        //                    DbParameter parameterNumber5 = command.CreateParameter();
        //                    parameterNumber5.ParameterName = columnList[5];
        //                    parameterNumber5.DbType = DbType.Double;
        //                    command.Parameters.Add(parameterNumber5);

        //                    DbParameter parameterNumber6 = command.CreateParameter();
        //                    parameterNumber6.ParameterName = columnList[6];
        //                    parameterNumber6.DbType = DbType.Double;
        //                    command.Parameters.Add(parameterNumber6);

        //                    for (int i = 0; i < limit; i++)
        //                    {
        //                        parameterNumber0.Value = newEntry.ID;
        //                        parameterNumber1.Value = newEntry.Abundance;
        //                        parameterNumber2.Value = newEntry.ChargeState;
        //                        parameterNumber3.Value = newEntry.DriftTime;
        //                        parameterNumber4.Value = newEntry.MassMonoisotopic;
        //                        parameterNumber5.Value = newEntry.RetentionTime;
        //                        parameterNumber6.Value = newEntry.Score;
        //                        command.ExecuteNonQuery();
        //                    }

        //                    #endregion

        //                    transaction.Commit();

        //                    didThisWork = true;
        //                }
        //            }

        //            catch (SQLiteException exception)
        //            {
        //                didThisWork = false;
        //                Console.WriteLine("Failed :" + exception.Message);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //            }
        //        }
        //        connection.Close();
        //    }
        //    return didThisWork;
        //}

        #endregion

        /// <summary>
        /// needs to be followed by a update to count
        /// </summary>
        /// <param name="newEntry"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool WriteDataTransferObject(DatabaseTransferObject newEntry, string fileName)
        {
            bool didThisWork = false;

            string connectionString = "";
            string commandTextString = "";
            connectionString = "Data Source=" + fileName;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    int limit = 1;//one at a time

                    command.Connection = connection;

                    #region set command for column names
                    //command.CommandText = "CREATE TABLE IF NOT EXISTS FeatureLiteTable (ID double, Abundance double, ChargeState double, DriftTime double, MonoisotopicMass double, RetentionTime double , Score double);";
                    //commandTextString = "CREATE TABLE IF NOT EXISTS FeatureLiteTable (";
                    commandTextString = "CREATE TABLE IF NOT EXISTS " + newEntry.TableName + " (";
                    for (int i = 0; i < newEntry.Columns.Count - 1; i++)
                    {
                        commandTextString += newEntry.Columns[i] + " double,";
                    }
                    commandTextString += newEntry.Columns[newEntry.Columns.Count - 1] + " double);";//last point
                    #endregion

                    command.CommandText = commandTextString;

                    try
                    {
                        command.ExecuteNonQuery();
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            #region set command string for inserts
                            //command.CommandText = "INSERT INTO FeatureLiteTable values (@ID, @Abundance, @ChargeState, @DriftTime, @MonoisotopicMass, @RetentionTime, @Score)";
                            //commandTextString = "INSERT INTO FeatureLiteTable values (";
                            commandTextString = "INSERT INTO " + newEntry.TableName + " values (";
                            for (int i = 0; i < newEntry.Columns.Count - 1; i++)
                            {
                                commandTextString += "@" + newEntry.Columns[i] + ",";
                            }
                            commandTextString += "@" + newEntry.Columns[newEntry.Columns.Count - 1] + ")";

                            #endregion

                            command.CommandText = commandTextString;

                            #region setup columns and add data to each column

                            for(int i=0; i<newEntry.ValuesTypes.Count; i++)
                            {
                                DbParameter parameterNumber = command.CreateParameter();
                                parameterNumber.ParameterName = newEntry.Columns[i];
                                parameterNumber.DbType = newEntry.ValuesTypes[i];// DbType.Double;
                                parameterNumber.Value = newEntry.Values[i];
                                command.Parameters.Add(parameterNumber);
                            }
                            
                            for (int i = 0; i < limit; i++)
                            {
                                command.ExecuteNonQuery();
                            }

                            #endregion

                            transaction.Commit();

                            didThisWork = true;
                        }
                    }
                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                connection.Close();
            }
            return didThisWork;
        }

        #endregion

        #region SQL List writers

        /// <summary>
        /// currently the best way to do this.  should be generic based on the type of the DatabaseTransferObjectList
        /// </summary>
        /// <param name="newEntry"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool WriteDataTransferObjectList(DatabaseTransferObjectList newEntry, string fileName)
        {
            bool didThisWork = false;

            string connectionString = "";
            string commandTextString = "";
            connectionString = "Data Source=" + fileName;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand())
                {
                    #region inside

                    int limit = 1;//one at a time

                    #region set command for column names and create table

                    command.Connection = connection;
                    //command.CommandText = "CREATE TABLE IF NOT EXISTS FeatureLiteTable (ID double, Abundance double, ChargeState double, DriftTime double, MonoisotopicMass double, RetentionTime double , Score double);";
                    //commandTextString = "CREATE TABLE IF NOT EXISTS FeatureLiteTable (";
                    commandTextString = "CREATE TABLE IF NOT EXISTS " + newEntry.TableName + " (";
                   
                    for (int i = 0; i < newEntry.Columns.Count - 1; i++)
                    {
                        switch(newEntry.ValuesTypes[i])
                        {
                            case DbType.Double:
                                {
                                    commandTextString += newEntry.Columns[i] + " double,";
                                }
                                break;
                            case DbType.String:
                                {
                                    commandTextString += newEntry.Columns[i] + " string,";
                                }
                                break;
                            case DbType.Int32:
                                {
                                    commandTextString += newEntry.Columns[i] + " integer,";
                                }
                                break;
                            default:
                                {
                                    commandTextString += newEntry.Columns[i] + " double,";
                                }
                                break;

                        }
                    }
                    //last point
                    switch (newEntry.ValuesTypes[newEntry.Columns.Count-1])
                    {
                        case DbType.Double:
                            {
                                commandTextString += newEntry.Columns[newEntry.Columns.Count - 1] + " double);";//last point
                            }
                            break;
                        case DbType.String:
                            {
                                commandTextString += newEntry.Columns[newEntry.Columns.Count - 1] + " string);";//last point
                            }
                            break;
                        case DbType.Int32:
                            {
                                commandTextString += newEntry.Columns[newEntry.Columns.Count - 1] + " integer);";//last point
                            }
                            break;
                        default:
                            {
                                commandTextString += newEntry.Columns[newEntry.Columns.Count - 1] + " double);";//last point
                            }
                            break;

                    }
                    //commandTextString += newEntry.Columns[newEntry.Columns.Count - 1] + " double);";//last point
                    
                    


                    command.CommandText = commandTextString;
                    command.ExecuteNonQuery();

                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            transaction.Commit();
                            //didThisWork = true;
                        }

                    #endregion

                    ///Now we have a table

                    #region Create an Index //this should be moved to the end so that we index after it is fully written

                    //if (newEntry.DatabaseTransferObjects[0].IndexedColumns.Count > 0)
                    //{
                    //    List<string> indexheaders = newEntry.DatabaseTransferObjects[0].IndexedColumns;
                    //        //CREATE INDEX idx_ex1 ON ex1(a,b,c,d,e,...,y,z);
                    //    //IF INDEXPROPERTY ( OBJECT_ID('your_table') , 'your_table_index' , 'IndexID' ) IS NULL"

                    //    //commandTextString = "CREATE INDEX  IF NOT EXISTS idx_" + newEntry.TableName + " ON " + newEntry.TableName + "(";
                    //    commandTextString = "CREATE INDEX  IF NOT EXISTS idx_" + newEntry.TableName;

                    //    for (int i = 0; i < indexheaders.Count-1; i++)
                    //    {
                    //        commandTextString += "_" + indexheaders[i] + "_";
                    //    }
                    //    commandTextString += indexheaders[indexheaders.Count - 1];

                    //    commandTextString += " ON " + newEntry.TableName + "(";
          

                    //    for (int i = 0; i < indexheaders.Count - 1; i++)
                    //    {
                    //        commandTextString += indexheaders[i] + ",";
                    //    }

                    //    commandTextString += indexheaders[indexheaders.Count - 1] + ");";

                    //    command.CommandText = commandTextString;
                    //    command.ExecuteNonQuery();

                    //    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    //    {
                    //        transaction.Commit();
                    //        //didThisWork = true;
                    //    }

                    //}

                    ///Now we have a table with an index
                    #endregion

                    #region insert values into table
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            #region set command string for inserts

                            //command.CommandText = "INSERT INTO FeatureLiteTable values (@ID, @Abundance, @ChargeState, @DriftTime, @MonoisotopicMass, @RetentionTime, @Score)";
                            //commandTextString = "INSERT INTO FeatureLiteTable values (";
                            commandTextString = "INSERT INTO " + newEntry.TableName + " values (";
                            for (int i = 0; i < newEntry.Columns.Count - 1; i++)
                            {
                                commandTextString += "@" + newEntry.Columns[i] + ",";
                            }
                            commandTextString += "@" + newEntry.Columns[newEntry.Columns.Count - 1] + ")";
                            command.CommandText = commandTextString;

                            #endregion

                            #region pile up parameters from each object in list to write

                            for (int item = 0; item < newEntry.DatabaseTransferObjects.Count; item++)
                            {
                                for (int i = 0; i < newEntry.ValuesTypes.Count; i++)
                                {
                                    DbParameter parameterNumber = command.CreateParameter();
                                    parameterNumber.ParameterName = newEntry.Columns[i];
                                    parameterNumber.DbType = newEntry.ValuesTypes[i];// DbType.Double;
                                    parameterNumber.Value = newEntry.DatabaseTransferObjects[item].Values[i];
                                    command.Parameters.Add(parameterNumber);
                                    command.Transaction = transaction;
                                }

                                command.ExecuteNonQuery();
                            }

                            #endregion

                            #region Commit once per list

                            transaction.Commit();

                            didThisWork = true;

                            #endregion
                        }
                    }
                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }

                    #endregion

                    #region old (off)
                    //try
                    //{
                    //command.ExecuteNonQuery();
                    //    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    //    {
                    //        #region set command string for inserts
                    //        //command.CommandText = "INSERT INTO FeatureLiteTable values (@ID, @Abundance, @ChargeState, @DriftTime, @MonoisotopicMass, @RetentionTime, @Score)";
                    //        //commandTextString = "INSERT INTO FeatureLiteTable values (";
                    //        commandTextString = "INSERT INTO " + DTOforColumns.TableName + " values (";
                    //        for (int i = 0; i < DTOforColumns.Columns.Count - 1; i++)
                    //        {
                    //            commandTextString += "@" + DTOforColumns.Columns[i] + ",";
                    //        }
                    //        commandTextString += "@" + DTOforColumns.Columns[DTOforColumns.Columns.Count - 1] + ")";

                    //        #endregion

                    //        command.CommandText = commandTextString;

                    //        #region setup columns and add data to each column

                    //        for (int i = 0; i < newEntry.ValuesTypes.Count; i++)
                    //        {
                    //            DbParameter parameterNumber = command.CreateParameter();
                    //            parameterNumber.ParameterName = DTOforColumns.Columns[i];
                    //            parameterNumber.DbType = DTOforColumns.ValuesTypes[i];// DbType.Double;
                    //            parameterNumber.Value = DTOforColumns.Values[i];
                    //            command.Parameters.Add(parameterNumber);
                    //        }

                    //        for (int i = 0; i < limit; i++)
                    //        {
                    //            command.ExecuteNonQuery();
                    //        }

                    //        #endregion

                    //        transaction.Commit();

                    //        didThisWork = true;
                    //    }
                    //}
                    //catch (SQLiteException exception)
                    //{
                    //    didThisWork = false;
                    //    Console.WriteLine("Failed :" + exception.Message);
                    //}
                    //finally
                    //{
                    //    connection.Close();
                    //}
                    #endregion

                    #endregion
                }

                connection.Close();
            }
            return didThisWork;
        }

        /// <summary>
        /// currently the best way to do this
        /// </summary>
        /// <param name="newEntry"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool WriteDataTransferObjectIndex(List<string> indexes, string tableName, string fileName)
        {
            bool didThisWork = false;

            string connectionString = "";
            string commandTextString = "";
            connectionString = "Data Source=" + fileName;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand())
                {
                    #region inside

                    command.Connection = connection;

                        #region Create an Index //this should be moved to the end so that we index after it is fully written

                        if (indexes.Count > 0)
                        {
                            foreach (string indexheaders in indexes)
                            {
                                commandTextString = "CREATE INDEX  IF NOT EXISTS idx_" + tableName;

                                List<string> columns = indexheaders.Split(',').ToList();
                                foreach (string word in columns)
                                {
                                    commandTextString += "_" + word;
                                }

                                commandTextString += " ON " + tableName + "(";


                                commandTextString += indexheaders + ");";

                                command.CommandText = commandTextString;
                                command.ExecuteNonQuery();

                                using (SQLiteTransaction transaction = connection.BeginTransaction())
                                {
                                    transaction.Commit();
                                    didThisWork = true;
                                }
                            }
                            //List<string> indexheaders = indexes;
                            //CREATE INDEX idx_ex1 ON ex1(a,b,c,d,e,...,y,z);
                            //IF INDEXPROPERTY ( OBJECT_ID('your_table') , 'your_table_index' , 'IndexID' ) IS NULL"

                            //commandTextString = "CREATE INDEX  IF NOT EXISTS idx_" + newEntry.TableName + " ON " + newEntry.TableName + "(";
                            

                        }

                        ///Now we have a table with an index
                        #endregion

                    #endregion
                }

                connection.Close();
            }
            return didThisWork;
        }

        /// <summary>
        /// currently the best way to do this also
        /// </summary>
        /// <param name="newEntry"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool UpdateDataTransferObjectList(DatabaseTransferObjectList newEntry, string fileName)
        {
            bool didThisWork = false;

            string connectionString = "";
            string commandTextString = "";
            connectionString = "Data Source=" + fileName;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand())
                {
                    #region inside

                    int limit = 1;//one at a time

                    #region set command for column names and create table

                    command.Connection = connection;
                    //command.CommandText = "CREATE TABLE IF NOT EXISTS FeatureLiteTable (ID double, Abundance double, ChargeState double, DriftTime double, MonoisotopicMass double, RetentionTime double , Score double);";
                    //commandTextString = "CREATE TABLE IF NOT EXISTS FeatureLiteTable (";
                    commandTextString = "CREATE TABLE IF NOT EXISTS " + newEntry.TableName + " (";

                    for (int i = 0; i < newEntry.Columns.Count - 1; i++)
                    {
                        switch (newEntry.ValuesTypes[i])
                        {
                            case DbType.Double:
                                {
                                    commandTextString += newEntry.Columns[i] + " double,";
                                }
                                break;
                            case DbType.String:
                                {
                                    commandTextString += newEntry.Columns[i] + " string,";
                                }
                                break;
                            case DbType.Int32:
                                {
                                    commandTextString += newEntry.Columns[i] + " integer,";
                                }
                                break;
                            default:
                                {
                                    commandTextString += newEntry.Columns[i] + " double,";
                                }
                                break;

                        }
                    }
                    //last point
                    switch (newEntry.ValuesTypes[newEntry.Columns.Count - 1])
                    {
                        case DbType.Double:
                            {
                                commandTextString += newEntry.Columns[newEntry.Columns.Count - 1] + " double);";//last point
                            }
                            break;
                        case DbType.String:
                            {
                                commandTextString += newEntry.Columns[newEntry.Columns.Count - 1] + " string);";//last point
                            }
                            break;
                        case DbType.Int32:
                            {
                                commandTextString += newEntry.Columns[newEntry.Columns.Count - 1] + " integer);";//last point
                            }
                            break;
                        default:
                            {
                                commandTextString += newEntry.Columns[newEntry.Columns.Count - 1] + " double);";//last point
                            }
                            break;

                    }
                    //commandTextString += newEntry.Columns[newEntry.Columns.Count - 1] + " double);";//last point




                    command.CommandText = commandTextString;
                    command.ExecuteNonQuery();

                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        transaction.Commit();
                        //didThisWork = true;
                    }

                    #endregion

                    ///Now we have a table

                    #region Create an Index //this should be moved to the end so that we index after it is fully written

                    if (newEntry.DatabaseTransferObjects[0].IndexedColumns.Count > 0)
                    {
                        List<string> indexheaders = newEntry.DatabaseTransferObjects[0].IndexedColumns;
                        //CREATE INDEX idx_ex1 ON ex1(a,b,c,d,e,...,y,z);
                        //IF INDEXPROPERTY ( OBJECT_ID('your_table') , 'your_table_index' , 'IndexID' ) IS NULL"

                        //commandTextString = "CREATE INDEX  IF NOT EXISTS idx_" + newEntry.TableName + " ON " + newEntry.TableName + "(";
                        commandTextString = "CREATE INDEX  IF NOT EXISTS idx_" + newEntry.TableName;

                        for (int i = 0; i < indexheaders.Count - 1; i++)
                        {
                            commandTextString += "_" + indexheaders[i] + "_";
                        }
                        commandTextString += indexheaders[indexheaders.Count - 1];

                        commandTextString += " ON " + newEntry.TableName + "(";


                        for (int i = 0; i < indexheaders.Count - 1; i++)
                        {
                            commandTextString += indexheaders[i] + ",";
                        }

                        commandTextString += indexheaders[indexheaders.Count - 1] + ");";

                        command.CommandText = commandTextString;
                        command.ExecuteNonQuery();

                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            transaction.Commit();
                            //didThisWork = true;
                        }

                    }

                    ///Now we have a table with an index
                    #endregion

                    #region insert values into table
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            #region set command string for inserts

                            //command.CommandText = 
                            //UPDATE T_Attribute_Centric 
                            //SET ID = @ID, isSignal = @isSignal, isNoise = @isNoise, isCentroided = @isCentroided, isMonoisotopic = @isMonoisotopic, isIsotope = @isIsotope, isMostAbundant = @isMostAbundant, isCharged = @isCharged, isCorrected = @isCorrected, isPrecursorMass = @isPrecursorMass 
                            //WHERE ID = @ID;"

                            commandTextString = "UPDATE " + newEntry.TableName + " SET ";
                            for (int i = 0; i < newEntry.Columns.Count - 1; i++)
                            {
                                commandTextString += newEntry.Columns[i] + " = @" + newEntry.Columns[i] + ", ";
                            }
                            commandTextString += newEntry.Columns[newEntry.Columns.Count - 1] + " = @" + newEntry.Columns[newEntry.Columns.Count - 1] + "";

                            //commandTextString += " WHERE PeakID = @PeakID;";//THIS IS A PROBLEM FOR TABLES WIHOUT A PEAK ID
                            commandTextString += " WHERE " + newEntry.Columns[0] + " = @" + newEntry.Columns[0] + ";";//THIS ASSUMES id USED IS IN SPOT 0
                            command.CommandText = commandTextString;

                            #endregion

                            #region pile up parameters from each object in list to write

                            for (int item = 0; item < newEntry.DatabaseTransferObjects.Count; item++)
                            {
                                for (int i = 0; i < newEntry.ValuesTypes.Count; i++)
                                {
                                    DbParameter parameterNumber = command.CreateParameter();
                                    parameterNumber.ParameterName = newEntry.Columns[i];
                                    parameterNumber.DbType = newEntry.ValuesTypes[i];// DbType.Double;
                                    parameterNumber.Value = newEntry.DatabaseTransferObjects[item].Values[i];
                                    command.Parameters.Add(parameterNumber);
                                    command.Transaction = transaction;
                                }

                                command.ExecuteNonQuery();
                            }

                            #endregion

                            #region Commit once per list

                            transaction.Commit();

                            didThisWork = true;

                            #endregion
                        }
                    }
                    catch (SQLiteException exception)
                    {
                        didThisWork = false;
                        Console.WriteLine("Failed :" + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }

                    #endregion

                    #endregion
                }

                connection.Close();
            }
            return didThisWork;
        }


        #endregion

        #region (off)
        //public bool GetSpectrum(int spectraID, int scanNum, ref double[] mz, ref float[] intensities, int peaksCount)
        //{
        //    bool success = false;
        //    try
        //    {
        //        // insert parameters into statement
        //        m_getSpectrumCommand.Parameters.Add(new SQLiteParameter(YafmsConstants.SPECTRAID, spectraID));
        //        m_getSpectrumCommand.Parameters.Add(new SQLiteParameter(YafmsConstants.SCANNUMBER, scanNum));

        //        m_dataReaderYafms = m_getSpectrumCommand.ExecuteReader();
        //        while (m_dataReaderYafms.Read())
        //        {
        //            mz = ConvertMZ((byte[])(m_dataReaderYafms[YafmsConstants.MZ]), peaksCount);
        //            intensities = ConvertIntensities((byte[])(m_dataReaderYafms[YafmsConstants.INTENSITIES]), peaksCount);

        //            success = true;
        //        }

        //        // clean up and return and return
        //        m_dataReaderYafms.Dispose();
        //        m_getSpectrumCommand.Parameters.Clear();

        //    }
        //    catch (Exception e)
        //    {
        //    }
        //    return success;
        //}
        #endregion 

        private static string ConvertColumnNamesToCommaString(List<string> fieldName)
        {
            string commandTextStringGetCount = "";

            for (int i = 0; i < fieldName.Count - 1; i++)
            {
                commandTextStringGetCount += fieldName[i] + ", ";
            }
            commandTextStringGetCount += fieldName[fieldName.Count - 1];

            return commandTextStringGetCount;
        }


    }
}
