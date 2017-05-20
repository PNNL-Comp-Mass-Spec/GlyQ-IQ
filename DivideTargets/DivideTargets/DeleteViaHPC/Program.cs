using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DivideTargetsLibraryX64.Parameters;
using DivideTargetsLibraryX64.FromGetPeaks;

namespace DeleteViaHPC
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up delete list creation...");
            //step 1 check if important files are present


            //step 2 load in files to delete
            //string HPCPrepName = @"\\picfs\projects\DMS\PIC_HPC\Hot\FO_Peptide_Merc_08_2Feb11_Sphinx_11-01-17_1\0y_HPC_OperationParameters_2068757711.txt";
            string HPCPrepName = args[0];

            bool overrideArgs = false;
            if(overrideArgs)
            {
                HPCPrepName = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_Std_V10_PsaLac_ESI_SN138_21Dec_C15_2530_1\0y_HPC_OperationParameters_1828401790.txt";
            
            }

            HPCPrepParameters hpcParameters = new HPCPrepParameters();
            hpcParameters.LoadParameters(HPCPrepName);

            int cores = hpcParameters.cores;

            string targetRoot = hpcParameters.TargetsNoEnding;
            string dataSetRoot = hpcParameters.DatasetFileNameNoEnding;

            string workingDirectory = hpcParameters.WorkingDirectory;
            string workingFolder = hpcParameters.WorkingFolder;
            string workingParametersPath = workingDirectory + @"\" + workingFolder + @"\" + "WorkingParameters";
            

            // base
            string targetBaseFolder = workingParametersPath;
            string locksBaseFolder = workingParametersPath + @"\" + "LocksFolder";
            string resultBaseFolder = workingDirectory + @"\" + workingFolder + @"\" + "Results" + @"\" + "Results" + "_" + hpcParameters.DatasetFileNameNoEnding;
            string workingApplicationBase = hpcParameters.WorkingDirectoryForExe;


            int goodFiles = 0;
            Console.WriteLine(HPCPrepName);
            if (File.Exists(HPCPrepName)) { goodFiles++; Console.WriteLine("--Pass"); }

            string resultListFile = "HPC_ResultList_" + dataSetRoot + ".txt";
            string resultListPath = workingParametersPath + @"\" + resultListFile;
            bool resultsFilePresent = false;
            Console.WriteLine(resultListPath);
            if (File.Exists(resultListPath)) { goodFiles++; resultsFilePresent = true; Console.WriteLine("--Pass"); }








            //targets
            List<string> targetsFiles = TargetsFiles(targetRoot, cores, targetBaseFolder);

            //locks
            List<string> locksFiles = LocksFiles(cores, locksBaseFolder);

            //results file
            List<string> resultFiles = new List<string>();
            if (resultsFilePresent)
            {
                resultFiles = ResultFiles(workingParametersPath, dataSetRoot, resultBaseFolder);
            }

            //data file
            List<string> dataFiles = bonusFiles(workingDirectory + @"\" + workingFolder,hpcParameters.DatasetFileNameNoEnding,".raw");

            //application folders
            List<string> applicationFolders = ApplicationFolders(workingApplicationBase);

            //results file
            List<string> extraFolders = bonusFolders(workingDirectory + @"\" + workingFolder);

            //check
            Console.WriteLine("we have " + targetsFiles.Count + " targetsFiles files to delete");
            Console.WriteLine("we have " + locksFiles.Count + " locks files to delete");
            Console.WriteLine("we have " + resultFiles.Count + " results files to delete");
            Console.WriteLine("we have " + applicationFolders.Count + " application FOLDERS to delete");
            Console.WriteLine("we have " + extraFolders.Count + " application FOLDERS to delete");
            Console.WriteLine("we have " + dataFiles.Count + " rawDataFiles files to delete");

            //append all toegher
            List<string> masterFileDeleteList = new List<string>();
            masterFileDeleteList.AddRange(dataFiles);//delete this first because it is a large file
            masterFileDeleteList.AddRange(targetsFiles);
            masterFileDeleteList.AddRange(locksFiles);
            masterFileDeleteList.AddRange(resultFiles);

            List<string> masterFolderDeleteList = new List<string>();
            masterFolderDeleteList.AddRange(applicationFolders);
            masterFolderDeleteList.AddRange(extraFolders);

            Console.WriteLine("Checking for files and folders on disk...");

            bool checkFilesOnDisk = false;
            if (true)
            {
                List<string> missingFiles = new List<string>();
                int detectableFiles = 0;
                foreach (string file in masterFileDeleteList)
                {
                    if (File.Exists(file)) {detectableFiles++;}
                    else {missingFiles.Add(file);}
                }
                int totalToFilesDelete = targetsFiles.Count + locksFiles.Count + resultFiles.Count;
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("We found " + detectableFiles + " out of " + totalToFilesDelete + " files");
                if (missingFiles.Count > 0)
                {
                    Console.WriteLine("One such missing file is" + missingFiles[0]);
                }

                Console.WriteLine(Environment.NewLine);
                List<string> missingFolders = new List<string>();
                int detectableFolders = 0;
                foreach (string folder in masterFolderDeleteList)
                {
                    if (Directory.Exists(folder)) { detectableFolders++; }
                    else { missingFolders.Add(folder); }
                }

                int totalToFoldersDelete = applicationFolders.Count;
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("We found " + detectableFolders + " out of " + totalToFoldersDelete + " folders");
                if (missingFolders.Count > 0)
                {
                    Console.WriteLine("One such missing file is" + missingFolders[0]);
                }
            }

            //write delete file lists to disk and count to iterate from
            StringListToDisk writer = new StringListToDisk();
            string pathForDeleteFiles = workingParametersPath + @"\" + "HPC_DeleteFilesAndFolders.txt";
            
            List<string> linesToWrite = new List<string>();
            int totalFilesandFoldersToDelete = masterFileDeleteList.Count + masterFolderDeleteList.Count;
            linesToWrite.Add("Total" + "," + totalFilesandFoldersToDelete);

            //do folders first.  generally larger blocks.  better to start first
            foreach (var folder in masterFolderDeleteList)
            {
                linesToWrite.Add("Directory" + "," + folder);
            }
            
            //then files 
            foreach (var file in masterFileDeleteList)
            {
                linesToWrite.Add("File" + "," + file);
            }

            
            writer.toDiskStringList(pathForDeleteFiles,linesToWrite);

            Console.WriteLine("Delete like the wind!");
        }

        private static List<string> ApplicationFolders(string workingApplicationFolder)
        {
            List<string> applicationFolders = new List<string>();
            applicationFolders.Add("GlyQ-IQ Application_Setup");
            applicationFolders.Add("GlyQ-IQ_Application");
            applicationFolders.Add("GlyQ-IQ_CheckFile");
            applicationFolders.Add("GlyQ-IQ_CombineNodeResults");
            applicationFolders.Add("GlyQ-IQ_Compositions");
            applicationFolders.Add("GlyQ-IQ_DeleteFiles");
            applicationFolders.Add("GlyQ-IQ_DivideTargets");
            applicationFolders.Add("GlyQ-IQ_DivideTargetsNode");
            applicationFolders.Add("GlyQ-IQ_ThermoDLL");
            applicationFolders.Add("GlyQ-IQ_HPC_Check");
            applicationFolders.Add("GlyQ-IQ_HPC_DeleteCloud");
            applicationFolders.Add("GlyQ-IQ_HPC_DeleteFilesList");
            applicationFolders.Add("GlyQ-IQ_HPC_JobCreation");
            applicationFolders.Add("GlyQ-IQ_HPCListCombineMaker");
            applicationFolders.Add("GlyQ-IQ_MultiSleep");
            applicationFolders.Add("GlyQ-IQ_PostProcessing");
            applicationFolders.Add("GlyQ-IQ_SingleLinkage");
            applicationFolders.Add("GlyQ-IQ_Sleep");
            applicationFolders.Add("GlyQ-IQ_Timer");
            applicationFolders.Add("GlyQ-IQ_ToGlycoGrid");
            applicationFolders.Add("GlyQ-IQ_ToGlycoGrid86");
            applicationFolders.Add("GlyQ-IQ_UnZip");
            applicationFolders.Add("GlyQ-IQ_WriteHPCFiles");
            applicationFolders.Add("GlyQ-IQ_Zip");

            for (int i = 0; i < applicationFolders.Count;i++)
            {
                applicationFolders[i] = workingApplicationFolder + @"\" + applicationFolders[i];
            }

            return applicationFolders;
        }

        private static List<string> bonusFolders(string workingFolder)
        {
            List<string> folders = new List<string>();
            folders.Add("RemoteThermo");
            

            for (int i = 0; i < folders.Count; i++)
            {
                folders[i] = workingFolder + @"\" + folders[i];
            }

            return folders;
        }

        private static List<string> bonusFiles(string workingFolder, string dataFileName, string dataFileEnding)
        {
            List<string> files = new List<string>();
            files.Add("RawData" + @"\" + dataFileName + dataFileEnding);


            for (int i = 0; i < files.Count; i++)
            {
                files[i] = workingFolder + @"\" + files[i];
            }

            return files;
        }


        private static List<string> ResultFiles(string workingParametersPath, string dataSetRoot, string resultBaseFolder)
        {
            string resultListFile = "HPC_ResultList_" + dataSetRoot + ".txt";
            string resultListPath = workingParametersPath + @"\" +  resultListFile;
            List<string> resultFiles = new List<string>();

            StringLoadTextFileLine reader = new StringLoadTextFileLine();
            List<string> lines = reader.SingleFileByLine(resultListPath);
            //parse lines
            char[] splitter = new char[] {'\\'};
            foreach (var dataToSplit in lines)
            {
                string[] words = dataToSplit.Split(splitter, StringSplitOptions.None);
                if (words.Length == 2)
                {
                    resultFiles.Add(resultBaseFolder + @"\" + words[1]);
                }
            }
            return resultFiles;
        }

        private static List<string> LocksFiles(int cores, string locksBaseFolder)
        {
            List<string> locksFiles = new List<string>();
            for (int i = 1; i <= cores; i++)
            {
                locksFiles.Add(locksBaseFolder + @"\" + "Lock" + "_" + i + ".txt");
            }
            return locksFiles;
        }

        private static List<string> TargetsFiles(string targetRoot, int cores, string targetBaseFolder)
        {
            List<string> targetsFiles = new List<string>();
            for (int i = 1; i <= cores; i++)
            {
                targetsFiles.Add(targetBaseFolder + @"\" + targetRoot + "_" + i + ".txt");
            }
            return targetsFiles;
        }
    }
}
