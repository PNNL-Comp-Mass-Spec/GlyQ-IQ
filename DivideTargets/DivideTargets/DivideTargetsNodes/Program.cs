using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DivideTargetsLibrary;
using DivideTargetsLibrary.Parameters;
using SleepDLL;

namespace DivideTargetsNodes
{
    class Program
    {
        static void Main(string[] args)
        {
            //"D:\PNNL CSharp1\SVN Divide Targets\DivideTargets\DivideTargets\Parameters_DivideTargets.txt" "D:\PNNL CSharp1\SVN Divide Targets\DivideTargets\DivideTargets\Parameters_DivideTargetsNodes.txt"
            //"D:\PNNL CSharp1\SVN Divide Targets\DivideTargets\DivideTargets\Parameters_DivideTargets.txt" "D:\PNNL CSharp1\SVN Divide Targets\DivideTargets\DivideTargets\Parameters_DivideTargetsNodes.txt"
            //"\\Pub-1000\Shared_PICFS\ToPIC\WorkingParameters\Parameters_DivideTargetsPIC.txt" "\\Pub-1000\Shared_PICFS\ToPIC\WorkingParameters\Parameters_DivideTargetsPICNodes.txt"
            string divideTargetsParameterFile = args[0];
            string divideTargetsParameterFileNodes = args[1];

            //step 1  read in individual vm parameters
            string baseTargetsFile;
            string fullTargetPath;
            string textFileEnding;
            ParameterDivideTargets parametersCores = LoadParameters.SetupDivideTargetParameters(divideTargetsParameterFile, out baseTargetsFile, out fullTargetPath, out textFileEnding);
            int cores = Converter.ConvertStringToInt(parametersCores.CoresString);
            string baseLibraryName = parametersCores.TargetsFileName;

            //step 2 read in global parameters that go across many nodes
            StringListToDisk writer = new StringListToDisk();


            ParametersNodes parametersNodes = new ParametersNodes();
            parametersNodes.SetParameters(divideTargetsParameterFileNodes);


            int nodes = parametersNodes.Nodes;
            for (int i = 0; i < nodes; i++)
            {
                Console.WriteLine(parametersNodes.NodeConnections[i] + " with " + cores + " cores on node");
            }

            //step 3.  setable parameters
            bool copyParameterFiles = true;
            bool copyNodeSpecificParameterFiles = true;
            bool copyTargetsFolder = true;
            bool copySleepParameterFiles = true;
            bool writeBatchFile = true;
            bool writeSendJobFiles = true;
            bool writeConsolidationParameter = true;
            
            Console.WriteLine("We are going to copy over " + nodes + " nodes" + Environment.NewLine);
            Console.WriteLine("copyParameterFiles: " + copyParameterFiles);
            Console.WriteLine("copyTargetsFolder: " + copyTargetsFolder);
            Console.WriteLine("writeBatchFile: " + writeBatchFile);
            Console.WriteLine(Environment.NewLine);

            
            //step 4 set up targets files (divide and return names) for each node.
            List<string> dividedTargetsFile = new List<string>();

            if (copyTargetsFolder)
            {
                dividedTargetsFile = DividedTargetsFile(baseTargetsFile, parametersNodes, textFileEnding, parametersCores, nodes);
            }
            Console.WriteLine("TargetsFiles Written.  Move to next section" + Environment.NewLine);
          
            //now that we know which library goes with which computer name and which computer ip address, we can make execution files and parameterfiles from parametersCores

            //step5 create divide parameter files specific to each node
            List<string> nodeSpecificParameterNames = new List<string>();

            if (copyNodeSpecificParameterFiles)
            {
                NodeSpecificParameterFiles(parametersCores, nodeSpecificParameterNames, baseLibraryName, nodes, parametersNodes);
            }
            Console.WriteLine("nodeSpecificParameterfiles Written.  Move to next section" + Environment.NewLine);

            //step 6 write sleep file
            List<string> sleepParameterFilePile = new List<string>();

            if (copySleepParameterFiles)
            {
                SleepParameterFilesForEachNode(parametersCores, writer, sleepParameterFilePile, parametersNodes, nodes);
            }
            Console.WriteLine("sleepParameter Written.  Move to next section" + Environment.NewLine);

            //step 7 write SendJobFiles

            List<string> sendJobNamePile = new List<string>();
            List<string> resetNodesNamePile = new List<string>();
            if (writeSendJobFiles)
            {
                WriteSendJobBatchFiles(sendJobNamePile, resetNodesNamePile, sleepParameterFilePile, baseLibraryName, nodeSpecificParameterNames, dividedTargetsFile, parametersNodes, nodes, parametersCores);
            }
            Console.WriteLine("SendJob Written.  Move to next section" + Environment.NewLine);

            //write All Run Batch



            //step 8 write consolidation parameter files and batch file to run and combine the nodes
            if (writeConsolidationParameter)
            {
                //Output,D:\Csharp\ConosleApps\LocalServer\IQ\GlyQ-IQ\Data Out\2013-7-6\PUB-2002_Results\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_iqResults.txt
                //Input,D:\Csharp\ConosleApps\LocalServer\IQ\GlyQ-IQ\Data Out\2013-7-6\PUB-2002_Results\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_0_iqResults.txt   
                string consolidationParameterForAllNodes = parametersNodes.ResultsLocation + @"\" + "ConsolidationParameterForAllNodes.txt";

                string output = "Output," + parametersNodes.LaunchHomeLocation + @"\" + "ResultsSummary" + @"\" + parametersCores.DataFileFileName + "_Global_iqResults.txt";

                List<string> input = new List<string>();
                List<string> dataToConsolidate = new List<string>();

                for (int i = 0; i < nodes; i++)
                {
                    bool isHPC = false;
                    string currentNodeFile = "";
                    if (isHPC)
                    {
                        currentNodeFile = @"E:\ScottK\Shared_PICFS\ToPIC\Results" + @"\" + parametersNodes.NodeConnections[i].Item1 + "_Results" + @"\" + parametersCores.DataFileFileName + "_iqResults.txt";
                    }
                    else
                    {

                        //currentNodeFile = @"E:\ScottK\Shared_PICFS\ToPIC\Results" + @"\" + parametersNodes.NodeConnections[i].Item1 + "_Results" + @"\" + "Results_" + parametersCores.DataFileFileName + "_1" + @"\" + parametersCores.DataFileFileName + "_1_iqResults.txt";
                        currentNodeFile = @"E:\ScottK\Shared_PICFS\ToPIC\Results" + @"\" + parametersNodes.NodeConnections[i].Item1 + "_Results" + @"\" + "Results_" + parametersCores.DataFileFileName + @"\" + parametersCores.DataFileFileName + "_iqResults.txt";
                    }

                    input.Add("Input," + currentNodeFile);
                }

               
                dataToConsolidate.Add(output);


                dataToConsolidate.AddRange(input);

                writer.toDiskStringList(consolidationParameterForAllNodes, dataToConsolidate);
            }

            Console.WriteLine("Done With Process Divide Node");
        }







        private static void WriteSendJobBatchFiles(List<string> sendJobNamePile, List<string> resetNodesNamePile, List<string> sleepParameterFilePile, string baseLibraryName, List<string> nodeSpecificParameterNames, List<string> dividedTargetsFile, ParametersNodes parametersNodes, int nodes, ParameterDivideTargets parametersCores)
        {
            for (int i = 0; i < nodes; i++)
            {
                //PIC_SendJob.bat
                string nodeName = parametersNodes.NodeConnections[i].Item1;
                string ipAddressForExecution = parametersNodes.NodeConnections[i].Item2;
                string Pub1000HomeFolder = parametersNodes.LaunchHomeLocation;
                string adminAccount = parametersNodes.AdminAccount;
                string adminPassword = parametersNodes.AdminPassword;
                string LaunchFolder = parametersNodes.LaunchHomeLocation;
                string PexecLocation = parametersNodes.PexecLocation;
                string sleepExeLocation = parametersNodes.SleepExeLocation;
                string runMeSeccond100XFileName = parametersNodes.RunMeSeccond100XFileName;
                string nodeShareFolderLocationOnNode = parametersNodes.NodeShareFolderLocationOnNode;
                string nodeShareFolder = parametersNodes.NodeShareFolder;


                string PIC_SendJobFileName = "PIC_SendJob_" + nodeName + ".bat";
                sendJobNamePile.Add(PIC_SendJobFileName);

                string PIC_ResetNodesName = "PIC_ResetServers_" + nodeName + ".bat";
                resetNodesNamePile.Add(PIC_ResetNodesName);

                List<string> SendJobFileNameData = new List<string>();
                List<string> ResetNodesData = new List<string>();
                //node name needs to be hard coded so it know where to go

                //we need to copy node specific files to the node share and de-code the file name so it is general.  for the divide targets parameter file and the targets file
                //echo f | xcopy /Y "\\pub-1000\Shared_PICFS\ToPIC\WorkingParameters\Parameters_DivideTargetsPIC_Pub-2002.txt" "F:\ScottK\ToPic\WorkingParameters\Parameters_DivideTargetsPIC.txt" /S
                string writeFolder = @"E:\ScottK\Shared_PICFS\ToPIC";
                //SendJobFileNameData.Add("echo f | xcopy /Y " + "\"" + parametersNodes.ResultsLocation + @"\" + nodeSpecificParameterNames[i] + "\"" + " " + "\"" + @"\\" + nodeName + @"\" + nodeShareFolder + @"\" + "Parameters_DivideTargetsPIC.txt" + "\"" + " " + "\"" + writeFolder +@" /S");

                //timer
                SendJobFileNameData.Add("\"" + parametersNodes.LaunchHomeLocation + @"\" + @"GlyQ-IQ Timer\Release\WriteTime.exe" + "\"" + " " + "\"" + parametersNodes.LaunchHomeLocation + @"\" + nodeName + "_Time.txt" + "\"");
                

                SendJobFileNameData.Add("echo f | xcopy /Y " + "\"" + parametersNodes.ResultsLocation + @"\" + nodeSpecificParameterNames[i] + "\"" + " " + "\"" + @"\\" + nodeName + @"\" + nodeShareFolder + @"\" + "Parameters_DivideTargetsPIC.txt" + "\"" + @" /S");

                SendJobFileNameData.Add("echo f | xcopy /Y " + "\"" + parametersNodes.ResultsLocation + @"\" + "ApplicationSetupParametersF.txt" + "\"" + " " + "\"" + @"\\" + nodeName + @"\" + nodeShareFolder + @"\" + "ApplicationSetupParameters.txt" + "\"" + @" /S");
                
                SendJobFileNameData.Add("echo f | xcopy /Y " + "\"" + parametersNodes.ResultsLocation + @"\" + dividedTargetsFile[i] + "\"" + " " + "\"" + @"\\" + nodeName + @"\" + nodeShareFolder + @"\" + baseLibraryName + "\"" + @" /S");


                SendJobFileNameData.Add("echo f | xcopy /Y " + "\"" + Pub1000HomeFolder + @"\" + runMeSeccond100XFileName + "\"" + " " + "\"" + @"\\" + nodeName + @"\" + nodeShareFolder + @"\" + runMeSeccond100XFileName + "\"" + @" /S");
                
                //SendJobFileNameData.Add(PexecLocation + @" \\" + ipAddressForExecution + " -u " + ipAddressForExecution + @"\" + adminAccount + " -p " + adminPassword + " " + "\"" + nodeShareFolderLocationOnNode + @"\" + nodeShareFolder + @"\" + runMeSeccond100XFileName + "\"");

                SendJobFileNameData.Add(@"MD " + "\"" + @"\\" + nodeName + @"\NodeShareFolder\ScottK\New Folder" + "\"");
                //sleep and wait for the job to finish on the slave computers
                //SendJobFileNameData.Add("\"" + sleepExeLocation + "\"" + " " + "\"" +  sleepParameterFilePile[i] + "\"");//we call this elseware
                SendJobFileNameData.Add("Exit");
                //SendJobFileNameData.Add("Pause");
                WriteBatchFile(SendJobFileNameData, PIC_SendJobFileName, LaunchFolder);



                //write reset nodes
                ResetNodesData.Add(PexecLocation + @" \\" + ipAddressForExecution + " -u " + ipAddressForExecution + @"\" + adminAccount + " -p " + adminPassword + " " + "\"" + nodeShareFolderLocationOnNode + @"\" + nodeShareFolder + @"\" + @"ScottK\resetSlaveWhenDone.bat" + "\"");
                ResetNodesData.Add("Exit");
                WriteBatchFile(ResetNodesData, PIC_ResetNodesName, LaunchFolder);
            }


            //Start PIC_SendJob_Pub-2002.bat

            //Call "E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ MultiSleep\Release\MultiSleep.exe" "E:\ScottK\Shared_PICFS\ToPIC\WorkingParameters\PIC_MultiSleepParameterFileGlobal.txt"


            string PIC_LaunchJobsName = "PIC_LaunchJobs.bat";
            string PIC_LaunchResetServersName = "PIC_LaunchResetServers.bat";
            string LaunchFolder2 = parametersNodes.LaunchHomeLocation;

            List<string> LaunchJobsData = new List<string>();
            List<string> LaunchResetServersData = new List<string>();
            //node name needs to be hard coded so it know where to go

            //we need to copy node specific files to the node share and de-code the file name so it is general.  for the divide targets parameter file and the targets file
            //echo f | xcopy /Y "\\pub-1000\Shared_PICFS\ToPIC\WorkingParameters\Parameters_DivideTargetsPIC_Pub-2002.txt" "F:\ScottK\ToPic\WorkingParameters\Parameters_DivideTargetsPIC.txt" /S

            //**********************This is where we set the library as a parameter file to IQGlyQ_Console_ParameterSetup.exe

            //"L_10_IQ_TargetsFirstAll_R.txt" "FragmentedTargetedWorkflowParameters_Velos_DH.txt" "Parameters_DivideTargetsPIC.txt" "GlyQIQ_Diabetes_Parameters_PIC.txt" "Factors_L10.txt"


            LaunchJobsData.Add("\"" + @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ Application Setup\Release\IQGlyQ_Console_ParameterSetup.exe" + "\"" + " " + "\"" + @"E:\ScottK\Shared_PICFS\ToPIC\WorkingParameters\" + "ApplicationSetupParameters.txt" + "\"");
            //LaunchJobsData.Add("Pause");
            LaunchJobsData.Add("Call  " + @"E:\ScottK\Shared_PICFS\ToPIC\PIC_NodeSetup.bat" );
            //LaunchJobsData.Add("Pause");
            for (int i = 0; i < nodes; i++)
            {
                LaunchJobsData.Add("Start " + sendJobNamePile[i]);
                LaunchResetServersData.Add("Start " + resetNodesNamePile[i]);
            }

            LaunchJobsData.Add("Call  " + "\"" + @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ MultiSleep\Release\MultiSleep.exe" + "\"" + " " + "\"" + @"E:\ScottK\Shared_PICFS\ToPIC\WorkingParameters\PIC_MultiSleepParameterFileGlobal.txt" + "\"" + " " + 1);

            LaunchJobsData.Add("Pause");
            
            WriteBatchFile(LaunchJobsData, PIC_LaunchJobsName, LaunchFolder2);
            WriteBatchFile(LaunchResetServersData, PIC_LaunchResetServersName, LaunchFolder2);

            int counter4 = 0;
            foreach (var parameterFile in sendJobNamePile)
            {
                if (File.Exists(parametersNodes.LaunchHomeLocation + @"\" + parameterFile))
                {
                    counter4++;
                }
            }

            if (counter4 == nodes)
            {
                Console.WriteLine("All SendJob files have been written");
            }
        }

        private static void SleepParameterFilesForEachNode(ParameterDivideTargets parametersCores, StringListToDisk writer, List<string> sleepParameterFilePile, ParametersNodes parametersNodes, int nodes)
        {
            for (int i = 0; i < nodes; i++)
            {
                //E:\ScottK\Shared_PICFS\ToPIC\Results\PUB-2002_Results\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_iqResults.txt

                string Pub1000HomeFolder = parametersNodes.LaunchHomeLocation;
                string nodeName = parametersNodes.NodeConnections[i].Item1;
                string sleepFileName = "PIC_SleepParameterFile_" + nodeName + ".txt";
                string sleepFilePath = parametersNodes.ResultsLocation + @"\" + sleepFileName;
                string fileTOwaitFor = ThisIsWhatItTakesToFindFinalResultsFile(parametersCores);
                    //sinpler in divide targets
                string lineFileToWaitFor = "FileToWaitFor," + Pub1000HomeFolder + @"\" + "Results" + @"\" + nodeName + "_Results" + @"\" + fileTOwaitFor;
                string lineBatchFileToRunAfterLoop = "BatchFileToRunAfterLoop," + Pub1000HomeFolder + @"\" + "testBatch.bat"; //we need line 1 from line 0 in the controller
                string lineWorkingFolder = "WorkingFolder," + Pub1000HomeFolder + @"\" + "WorkingParameters";

                string lineSeconds = "Seconds," + 20;

                List<string> linesForSleepParameteFile = new List<string>();
                linesForSleepParameteFile.Add(lineFileToWaitFor);
                linesForSleepParameteFile.Add(lineBatchFileToRunAfterLoop);
                linesForSleepParameteFile.Add(lineWorkingFolder);
                linesForSleepParameteFile.Add(lineSeconds);

                sleepParameterFilePile.Add(sleepFilePath);


                writer.toDiskStringList(sleepFilePath, linesForSleepParameteFile);
            }

            int counter3 = 0;
            foreach (var parameterFile in sleepParameterFilePile)
            {
                if (File.Exists(parameterFile))
                {
                    counter3++;
                }
            }

            if (counter3 == nodes)
            {
                Console.WriteLine("All sleepParameter files have been written");
            }
        }

        private static void NodeSpecificParameterFiles(ParameterDivideTargets parametersCores, List<string> nodeSpecificParameterNames, string baseLibraryName, int nodes, ParametersNodes parametersNodes)
        {
            for (int i = 0; i < nodes; i++)
            {
                string nodeName = parametersNodes.NodeConnections[i].Item1;
                string nodeFileName = "Parameters_DivideTargetsPIC_" + nodeName + ".txt";
                string nodeFileNamePath = parametersNodes.ResultsLocation + @"\" + nodeFileName;
                nodeSpecificParameterNames.Add(nodeFileName);

                //parametersCores.TargetsFilePath = parametersNodes.NodeLibraries[i];
                parametersCores.TargetsFileName = baseLibraryName;
                    //we can get away with using the base name here because when we copy the library to the node, it is renamed to the base
                parametersCores.TargetsFileFolder = parametersCores.WriteFolder + @"\" + "WorkingParameters";

                //parametersCores.WriteParameters(parametersCores, nodeFileNamePath);
                parametersCores.WriteParameters(nodeFileNamePath);
            }

            int counter2 = 0;
            foreach (var parameterFile in nodeSpecificParameterNames)
            {
                if (File.Exists(parametersNodes.ResultsLocation + @"\" + parameterFile))
                {
                    counter2++;
                }
            }

            if (counter2 == nodes)
            {
                Console.WriteLine("All nodeSpecificParameterName files have been written");
            }
        }

        private static List<string> DividedTargetsFile(string baseTargetsFile, ParametersNodes parametersNodes, string textFileEnding, ParameterDivideTargets parametersCores, int nodes)
        {
            List<string> dividedTargetsFile = new List<string>();
            string completeTargetsFileName = parametersCores.TargetsFileFolder + @"\" + parametersCores.TargetsFileName;
            if (File.Exists(completeTargetsFileName))
            {
                DivideTargetsFile.TargetsFolderSetup(completeTargetsFileName, parametersNodes.ResultsLocation, nodes, baseTargetsFile, textFileEnding, out dividedTargetsFile);

                if (parametersNodes.NodeConnections.Count == dividedTargetsFile.Count)
                {
                    for (int i = 0; i < nodes; i++)
                    {
                        parametersNodes.NodeLibraries[i] = dividedTargetsFile[i];

                        Console.WriteLine(parametersNodes.NodeConnections[i] + " with " + dividedTargetsFile[i] + " library");
                    }
                }

                int counter = 0;
                foreach (var targets in dividedTargetsFile)
                {
                    if (File.Exists(parametersNodes.ResultsLocation + @"\" + targets))
                    {
                        counter++;
                    }
                }

                if (counter == nodes)
                {
                    Console.WriteLine("All targets files have been divided and written");
                }
            }
            return dividedTargetsFile;
        }

        private static string ThisIsWhatItTakesToFindFinalResultsFile(ParameterDivideTargets parametersCores)
        {

            string fileTOwaitFor = parametersCores.DataFileFileName + "_iqResults.txt";



            return fileTOwaitFor;
        }

        private static void WriteBatchFile(List<string> stringList, string fileName, string folder)
        {
            StringListToDisk writer = new StringListToDisk();
            string outputLocation;
            outputLocation = folder + @"\" + fileName;
            writer.toDiskStringList(outputLocation, stringList);
        }
    }
}
