using System;
using System.Collections.Generic;
using DivideTargetsLibraryX64;
using DivideTargetsLibraryX64.FromGetPeaks;
using DivideTargetsLibraryX64.Parameters;

namespace DivideTargets
{
    class Program
    {
        static void Main(string[] args)
        {
            //TO PIC build location
            //F:\ScottK\ToPIC\GlyQ-IQ Application\Release\


            //args
            //"D:\PNNL CSharp1\SVN Divide Targets\DivideTargets\DivideTargets\Parameters_DivideTargets.txt"
            //"F:\ScottK\ToPIC\WorkingParameters\Parameters_DivideTargetsPIC.txt"//current


            //"E:\ScottK\IQ\RunFiles\L_10_IQ_TargetsFirst3.txt" "E:\ScottK\GetPeaks Data\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12.raw" ".raw" "2"  "R:\RAM Files\GetPeaks\IQGlyQ_Console\bin\Release\IQGlyQ_Console.exe" "E:\ScottK\IQ\RunFiles" "E:\ScottK\IQ\NewIQRunFiles\GlyQIQ_Diabetes_Parameters.txt"
            //"E:\ScottK\IQ\RunFiles\L_10_IQ_TargetsFirst3.txt" "Z:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw" ".raw" "2" "R:\PNNL RAM 500\GetPeaks\IQGlyQ_Console\bin\Release\IQGlyQ_Console.exe" "E:\ScottK\IQ\RunFiles"
            //"E:\ScottK\IQ\RunFiles\L_10_IQ_TargetsFirst3.txt" "Z:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw" ".raw" "2" "R:\PNNL RAM 500\GetPeaks\IQGlyQ_Console\bin\Release\IQGlyQ_Console.exe" "E:\ScottK\IQ\RunFiles" "E:\ScottK\IQ\NewIQRunFiles\GlyQIQ_Diabetes_Parameters.txt"

            string divideTargetsParameterFile = args[0];

            const bool ovverideArgs = false;
            if(ovverideArgs)
            {
                divideTargetsParameterFile = @"\\picfs\projects\DMS\PIC_HPC\Hot\FQ_Gly09_Velos3_Jaguar_200nL_C12_AntB1_3X_25Dec12_1\HPC-Parameters_DivideTargetsPIC_Asterisks.txt";
            }

            string baseTargetsFile;
            string fullTargetPath;
            string textFileEnding;
            ParameterDivideTargets parameters = LoadParameters.SetupDivideTargetParameters(divideTargetsParameterFile, out baseTargetsFile, out fullTargetPath, out textFileEnding);


            //3.  setable parameters
            bool copyDataFile = Convert.ToBoolean(parameters.DuplicateDataBool);
            const bool copyTargetsFolder = true;
            const bool writeBatchFile = false;
            const bool writeLocksFile = false;

            int cores = Converter.ConvertStringToInt(parameters.CoresString);
            var locks = new List<string>();

            Console.WriteLine("We are going to copy over " + cores + " cores" + Environment.NewLine);
            Console.WriteLine("copyDataFile: " + copyDataFile);
            Console.WriteLine("copyTargetsFolder: " + copyTargetsFolder);
            Console.WriteLine("writeBatchFile: " + writeBatchFile);
            Console.WriteLine(Environment.NewLine);

            //4.  setup AppExecuteObject.  everything we need to run the console.  we pass this object like a results object and populate the fields
            var runLines = new List<AppExecuteObject>();
            //for (int i = 0; i < cores; i++)
            for (int i = 1; i <= cores; i++)
            {
                var appObject = new AppExecuteObject
                {
	                AppLocationPath = parameters.AppIqGlyQConsoleLocationPath,
	                DataFolder = parameters.DataFileFolder,
	                iQparameterFile = parameters.GlyQIQparameterFile,
	                ResultsLocation = parameters.OutputLocationPath,
	                LockFile = "Lock_" + i
                };
	            locks.Add(appObject.LockFile);
                //appObject.ResultsLocation = parameters.OutputLocationPath + "_" + i;
                runLines.Add(appObject);

            }

            //5.  do work

            if (copyDataFile)
            {
                Console.WriteLine("Duplicating Data Files...");
                //DataFileSetup(cores, parameters, ref runLines);
                List<string> dividedDataFileNames;
                List<string> dividedDataFileEnding;
                DataFileSetupForMultipleCopies.DataFileSetup(cores, parameters, out dividedDataFileNames, out dividedDataFileEnding);
                AssignDataFileNames(dividedDataFileNames, dividedDataFileEnding, runLines, cores);
            }

            if (copyTargetsFolder)
            {
                List<string> dividedTargetsFile;
                DivideTargetsFile.TargetsFolderSetup(fullTargetPath, parameters.OutputLocationPath, cores, baseTargetsFile, textFileEnding, out dividedTargetsFile);
                AssignTargetsFileNames(dividedTargetsFile, runLines, cores);
            }


            if (writeBatchFile)
            {
                Console.WriteLine("Writing Batch Files...");
                StringListToDisk writer = new StringListToDisk();
                List<string> threadedBatchFileLines = new List<string>();
                for (int i = 0; i < runLines.Count;i++ )
                {
                    AppExecuteObject core = runLines[i];
                    List<string> executableLines = new List<string>();
                    string q = "\"";
                    string line = "Call " + q + core.AppLocationPath + q + " " + q + core.DataFolder + q + " " + q + core.DataFileName + q + " " + q + core.DataFileEnding + q + " " + q + core.TargetsFile + q + " " + q + core.iQparameterFile + q + " " + q + core.ResultsLocation + q + " " + q + core.LockFile + q + " " + q + parameters.WriteFolder + @"\Results\Results" + q + " " +  i;
                    executableLines.Add(line);

                    //string lineForPostProcessing = "Call " + q + @"F:\ScottK\ToPIC\PIC_RunPostProcessing.bat" + q;//good post processing
                    //executableLines.Add(lineForPostProcessing);


                    
                    executableLines.Add("Exit");
                    //executableLines.Add("Pause");

                    Console.WriteLine("Write to " + parameters.OutputLocationPath + @"\" + "RunMe" + i + ".bat");

                    string name = parameters.OutputLocationPath + @"\" + "RunMe" + i + ".bat";


                    threadedBatchFileLines.Add("Start " + name);
                    writer.toDiskStringList(name, executableLines);
                }

                Console.WriteLine("Write to " + parameters.OutputLocationPath + @"\" + "RunMeThreads.bat");
                writer.toDiskStringList(parameters.OutputLocationPath + @"\" + "RunMeThreads.bat", threadedBatchFileLines);

                Console.WriteLine("...Done Writing Batch Files");
            }

            if(writeLocksFile)
            {
                Console.WriteLine("Writing Locks Files...");
                
                StringListToDisk writer = new StringListToDisk();
                List<string> locksToWrite = new List<string>();
                List<string> locksDoneToWrite = new List<string>();

                bool isExists = System.IO.Directory.Exists(parameters.OutputLocationPath + @"\LocksFolder");

                if (!isExists)
                    System.IO.Directory.CreateDirectory(parameters.OutputLocationPath + @"\LocksFolder");

                string LocksHeadder = parameters.OutputLocationPath + @"\LocksFolder\" + parameters.LockController;
                string LocksDoneHeadder = parameters.OutputLocationPath + @"\LocksFolder\" + parameters.LockControllerDone;//finished locks

                locksToWrite.Add(LocksHeadder);
                //locksDoneToWrite.Add(LocksDoneHeadder);no headder needed

                for (int i = 0; i < locks.Count;i++ )
                {
                    locksToWrite.Add(locks[i] + ".txt");
                    locksDoneToWrite.Add(locks[i] + "_Done.txt");
                }

                writer.toDiskStringList(LocksHeadder,locksToWrite);
                writer.toDiskStringList(LocksDoneHeadder, locksDoneToWrite);

                
                for (int i = 0; i < locks.Count; i++)
                {
                    List<string> lockFile = new List<string>();
                    lockFile.Add("running");
                    writer.toDiskStringList(parameters.OutputLocationPath + @"\LocksFolder\" + locks[i] + ".txt", lockFile);
                }
            }

            Console.WriteLine("done");
            //Console.ReadKey();
        }





        private static void AssignTargetsFileNames(List<string> dividedTargetsFile, List<AppExecuteObject> runLines, int cores)
        {
            for (int i = 0; i < cores; i++)
            {
                AppExecuteObject currentAppObject = runLines[i];
	            if (i < dividedTargetsFile.Count)
		            currentAppObject.TargetsFile = dividedTargetsFile[i];
	            else
		            Console.WriteLine("Warning: dividedTargetsFile not associated with runLines[" + i
		                              + "] since " + i + " >= dividedTargetsFile.Count (" + dividedTargetsFile.Count + ")");
            }
        }

        private static void AssignDataFileNames(List<string> dividedDataFileNames, List<string> dividedDataFileEnding, List<AppExecuteObject> runLines, int cores)
        {
            for (int i = 0; i < cores; i++)
            //for (int i = 1; i <= cores; i++)
            {
                AppExecuteObject currentAppObject = runLines[i];
                currentAppObject.DataFileName = dividedDataFileNames[i];
                currentAppObject.DataFileEnding = dividedDataFileEnding[i];
            }
        }
    }
}
