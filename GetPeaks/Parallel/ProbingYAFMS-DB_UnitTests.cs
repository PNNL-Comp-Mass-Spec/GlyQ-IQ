using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks.UnitTests;
using GetPeaks_DLL.Go_Decon_Modules;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Objects.ParameterObjects;
using GetPeaks_DLL.Parallel;
using GetPeaks_DLL.SQLiteEngine;
using NUnit.Framework;
using PNNLOmics.Data;

namespace Parallel
{
    class ProbingYAFMS_DB_UnitTests
    {
        public PopulatorArgs _fileParameters { get; set; }
        public InputOutputFileName _newFile { get; set; }

        [Test]
        public void ArgsToParameters()
        {
            //"E:\ScottK\GetPeaks Data" "Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12" "RAW" "E:\ScottK\Populator" "E:\ScottK\Populator\Logs1.txt" "E:\ScottK\Populator" "0_Glyco08 LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_Fitpt2_Velos"
            string[] args = new string[7];
            args[0] = @"E:\ScottK\GetPeaks Data";
            args[1] = @"Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12";
            args[2] = @"RAW";
            args[3] = @"E:\ScottK\Populator";
            args[4] = @"E:\ScottK\Populator\Logs1.txt";
            args[5] = @"E:\ScottK\Populator";
            args[6] = @"0_Glyco08 LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_Fitpt2_Velos";

            //TEST Code

            PopulatorArgs fileParametersSetup = new PopulatorArgs(args);

            Assert.AreEqual(fileParametersSetup.fileExtension, "RAW");
            Assert.AreEqual(fileParametersSetup.fileNameOnly, "Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12");
            Assert.AreEqual(fileParametersSetup.inputDatasetFolder, "E:\\ScottK\\GetPeaks Data");
            Assert.AreEqual(fileParametersSetup.logFile, "E:\\ScottK\\Populator\\Logs1.txt");
            Assert.AreEqual(fileParametersSetup.parameterFileFolder, "E:\\ScottK\\Populator");
            Assert.AreEqual(fileParametersSetup.parameterFileNameOnly, "0_Glyco08 LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_Fitpt2_Velos");
            Assert.AreEqual(fileParametersSetup.sqLiteFolder, "E:\\ScottK\\Populator");

            _fileParameters = fileParametersSetup;
        }

        [Test]
        public void SetupObjectsForTest()
        {
            ArgsToParameters();
            
            const char letter = 'Z';
            const bool useParameterFileValues = true; //True will use fit and ThrashPBR from the file.  File will use it from the code 
            
            //TEST Code

            ParalellController engineController;
            string sqLiteFile;
            string sqLiteFolder;
            int computersToDivideOver;
            int coresPerComputer;
            string logFile;
            InputOutputFileName newFile;
            ParametersSQLite sqliteDetails;

            ParametersForTesting.Load(letter, useParameterFileValues, out engineController, out sqLiteFile, out sqLiteFolder, out computersToDivideOver, out coresPerComputer, out logFile, out newFile, out sqliteDetails, _fileParameters);

            Assert.AreEqual(sqLiteFile, "Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_13PBR_20fit_0HPBR");
            Assert.AreEqual(sqLiteFolder, "E:\\ScottK\\Populator\\");
            Assert.AreEqual(computersToDivideOver, 1);
            Assert.AreEqual(coresPerComputer, 1);
            Assert.AreEqual(logFile, "E:\\ScottK\\Populator\\Logs1.txt");
            Assert.AreEqual(newFile.OutputPath, "E:\\ScottK\\Populator\\");
            Assert.AreEqual(newFile.OutputSQLFileName, "E:\\ScottK\\Populator\\");

            Assert.AreEqual(sqliteDetails.PageName, "T_Scan_Centric");
            Assert.AreEqual(sqliteDetails.ColumnHeaders.Count, 0);
            Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[0], "T_Scan_Peaks");
            Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[1], "T_Scans");
            Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[2], "T_Scans_Precursor_Peaks");
            Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[3], "T_Scan_MonoPeaks");
            Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[4], "T_Peak_Centric");
            Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[5], "T_Scan_Centric");

            _newFile = newFile;
        }

        [Test]
        public void SetupPrecursorsForTest()
        {
            ArgsToParameters();
            SetupObjectsForTest();
            
            const int limitFileToThisManyScans = 9999999;//400//3000/99999

            //TestCode

            List<PrecursorInfo> precursors = new List<PrecursorInfo>();
            //List<int> scanMSLevelList = new List<int>();
            List<int> scanLevels = new List<int>();
            List<int> scanLevelsWithTandem = new List<int>();

            YafmsDbUtilities.ProcessScanSpectraNumbers(_newFile, limitFileToThisManyScans, out precursors, out scanLevels, out scanLevelsWithTandem);

            Assert.AreEqual(precursors.Count, 7198);
            Assert.AreEqual(scanLevels.Count, 7198);
            Assert.AreEqual(scanLevelsWithTandem.Count, 1275);
        }

       
    }


}
