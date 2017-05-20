using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using System.Data.SQLite;
using System.Data.Common;
using DeconTools.Backend.Core;
using GetPeaks_DLL.Objects;
using PNNLOmics.Data;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Objects.TandemMSObjects;
using GetPeaks_DLL.SQLite.DataTransferObjects;

namespace GetPeaks_DLL.SQLite
{
    /// <summary>
    /// 1.  create a DatabaseGenericObject.cs that has the desited return object properties
    /// 2.  create a readGenericData(string fileName, int index, out DatabaseGenericObject loadedGenericObject)
    /// 3.  create a LoadedDataToTandemObject() below
    /// 4.  create a precursor page
    /// 5.  create a m_GetGenericCommmndString 
    /// 6.  create a loader LoadGenericData.cs
    /// 7.  create a public List<Generic> GetDataGeneric(InputOutputFileName dataFile) in GetDataController.cs
       
    /// </summary>
    public class DatabaseReader
    {
        string m_GetFeatureCommmndString { get; set; }
        List<string> m_FeatureLightColumnList { get; set; }

        string m_GetFeatureMultiAlignCommmndString { get; set; }
        List<string> m_FeatureMultiAlignColumnList { get; set; }

        string m_GetElutingPeakCommmndString { get; set; }
        List<string> m_ElutingPeakColumnList { get; set; }

        string m_GetIsotopesCommmndString { get; set; }
        List<string> m_IsotopeColumnList { get; set; }

        string m_GetMonoIsotopesCommmndString { get; set; }
        List<string> m_MonoIsotopeColumnList { get; set; }

        string m_GetAllMonoIsotopesCommmndString { get; set; }
        List<string> m_AllMonoIsotopeColumnList { get; set; }

        string m_GetTandemCommmndString { get; set; }
        List<string> m_TandemColumnList { get; set; }

        string m_GetAllPeaksCommmndString { get; set; }
        List<string> m_T_Scan_PeaksColumnList { get; set; }

        public DatabaseReader()
        {

            #region AllPeaks page

            DatabasePeakProcessedObject sampleObjectForAllPeaks = new DatabasePeakProcessedObject();

            string T_Scan_PeaksTableName = "T_Scan_Peaks";
            m_T_Scan_PeaksColumnList = sampleObjectForAllPeaks.Columns;

            string commandTextStringGetAllPeaks_Select = "";
            string commandTextStringGetAllPeaks_From = "";
            string commandTextStringGetAllPeaks_Where = "";
            #region set string
            //m_GetFeatureCommmndString = "SELECT ID, Abundance, ChargeState, DriftTime, MonoisotopicMass, RetentionTime, Score FROM FeatureLiteTable WHERE rowid = ? ";//(ID double, Abundance double, ChargeState double, DriftTime double, myMassMonoisotopicColumn double, myRetentionTimeColumn double , myScoreColumn double);";
            commandTextStringGetAllPeaks_Select = "SELECT ";
            for (int i = 0; i < m_T_Scan_PeaksColumnList.Count - 1; i++)
            {
                commandTextStringGetAllPeaks_Select += m_T_Scan_PeaksColumnList[i] + ",";
            }
            commandTextStringGetAllPeaks_Select += m_T_Scan_PeaksColumnList[m_T_Scan_PeaksColumnList.Count - 1];

            commandTextStringGetAllPeaks_From = " FROM " + T_Scan_PeaksTableName;

            //commandTextStringAllMonoIsotope_Where = " WHERE rowid = ? ";//last point

            commandTextStringGetAllPeaks_Where = " WHERE scan_num >= :minScan AND scan_num <= :maxScan";

            m_GetAllPeaksCommmndString = commandTextStringGetAllPeaks_Select + commandTextStringGetAllPeaks_From + commandTextStringGetAllPeaks_Where;
            #endregion

            m_GetAllPeaksCommmndString = m_GetAllPeaksCommmndString;
            #endregion
            
            #region feature light page
            List<string> columnListFL = new List<string>();
            columnListFL.Add("ID");
            columnListFL.Add("Abundance");
            columnListFL.Add("ChargeState");
            columnListFL.Add("DriftTime");
            columnListFL.Add("MonoisotopicMass");
            columnListFL.Add("RetentionTime");
            columnListFL.Add("Score");

            m_FeatureLightColumnList = columnListFL;

            string commandTextStringFeatureLight = "";

            #region set string
            //m_GetFeatureCommmndString = "SELECT ID, Abundance, ChargeState, DriftTime, MonoisotopicMass, RetentionTime, Score FROM FeatureLiteTable WHERE rowid = ? ";//(ID double, Abundance double, ChargeState double, DriftTime double, myMassMonoisotopicColumn double, myRetentionTimeColumn double , myScoreColumn double);";
            commandTextStringFeatureLight = "SELECT ";
            for (int i = 0; i < columnListFL.Count - 1; i++)
            {
                commandTextStringFeatureLight += columnListFL[i] + ",";
            }
            commandTextStringFeatureLight += columnListFL[columnListFL.Count - 1] + " FROM FeatureLiteTable WHERE rowid = ? ";//last point
            #endregion
            m_GetFeatureCommmndString = commandTextStringFeatureLight;
            #endregion

            #region Eluting peak page
            List<string> columnListEP = new List<string>();
            columnListEP.Add("ElutingPeakID");
            columnListEP.Add("ElutingPeakMass");
            columnListEP.Add("ElutingPeakScanStart");
            columnListEP.Add("ElutingPeakScanEnd");
            columnListEP.Add("ElutingPeakScanMaxIntensity");
            columnListEP.Add("ElutingPeakNumberofPeaks");
            columnListEP.Add("ElutingPeakNumberOfPeaksFlag");
            columnListEP.Add("ElutingPeakNumberOfPeaksMode");
            columnListEP.Add("ElutingPeakSummedIntensity");
            columnListEP.Add("ElutingPeakIntensityAggregate");

            m_ElutingPeakColumnList = columnListEP;

            string commandTextStringElutingPeak = "";

            #region set string
            //m_GetFeatureCommmndString = "SELECT ID, Abundance, ChargeState, DriftTime, MonoisotopicMass, RetentionTime, Score FROM FeatureLiteTable WHERE rowid = ? ";//(ID double, Abundance double, ChargeState double, DriftTime double, myMassMonoisotopicColumn double, myRetentionTimeColumn double , myScoreColumn double);";
            commandTextStringElutingPeak = "SELECT ";
            for (int i = 0; i < columnListEP.Count - 1; i++)
            {
                commandTextStringElutingPeak += columnListEP[i] + ",";
            }
            commandTextStringElutingPeak += columnListEP[columnListFL.Count - 1] + " FROM ElutingPeakTable WHERE rowid = ? ";//last point
            #endregion
            m_GetElutingPeakCommmndString = commandTextStringElutingPeak;
            #endregion

            #region isotopes page
            List<string> columnListIsotope = new List<string>();
            columnListIsotope.Add("MonoisotopicMass");
            columnListIsotope.Add("ExperimentMass");
            columnListIsotope.Add("IsotopeMasses");
            columnListIsotope.Add("IsotopeIntensities");

            m_IsotopeColumnList = columnListIsotope;

            string commandTextStringIsotope = "";

            #region set string
            //m_GetFeatureCommmndString = "SELECT ID, Abundance, ChargeState, DriftTime, MonoisotopicMass, RetentionTime, Score FROM FeatureLiteTable WHERE rowid = ? ";//(ID double, Abundance double, ChargeState double, DriftTime double, myMassMonoisotopicColumn double, myRetentionTimeColumn double , myScoreColumn double);";
            commandTextStringIsotope = "SELECT ";
            for (int i = 0; i < columnListIsotope.Count - 1; i++)
            {
                commandTextStringIsotope += columnListIsotope[i] + ",";
            }
            commandTextStringIsotope += columnListIsotope[columnListIsotope.Count - 1] + " FROM IsotopeTable WHERE rowid = ? ";//last point
            #endregion
            m_GetIsotopesCommmndString = commandTextStringIsotope;
            #endregion

            #region MonoIsotopes page
            string monoisotopeTableName = "IsostopeTable";


            DatabaseIsotopeObject sampleObject = new DatabaseIsotopeObject();

            List<string> columnListMonoIsotope = new List<string>();
            foreach (string column in sampleObject.Columns)
            {
                columnListMonoIsotope.Add(column);
            }

            m_MonoIsotopeColumnList = columnListMonoIsotope;

            string commandTextStringMonoIsotope = "";

            #region set string
            //m_GetFeatureCommmndString = "SELECT ID, Abundance, ChargeState, DriftTime, MonoisotopicMass, RetentionTime, Score FROM FeatureLiteTable WHERE rowid = ? ";//(ID double, Abundance double, ChargeState double, DriftTime double, myMassMonoisotopicColumn double, myRetentionTimeColumn double , myScoreColumn double);";
            commandTextStringMonoIsotope = "SELECT ";
            for (int i = 0; i < columnListMonoIsotope.Count - 1; i++)
            {
                commandTextStringMonoIsotope += columnListMonoIsotope[i] + ",";
            }
            commandTextStringMonoIsotope += columnListMonoIsotope[columnListMonoIsotope.Count - 1] + " FROM " + monoisotopeTableName + " WHERE rowid = ? ";//last point
            #endregion

            m_GetMonoIsotopesCommmndString = commandTextStringMonoIsotope;
            #endregion

            #region AllMonoIsotopes page
            monoisotopeTableName = "IsostopeTable";
            m_AllMonoIsotopeColumnList = m_MonoIsotopeColumnList;

            string commandTextStringAllMonoIsotope_Select = "";
            string commandTextStringAllMonoIsotope_From = "";
            string commandTextStringAllMonoIsotope_Where = "";
            #region set string
            //m_GetFeatureCommmndString = "SELECT ID, Abundance, ChargeState, DriftTime, MonoisotopicMass, RetentionTime, Score FROM FeatureLiteTable WHERE rowid = ? ";//(ID double, Abundance double, ChargeState double, DriftTime double, myMassMonoisotopicColumn double, myRetentionTimeColumn double , myScoreColumn double);";
            commandTextStringAllMonoIsotope_Select = "SELECT ";
            for (int i = 0; i < columnListMonoIsotope.Count - 1; i++)
            {
                commandTextStringAllMonoIsotope_Select += columnListMonoIsotope[i] + ",";
            }
            commandTextStringAllMonoIsotope_Select += columnListMonoIsotope[columnListMonoIsotope.Count - 1];

            commandTextStringAllMonoIsotope_From = " FROM " + monoisotopeTableName;

            //commandTextStringAllMonoIsotope_Where = " WHERE rowid = ? ";//last point

            commandTextStringAllMonoIsotope_Where = " WHERE scan_num >= :minScan AND scan_num <= :maxScan";

            m_GetAllMonoIsotopesCommmndString = commandTextStringAllMonoIsotope_Select + commandTextStringAllMonoIsotope_From + commandTextStringAllMonoIsotope_Where;
            #endregion

            m_GetMonoIsotopesCommmndString = commandTextStringMonoIsotope;
            #endregion

            #region Tandem page
            List<string> columnListTandem = new List<string>();
            columnListTandem.Add("ScanNum");
            columnListTandem.Add("PrecursorScanNum");
            columnListTandem.Add("PrecursorMz");
            //columnListTandem.Add("ScanTime");

            m_TandemColumnList = columnListTandem;

            string commandTextStringTandem = "";

            #region set string
            //m_GetFeatureCommmndString = "SELECT ID, Abundance, ChargeState, DriftTime, MonoisotopicMass, RetentionTime, Score FROM FeatureLiteTable WHERE rowid = ? ";//(ID double, Abundance double, ChargeState double, DriftTime double, myMassMonoisotopicColumn double, myRetentionTimeColumn double , myScoreColumn double);";
            commandTextStringTandem = "SELECT ";
            for (int i = 0; i < columnListTandem.Count - 1; i++)
            {
                commandTextStringTandem += columnListTandem[i] + ",";
            }
            commandTextStringTandem += columnListTandem[columnListTandem.Count - 1] + " FROM Spectra_Data WHERE rowid = ? ";//last point
            #endregion
            m_GetTandemCommmndString = commandTextStringTandem;
            #endregion

            #region MultiAlign Page
            List<string> columnListFMAL = new List<string>();
            columnListFL.Add("ID");
            columnListFL.Add("Abundance");
            columnListFL.Add("ChargeState");
            columnListFL.Add("DriftTime");
            columnListFL.Add("MonoisotopicMass");
            columnListFL.Add("RetentionTime");
            columnListFL.Add("Score");

            m_FeatureLightColumnList = columnListFL;

            string commandTextStringFeatureMultiAlign = "";

            #region set string
            //m_GetFeatureCommmndString = "SELECT ID, Abundance, ChargeState, DriftTime, MonoisotopicMass, RetentionTime, Score FROM FeatureLiteTable WHERE rowid = ? ";//(ID double, Abundance double, ChargeState double, DriftTime double, myMassMonoisotopicColumn double, myRetentionTimeColumn double , myScoreColumn double);";
            commandTextStringFeatureMultiAlign = "SELECT ";
            for (int i = 0; i < columnListFL.Count - 1; i++)
            {
                commandTextStringFeatureMultiAlign += columnListFL[i] + ",";
            }
            commandTextStringFeatureMultiAlign += columnListFL[columnListFL.Count - 1] + " FROM FeatureLiteTable WHERE rowid = ? ";//last point
            #endregion
            m_GetFeatureCommmndString = commandTextStringFeatureMultiAlign;
            #endregion


        }

        public bool readFeatrueLiteData(string fileName, int index, out FeatureLight loadedFeature)
        {
            if (index == 0)
            {
                Console.WriteLine("must be non zero");
                Console.ReadKey();
                Console.WriteLine("press a button to close");
                 
            }
            
            bool didThisWork = false;

            string connectionString = "";
            connectionString = "Data Source=" + fileName;
            
            int ID = 0;
            Int64 abundance = 0;
            int chargeState = 0;
            double driftTime = 0;
            double monoisotopicMass = 0;
            double retentionTime = 0;
            float score = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getFeatureCommand = connection.CreateCommand())
                {
                    getFeatureCommand.CommandText = m_GetFeatureCommmndString;
                    getFeatureCommand.Prepare();

                    SQLiteParameter newSqliteParamater = new SQLiteParameter("rowid", index);
                    getFeatureCommand.Parameters.Add(newSqliteParamater);
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getFeatureCommand.ExecuteReader();
                            databaseReader.Read();

                            ID = Convert.ToInt32(databaseReader[m_FeatureLightColumnList[0]]);
                            abundance = Convert.ToInt64(databaseReader[m_FeatureLightColumnList[1]]);
                            chargeState = Convert.ToInt32(databaseReader[m_FeatureLightColumnList[2]]);
                            driftTime = Convert.ToDouble(databaseReader[m_FeatureLightColumnList[3]]);
                            monoisotopicMass = Convert.ToDouble(databaseReader[m_FeatureLightColumnList[4]]);
                            retentionTime = Convert.ToDouble(databaseReader[m_FeatureLightColumnList[5]]);
                            score = Convert.ToSingle(databaseReader[m_FeatureLightColumnList[6]]);

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
            }

            loadedFeature = LoadedDataToFeatureLite(ID, abundance, chargeState, driftTime = 0, monoisotopicMass, retentionTime, score);
        
            return didThisWork;
        }

        public bool readMultiAllignFeatrueData(string fileName, int index, out FeatureMultiAlign loadedFeature)
        {
            if (index == 0)
            {
                Console.WriteLine("must be non zero");
                Console.ReadKey();
                Console.WriteLine("press a button to close");

            }

            bool didThisWork = false;

            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            int id = 0;
            int dataset_ID = 0;
            int cluster_ID = 0;
            int conformation_ID = 0; 
            double mass = 0; 
            double mass_Calibrated = 0;
            double net = 0;
            double mz = 0;
            int scan_LC = 0;
            int scan_LC_Start = 0;
            int scan_LC_End = 0;
            int scan_LC_Aligned = 0;
            int charge = 0;
            double abundance_Max = 0;
            double abundance_Sum = 0;
            double drift_Time = 0;
            double average_Interference_Score = 0;
            double average_Decon_Fit_Score = 0;
            int UMC_Member_Count = 0;
            int saturated_Member_Count = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getFeatureCommand = connection.CreateCommand())
                {
                    getFeatureCommand.CommandText = m_GetFeatureCommmndString;
                    getFeatureCommand.Prepare();

                    SQLiteParameter newSqliteParamater = new SQLiteParameter("rowid", index);
                    getFeatureCommand.Parameters.Add(newSqliteParamater);
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getFeatureCommand.ExecuteReader();
                            databaseReader.Read();

                            id = Convert.ToInt32(databaseReader[m_FeatureLightColumnList[0]]);
                            dataset_ID = Convert.ToInt32(databaseReader[m_FeatureLightColumnList[1]]);
                            cluster_ID = Convert.ToInt32(databaseReader[m_FeatureLightColumnList[2]]);
                            conformation_ID = Convert.ToInt32(databaseReader[m_FeatureLightColumnList[3]]);
                            mass = Convert.ToDouble(databaseReader[m_FeatureLightColumnList[4]]);
                            mass_Calibrated = Convert.ToDouble(databaseReader[m_FeatureLightColumnList[5]]);
                            net = Convert.ToDouble(databaseReader[m_FeatureLightColumnList[6]]);
                            mz = Convert.ToDouble(databaseReader[m_FeatureLightColumnList[7]]);
                            scan_LC = Convert.ToInt32(databaseReader[m_FeatureLightColumnList[8]]);
                            scan_LC_Start = Convert.ToInt32(databaseReader[m_FeatureLightColumnList[9]]);
                            scan_LC_End = Convert.ToInt32(databaseReader[m_FeatureLightColumnList[10]]);
                            scan_LC_Aligned = Convert.ToInt32(databaseReader[m_FeatureLightColumnList[11]]);
                            charge = Convert.ToInt32(databaseReader[m_FeatureLightColumnList[12]]);
                            abundance_Max = Convert.ToDouble(databaseReader[m_FeatureLightColumnList[13]]);
                            abundance_Sum = Convert.ToDouble(databaseReader[m_FeatureLightColumnList[14]]);
                            drift_Time = Convert.ToDouble(databaseReader[m_FeatureLightColumnList[15]]);
                            average_Interference_Score = Convert.ToDouble(databaseReader[m_FeatureLightColumnList[16]]);
                            average_Decon_Fit_Score = Convert.ToDouble(databaseReader[m_FeatureLightColumnList[17]]);
                            UMC_Member_Count = Convert.ToInt32(databaseReader[m_FeatureLightColumnList[18]]);;
                            saturated_Member_Count = Convert.ToInt32(databaseReader[m_FeatureLightColumnList[19]]);

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
            }

            loadedFeature = LoadedDataToFeatureMultiAlign(id,dataset_ID,cluster_ID,conformation_ID,mass,mass_Calibrated,net,mz,scan_LC,scan_LC_Start,scan_LC_End,scan_LC_Aligned,charge,abundance_Max,abundance_Sum,drift_Time,average_Interference_Score,average_Decon_Fit_Score,UMC_Member_Count,saturated_Member_Count);

            return didThisWork;
        }

        public bool readElutingPeakdata(string fileName, int index, out ElutingPeakLite loadedElutingPeak)
        {
            bool didThisWork = false;

            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            int ElutingPeakID = 0;
            double ElutingPeakMass = 0;
            int ElutingPeakScanStart = 0;
            int ElutingPeakScanEnd = 0;
            int ElutingPeakScanMaxIntensity = 0;
            int ElutingPeakNumberofPeaks = 0;
            int ElutingPeakNumberOfPeaksFlag = 0;
            string ElutingPeakNumberOfPeaksMode ="";
            float ElutingPeakSummedIntensity = 0;
            double ElutingPeakIsosResultIntensityAggregate = 0;


            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getElutingPeakCommand = connection.CreateCommand())
                {
                    getElutingPeakCommand.CommandText = m_GetElutingPeakCommmndString;
                    getElutingPeakCommand.Prepare();

                    SQLiteParameter newSqliteParamater = new SQLiteParameter("rowid", index);
                    getElutingPeakCommand.Parameters.Add(newSqliteParamater);
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getElutingPeakCommand.ExecuteReader();
                            databaseReader.Read();

                            ElutingPeakID = Convert.ToInt32(databaseReader[m_ElutingPeakColumnList[0]]);
                            ElutingPeakMass = Convert.ToDouble(databaseReader[m_ElutingPeakColumnList[1]]);

                            ElutingPeakScanStart = Convert.ToInt32(databaseReader[m_ElutingPeakColumnList[2]]);
                            ElutingPeakScanEnd = Convert.ToInt32(databaseReader[m_ElutingPeakColumnList[3]]);
                            ElutingPeakScanMaxIntensity = Convert.ToInt32(databaseReader[m_ElutingPeakColumnList[4]]);

                            ElutingPeakNumberofPeaks = Convert.ToInt32(databaseReader[m_ElutingPeakColumnList[5]]);
                            ElutingPeakNumberOfPeaksFlag = Convert.ToInt32(databaseReader[m_ElutingPeakColumnList[6]]);
                            ElutingPeakNumberOfPeaksMode = Convert.ToString(databaseReader[m_ElutingPeakColumnList[7]]);

                            ElutingPeakSummedIntensity = Convert.ToSingle(databaseReader[m_ElutingPeakColumnList[8]]);
                            //Console.WriteLine(m_ElutingPeakColumnList[9]);
                            //ElutingPeakIsosResultIntensityAggregate = Convert.ToDouble(databaseReader[m_ElutingPeakColumnList[9]]);//TODO fix aggregate intensity

                            //Console.WriteLine(ElutingPeakID + " " + ElutingPeakMass + " " + ElutingPeakScanStart + " " + ElutingPeakScanEnd + " " + ElutingPeakScanMaxIntensity + " " + ElutingPeakNumberofPeaks + " " + ElutingPeakNumberOfPeaksFlag + " " + ElutingPeakNumberOfPeaksMode + " " + ElutingPeakSummedIntensity + " "+ ElutingPeakIsosResultIntensityAggregate);
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

            loadedElutingPeak = LoadedDataToElutingPeakLite(ElutingPeakID, ElutingPeakMass, ElutingPeakScanStart, ElutingPeakScanEnd, ElutingPeakScanMaxIntensity,
                ElutingPeakNumberofPeaks, ElutingPeakNumberOfPeaksFlag, ElutingPeakNumberOfPeaksMode, ElutingPeakSummedIntensity, ElutingPeakIsosResultIntensityAggregate);
            return didThisWork;
        }

        public bool readIsotopeDataOld(string fileName, int index, out IsotopeObject loadedIsotopeObject)
        {
            if (index == 0)
            {
                Console.WriteLine("must be non zero");
                Console.ReadKey();
                Console.WriteLine("press a button to close");

            }

            bool didThisWork = false;

            string connectionString = "";
            connectionString = "Data Source=" + fileName;

           
            double monoisotopicMass = 0;
            double experimentMass = 0;
            string isotopeMassString = "";
            string isotopeIntensityString = "";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    getIsotopesCommmnd.CommandText = m_GetIsotopesCommmndString;
                    getIsotopesCommmnd.Prepare();

                    SQLiteParameter newSqliteParamater = new SQLiteParameter("rowid", index);
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamater);
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();
                            databaseReader.Read();

                            monoisotopicMass = Convert.ToDouble(databaseReader[m_IsotopeColumnList[0]]);
                            experimentMass = Convert.ToDouble(databaseReader[m_IsotopeColumnList[1]]);
                            isotopeMassString = (string)databaseReader[m_IsotopeColumnList[2]];
                            isotopeIntensityString = Convert.ToString(databaseReader[m_IsotopeColumnList[3]]);

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
            }

            loadedIsotopeObject = LoadedDataToIsotopeObject(monoisotopicMass, experimentMass, isotopeMassString, isotopeIntensityString);

            return didThisWork;
        }

        public bool readMonoIsotopeData(string fileName, int index, out DatabaseIsotopeObject loadedMonoIsotopeObject)
        {
            if (index == 0)
            {
                Console.WriteLine("must be non zero");
                Console.ReadKey();
                Console.WriteLine("press a button to close");

            }

            bool didThisWork = false;

            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            loadedMonoIsotopeObject = new DatabaseIsotopeObject();
            
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    getIsotopesCommmnd.CommandText = m_GetMonoIsotopesCommmndString;
                    getIsotopesCommmnd.Prepare();

                    SQLiteParameter newSqliteParamater = new SQLiteParameter("rowid", index);
                    getIsotopesCommmnd.Parameters.Add(newSqliteParamater);
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();
                            databaseReader.Read();

                            loadedMonoIsotopeObject.MonoIsotopicMass = Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[0]]);
                            loadedMonoIsotopeObject.ExperimentMass = Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[1]]);
                            loadedMonoIsotopeObject.IsotopeMassesCSV = Convert.ToString(databaseReader[m_MonoIsotopeColumnList[2]]);
                            loadedMonoIsotopeObject.IsotopeIntensitiesCSV = Convert.ToString(databaseReader[m_MonoIsotopeColumnList[3]]);
                            loadedMonoIsotopeObject.scan_num = Convert.ToInt32(databaseReader[m_MonoIsotopeColumnList[4]]);
                            loadedMonoIsotopeObject.charge = Convert.ToInt32(databaseReader[m_MonoIsotopeColumnList[5]]);
                            loadedMonoIsotopeObject.abundance = Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[6]]);
                            loadedMonoIsotopeObject.mz = Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[7]]);
                            loadedMonoIsotopeObject.fit = Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[8]]);
                            loadedMonoIsotopeObject.average_mw = Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[9]]);
                            loadedMonoIsotopeObject.monoisotopic_mw = Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[10]]);
                            loadedMonoIsotopeObject.mostabundant_mw = Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[11]]);
                            loadedMonoIsotopeObject.fwhm = Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[12]]);
                            loadedMonoIsotopeObject.signal_noise = Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[13]]);
                            loadedMonoIsotopeObject.mono_abundance = Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[14]]);
                            loadedMonoIsotopeObject.mono_plus2_abundance = Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[15]]);
                            loadedMonoIsotopeObject.flag = Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[16]]);
                            loadedMonoIsotopeObject.interference_score = Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[17]]);
                            
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

            

            return didThisWork;
        }

        public bool readAllMonoIsotopeData(string fileName, int sizeOfFile, out List<DatabaseIsotopeObject> loadedMonoIsotopeObjectList)
        {
            //FrameType frameType
            int startFrameNumber = 0;
            int endFrameNumber = 1000;
            int startScan;
            int endScan;
            string fieldName = "";

            int nframes = endFrameNumber - startFrameNumber + 1;

            bool didThisWork = false;

            string connectionString = "";
            connectionString = "Data Source=" + fileName;

            loadedMonoIsotopeObjectList = new List<DatabaseIsotopeObject>();
            string Select = m_GetAllMonoIsotopesCommmndString;
            /*
            string sql = " SELECT Frame_Scans.FrameNum, Sum(Frame_Scans." + fieldName + ") AS Value " +
                         " FROM Frame_Scans INNER JOIN Frame_Parameters ON Frame_Scans.FrameNum = Frame_Parameters.FrameNum " +
                         " WHERE Frame_Parameters.FrameNum >= " + startFrameNumber + " AND " +
                               " Frame_Parameters.FrameNum <= " + endFrameNumber;
            */

            string arg = Select;
            //string arg2 = sql;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getIsotopesCommmnd = connection.CreateCommand())
                {
                    getIsotopesCommmnd.CommandText = m_GetAllMonoIsotopesCommmndString;
                    getIsotopesCommmnd.Prepare();
                    //SQLiteParameter newSqliteParamater = new SQLiteParameter("rowid", 1);
                    //for (int index = 1; index < 4; index++)
                    //{
                        //int index = 1;
                        //newSqliteParamater = new SQLiteParameter("rowid", index);
                        getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("minScan", 1));
                        getIsotopesCommmnd.Parameters.Add(new SQLiteParameter("maxScan", sizeOfFile));
                        
                        //getIsotopesCommmnd.CreateParameter();
                        //getIsotopesCommmnd.Parameters.Add(newSqliteParamater);

                    //}

                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getIsotopesCommmnd.ExecuteReader();
                            
                            while(databaseReader.Read())
                            {
                           
                                DatabaseIsotopeObject loadedMonoIsotopeObject = new DatabaseIsotopeObject();

                                loadedMonoIsotopeObject.MonoIsotopicMass =
                                    Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[0]]);
                                loadedMonoIsotopeObject.ExperimentMass =
                                    Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[1]]);
                                loadedMonoIsotopeObject.IsotopeMassesCSV =
                                    Convert.ToString(databaseReader[m_MonoIsotopeColumnList[2]]);
                                loadedMonoIsotopeObject.IsotopeIntensitiesCSV =
                                    Convert.ToString(databaseReader[m_MonoIsotopeColumnList[3]]);
                                loadedMonoIsotopeObject.scan_num =
                                    Convert.ToInt32(databaseReader[m_MonoIsotopeColumnList[4]]);
                                loadedMonoIsotopeObject.charge =
                                    Convert.ToInt32(databaseReader[m_MonoIsotopeColumnList[5]]);
                                loadedMonoIsotopeObject.abundance =
                                    Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[6]]);
                                loadedMonoIsotopeObject.mz = 
                                    Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[7]]);
                                loadedMonoIsotopeObject.fit =
                                    Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[8]]);
                                loadedMonoIsotopeObject.average_mw =
                                    Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[9]]);
                                loadedMonoIsotopeObject.monoisotopic_mw =
                                    Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[10]]);
                                loadedMonoIsotopeObject.mostabundant_mw =
                                    Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[11]]);
                                loadedMonoIsotopeObject.fwhm =
                                    Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[12]]);
                                loadedMonoIsotopeObject.signal_noise =
                                    Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[13]]);
                                loadedMonoIsotopeObject.mono_abundance =
                                    Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[14]]);
                                loadedMonoIsotopeObject.mono_plus2_abundance =
                                    Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[15]]);
                                loadedMonoIsotopeObject.flag =
                                    Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[16]]);
                                loadedMonoIsotopeObject.interference_score =
                                    Convert.ToDouble(databaseReader[m_MonoIsotopeColumnList[17]]);

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



            return didThisWork;
        }

        public bool readTandemMassSpectraData(string fileName, int index, out double loadedTandemMZ)
        {
            //this works.  one problem is with the raw to yafms converter in that is misses some fragmentation precursor masses 10-12-11 
            if (index == 0)
            {
                Console.WriteLine("must be non zero");
                Console.ReadKey();
                Console.WriteLine("press a button to close");
            }

            bool didThisWork = false;

            string connectionString = "";
            connectionString = "Data Source=" + fileName;



            int precursorScan = 0;
            double precursorMZ = 0;
            int dataSetScanNumber = 0;


            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand getTandemCommmand = connection.CreateCommand())
                {
                    getTandemCommmand.CommandText = m_GetTandemCommmndString;
                    getTandemCommmand.Prepare();

                    SQLiteParameter newSqliteParamater = new SQLiteParameter("rowid", index);
                    getTandemCommmand.Parameters.Add(newSqliteParamater);
                    try
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteDataReader databaseReader = getTandemCommmand.ExecuteReader();
                            databaseReader.Read();

                            dataSetScanNumber = -1;
                            precursorScan = -1;
                            precursorMZ = -1;
                            try
                            {
                                dataSetScanNumber = Convert.ToInt32(databaseReader[m_TandemColumnList[0]]);
                            }
                            catch { }
                            try
                            {
                                precursorScan = Convert.ToInt32(databaseReader[m_TandemColumnList[1]]);
                            }
                            catch { }
                            try
                            {
                                precursorMZ = Convert.ToDouble(databaseReader[m_TandemColumnList[2]]);
                                
                            }
                            catch { }
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
            }

            loadedTandemMZ = precursorMZ;

            return didThisWork;
        }

        private ElutingPeakLite LoadedDataToElutingPeakLite(int elutingPeakID, double elutingPeakMass, int elutingPeakScanStart, int elutingPeakScanEnd, int elutingPeakScanMaxIntensity, int elutingPeakNumberofPeaks, int elutingPeakNumberOfPeaksFlag, string elutingPeakNumberOfPeaksMode, float elutingPeakSummedIntensity, double elutingPeakIsosResultIntensityAggregate)
        {
            ElutingPeakLite loadedElutingPeak = new ElutingPeakLite();
            loadedElutingPeak.ID = elutingPeakID;
            loadedElutingPeak.Mass = elutingPeakMass;
            loadedElutingPeak.ScanStart = elutingPeakScanStart;
            loadedElutingPeak.ScanEnd = elutingPeakScanEnd;
            loadedElutingPeak.ScanMaxIntensity = elutingPeakScanMaxIntensity;

            loadedElutingPeak.NumberOfPeaks = elutingPeakNumberofPeaks;
            loadedElutingPeak.NumberOfPeaksFlag = elutingPeakNumberOfPeaksFlag;
            loadedElutingPeak.NumberOfPeaksMode = elutingPeakNumberOfPeaksMode;

            loadedElutingPeak.SummedIntensity = elutingPeakSummedIntensity;
            loadedElutingPeak.AggregateIntensity = elutingPeakIsosResultIntensityAggregate;

            return loadedElutingPeak;

        }

        private FeatureLight LoadedDataToFeatureLite(int ID, Int64 abundance, int chargeState, double driftTime , double monoisotopicMass , double retentionTime , float score )
        {
            FeatureLight loadedFeature = new FeatureLight();
            loadedFeature.ID = ID;
            loadedFeature.Abundance = abundance;
            loadedFeature.ChargeState = chargeState;
            loadedFeature.DriftTime = driftTime;
            loadedFeature.MassMonoisotopic = monoisotopicMass;
            loadedFeature.RetentionTime = retentionTime;
            loadedFeature.Score = score;

            return loadedFeature;

        }

        private FeatureMultiAlign LoadedDataToFeatureMultiAlign(int id, int dataset_ID, int cluster_ID, int conformation_ID, double mass, double mass_Calibrated, double net, double mz, int scan_LC, int scan_LC_Start, int scan_LC_End, int scan_LC_Aligned, int charge, double abundance_Max, double abundance_Sum, double drift_Time, double average_Interference_Score, double average_Decon_Fit_Score, int UMC_Member_Count, int saturated_Member_Count)
        {
            FeatureMultiAlign loadedFeature = new FeatureMultiAlign();
            loadedFeature.Feature_ID = id; 
            loadedFeature.Dataset_ID = dataset_ID;
            loadedFeature.Cluster_ID = cluster_ID;
            loadedFeature.Conformation_ID = conformation_ID;
            loadedFeature.Mass = mass;
            loadedFeature.Mass_Calibrated = mass_Calibrated;
            loadedFeature.NET = net;
            loadedFeature.MZ = mz;
            loadedFeature.Scan_LC = scan_LC;
            loadedFeature.Scan_LC_Start = scan_LC_Start;
            loadedFeature.Scan_LC_End = scan_LC_End;
            loadedFeature.Scan_LC_Aligned = scan_LC_Aligned;
            loadedFeature.Charge  = charge;
            loadedFeature.Abundance_Max  = abundance_Max;
            loadedFeature.Abundance_Sum  = abundance_Sum;
            loadedFeature.Drift_Time  = drift_Time;
            loadedFeature.Average_Interference_Score = average_Interference_Score;
            loadedFeature.Average_Decon_Fit_Score = average_Decon_Fit_Score;
            loadedFeature.UMC_Member_Count = UMC_Member_Count;
            loadedFeature.Saturated_Member_Count = saturated_Member_Count;

            return loadedFeature;

        }

        private IsotopeObject LoadedDataToIsotopeObject(double monoisotopicMass, double experimentalMass, string isotopeMass, string isotopeIntensity)
        {
            IsotopeObject loadedIsotope = new IsotopeObject();
            loadedIsotope.MonoIsotopicMass = monoisotopicMass;
            loadedIsotope.ExperimentMass = experimentalMass;
            //cycle through peaks from strings
            string[] masses = isotopeMass.Split(',');
            string[] intensities = isotopeIntensity.Split(',');
            for(int i=0;i<masses.Length;i++)
            {
                float massValue = Convert.ToSingle(masses[i]);
                float intensity = Convert.ToSingle(intensities[i]);
                PNNLOmics.Data.Peak newPeak = new PNNLOmics.Data.Peak();
                newPeak.XValue = massValue;
                newPeak.Height = intensity;
                loadedIsotope.IsotopeList.Add(newPeak);
            }
            return loadedIsotope;

        }
    }
}
