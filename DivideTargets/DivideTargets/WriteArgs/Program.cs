using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaksDllLite.DataFIFO;

namespace WriteArgs
{
    class Program
    {
        static void Main(string[] args)
        {
            string targetsFilePath = @"E:\ScottK\IQ\RunFiles\L_10_IQ_TargetsFirst3.txt";

            string dataFileParentPath = @"Z:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
            string dataFileEnding =  @".raw";
            string coresString = "2";
            string appLocationPath = @"R:\PNNL RAM 500\GetPeaks\IQGlyQ_Console\bin\Release\IQGlyQ_Console.exe";
            string outputLocationPath =  @"E:\ScottK\IQ\RunFiles";
            string lockControllerName = "LockController.txt";


            string q = "\"";//quote
            string line = 
                q + @"L:\PNNL Files\PNNL CSharp\SK IQDivideTargets\DivideTargets\DivideTargets\bin\Release\DivideTargets.exe" + q + " " +
                q + targetsFilePath + q + " " +
                q + dataFileParentPath + q + " " +
                q + dataFileEnding + q + " " +
                q + coresString + q + " " +
                q + appLocationPath + q + " " +
                q + outputLocationPath + q + "" +
                q + lockControllerName;

            List<string> toWrite = new List<string>();
            toWrite.Add(line);

            string runName = "argsForDiviteTargets.bat";
            string runPath = outputLocationPath + @"\" + runName;
            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(runPath, toWrite);

        }
    }
}
