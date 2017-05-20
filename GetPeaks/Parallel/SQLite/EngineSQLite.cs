using System;
using System.Collections.Generic;
using GetPeaks_DLL.SQLite;
using GetPeaks_DLL.Objects;
using PNNLOmics.Data;
using GetPeaks_DLL.SQLite.TableInformation;
using Parallel.THRASH;

namespace Parallel.SQLite
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

            didThisWork = DatabaseFramework.Create(localSQLFileName, databaseLock, sqlParameters.ColumnHeadersCounts);
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

        //public bool WriteIsosData(EngineSQLite engineToUse, Peak resultPeak, int scanNum)
        //public bool WriteIsosData(EngineSQLite engineToUse, ResultsTHRASH isotopeResults)
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
                EngineSQLite sqlEngine = new EngineSQLite(sqlParameters, engineLock, i+1);
                //EngineSQLite SQLEngine = new EngineSQLite((ParametersSQLite)parameters, engineLock);
                engines.Add(sqlEngine);

            }

            ParametersSQLite sqlParametersBonus = (ParametersSQLite)parameters;
            EngineSQLite sqlEngineBonus = new EngineSQLite(sqlParametersBonus, engineLock, 0);
            engineStation.ExtraEngines.Add(sqlEngineBonus);

            return engineStation;
        }
    }


}
