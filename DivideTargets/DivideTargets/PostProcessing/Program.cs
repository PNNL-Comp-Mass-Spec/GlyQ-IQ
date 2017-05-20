using System;
using System.Collections.Generic;
using System.IO;
using CheckLocks;
using DivideTargetsLibraryX64.Combine;
using DivideTargetsLibraryX64.FromGetPeaks;

namespace PostProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            //step 1  check to see if all locks are present and ready to go

            bool test = false;

            if (test)
            {
                args = new string[2];
                //args[0] = @"E:\ScottK\IQ\RunFiles\LocksFolder";
                args[0] = @"R:\RAM Files\LocksFolder";
                //args[0] = @"R:\PNNL RAM 2500\LocksFolder";//folderWithLocks
                args[1] = "LockController.txt"; //controllerFile
            }

            Console.WriteLine("Args[0] is here: " + args[0]);
            Console.WriteLine("Args[1] is here: " + args[1]);
            Console.WriteLine("Args[2] is here: " + args[2]);
            Console.WriteLine("Path is here: " + args[0] + @"\" + args[1]);
            

            ParametersPostProcessing parametersPostProcesing = new ParametersPostProcessing();

            

            parametersPostProcesing.ArgsToParameters(args);

            Console.WriteLine("Locksfolder is here: " + parametersPostProcesing.LocksFolderPath);
            Console.WriteLine("LocksController is here: " + parametersPostProcesing.PathToLocksController);

            bool areLocksReady = LockCheck.AreLocksReady(parametersPostProcesing.LocksFolderPath, parametersPostProcesing.LocksFolderName, parametersPostProcesing.PathToLocksController);

            Console.WriteLine("Are We ready " + areLocksReady);

            
            

            //step 2  read all lock files and convert to parameters.  

            ParametersLocks parameters = new ParametersLocks(parametersPostProcesing.PathToLocksController);
            parameters.DetectClosedLocksFromLocksFiles(parametersPostProcesing.LocksFolderPath);

            StringLoadTextFileLine reader = new StringLoadTextFileLine();

            
            CombineParameters combineParameters = new CombineParameters();
            

            foreach (string lockName in parameters.ClosedLockFiles)
            {


                string lockPath = parametersPostProcesing.LocksFolderPath + @"\" + lockName;

                
                List<string> strings = reader.SingleFileByLine(lockPath);
                if (strings.Count == 3)
                {
                    if (combineParameters.OutputPath == "")
                    {
                        string commonResultsNameOutput = strings[1];
                        
                        combineParameters.OutputPath = commonResultsNameOutput;
                    }
                    combineParameters.InputPaths.Add(strings[2]);
                }
            }

            //check to see if all files are present
            int count = 0;
            foreach (string files in combineParameters.InputPaths)
            {
                if (File.Exists(files))
                {
                    Console.WriteLine(files + " Exists");
                    count++;
                }
                else
                {
                    Console.WriteLine(files + " Does not Exist");
                }
            }

            if (count == combineParameters.InputPaths.Count)
            {
                Console.WriteLine("All Files are Present and ready to be consolidated");

                //step 3  consolidate results based on parameters

                ResultsFiles.ConsolidateFiles(combineParameters);



                //step 4 delete files and locks and copy data back to server.  running clean up batch files

                
                string DeleteXYDataNameBatch = parametersPostProcesing.launchFolderPath + @"\" +"PUB100X_DeleteXYDataFolder.bat";
                string CopyResultBackNameBatch = parametersPostProcesing.launchFolderPath + @"\" +"PUB100X_CopyResultBack.bat";
                string PIC_DeleteFilesFileName = parametersPostProcesing.launchFolderPath + @"\" +"PIC_DeleteFiles.bat";

                Console.WriteLine("Launch " + DeleteXYDataNameBatch);
               // Console.ReadKey();
                RunCMD(parametersPostProcesing.launchFolderPath, DeleteXYDataNameBatch);

                Console.WriteLine("Launch " + CopyResultBackNameBatch);
                //Console.ReadKey();
                RunCMD(parametersPostProcesing.launchFolderPath, CopyResultBackNameBatch);
                
                //Console.ReadKey();
                RunCMD(parametersPostProcesing.launchFolderPath, PIC_DeleteFilesFileName);

                Console.WriteLine("Done");
            }
            else
            {
                Console.WriteLine("missing files. natural end");
                Console.ReadKey();//forced end to kill the cmd
            }

            

            //Console.ReadKey();
        }

        private static void RunCMD(string workingFolderPath, string batchFileToRun)
        {
//write launchBatchFile
            string batchName = "SleepLaunch.bat";
            string launchPath = workingFolderPath + @"\" + batchName;

            //make a temp batch file to run the real batch file
            List<string> launchStrings = new List<string>();
            launchStrings.Add("Call " + "\"" + batchFileToRun + "\"");
            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(launchPath, launchStrings);

            Console.WriteLine("Done writing SleepLaunch");
            //Console.ReadKey();

            string command = launchPath;


            Console.WriteLine("launchPath is " + launchPath);
            //Console.ReadKey();

            System.Diagnostics.Process proc = new System.Diagnostics.Process(); // Declare New Process
            proc.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
            proc.StartInfo.Arguments = "/c " + command;  
               
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = false;//false will print the text to the console
            proc.StartInfo.UseShellExecute = false;

            proc.StartInfo.CreateNoWindow = false; // Do not create the black window.//true
               
            proc.Start();

            //this is needed or it will not run
            proc.WaitForExit();

            Console.WriteLine("Done withProcess");
            //Console.ReadKey();

            //delete temp batch file
            if(File.Exists(launchPath))
            {
                File.Delete(launchPath);
            }
        }
    }
}
