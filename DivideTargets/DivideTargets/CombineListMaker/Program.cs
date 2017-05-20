using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SleepDLL;

namespace CombineListMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            //"Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_iqResults_" ".txt" 1 2 "F:\ScottK\ToPIC\WorkingParameters\HPCResultList.txt" "F:\ScottK\ToPIC\WorkingParameters\HPC_ConsolidationParameterForAllNodes.txt" "F:\ScottK\ToPIC\Results"
            
            string baseNameofResult = args[0];
            string baseNameofResultEnding = args[1];
            string intStart = args[2];
            string intStop = args[3];
            string HPCResultListName = args[4];
            string HPCConsolidateName = args[5];
            string resultsPath = args[6];

            SleepDLL.StringListToDisk writer = new StringListToDisk();

            int start = Convert.ToInt32(intStart);
            int stop = Convert.ToInt32(intStop);

            List<string> files = new List<string>();
            for(int i = start;i<=stop;i++)
            {
                //string line = "Results_" + baseNameofResult +"_" + 1 + @"\" + baseNameofResult + "_" + 1 + "_iqResults_" + i + baseNameofResultEnding;
                string line = "Results_" + baseNameofResult + @"\" + baseNameofResult + "_iqResults_" + i + baseNameofResultEnding;
                files.Add(line);
            }

            writer.toDiskStringList(HPCResultListName,files);


            List<string> consolidateFiles = new List<string>();

            consolidateFiles.Add("Output," + resultsPath + @"\" + "ResultsSummary" + @"\" + baseNameofResult + "_Global_iqResults.txt");

            //for (int i = 0; i < files.Count; i++)
            //{
            //    string line = "Input," + resultsPath +@"\"+ files[i];
            //    consolidateFiles.Add(line);
            //}

            for (int i = start; i <= stop; i++)
            {
                string line = "Input," + resultsPath + @"\" + "Results" + @"\" + "Results_" + baseNameofResult + @"\" + baseNameofResult + "_iqResults_" + i + baseNameofResultEnding;
                //string line = "Input," + resultsPath + @"\" + "Results" + @"\" + "Results_" + baseNameofResult + "_" + 1 + @"\" + baseNameofResult + "_" + 1 + "_iqResults_" + i + baseNameofResultEnding;
                //string line = "Input," + resultsPath + @"\" + "Results" + @"\" + "Results_" + baseNameofResult + @"\" + baseNameofResult + "_" + 1 + "_iqResults_" + i + baseNameofResultEnding;
                consolidateFiles.Add(line);
            }

            writer.toDiskStringList(HPCConsolidateName, consolidateFiles);
        }
    }
}
