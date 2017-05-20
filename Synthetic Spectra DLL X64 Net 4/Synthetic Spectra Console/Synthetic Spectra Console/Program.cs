using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Synthetic_Spectra_DLL_X64_Net_4.Launch;
using System.Diagnostics;

namespace Synthetic_Spectra_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            int massInt64Shift = 100000;//  1000.12345 will be 100012345
            double intensityInt64Shift = 1;
            string fileListLocation;
            string outputFileDestination = "";

            int location = 0;
            switch (location)
            {
                case 0:
                    {
                        fileListLocation = @"D:\Csharp\Syn Output\ParameterFiles\0_FileLoadIndex5Syn.txt";
                        //outputFileDestination = @"d:\Csharp\Syn Output\LCSynthetic.txt";
                        outputFileDestination = @"d:\Csharp\Syn Output\";
                    }
                    break;

                case 1:
                    {
                        fileListLocation = @"g:\PNNL Files\Csharp\Syn Output\ParameterFiles\0_HomeFileLoadIndex5Syn.txt";
                        //outputFileDestination = @"g:\PNNL Files\Csharp\Syn Output\LCSynthetic.txt";
                        outputFileDestination = @"g:\PNNL Files\Csharp\Syn Output\";
                    }
                    break;
                case 2:
                    {
                        fileListLocation = @"e:\ScottK\Synthetic Spectra Parameters\0_FileLoadIndex5Syn.txt";
                        outputFileDestination = @"e:\ScottK\Synthetic Spectra Output\";
                    }
                    break;
                default:
                    {
                        fileListLocation = @"D:\Csharp\Syn Output\ParameterFiles\0_FileLoadIndex5Syn.txt";
                        //outputFileDestination = @"d:\Csharp\Syn Output\LCSynthetic.txt";
                        outputFileDestination = @"d:\Csharp\Syn Output\";
                    }
                    break;
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

            stopWatch.Stop();
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to make the data set");

            Console.WriteLine("complete, Press return to exit");
            Console.ReadKey();
        }
    }
}
