using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using GetPeaks_DLL.DataFIFO;
using GetPeaksDllLite.DataFIFO;
using IQGlyQ.Enumerations;

namespace IQGlyQ
{
    public class FragmentedTargetedPeakProcessingParameters
    {
        /// <summary>
        /// how many scans to sum when generating MS  3 is ok if this is needed at all.  this can effect intenstities
        /// </summary>
        public int NumMSScansToSum { get; set; }

        /// <summary>
        /// SG smooth window  9 is typical for orbitrap 100min runs.  this does not effect intensties
        /// </summary>
        public int ChromSmootherNumPointsInSmooth { get; set; }

        /// <summary>
        /// if a moving average is setup, how many points are included in the kernel
        /// </summary>
        public int MovingAveragePoints { get; set; }

        /// <summary>
        /// ??
        /// </summary>
        public string ProcessLcSectionCorrelationObjectEnum { get; set; }
        public string ProcessLcChromatogramEnum { get; set; }
        
        /// <summary>
        /// how many points are a minimum to have on the shoulder of the peak
        /// </summary>
        public int PointsPerShoulder { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double ParametersOmicsThreshold { get; set; }

        /// <summary>
        /// One method for bounding the charge states
        /// </summary>
        public int MinMzForDefiningChargeStateTargets { get; set; }
        public int MaxMzForDefiningChargeStateTargets { get; set; }

        /// <summary>
        /// iso fit score cuttoff.  this includes 1Da error corrections and multiple ion corrections
        /// </summary>
        public double FitScoreCuttoff{ get; set; }

        /// <summary>
        /// allowable correlation coefficient cuttoff
        /// </summary>
        public double CorrelationScoreCuttoff{ get; set; }

        /// <summary>
        /// coeffieicnt for LC curve fitting
        /// </summary>
        public double LM_RsquaredCuttoff{ get; set; }

        /// <summary>
        /// mass accuracy to search for
        /// </summary>
        public double MSToleranceInPPM { get; set; }

        /// <summary>
        /// I think this is for Hammer
        /// </summary>
        public float SignalToShoulderCuttoff { get; set; }

        /// <summary>
        /// should we apply calibration to the targets before searching
        /// </summary>
        public bool ToCalibrate { get; set; }

        /// <summary>
        /// should we apply a shift to the isotope profiles so there is a 0 for a peak in advance of the featrue.  1Da errors
        /// </summary>
        public bool ToShiftProfile { get; set; }

        /// <summary>
        /// linear calibration shift in the m/z domain
        /// </summary>
        public double CalibrateShiftMZ { get; set; }
        
        /// <summary>
        /// a calibration shift to the monoisotopic masses.  not very common
        /// </summary>
        public double CalibrateShiftMono { get; set; }

        /// <summary>
        /// H or DH proccesing.  H is normal
        /// </summary>
        public EnumerationIsotopicProfileMode IsotopicProfileMode { get; set; }
        
        /// <summary>
        /// corrects the fit score for the number of ion in the least squares fit
        /// </summary>
        public bool DivideFitScoreByNumberOfIons { get; set; }

        /// <summary>
        /// limints the number of charge states in FragmentedTargetedIQWorkflow
        /// </summary>
        public int ChargeStateMin { get; set; }public int ChargeStateMax { get; set; }

        /// <summary>
        /// ChromToleranceInPPMInitial.  this is the first pass EIC that is good enough to commence optimization.  this coefficient is multiplied by mass ppm for initial window
        /// </summary>
        public int ChromToleranceInPPMInitialFactor { get; set; }
        
        /// <summary>
        /// MaxEICExtractPPM does not allow EIC ppm to excede this ppm
        /// </summary>
        public double ChromToleranceInPPMMax { get; set; }

        /// <summary>
        /// when auto selecting ppm for extrction, select at half width of most abundant peak (2 = full width at half max)
        /// </summary>
        public int AutoSelectEICAt_X_partOfPeakWidth { get; set; }

        /// <summary>
        /// fraction of integrated isotopic envelope that has to be detected.  0.75 means 75% of the area (peaks that add up to 75%) need to be detected in data to keep
        /// 0 means everything passes
        /// </summary>
        public double CuttOffArea { get; set; }

        public void SetParameters(string processingParametersParameterFile)
        {
            Console.WriteLine("Setting FragmentedTargetedPeakProcessingParameters Parameters...");
            
            StringLoadTextFileLine loadSpectraL = new StringLoadTextFileLine();
            List<string> linesFromParameterFile = new List<string>();
            List<string> parameterList = new List<string>();
            //load strings
            linesFromParameterFile = loadSpectraL.SingleFileByLine(processingParametersParameterFile); //loads all isos

            foreach (string line in linesFromParameterFile)
            {
                char spliter = ',';
                string[] wordArray = line.Split(spliter);
                if (wordArray.Length == 2)
                    parameterList.Add(wordArray[1]);
            }

            if (parameterList.Count != 25)
            {
                Console.WriteLine("Missing Parameters in FragmentedTargetedPeakProcessingParameters, File name " + processingParametersParameterFile + " should have 25 parameters");
                System.Threading.Thread.Sleep(3000);
            }

            NumMSScansToSum = Convert.ToInt32(parameterList[0]);
            ChromSmootherNumPointsInSmooth = Convert.ToInt32(parameterList[1]);
            MovingAveragePoints = Convert.ToInt32(parameterList[2]);
            ProcessLcSectionCorrelationObjectEnum = parameterList[3];
            ProcessLcChromatogramEnum = parameterList[4];
            PointsPerShoulder = Convert.ToInt32(parameterList[5]);
            ParametersOmicsThreshold = Convert.ToDouble(parameterList[6]);
            MinMzForDefiningChargeStateTargets = Convert.ToInt32(parameterList[7]);
            MaxMzForDefiningChargeStateTargets = Convert.ToInt32(parameterList[8]);
            FitScoreCuttoff = Convert.ToDouble(parameterList[9]);
            CorrelationScoreCuttoff = Convert.ToDouble(parameterList[10]);
            LM_RsquaredCuttoff = Convert.ToDouble(parameterList[11]);
            MSToleranceInPPM = Convert.ToDouble(parameterList[12]);
            SignalToShoulderCuttoff = Convert.ToSingle(parameterList[13]);
            ToCalibrate = Convert.ToBoolean(parameterList[14]);
            CalibrateShiftMZ = Convert.ToDouble(parameterList[15]);
            CalibrateShiftMono = Convert.ToDouble(parameterList[16]);
            IsotopicProfileMode = ConvertStringToIsoMode(parameterList[17]);
            ChargeStateMin = Convert.ToInt32(parameterList[18]);
            ChargeStateMax = Convert.ToInt32(parameterList[19]);
            DivideFitScoreByNumberOfIons = Convert.ToBoolean(parameterList[20]);
            ChromToleranceInPPMInitialFactor = Convert.ToInt32(parameterList[21]);
            ChromToleranceInPPMMax = Convert.ToDouble(parameterList[22]);
            AutoSelectEICAt_X_partOfPeakWidth = Convert.ToInt32(parameterList[23]);
            CuttOffArea = Convert.ToDouble(parameterList[24]);
        }
       

        public EnumerationIsotopicProfileMode ConvertStringToIsoMode(string isoModeIn)
        {
            EnumerationIsotopicProfileMode isoMode = new EnumerationIsotopicProfileMode();
            isoMode = EnumerationIsotopicProfileMode.Unknown;
            if(isoModeIn=="H" || isoModeIn=="h")
            {
                isoMode = EnumerationIsotopicProfileMode.H;
            }
            if (isoModeIn == "DH" || isoModeIn == "dh")
            {
                isoMode = EnumerationIsotopicProfileMode.DH;
            }
            return isoMode;
        }
    }
}
