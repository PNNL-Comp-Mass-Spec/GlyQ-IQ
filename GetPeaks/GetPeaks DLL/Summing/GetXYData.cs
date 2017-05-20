using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks;
using GetPeaks_DLL.Go_Decon_Modules;

namespace GetPeaks_DLL.Summing
{
    public static class GetXYData
    {
        public static void GetData(Run run)
        {
            try
            {
                //MSGeneratorFactory msgenFactory = new MSGeneratorFactory();
                //Task msGenerator = msgenFactory.CreateMSGenerator(run.MSFileType);

                
                //since the scanset has several scans, the msGenerator will sum them together
                //run.MSParameters.MinMZ = 0;//TODO is this important? 9-4-12
                //run.MSParameters.MaxMZ = 5000;//TODO is this important? 9-4-12

                GoSpectraGenerator.GenerateMS(run);

                //msgenFactory = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
    }
}
