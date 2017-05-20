using GetPeaks_DLL.Functions;
using Parallel.THRASH;

namespace Parallel
{
    public static class  ParalellHardDrive
    {
        public static string Select(bool MultithreadedHardDriveMode, ParametersTHRASH thrashParameters, int engineNumber)
        {
            string fileNameToUse = "";

            if(MultithreadedHardDriveMode ==true)
            {
                fileNameToUse = RemoveEnding.RAW(thrashParameters.FileInforamation.InputFileName) + " (" + engineNumber + ").RAW";
            }
            else
            {
                fileNameToUse = thrashParameters.FileInforamation.InputFileName;
            }
            return fileNameToUse;
        }
    }
}
