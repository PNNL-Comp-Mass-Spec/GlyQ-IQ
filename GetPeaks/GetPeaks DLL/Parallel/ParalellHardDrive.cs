using GetPeaks_DLL.Functions;
using GetPeaks_DLL.Go_Decon_Modules;

namespace GetPeaks_DLL.Parallel
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
