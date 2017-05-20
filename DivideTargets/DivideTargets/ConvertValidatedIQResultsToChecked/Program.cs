using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivideTargetsLibraryX64.Combine;
using DivideTargetsLibraryX64.FromGetPeaks;

namespace ConvertValidatedIQResultsToChecked
{
    class Program
    {
        static void Main(string[] args)
        {

            string fileNoEnding;//arg0
            string validateddataPath;//arg1

            string validatedDataExtenstion = "_validated";
            string outputFileAddOn = "Family_iqResults";
            bool ovverrideParams = false;
            if (ovverrideParams)
            {
                validateddataPath = @"D:\PNNL\Projects\GlyQ-IQ Paper\Data November\FE_S_SN129_1\ResultsSummaryPlay";
                fileNoEnding = "S_SN129_1";

                validateddataPath = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_CA_V10P007vS100v1_SPIN_CAcid660uM_19Dec13_L28_140C_C16_1\ResultsSummary";
                fileNoEnding = "SPIN_CAcid660uM_19Dec13_L28_140C_C16_1";
            }
            else
            {
                fileNoEnding = args[0];//"S_SN129_1";
                //output file name for data that has atleast one family member
                validateddataPath = args[1];
            }

            string nonModifiedFile = validateddataPath + @"\" + fileNoEnding + "_" + outputFileAddOn + ".txt";

            string fullValidateFileName = fileNoEnding + "_" + outputFileAddOn + validatedDataExtenstion + ".txt";
            string fullValidatedPath = validateddataPath + @"\" + fullValidateFileName;

            string OutputFileName = fileNoEnding + "_" + outputFileAddOn + "_" + "Check.txt";
            string fullOutputFilePath = validateddataPath + @"\" + OutputFileName;
            
                
            //step 1 read in validated file

            ResultsFilesCompostion loader = new ResultsFilesCompostion();
            
            string firstLineRejected = null;
            List<DataHolderForSort> dataPileReject = loader.loadDataReject(fullValidatedPath, out firstLineRejected);


           
            //step 2 merge and remove those labled "no" from sipper

            List<string> keepArray = new List<string>();
            List<string> keepCompositionArray = new List<string>();
            foreach (var manualAnnotation in dataPileReject)
            {
                keepArray.Add(manualAnnotation.SortKeyString2);
                keepCompositionArray.Add(manualAnnotation.SortKeyString);
            }

            foreach (var manualAnnotation in dataPileReject)
            {
                manualAnnotation.LineOfText = null;
                manualAnnotation.SortKeyComposition = null;
                manualAnnotation.SortKeyString = null;
                manualAnnotation.SortKeyString2 = null;
            }

            dataPileReject = null;//free up menory

            //step 3 read in clean results file before removed data

            
            string firstLineClean = null;
            List<DataHolderForSort> dataPileClean = loader.loadData2(nonModifiedFile, out firstLineClean);


            int checkCount = 0;
            for(int i=0;i< dataPileClean.Count;i++)
            {
                if(dataPileClean[i].SortKeyComposition!=keepCompositionArray[i])
                {
                    checkCount++;
                }
            }
            if (checkCount>0)
            {
                Console.WriteLine("files do not match");
                Console.ReadKey();
            }
            //check compositions are consistant between files



            //List<DataHolderForSort> dataPileSortedByComposition = dataPile2.OrderBy(p => p.SortKeyComposition).ToList();
            //step 4 remove bad records

            if (keepArray.Count == dataPileClean.Count)
            {
                for (int i = dataPileClean.Count - 1; i > 0; i--)
                {
                    string toKeep = keepArray[i];

                    if (toKeep == "No")
                    {
                        DataHolderForSort tempBadData = dataPileClean[i];
                        Console.WriteLine(tempBadData.SortKeyComposition + " is bad?...");

                        dataPileClean.RemoveAt(i);
                    }
                }
            }

            //step 5 write new file

            Console.WriteLine("Writing data...");

            List<string> stringsToWrite = new List<string>();
            stringsToWrite.Add(firstLineClean);
            for (int i = 0; i < dataPileClean.Count; i++)
            {
                stringsToWrite.Add(dataPileClean[i].LineOfText);
            }
            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(fullOutputFilePath, stringsToWrite);

            Console.WriteLine("Done!");
        }
    }
}
