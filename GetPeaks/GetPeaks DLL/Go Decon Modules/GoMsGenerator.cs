using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks;

namespace GetPeaks_DLL.Go_Decon_Modules
{
    public static class GoSpectraGenerator
    {
        public static void GenerateMS(Run run)
        {
            var msGenerator = MSGeneratorFactory.CreateMSGenerator(run.MSFileType);
            msGenerator.Execute(run.ResultCollection);
            msGenerator.Cleanup();
            msGenerator = null;
        }
    }
}
