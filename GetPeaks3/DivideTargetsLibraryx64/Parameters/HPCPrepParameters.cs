using System;
using System.Collections.Generic;
using System.Globalization;
using DivideTargetsLibraryX64.FromGetPeaks;

namespace DivideTargetsLibraryX64.Parameters
{
    public class HPCPrepParameters
    {
        public int cores { get; set; }
        public string WorkingDirectory { get; set; }
        public string DataSetDirectory { get; set; }
        public string DatasetFileNameNoEnding { get; set; }
        public string DatasetFileExtenstion { get; set; }
        public string WorkingDirectoryForExe { get; set; }
        public string TargetsNoEnding { get; set; }
        public string FactorsName { get; set; }
        public string ExecutorParameterFile { get; set; }
        public string IQParameterFile { get; set; }
        public string HPC_MultiSleepParameterFileGlobal { get; set; }
        public string ipaddress { get; set; }
        public string LogIpaddress { get; set; }
        public string ExeLaunchDirectory { get; set; }
        public string HPCNodeGroupName { get; set; }

		public string PICHPCUsername { get; set; }
		public string PICHPCPassword { get; set; }

        /// <summary>
        /// WorkingDirectory + WorkingFolder = WorkingDirectory
        /// </summary>
        public string WorkingFolder { get; set; }

        /// <summary>
        /// true wil launch HPC, false will run locally
        /// </summary>
        public string IsHPC { get; set; }

        public string FrankenDelete { get; set; }

        /// <summary>
        /// "Deception2.pnnl.gov"
        /// </summary>
        public string ClusterName { get; set; }

        /// <summary>
        /// "GlyQIQ"
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// LKibrary size for divide targets and HPC run = piece count
        /// </summary>
        public int MaxTargetNumber { get; set; }


        //parameter files to make
        public string makeResultsListName = "HPC_MakeResultsList_Asterisks.bat";
        public string divideTargetsParameterFile = "HPC-Parameters_DivideTargetsPIC_Asterisks.txt";
        public string consoleOperatingParameters = @"WorkingParameters\GlyQIQ_Diabetes_Parameters_PICFS.txt";
        public string scottsFirstHPLCLauncher = "HPC_ScottsFirstHPLCLauncher.bat";
        public string HPC_DivideTargetsLaunch = "2_HPC_DivideTargets.bat";



        public HPCPrepParameters()
        {
            Console.WriteLine("DivideTargetLibraryX64");
            cores = 800;
            //WorkingDirectory = @"\\picfs\projects\DMS\PIC_HPC\Home";
            WorkingDirectory = @"\\picfs\projects\DMS\PIC_HPC";
            WorkingFolder = "Home";
            DatasetFileNameNoEnding = "Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12";
            WorkingDirectoryForExe = @"\\picfs\projects\DMS\ScottK\ScottK_PUB-100X_Launch_Folder\ToPIC";
            TargetsNoEnding = "L_10_IQ_TargetsFirstAll_R";
            FactorsName = "Factors_L10.txt";
            ExecutorParameterFile = "SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Work HM Test.xml";
            IQParameterFile = "FragmentedTargetedWorkflowParameters_Velos_DH.txt";
            DataSetDirectory = @"\\picfs\projects\DMS\PIC_HPC\Home\RawData";

            //parameter files to make
            makeResultsListName = "HPC_MakeResultsList_Asterisks.bat";
            divideTargetsParameterFile = "HPC-Parameters_DivideTargetsPIC_Asterisks.txt";
            consoleOperatingParameters = @"WorkingParameters\GlyQIQ_Diabetes_Parameters_PICFS.txt";
            scottsFirstHPLCLauncher = "HPC_ScottsFirstHPLCLauncher.bat";
            HPC_MultiSleepParameterFileGlobal = "HPC_MultiSleepParameterFileGlobal";
            //ipaddress = "172.17.130.199";//faster
            ipaddress = "172.16.112.12";//picfs
            LogIpaddress = "172.17.130.199";//picnvas
            ExeLaunchDirectory = @"\\picfs\projects\DMS\ScottK\ScottK_PUB-100X_Launch_Folder\ToPIC";
            HPCNodeGroupName = "ComputeNodes";
            IsHPC = "false";
            DatasetFileExtenstion = "raw";
            FrankenDelete = "true";
            ClusterName = "Deception2.pnnl.gov";
            TemplateName = "GlyQIQ";
            MaxTargetNumber = 3000;//number of pieces to break into

			PICHPCUsername = string.Empty;
			PICHPCPassword = string.Empty;		// Note: this is an encoded password
        }

        public void LoadParameters(string parameterFileName)
        {
			var splitChars = new char[] { ',' };

			var reader = new StringLoadTextFileLine();
			List<string> lines = reader.SingleFileByLine(parameterFileName);
			int validParameters = 0;

			foreach (var line in lines)
			{
				string[] words = line.Split(splitChars, 2);
				if (words.Length > 0)
				{
					validParameters++;

					switch (words[0])
					{
						case "Cores":
							cores = Convert.ToInt32(words[1]);
							break;
						case "WorkingDirectory":
							WorkingDirectory = words[1];
							break;
						case "WorkingFolder":
							WorkingFolder = words[1];
							break;
						case "DataSetDirectory":
							DataSetDirectory = words[1];
							break;
						case "DatasetFileName":
							DatasetFileNameNoEnding = words[1];
							break;
						case "WorkingDirectoryForExe":
							WorkingDirectoryForExe = words[1];
							break;
						case "Targets":
							TargetsNoEnding = words[1];
							break;
						case "FactorsName":
							FactorsName = words[1];
							break;
						case "ExecutorParameterFile":
							ExecutorParameterFile = words[1];
							break;
						case "IQParameterFile":
							IQParameterFile = words[1];
							break;
						case "MakeResultsListName":
							makeResultsListName = words[1];
							break;
						case "DivideTargetsParameterFile":
							divideTargetsParameterFile = words[1];
							break;
						case "ConsoleOperatingParameters":
							consoleOperatingParameters = words[1];
							break;
						case "ScottsFirstHPLCLauncher":
							scottsFirstHPLCLauncher = words[1];
							break;
						case "HPC_MultiSleepParameterFileGlobal_Root":
							HPC_MultiSleepParameterFileGlobal = words[1];
							break;
						case "ipaddress":
							ipaddress = words[1];
							break;
						case "LogIpAddress":
							LogIpaddress = words[1];
							break;
						case "HPCExeLaunchDirectory":
							ExeLaunchDirectory = words[1];
							break;
						case "HPCClusterGroupName-ComputeNodes-AzureNodes-@PNNL-Kronies":
							HPCNodeGroupName = words[1];
							break;
						case "isThisToBeRunOnHPC":
							IsHPC = words[1];
							break;
						case "DataSetFileExtension":
							DatasetFileExtenstion = words[1];
							break;
						case "FrankenDelete":
							FrankenDelete = words[1];
							break;
						case "ClusterName":
							ClusterName = words[1];
							break;
						case "TemplateName":
							TemplateName = words[1];
							break;
						case "MaxTargetNumber":
							MaxTargetNumber = Convert.ToInt32(words[1]);
							break;
						case "PICHPCUsername":
							PICHPCUsername = words[1];
							break;
						case "PICHPCPassword":
							PICHPCPassword = words[1];
							break;
						default:
							validParameters--;
							break;

					}
					
				}
			}
			
			if (validParameters < 25)
				Console.WriteLine("Warning, loaded " + validParameters + " valid parameters; expecting 25");				
			else
				Console.WriteLine("Parameters Loaded");

        }

        public void Write(string parameterFileName)
        {
			var writer = new StringListToDisk();
			var data = new List<string>();
            data.Add("Cores" + "," + cores.ToString(CultureInfo.InvariantCulture));
            data.Add("WorkingDirectory" + "," + WorkingDirectory);
            data.Add("WorkingFolder" + "," + WorkingFolder);
            data.Add("DataSetDirectory" + "," + DataSetDirectory);
            data.Add("DatasetFileName" + "," + DatasetFileNameNoEnding);
            data.Add("WorkingDirectoryForExe" + "," + WorkingDirectoryForExe);
            data.Add("Targets" + "," + TargetsNoEnding);
            data.Add("FactorsName" + "," + FactorsName);
            data.Add("ExecutorParameterFile" + "," + ExecutorParameterFile);
            data.Add("IQParameterFile" + "," + IQParameterFile);

            data.Add("MakeResultsListName" + "," + makeResultsListName);
            data.Add("DivideTargetsParameterFile" + "," + divideTargetsParameterFile);
            data.Add("ConsoleOperatingParameters" + "," + consoleOperatingParameters);
            data.Add("ScottsFirstHPLCLauncher" + "," + scottsFirstHPLCLauncher);
            data.Add("HPC_MultiSleepParameterFileGlobal_Root" + "," + HPC_MultiSleepParameterFileGlobal);
            data.Add("ipaddress" + "," + ipaddress);
            data.Add("LogIpAddress" + "," + LogIpaddress);
            data.Add("HPCExeLaunchDirectory" + "," + ExeLaunchDirectory);
            data.Add("HPCClusterGroupName-ComputeNodes-AzureNodes-@PNNL-Kronies" + "," + HPCNodeGroupName);
            data.Add("isThisToBeRunOnHPC" + "," + IsHPC);
            data.Add("DataSetFileExtension" + "," + DatasetFileExtenstion);
            data.Add("FrankenDelete" + "," + FrankenDelete);

            data.Add("ClusterName" + "," + ClusterName);
            data.Add("TemplateName" + "," + TemplateName);
            data.Add("MaxTargetNumber" + "," + MaxTargetNumber);

			data.Add("PICHPCUsername" + "," + PICHPCUsername);
			data.Add("PICHPCPassword" + "," + PICHPCPassword);

            writer.toDiskStringList(parameterFileName,data);
        }
    }

}
