using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivideTargetsLibraryX64.FromGetPeaks;
using ResultsToGlycoGridX64.ImportedFromGetPeaks;
using ResultsToGlycoGridX64.ImportedFromGetPeaks.FIFO;
using ResultsToGlycoGridX64.ImportedFromGetPeaks.Objects;

namespace ResultsToGlycoGridX64
{
    class Program
    {
        static void Main(string[] args)
        {
            
            if (args.Length != 5)
            {
                Console.WriteLine("Missing Args for GlycoGrid");
                Console.ReadKey();
            }
            string resultsFileName = args[0];
            string generalResultsFolder = args[1];
            string resultsBaseName = args[2];
            string factorsName = args[3];
            string factorsPath = args[4];
            //load in factors

            bool ovverrideArgs = false;
            if(ovverrideArgs)
            {
                //"\\picfs\projects\DMS\PIC_HPC\Home\Results store\Gly09_Velos3_Jaguar_200nL_C12_SN129_3X_23Dec12_Global_iqResults.txt" "\\picfs\projects\DMS\PIC_HPC\Home\Results store" Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12 Factors_PSA.txt ""\\picfs\projects\DMS\PIC_HPC\Home\WorkingParameters"
                //"\\picfs\projects\DMS\PIC_HPC\Hot\FR_V_SN129_1\ResultsSummary\V_SN129_1_Family_iqResults.txt" "\\picfs\projects\DMS\PIC_HPC\Hot\FR_V_SN129_1" FR_V_SN129_1 Factors_L10PSA.txt "\\picfs\projects\DMS\PIC_HPC\Hot\FR_V_SN129_1\WorkingParameters"
                
                //"GlyQ-IQ_ToGlycoGrid\Release\ResultsToGlycoGridX64.exe"
                //"\\picfs\projects\DMS\PIC_HPC\Hot\FB_ESI_Cell_Norm1_26Nov13_40uL_230nL_C14_1\ResultsSummary\ESI_Cell_Norm1_26Nov13_40uL_230nL_C14_1_Family_iqResults.txt"
                //"\\picfs\projects\DMS\PIC_HPC\Hot\FB_ESI_Cell_Norm1_26Nov13_40uL_230nL_C14_1"
                //ESI_Cell_Norm1_26Nov13_40uL_230nL_C14_1 Factors_L10PSA.txt
                //"\\picfs\projects\DMS\PIC_HPC\Hot\FB_ESI_Cell_Norm1_26Nov13_40uL_230nL_C14_1\WorkingParameters"

                //string folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\FE_ESI_Cell_Norm1_26Nov13_40uL_230nL_C14_1";
                //resultsBaseName = "ESI_Cell_Norm1_26Nov13_40uL_230nL_C14_1 Factors_L10PSA.txt";
                //string resultName = "FilteredResults.txt";

                string folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\Fi_V_SN129_1";
                resultsBaseName = "Fi_V_SN129_1";
                string resultName = "V_SN129_1_Family_iqResults.txt";
                //string resultName = "S_SN129_1_Family_iqResults.txt";
                
                resultsFileName = folder + @"\" + "ResultsSummary" + @"\" + resultName;
                generalResultsFolder = folder;
                



                factorsPath =folder + @"\" + "WorkingParameters";
            
            }



            //List<FragmentIQTarget> factors = RunMeIQGlyQ.factorsToFragments(factorsPath + @"\" + factorsName, false);
            List<FragmentIQTarget64> factors = ConvertFactorToIQTargets.FactorsToFragments(factorsPath + @"\" + factorsName, false);

            //set up simple 1,1,1,1 files for GlycoGrid
            string baseFolderName = generalResultsFolder + @"\" + "G_Grid";
            string datasetFolderName = generalResultsFolder + @"\" + "G_Grid" + @"\" + resultsBaseName;
            Console.WriteLine("Start GlycoGridWrite...");
            Console.WriteLine("step1... CreateGridFiles");
            GlycoGridWrite.ExportToGlycoGridFormat(resultsFileName, baseFolderName, datasetFolderName, factors, factorsName);
            Console.WriteLine("step1... Create SIPPER Files");
            Console.WriteLine(Environment.NewLine + "Converting to GlycoGrid and Seperate Results files..." + Environment.NewLine);

            GlycoGridWrite.BreakUpFinalResultsIntoFiles(resultsFileName, baseFolderName, resultsBaseName, factorsName);
        }
    }
}
