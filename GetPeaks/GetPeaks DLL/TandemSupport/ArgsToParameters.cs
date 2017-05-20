using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Functions;
using GetPeaks_DLL.ConosleUtilities;
using GetPeaks_DLL.Objects;
using PNNLOmics.Algorithms.PeakDetection;

namespace GetPeaks_DLL.TandemSupport
{
    public static class ArgsToParameters
    {
        public static SimpleWorkflowParameters Load(string[] args)//create eluting peaks and store them in run.ResultsCollection.ElutingPeaks
        {
            SimpleWorkflowParameters parameters = new SimpleWorkflowParameters();
            //List<TandemObject> tandemScan = new List<TandemObject>();
            //TandemObject newpoint = new TandemObject();
            //tandemScan.Add(newpoint);

            #region setup main parameter file from args

            List<string> mainParameterFile;
            List<string> stringListFromParameterFile;

            ConvertArgsToStringList argsToList = new ConvertArgsToStringList();
            argsToList.Convert(args, out mainParameterFile, out stringListFromParameterFile);
            //argsToList.Convert2013(args, out mainParameterFile, out stringListFromParameterFile);

            #endregion

            #region convert parameter file to variables
            string version = stringListFromParameterFile[0];
            string serverDataFileName = stringListFromParameterFile[1];
            string folderID = stringListFromParameterFile[2];
            int startScan = Convert.ToInt32(stringListFromParameterFile[3]);
            int endScan = Convert.ToInt32(stringListFromParameterFile[4]);
            int serverBlockTotal = Convert.ToInt32(stringListFromParameterFile[5]);
            int serverBlock = Convert.ToInt32(stringListFromParameterFile[6]);
            double DataSpecificMassNeutron = Convert.ToDouble(stringListFromParameterFile[7]);
            double part1SN = Convert.ToDouble(stringListFromParameterFile[8]);
            double part2SN = Convert.ToDouble(stringListFromParameterFile[9]);
            string DeconType = stringListFromParameterFile[10];

            int numberOfDeconvolutionThreads = Convert.ToInt32(stringListFromParameterFile[11]);

            #endregion

            DeconvolutionType loadedDeconvolutionType = new DeconvolutionType();
            ConvertAToB converter = new ConvertAToB();
            loadedDeconvolutionType = converter.stringTODeconvolutionType(DeconType);

            parameters.FileNameUsed = serverDataFileName;
            parameters.Part1Parameters.StartScan = startScan;
            parameters.Part1Parameters.StopScan = endScan;
            parameters.Part1Parameters.ElutingPeakNoiseThreshold = part1SN;//when NoiseRemoved, take 3 sigma off before the orbitrap filter
            parameters.Part1Parameters.NoiseType = InstrumentDataNoiseType.NoiseRemoved;
            parameters.Part1Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
            parameters.Part1Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
            parameters.Part1Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;//6000 is start point
            parameters.Part2Parameters.DeconvolutionType = loadedDeconvolutionType;

            return parameters;
        }
    }
}
