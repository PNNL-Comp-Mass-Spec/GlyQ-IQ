using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivideTargetsLibraryX64.Combine;

namespace CombineResults
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new string[1];
            args[0] = @"D:\Csharp\ConosleApps\LocalServer\IQ\GlyQ-IQ\PIC\CombineFilesParameterFile.txt";
            //args[0] = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\GlyQ-IQ\PICCombineFilesParameterFile.txt";
            CombineParameters parameters = new CombineParameters(args[0]);

            ResultsFiles.ConsolidateFiles(parameters);



            //Console.ReadKey();
        }

        
    }
}
