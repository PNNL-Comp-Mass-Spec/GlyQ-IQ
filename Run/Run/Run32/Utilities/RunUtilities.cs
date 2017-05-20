using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Run32.Utilities
{
    public class RunUtilities
    {

        #region Constructors
        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public static string GetDatasetName(string datasetPath)
        {
            string datasetName;

            FileAttributes attr = File.GetAttributes(datasetPath);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                DirectoryInfo sourceDirInfo;
                sourceDirInfo = new DirectoryInfo(datasetPath);
                datasetName = sourceDirInfo.Name;
            }
            else
            {
                datasetName = Path.GetFileNameWithoutExtension(datasetPath);
            }

            return datasetName;

        }


        public static string GetDatasetParentFolder(string datasetPath)
        {
            string datasetFolderPath;

            FileAttributes attr = File.GetAttributes(datasetPath);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                DirectoryInfo sourceDirInfo;
                sourceDirInfo = new DirectoryInfo(datasetPath);
                datasetFolderPath = sourceDirInfo.FullName;
            }
            else
            {
                datasetFolderPath = Path.GetDirectoryName(datasetPath);
            }

            return datasetFolderPath;

        }

        #endregion

        #region Private Methods

        #endregion



        public static List<int> FindPrimaryLcScanNumbers(IEnumerable<Run32.Backend.DTO.MSPeakResult> msPeaks)
		{
			HashSet<int> primaryLcScanNumbers = new HashSet<int>();

			foreach (var msPeakResult in msPeaks)
			{
				int scan = msPeakResult.FrameNum > 0 ? msPeakResult.FrameNum : msPeakResult.Scan_num;
				primaryLcScanNumbers.Add(scan);
			}

			return primaryLcScanNumbers.ToList();
		}
    }
}
