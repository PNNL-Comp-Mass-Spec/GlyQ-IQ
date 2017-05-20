using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using YAFMS_DB.GetPeaks;

namespace YAFMS_DB.Reader
{
    public class DatabaseLayerYAFMSDB
    {
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

            string selectStatment = "";
            selectStatment += fieldNamePeaks[0] + ", ";
            selectStatment += fieldNamePeaks[2] + ", ";
            selectStatment += fieldNamePeaks[3];
            
            //width
            //selectStatment += fieldNamePeaks[4];
           
            commandTextStringGetCount += selectStatment + " ";

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

                                //loadedPeakCentric.PeakData.ScanNumber = Convert.ToInt32(databaseReader["TP_ScanID"]);
                                loadedPeakCentric.PeakData.ScanNumber = scanNumber;
                                //loadedPeakCentric.PeakData.ScanNumber = Convert.ToInt32(databaseReader["TS_ScanID"]);
                                loadedPeakCentric.PeakData.XValue = Convert.ToDouble(databaseReader["TP_Mz"]);
                                loadedPeakCentric.PeakData.Height = Convert.ToDouble(databaseReader["TP_Height"]);
                                //loadedPeakCentric.PeakData.Width = (float)Convert.ToDouble(databaseReader["TP_Width"]);
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
                    }
                }
                connection.Close();
            }

            //return results;
            //return didThisWork;
        }

        public void SK2_Final_SelectProcessedPeaks(string fileName, string tablenamePeaks, string tablenameScans, int scanNumber, DatabaseTransferObject sampleObjectPeakCentric, DatabaseTransferObject sampleObjectScanCentric, out DatabasePeakCentricObjectList resultPeaks, bool isMono)
        {
            string aliasTable1Peaks = "TP";//tableNamepeaks
            string aliasTable2Scans = "TS";//tableNamepeaks

            DatabasePeakCentricObject currentSampleObjectPeaks = (DatabasePeakCentricObject)sampleObjectPeakCentric;
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

            string selectStatment = "";
            //selectStatment += fieldNameScans[2] + ", ";//ScanNumLc
            selectStatment += fieldNamePeaks[5] + ", ";//Mz
            selectStatment += fieldNamePeaks[7] + ", ";//Height
            selectStatment += fieldNamePeaks[8] + ", ";//Width
            selectStatment += fieldNamePeaks[14] + ", ";//isSignal
            selectStatment += fieldNamePeaks[15];//isCentroided

            //width
            //selectStatment += fieldNamePeaks[4];

            commandTextStringGetCount += selectStatment + " ";

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

            resultPeaks = new DatabasePeakCentricObjectList();

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

                                //loadedPeakCentric.PeakData.ScanNumber = Convert.ToInt32(databaseReader["TP_ScanID"]);
                                loadedPeakCentric.PeakCentricData.ScanNumber = scanNumber;
          //                      loadedPeakCentric.PeakCentricData.ScanNumber = Convert.ToInt32(databaseReader["TS_ScanNumLc"]); ;
                                //loadedPeakCentric.PeakData.ScanNumber = Convert.ToInt32(databaseReader["TS_ScanID"]);
                                loadedPeakCentric.PeakCentricData.XValue = Convert.ToDouble(databaseReader["TP_Mz"]);
                                loadedPeakCentric.PeakCentricData.Height = Convert.ToDouble(databaseReader["TP_Height"]);
                                loadedPeakCentric.PeakCentricData.Width = (float)Convert.ToDouble(databaseReader["TP_Width"]);
                                //loadedPeakCentric. = (float)Convert.ToDouble(databaseReader["TP_Width"]);
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
                    }
                }
                connection.Close();
            }

            //return results;
            //return didThisWork;
        }


        public void SK2_Final_SelectEIC(string fileName, string tablenamePeaks, string tablenameScans, double lowerMass, double upperMass, DatabaseTransferObject sampleObjectPeakCentric, DatabaseTransferObject sampleObjectScanCentric, out DatabasePeakCentricLiteObjectList resultPeaks)
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

            //set up variables
            string variableLowerMass = "@lowerMass";//column matches to input scanNumber.
            string variableUpperMass = "@upperMass";//column matches to input scanNumber.


            //Set up SQLite statement

            string commandTextStringGetCount;

            commandTextStringGetCount = "SELECT ";//+ "Scan, PeakNumber, XValue" + " ";

            string selectStatment = "";
            //selectStatment += "MIN(" + aliasTable1Peaks + "." + currentSampleObjectPeaks.Columns[1] + ") AS " + "'" + aliasTable1Peaks + "_" + currentSampleObjectPeaks.Columns[1] + "'" + ", ";
            selectStatment += aliasTable2Scans + "." + currentSampleObjectScans.Columns[2] + " AS " + "'" + aliasTable2Scans + "_" + currentSampleObjectScans.Columns[2] + "'" + ", ";
            selectStatment += "MAX(" + aliasTable1Peaks + "." + currentSampleObjectPeaks.Columns[3] + ") AS " + "'" + aliasTable1Peaks + "_" + currentSampleObjectPeaks.Columns[3] + "'" ;

            commandTextStringGetCount += selectStatment + " ";

            commandTextStringGetCount += "FROM " + tablenamePeaks + " " + aliasTable1Peaks + " ";//where alias is connected
            commandTextStringGetCount += "INNER JOIN " + tablenameScans + " " + aliasTable2Scans + " ";//where alias is connected
            commandTextStringGetCount += "ON " + aliasTable1Peaks + "." + currentSampleObjectPeaks.Columns[1] + " = " + aliasTable2Scans + "." + currentSampleObjectScans.Columns[0] + " ";

            commandTextStringGetCount += "WHERE " + aliasTable1Peaks + "." + currentSampleObjectPeaks.Columns[2] + " > " + variableLowerMass + " ";
            commandTextStringGetCount += "AND " + aliasTable1Peaks + "." + currentSampleObjectPeaks.Columns[2] + " < " + variableUpperMass + " ";

            commandTextStringGetCount += "GROUP BY " + aliasTable2Scans + "." + currentSampleObjectScans.Columns[2] +";";

            #region querry as Text
            //    SELECT Min(TP.scanid) AS 'TP_ScanID', 
            //       TS.scannumlc   AS 'TS_ScanNumLc', 
            //       Max(TP.height) AS 'TP_Height' 
            //FROM   t_peak_centric TP 
            //       INNER JOIN t_scan_centric TS 
            //               ON TP.scanid = TS.scanid 
            //WHERE  TP.mz > 280 
            //       AND TP.mz < 282 
            //GROUP  BY TS.scannumlc;  

            //    SELECT Min(TP.scanid) AS 'TP_ScanID', 
            //       TS.scannumlc   AS 'TS_ScanNumLc', 
            //       Max(TP.height) AS 'TP_Height' 
            //FROM   t_peak_centric TP 
            //       INNER JOIN t_scan_centric TS 
            //               ON TP.scanid = TS.scanid 
            //WHERE  TP.mz > @lowerMass 
            //       AND TP.mz < @upperMass 
            //GROUP  BY TS.scannumlc;  
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
                    SQLiteParameter newSqliteParamaterLowerMass = new SQLiteParameter(variableLowerMass, lowerMass);
                    SQLiteParameter newSqliteParamaterUpperMass = new SQLiteParameter(variableUpperMass, upperMass);
                    newSqliteParamaterLowerMass.DbType = DbType.Double;
                    newSqliteParamaterUpperMass.DbType = DbType.Double;

                    getIsotopesCommmnd.CreateParameter();
                    getIsotopesCommmnd.CreateParameter();
                    
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamaterLowerMass);
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamaterUpperMass);

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

                                //loadedPeakCentric.PeakData.ScanNumber = Convert.ToInt32(databaseReader["TP_ScanID"]);
                                loadedPeakCentric.PeakData.ScanNumber = Convert.ToInt32(databaseReader["TS_ScanNumLc"]);
                                loadedPeakCentric.PeakData.Height = Convert.ToDouble(databaseReader["TP_Height"]);
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
                    }
                }
                connection.Close();
            }

            //return results;
            //return didThisWork;
        }

    }


}
