using System;
using System.Collections.Generic;
using System.Linq;

namespace IQGlyQ.Objects
{
    public static class SmallScoreCalculator
    {
        public static int SelectForScore(List<string> simpleOutput, List<SmallDHResult> globalHitsIndexes, out SmallDHResult hitToReport)
        {
            int firstLine = 1;
            foreach (var line in simpleOutput)
            {
                if (firstLine == 0)
                {
                    string[] words = line.Split('\t');

                    if (words.Length == 90)//as set by headder
                    {
                        SmallDHResult miniResult = new SmallDHResult();
                        miniResult.Code = words[1];
                        miniResult.Scan = words[11];
                        miniResult.GlobalResult = words[36];
                        miniResult.FinalDecision = words[38];

                        Console.WriteLine();
                        Console.WriteLine("Version: " + 2.03);
                        Console.WriteLine("Code: " + words[1]);
                        Console.WriteLine("Scan: " + words[11]);
                        Console.WriteLine("GlobalResult: " + words[36]);
                        Console.WriteLine("FinalDecision: " + words[38]);
                        Console.WriteLine("H12: " + words[78]);
                        Console.WriteLine("D12: " + words[83]);
                        Console.WriteLine("DHratio: " + words[49]);

                        if (miniResult.GlobalResult == "True")
                        {
                            double H12 = Convert.ToDouble(words[78]);
                            miniResult.H_12 = H12;

                            double D12 = Convert.ToDouble(words[83]);
                            miniResult.D_12 = D12;

                            double DHratio = Convert.ToDouble(words[49]);
                            miniResult.DHratio = DHratio;

                            double HMono = Convert.ToDouble(words[54]);
                            miniResult.HMono = HMono;

                            if (H12 > 0 && DHratio > 0 && HMono > 0)
                            {
                                miniResult.DMono = miniResult.HMono/miniResult.H_12*miniResult.DHratio;
                            }
                            else
                            {
                                miniResult.DMono = 0;
                            }


                            if (miniResult.GlobalResult == "True")
                            {
                                globalHitsIndexes.Add(miniResult);
                            }
                        }
                    }
                }
                else
                {
                    firstLine = 0;
                }
            }

            bool correctGlycanPresent = globalHitsIndexes.Any(item => item.FinalDecision == "CorrectGlycan");
            bool nonValidatedHit = globalHitsIndexes.Any(item => item.FinalDecision == "NonValidatedHit");

            int score = 0;
            hitToReport = new SmallDHResult();
            if (nonValidatedHit)
            {
                SmallDHResult winnersNVH = globalHitsIndexes.Where(item => item.FinalDecision == "NonValidatedHit").ToList().OrderByDescending(n => n.HMono).FirstOrDefault();
                if (winnersNVH != null)
                {
                    score = 1;
                    hitToReport.HMono = winnersNVH.HMono; //or H12
                    hitToReport.DMono = winnersNVH.DMono; //or D12
                    hitToReport.H_12 = winnersNVH.H_12; //or H12
                    hitToReport.D_12 = winnersNVH.D_12; //or D12
                    hitToReport.FinalDecision = "NonValidatedHit";
                }
            }
            if (correctGlycanPresent)
            {
                SmallDHResult winnersCG = globalHitsIndexes.Where(item => item.FinalDecision == "CorrectGlycan").ToList().OrderByDescending(n => n.HMono).FirstOrDefault();
                if (winnersCG != null)
                {
                    score = 2;
                    hitToReport.HMono = winnersCG.HMono; //or H12
                    hitToReport.DMono = winnersCG.DMono; //or D12
                    hitToReport.H_12 = winnersCG.H_12; //or H12
                    hitToReport.D_12 = winnersCG.D_12; //or D12
                    hitToReport.FinalDecision = "CorrectGlycan";
                }
            }

            return score;
        }


    }
}



//        public static void WriteResultToBrain(GlyqIqTask glyqIqTask, DistributionParameters distributionParameters, int count, double version)
//        {
//            StringListToDisk writer = new StringListToDisk();
//            string filename = distributionParameters.RootFolder + @"\" + "ResultOutPut_" + glyqIqTask.Id + ".txt";
//            List<string> lines = new List<string>();
//            lines.Add("messing with output");
//            lines.Add("RootFolder: " + distributionParameters.RootFolder);
//            lines.Add("DatasetID: " + distributionParameters.DataSetInfo.Id);
//            lines.Add("GeographicLocation: " + distributionParameters.GeographicLocation);
//            lines.Add("DatasetID: " + glyqIqTask.Id);
//            lines.Add("IQGlyQVersion: " + version);
//            lines.Add("ResultCount: " + count);

//            writer.toDiskStringList(filename, lines);
//        }
//}
