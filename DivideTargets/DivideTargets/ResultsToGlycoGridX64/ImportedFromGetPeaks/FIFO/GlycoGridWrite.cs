using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DivideTargetsLibraryX64;
using ResultsToGlycoGridX64.ImportedFromGetPeaks.Enumerations;
using ResultsToGlycoGridX64.ImportedFromGetPeaks.GetPeaks;
using ResultsToGlycoGridX64.ImportedFromGetPeaks.Objects;

namespace ResultsToGlycoGridX64.ImportedFromGetPeaks.FIFO
{
    public class GlycoGridWrite
    {
        public static void ExportToGlycoGridFormat(string fileName, string baseFolderName, string datasetFolderName, List<FragmentIQTarget64> factors, string factorsFileNameIn)
        {
            //step 1:  Load
            ImportGlyQResult importer = new ImportGlyQResult();
            List<GlyQIqResult> results = importer.Import(fileName);

            List<GlyQIqResult> selectValidatedGlycanFragment;
            List<GlyQIqResult> selectResultsNonValidatedHit;
            List<GlyQIqResult> selectResultsCorrectGlycan;
            List<GlyQIqResult> selectResultsFutureTarget;
            List<GlyQIqResult> selectResults4Var5Var;
            SelectResultsCorrectGlycan(results, out selectValidatedGlycanFragment, out selectResultsNonValidatedHit, out selectResultsCorrectGlycan, out selectResultsFutureTarget, out selectResults4Var5Var);

            

            //Step 3.  convert
            List<string> linesCorrectGlycan = CreateGlycoGridFile(selectResultsCorrectGlycan, "green");
            List<string> linesNonValidatedHit = CreateGlycoGridFile(selectResultsNonValidatedHit, "blue");
            List<string> linesFutureTarget = CreateGlycoGridFile(selectResultsFutureTarget, "yellow");
            List<string> linesValidatedGlycanFragment = CreateGlycoGridFile(selectValidatedGlycanFragment, "red");
            List<string> linesResults4Var5Var = CreateGlycoGridFile(selectResults4Var5Var, "orange");

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
            string fileNameToWrite4Var5Var = datasetFolderName + @"\" + "4Var5Var.txt";
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
            writer.toDiskStringList(fileNameToWrite4Var5Var, linesResults4Var5Var);
            
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
            List<GlyQIqResult> selectResults4Var5Var;
            SelectResultsCorrectGlycan(results, out selectValidatedGlycanFragment, out selectResultsNonValidatedHit, out selectResultsCorrectGlycan, out selectResultsFutureTarget, out selectResults4Var5Var);

            //step 3:  convert to string

            List<string> selectValidatedGlycanFragmentLines = ConvertToLines(selectValidatedGlycanFragment, headerResult);
            List<string> selectResultsNonValidatedHitLines = ConvertToLines(selectResultsNonValidatedHit, headerResult);
            List<string> selectResultsCorrectGlycanLines = ConvertToLines(selectResultsCorrectGlycan, headerResult);
            List<string> selectResultsFutureTargetLines = ConvertToLines(selectResultsFutureTarget, headerResult);
            List<string> selectResults4Var5VarLines = ConvertToLines(selectResults4Var5Var, headerResult);

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
            writer.toDiskStringList(baseFileLocation + @"\" + datasetFolderName + "_" + "4Var5Var.txt", selectResults4Var5VarLines);
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
                
                //List<int> composition =  ConvertStringGlycanCodeToIntegers(glyQIqResult.Code);
                //List<int> composition = Converter.ConvertStringGlycanCodeToIntegers(glyQIqResult.TargetID);
                List<int> composition = Converter.ConvertStringGlycanCodeToIntegers(glyQIqResult.Code);
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


        //public static List<int> ConvertStringGlycanCodeToIntegers(string code)
        //{
        //    //Hex:HexNAc:Fucose:SialicAcid
        //    //HH:N:F:SS  102124

        //    List<int> compositions = new List<int>();

        //    int codeAsInt = Convert.ToInt32(code);
        //    double codeAsDouble = Convert.ToDouble(codeAsInt);

        //    int preHexoseFactor = 10000;
        //    double preHexose = codeAsDouble / preHexoseFactor;

        //    int hexose = Convert.ToInt32(Math.Truncate(preHexose));
        //    compositions.Add(hexose);

        //    int preHexNAcFactor = 1000;
        //    double preHexNAc = (codeAsInt - hexose * preHexoseFactor) / preHexNAcFactor;

        //    int hexNAc = Convert.ToInt32(Math.Truncate(preHexNAc));

        //    compositions.Add(hexNAc);

        //    int preFucoseFactor = 100;
        //    double preFucose = (codeAsInt - hexose * preHexoseFactor - hexNAc * preHexNAcFactor) / preFucoseFactor;

        //    int fucose = Convert.ToInt32(Math.Truncate(preFucose));
        //    compositions.Add(fucose);

        //    int preSialicAcidFactor = 1;
        //    double preSialicAcid = (codeAsInt - hexose * preHexoseFactor - hexNAc * preHexNAcFactor - fucose * preFucoseFactor) / preSialicAcidFactor;

        //    int sialicAcid = Convert.ToInt32(Math.Truncate(preSialicAcid));
        //    compositions.Add(sialicAcid);

        //    return compositions;
        //}


        private static void SelectResultsCorrectGlycan(
            List<GlyQIqResult> results,
            out List<GlyQIqResult> selectValidatedGlycanFragment,
            out List<GlyQIqResult> selectResultsNonValidatedHit,
            out List<GlyQIqResult> selectResultsCorrectGlycan,
            out List<GlyQIqResult> selectResultsFutureTarget,
            out List<GlyQIqResult> selectResults4Var5Var)
        {
            //step 2:  Select
            List<GlyQIqResult> trueGlycanResults = (from glycan in results
                                                    where
                                                        glycan.GlobalResult == true.ToString() || glycan.GlobalResult == "TRUE"
                                                    select glycan).ToList();

            selectResultsCorrectGlycan = (from glycan in trueGlycanResults
                                          where
                                              glycan.FinalDecision == EnumerationFinalDecision.CorrectGlycan.ToString()
                                          select glycan).ToList();
            selectResultsNonValidatedHit = (from glycan in trueGlycanResults
                                            where
                                                glycan.FinalDecision == EnumerationFinalDecision.NonValidatedHit.ToString()
                                            select glycan).ToList();
            selectResultsFutureTarget = (from glycan in trueGlycanResults
                                         where
                                             glycan.FinalDecision == EnumerationFinalDecision.FutureTarget.ToString()
                                         select glycan).ToList();
            selectValidatedGlycanFragment = (from glycan in trueGlycanResults
                                             where
                                                glycan.FinalDecision == EnumerationFinalDecision.ValidatedGlycanFragment.ToString()
                                             select glycan).ToList();

            selectResultsCorrectGlycan = selectResultsCorrectGlycan.OrderBy(n => n.MonomassTheor).ToList();
            selectResultsNonValidatedHit = selectResultsNonValidatedHit.OrderBy(n => n.MonomassTheor).ToList();
            selectResultsFutureTarget = selectResultsNonValidatedHit.OrderBy(n => n.MonomassTheor).ToList();
            selectValidatedGlycanFragment = selectValidatedGlycanFragment.OrderBy(n => n.MonomassTheor).ToList();

            
            selectResults4Var5Var = selectResultsNonValidatedHit.ToList();
            selectResults4Var5Var.AddRange(selectResultsCorrectGlycan);//add correct glycans to nonvalidated evn if they do not overlap
            
            //selectResultsCorrectGlycan = (from glycan in results
            //                              where
            //                                  glycan.GlobalResult == true.ToString() || 
            //                                  glycan.GlobalResult == "TRUE" &&
            //                                  glycan.FinalDecision == EnumerationFinalDecision.CorrectGlycan.ToString()
            //                              select glycan).ToList();
            //selectResultsNonValidatedHit = (from glycan in results
            //                                where
            //                                    glycan.GlobalResult == true.ToString() || glycan.GlobalResult == "TRUE" &&
            //                                    glycan.FinalDecision == EnumerationFinalDecision.NonValidatedHit.ToString()
            //                                select glycan).ToList();
            //selectResultsFutureTarget = (from glycan in results
            //                             where
            //                                 glycan.GlobalResult == true.ToString() || glycan.GlobalResult == "TRUE" &&
            //                                 glycan.FinalDecision == EnumerationFinalDecision.FutureTarget.ToString()
            //                             select glycan).ToList();
            //selectValidatedGlycanFragment = (from glycan in results
            //                                 where
            //                                    glycan.GlobalResult == true.ToString() || glycan.GlobalResult == "TRUE" &&
            //                                     glycan.FinalDecision ==
            //                                     EnumerationFinalDecision.ValidatedGlycanFragment.ToString()
            //                                 select glycan).ToList();
        }

    }
}
