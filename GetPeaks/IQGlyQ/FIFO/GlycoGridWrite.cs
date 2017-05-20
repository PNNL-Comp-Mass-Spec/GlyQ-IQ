using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GetPeaks_DLL.DataFIFO;
using IQGlyQ.Enumerations;

namespace IQGlyQ.FIFO
{
    public class GlycoGridWrite
    {
        public static void ExportToGlycoGridFormat(string fileName, string baseFolderName, string datasetFolderName, List<FragmentIQTarget> factors, string factorsFileNameIn)
        {
            //step 1:  Load
            ImportGlyQResult importer = new ImportGlyQResult();
            List<GlyQIqResult> results = importer.Import(fileName);

            List<GlyQIqResult> selectValidatedGlycanFragment;
            List<GlyQIqResult> selectResultsNonValidatedHit;
            List<GlyQIqResult> selectResultsCorrectGlycan;
            List<GlyQIqResult> selectResultsFutureTarget;
            SelectResultsCorrectGlycan(results, out selectValidatedGlycanFragment, out selectResultsNonValidatedHit, out selectResultsCorrectGlycan, out selectResultsFutureTarget);

            

            //Step 3.  convert
            List<string> linesCorrectGlycan = CreateGlycoGridFile(selectResultsCorrectGlycan, "green");
            List<string> linesNonValidatedHit = CreateGlycoGridFile(selectResultsNonValidatedHit, "blue");
            List<string> linesFutureTarget = CreateGlycoGridFile(selectResultsFutureTarget, "yellow");
            List<string> linesValidatedGlycanFragment = CreateGlycoGridFile(selectValidatedGlycanFragment, "red");

            //step 4 create factorsFile
            
            List<string> linesFactors = new List<string>();
            //linesFactors.Add("Hexose,3,12");
            //linesFactors.Add("NAcetylhexosamine,2,8");
            //linesFactors.Add("Deoxyhexose,0,7");
            //linesFactors.Add("NeuraminicAcid,0,9");
            //string factorsFileName = "Factors_L10.txt";

            //this needs to be fixed because the order is wrong
            foreach (var factor in factors)
            {
                string line = factor.DifferenceName + "," + factor.ScanInfo.Min + "," + factor.ScanInfo.Max;
                linesFactors.Add(line);
            }
            string factorsFileName = factorsFileNameIn;

            string factorsFileNameNoEnding = Regex.Replace(factorsFileNameIn, ".txt" + @"$", String.Empty);

            factorsFileNameNoEnding = Regex.Replace(factorsFileNameIn, @"^" + "Factors", String.Empty);

            datasetFolderName += factorsFileNameNoEnding;
            //step4: write
            //string baseFolderName = @"D:\Csharp\ConosleApps\LocalServer\IQ\GlyQ-IQ\UnitTest\GlycoGridDataFolder";
            //string datasetFolderName = @"D:\Csharp\ConosleApps\LocalServer\IQ\GlyQ-IQ\UnitTest\GlycoGridDataFolder\TestDataSet";


            string fileNameToWriteCorrectGlycan = datasetFolderName + @"\" + "CorrectGlycan.txt";
            string fileNameToWriteNonValidatedHit = datasetFolderName + @"\" + "NonValidatedHit.txt";
            string fileNameToWriteFutureTarget = datasetFolderName + @"\" + "FutureTarget.txt";
            string fileNameToWriteGlycanFragment = datasetFolderName + @"\" + "GlycanFragment.txt";

            //string fileNameToWriteCorrectGlycan = datasetFolderName + @"\" + "Quad.txt";
            //string fileNameToWriteNonValidatedHit = datasetFolderName + @"\" + "Tri.txt";
            //string fileNameToWriteFutureTarget = datasetFolderName + @"\" + "Future.txt";
            //string fileNameToWriteGlycanFragment = datasetFolderName + @"\" + "Frag.txt";

            //string fileNameToWriteCorrectGlycan = datasetFolderName + @"\" + "4.txt";
            //string fileNameToWriteNonValidatedHit = datasetFolderName + @"\" + "3.txt";
            //string fileNameToWriteFutureTarget = datasetFolderName + @"\" + "F.txt";
            //string fileNameToWriteGlycanFragment = datasetFolderName + @"\" + "X.txt";
            string fileNameToWriteFactors = baseFolderName + @"\" + factorsFileName;

            System.IO.Directory.CreateDirectory(baseFolderName);
            System.IO.Directory.CreateDirectory(datasetFolderName);
            StringListToDisk writer = new StringListToDisk();

            writer.toDiskStringList(fileNameToWriteCorrectGlycan, linesCorrectGlycan);
            writer.toDiskStringList(fileNameToWriteNonValidatedHit, linesNonValidatedHit);
            writer.toDiskStringList(fileNameToWriteFutureTarget, linesFutureTarget);
            writer.toDiskStringList(fileNameToWriteGlycanFragment, linesValidatedGlycanFragment);
            writer.toDiskStringList(fileNameToWriteFactors, linesFactors);

            
        }

        //public static void BreakUpFinalResultsIntoFiles(string fileName, string baseFolderName, string datasetFolderName, List<FragmentIQTarget> factors, string factorsFileName)
        
        public static void BreakUpFinalResultsIntoFiles(string fileName, string baseFolderName, string datasetFolderName, string factorsFileName)
        {
            baseFolderName += "ResultsBreakDown";

           

            //step 1:  Load
            ImportGlyQResult importer = new ImportGlyQResult();
            List<GlyQIqResult> results = importer.Import(fileName);

            //step 2:  break up final results
            GlyQIqResult headerResult = new GlyQIqResult();
            if (results != null && results.Count > 0)
            {
                headerResult = results[0];
            }
            else
            {
                Console.WriteLine("Failed To Load Results");
                Console.ReadKey();
            }

            List<GlyQIqResult> selectValidatedGlycanFragment;
            List<GlyQIqResult> selectResultsNonValidatedHit;
            List<GlyQIqResult> selectResultsCorrectGlycan;
            List<GlyQIqResult> selectResultsFutureTarget;
            SelectResultsCorrectGlycan(results, out selectValidatedGlycanFragment, out selectResultsNonValidatedHit, out selectResultsCorrectGlycan, out selectResultsFutureTarget);

            //step 3:  convert to string

            List<string> selectValidatedGlycanFragmentLines = ConvertToLines(selectValidatedGlycanFragment, headerResult);
            List<string> selectResultsNonValidatedHitLines = ConvertToLines(selectResultsNonValidatedHit, headerResult);
            List<string> selectResultsCorrectGlycanLines = ConvertToLines(selectResultsCorrectGlycan, headerResult);
            List<string> selectResultsFutureTargetLines = ConvertToLines(selectResultsFutureTarget, headerResult);

            //step 4 write
            if (!System.IO.Directory.Exists(baseFolderName))
            {
                System.IO.Directory.CreateDirectory(baseFolderName);
            }

            string factorsFileNameNoEnding = Regex.Replace(factorsFileName, ".txt" + @"$", String.Empty);

            factorsFileNameNoEnding = Regex.Replace(factorsFileName, @"^" + "Factors", String.Empty);

            string baseFileLocation = baseFolderName + @"\" + datasetFolderName + factorsFileNameNoEnding;
            if (!System.IO.Directory.Exists(baseFileLocation))
            {
                System.IO.Directory.CreateDirectory(baseFileLocation);
            }
            
            StringListToDisk writer = new StringListToDisk();



            writer.toDiskStringList(baseFileLocation + @"\" + datasetFolderName + "_" + "ValidatedGlycanFragment.txt", selectValidatedGlycanFragmentLines);
            writer.toDiskStringList(baseFileLocation + @"\" + datasetFolderName + "_" + "NonValidatedHit.txt", selectResultsNonValidatedHitLines);
            writer.toDiskStringList(baseFileLocation + @"\" + datasetFolderName + "_" + "CorrectGlycan.txt", selectResultsCorrectGlycanLines);
            writer.toDiskStringList(baseFileLocation + @"\" + datasetFolderName + "_" + "FutureTarget.txt", selectResultsFutureTargetLines);
        }

        private static List<string> ConvertToLines(List<GlyQIqResult> selectValidatedGlycanFragment, GlyQIqResult header)
        {
            List<string> selectValidatedGlycanFragmentLines = new List<string>();

            selectValidatedGlycanFragmentLines.Add(header.GlyQIqResultToString());

            foreach (GlyQIqResult result in selectValidatedGlycanFragment)
            {
                selectValidatedGlycanFragmentLines.Add(result.GlyQIqResultToString());
            }
            return selectValidatedGlycanFragmentLines;
        }

        public static List<string> CreateGlycoGridFile(List<GlyQIqResult> selectResults, string selectedColor)
        {
            List<string> lines = new List<string>();

            lines.Add(selectedColor);

            foreach (var glyQIqResult in selectResults)
            {
                
                List<int> composition = Utiliites.ConvertStringGlycanCodeToIntegers(glyQIqResult.Code);
                string seperator = ",";
                string line = "";
                for (int i = 0; i < composition.Count - 1; i++)
                {
                    line += composition[i] + seperator;
                }
                line += composition[composition.Count - 1];

                lines.Add(line);
            }

            return lines;
        }

        private static void SelectResultsCorrectGlycan(List<GlyQIqResult> results, out List<GlyQIqResult> selectValidatedGlycanFragment, out List<GlyQIqResult> selectResultsNonValidatedHit, out List<GlyQIqResult> selectResultsCorrectGlycan, out List<GlyQIqResult> selectResultsFutureTarget)
        {
            //step 2:  Select
            selectResultsCorrectGlycan = (from glycan in results
                                          where
                                              glycan.GlobalResult == true.ToString() &&
                                              glycan.FinalDecision == EnumerationFinalDecision.CorrectGlycan.ToString()
                                          select glycan).ToList();
            selectResultsNonValidatedHit = (from glycan in results
                                            where
                                                glycan.GlobalResult == true.ToString() &&
                                                glycan.FinalDecision == EnumerationFinalDecision.NonValidatedHit.ToString()
                                            select glycan).ToList();
            selectResultsFutureTarget = (from glycan in results
                                         where
                                             glycan.GlobalResult == true.ToString() &&
                                             glycan.FinalDecision == EnumerationFinalDecision.FutureTarget.ToString()
                                         select glycan).ToList();
            selectValidatedGlycanFragment = (from glycan in results
                                             where
                                                 glycan.GlobalResult == true.ToString() &&
                                                 glycan.FinalDecision ==
                                                 EnumerationFinalDecision.ValidatedGlycanFragment.ToString()
                                             select glycan).ToList();
        }

    }
}
