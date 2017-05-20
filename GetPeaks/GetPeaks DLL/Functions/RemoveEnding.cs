using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Functions
{
    public static class RemoveEnding
    {
        public static string RAW(string inputDatasetFileName)
        {
            string item = @".RAW";
            string choppedFileName;
            if (inputDatasetFileName.EndsWith(item))
            {
                choppedFileName = inputDatasetFileName.Substring(0, inputDatasetFileName.LastIndexOf(item));
            }
            else
            {
                choppedFileName = inputDatasetFileName;
            }

            return choppedFileName;
        }
    }
}
