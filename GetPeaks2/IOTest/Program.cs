using System;
using System.Collections.Generic;
using GetPeaksDllLite.DataFIFO;
using IQGlyQ;
using IQGlyQ.Processors;
using Run32.Backend.Core;
using Run32.Backend.Runs;
using Run32.Utilities;

namespace IOTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = args[0];
            string outFileName = args[1];

            int numMSScansToSum = 5;
            RunFactory rf = new RunFactory();
            var run = rf.CreateRun(fileName);

            Console.WriteLine("RunCreated");

            Check.Ensure(run != null, "RunUtilites could not create run. Run is null.");
            //end copy run

           
            ///this is only copy of Run scan set creation!!!!!!
            run.ScanSetCollection.Create(run, numMSScansToSum, 1, false);//This is the only scan set collection creator!

            ScanSet scanSelected = SelectClosest.SelectClosestScanSetToScan(run, 1000);

            ProcessingParametersMassSpectra msParameters = new ProcessingParametersMassSpectra();
            msParameters.MsGeneratorParameters.MsFileType = run.MSFileType;
            ProcessorMassSpectra _msProcessor = new ProcessorMassSpectra(msParameters);
            Run32.Backend.Data.XYData spectraAtMaxEICPeak = _msProcessor.DeconMSGeneratorWrapper(run, scanSelected);

            Console.WriteLine("the first mass is " + spectraAtMaxEICPeak.Xvalues[0]);

            List<string> dataToWrite = new List<string>();
            dataToWrite.Add(spectraAtMaxEICPeak.Xvalues[0].ToString());
            dataToWrite.Add("If there is a number above this line, The MSFileReader.XRawfile2.dll registration worked");
            StringListToDisk writer = new StringListToDisk();

            writer.toDiskStringList(outFileName,dataToWrite);
        }
    }
}
