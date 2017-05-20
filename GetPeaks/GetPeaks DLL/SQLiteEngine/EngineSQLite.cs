using System;
using System.Collections.Generic;
using GetPeaks_DLL.SQLite;
using GetPeaks_DLL.Objects;
using PNNLOmics.Data;
using GetPeaks_DLL.SQLite.TableInformation;
using Parallel.THRASH;
using GetPeaks_DLL.Parallel;
using GetPeaks_DLL.Go_Decon_Modules;
using Parallel.SQLite;
using PNNLOmics.Data.Features;
using GetPeaks_DLL.Objects.TandemMSObjects;

namespace GetPeaks_DLL.SQLiteEngine
{
    public class EngineSQLite:ParalellEngine
    {
        public DatabaseAdaptor newDataBaseAdaptor { get; set; }

        public List<string> ColumnNamesForCount { get; set; } 

        //public DatabaseLayer newDataBaseLayer { get; set; }//this is handeled inside the database adaptor
        
        public EngineSQLite(ParametersSQLite sqlParameters, Object databaseLock, int engineNumber)
        {
            this.DatabaseLock = databaseLock;
            this.Parameters = sqlParameters;
            this.ColumnNamesForCount = sqlParameters.ColumnHeadersCounts;
            //newDataBaseLayer = new DatabaseLayer();
            bool didThisWork = false;

            string localSQLFileName = sqlParameters.FileInforamation.OutputPath + sqlParameters.FileInforamation.OutputSQLFileName + "_(" + engineNumber + ").db";
            //localSQLFileName = sqlParameters.FileInforamation.OutputPath + sqlParameters.UniqueFileName;

            newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName);

            if (!System.IO.File.Exists(localSQLFileName))
            {
                didThisWork = DatabaseFramework.Create(localSQLFileName, databaseLock, sqlParameters.ColumnHeadersCounts);
            }
            else
            {
                
            }

            
            //didThisWork = FeatureLiteTable.CreateFeatrueLiteTableInformation(localSQLFileName, databaseLock, sqlParameters.ColumnHeadersCounts);

            //didThisWork = newDataBaseLayer.CreateFeatrueLiteTableInformation(localSQLFileName);

            if (didThisWork == true)
            {
                Console.WriteLine("FileCreated with column headers");
            }
            else
            {
                Console.WriteLine("this did not work, ElutingPeakFinderPart2");
            }
            
            //newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName);
            
        }

        public EngineSQLite(Object databaseLock, int engineNumber)
        {
            this.DatabaseLock = databaseLock;
            //newDataBaseLayer = new DatabaseLayer();
        }

        public bool WritePeakData(EngineSQLite engineToUse, Peak resultPeak, int scanNum)
        {
            string localSQLFileName = engineToUse.Parameters.FileInforamation.OutputPath + engineToUse.Parameters.FileInforamation.OutputSQLFileName +".db";

            

            //perhaps read in lenght of peak table so we can get index number
            int index = 1;

            bool isSuccessfull = false;
            using (newDataBaseAdaptor)
            //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName))
            {

                //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);

                lock (this.DatabaseLock)
                {
                    isSuccessfull = newDataBaseAdaptor.WritePeakToSQLite(resultPeak, index, scanNum);
                }

                if (isSuccessfull)
                {
                    Console.WriteLine("++Data Stored-Mass: " + resultPeak.XValue + " Height: " + resultPeak.Height);
                }
                else
                {
                    Console.WriteLine("Data Not Stored");
                    Console.WriteLine("data not stored, Engine SQLite");
                }

                //Profiler memoryCheck = new Profiler();
                //memoryCheck.printMemory("baseline during write");

                //ePeak = null;
                //resultPeak = null;
            }

            return isSuccessfull;

        }

        public bool WriteProcessedPeakList(EngineSQLite engineToUse, List<ProcessedPeak> resultPeaks, ParametersSQLite parameters)
        {
            //string localSQLFileName = engineToUse.Parameters.FileInforamation.OutputPath + engineToUse.Parameters.FileInforamation.OutputSQLFileName + ".db";
            string TableName = parameters.PageName;
            string DataTableCountName = "TableInfo";

            
            //List<string> columnNamesForCount = new List<string>();
            

            List<ProcessedPeak> peakPile = resultPeaks;
            //perhaps read in lenght of peak table so we can get index number
            int index = 1;

            bool isSuccessfull = false;
            using (newDataBaseAdaptor)
            //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName))
            {

                //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);

                lock (this.DatabaseLock)
                {
                    //int scanNum = isotopeResults.Scan;
                    //isSuccessfull = newDataBaseAdaptor.WritePeakToSQLite(resultPeak, index, scanNum);
                    //isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageMonoListToSQLite(isotopePile, scanNum, IsosDataTableName, ColumnNamesForCount);
                    isSuccessfull = newDataBaseAdaptor.WriteProcessedPeakListToSQLite(peakPile, TableName, DataTableCountName);
                }

                if (isSuccessfull)
                {
                    //Console.WriteLine("++Data Stored-Mass: " + resultPeak.XValue + " Height: " + resultPeak.Height);
                    Console.WriteLine("++Data Stored-Mass: " + peakPile[0].XValue + " Height: " + peakPile[0].Height);
                }
                else
                {
                    Console.WriteLine("Data Not Stored");
                    Console.WriteLine("data not stored, Engine SQLite");
                    Console.ReadKey();
                }

                //Profiler memoryCheck = new Profiler();
                //memoryCheck.printMemory("baseline during write");

                //ePeak = null;
                //resultPeak = null;
            }

            return isSuccessfull;

        }




    



        #region centric writers
        //5-18-2013
        public bool WritePeakCentricList(EngineSQLite engineToUse, List<PeakCentric> resultPeaks, ParametersSQLite parameters)
        {
            //string localSQLFileName = engineToUse.Parameters.FileInforamation.OutputPath + engineToUse.Parameters.FileInforamation.OutputSQLFileName + ".db";
            string TableName = parameters.PageName;
            string DataTableCountName = "TableInfo";


            //List<string> columnNamesForCount = new List<string>();


            List<PeakCentric> featurePile = resultPeaks;
            //perhaps read in lenght of peak table so we can get index number
            int index = 1;

            bool isSuccessfull = false;
            using (newDataBaseAdaptor)
            //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName))
            {

                //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);

                lock (this.DatabaseLock)
                {
                    //int scanNum = isotopeResults.Scan;
                    //isSuccessfull = newDataBaseAdaptor.WritePeakToSQLite(resultPeak, index, scanNum);
                    //isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageMonoListToSQLite(isotopePile, scanNum, IsosDataTableName, ColumnNamesForCount);
                    isSuccessfull = newDataBaseAdaptor.WritePeakCentricListToSQLite(featurePile, TableName, DataTableCountName);
                }

                if (isSuccessfull)
                {
                    //Console.WriteLine("++Data Stored-Mass: " + resultPeak.XValue + " Height: " + resultPeak.Height);
                    Console.WriteLine("++Data Stored-Mass: " + featurePile[0].Mass + " Height: " + featurePile[0].Abundance);
                }
                else
                {
                    Console.WriteLine("Data Not Stored");
                    Console.ReadKey();
                    Console.WriteLine("data not stored, Engine SQLite");
                }

                //Profiler memoryCheck = new Profiler();
                //memoryCheck.printMemory("baseline during write");

                //ePeak = null;
                //resultPeak = null;
            }

            return isSuccessfull;

        }

        public bool Write_XYData_List(EngineSQLite engineToUse, List<PeakCentric> resultPeaks, ParametersSQLite parameters)
        {
            //string localSQLFileName = engineToUse.Parameters.FileInforamation.OutputPath + engineToUse.Parameters.FileInforamation.OutputSQLFileName + ".db";
            string TableName = parameters.PageName;
            string DataTableCountName = "TableInfo";


            //List<string> columnNamesForCount = new List<string>();


            List<PeakCentric> featurePile = resultPeaks;
            //perhaps read in lenght of peak table so we can get index number
            int index = 1;

            bool isSuccessfull = false;
            using (newDataBaseAdaptor)
            //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName))
            {

                //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);

                lock (this.DatabaseLock)
                {
                    //int scanNum = isotopeResults.Scan;
                    //isSuccessfull = newDataBaseAdaptor.WritePeakToSQLite(resultPeak, index, scanNum);
                    //isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageMonoListToSQLite(isotopePile, scanNum, IsosDataTableName, ColumnNamesForCount);
                    isSuccessfull = newDataBaseAdaptor.WriteXYDataToSQLite(featurePile, TableName, DataTableCountName);
                }

                if (isSuccessfull)
                {
                    //Console.WriteLine("++Data Stored-Mass: " + resultPeak.XValue + " Height: " + resultPeak.Height);
                    Console.WriteLine("++Data Stored-Mass: " + featurePile[0].Mass + " Height: " + featurePile[0].Abundance);
                }
                else
                {
                    Console.WriteLine("Data Not Stored");
                    Console.ReadKey();
                    Console.WriteLine("data not stored, Engine SQLite");
                }

                //Profiler memoryCheck = new Profiler();
                //memoryCheck.printMemory("baseline during write");

                //ePeak = null;
                //resultPeak = null;
            }

            return isSuccessfull;

        }


        public bool WriteScanCentricList(EngineSQLite engineToUse, List<ScanCentric> resultPeaks, ParametersSQLite parameters)
        {
            //string localSQLFileName = engineToUse.Parameters.FileInforamation.OutputPath + engineToUse.Parameters.FileInforamation.OutputSQLFileName + ".db";
            string TableName = parameters.PageName;
            string DataTableCountName = "TableInfo";


            //List<string> columnNamesForCount = new List<string>();


            List<ScanCentric> featurePile = resultPeaks;
            //perhaps read in lenght of peak table so we can get index number
            int index = 1;

            bool isSuccessfull = false;
            using (newDataBaseAdaptor)
            //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName))
            {

                //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);

                lock (this.DatabaseLock)
                {
                    //int scanNum = isotopeResults.Scan;
                    //isSuccessfull = newDataBaseAdaptor.WritePeakToSQLite(resultPeak, index, scanNum);
                    //isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageMonoListToSQLite(isotopePile, scanNum, IsosDataTableName, ColumnNamesForCount);
                    isSuccessfull = newDataBaseAdaptor.WriteScanCentricListToSQLite(featurePile, TableName, DataTableCountName);
                }

                if (isSuccessfull)
                {
                    //Console.WriteLine("++Data Stored-Mass: " + resultPeak.XValue + " Height: " + resultPeak.Height);
                    Console.WriteLine("++Scan: " + featurePile[0].ScanNumLc);
                }
                else
                {
                    Console.WriteLine("Data Not Stored");
                    
                    Console.ReadKey();
                    Console.WriteLine("data not stored, Engine SQLite");
                }

                //Profiler memoryCheck = new Profiler();
                //memoryCheck.printMemory("baseline during write");

                //ePeak = null;
                //resultPeak = null;
            }

            return isSuccessfull;

        }

        #region off
        //public bool WriteAttributeCentricList(EngineSQLite engineToUse, List<AttributeCentric> resultPeaks, ParametersSQLite parameters)
        //{
        //    //string localSQLFileName = engineToUse.Parameters.FileInforamation.OutputPath + engineToUse.Parameters.FileInforamation.OutputSQLFileName + ".db";
        //    string TableName = parameters.PageName;
        //    string DataTableCountName = "TableInfo";


        //    //List<string> columnNamesForCount = new List<string>();


        //    List<AttributeCentric> featurePile = resultPeaks;
        //    //perhaps read in lenght of peak table so we can get index number
        //    int index = 1;

        //    bool isSuccessfull = false;
        //    using (newDataBaseAdaptor)
        //    //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName))
        //    {

        //        //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);

        //        lock (this.DatabaseLock)
        //        {
        //            //int scanNum = isotopeResults.Scan;
        //            //isSuccessfull = newDataBaseAdaptor.WritePeakToSQLite(resultPeak, index, scanNum);
        //            //isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageMonoListToSQLite(isotopePile, scanNum, IsosDataTableName, ColumnNamesForCount);
        //            isSuccessfull = newDataBaseAdaptor.WriteAttributeCentricListToSQLite(featurePile, TableName, DataTableCountName);
        //        }

        //        if (isSuccessfull)
        //        {
        //            //Console.WriteLine("++Data Stored-Mass: " + resultPeak.XValue + " Height: " + resultPeak.Height);
        //            Console.WriteLine("++Centroided?: " + featurePile[0].isCentroided);
        //        }
        //        else
        //        {
        //            Console.WriteLine("Data Not Stored");
                    
        //            Console.ReadKey();
        //            Console.WriteLine("data not stored, Engine SQLite");
        //        }

        //        //Profiler memoryCheck = new Profiler();
        //        //memoryCheck.printMemory("baseline during write");

        //        //ePeak = null;
        //        //resultPeak = null;
        //    }

        //    return isSuccessfull;

        //}

        //public bool WriteFragmentCentricList(EngineSQLite engineToUse, List<FragmentCentric> resultPeaks, ParametersSQLite parameters)
        //{
        //    //string localSQLFileName = engineToUse.Parameters.FileInforamation.OutputPath + engineToUse.Parameters.FileInforamation.OutputSQLFileName + ".db";
        //    string TableName = parameters.PageName;
        //    string DataTableCountName = "TableInfo";


        //    //List<string> columnNamesForCount = new List<string>();


        //    List<FragmentCentric> featurePile = resultPeaks;
        //    //perhaps read in lenght of peak table so we can get index number
        //    int index = 1;

        //    bool isSuccessfull = false;
        //    using (newDataBaseAdaptor)
        //    //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName))
        //    {

        //        //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);

        //        lock (this.DatabaseLock)
        //        {
        //            //int scanNum = isotopeResults.Scan;
        //            //isSuccessfull = newDataBaseAdaptor.WritePeakToSQLite(resultPeak, index, scanNum);
        //            //isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageMonoListToSQLite(isotopePile, scanNum, IsosDataTableName, ColumnNamesForCount);
        //            isSuccessfull = newDataBaseAdaptor.WriteFragmentCentricListToSQLite(featurePile, TableName, DataTableCountName);
        //        }

        //        if (isSuccessfull)
        //        {
        //            //Console.WriteLine("++Data Stored-Mass: " + resultPeak.XValue + " Height: " + resultPeak.Height);
        //            Console.WriteLine("++ParentScan: " + featurePile[0].ParentScanNumber);
        //        }
        //        else
        //        {
        //            Console.WriteLine("Data Not Stored");
                    
        //            Console.ReadKey();
        //            Console.WriteLine("data not stored, Engine SQLite");
        //        }

        //        //Profiler memoryCheck = new Profiler();
        //        //memoryCheck.printMemory("baseline during write");

        //        //ePeak = null;
        //        //resultPeak = null;
        //    }

        //    return isSuccessfull;

        //}
        #endregion

        public bool UpdatePeakCentricList(EngineSQLite engineToUse, List<PeakCentric> resultPeaks, ParametersSQLite parameters, List<int> activeColumns)
        {
            //string localSQLFileName = engineToUse.Parameters.FileInforamation.OutputPath + engineToUse.Parameters.FileInforamation.OutputSQLFileName + ".db";
            string TableName = parameters.PageName;
            string DataTableCountName = "TableInfo";


            //List<string> columnNamesForCount = new List<string>();


            List<PeakCentric> featurePile = resultPeaks;
            //perhaps read in lenght of peak table so we can get index number
            int index = 1;

            bool isSuccessfull = false;
            using (newDataBaseAdaptor)
            //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName))
            {

                //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);

                lock (this.DatabaseLock)
                {
                    //int scanNum = isotopeResults.Scan;
                    //isSuccessfull = newDataBaseAdaptor.WritePeakToSQLite(resultPeak, index, scanNum);
                    //isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageMonoListToSQLite(isotopePile, scanNum, IsosDataTableName, ColumnNamesForCount);
                    isSuccessfull = newDataBaseAdaptor.UpdatePeakCentricListToSQLite(featurePile, TableName, activeColumns);
                }

                if (isSuccessfull)
                {
                    //Console.WriteLine("++Data Stored-Mass: " + resultPeak.XValue + " Height: " + resultPeak.Height);
                    Console.WriteLine("++Data Stored-Mass: " + featurePile[0].Mass + " Height: " + featurePile[0].Abundance);
                }
                else
                {
                    Console.WriteLine("Data Not Stored");
                    
                    Console.ReadKey();
                    Console.WriteLine("data not stored, Engine SQLite");
                }

                //Profiler memoryCheck = new Profiler();
                //memoryCheck.printMemory("baseline during write");

                //ePeak = null;
                //resultPeak = null;
            }

            return isSuccessfull;

        }

        #region off
        //public bool UpdateAttributeCentricList(EngineSQLite engineToUse, List<AttributeCentric> resultPeaks, ParametersSQLite parameters, List<int> activeColumns )
        //{
        //    //string localSQLFileName = engineToUse.Parameters.FileInforamation.OutputPath + engineToUse.Parameters.FileInforamation.OutputSQLFileName + ".db";
        //    string TableName = parameters.PageName;
        //    string DataTableCountName = "TableInfo";


        //    //List<string> columnNamesForCount = new List<string>();


        //    List<AttributeCentric> featurePile = resultPeaks;
        //    //perhaps read in lenght of peak table so we can get index number
        //    int index = 1;

        //    bool isSuccessfull = false;
        //    using (newDataBaseAdaptor)
        //    //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName))
        //    {

        //        //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);

        //        lock (this.DatabaseLock)
        //        {
        //            //int scanNum = isotopeResults.Scan;
        //            //isSuccessfull = newDataBaseAdaptor.WritePeakToSQLite(resultPeak, index, scanNum);
        //            //isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageMonoListToSQLite(isotopePile, scanNum, IsosDataTableName, ColumnNamesForCount);
        //            //isSuccessfull = newDataBaseAdaptor.UpdateAttributeCentricListToSQLite(featurePile, TableName, DataTableCountName);
        //            isSuccessfull = newDataBaseAdaptor.UpdateAttributeCentricListToSQLite(featurePile, TableName, activeColumns);
        //        }

        //        if (isSuccessfull)
        //        {
        //            //Console.WriteLine("++Data Stored-Mass: " + resultPeak.XValue + " Height: " + resultPeak.Height);
        //            Console.WriteLine("++Centroided?: " + featurePile[0].isCentroided);
        //        }
        //        else
        //        {
        //            Console.WriteLine("Data Not Stored");
                   
        //            Console.ReadKey();
        //            Console.WriteLine("data not stored, Engine SQLite");
        //        }

        //        //Profiler memoryCheck = new Profiler();
        //        //memoryCheck.printMemory("baseline during write");

        //        //ePeak = null;
        //        //resultPeak = null;
        //    }

        //    return isSuccessfull;

        //}

        //public bool UpdateFragmentCentricList(EngineSQLite engineToUse, List<FragmentCentric> resultFragment, ParametersSQLite parameters, List<int> activeColumns)
        //{
        //    //string localSQLFileName = engineToUse.Parameters.FileInforamation.OutputPath + engineToUse.Parameters.FileInforamation.OutputSQLFileName + ".db";
        //    string TableName = parameters.PageName;
        //    string DataTableCountName = "TableInfo";


        //    //List<string> columnNamesForCount = new List<string>();


        //    List<FragmentCentric> featurePile = resultFragment;
        //    //perhaps read in lenght of peak table so we can get index number
        //    int index = 1;

        //    bool isSuccessfull = false;
        //    using (newDataBaseAdaptor)
        //    //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName))
        //    {

        //        //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);

        //        lock (this.DatabaseLock)
        //        {
        //            //int scanNum = isotopeResults.Scan;
        //            //isSuccessfull = newDataBaseAdaptor.WritePeakToSQLite(resultPeak, index, scanNum);
        //            //isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageMonoListToSQLite(isotopePile, scanNum, IsosDataTableName, ColumnNamesForCount);
        //            isSuccessfull = newDataBaseAdaptor.UpdateFragmentCentricListToSQLite(featurePile, TableName, activeColumns);
        //        }

        //        if (isSuccessfull)
        //        {
        //            //Console.WriteLine("++Data Stored-Mass: " + resultPeak.XValue + " Height: " + resultPeak.Height);
        //            Console.WriteLine("++Data Stored-Mass: " + featurePile[0].MsLevel + " Height: " + featurePile[0].ParentScanNumber);
        //        }
        //        else
        //        {
        //            Console.WriteLine("Data Not Stored");
                    
        //            Console.ReadKey();
        //            Console.WriteLine("data not stored, Engine SQLite");
        //        }

        //        //Profiler memoryCheck = new Profiler();
        //        //memoryCheck.printMemory("baseline during write");

        //        //ePeak = null;
        //        //resultPeak = null;
        //    }

        //    return isSuccessfull;

        //}

        #endregion

        public bool UpdateScanCentricList(EngineSQLite engineToUse, List<ScanCentric> resultFragment, ParametersSQLite parameters, List<int> activeColumns)
        {
            //string localSQLFileName = engineToUse.Parameters.FileInforamation.OutputPath + engineToUse.Parameters.FileInforamation.OutputSQLFileName + ".db";
            string TableName = parameters.PageName;
            string DataTableCountName = "TableInfo";


            //List<string> columnNamesForCount = new List<string>();


            List<ScanCentric> featurePile = resultFragment;
            //perhaps read in lenght of peak table so we can get index number
            int index = 1;

            bool isSuccessfull = false;
            using (newDataBaseAdaptor)
            //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName))
            {

                //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);

                lock (this.DatabaseLock)
                {
                    //int scanNum = isotopeResults.Scan;
                    //isSuccessfull = newDataBaseAdaptor.WritePeakToSQLite(resultPeak, index, scanNum);
                    //isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageMonoListToSQLite(isotopePile, scanNum, IsosDataTableName, ColumnNamesForCount);
                    isSuccessfull = newDataBaseAdaptor.UpdateScanCentricListToSQLite(featurePile, TableName, activeColumns);
                }

                if (isSuccessfull)
                {
                    //Console.WriteLine("++Data Stored-Mass: " + resultPeak.XValue + " Height: " + resultPeak.Height);
                    Console.WriteLine("++Data Stored-Mass: " + featurePile[0].ScanID + " Height: " + featurePile[0].PeakID);
                }
                else
                {
                    Console.WriteLine("Data Not Stored");

                    Console.ReadKey();
                    Console.WriteLine("data not stored, Engine SQLite");
                }

                //Profiler memoryCheck = new Profiler();
                //memoryCheck.printMemory("baseline during write");

                //ePeak = null;
                //resultPeak = null;
            }

            return isSuccessfull;

        }


        #endregion








        //public bool WriteProcessedPeak(EngineSQLite engineToUse, ProcessedPeak resultPeak, ParametersSQLite parameters)
        //{
        //    //string localSQLFileName = engineToUse.Parameters.FileInforamation.OutputPath + engineToUse.Parameters.FileInforamation.OutputSQLFileName + ".db";
        //    string TableName = parameters.PageName;
        //    string DataTableCountName = "TableInfo";


        //    //List<string> columnNamesForCount = new List<string>();


        //    //List<ProcessedPeak> peakPile = resultPeaks;
        //    //perhaps read in lenght of peak table so we can get index number
        //    int index = 1;

        //    bool isSuccessfull = false;
        //    using (newDataBaseAdaptor)
        //    //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName))
        //    {

        //        //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);

        //        lock (this.DatabaseLock)
        //        {
        //            //int scanNum = isotopeResults.Scan;
        //            //isSuccessfull = newDataBaseAdaptor.WritePeakToSQLite(resultPeak, index, scanNum);
        //            //isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageMonoListToSQLite(isotopePile, scanNum, IsosDataTableName, ColumnNamesForCount);
        //            isSuccessfull = newDataBaseAdaptor.WritePeakToSQLite(resultPeak, index, DataTableCountName);
        //        }

        //        if (isSuccessfull)
        //        {
        //            //Console.WriteLine("++Data Stored-Mass: " + resultPeak.XValue + " Height: " + resultPeak.Height);
        //            Console.WriteLine("++Data Stored-Mass: " + peakPile[0].XValue + " Height: " + peakPile[0].Height);
        //        }
        //        else
        //        {
        //            Console.WriteLine("Data Not Stored");
        //            Console.WriteLine("data not stored, Engine SQLite");
        //            Console.ReadKey();
        //        }

        //        //Profiler memoryCheck = new Profiler();
        //        //memoryCheck.printMemory("baseline during write");

        //        //ePeak = null;
        //        //resultPeak = null;
        //    }

        //    return isSuccessfull;

        //}

        public bool WritePrecursorPeak(EngineSQLite engineToUse, ProcessedPeak resultPeak, ParametersSQLite parameters, double mzFromHeader, int tandemScan, int charge)
        {
            //string localSQLFileName = engineToUse.Parameters.FileInforamation.OutputPath + engineToUse.Parameters.FileInforamation.OutputSQLFileName + ".db";
            string TableName = parameters.PageName;
            string DataTableCountName = "TableInfo";

            //List<ProcessedPeak> peakPile = resultPeaks;
            //perhaps read in lenght of peak table so we can get index number
            int index = -1;

            bool isSuccessfull = false;
            using (newDataBaseAdaptor)
            {
                lock (this.DatabaseLock)
                {
                    isSuccessfull = newDataBaseAdaptor.WritePrecursorPeakToSQLite(resultPeak, mzFromHeader, tandemScan, index, TableName, DataTableCountName, charge);
                }

                if (isSuccessfull)
                {
                    Console.WriteLine("++Data Stored-Mass: " + resultPeak.XValue + " Height: " + resultPeak.Height);
                }
                else
                {
                    Console.WriteLine("Data Not Stored");
                    Console.WriteLine("data not stored, Engine SQLite");
                    Console.ReadKey();
                }

                //Profiler memoryCheck = new Profiler();
                //memoryCheck.printMemory("baseline during write");

                //ePeak = null;
                //resultPeak = null;
            }

            return isSuccessfull;

        }


        public bool WriteScanList(EngineSQLite engineToUse, List<MSSpectra> spectra, ParametersSQLite parameters)
        {
            //string localSQLFileName = engineToUse.Parameters.FileInforamation.OutputPath + engineToUse.Parameters.FileInforamation.OutputSQLFileName + ".db";
            string TableName = parameters.PageName;
            string DataTableCountName = "TableInfo";


            //List<string> columnNamesForCount = new List<string>();


            List<MSSpectra> spectraPile = spectra;
            //perhaps read in lenght of peak table so we can get index number
            int index = 1;

            bool isSuccessfull = false;
            using (newDataBaseAdaptor)
            //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName))
            {

                //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);

                lock (this.DatabaseLock)
                {
                    //int scanNum = isotopeResults.Scan;
                    //isSuccessfull = newDataBaseAdaptor.WritePeakToSQLite(resultPeak, index, scanNum);
                    //isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageMonoListToSQLite(isotopePile, scanNum, IsosDataTableName, ColumnNamesForCount);
                    isSuccessfull = newDataBaseAdaptor.WriteScansToSQLite(spectraPile, TableName, DataTableCountName);
                }

                if (isSuccessfull)
                {
                    //Console.WriteLine("++Data Stored-Mass: " + resultPeak.XValue + " Height: " + resultPeak.Height);
                    //Console.WriteLine("++Data Stored-Mass: " + pile[0].XValue + " Height: " + pile[0].Height);
                }
                else
                {
                    Console.WriteLine("Data Not Stored");
                    Console.WriteLine("data not stored, Engine SQLite");
                    Console.ReadKey();
                }

                //Profiler memoryCheck = new Profiler();
                //memoryCheck.printMemory("baseline during write");

                //ePeak = null;
                //resultPeak = null;
            }

            return isSuccessfull;

        }
        
        public bool WriteIsosData(ResultsTHRASH isotopeResults)
        {
            //string localSQLFileName = engineToUse.Parameters.FileInforamation.OutputPath + engineToUse.Parameters.FileInforamation.OutputSQLFileName + ".db";
            string IsosDataTableName = "IsotopeTable";
            string IsosDataTableCountName = "TableInfo";

            List<string> columnNamesForCount = ColumnNamesForCount;


            List<IsotopeObject> isotopePile = isotopeResults.ResultsFromRunConverted;
            //perhaps read in lenght of peak table so we can get index number
            int index = 1;

            bool isSuccessfull = false;
            using (newDataBaseAdaptor)
            //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName))
            {

                //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);

                lock (this.DatabaseLock)
                {
                    int scanNum = isotopeResults.Scan;
                    //isSuccessfull = newDataBaseAdaptor.WritePeakToSQLite(resultPeak, index, scanNum);
                    //isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageMonoListToSQLite(isotopePile, scanNum, IsosDataTableName, ColumnNamesForCount);
                    isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageMonoListToSQLite(isotopePile, scanNum, IsosDataTableName, IsosDataTableCountName, columnNamesForCount);
                }

                if (isSuccessfull)
                {
                    //Console.WriteLine("++Data Stored-Mass: " + resultPeak.XValue + " Height: " + resultPeak.Height);
                    Console.WriteLine("++Data Stored-Mass: " + isotopePile[0].ExperimentMass + " Height: " + isotopePile[0].MonoIsotopicMass);
                }
                else
                {
                    Console.WriteLine("Data Not Stored");
                    Console.WriteLine("data not stored, Engine SQLite");
                    Console.ReadKey();
                }

                //Profiler memoryCheck = new Profiler();
                //memoryCheck.printMemory("baseline during write");

                //ePeak = null;
                //resultPeak = null;
            }

            return isSuccessfull;

        }
            
        public override ParalellEngineStation SetupEngines(ParalellParameters parameters)
        {
            ParalellEngineStation engineStation = new EngineStationSQLite();

            List<ParalellEngine> engines = engineStation.Engines;
            object engineLock = engineStation.EngineLock;

            //EngineSQLite singleEngine = new EngineSQLite((ParametersSQLite)parameters, engineLock);

            for (int i = 0; i < parameters.CoresPerComputer; i++)
            {
                ParametersSQLite sqlParameters = (ParametersSQLite)parameters;
                //sqlParameters.UniqueFileName = sqlParameters.FileInforamation.OutputSQLFileName + " (" + 0 + ").db";
                //EngineSQLite sqlEngine = new EngineSQLite(sqlParameters, engineLock, i+1);
                EngineSQLite sqlEngine = new EngineSQLite(sqlParameters, engineLock, i);
                //EngineSQLite SQLEngine = new EngineSQLite((ParametersSQLite)parameters, engineLock);
                engines.Add(sqlEngine);

            }

            //ParametersSQLite sqlParametersBonus = (ParametersSQLite)parameters;
            //EngineSQLite sqlEngineBonus = new EngineSQLite(sqlParametersBonus, engineLock, 0);
            //engineStation.ExtraEngines.Add(sqlEngineBonus);

            return engineStation;
        }
    }


}
