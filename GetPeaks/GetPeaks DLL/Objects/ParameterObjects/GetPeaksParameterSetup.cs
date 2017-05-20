using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.ConosleUtilities;
using PNNLOmics.Algorithms.PeakDetection;
using DeconTools.Backend.Core;

namespace GetPeaks_DLL.Objects.ParameterObjects
{
    public class GetPeaksParameterSetup
    {
        public static SimpleWorkflowParameters ParameterRoundHouse(List<string> mainParameterFile, BatchFileGetPeaksParameterObject parametersFromOutside, InputOutputFileName newFile, Run run)
        {
            SimpleWorkflowParameters parameters = new SimpleWorkflowParameters();

            DeconvolutionType loadedDeconvolutionType = new DeconvolutionType();
            ConvertAToB converter = new ConvertAToB();
            loadedDeconvolutionType = converter.stringTODeconvolutionType(parametersFromOutside.DeconType);

            MemorySplitObject newMemorySplitter = new MemorySplitObject();
            newMemorySplitter.NumberOfBlocks = parametersFromOutside.serverBlockTotal;
            newMemorySplitter.BlockNumber = parametersFromOutside.serverBlock;

            WriteVariablesToConsole(newFile, parametersFromOutside.startScan, parametersFromOutside.endScan, parametersFromOutside.DataSpecificMassNeutron, parametersFromOutside.part1SN, parametersFromOutside.part2SN, newMemorySplitter);

            #region Enumerate Parameters for complete data set to find eluting peaks

            parameters = new SimpleWorkflowParameters();

            //part 1 peak detector decontools only.  this gets overwritten below
            parameters.Part1Parameters.MSPeakDetectorPeakBR = 1.3;
            parameters.Part1Parameters.ElutingPeakNoiseThreshold = 2;
            parameters.Part2Parameters.MSPeakDetectorPeakBR = 1.3;
            //parameters.Part2Parameters.MSPeakDetectorSigNoise = 2;
            parameters.Part2Parameters.MSPeakDetectorSigNoise = parametersFromOutside.part2SN;//run 1

            parameters.ConsistancyCrossErrorPPM = 20;
            parameters.FileNameUsed = newFile.InputFileName;
            parameters.SummingMethod = SummingMethodSelector(parametersFromOutside.sumMethod);

            //controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = 3;//data 1 when NoiseRemoved, take 3 sigma off before the orbitrap filter
            parameters.Part1Parameters.ElutingPeakNoiseThreshold = parametersFromOutside.part1SN;//when NoiseRemoved, take 3 sigma off before the orbitrap filter
            parameters.Part1Parameters.NoiseType = InstrumentDataNoiseType.NoiseRemoved;
            parameters.Part1Parameters.isDataAlreadyThresholded = false;//true for orbitrap
            parameters.Part1Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
            parameters.Part1Parameters.StartScan = parametersFromOutside.startScan;
            parameters.Part1Parameters.StopScan = parametersFromOutside.endScan;
            parameters.Part1Parameters.MaxHeightForNewPeak = 0.75;
            parameters.Part1Parameters.AllignmentToleranceInPPM = 15;
            parameters.Part1Parameters.ParametersOrbitrap.massNeutron = parametersFromOutside.DataSpecificMassNeutron;
            parameters.Part1Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;
            parameters.Part1Parameters.ScansToBeSummed = parametersFromOutside.part1ScansToSum;//default is 3
            parameters.Part1Parameters.ParametersOrbitrap.withLowMassesAllowed = true;
            parameters.Part1Parameters.ParametersOrbitrap.ExtraSigmaFactor = 1.5;//1=default
            parameters.Part1Parameters.MSLevelOnly = true;//only look at ms level 1

            parameters.Part2Parameters.NoiseType = InstrumentDataNoiseType.Standard;
            parameters.Part2Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
            parameters.Part2Parameters.DeconvolutionType = loadedDeconvolutionType;
            parameters.Part2Parameters.Multithread = false;
            parameters.Part2Parameters.DynamicRangeToOne = 300000;
            parameters.Part2Parameters.MaxScanSpread = 500;
            parameters.Part2Parameters.ParametersOrbitrap.massNeutron = parametersFromOutside.DataSpecificMassNeutron;
            parameters.Part2Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;
            parameters.Part2Parameters.MemoryDivider = newMemorySplitter;
            parameters.Part2Parameters.numberOfDeconvolutionThreads = parametersFromOutside.numberOfDeconvolutionThreads;
            parameters.Part2Parameters.ParametersOrbitrap.withLowMassesAllowed = true;
            parameters.Part2Parameters.ParametersOrbitrap.ExtraSigmaFactor = 1.5;//1=default
            parameters.Part2Parameters.MSLevelOnly = true;//only look at ms level 1

            #endregion

            return parameters;
        }

        private static void WriteVariablesToConsole(InputOutputFileName newFile, int startScan, int endScan, double DataSpecificMassNeutron, double part1SN, double part2SN, MemorySplitObject newMemorySplitter)
        {
            Console.WriteLine("SQLite Output: " + newFile.OutputSQLFileName + "\n");
            Console.WriteLine("Startscan: " + startScan + " EndScan: " + endScan);
            Console.WriteLine("Part1 SN: " + part1SN + " Part2 SN: " + part2SN);
            Console.WriteLine("DataSpecificMassNeutron: " + DataSpecificMassNeutron);
            Console.WriteLine("Processing Block# " + (newMemorySplitter.BlockNumber + 1).ToString() + " of " + newMemorySplitter.NumberOfBlocks);
            Console.WriteLine("Press Enter");
            //Console.ReadKey();
        }

        private static ScanSumSelectSwitch SummingMethodSelector(string sumMethod)
        {
            ScanSumSelectSwitch summingSwitch = new ScanSumSelectSwitch();
            switch (sumMethod)
            {
                case "MaxScan":
                    {
                        summingSwitch = ScanSumSelectSwitch.MaxScan;//sum scans prior to deisotoping or take best scan
                        break;
                    }
                case "SumScan":
                    {
                        summingSwitch = ScanSumSelectSwitch.SumScan;//sum scans prior to deisotoping or take best scan
                        break;
                    }
                case "AlignScan":
                    {
                        summingSwitch = ScanSumSelectSwitch.AlignScan;//sum scans prior to deisotoping or take best scan
                        Console.WriteLine("Summing Method is not implemented yet");
                        //Console.ReadKey();
                        break;
                    }
                default:
                    {
                        summingSwitch = ScanSumSelectSwitch.MaxScan;//sum scans prior to deisotoping or take best scan
                        Console.WriteLine("Summing Method type is not clear");
                        Console.ReadKey();
                        break;
                    }
            }
            return summingSwitch;
        }

    }
}
