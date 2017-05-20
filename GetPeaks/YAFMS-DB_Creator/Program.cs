using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQGlyQ.UnitTesting;

namespace YAFMS_DB_Creator
{
    class Program
    {
        static void Main(string[] args)
        {
            //"E:\ScottK\GetPeaks Data\Diabetes_LC\\" "Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12" “raw" "E:\ScottK\WorkingResults\\"
            
            if(args.Length==5)
            {
                string datasetFolder = args[0];
                string datasetFileName = args[1];
                string datasetEnding = @"." + args[2];

                string databaseFolder = args[3];

                string scansToSum = args[4];
                int scansToSumInt = Convert.ToInt32(scansToSum);
                string databaseFile = datasetFileName;

                Console.WriteLine("DatasetFolder:  " + datasetFolder);
                Console.WriteLine("DatasetFileName:  " + datasetFileName);
                Console.WriteLine("DatasetEnding:  " + datasetEnding);
                Console.WriteLine("DatabaseFolder:  " + databaseFolder);
                Console.WriteLine("ScansToSum:  " + scansToSumInt);
                Console.WriteLine(Environment.NewLine + "Are the parameters correct?.  Press Enter");
                Console.ReadKey();

                YAFMS_DB_UnitTestsIQ tester = new YAFMS_DB_UnitTestsIQ();
                tester.WriteDatasetFX(datasetFolder, datasetFileName, datasetEnding, databaseFolder, databaseFile, @".db", scansToSumInt);
            }
            else
            {
                Console.WriteLine("Missing Args");
                Console.WriteLine("datasetFolder, datasetFileName, datasetEnding, databaseFolder");
                Console.WriteLine("press -Enter- to exit");
                Console.ReadKey();
            }

        }
    }
}
