using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HPC_Connector;
using HPC_JobCreator.HPC_JobCreator;
using HPC_Submit;

namespace HPC_Delete
{
    class ProgramDelete
    {
        static void Main(string[] args)
        {
            
            string workingDirectry = @"\\picfs\projects\DMS\PIC_HPC\Hot\FO_Peptide_Merc_08_2Feb11_Sphinx_11-01-17_1";
            string workerNodeGroup = "Kronies";
            int cores = 800;



            bool ovverrideParameters = false;
            if(ovverrideParameters)
            {
                workingDirectry = @"\\picfs.pnl.gov\projects\DMS\PIC_HPC\Hot\F_CellLines0127b_Cell_00mM_1_C16_1";
                workerNodeGroup = "PrePost";
                cores = Convert.ToInt32("16");// -1;//we have a -1 on the parameter sweep the +1 is is in the engine. 
            }
            else
            {
                workingDirectry = args[0];
                workerNodeGroup = args[1];
                cores = Convert.ToInt32(args[2]);// -1;//we have a -1 on the parameter sweep the +1 is is in the engine. 

            }


            string datafileName = "HPC_DeleteFilesAndFolders";
            string workingDirectryIP = workingDirectry;
            string logDirectoryIPPath = workingDirectry;
            string paraemterFileNamePath = workingDirectry + @"\" + "WorkingParameters" + @"\" + datafileName + ".txt";
            string jobNameMustBeShort = cores + "_FrankenDelete";
            string templateName = workerNodeGroup;

            JobToHPC sendMe = SetUpFrankenDelete(datafileName, jobNameMustBeShort, cores, templateName, workingDirectry, paraemterFileNamePath);
           
            //delete results folder
            sendMe.ReleaseNodeTaskParameters = SetUpNodeReleaseClean(workingDirectry);


            HPC_Submit.WindowsHPC2012 toScheduler = new WindowsHPC2012();
            toScheduler.Send(sendMe);
            //SendDelete sender = new SendDelete();
            //sender.Send(cores, workingDirectry, workingDirectryIP, logDirectoryIPPath, datafileName, workerNodeGroup, paraemterFileNamePath);

        }

        private static ParametersTask SetUpNodeReleaseClean(string workingDirectry)
        {
            ParametersTask finalCleanup = new ParametersTask("deleteResultsFolder");
            finalCleanup.CommandLine = SetCommandLineDeleteResults(workingDirectry);
            finalCleanup.TaskTypeOption = HPCTaskType.NodeRelease;
            finalCleanup.WorkDirectory = workingDirectry;
            return finalCleanup;
        }

        private static JobToHPC SetUpFrankenDelete(string datafileName, string jobNameMustBeShort, int coresToUse, string templateName, string workDirectory, string parameterFile)
        {
            string clusterName = "Deception2.pnnl.gov";
            ParametersCluster newClusterParameters = new ParametersCluster(clusterName);
            //newClusterParameters.WorkerNodeGroup = templateName;

            ParametersJob newJobParameters = new ParametersJob(jobNameMustBeShort);
            newJobParameters.MaxNumberOfCores = coresToUse;
            newJobParameters.MinNumberOfCores = 1;
            newJobParameters.PriorityLevel = PriorityLevel.Normal;
            newJobParameters.ProjectName = "GlyQIQ";
            newJobParameters.TargetHardwareUnitType = HardwareUnitType.Core;
            newJobParameters.TemplateName = templateName;
            newJobParameters.MaxRunTimeHours = 1;

            Random rand = new Random();
            int taskNumber = rand.Next(1, 1000);
            string cmdNewLine = @"&";
            string q = "\"";
            string star = @"\*";


            string glyQIQConsoleCommandLine = SetCommandLineFrankenDelete(workDirectory, parameterFile, coresToUse);

            ParametersTask sweepGlyQIQParameters = new ParametersTask("FrankenDelete");
            sweepGlyQIQParameters.CommandLine = glyQIQConsoleCommandLine;
            sweepGlyQIQParameters.ParametricStartIndex = 1;
            sweepGlyQIQParameters.ParametricStopIndex = coresToUse;
            sweepGlyQIQParameters.ParametricIncrement = 1;

            sweepGlyQIQParameters.StdOutFilePath = workDirectory + @"\" + @"Results\test" + datafileName + "_" + "*" + @".log";
            sweepGlyQIQParameters.TaskTypeOption = HPCTaskType.ParametricSweep;
            sweepGlyQIQParameters.WorkDirectory = workDirectory;

            HPC_Connector.JobToHPC sendMe = new JobToHPC(clusterName, newJobParameters.JobName, sweepGlyQIQParameters.TaskName);
            sendMe.JobParameters = newJobParameters;
            sendMe.TaskParameters = sweepGlyQIQParameters;
            sendMe.ClusterParameters = newClusterParameters;
            return sendMe;
        }

        private static string SetCommandLineFrankenDelete(string workDirectory, string parameterFile, int cores)
        {
            string fullCommandLine = "";
            string q = "\"";

            List<string> commandLines = new List<string>();
            commandLines.Add(q + workDirectory + @"\" + "ApplicationFiles" + @"\" + @"GlyQ-IQ_HPC_DeleteEngine\Release\DeleteViaHPCEngine.exe" + q);
            commandLines.Add(q + parameterFile + q);
            commandLines.Add(cores.ToString());
            commandLines.Add(@"*");

            for (int i = 0; i < commandLines.Count - 1; i++)
            {
                fullCommandLine += commandLines[i] + " ";
            }
            fullCommandLine += commandLines[commandLines.Count - 1];

            return fullCommandLine;
        }

        private static string SetCommandLineDeleteResults(string workDirectory)
        {
            string fullCommandLine = "";
            string q = "\"";

            List<string> commandLines = new List<string>();
            commandLines.Add("del " + @"/f/s/q " + workDirectory + @"\" + "Results" + " " + @"> nul");
            commandLines.Add(@"&");
            commandLines.Add("rmdir " + @"/s/q " + workDirectory + @"\" + "Results");
            
            //del /f/s/q \\picfs\projects\DMS\PIC_HPC\Hot\FA_D60A_1 > nul
            //rmdir /s/q \\picfs\projects\DMS\PIC_HPC\Hot\FA_D60A_1
            for (int i = 0; i < commandLines.Count - 1; i++)
            {
                fullCommandLine += commandLines[i] + " ";
            }
            fullCommandLine += commandLines[commandLines.Count - 1];

            return fullCommandLine;


            //& [...]  command1 & command2
            // Use to separate multiple commands on one command line. Cmd.exe runs the first command, and then the second command.

            //&& [...]  command1 && command2
            // Use to run the command following && only if the command preceding the symbol is successful. Cmd.exe runs the first command, and then runs the second command only if the first command completed successfully. 

            //|| [...]  command1 || command2
            // Use to run the command following || only if the command preceding || fails. Cmd.exe runs the first command, and then runs the second command only if the first command did not complete successfully (receives an error code greater than zero).

            //( ) [...]  (command1 & command2)
            // Use to group or nest multiple commands.

            //; or , command1 parameter1;parameter2
            // Use to separate command parameters.

        }
    }
}
