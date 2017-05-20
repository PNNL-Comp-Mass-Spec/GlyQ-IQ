
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.SQLite.DataTransferObjects;
using PNNLOmics.Data.Features;
using DeconTools.Backend.Core;
using PNNLOmics.Data;
using GetPeaks_DLL.Objects.TandemMSObjects;
using YAFMS_DB.GetPeaks;
using ScanCentric = GetPeaks_DLL.Objects.TandemMSObjects.ScanCentric;

namespace GetPeaks_DLL.SQLite.DataTransferConverters
{
    public static class SetDataTransferObjects
    {

        /// <summary>
        /// Decon ElutingPeak
        /// </summary>
        /// <param name="ePeak"></param>
        /// <returns></returns>
        public static DatabaseElutingPeakObject SetElutingPeak(ElutingPeak ePeak)
        {
            DatabaseElutingPeakObject outputObject = new DatabaseElutingPeakObject();

            //convert eluting peak to DTO.  Other fields are in featurelite
            outputObject.ElutingPeakID = ePeak.ID;
            outputObject.ElutingPeakMass = ePeak.Mass;

            outputObject.ElutingPeakScanStart = ePeak.ScanStart;
            outputObject.ElutingPeakScanMaxIntensity = ePeak.ScanMaxIntensity;
            outputObject.ElutingPeakScanEnd = ePeak.ScanEnd;

            outputObject.ElutingPeakNumberofPeaks = ePeak.NumberOfPeaks;
            outputObject.ElutingPeakNumberOfPeaksFlag = ePeak.NumberOfPeaksFlag;
            outputObject.ElutingPeakNumberOfPeaksMode = ePeak.NumberOfPeaksMode;

            outputObject.ElutingPeakSummedIntensity = ePeak.SummedIntensity;
            outputObject.ElutingPeakIntensityAggregate = ePeak.AggregateIntensity;

            return outputObject;
        }

        /// <summary>
        /// Omics ElutingPeak
        /// </summary>
        /// <param name="ePeak"></param>
        /// <returns></returns>
        public static DatabaseElutingPeakObject SetElutingPeak(ElutingPeakOmics ePeak)
        {
            DatabaseElutingPeakObject outputObject = new DatabaseElutingPeakObject();

            //convert eluting peak to DTO.  Other fields are in featurelite
            outputObject.ElutingPeakID = ePeak.ID;
            outputObject.ElutingPeakMass = ePeak.Mass;

            outputObject.ElutingPeakScanStart = ePeak.ScanStart;
            outputObject.ElutingPeakScanMaxIntensity = ePeak.ScanMaxIntensity;
            outputObject.ElutingPeakScanEnd = ePeak.ScanEnd;

            outputObject.ElutingPeakNumberofPeaks = ePeak.NumberOfPeaks;
            outputObject.ElutingPeakNumberOfPeaksFlag = ePeak.NumberOfPeaksFlag;
            outputObject.ElutingPeakNumberOfPeaksMode = ePeak.NumberOfPeaksMode;

            outputObject.ElutingPeakSummedIntensity = ePeak.SummedIntensity;
            outputObject.ElutingPeakIntensityAggregate = ePeak.AggregateIntensity;

            return outputObject;
        }

        /// <summary>
        /// Omcis feature lite
        /// </summary>
        /// <param name="FeatureLite"></param>
        /// <returns></returns>
        public static DatabaseFeatureLiteObject SetFeatureLite(FeatureLight FeatureLite)
        {
            DatabaseFeatureLiteObject outputObject = new DatabaseFeatureLiteObject();

            outputObject.ID = FeatureLite.ID;
            outputObject.Abundance = FeatureLite.Abundance;
            outputObject.ChargeState = FeatureLite.ChargeState;
            outputObject.DriftTime = FeatureLite.DriftTime;
            outputObject.MassMonoisotopic = FeatureLite.MassMonoisotopic;
            outputObject.RetentionTime = FeatureLite.RetentionTime;
            outputObject.Score = FeatureLite.Score;

            outputObject.Values.Add(outputObject.ID);
            outputObject.Values.Add(outputObject.Abundance);
            outputObject.Values.Add(outputObject.ChargeState);
            outputObject.Values.Add(outputObject.DriftTime);
            outputObject.Values.Add(outputObject.MassMonoisotopic);
            outputObject.Values.Add(outputObject.Mass);
            outputObject.Values.Add(outputObject.RetentionTime);
            outputObject.Values.Add(outputObject.Score);

            outputObject.TableName = "FeatureLiteTable";

            return outputObject;
        }

        /// <summary>
        /// scotts isos information
        /// </summary>
        /// <param name="iObject"></param>
        /// <param name="scanNum"></param>
        /// <returns></returns>
        public static DatabaseIsotopeObject SetToIsotopeStorageOutput(IsotopeObject iObject, int scanNum)
        {
            DatabaseIsotopeObject newIsotopeObject = new DatabaseIsotopeObject();

            newIsotopeObject.MonoIsotopicMass = iObject.MonoIsotopicMass;
            newIsotopeObject.ExperimentMass = iObject.ExperimentMass;
            newIsotopeObject.IsotopeIntensitiesCSV = "inprogress";// SetIsotopeIntensitiesCSV(iObject);
            //newIsotopeObject.IsotopeMassesCSV = SetIsotopeMassesCSV(iObject);
            newIsotopeObject.IsotopeMassesCSV = iObject.IsotopeMassString;
          
            newIsotopeObject.scan_num = scanNum;
            newIsotopeObject.charge = iObject.Charge;
            //abundance
            newIsotopeObject.mz = iObject.ExperimentMass;
            newIsotopeObject.fit = iObject.FitScore;
            newIsotopeObject.average_mw = 0;//TODO
            newIsotopeObject.monoisotopic_mw = iObject.MonoIsotopicMass;
            newIsotopeObject.mostabundant_mw = 0;//TODO
            //fwhm
            //signal_noise
            //mono_abundance
            //mono_plus2_abundance
            newIsotopeObject.flag = 0;//TODO
            newIsotopeObject.interference_score = 0;//TODO

            if (iObject.IsotopeList.Count > 0)
            {
                newIsotopeObject.abundance = iObject.IsotopeList[0].Height;

                newIsotopeObject.fwhm = iObject.IsotopeList[0].Width;
                newIsotopeObject.signal_noise = iObject.IsotopeList[0].LocalSignalToNoise;
                newIsotopeObject.mono_abundance = iObject.IsotopeList[0].Height;
                if (iObject.IsotopeList.Count > 1)
                {
                    newIsotopeObject.mono_plus2_abundance = iObject.IsotopeList[1].Height;
                }
            }
            else
            {
                newIsotopeObject.abundance = 1;
                newIsotopeObject.fwhm = 1;
                newIsotopeObject.signal_noise = 1;
                newIsotopeObject.mono_abundance = 1;
                newIsotopeObject.mono_plus2_abundance = 1;

            }

            newIsotopeObject.Values.Add(newIsotopeObject.MonoIsotopicMass);
            newIsotopeObject.Values.Add(newIsotopeObject.ExperimentMass);
            newIsotopeObject.Values.Add(newIsotopeObject.IsotopeMassesCSV);
            newIsotopeObject.Values.Add(newIsotopeObject.IsotopeIntensitiesCSV);

            newIsotopeObject.Values.Add(newIsotopeObject.scan_num);
            newIsotopeObject.Values.Add(newIsotopeObject.charge);
            newIsotopeObject.Values.Add(newIsotopeObject.abundance);
            newIsotopeObject.Values.Add(newIsotopeObject.mz);
            newIsotopeObject.Values.Add(newIsotopeObject.fit);
            newIsotopeObject.Values.Add(newIsotopeObject.average_mw);
            newIsotopeObject.Values.Add(newIsotopeObject.monoisotopic_mw);
            newIsotopeObject.Values.Add(newIsotopeObject.mostabundant_mw);
            newIsotopeObject.Values.Add(newIsotopeObject.fwhm);
            newIsotopeObject.Values.Add(newIsotopeObject.signal_noise);
            newIsotopeObject.Values.Add(newIsotopeObject.mono_abundance);
            newIsotopeObject.Values.Add(newIsotopeObject.mono_plus2_abundance);
            newIsotopeObject.Values.Add(newIsotopeObject.flag);
            newIsotopeObject.Values.Add(newIsotopeObject.interference_score);

            return newIsotopeObject;
        }

        /// <summary>
        /// PNNL omics Peak information
        /// </summary>
        /// <param name="iObject"></param>
        /// <returns></returns>
        //public static DatabasePeakObject SetPeakOutput(Peak aPeak, int scanNum)
        public static DatabaseDeconPeakObject SetPeakOmicsOutput(PNNLOmics.Data.Peak aPeak, int scanNum)
        {
            DatabaseDeconPeakObject newPeakObject = new DatabaseDeconPeakObject();
            newPeakObject.PeakIndex = -1;
            newPeakObject.ScanNum = scanNum;
            newPeakObject.MZ = aPeak.XValue;
            newPeakObject.Intensity = aPeak.Height;
            newPeakObject.SignalToNoise = aPeak.LocalSignalToNoise;
            newPeakObject.MSFeatureID = -1;

            newPeakObject.Values.Add(newPeakObject.PeakIndex);
            newPeakObject.Values.Add(newPeakObject.ScanNum);
            newPeakObject.Values.Add(newPeakObject.MZ);
            newPeakObject.Values.Add(newPeakObject.Intensity);
            newPeakObject.Values.Add(newPeakObject.SignalToNoise);
            newPeakObject.Values.Add(newPeakObject.SignalToNoise);
            newPeakObject.Values.Add(newPeakObject.MSFeatureID);

            return newPeakObject;
        }

        /// <summary>
        /// PNNL omics Peak information
        /// </summary>
        /// <param name="iObject"></param>
        /// <returns></returns>
        //public static DatabasePeakObject SetPeakOutput(Peak aPeak, int scanNum)
        public static DatabaseDeconPeakObject SetPeakDeconOutput(DeconTools.Backend.Core.Peak aPeak, int scanNum)
        {
            DatabaseDeconPeakObject newPeakObject = new DatabaseDeconPeakObject();
            newPeakObject.PeakIndex = -1;
            newPeakObject.ScanNum = scanNum;
            newPeakObject.MZ = aPeak.XValue;
            newPeakObject.Intensity = aPeak.Height;
            newPeakObject.SignalToNoise = -1;
            newPeakObject.MSFeatureID = -1;

            newPeakObject.Values.Add(newPeakObject.PeakIndex);
            newPeakObject.Values.Add(newPeakObject.ScanNum);
            newPeakObject.Values.Add(newPeakObject.MZ);
            newPeakObject.Values.Add(newPeakObject.Intensity);
            newPeakObject.Values.Add(newPeakObject.SignalToNoise);
            newPeakObject.Values.Add(newPeakObject.SignalToNoise);
            newPeakObject.Values.Add(newPeakObject.MSFeatureID);

            return newPeakObject;
        }

        /// <summary>
        /// Processed Peak from PNNL Omics
        /// </summary>
        /// <param name="pPeak"></param>
        /// <returns></returns>
        public static DatabasePeakProcessedObject SetPeakProcessedOutput(ProcessedPeak pPeak, int peakNumber)
        {
            DatabasePeakProcessedObject newPeakObject = new DatabasePeakProcessedObject();
            newPeakObject.ScanNum = pPeak.ScanNumber;
            newPeakObject.PeakNumber = peakNumber;
            newPeakObject.XValue = pPeak.XValue;
            newPeakObject.Height = pPeak.Height;
            newPeakObject.Charge = pPeak.Charge;
            newPeakObject.LocalSignalToNoise = pPeak.LocalSignalToNoise;
            newPeakObject.Background = pPeak.Background;
            newPeakObject.Width = pPeak.Width;
            newPeakObject.LocalLowestMinimaHeight = pPeak.LocalLowestMinimaHeight;
            newPeakObject.SignalToBackground = pPeak.SignalToBackground;
            newPeakObject.SignalToNoiseGlobal = pPeak.SignalToNoiseGlobal;
            newPeakObject.SignalToNoiseLocalMinima = pPeak.SignalToNoiseLocalHighestMinima;

            newPeakObject.Values.Add(newPeakObject.ScanNum);
            newPeakObject.Values.Add(newPeakObject.PeakNumber);
            newPeakObject.Values.Add(newPeakObject.XValue);
            newPeakObject.Values.Add(newPeakObject.Height);
            newPeakObject.Values.Add(newPeakObject.Charge);
            newPeakObject.Values.Add(newPeakObject.LocalSignalToNoise);
            newPeakObject.Values.Add(newPeakObject.Background);
            newPeakObject.Values.Add(newPeakObject.Width);
            newPeakObject.Values.Add(newPeakObject.LocalLowestMinimaHeight);
            newPeakObject.Values.Add(newPeakObject.SignalToBackground);
            newPeakObject.Values.Add(newPeakObject.SignalToNoiseGlobal);
            newPeakObject.Values.Add(newPeakObject.SignalToNoiseLocalMinima);

            return newPeakObject;
        }

        /// <summary>
        /// Processed Peak from PNNL Omics
        /// </summary>
        /// <param name="pPeak"></param>
        /// <returns></returns>
        public static DatabasePeakProcessedWithMZObject SetPrecursorPeakOutput(ProcessedPeak pPeak, double mzRaw, int tandemScanNumber, int precursorScanNumber, int PeakNumber, int charge)
        {
            DatabasePeakProcessedWithMZObject newPeakObject = new DatabasePeakProcessedWithMZObject();
            newPeakObject.ScanNumberTandem = tandemScanNumber;
            newPeakObject.ScanNumberPrecursor = precursorScanNumber;
            newPeakObject.PeakNumber = PeakNumber;
            newPeakObject.XValue = pPeak.XValue;
            newPeakObject.XValueRaw = mzRaw;
            newPeakObject.Charge = charge;

            newPeakObject.Height = pPeak.Height;
            newPeakObject.LocalSignalToNoise = pPeak.LocalSignalToNoise;
            newPeakObject.Background = pPeak.Background;
            newPeakObject.Width = pPeak.Width;
            newPeakObject.LocalLowestMinimaHeight = pPeak.LocalLowestMinimaHeight;

            newPeakObject.SignalToBackground = pPeak.SignalToBackground;
            newPeakObject.SignalToNoiseGlobal = pPeak.SignalToNoiseGlobal;
            newPeakObject.SignalToNoiseLocalMinima = pPeak.SignalToNoiseLocalHighestMinima;



            newPeakObject.Values.Add(newPeakObject.ScanNumberTandem);
            newPeakObject.Values.Add(newPeakObject.ScanNumberPrecursor);
            newPeakObject.Values.Add(newPeakObject.PeakNumber);
            newPeakObject.Values.Add(newPeakObject.XValue);
            newPeakObject.Values.Add(newPeakObject.XValueRaw);
            newPeakObject.Values.Add(newPeakObject.Charge);

            newPeakObject.Values.Add(newPeakObject.Height);
            newPeakObject.Values.Add(newPeakObject.LocalSignalToNoise);
            newPeakObject.Values.Add(newPeakObject.Background);
            newPeakObject.Values.Add(newPeakObject.Width);
            newPeakObject.Values.Add(newPeakObject.LocalLowestMinimaHeight);

            newPeakObject.Values.Add(newPeakObject.SignalToBackground);
            newPeakObject.Values.Add(newPeakObject.SignalToNoiseGlobal);
            newPeakObject.Values.Add(newPeakObject.SignalToNoiseLocalMinima);

            return newPeakObject;
        }

        /// <summary>
        /// for tandem scan relatioships
        /// </summary>
        /// <param name="spectra"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static DatabaseScanObject SetScanOutput(MSSpectra spectra, int index)
        {
            DatabaseScanObject sObject = new DatabaseScanObject();
            sObject.IndexId = index;
            sObject.Scan = spectra.Scan;
            sObject.MSLevel = spectra.MSLevel;

            if (spectra.GroupID > 0)
            {
                sObject.ParentScan = spectra.GroupID;
            }
            else
            {
                sObject.ParentScan = -1;
            }

            sObject.Peaks = spectra.Peaks.Count;
            sObject.PeaksProcessed = spectra.PeaksProcessed.Count;
            sObject.PeakProcessingLevel = spectra.PeakProcessingLevel.ToString();

            sObject.Values.Add(sObject.IndexId);
            sObject.Values.Add(sObject.Scan);
            sObject.Values.Add(sObject.MSLevel);
            sObject.Values.Add(sObject.ParentScan);
            sObject.Values.Add(sObject.Peaks);
            sObject.Values.Add(sObject.PeaksProcessed);
            sObject.Values.Add(sObject.PeakProcessingLevel);

            return sObject;
        }

        ///// <summary>
        ///// for peak data
        ///// </summary>
        ///// <param name="peakData"></param>
        ///// <param name="indexPeak"></param>
        ///// <param name="indexScan"></param>
        ///// <returns></returns>
        //public static DatabasePeakCentricObject SetPeakCentricOutput(PeakCentric peakData, int indexPeak, int indexScan)
        //{
        //    DatabasePeakCentricObject pObject = new DatabasePeakCentricObject();
        //    pObject.PeakCentricData.PeakID = indexPeak;
        //    pObject.PeakCentricData.ScanID = indexScan;
        //    pObject.PeakCentricData.GroupID = peakData.GroupID;
        //    pObject.PeakCentricData.MonoisotopicClusterID = peakData.MonoisotopicClusterID;
        //    pObject.PeakCentricData.FeatureClusterID = peakData.FeatureClusterID;
        //    pObject.PeakCentricData.MassMonoisotopic = peakData.MassMonoisotopic;
        //    pObject.PeakCentricData.Mass = peakData.Mz;
        //    pObject.PeakCentricData.Height = peakData.Height;
        //    pObject.PeakCentricData.Width = peakData.Width;
        //    pObject.PeakCentricData.Background = peakData.Background;
        //    pObject.PeakCentricData.ChargeState = peakData.ChargeState;
        //    pObject.PeakCentricData.MassMonoisotopic = peakData.MassMonoisotopic;
        //    pObject.PeakCentricData.Score = peakData.Score;
        //    pObject.PeakCentricData.AmbiguityScore = peakData.AmbiguityScore;

        //    pObject.PeakCentricData.isSignal = peakData.isSignal;
        //    pObject.PeakCentricData.isCentroided = peakData.isCentroided;
        //    pObject.PeakCentricData.isMonoisotopic = peakData.isMonoisotopic;
        //    pObject.PeakCentricData.isIsotope = peakData.isIsotope;
        //    pObject.PeakCentricData.isMostAbundant = peakData.isMostAbundant;
        //    pObject.PeakCentricData.isCharged = peakData.isCharged;
        //    pObject.PeakCentricData.isCorrected = peakData.isCorrected;
        //    //pObject.PeakCentricData.isPrecursorMass = peakData.isPrecursorMass;


        //    pObject.Values.Add(pObject.PeakCentricData.PeakID);
        //    pObject.Values.Add(pObject.PeakCentricData.ScanID);
        //    pObject.Values.Add(pObject.PeakCentricData.GroupID);
        //    pObject.Values.Add(pObject.PeakCentricData.MonoisotopicClusterID);
        //    pObject.Values.Add(pObject.PeakCentricData.FeatureClusterID);

        //    pObject.Values.Add(pObject.PeakCentricData.Mass);
        //    pObject.Values.Add(pObject.PeakCentricData.ChargeState);

        //    pObject.Values.Add(pObject.PeakCentricData.Height);
        //    pObject.Values.Add(pObject.PeakCentricData.Width);
        //    pObject.Values.Add(pObject.PeakCentricData.Background);
        //    pObject.Values.Add(pObject.PeakCentricData.LocalSignalToNoise); 

        //    pObject.Values.Add(pObject.PeakCentricData.MassMonoisotopic);
        //    pObject.Values.Add(pObject.PeakCentricData.Score);
        //    pObject.Values.Add(pObject.PeakCentricData.AmbiguityScore);


        //    pObject.Values.Add(pObject.PeakCentricData.isSignal);
        //    pObject.Values.Add(pObject.PeakCentricData.isCentroided);
        //    pObject.Values.Add(pObject.PeakCentricData.isMonoisotopic);
        //    pObject.Values.Add(pObject.PeakCentricData.isIsotope);
        //    pObject.Values.Add(pObject.PeakCentricData.isMostAbundant);
        //    pObject.Values.Add(pObject.PeakCentricData.isCharged);
        //    pObject.Values.Add(pObject.PeakCentricData.isCorrected);
        //    //pObject.Values.Add(pObject.PeakCentricData.isPrecursorMass);
            
        //    return pObject;
        //}

        /// <summary>
        /// for peak data
        /// </summary>
        /// <param name="peakData"></param>
        /// <param name="indexPeak"></param>
        /// <param name="indexScan"></param>
        /// <returns></returns>
        public static DatabasePeakCentricObject SetPeakCentricOutput(PeakCentric peakData, int indexPeak, int indexScan)
        {
            DatabasePeakCentricObject pObject = new DatabasePeakCentricObject();
            pObject.PeakID = indexPeak;
            pObject.ScanID = indexScan;
            pObject.GroupID = peakData.GroupID;
            pObject.MonoisotopicClusterID = peakData.MonoisotopicClusterID;
            pObject.FeatureClusterID = peakData.FeatureClusterID;
            pObject.MassMonoisotopic = peakData.MassMonoisotopic;
            pObject.Mz = peakData.Mz;
            pObject.Height = peakData.Height;
            pObject.Width = peakData.Width;
            pObject.Background = peakData.Background;
            pObject.ChargeState = peakData.ChargeState;
            pObject.MassMonoisotopic = peakData.MassMonoisotopic;
            pObject.Score = peakData.Score;
            pObject.AmbiguityScore = peakData.AmbiguityScore;

            pObject.isSignal = peakData.isSignal;
            pObject.isCentroided = peakData.isCentroided;
            pObject.isMonoisotopic = peakData.isMonoisotopic;
            pObject.isIsotope = peakData.isIsotope;
            pObject.isMostAbundant = peakData.isMostAbundant;
            pObject.isCharged = peakData.isCharged;
            pObject.isCorrected = peakData.isCorrected;
            //pObject.PeakCentricData.isPrecursorMass = peakData.isPrecursorMass;


            pObject.Values.Add(pObject.PeakID);
            pObject.Values.Add(pObject.ScanID);
            pObject.Values.Add(pObject.GroupID);
            pObject.Values.Add(pObject.MonoisotopicClusterID);
            pObject.Values.Add(pObject.FeatureClusterID);

            pObject.Values.Add(pObject.Mz);
            pObject.Values.Add(pObject.ChargeState);

            pObject.Values.Add(pObject.Height);
            pObject.Values.Add(pObject.Width);
            pObject.Values.Add(pObject.Background);
            pObject.Values.Add(pObject.LocalSignalToNoise);

            pObject.Values.Add(pObject.MassMonoisotopic);
            pObject.Values.Add(pObject.Score);
            pObject.Values.Add(pObject.AmbiguityScore);


            pObject.Values.Add(pObject.isSignal);
            pObject.Values.Add(pObject.isCentroided);
            pObject.Values.Add(pObject.isMonoisotopic);
            pObject.Values.Add(pObject.isIsotope);
            pObject.Values.Add(pObject.isMostAbundant);
            pObject.Values.Add(pObject.isCharged);
            pObject.Values.Add(pObject.isCorrected);
            //pObject.Values.Add(pObject.PeakCentricData.isPrecursorMass);

            return pObject;
        }

        /// <summary>
        /// for peak data at the simplelevel
        /// </summary>
        /// <param name="peakData"></param>
        /// <param name="indexPeak"></param>
        /// <param name="indexScan"></param>
        /// <returns></returns>
        public static DatabasePeakCentricObject SetPeakCentricLightOutput(PeakCentric peakData, int indexPeak, int indexScan)
        {
            DatabasePeakCentricObject pObject = new DatabasePeakCentricObject();
            pObject.PeakID = indexPeak;
            pObject.ScanID = indexScan;
            pObject.Mz = peakData.Mz;
            pObject.Height = peakData.Height;
            //pObject.PeakCentricData.Width = peakData.Width;


            pObject.Values.Add(pObject.PeakID);
            pObject.Values.Add(pObject.ScanID);
            pObject.Values.Add(pObject.Mz);
            pObject.Values.Add(pObject.Height);
            //pObject.Values.Add(pObject.PeakCentricData.Width);


            return pObject;
        }


        /// <summary>
        /// for scan data
        /// </summary>
        /// <param name="scanData"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static DatabaseScanCentricObject SetScanCentricOutput(ScanCentric scanData, int index)
        {
            DatabaseScanCentricObject sObject = new DatabaseScanCentricObject();
            sObject.ScanCentricData.ScanID = index;
            sObject.ScanCentricData.PeakID = scanData.PeakID;
            sObject.ScanCentricData.ScanNumLc = scanData.ScanNumLc;
            sObject.ScanCentricData.ElutionTime = scanData.ElutionTime;
            sObject.ScanCentricData.FrameNumberDt = scanData.FrameNumberDt;
            sObject.ScanCentricData.ScanNumDt = scanData.ScanNumDt;
            sObject.ScanCentricData.DriftTime = scanData.DriftTime;

            sObject.ScanCentricData.MsLevel = scanData.MsLevel;
            sObject.ScanCentricData.ParentScanNumber = scanData.ParentScanNumber;
            sObject.ScanCentricData.TandemScanNumber = scanData.TandemScanNumber;

            sObject.Values.Add(sObject.ScanCentricData.ScanID);
            sObject.Values.Add(sObject.ScanCentricData.PeakID);
            sObject.Values.Add(sObject.ScanCentricData.ScanNumLc);
            sObject.Values.Add(sObject.ScanCentricData.ElutionTime);

            sObject.Values.Add(sObject.ScanCentricData.FrameNumberDt);
            sObject.Values.Add(sObject.ScanCentricData.ScanNumDt);
            sObject.Values.Add(sObject.ScanCentricData.DriftTime);

            sObject.Values.Add(sObject.ScanCentricData.MsLevel);
            sObject.Values.Add(sObject.ScanCentricData.ParentScanNumber);
            sObject.Values.Add(sObject.ScanCentricData.TandemScanNumber);

            return sObject;
        }

        #region off
        ///// <summary>
        ///// for scan data
        ///// </summary>
        ///// <param name="fragmentData"></param>
        ///// <param name="index"></param>
        ///// <returns></returns>
        //public static DatabaseFragmentCentricObject SetFragmentCentricOutput(FragmentCentric fragmentData, int index)
        //{
        //    DatabaseFragmentCentricObject fObject = new DatabaseFragmentCentricObject();
        //    fObject.FragmentCentricData.ScanID = index;
        //    fObject.FragmentCentricData.MsLevel = fragmentData.MsLevel;
        //    fObject.FragmentCentricData.ParentScanNumber = fragmentData.ParentScanNumber;
        //    fObject.FragmentCentricData.TandemScanNumber = fragmentData.TandemScanNumber;
            
        //    fObject.Values.Add(fObject.FragmentCentricData.ScanID);
        //    fObject.Values.Add(fObject.FragmentCentricData.MsLevel);
        //    fObject.Values.Add(fObject.FragmentCentricData.ParentScanNumber);                                    
        //    fObject.Values.Add(fObject.FragmentCentricData.TandemScanNumber);

        //    return fObject;
        //}

        ///// <summary>
        ///// for scan data
        ///// </summary>
        ///// <param name="attributeData"></param>
        ///// <param name="index"></param>
        ///// <returns></returns>
        //public static DatabaseAttributeCentricObject SetAttributeCentricOutput(AttributeCentric attributeData, int index)
        //{
        //    DatabaseAttributeCentricObject aObject = new DatabaseAttributeCentricObject();
        //    aObject.AttributeCentricData.PeakID = index;
        //    aObject.AttributeCentricData.isSignal = attributeData.isSignal;
        //    aObject.AttributeCentricData.isCentroided = attributeData.isCentroided;
        //    aObject.AttributeCentricData.isMonoisotopic = attributeData.isMonoisotopic;
        //    aObject.AttributeCentricData.isIsotope = attributeData.isIsotope;
        //    aObject.AttributeCentricData.isMostAbundant = attributeData.isMostAbundant;
        //    aObject.AttributeCentricData.isCharged = attributeData.isCharged;
        //    aObject.AttributeCentricData.isCorrected = attributeData.isCorrected;
        //    aObject.AttributeCentricData.isPrecursorMass = attributeData.isPrecursorMass;

        //    aObject.Values.Add(aObject.AttributeCentricData.PeakID);
        //    aObject.Values.Add(aObject.AttributeCentricData.isSignal);
        //    aObject.Values.Add(aObject.AttributeCentricData.isCentroided);
        //    aObject.Values.Add(aObject.AttributeCentricData.isMonoisotopic);
        //    aObject.Values.Add(aObject.AttributeCentricData.isIsotope);
        //    aObject.Values.Add(aObject.AttributeCentricData.isMostAbundant);
        //    aObject.Values.Add(aObject.AttributeCentricData.isCharged);
        //    aObject.Values.Add(aObject.AttributeCentricData.isCorrected);
        //    aObject.Values.Add(aObject.AttributeCentricData.isPrecursorMass);


        //    return aObject;
        //}
        #endregion


        #region private functions

        private static string SetIsotopeIntensitiesCSV(IsotopeObject iObject)
        {
            string commaSeperatedString = "";
            for (int i = 0; i < iObject.IsotopeList.Count - 1; i++)
            {
                commaSeperatedString += iObject.IsotopeList[i].Height.ToString() + ",";
            }

            commaSeperatedString += iObject.IsotopeList[iObject.IsotopeList.Count - 1].Height.ToString();

            return commaSeperatedString;
        }

        private static string SetIsotopeMassesCSV(IsotopeObject iObject)
        {
            string commaSeperatedString = "";
            for (int i = 0; i < iObject.IsotopeList.Count - 1; i++)
            {
                commaSeperatedString += iObject.IsotopeList[i].XValue.ToString() + ",";
            }

            commaSeperatedString += iObject.IsotopeList[iObject.IsotopeList.Count - 1].XValue.ToString();

            return commaSeperatedString;
        }

        #endregion

    }
}
