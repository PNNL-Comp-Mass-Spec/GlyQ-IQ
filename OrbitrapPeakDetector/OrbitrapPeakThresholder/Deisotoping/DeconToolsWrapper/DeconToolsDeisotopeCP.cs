using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using GetPeaks_DLL.Objects.TandemMSObjects;
using GetPeaks_DLL;
using GetPeaks_DLL.Objects;
using PNNLOmics.Data.Constants;
using PNNLOmics.Data.Constants.Libraries;

namespace Deisotoping.DeconToolsWrapper.DeconToolsWrapper
{
    public class DeconToolsDeisotopeCP
    {
        public List<IsosObject> Deisotope(SortedDictionary<int, XYData> signalDictionary, SortedDictionary<int, XYData> loadedRawDataFromFileDictionary, InputOutputFileName fileIn)
        {
            //ConvertAutoData xyConvert = new ConvertAutoData();

           
            //convert lists
            List<XYData> simpleXYList = new List<XYData>();
            foreach (KeyValuePair<int, XYData> point in loadedRawDataFromFileDictionary)
            {
                simpleXYList.Add(point.Value);

            }
            
            List<ProcessedPeak> simpleProcessedPeakList = new List<ProcessedPeak>();
            foreach (KeyValuePair<int, XYData> point in signalDictionary)
            {
                ProcessedPeak dataPoint = new ProcessedPeak();
                dataPoint.XValue = point.Value.X;
                dataPoint.Height = point.Value.Y;
                dataPoint.LocalSignalToNoise = 10;
                dataPoint.SignalToNoiseGlobal = 100;
                dataPoint.Width = (float)0.01;

                simpleProcessedPeakList.Add(dataPoint);
            }

            InputOutputFileName newFile = new InputOutputFileName();
            newFile = fileIn;

            SimpleWorkflowParameters parameters = new SimpleWorkflowParameters();
            parameters.Part2Parameters.numberOfDeconvolutionThreads = 1;
            parameters.Part2Parameters.MSPeakDetectorPeakBR = 1;
            parameters.Part2Parameters.DeconvolutionType = DeconvolutionType.Thrash;
            parameters.FileNameUsed = newFile.InputFileName;
            

            TandemObject testObject = new TandemObject();
            testObject.Parameters = parameters;
            testObject.PrecursorScanPeaks = simpleProcessedPeakList;
            testObject.PrecursorData = simpleXYList;
            testObject.NewFile = newFile;

            testObject.DeisotopeFragmentationPeaks();

            List<ProcessedPeak> monoResults = testObject.PrecursorMonoIsotopicPeaks;
            List<int> chargeStateResults = testObject.PrecursorScanChargeStates;



            List<IsosObject> monoResultsWithChargeStates = new List<IsosObject>();
            for(int i=0;i<monoResults.Count;i++)
            {
                IsosObject newMonoPeak = new IsosObject();
                newMonoPeak.charge = chargeStateResults[i];
                newMonoPeak.monoisotopic_mw = monoResults[i].XValue;
                newMonoPeak.mono_abundance = monoResults[i].Height;
                newMonoPeak.mz = (newMonoPeak.monoisotopic_mw + newMonoPeak.charge * Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic)/ newMonoPeak.charge;
                monoResultsWithChargeStates.Add(newMonoPeak);
            }

            return monoResultsWithChargeStates;
        }
    }
}
