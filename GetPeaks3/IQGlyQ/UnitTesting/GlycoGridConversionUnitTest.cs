using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaksDllLite.DataFIFO;
using IQGlyQ.Enumerations;
using IQGlyQ.FIFO;
using NUnit.Framework;
//using GetPeaks_DLL.DataFIFO;

namespace IQGlyQ.UnitTesting
{
    class GlycoGridConversionUnitTest
    {
        [Test]
        public void Load_Select_Convert_Write()
        {
            //string fileName = @"D:\Csharp\ConosleApps\LocalServer\IQ\GlyQ-IQ\UnitTest\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_iqResults_777.txt";
            string fileName = @"D:\Csharp\ConosleApps\LocalServer\IQ\GlyQ-IQ\UnitTest\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_1_iqResults_3.txt";
            //step 1:  Load
            ImportGlyQResult importer = new ImportGlyQResult();
            List<GlyQIqResult> results = importer.Import(fileName);

            //step 2a:  Select
            List<GlyQIqResult> selectResultsCorrectGlycan = (from glycan in results where glycan.GlobalResult == true.ToString() && glycan.FinalDecision == EnumerationFinalDecision.CorrectGlycan.ToString() select glycan).ToList();

            //Step 3b.  convert
            List<string> linesCorrectGlycan = GlycoGridWrite.CreateGlycoGridFile(selectResultsCorrectGlycan, "green");

            //step 2b:  Select
            List<GlyQIqResult> selectResultsNonValidatedHit = (from glycan in results where glycan.GlobalResult == true.ToString() && glycan.FinalDecision == EnumerationFinalDecision.NonValidatedHit.ToString() select glycan).ToList();
            
            //Step 3b.  convert
            List<string> linesNonValidatedHit = GlycoGridWrite.CreateGlycoGridFile(selectResultsNonValidatedHit, "blue");

            //step 2b:  Select
            List<GlyQIqResult> selectResultsFutureTarget = (from glycan in results where glycan.GlobalResult == true.ToString() && glycan.FinalDecision == EnumerationFinalDecision.FutureTarget.ToString() select glycan).ToList();

            //Step 3b.  convert
            List<string> linesFutureTarget = GlycoGridWrite.CreateGlycoGridFile(selectResultsFutureTarget, "yellow");

            List<GlyQIqResult> selectValidatedGlycanFragment = (from glycan in results where glycan.GlobalResult == true.ToString() && glycan.FinalDecision == EnumerationFinalDecision.ValidatedGlycanFragment.ToString() select glycan).ToList();

            //Step 3b.  convert
            List<string> linesValidatedGlycanFragment = GlycoGridWrite.CreateGlycoGridFile(selectValidatedGlycanFragment, "red");

            //step 4 create factorsFile

            List<string> linesFactors = new List<string>();
            linesFactors.Add("Hexose,3,12");
            linesFactors.Add("NAcetylhexosamine,2,8");
            linesFactors.Add("Deoxyhexose,0,7");
            linesFactors.Add("NeuraminicAcid,0,9");
            string factorsFileName = "Factors_L10.txt";


            //step4: write
            string glycoGridDataFolder = @"D:\Csharp\ConosleApps\LocalServer\IQ\GlyQ-IQ\UnitTest\GlycoGridDataFolder";
            string glycoGridDataSetFolder = @"D:\Csharp\ConosleApps\LocalServer\IQ\GlyQ-IQ\UnitTest\GlycoGridDataFolder\TestDataSet";
            string fileNameToWriteCorrectGlycan = glycoGridDataSetFolder + @"\" + "CorrectGlycan.txt";
            string fileNameToWriteNonValidatedHit = glycoGridDataSetFolder + @"\" + "NonValidatedHit.txt";
            string fileNameToWriteFutureTarget = glycoGridDataSetFolder + @"\" + "FutureTarget.txt";
            string fileNameToWriteGlycanFragment = glycoGridDataSetFolder + @"\" + "GlycanFragment.txt";
            string fileNameToWriteFactors = glycoGridDataFolder + @"\" + factorsFileName;
            
            System.IO.Directory.CreateDirectory(glycoGridDataFolder);
            System.IO.Directory.CreateDirectory(glycoGridDataSetFolder);
            StringListToDisk writer = new StringListToDisk();

            writer.toDiskStringList(fileNameToWriteCorrectGlycan, linesCorrectGlycan);
            writer.toDiskStringList(fileNameToWriteNonValidatedHit, linesNonValidatedHit);
            writer.toDiskStringList(fileNameToWriteFutureTarget, linesFutureTarget);
            writer.toDiskStringList(fileNameToWriteGlycanFragment, linesValidatedGlycanFragment);
            writer.toDiskStringList(fileNameToWriteFactors, linesFactors);

            Assert.AreEqual(selectResultsCorrectGlycan.Count,2);
        }
    }
}
