using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;

namespace GetPeaks_DLL.ParameterWriters
{
    public class GetPeaksParameterFileCreator
    {
        public int CreateFile(SimpleWorkflowParameters parametersToSet, string fileName, string folderID, string outputLocation)
        {
            int didThisWork = 0;

            List<string> whatToWrite = new List<string>();

            whatToWrite.Add("data file," + fileName + ", raw or YAFMS LCMS data file");
            whatToWrite.Add("folderID," + folderID + ", identifier that is added into the folder name and onto the files");
            whatToWrite.Add("startScan," + parametersToSet.Part1Parameters.StartScan + ", first scan to start with");
            whatToWrite.Add("endScan," + parametersToSet.Part1Parameters.StopScan + ", last scan to use");
            whatToWrite.Add("number of serverblocks in total," + parametersToSet.Part2Parameters.MemoryDivider.NumberOfBlocks +", unique block of data to run 4");
            whatToWrite.Add("serverblock," + parametersToSet.Part2Parameters.MemoryDivider.BlockNumber +", unique block of data to run 0,1,2,3");
            whatToWrite.Add("Mass neutron data specific," + parametersToSet.Part1Parameters.ParametersOrbitrap.massNeutron +", the difference between monoisotopic peak and c13 etc");
            whatToWrite.Add("Part1Sigma Threshold," + parametersToSet.Part1Parameters.MSPeakDetectorPeakBR+", how many sigma above the average value");
            whatToWrite.Add("Part2Sigma Threhosld," + parametersToSet.Part2Parameters.MSPeakDetectorPeakBR+", how many sigma above the average value");
            whatToWrite.Add("DeconcolutionType," + parametersToSet.Part2Parameters.DeconvolutionType + ",use RAPID or THRASH deconvolution");
            whatToWrite.Add("DeconvolutionCores," + parametersToSet.Part2Parameters.numberOfDeconvolutionThreads + ", number of threads to allow 1,2,4,16,24 etc.");
            whatToWrite.Add("Level 1 Summing," + parametersToSet.Part1Parameters.ScansToBeSummed+", how many scans to sum during eluting peak generation. 1,3,5,7 etc.");
            whatToWrite.Add("Level 2 Summing Method," + parametersToSet.SummingMethod+", select MaxScan, SumScan, or AlignScan to deal with eluting peak scan ranges prior to Thrash");
           
            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(outputLocation, whatToWrite, "GetPeaks,Version 1.0, Parameter File version");
            didThisWork = 1;
            return didThisWork;
        }
    }
}
