using System;
using System.Collections.Generic;
using GetPeaks_DLL.DataFIFO;

namespace HammerPeakDetectorConsole
{
    public class PeakDetectionParameters
    {
        /// <summary>
        /// data is here 
        /// </summary>
        public string WorkingDirectory { get; set; }

        public string OutputDirectory { get; set; }

        /// <summary>
        /// file name with path and .raw
        /// </summary>
        public string DataFileName { get; set; }
        public string DataFileExtension { get; set; }

        public string DataFileNameWithPath  { get; set; }
        public string OutputPeaksFileNameWithPath { get; set; }


        /// <summary>
        /// how many scans to sum prior to peak detection
        /// </summary>
        public int ScansToSum { get; set; }

        /// <summary>
        /// which peak detector to use.  true will run hammer, false will run decon
        /// </summary>
        public bool isHammer { get; set; }

        /// <summary>
        /// int mode = 1;  //cluster and threshold. thresholde by 0 or some sigma above moving average.  we keep all peak above this moving threshold including peaks that are not in clusters
        /// int mode = 2; //cluster only with optimize with 1 iteration.  
        /// int mode = 3; //cluster and threshold with optimize with 1 iteration. we keep all peak above this moving threshold including peaks that are not in clusters
        /// int mode = 4; //cluster optimize with many iterations. 
        /// </summary>
        public int HammerMode { get; set; }

        /// <summary>
        /// decon specific parameters
        /// </summary>
        public double PeakToBackgroundRatio { get; set; }
        public double SignalToNoiseThreshold { get; set; }

        /// <summary>
        /// just ms1 = false or all msms = true
        /// </summary>
        public bool ProcessMSMS { get; set; }

        /// <summary>
        /// if there is all the low abundant noise present, this whould be false.  Orbitrap data this is true
        /// </summary>
        public bool IsDataThresholdedAndNoiseFloorRemoved { get; set; }

        /// <summary>
        /// load parameter files from parameter file
        /// </summary>
        /// <param name="parameterFilePath"></param>
        public void LoadParameters(string parameterFilePath)
        {

            var splitter = new char[] { '='};

            var reader = new StringLoadTextFileLine();
            List<string> lines = reader.SingleFileByLine(parameterFilePath);

			foreach (var dataLine in lines)
			{
				var lineTrimmed = dataLine.Trim();
				if (lineTrimmed.StartsWith("#"))
					continue;

				string[] words = lineTrimmed.Split(splitter, 2);

				if (words.Length < 2)
					continue;

				string key = words[0];
				string parameter = words[1];

				switch (key)
				{
					case "ScansToSum": 
						ScansToSum = TryConvertToInt(parameter); 
						break;
					case "UseHammer":
						isHammer = Convert.ToBoolean(parameter); 
						break;
					case "HammerMode": 
						HammerMode = TryConvertToInt(parameter); 
						break;
					case "PeakToBackgroundRatio":
						PeakToBackgroundRatio = Convert.ToDouble(parameter);
						break;
					case "SignalToNoiseThreshold":
						SignalToNoiseThreshold = Convert.ToDouble(parameter);
						break;
					case "ProcessMSMS":
						ProcessMSMS = Convert.ToBoolean(parameter); 
						break;
					case "IsDataThresholdedAndNoiseFloorRemoved":
						IsDataThresholdedAndNoiseFloorRemoved = Convert.ToBoolean(parameter); 
						break;					
				}
			}

			// Assure that ScansToSum is odd
			if (ScansToSum == 0 || ScansToSum % 2 == 0)
		        throw new ArgumentOutOfRangeException("ScansToSum must be an odd number, not " + ScansToSum);

			if (!isHammer)
	        {
		        if (PeakToBackgroundRatio < Single.Epsilon)
			        throw new ArgumentOutOfRangeException("PeakToBackgroundRatio must be positive, not " + PeakToBackgroundRatio);

		        if (SignalToNoiseThreshold < Single.Epsilon)
			        throw new ArgumentOutOfRangeException("SignalToNoiseThreshold must be positive, not " +
			                                              SignalToNoiseThreshold);
	        }
        }

        public void UpdatePaths()
        {
            DataFileNameWithPath = System.IO.Path.Combine(WorkingDirectory, DataFileName + DataFileExtension);

            OutputPeaksFileNameWithPath = System.IO.Path.Combine(OutputDirectory, DataFileName + "_peaks.txt");
        }

        private static int TryConvertToInt(string input)
        {
            int numVal = 0;
            try
            {
                numVal = Convert.ToInt32(input);
            }
            catch (FormatException)
            {
                Console.WriteLine("Input string is not a sequence of digits.");
            }
            catch (OverflowException)
            {
                Console.WriteLine("The number cannot fit in an Int32.");
            }
            
            return numVal;
        }


        public PeakDetectionParameters()
        {
        }

        public PeakDetectionParameters(string workingDirectory, string outputDirectory ,string fileName, string datafileExtnesion)
        {
            DataFileName = fileName;
            DataFileExtension = datafileExtnesion;
            WorkingDirectory = workingDirectory;

            DataFileNameWithPath = workingDirectory + @"\" + fileName + "." + datafileExtnesion;

            OutputPeaksFileNameWithPath = outputDirectory + @"\" + fileName + "_peaks.txt";
        }


        public void WriteParameters(string parameterFileName)
        {
            string divider = ",";
            List<string> lines = new List<string>();
            lines.Add("Working Directory" + divider + WorkingDirectory);
            lines.Add("Peaks Output Directory" + divider + OutputDirectory);
            //lines.Add("Data File Name no Ending" + divider + DataFileName);
            //lines.Add("Data File Extenstion" + divider + DataFileExtension);
            lines.Add("LC Scans to Sum (odd number)" + divider + ScansToSum);
            
            string  writeDetector;
            if (isHammer)
            {
                writeDetector = "true";
            }
            else
            {
                writeDetector = "false";
            }

            lines.Add("true=Hammer or false=Decontools)" + divider + writeDetector);
            lines.Add("HammerMode" + divider + HammerMode);
            lines.Add("Decon Peak To Background Ratio" + divider + PeakToBackgroundRatio);
            lines.Add("Decon Peak Signal to Noise Threshold" + divider + SignalToNoiseThreshold);
            lines.Add("ProcessMSMS" + divider + ProcessMSMS);
            lines.Add("IsDataThresholdedAndNoiseFloorRemoved" + divider + IsDataThresholdedAndNoiseFloorRemoved);
            StringListToDisk writer= new StringListToDisk();
            writer.toDiskStringList(parameterFileName,lines);
        }

    }
}
