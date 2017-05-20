using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Synthetic_Spectra_DLL_X64_Net_4.Launch;

namespace Synthetic_Spectra_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            int massInt64Shift = 100000;//  1000.12345 will be 100012345
            double intensityInt64Shift = 1;
            string fileListLocation;
            string outputFileDestination = "";

            if (1 == 1)
            {
                fileListLocation = @"D:\Csharp\Syn Output\ParameterFiles\0_FileLoadIndex5Syn.txt";
                //outputFileDestination = @"d:\Csharp\Syn Output\LCSynthetic.txt";
                outputFileDestination = @"d:\Csharp\Syn Output\";
            }
            else
            {
                fileListLocation = @"g:\PNNL Files\Csharp\Syn Output\ParameterFiles\0_HomeFileLoadIndex5Syn.txt";
                //outputFileDestination = @"g:\PNNL Files\Csharp\Syn Output\LCSynthetic.txt";
                outputFileDestination = @"g:\PNNL Files\Csharp\Syn Output\";
            }

            SyntheticSpectraLaunchParameters ParametersApplicationLevel = new SyntheticSpectraLaunchParameters();
            ParametersApplicationLevel.FileListLocation = fileListLocation;
            ParametersApplicationLevel.FileOutputLocation = outputFileDestination;
            ParametersApplicationLevel.MassInt64Shift = massInt64Shift;
            ParametersApplicationLevel.IntensityInt64Shift = intensityInt64Shift;
            ParametersApplicationLevel.OutputToTextFiles = true;
            ParametersApplicationLevel.OutputToYafms = true;
            SyntheticSpectraLaunch getMeASpectra = new SyntheticSpectraLaunch();
            getMeASpectra.Launch(ParametersApplicationLevel);

            Console.WriteLine("complete, Press return to exit");
            Console.ReadKey();
        }
    }
}
