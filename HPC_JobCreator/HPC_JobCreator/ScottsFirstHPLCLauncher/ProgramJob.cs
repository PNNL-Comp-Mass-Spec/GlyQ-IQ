using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HPC_Connector;
using HPC_Submit;
using HPC_JobCreator;

namespace ScottsFirstHPLCLauncher
{
	internal static class ProgramJob
	{

		/// <summary>
		/// Runs Command to consolidate results from HPC
		/// </summary>
		private static bool LaunchPreRunCleanUp { get; set; }

		/// <summary>
		/// Runs Command to consolidate results from HPC
		/// </summary>
		private static bool LaunchPostRun { get; set; }

		private static void Main(string[] args)
		{

			try
			{



				//SEND A Job to the HPC

				//string datafileName = "Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_1";
				//string library = "L_10_IQ_TargetsFirstAll_R";
				//int cores = 800;
				//string workDirectory = @"\\picfs\projects\DMS\PIC_HPC\Home";


				//string datafileName = "Gly09_SN129_21Feb13_Cheetah_C14_230nL_SPIN_1700V_300mlmin_22Torr_100C_60kHDR2M_1";
				//string library = "L_10_IQ_TargetsFirstAll_R";
				//int cores = 49;
				//string workDirectory = @"\\pichvnas01\data\MassSpecOmic\Home";
				//string parameterFile = "GlyQIQ_Diabetes_Parameters_PICFS_SPIN_SN129.txt";
				//string workDirectoryIP = @"\\172.17.130.199\data\MassSpecOmic\Home";

				//Gly09_SN129_21Feb13_Cheetah_C14_230nL_SPIN_1700V_300mlmin_22Torr_100C_60kHDR2M_1 L_10_IQ_TargetsFirstAll_R  49 \\pichvnas01\data\MassSpecOmic\Home GlyQIQ_Diabetes_Parameters_PICFS_SPIN_SN129.txt "\\172.17.130.199\data\MassSpecOmic\Home"

				//Gly09_SN129_21Feb13_Cheetah_C14_230nL_SPIN_1700V_300mlmin_22Torr_100C_60kHDR2M_1 L_10_IQ_TargetsFirstAll_R  49 \\pichvnas01\data\MassSpecOmic\Home GlyQIQ_Diabetes_Parameters_PICFS_SPIN_SN129.txt "\\172.17.130.199\data\MassSpecOmic\Home" "\\172.17.130.199\data\MassSpecOmic\Home" kronies

				//Gly08_Velos4_Jaguar_200nL_D60A_1X_C1_2Sept12_1 L_10_HighMannose_TargetsFirstAll 9 "\\picfs\projects\DMS\PIC_HPC\FieldOffice_Velos_Azure" GlyQIQ_Diabetes_Parameters_PICFS_Velos_SN129_L10PSA.txt "%CCP_PACKAGE_ROOT%\FieldOffice_Velos_Azure" "%CCP_PACKAGE_ROOT%\FieldOffice_Velos_Azure" "\\picfs\projects\DMS\PIC_HPC\FieldOffice_Velos_Azure\ApplicationFiles" AzureNodes

				if (args.Length < 11)
				{
					Console.WriteLine("The HPC Job Creator requires 11 or 13 arguments, each separated by a space: ");

					Console.WriteLine(" DatafileName Library Cores WorkDirectory");
					Console.WriteLine(" ParameterFile WorkDirectoryIPPath");
					Console.WriteLine(" LogDirectoryIPPath ExeHomeLocationDirectory");
					Console.WriteLine(" ClusterName TemplateName MaxTargetNumber");
					Console.WriteLine();

					Console.WriteLine("Optionally include these additional parameters:");
					Console.WriteLine(" HPCUsername HPCPassword");

					System.Threading.Thread.Sleep(2000);
					return;
				}

				string datafileName = args[0];
				string library = args[1];
				int cores = Convert.ToInt32(args[2]);
				string workDirectory = args[3];//@"\\picfs\projects\DMS\PIC_HPC\Home"
				string parameterFile = args[4];//@"GlyQIQ_Diabetes_Parameters_PICFS.txt"
				string workDirectoryIPPath = args[5];//@"\\iphere\projects\DMS\PIC_HPC\Home"
				string logDirectoryIPPath = args[6];//@"\\iphere\projects\DMS\PIC_HPC\Home"
				string exeHomeLocationDirectory = args[7];//@"\\172.16.112.12\projects\DMS\ScottK\ScottK_PUB-100X_Launch_Folder\ToPIC";
				//string workerNodeGroup = args[8];//kronies
				string clusterName = args[8];//"Deception2.pnnl.gov"
				string templateName = args[9];//GlyQIQ
				int maxTargetNumber = Convert.ToInt32(args[10]);

				string hpcUsername = "";
				string hpcPassword = "";

				if (args.Length >= 13)
				{
					hpcUsername = args[11];
					hpcPassword = args[12];
				}


				Console.WriteLine("datafileName " + datafileName);
				Console.WriteLine("library " + library);
				Console.WriteLine("cores " + cores);
				Console.WriteLine("parameterFile " + parameterFile);
				Console.WriteLine("workDirectory " + workDirectory);
				Console.WriteLine("workDirectoryIPPath " + workDirectoryIPPath);
				Console.WriteLine("logDirectoryIPPath " + logDirectoryIPPath);
				Console.WriteLine("exeHomeLocationDirectory " + exeHomeLocationDirectory);
				//Console.WriteLine("workerNodeGroup " + workerNodeGroup);
				Console.WriteLine("ClusterName " + clusterName);
				Console.WriteLine("Template is " + templateName);
				Console.WriteLine("The Target List is broken in to " + maxTargetNumber + " pieces");
				//Console.WriteLine("rootName " + rootName);

				Console.WriteLine("We will run on the HPC template: " + templateName);

				LaunchPostRun = true;
				LaunchPreRunCleanUp = true;

				//bool isKronies = workerNodeGroup=="Kronies";


				const string ccpPackageRoot = @"%CCP_PACKAGE_ROOT%";
				//if (isKronies)
				//{
				//    //workDirectory = @"\\picfs\projects\DMS\PIC_HPC\Home";
				//    //workDirectoryIPPath = @"\\picfs\projects\DMS\PIC_HPC\Home";
				//    //logDirectoryIPPath = @"\\picfs\projects\DMS\PIC_HPC\Home";

				//    //workDirectory = @"\\picfs\projects\DMS\PIC_HPC\FieldOffice";
				//    //workDirectoryIPPath = @"\\picfs\projects\DMS\PIC_HPC\FieldOffice";
				//    //logDirectoryIPPath = @"\\picfs\projects\DMS\PIC_HPC\FieldOffice";

				//    //workDirectory = workDirectory;
				//    workDirectoryIPPath = workDirectory;
				//    logDirectoryIPPath = workDirectory;
				//}

				const bool isOverride = false; //true will work on azure
				if (isOverride)
				{
					datafileName = "D60A_1";
					library = "L_10_HighMannose_TargetsFirstAll";
					cores = 9;
					workDirectory = @"\\picfs\projects\DMS\PIC_HPC\Hot\FA_D60A_1";
					parameterFile = "GlyQIQ_Params_Velos_SN129_L10PSA.txt";
					workDirectoryIPPath = ccpPackageRoot + @"\" + "FA_D60A_1";
					logDirectoryIPPath = workDirectoryIPPath;
					exeHomeLocationDirectory = @"\\picfs\projects\DMS\PIC_HPC\Hot\FA_D60A_1\ApplicationFiles";
					//workerNodeGroup = "AzureNodes";

				}

				const bool isOverride2 = false; //true will work on azure
				if (isOverride2)
				{
					string fileSystem = @"\\picfs.pnl.gov";
					datafileName = "SPIN_SN138_16Dec13_C15_1";
					library = "L_10_PSA21_TargetsFirstAll_R";
					cores = 20;
					workDirectory = fileSystem + @"\" + @"projects\DMS\PIC_HPC\Hot\F_TestingLong0020D2_SPIN_SN138_16Dec13_C15_1";
					parameterFile = "Default.txt";
					workDirectoryIPPath = workDirectory;
					logDirectoryIPPath = workDirectory;
					exeHomeLocationDirectory = fileSystem + @"\" + @"projects\DMS\PIC_HPC\Hot\F_TestingLong0020D2_SPIN_SN138_16Dec13_C15_1\ApplicationFiles";
					//workerNodeGroup = "ComputeNodes";

				}

				if (LaunchPreRunCleanUp)
				{
					PreJob(workDirectory);
				}

				const bool isNewMethod = true;

				int jobID = 0;

				if (isNewMethod)
				{
					int lastTarget = maxTargetNumber;//3257;
					int maxcCoresToUse = cores;//64

					//string clusterName = "Deception.pnl.gov";//deception 1
					//string clusterName = "Deception2.pnnl.gov";//deception 2
					//string templateName = "GlyQIQ";

					JobToHPC sendMe = SetUpNewJob(datafileName, maxcCoresToUse, lastTarget, workDirectory, exeHomeLocationDirectory, library, parameterFile, clusterName, templateName);

					HPC_Submit.WindowsHPC2012 toScheduler;

					if (string.IsNullOrEmpty(hpcUsername))
						toScheduler = new WindowsHPC2012();
					else
						toScheduler = new WindowsHPC2012(hpcUsername, DecodePassword(hpcPassword));

					jobID = toScheduler.Send(sendMe);

					Console.WriteLine("HPCJobID = " + jobID);
				}
				else
				{
					string workerNodeGroup = "ComputeNodes";
					SendJob sender = new SendJob();
					sender.Send(cores, datafileName, library, workDirectory, parameterFile, workDirectoryIPPath, logDirectoryIPPath, exeHomeLocationDirectory, workerNodeGroup);
				}

				if (LaunchPostRun)
				{
					string uniquieStartCollectResultsName = "1_HPC_StartCollectResults_" + datafileName + ".bat";

					Console.WriteLine("We will run " + workDirectory + @"\" + uniquieStartCollectResultsName);

					Utilities.RunCMD(workDirectory, uniquieStartCollectResultsName);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in HPC submission:  " + ex.Message);
				Console.WriteLine("Error in HPC submission:  " + ex.StackTrace);
			}
		}

		/// <summary>
		/// Decrypts password received from ini file
		/// </summary>
		/// <param name="enPwd">Encoded password</param>
		/// <returns>Clear text password</returns>
		private static string DecodePassword(string enPwd)
		{
			// Decrypts password received from ini file
			// Password was created by alternately subtracting or adding 1 to the ASCII value of each character

			// Convert the password string to a character array
			char[] pwdChars = enPwd.ToCharArray();
			dynamic pwdBytes = new List<byte>();
			dynamic pwdCharsAdj = new List<char>();

			for (int i = 0; i <= pwdChars.Length - 1; i++)
			{
				pwdBytes.Add(Convert.ToByte(pwdChars[i]));
			}

			// Modify the byte array by shifting alternating bytes up or down and convert back to char, and add to output string

			for (int byteCntr = 0; byteCntr <= pwdBytes.Count - 1; byteCntr++)
			{
				if ((byteCntr % 2) == 0)
				{
					pwdBytes[byteCntr] += Convert.ToByte(1);
				}
				else
				{
					pwdBytes[byteCntr] -= Convert.ToByte(1);
				}
				pwdCharsAdj.Add(Convert.ToChar(pwdBytes[byteCntr]));
			}

			return string.Join("", pwdCharsAdj);

		}

		private static JobToHPC SetUpNewJob(
			string datafileName,
			int coresToUse,
			int lastTarget,
			string workDirectory,
			string exeHomeLocationDirectory,
			string targetsFile,
			string parameterFile,
			string clusterName,
			string templateName)
		{
			var newClusterParameters = new ParametersCluster(clusterName);
			//newClusterParameters.WorkerNodeGroup = workerNodeGroup;

			string jobNameMustBeShort = coresToUse + " Cores and x nodes " + datafileName;

			List<char> letters = jobNameMustBeShort.ToList();
			if (jobNameMustBeShort.Length > 80)
			{
				jobNameMustBeShort = "";
				for (int i = 0; i < 80; i++)
				{
					jobNameMustBeShort += letters[i];
				}
			}

			var newJobParameters = new ParametersJob(jobNameMustBeShort)
			{
				MaxNumberOfCores = coresToUse,
				MinNumberOfCores = 1,
				PriorityLevel = PriorityLevel.Normal,
				ProjectName = "GlyQIQ",
				TargetHardwareUnitType = HardwareUnitType.Core,
				TemplateName = templateName,
				MaxRunTimeHours = 24 * 7,
				isExclusive = false
			};

			string glyQIQConsoleCommandLine = SetCommandLine(workDirectory, exeHomeLocationDirectory, datafileName, targetsFile, parameterFile, isKronies: false, isAzure: false);

			var sweepGlyQIQParameters = new ParametersTask("GlyQIQSweep")
			{
				CommandLine = glyQIQConsoleCommandLine,
				ParametricStartIndex = 1,
				ParametricStopIndex = lastTarget,
				ParametricIncrement = 1,
				StdOutFilePath = workDirectory + @"\" + @"Results\test" + datafileName + "_" + "*" + @".log",
				TaskTypeOption = HPCTaskType.ParametricSweep,
				WorkDirectory = workDirectory
			};

			var sendMe = new JobToHPC(clusterName, newJobParameters.JobName, sweepGlyQIQParameters.TaskName)
			{
				JobParameters = newJobParameters,
				TaskParameters = sweepGlyQIQParameters,
				ClusterParameters = newClusterParameters
			};
			return sendMe;
		}

		private static string SetCommandLine(
			string workDirectory,
			string exeHomeLocationDirectory,
			string datafileName,
			string targetsFile,
			string parameterFile,
			bool isKronies,
			bool isAzure)
		{
			const string q = "\"";

			var commandLines = new List<string>();

			if (string.IsNullOrEmpty(exeHomeLocationDirectory))
				exeHomeLocationDirectory = Path.Combine(workDirectory, "ApplicationFiles");

			if (isKronies)//(2/3)
			{
				commandLines.Add(q + Path.Combine(exeHomeLocationDirectory, @"GlyQ-IQ_Application\Release\IQGlyQ_Console.exe") + q);
			}
			else
			{
				if (isAzure)
				{
					commandLines.Add(Path.Combine(exeHomeLocationDirectory, @"GlyQ-IQ_Application\Release\IQGlyQ_Console.exe"));

				}
				else
				{
					commandLines.Add(q + Path.Combine(exeHomeLocationDirectory, @"GlyQ-IQ_Application\Release\IQGlyQ_Console.exe") + q);
				}
			}

			commandLines.Add(Path.Combine(workDirectory, "RawData"));
			commandLines.Add(q + datafileName + q);
			commandLines.Add(q + @"raw" + q);
			commandLines.Add(q + targetsFile + @"_*.txt" + q);
			commandLines.Add(q + parameterFile + q);
			commandLines.Add(Path.Combine(workDirectory, "WorkingParameters"));
			commandLines.Add(@"Lock_*");
			commandLines.Add(Path.Combine(workDirectory, @"Results\Results"));
			commandLines.Add(@"*");

			//this is the popd ending
			//commandLines.Add(cmdNewLine + " " + "echo HPC run completed...");
			//commandLines.Add(cmdNewLine + " " + "popd");
			//commandLines.Add(cmdNewLine + " " + "echo popd completed...");

			var fullCommandLine = new StringBuilder();
			for (int i = 0; i < commandLines.Count; i++)
			{
				fullCommandLine.Append(commandLines[i]);
				if (i < commandLines.Count - 1)
					fullCommandLine.Append(" ");

			}

			return fullCommandLine.ToString();
		}

		private static void PreJob(string workDirectory)
		{

			Console.WriteLine("looking for (" + workDirectory + @"\" + "Results" + ") so we can erase it...");

			if (Directory.Exists(workDirectory + @"\" + "Results"))
			{
				//Console.WriteLine("Do you want to delete the Results Directory on " + workDirectory +"?");
				//Console.WriteLine("Press y for delete");
				//ConsoleKeyInfo input = Console.ReadKey();
				//if (input.Key == ConsoleKey.Y)

				const bool test = true;
				if (test)
				{
					Console.WriteLine(Environment.NewLine + "this may take a while...");
					Console.WriteLine("We want to delete " + workDirectory + @"\" + "Results");
					//Directory.Delete(@"\\picfs\projects\DMS\PIC_HPC\Home\Results", true);
					var directory = new DirectoryInfo(workDirectory + @"\" + "Results");


					Empty(directory);
					Directory.CreateDirectory(workDirectory + @"\" + "Results");

					//Console.WriteLine(Environment.NewLine + "this may take a while for the log directory...");
					//Console.WriteLine("We want to delete " + logDirectory + @"\" + "Results");
					//DirectoryInfo directoryLog = new System.IO.DirectoryInfo(logDirectory + @"\" + "Results");

					//Empty(directoryLog);
					//Directory.CreateDirectory(logDirectory + @"\" + "Results");

					Console.WriteLine("Results Deleted");
				}
			}


			//if (Directory.Exists(workDirectory + @"\" + "Results"))
			//{
			//    Console.WriteLine("Do you want to delete the Results Directory on pichvnas01?");
			//    Console.WriteLine("Press y for delete");
			//    ConsoleKeyInfo input = Console.ReadKey();
			//    if (input.Key == ConsoleKey.Y)
			//    {
			//        Console.WriteLine(Environment.NewLine + "this may take a while...");
			//        //Directory.Delete(@"\\picfs\projects\DMS\PIC_HPC\Home\Results", true);
			//        DirectoryInfo directory = new System.IO.DirectoryInfo(workDirectory + @"\" + "Results");

			//        Empty(directory);
			//        Directory.CreateDirectory(workDirectory + @"\" + "Results");
			//    }
			//}
		}

		public static void Empty(DirectoryInfo directory)
		{
			foreach (FileInfo file in directory.GetFiles()) file.Delete();
			foreach (DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
		}

	}
}