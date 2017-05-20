using System;
using System.Collections.Generic;
using System.IO;
using HammerPeakDetectorConsole;

namespace HammerOrDeconSimplePeakDetector
{
    static class Program
    {
		public const string PROGRAM_DATE = "May 29, 2014";
        
		private static string mDataFilePath;
		private static string mParamFilePath;
	    private static string mOutputFolderPath;

		public static int Main()
        {
			var objParseCommandLine = new clsParseCommandLine();

			mDataFilePath = string.Empty;
			mParamFilePath = string.Empty;
			mOutputFolderPath = string.Empty;

            try
            {
				bool success = false;

				if (objParseCommandLine.ParseCommandLine())
				{
					if (SetOptionsUsingCommandLineParameters(objParseCommandLine))
						success = true;
				}

				if (!success ||
					objParseCommandLine.NeedToShowHelp ||
					objParseCommandLine.ParameterCount + objParseCommandLine.NonSwitchParameterCount == 0)
				{
					ShowProgramHelp();
					return -2;
				}

	            if (string.IsNullOrEmpty(mDataFilePath))
	            {
		            ShowErrorMessage("Input data file path not provided");
		            ShowProgramHelp();
		            return -3;
	            }

				if (!File.Exists(mDataFilePath))
				{
					ShowErrorMessage("Input data file not found: " + mDataFilePath);
					return -4;
				}

	            if (string.IsNullOrEmpty(mParamFilePath))
	            {
		            ShowErrorMessage("Parameter file path not provided");
		            ShowProgramHelp();
		            return -5;
	            }

				if (!File.Exists(mParamFilePath))
				{
					ShowErrorMessage("Parameter file not found: " + mParamFilePath);
					return -6;
				}

	            if (string.IsNullOrEmpty(mOutputFolderPath))
		            mOutputFolderPath = ".";

	            try
	            {
		            var diOutputfolder = new DirectoryInfo(mOutputFolderPath);
		            if (!diOutputfolder.Exists)
			            diOutputfolder.Create();
	            }
	            catch (Exception ex)
	            {
		            ShowErrorMessage("Error creating the missing output folder: " + ex.Message);
		            return -7;
	            }

	            try
	            {
		            PeakDetectorEngine.Execute(mDataFilePath, mOutputFolderPath, mParamFilePath);
	            }
	            catch (Exception ex)
	            {
		            ShowErrorMessage("Error running the Peak Detector: " + ex.Message);
		            return -8;
	            }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred in Program->Main: " + Environment.NewLine + ex.Message);
                return -1;
            }

            return 0;
		}

		private static string GetAppVersion()
		{
			return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " (" + PROGRAM_DATE + ")";
		}

		private static bool SetOptionsUsingCommandLineParameters(clsParseCommandLine objParseCommandLine)
        {
            // Returns True if no problems; otherwise, returns false

			var lstValidParameters = new List<string> {"I", "O", "P"};

            try
            {
                // Make sure no invalid parameters are present
                if (objParseCommandLine.InvalidParametersPresent(lstValidParameters))
                {					
					var badArguments = new List<string>();
					foreach (string item in objParseCommandLine.InvalidParameters(lstValidParameters))
					{
						badArguments.Add("/" + item);
					}

					ShowErrorMessage("Invalid commmand line parameters", badArguments);
                
                    return false;
                }
	            
		        // Query objParseCommandLine to see if various parameters are present						
		        string strValue;
		        if (objParseCommandLine.RetrieveValueForParameter("I", out strValue))
		        {
			        mDataFilePath = string.Copy(strValue);
		        }
		        else if (objParseCommandLine.NonSwitchParameterCount > 0)
		        {
			        mDataFilePath = objParseCommandLine.RetrieveNonSwitchParameter(0);
		        }

		        if (objParseCommandLine.RetrieveValueForParameter("O", out strValue))
		        {
			        mOutputFolderPath = string.Copy(strValue);
		        }

		        if (objParseCommandLine.RetrieveValueForParameter("P", out strValue))
		        {
			        mParamFilePath = string.Copy(strValue);
		        }

	            return true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error parsing the command line parameters: " + Environment.NewLine + ex.Message);
            }

            return false;
        }

		
		private static void ShowErrorMessage(string strMessage)
		{
			const string strSeparator = "------------------------------------------------------------------------------";

			Console.WriteLine();
			Console.WriteLine(strSeparator);
			Console.WriteLine(strMessage);
			Console.WriteLine(strSeparator);
			Console.WriteLine();

			WriteToErrorStream(strMessage);
		}

		private static void ShowErrorMessage(string strTitle, IEnumerable<string> items)
		{
			const string strSeparator = "------------------------------------------------------------------------------";

			Console.WriteLine();
			Console.WriteLine(strSeparator);
			Console.WriteLine(strTitle);
			string strMessage = strTitle + ":";

			foreach (string item in items) {
				Console.WriteLine("   " + item);
				strMessage += " " + item;
			}
			Console.WriteLine(strSeparator);
			Console.WriteLine();

			WriteToErrorStream(strMessage);
		}


        private static void ShowProgramHelp()
        {
			string exeName = Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            try
            {
				Console.WriteLine();
                Console.WriteLine("This program detects peaks in a Thermo .Raw file using the Hammer Peak Detector or the Simple Peak Detector");
                Console.WriteLine();
                Console.WriteLine("Program syntax:" + Environment.NewLine + exeName);

				Console.WriteLine(" RawFilePath /P:ParameterFilePath [/O:OutputFolder]");

				Console.WriteLine();
				Console.WriteLine("Both a thermo .raw file and a parameter file must be provided");
				Console.WriteLine();
				Console.WriteLine("If /O is not used, then the output file will be created in the working directory");
				
				Console.WriteLine();
                Console.WriteLine("Program written by Scott Kronewitter and Matthew Monroe for the Department of Energy (PNNL, Richland, WA) in 2014");
                Console.WriteLine("Version: " + GetAppVersion());
                Console.WriteLine();

                Console.WriteLine("E-mail: matthew.monroe@pnl.gov or matt@alchemistmatt.com");
                Console.WriteLine("Website: http://omics.pnl.gov/ or http://www.sysbio.org/resources/staff/");
                Console.WriteLine();

                // Delay for 750 msec in case the user double clicked this file from within Windows Explorer (or started the program via a shortcut)
                System.Threading.Thread.Sleep(750);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error displaying the program syntax: " + ex.Message);
            }

        }

		private static void WriteToErrorStream(string strErrorMessage)
		{
			try {
				using (var swErrorStream = new StreamWriter(Console.OpenStandardError())) {
					swErrorStream.WriteLine(strErrorMessage);
				}
			} 
			// ReSharper disable once EmptyGeneralCatchClause
			catch 
			{
				// Ignore errors here
			}
		}
		
		static void ShowErrorMessage(string message, bool pauseAfterError)
		{
			Console.WriteLine();
			Console.WriteLine("===============================================");

			Console.WriteLine(message);

			if (pauseAfterError)
			{
				Console.WriteLine("===============================================");
				System.Threading.Thread.Sleep(1500);
			}
		}

    }

}
