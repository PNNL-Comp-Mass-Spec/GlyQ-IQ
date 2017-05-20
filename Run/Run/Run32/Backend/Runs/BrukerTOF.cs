using System;
using System.IO;
#if BRUKER_EDAL
using EDAL;
#endif
using Run32.Backend.Core;
using Run32.Backend.Data;

namespace Run32.Backend.Runs
{
	public class BrukerTOF : Run
	{
#if BRUKER_EDAL
        private EDAL.IMSAnalysis _msAnalysis;
        private MSSpectrumCollection _spectrumCollection;
#endif
		public BrukerTOF()
		{
			XYData = new XYData();
			IsDataThresholded = true;
			MSFileType = Globals.MSFileType.Bruker;
			ContainsMSMSData = false;
		}


		public BrukerTOF(string folderName)
			: this()
		{
#if !BRUKER_EDAL
			throw new Exception("Bruker TOF data is not supported since EDAL was not present at the time of compilation");
#endif

			if (!Directory.Exists(folderName))
			{
				throw new DirectoryNotFoundException(
					"Could not create Bruker dataset. Folder path does not exist. Ensure you are pointing to parent folder that contains the raw MS files (eg analysis.baf)");

			}

			bool isDir;

			try
			{
				isDir = (File.GetAttributes(folderName) & FileAttributes.Directory)
				 == FileAttributes.Directory;

			}
			catch (Exception exception)
			{

				throw new IOException("Could not create Bruker dataset. Folder path " + folderName + " does not exist. Ensure you are pointing to parent folder that contains the raw MS files (eg analysis.baf)", exception);

			}

#if BRUKER_EDAL

            _msAnalysis = new MSAnalysis();
            _msAnalysis.Open(folderName);

            Filename = folderName;
            _spectrumCollection = _msAnalysis.MSSpectrumCollection;
#endif

			DatasetName = getDatasetName(Filename);
			DataSetPath = getDatasetfolderName(Filename);

			MinLCScan = GetMinPossibleLCScanNum();
			MaxLCScan = GetMaxPossibleLCScanNum();

		}




		public override XYData XYData { get; set; }
		public override int GetNumMSScans()
		{
#if BRUKER_EDAL
				return _spectrumCollection.Count;
#else
			return 0;
#endif
		}

		public override double GetTime(int scanNum)
		{

#if BRUKER_EDAL
			var spectrum = _spectrumCollection[scanNum];


			//NOTE: retention time is reported in seconds. DeconTools normally reports in Minutes. So need to change this.
			return spectrum.RetentionTime / 60;

#else
			return 0;
#endif


		}

		public override int GetMSLevelFromRawData(int scanNum)
		{
#if BRUKER_EDAL
			var spectrum = _spectrumCollection[scanNum];

			return spectrum.MSMSStage;

#else
			return 0;
#endif

		}

		public override XYData GetMassSpectrum(ScanSet scanset, double minMZ, double maxMZ)
		{
			return GetMassSpectrum(scanset);
		}

		public override XYData GetMassSpectrum(ScanSet scanset)
		{
			object mzVals;
			object intensityVals;

#if !BRUKER_EDAL
			return new XYData();
#else
			var spectrum = _spectrumCollection[scanset.PrimaryScanNumber];


			spectrum.GetMassIntensityValues(SpectrumTypes.SpectrumType_Profile, out mzVals, out intensityVals);

			XYData xydata = new XYData();
			xydata.Xvalues = (double[])mzVals;
			xydata.Yvalues = (double[])intensityVals;
			return xydata;
#endif

		}


		public override int GetMinPossibleLCScanNum()
		{
			return 1;     //one-based
		}

		public override int GetMaxPossibleLCScanNum()
		{
			return GetNumMSScans();
		}

		private string getDatasetName(string fullFolderPath)
		{

			DirectoryInfo dirInfo = new DirectoryInfo(fullFolderPath);

			if (dirInfo.Name.EndsWith(".d", StringComparison.OrdinalIgnoreCase))
			{
				return dirInfo.Name.Substring(0, dirInfo.Name.Length - ".d".Length);
			}
			else
			{
				return dirInfo.Name;
			}

		}

		private string getDatasetfolderName(string fullFolderPath)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(fullFolderPath);
			return dirInfo.FullName;
		}

	}
}
