using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core.Results;
using DeconToolsV2.Peaks;
using GetPeaks_DLL.THRASH;
using PNNLOmics.Data;
using DeconTools.Backend.Core;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Go_Decon_Modules;
using GetPeaks_DLL.Functions;
using PNNLOmics.Algorithms.PeakDetection;
using GetPeaks_DLL.SQLite;
using GetPeaks_DLL.Objects.TandemMSObjects;
using Parallel.THRASH;
using GetPeaks_DLL.Parallel;
using GetPeaks_DLL.Objects.Enumerations;
//using OrbitrapPeakDetection;
using DeconTools.Backend.Algorithms.ChargeStateDetermination.PattersonAlgorithm;
using Peak = DeconTools.Backend.Core.Peak;
using DeconTools.Backend;
using DeconTools.Backend.ProcessingTasks.TheorFeatureGenerator;
using DeconTools.Backend.ProcessingTasks.TargetedFeatureFinders;
using DeconTools.Backend.ProcessingTasks.FitScoreCalculators;
using GetPeaks_DLL.Isotopes;

namespace GetPeaks_DLL.TandemSupport
{
    public class SpectraTandemPopulator
    {
        /// <summary>
        /// common run since we need it so much.  We need to add a private and deconstructor
        /// </summary>
        private Run m_run;


        /// <summary>
        /// Targeted
        /// </summary>
        private Task theorFeatureGen { get; set; }
        /// <summary>
        /// Targeted
        /// </summary>
        private Task targetedFeatureFinder { get; set; }
        /// <summary>
        /// Targeted
        /// </summary>
        private Task fitScoreCalc { get; set; } 


        ///// <summary>
        ///// this is a place to store peaks for use within the algoirthm.  A good place to store the precursor peaks since they will be overwritten when deisotoped
        ///// </summary>
        //public List<IsotopeObject> IsosStorage { get; set; } 

        //private ParametersTHRASH m_ThrashParameters { get; set; }

        /// <summary>
        /// This is needd to link the data on the disk so we can load XYdata
        /// </summary>
        public InputOutputFileName InputFileName { get; set; }

        const double SetPpmErrorForTargeted = 30;//5 is default
        
        public SpectraTandemPopulator()
        {
            char letter = 'K';
            string sqLiteFile;
            int computersToDivideOver;
            int coresPerComputer;
            string logFile;
            
            //m_ThrashParameters = ParametersForTesting.Load(letter, out sqLiteFile, out computersToDivideOver, out coresPerComputer, out logFile);
        
            theorFeatureGen = new TomTheorFeatureGenerator(DeconTools.Backend.Globals.LabellingType.NONE, 0.005);

            targetedFeatureFinder = new BasicTFF(SetPpmErrorForTargeted);
            //fitScoreCalc = new MassTagFitScoreCalculator();
            fitScoreCalc = new IsotopicProfileFitScoreCalculator();
        }

        /// <summary>
        /// loads precursor scan
        /// </summary>
        /// <param name="spectra"></param>
        public void XYDataSpectraGenerator(MSSpectra spectra)
        {
            bool IsThereAScanNumber = CheckScanNumber(spectra);

            if (IsThereAScanNumber)
            {
                spectra.Peaks = GetData(spectra.Scan);
            }
        }

        /// <summary>
        /// peak processing for precursor data
        /// </summary>
        /// <param name="spectra"></param>
        /// <param name="parameters"></param>
        public void PeakGenerator(MSSpectra spectra, ParametersPeakDetection peakParameters)
        {
            Console.WriteLine(Environment.NewLine + " PEAK Thresholding in scan " + spectra.Scan);
            
            bool IsThereAScanNumber = CheckScanNumber(spectra);
            bool HasTheXYDataBeenLoaded = CheckXYDataExists(spectra);

            if (!HasTheXYDataBeenLoaded)//if not present, go get them
            {
                XYDataSpectraGenerator(spectra);
                HasTheXYDataBeenLoaded = true;
            }

            if (IsThereAScanNumber && HasTheXYDataBeenLoaded)
            {
                spectra.PeaksProcessed = PeakFindAndThreshold(spectra.Peaks, m_run, peakParameters);

                int scan = spectra.Scan;
                foreach (ProcessedPeak peak in spectra.PeaksProcessed)
                {
                    peak.ScanNumber = scan;
                }
                spectra.PeakProcessingLevel = PeakProcessingLevel.Thresholded;
            }
        }


        /// <summary>
        /// peak processing for precursor data
        /// </summary>
        /// <param name="spectra"></param>
        /// <param name="parameters"></param>
        public void CentroidGenerator(MSSpectra spectra, ParametersPeakDetection peakParameters)
        {
            Console.WriteLine(Environment.NewLine + " CENTROID Peaks in scan " + spectra.Scan);
            
            bool IsThereAScanNumber = CheckScanNumber(spectra);
            bool HasTheXYDataBeenLoaded = CheckXYDataExists(spectra);

            if (!HasTheXYDataBeenLoaded)//if not present, go get them
            {
                XYDataSpectraGenerator(spectra);
                HasTheXYDataBeenLoaded = true;
            }

            if (IsThereAScanNumber && HasTheXYDataBeenLoaded)
            {
                spectra.PeaksProcessed = FindAllPeaks(spectra.Peaks, m_run, peakParameters);//decon centroid defaulted

                int scan = spectra.Scan;
                foreach (ProcessedPeak peak in spectra.PeaksProcessed)
                {
                    peak.ScanNumber = scan;
                }
                spectra.PeakProcessingLevel = PeakProcessingLevel.Centroided;
            }
        }

        /// <summary>
        /// Get the Precursor MZ from the raw file and improve it via peak picking
        /// </summary>
        /// <param name="spectra"></param>
        /// <param name="parameters"></param>
        public void LoadPrecursorMZ(MSSpectra spectra, PrecursorInfo precursor)
        {
            //bool IsThereAScanNumber = CheckScanNumber(spectra);
            //bool HasTheXYDataBeenLoaded = CheckXYDataExists(spectra);


            //if (!HasTheXYDataBeenLoaded)//if not present, go get them
            //{
            //    this.XYDataSpectraGenerator(spectra);
            //    HasTheXYDataBeenLoaded = true;
            //}

            //bool AreThePeaksDetected = CheckProcessedPeaksExists(spectra);

            //bool IsAChildSpectrPresent = CheckSpectra(spectra.ChildSpectra[0]);
            //bool IsThereAChildScanNumber = CheckScanNumber(spectra.ChildSpectra[0]);

            //bool IsAChildSpectrPresent = true;
            //bool IsThereAChildScanNumber = true;

            bool IsAPrecursorMassPresent = CheckPrecursorMassExistsInFile(precursor);

            if (IsAPrecursorMassPresent)
            {
                #region 1.  load mass from data file

                m_run.MinLCScan = spectra.Scan;
                m_run.MaxLCScan = m_run.MinLCScan;

                int scansToSum = 1;
                try
                {
                    bool GetMSMSDataAlso = true;

                    m_run.ScanSetCollection.Create(m_run, m_run.MinLCScan, m_run.MaxLCScan, scansToSum, 1, GetMSMSDataAlso);
                    //m_run.ScanSetCollection = ScanSetCollection.Create(m_run, m_run.MinLCScan, m_run.MaxLCScan, scansToSum, 1, GetMSMSDataAlso);
                    Console.WriteLine("LoadingData2...");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
  
                switch (m_run.MSFileType)
                {
                    case DeconTools.Backend.Globals.MSFileType.YAFMS:
                        {
                            double PrecursorYAFMS = 0;
                            DatabaseReader newReader = new DatabaseReader();
                            newReader.readTandemMassSpectraData(m_run.Filename, m_run.MinLCScan, out PrecursorYAFMS);
                            spectra.PrecursorMZ = PrecursorYAFMS;
                        }
                        break;

                    case DeconTools.Backend.Globals.MSFileType.Finnigan:
                        {
                            spectra.PrecursorMZ = precursor.PrecursorMZ;

                            //string scanInfo = m_run.GetScanInfo(precursor.PrecursorScan);//0 because we are only allowing one scan through 
                            //spectra.PrecursorMZ = ParseThermoScanInfo.ExtractMass(scanInfo);
                        }
                        break;

                    case DeconTools.Backend.Globals.MSFileType.Agilent_D:
                        {
                            spectra.PrecursorMZ = precursor.PrecursorMZ;
                            spectra.PrecursorChargeState = precursor.PrecursorCharge;
                            ProcessedPeak agilentPeak = new ProcessedPeak();
                            agilentPeak.XValue = precursor.PrecursorMZ;
                            agilentPeak.Height = precursor.PrecursorIntensity;
                            spectra.PrecursorPeak = agilentPeak;
                        }
                        break;
                    default:
                        {
                            Console.WriteLine("wrong file type in TandemObject");

                            string scanInfo = m_run.GetScanInfo(m_run.ScanSetCollection.ScanSetList[0].PrimaryScanNumber);//0 because we are only allowing one scan through
                            spectra.PrecursorMZ = ParseThermoScanInfo.ExtractMass(scanInfo);
                            Console.ReadKey();
                        }
                        break;
                }

                #endregion//region 1
            }
        }

        /// <summary>
        /// refine mass by going back to the precursor spectra and finding the largest peak in the mass window around the selected ion.
        /// We also calculate a charge state from the peak data.  A full thrash charge state will be better and calcualted ans replaced elseware.
        /// </summary>
        /// <param name="precursorSpectra"></param>
        /// <param name="tandemSpectra"></param>
        public void LoadRefinedMZ(MSSpectra precursorSpectra, MSSpectra tandemSpectra, double massProton)
        {
            double mass = tandemSpectra.PrecursorMZ;
            
            bool IsThereAScanNumber = CheckScanNumber(precursorSpectra);
            bool HasTheXYDataBeenLoaded = CheckXYDataExists(precursorSpectra);


            if (!HasTheXYDataBeenLoaded)//if not present, go get them
            {
                this.XYDataSpectraGenerator(precursorSpectra);
                HasTheXYDataBeenLoaded = true;
            }

            bool AreThePeaksDetected = CheckProcessedPeaksExists(precursorSpectra);

            bool IsAPrecursorMassPresent = CheckPrecursorMassisPopulated(mass);

            if (IsThereAScanNumber && HasTheXYDataBeenLoaded && AreThePeaksDetected && IsAPrecursorMassPresent)
            {
                #region 1.  refine mass using peaks from data file

                ProcessedPeak bestHit;
                List<ProcessedPeak> anotherList = GoGetPeakFromPeakList(precursorSpectra, mass, out bestHit, massProton);

                tandemSpectra.PrecursorPeak = bestHit;

                #endregion

                #region find charge state via pattersons

                if (bestHit.XValue > 0)//go after charge state
                {
                    //clear another list
                    AnotherListDispose(ref anotherList);
       

                    DeconTools.Backend.Core.MSPeak bestHitAsDeconPeak = new MSPeak();
                    bestHitAsDeconPeak.Height = Convert.ToSingle(bestHit.Height);
                    bestHitAsDeconPeak.Width = bestHit.Width;
                    bestHitAsDeconPeak.XValue = bestHit.XValue;
                    bestHitAsDeconPeak.SignalToNoise = Convert.ToSingle(bestHit.SignalToNoiseGlobal);

                    if (precursorSpectra.Peaks.Count > 0)
                    {
                        Console.WriteLine("TheXY Peaks Have been loaded");
                    }

                    //minMass = bestHit.XValue;//include peak
                    //maxMass = bestHit.XValue + 2 + tollerance; // -0.25 to 2.25 around peak
                    //int chargeStateMax = 7;
                    //int[] chargeCount = new int[chargeStateMax];
                    //for(int i=0;i<chargeStateMax;i++)
                    //{
                    //    chargeCount[i] = 0;
                    //}

                    //1
                    //double minMassWindow = bestHit.XValue;//include peak
                    //double maxMassWindow = bestHit.XValue + 2.25; // -0.25 to 2.25 around peak

                    //List<ProcessedPeak> localDataWithin2Da = (from peak in precursorSpectra.PeaksProcessed where peak.XValue >= minMassWindow && peak.XValue <= maxMassWindow select peak).ToList();

                    List<DeconTools.Backend.Core.Peak> deconPeaks = new List<Peak>();
                    //foreach (ProcessedPeak omicsPeak in localDataWithin2Da)
                    foreach (ProcessedPeak omicsPeak in precursorSpectra.PeaksProcessed)
                    {
                        DeconTools.Backend.Core.Peak deconPeak = new Peak();
                        deconPeak.Height = Convert.ToSingle(omicsPeak.Height);
                        deconPeak.Width = omicsPeak.Width;
                        deconPeak.XValue = omicsPeak.XValue;

                        deconPeaks.Add(deconPeak);
                    }

                    //2
                    //List<PNNLOmics.Data.XYData> localXYDataWithin2Da = (from peak in precursorSpectra.Peaks where peak.X >= minMassWindow && peak.X <= maxMassWindow select peak).ToList();

                    DeconTools.Backend.XYData deconXYData = new DeconTools.Backend.XYData();
                    //deconXYData.Xvalues = new double[localXYDataWithin2Da.Count];
                    //deconXYData.Yvalues = new double[localXYDataWithin2Da.Count];
                    deconXYData.Xvalues = new double[precursorSpectra.Peaks.Count];
                    deconXYData.Yvalues = new double[precursorSpectra.Peaks.Count];
                    //for(int i=0;i<localXYDataWithin2Da.Count;i++)
                    for (int i = 0; i < precursorSpectra.Peaks.Count; i++)
                    {
                        //PNNLOmics.Data.XYData omicsPeak = localXYDataWithin2Da[i];
                        PNNLOmics.Data.XYData omicsPeak = precursorSpectra.Peaks[i];
                        deconXYData.Xvalues[i] = omicsPeak.X;
                        deconXYData.Yvalues[i] = omicsPeak.Y;
                    }


                    PattersonChargeStateCalculator chargeFinder = new PattersonChargeStateCalculator();
                    int chargestateDecon = chargeFinder.GetChargeState(deconXYData, deconPeaks, bestHitAsDeconPeak);

                    Console.WriteLine("Decon Charge State is " + chargestateDecon);

                    int finalChargeState = chargestateDecon;

                    //3
                    //bool histogramChargeState = false;
                    //if (histogramChargeState)
                    //{

                    //    int chargeStateMax = 7;
                    //    int[] chargeCount = new int[chargeStateMax];
                    //    ;
                    //    int chargestateOmics = OmcisChargestateCalculator(localDataWithin2Da, chargeStateMax, bestHit, ref chargeCount);
                    //    //int omicsChargeState = chargestate;
                    //    Console.WriteLine("Omics Charge State is " + chargestateOmics);


                    //    finalChargeState = chargestateDecon;

                    //    if (chargestateDecon == chargestateOmics)
                    //    {
                    //        Console.WriteLine("--Both charge states are the same");
                    //    }
                    //    else
                    //    {
                    //        Console.WriteLine("--One ChargeState Is correct");
                    //        if (chargestateDecon == -1)
                    //        {
                    //            finalChargeState = chargestateOmics;
                    //        }
                    //    }
                    //}
                    
                    tandemSpectra.PrecursorChargeState = finalChargeState;
                }

                #endregion

            }
        }

        private static List<ProcessedPeak> GoGetPeakFromPeakList(MSSpectra precursorSpectra, double mass, out ProcessedPeak bestHit, double massProton)
        {
            int fullSelectionWindowSize = 3;//Da
            bool takeAnyPeakInIsolationWindow = true;
            double daTollerance = 0.25; //Da

            double tempMass = mass;
            double tempDaTollerance = daTollerance;
            List<ProcessedPeak> anotherList = new List<ProcessedPeak>();

            if (takeAnyPeakInIsolationWindow)
            {
                for (int i = 0; i < 2; i++)//search good hit first then expand (loop 1<2) tollerance to huge.  if several are returned, they will be refined later (max abundance)
                {
                    double minMass = tempMass - tempDaTollerance;
                    double maxMass = tempMass + tempDaTollerance;

                    anotherList = (from peak in precursorSpectra.PeaksProcessed where peak.XValue >= minMass && peak.XValue <= maxMass select peak).ToList();
                    anotherList = anotherList.OrderBy(p => p.XValue).ToList();

                    if (anotherList.Count > 0)
                    {
                        break;//canddidate peak result found
                    }
                    else
                    {
                        tempDaTollerance = fullSelectionWindowSize / 2;
                    }

                }
            }
            else
            {
                double minMass = tempMass - daTollerance;//this is more likely to not return a peak because it is so selective
                    double maxMass = tempMass + daTollerance;

                    anotherList = (from peak in precursorSpectra.PeaksProcessed where peak.XValue >= minMass && peak.XValue <= maxMass select peak).ToList();
                    anotherList = anotherList.OrderBy(p => p.XValue).ToList();

            }
            //if we can't find a mass within error, use origional
            //ProcessedPeak bestHit;

            if (anotherList.Count > 0) //if there are more peaks in the window
            {
                //string selectionType = "Mass";
                string selectionType = "MaxIntnsity";
                switch (selectionType)
                {
                        #region inside

                    case "Mass":
                        {
                            #region inside

                            //store first pick
                            double lowestppm = ErrorCalculator.PPMAbsolute(precursorSpectra.PrecursorMZ, anotherList[0].XValue);
                            bestHit = anotherList[0];
                            //check for better picks
                            double challengePPM;
                            foreach (ProcessedPeak peak in anotherList)
                            {
                                ProcessedPeak challengerPeak = peak;
                                challengePPM = ErrorCalculator.PPMAbsolute(challengerPeak.XValue, precursorSpectra.PrecursorMZ);
                                if (challengePPM < lowestppm)
                                {
                                    lowestppm = challengePPM;
                                    bestHit = challengerPeak;
                                }
                            }

                            #endregion
                        }
                        break;
                    case "MaxIntnsity":
                        {
                            #region inside

                            //store first pick
                            double largestPeak = anotherList[0].Height;
                            bestHit = anotherList[0];
                            //check for better picks
                            double challengeHeight;
                            foreach (ProcessedPeak peak in anotherList)
                            {
                                ProcessedPeak challengerPeak = peak;
                                challengeHeight = challengerPeak.Height;
                                if (challengeHeight >= largestPeak)
                                {
                                    largestPeak = challengeHeight;
                                    bestHit = challengerPeak;
                                }
                            }

                            #endregion
                        }
                        break;
                    default:
                        {
                            bestHit = anotherList[0];
                            Console.WriteLine("Select the correct type SpectrumPopulator");
                            Console.ReadKey();
                        }
                        break;

                        #endregion
                }
            }
            else
            {
                ProcessedPeak dummyPeak = new ProcessedPeak();
                dummyPeak.XValue = precursorSpectra.PrecursorMZ; //use origional data
                dummyPeak.Height = 1; //value greater than 0;
                bestHit = dummyPeak;
            }
            return anotherList;
        }

        /// <summary>
        /// if we have a monoisotopic mass from the precursor scan we can use it to get the charge state
        /// </summary>
        /// <param name="precursorSpectra"></param>
        /// <param name="tandemSpectra"></param>
        public void RefineChargeStateOfPrecursor(List<IsotopeObject> tandemResultsOut, MSSpectra tandemSpectra)
        {
            double mass = tandemSpectra.PrecursorMZ;
            double tollerence = 0.25;

            bool IsThereAScanNumber = CheckScanNumber(tandemSpectra);
            bool HasTheXYDataBeenLoaded = CheckXYDataExists(tandemSpectra);


            if (!HasTheXYDataBeenLoaded)//if not present, go get them
            {
                this.XYDataSpectraGenerator(tandemSpectra);
                HasTheXYDataBeenLoaded = true;
            }

            bool AreThePeaksDetected = CheckProcessedPeaksExists(tandemSpectra);

            bool IsAPrecursorMassPresent = CheckPrecursorMassisPopulated(mass);

            int correctedCharge = tandemSpectra.PrecursorChargeState;
            
            if (IsThereAScanNumber && HasTheXYDataBeenLoaded && AreThePeaksDetected && IsAPrecursorMassPresent)
            {
                
                double minMassWindow = mass - tollerence;
                double maxMassWindow = mass + tollerence;

                //List<IsotopeObject> localDataWithin1Da = (from peak in precursorSpectra.PeaksProcessed where peak.XValue >= minMassWindow && peak.XValue <= maxMassWindow select peak).ToList();
                //List<XYData> localDataWithin1Da = (from peak in valuesForSearch where peak.X >= minMassWindow && peak.X <= maxMassWindow select peak).ToList();
                List<IsotopeObject> localDataWithin1Da = (from isosPeak in tandemResultsOut where isosPeak.ExperimentMass >= minMassWindow && isosPeak.ExperimentMass <= maxMassWindow select isosPeak).ToList();

                if(localDataWithin1Da.Count==1)//just take it and go
                {
                    correctedCharge = Convert.ToInt32(localDataWithin1Da[0].Charge);
                }
                else
                {
                    if(localDataWithin1Da.Count>1)
                    {
                        Console.WriteLine("There are too many peaks in the window so we don't know which is the correct one to choose");
                        //perhaps shorten window or find the one with the closes difference

                        double difference = (localDataWithin1Da[0].ExperimentMass - mass) * (localDataWithin1Da[0].ExperimentMass - mass);
                        int charge = localDataWithin1Da[0].Charge;
                        foreach (var isotopeObject in localDataWithin1Da)
                        {
                            double testDifference = (isotopeObject.ExperimentMass - mass) * (isotopeObject.ExperimentMass - mass);
                            if(testDifference<difference)
                            {
                                difference = testDifference;
                                correctedCharge = isotopeObject.Charge;
                            }
                        }

                    }
                    else
                    {
                        //leave the charge state we already have
                    }
                    
                }

                if(tandemSpectra.PrecursorChargeState != correctedCharge)
                {
                    Console.WriteLine("The Charge has been corrected from " + tandemSpectra.PrecursorChargeState + "to " + correctedCharge);
                }

            }

            tandemSpectra.PrecursorChargeState = correctedCharge;
        }

        public void ChargeStateViaTargeted(MSSpectra precursorSpectra, MSSpectra tandemSpectra, List<ProcessedPeak> centroidedPeaks, int maxCharge)
        {
            double massProton = Globals.PROTON_MASS;

            int daltonCorrectionStepTether = 3;
            //namespace DeconTools.Backend.ProcessingTasks.TargetedFeatureFinders
            //Public abstract class TFFBase : Task

            //namespace DeconTools.UnitTesting2.TargetedProcessing_Tests
            //public class TargetedProcessingOrbiDataTests

            #region 1.  get monoisotopic mass from tandem scan

            double selectedMassForFragmentation = tandemSpectra.PrecursorMZ;

            #endregion

            #region 2.  convert mZ to several Mono.  include several mono for 1Da correction.  This is needed if an isotope was selected for fragmentation

            List<ChargeObject> chargeObjectPile = new List<ChargeObject>();
            int charge;
            for (int i = 0; i < maxCharge; i++)
            {
                ChargeObject chargeMyObject = new ChargeObject();
                chargeMyObject.Charge = i + 1;
                chargeMyObject.MonoisotopicMass = ConvertMzToMono.Execute(selectedMassForFragmentation, chargeMyObject.Charge, massProton);
                chargeObjectPile.Add(chargeMyObject);
            }

            //this is where we set the window for Da correction
            int expandRangedaError = daltonCorrectionStepTether;//looks 2 da away//this is similar to isolation window

            #endregion

            #region 3.  make a new list of monos (neutral mass) that includes the dalton corrections

            for (int i = 0; i < maxCharge; i++)
            {              
                for (int daltonError = 1; daltonError < expandRangedaError + 1; daltonError++)//+1 for errors
                {
                    ChargeObject chargeMyObject = new ChargeObject();
                    chargeMyObject.Charge = i + 1;
                    double shiftedMono = chargeObjectPile[i].MonoisotopicMass - (daltonError * 1.0032);
                    chargeMyObject.MonoisotopicMass = shiftedMono;
                    chargeObjectPile.Add(chargeMyObject);
                }
            }

            chargeObjectPile = chargeObjectPile.OrderBy(p => p.MonoisotopicMass).ToList();
            chargeObjectPile = chargeObjectPile.OrderBy(p => p.Charge).ToList();
            
            //pileofMono.Sort();
            //pileofMonoCharges.Sort();//this can be a problem if since these are not coupled

            #endregion

            #region 4.  convert mono to elemental composition via averagine

            Averagine myAveragine = new Averagine();

            myAveragine = myAveragine.AveragineSetup(AveragineType.Glycan);

            string numberOfCarbons;
            string numberOfHydrogen;
            string numberOfNitrogen;
            string numberOfOxygen;
            string numberOfSulfur;
            for (int i = 0; i < chargeObjectPile.Count; i++)
            {
                charge = chargeObjectPile[i].Charge;
                double mono = chargeObjectPile[i].MonoisotopicMass;
                double factor = mono / myAveragine.isoUnit;
                numberOfCarbons = Convert.ToString(Math.Truncate(factor * myAveragine.isoC));
                numberOfHydrogen = Convert.ToString(Math.Truncate(factor * myAveragine.isoH));
                numberOfNitrogen = Convert.ToString(Math.Truncate(factor * myAveragine.isoN));
                numberOfOxygen = Convert.ToString(Math.Truncate(factor * myAveragine.isoO));
                numberOfSulfur = Convert.ToString(Math.Truncate(factor * myAveragine.isoS));

                string empericalFormula = "";
                empericalFormula += "C" + numberOfCarbons;
                empericalFormula += "H" + numberOfHydrogen;
                empericalFormula += "N" + numberOfNitrogen;
                empericalFormula += "O" + numberOfOxygen;
                empericalFormula += "S" + numberOfSulfur;

                Console.WriteLine("There are " + empericalFormula + " atoms");
                chargeObjectPile[i].EmpericalFormula = empericalFormula;
            }

            #endregion

            #region 5.  set up theoretical isotope distributions

            for (int i = 0; i < chargeObjectPile.Count; i++)
            {
                double mass = chargeObjectPile[i].MonoisotopicMass;
                string empericalForumla = chargeObjectPile[i].EmpericalFormula;

                charge = chargeObjectPile[i].Charge;
                TargetBase mt = new DeconTools.Backend.Core.LcmsFeatureTarget();
                mt.MonoIsotopicMass = mass;
                mt.MZ = selectedMassForFragmentation;
                mt.ChargeState = (short) charge;
                mt.EmpiricalFormula = empericalForumla; //"C66H114O50N3";
                mt.ElutionTimeUnit = Globals.ElutionTimeUnit.ScanNum;

                m_run.CurrentMassTag = mt;

                theorFeatureGen.Execute(m_run.ResultCollection);

                mt = m_run.CurrentMassTag;

                chargeObjectPile[i].TheoreticalModel = mt;
            }

            #endregion

            
            //6.  compare theoreticals to experimental and score 
            #region

            for (int i = 0; i < chargeObjectPile.Count; i++)
            {
                ChargeObject currentChargeObject = chargeObjectPile[i];
                
                m_run.CurrentMassTag = currentChargeObject.TheoreticalModel;
                //theorFeatureGen.Execute(m_run.ResultCollection);

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("------------------- MassTag = " + m_run.CurrentMassTag.ID + "---------------------------");
                Console.WriteLine("monoMass = " + m_run.CurrentMassTag.MonoIsotopicMass.ToString("0.0000") + "; monoMZ = " + m_run.CurrentMassTag.MZ.ToString("0.0000") + "; ChargeState = " + m_run.CurrentMassTag.ChargeState + "; NET = " + m_run.CurrentMassTag.NormalizedElutionTime.ToString("0.000") + "; Sequence = " + m_run.CurrentMassTag.Code + "; EmpiricalFormula= " + m_run.CurrentMassTag.EmpiricalFormula + "\n");

                //TODO load up peak list
                m_run.ResultCollection.Run.PeakList = new List<Peak>();

                foreach (ProcessedPeak pPeak in centroidedPeaks)
                {
                    MSPeak newPeak = new MSPeak();
                    newPeak.XValue = pPeak.XValue;
                    newPeak.Width = pPeak.Width;
                    newPeak.Height = Convert.ToSingle(pPeak.Height);
                    m_run.ResultCollection.Run.PeakList.Add(newPeak);
                }

                TargetedResultBase resultStorage = new LcmsFeatureTargetedResult();
                resultStorage.ResetResult();
                resultStorage.ScanSet = m_run.ScanSetCollection.ScanSetList[0];
                m_run.ResultCollection.CurrentTargetedResult = resultStorage;

                targetedFeatureFinder.Execute(m_run.ResultCollection);

                DeconTools.Backend.Core.IsotopicProfile experimentalResults = m_run.ResultCollection.CurrentTargetedResult.IsotopicProfile;

                //record number of peaks detected with targeted approach and the first peak in the distribution
                int peakCount = 0;
                if (experimentalResults != null)
                {
                    peakCount = experimentalResults.Peaklist.Count;
                    currentChargeObject.PeakCount = peakCount;

                    double lowestMass = experimentalResults.Peaklist[0].XValue;
                    currentChargeObject.LowestMassToCharge = lowestMass;
                }
                else
                {
                    currentChargeObject.PeakCount = 0;
                    currentChargeObject.LowestMassToCharge = 0;
                }

                //score two isotopeProfiles
                double scottScore = LeastSquareesOnPeaks(m_run.ResultCollection, peakCount);

                if (scottScore < 1)
                {
                    Console.WriteLine("Add " + scottScore + " to pile at charge " + i);
                    int index = currentChargeObject.Charge-1;//-1 because these are charges
                    //pileofValidCharges[index] = true;
                }

                currentChargeObject.Score = scottScore;

                bool deconScoring = false;
                if (deconScoring)
                {
                    fitScoreCalc.Execute(m_run.ResultCollection);

                    //pileOfScores.Add(m_run.ResultCollection.CurrentTargetedResult.Score);
                }
            }

            #endregion

            int deconCharge = tandemSpectra.PrecursorChargeState;//from decon
          
            double lowestScore = chargeObjectPile[0].Score;
            double lowMass = chargeObjectPile[0].LowestMassToCharge;

            List<ChargeObject>lowestPerChargeList = new List<ChargeObject>();

            //find lowest score for each charge state.  this will hone in the allighment
            for (int i = 0; i < maxCharge; i++)
            {
                ChargeObject lowestChargeObject = new ChargeObject();
                
                lowestScore = chargeObjectPile[i * maxCharge].Score;
                for (int daltonError = 1; daltonError < expandRangedaError + 2; daltonError++)//+2 for errors and including non moved mono
                {
                    int index = i*maxCharge + daltonError-1;

                    if (chargeObjectPile[index].Score < 1) //we passed the targeted//-1 to corrrect for da eeror
                    {
                        if (chargeObjectPile[index].Score <= lowestScore)//= so we include origional charge estimate
                        {
                            lowestScore = chargeObjectPile[index].Score;
                            lowestChargeObject = chargeObjectPile[index];
                        }
                    }
                }

                if (lowestChargeObject.TheoreticalModel != null)
                {
                    lowestPerChargeList.Add(lowestChargeObject);
                }
            }

            lowestPerChargeList = lowestPerChargeList.OrderByDescending(p => p.PeakCount).ToList();

            ChargeObject finalChargeObject = new ChargeObject();

            if (lowestPerChargeList.Count > 0)
            {
                if (lowestPerChargeList.Count > 1)
                {
                    if (lowestPerChargeList[0] != null && lowestPerChargeList[1] != null)
                    {
                        if (lowestPerChargeList[0].PeakCount == lowestPerChargeList[1].PeakCount)
                        {
                            //TAKE LARGER CHARGE ON TIES WHERE PEAK COUNT IS THE SAME
                            finalChargeObject = lowestPerChargeList[0];
                            if (lowestPerChargeList[1].Charge > lowestPerChargeList[0].Charge)
                            {
                                finalChargeObject = lowestPerChargeList[1];
                            }
                        }
                        else
                        {
                            //NO TIE, JUST TAKE FIRST ONE WITH HIGHEST PEAK COUNT
                            if (lowestPerChargeList[0] != null)
                            {
                                finalChargeObject = lowestPerChargeList[0];
                            }
                        }
                    }
                }
                else
                {
                    //NO TIE, JUST TAKE FIRST ONE WITH HIGHEST PEAK COUNT
                    if (lowestPerChargeList[0] != null)
                    {
                        finalChargeObject = lowestPerChargeList[0];
                    }
                    else
                    {
                        finalChargeObject = new ChargeObject();
                    }
                }
            }

            int finalCharge = finalChargeObject.Charge;
            double finalMono = finalChargeObject.MonoisotopicMass;

            //store charge state
            tandemSpectra.PrecursorChargeState = finalCharge;
            if (tandemSpectra.PrecursorPeak != null)
            {
                tandemSpectra.PrecursorPeak.Charge = finalCharge;
                tandemSpectra.PrecursorPeak.Background = Convert.ToSingle(finalMono);
            }

            Console.WriteLine("The final Charge via targeted is " + finalCharge + " and the old one is " + deconCharge);
            Console.WriteLine("The scan is " + precursorSpectra.Scan + " and mass is " + tandemSpectra.PrecursorMZ);
            Console.WriteLine("The mono has been corrected to " + ConvertMonoToMz.Execute(finalMono, finalCharge, massProton) + " from " + tandemSpectra.PrecursorPeak.XValue);
            if (lowestScore < 1)
            {
                Console.WriteLine("yea");
            }
            //TODO update mono peak with shifted mass.  go back to peak list and grab correct mz and height
            //precursorSpectra.ParentSpectra.Peaks;

            ProcessedPeak bestHit;
            List<ProcessedPeak> anotherList = GoGetPeakFromPeakList(precursorSpectra, tandemSpectra.PrecursorMZ, out bestHit, massProton);

            if (finalCharge == 0)
            {
                if (deconCharge > 0)
                {
                    finalCharge = deconCharge;
                }
                else
                {
                    finalCharge = 1;//defaut charge
                }
            }

            bestHit.Charge = finalCharge;
            tandemSpectra.PrecursorPeak = bestHit;
            //organize scores
            //select best charge state:  multiply hit mass by 2 and look for double charged..multiply hit by 3 and look for +3 etc
        }

        private double LeastSquareesOnPeaks(ResultCollection resultList, int peakCount)
        {

            double score = 1;

            DeconTools.Backend.Core.IsotopicProfile theoreticalProfile = resultList.Run.CurrentMassTag.IsotopicProfile;
            DeconTools.Backend.Core.IsotopicProfile experimentalProfile = resultList.CurrentTargetedResult.IsotopicProfile;
            //IsotopeProfile experimantalProfile = m_run.ResultCollection

            if (theoreticalProfile == null || theoreticalProfile.Peaklist == null || theoreticalProfile.Peaklist.Count == 0)
            {
                score = 1;   // this is the worst possible fit score. ( 0.000 is the best possible fit score);  Maybe we want to return a '-1' to indicate a failure...              
                return score;
            }

            if (experimentalProfile == null || experimentalProfile.Peaklist == null || experimentalProfile.Peaklist.Count == 0)
            {
                score = 1;   // this is the worst possible fit score. ( 0.000 is the best possible fit score);  Maybe we want to return a '-1' to indicate a failure...              
                return score;
            }

            //we have peak lists now
            Console.WriteLine("Experimental Data " + experimentalProfile.Peaklist.Count + "Theory Data " + theoreticalProfile.Peaklist.Count);

            //not convert to XYData
            double maxTheoryIntensity = getMostIntensePeak(theoreticalProfile.Peaklist).Height;
            double maxExperimentalIntensity = getMostIntensePeak(experimentalProfile.Peaklist).Height;

            List<PNNLOmics.Data.XYData> theoryXYData = new List<PNNLOmics.Data.XYData>();
            List<PNNLOmics.Data.XYData> experimentalXYData = new List<PNNLOmics.Data.XYData>();

            foreach (MSPeak peak in theoreticalProfile.Peaklist)
            {
                PNNLOmics.Data.XYData point = new PNNLOmics.Data.XYData(peak.XValue, peak.Height / maxTheoryIntensity);
                theoryXYData.Add(point);
                Console.WriteLine("Theory " + point.Y);
            }

            foreach (MSPeak peak in experimentalProfile.Peaklist)
            {
                PNNLOmics.Data.XYData point = new PNNLOmics.Data.XYData(peak.XValue, peak.Height / maxExperimentalIntensity);
                experimentalXYData.Add(point);
                Console.WriteLine("Experimental " + point.Y);
            }

            //find shortest list
            int count = experimentalProfile.Peaklist.Count;
            if (theoreticalProfile.Peaklist.Count < count)
            {
                count = theoreticalProfile.Peaklist.Count;
            }

            double sum = 0;
            for(int i=0;i<count;i++)
            {
                sum+= Math.Abs(theoryXYData[i].Y - experimentalXYData[i].Y);
            }
            sum = sum / count;//weighted least squares.  since count is like a maximum score with a difference of 1 at each point

            score = sum;

            return score;
        }

        public static MSPeak getMostIntensePeak(List<MSPeak> _peaklist)
        {
            if (_peaklist == null || _peaklist.Count == 0) return null;

            MSPeak maxPeak = new MSPeak();
            foreach (MSPeak peak in _peaklist)
            {
                if (peak.Height >= maxPeak.Height)
                {
                    maxPeak = peak;
                }

            }
            return maxPeak;

        }

        public static int OmcisChargestateCalculator(List<ProcessedPeak> localDataWithin2Da, int chargeStateMax, ProcessedPeak peakOfInterest, ref int[] chargeCount)
        {
            double massDifference = 1.00321;
            double massDifferenceTollerence = 0.05;
            double massDifferenceLow = massDifference - massDifferenceTollerence;
            double massDifferenceHigh = massDifference + massDifferenceTollerence;
            List<double> viewPeaks = new List<double>();

            //int chargeStateMax = 7;
            //int[] chargeCount = new int[chargeStateMax]; ;

            
            
            
            for (int i = 0; i < chargeStateMax; i++)
            {
                chargeCount[i] = 0;
            }

            foreach (ProcessedPeak peak in localDataWithin2Da)
            {
                double difference = peak.XValue - peakOfInterest.XValue;
                viewPeaks.Add(difference);
                for (int i = 1; i < chargeStateMax + 1; i++) //+1 so we start with +1 charged ions
                {
                    if (difference > massDifferenceLow/i && difference < massDifferenceHigh/i || difference > massDifferenceLow*2/i && difference < massDifferenceHigh*2/i)
                    {
                        chargeCount[i - 1]++;
                        //Console.WriteLine("charge " + i + " is possible");
                    }
                }
            }

            int chargeStateExperiment = chargeCount[0];
            int chargestate = 1;
            for (int i = 0; i < chargeStateMax; i++)
            {
                int testMax = chargeCount[i];

                if (testMax > chargeStateExperiment)
                {
                    chargestate = i; //offset of 1
                    //Console.WriteLine("the charge is now " + chargestate);
                }
                else
                {
                    //Console.WriteLine("the charge is still " + chargestate);
                }
            }

            foreach (double peak in viewPeaks)
            {
                double miniMass = Math.Round(peak, 3);
                Console.WriteLine("     Possible differences " + miniMass.ToString());
            }
            return chargestate;
        }

        /// <summary>
        /// deisotope date.  XYData and PeakList required
        /// </summary>
        /// <param name="spectra"></param>
        /// <param name="parameters"></param>
        //public void DeisotopePeaks(MSSpectra spectra, int scanNumber, ParalellController deisotopingController)
        public void DeisotopePeaks(MSSpectra spectra, int scanNumber, EngineThrashDeconvolutor currentEngine, out List<IsotopeObject> resultsOut)
        {
            Console.WriteLine(Environment.NewLine + " DEISOTOPE Peaks in scan " + spectra.Scan);
            
            bool isThereAScanNumber = CheckScanNumber(spectra);
            bool hasTheXyDataBeenLoaded = CheckXYDataExists(spectra);
            bool areTherePeaks = CheckProcessedPeaksExists(spectra);

            if (!hasTheXyDataBeenLoaded)//if not present, go get them
            {
                XYDataSpectraGenerator(spectra);
                hasTheXyDataBeenLoaded = true;
            }

            //if(!areTherePeaks)//if there are no peaks, go get them
            //{
            //    ParametersTHRASH currentParameters = (ParametersTHRASH) deisotopingController.ParameterStorage1;
            //    ParametersPeakDetection currentPeakParameters = currentParameters.PeakDetectionMultiParameters;
            //    PeakGenerator(spectra, currentPeakParameters);
            //    areTherePeaks = true;
            //}

            if (!areTherePeaks)//if there are no peaks, go get them
            {
                Console.WriteLine("WeNeed Peaks to work");
                Console.ReadKey();
            }

            resultsOut = new List<IsotopeObject>();

            if (isThereAScanNumber && hasTheXyDataBeenLoaded && areTherePeaks)//If everything is here...
            {
                
                spectra.PeaksProcessed = DeisotopeOmicsPeaks(spectra.Peaks, spectra.PeaksProcessed, m_run, scanNumber, currentEngine, out resultsOut);

                int scan = spectra.Scan;
                foreach (ProcessedPeak peak in spectra.PeaksProcessed)
                {
                    peak.ScanNumber = scan;
                }
                spectra.PeakProcessingLevel = PeakProcessingLevel.Deisotoped;

                
            }
        }

        /// <summary>
        /// creates a run on a tandem object
        /// </summary>
        public void LoadRun()
        {
            if (InputFileName != null)
            {
                if (m_run == null)
                {
                    m_run = GoCreateRun.CreateRun(InputFileName);
                }
                else
                {
                    Console.WriteLine("run alreadt exists");
                }
            }
            else
            {
                Console.WriteLine("FileName is Missing to create run");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// clears a run on a tandem object
        /// </summary>
        public void DisposeRun()
        {
            m_run.Dispose();
            m_run = null;
        }

        #region private functions

        private static bool CheckScanNumber(MSSpectra spectra)
        {
            bool check = false;
            if (spectra.Scan < 0)///check to see we have a scan number to work on
            {
                Console.WriteLine("we need a Child Spectra and a precursor scan number before we can populate the XYdata");
                Console.ReadKey();
                check = false;
            }
            check = true;
            return check;
        }

        private static bool CheckSpectra(MSSpectra spectra)
        {
            bool check = false;
            if (spectra == null)///check to see we have a scan number to work on
            {
                Console.WriteLine("we need a Child Spectra and a precursor scan number before we can populate the XYdata");
                Console.ReadKey();
                check = false;
            }
            check = true;
            return check;
        }

        private static bool CheckXYDataExists(MSSpectra spectra)
        {
            bool check = false;
            if (spectra.Peaks.Count == 0)///check to see we have a scan number to work on
            {
                Console.WriteLine("we need a Child Spectra and a precursor scan number before we can populate the XYdata");
                Console.ReadKey();
                check = false;
            }
            check = true;
            return check;
        }

        private static bool CheckProcessedPeaksExists(MSSpectra spectra)
        {
            bool check = false;
            if (spectra.PeaksProcessed.Count == 0)///check to see we have a scan number to work on
            {
                Console.WriteLine("we need a Child Spectra and a precursor scan number before we can populate the XYdata");
                //Console.ReadKey();
                check = false;
            }
            check = true;
            return check;
        }

        private static bool CheckPrecursorMassExistsInFile(PrecursorInfo precursor)
        {
            bool check = false;
            if (precursor.PrecursorMZ <= 0)///check to see we have a mass from the file
            {
                Console.WriteLine("we need a precursor mass before we can refine it");
                Console.ReadKey();
                check = false;
            }
            check = true;
            return check;
        }

        private static bool CheckPrecursorMassisPopulated(double mass)
        {
            bool check = false;
            if (mass <= 0)///check to see we have a mass from the file
            {
                Console.WriteLine("we need a precursor mass before we can refine it");
                Console.ReadKey();
                check = false;
            }
            check = true;
            return check;
        }

        /// <summary>
        /// combines peak detection and thresholding
        /// </summary>
        /// <param name="currentData"></param>
        /// <returns></returns>
        private static List<ProcessedPeak> PeakFindAndThreshold(List<PNNLOmics.Data.XYData> currentData, Run runGo, ParametersPeakDetection peakParameters)
        {
            ////2.  centroid peaks
            //PeakCentroider newPeakCentroider = new PeakCentroider();
            //newPeakCentroider.Parameters.FWHMPeakFitType = PeakFitType.Parabola;

            //List<ProcessedPeak> discoveredPeakList = new List<ProcessedPeak>();
            //discoveredPeakList = newPeakCentroider.DiscoverPeaks(currentData);

            ////3.  threshold
            //PeakThresholder newPeakThresholder = new PeakThresholder();
            //newPeakThresholder.Parameters.SignalToShoulderCuttoff = (float)parameters.Part1Parameters.MSPeakDetectorPeakBR;

            //discoveredPeakList = newPeakThresholder.ApplyThreshold(discoveredPeakList);
            //return discoveredPeakList;

            List<ProcessedPeak> discoveredPeakList = TandemObject.PeakFindAndThreshold(currentData, runGo, peakParameters);
            return discoveredPeakList;
        }


        /// <summary>
        /// combines peak detection and thresholding
        /// </summary>
        /// <param name="currentData"></param>
        /// <returns></returns>
        private static List<ProcessedPeak> FindAllPeaks(List<PNNLOmics.Data.XYData> currentData, Run runGo, ParametersPeakDetection peakParameters)
        {
            ////2.  centroid peaks
            //PeakCentroider newPeakCentroider = new PeakCentroider();
            //newPeakCentroider.Parameters.FWHMPeakFitType = PeakFitType.Parabola;

            //List<ProcessedPeak> discoveredPeakList = new List<ProcessedPeak>();
            //discoveredPeakList = newPeakCentroider.DiscoverPeaks(currentData);

            ////3.  threshold
            //PeakThresholder newPeakThresholder = new PeakThresholder();
            //newPeakThresholder.Parameters.SignalToShoulderCuttoff = (float)parameters.Part1Parameters.MSPeakDetectorPeakBR;

            //discoveredPeakList = newPeakThresholder.ApplyThreshold(discoveredPeakList);
            //return discoveredPeakList;

            List<ProcessedPeak> discoveredPeakList = TandemObject.PeakCentroidOnly(currentData, runGo, peakParameters);
            return discoveredPeakList;
        }

        /// <summary>
        /// deisotope data. 
        /// </summary>
        /// <param name="currentXyData"></param>
        /// <param name="currentPeaks"></param>
        /// <param name="runGo"></param>
        /// <param name="scanNumber"></param>
        /// <param name="deisotopingController">contains the Thrash parameters within the engines</param>
        /// <returns></returns>
        //private static List<ProcessedPeak> DeisotopeOmicsPeaks(List<XYData> currentXyData, List<ProcessedPeak> currentPeaks, Run runGo, int scanNumber, ParalellController deisotopingController)
        private static List<ProcessedPeak> DeisotopeOmicsPeaks(List<PNNLOmics.Data.XYData> currentXyData, List<ProcessedPeak> currentPeaks, Run runGo, int scanNumber, EngineThrashDeconvolutor engine, out List<IsotopeObject> resultsOut )
        {
            TandemObject placeHolderObject = new TandemObject();
            placeHolderObject.m_run = runGo;

            ConvertOmicsProcessedPeaksToDeconPeaks peakConverter = new ConvertOmicsProcessedPeaksToDeconPeaks();
            runGo.PeakList = peakConverter.ConvertPeaksForRun(currentPeaks);

            ConvertXYData.OmicsXYDataToRunXYDataRun(ref runGo, currentXyData);

            #region pulled out side due to memory problems
            //ParalellController newController = new ParalellController(ThrashParameters);
            //EngineThrashDeconvolutor thrashEngineDeconvolutorBuilder = new EngineThrashDeconvolutor();
            //newController.EngineStation = thrashEngineDeconvolutorBuilder.SetupEngines(ThrashParameters);
            #endregion

            ResultsTHRASH results = new ResultsTHRASH(scanNumber);
            //EngineThrashDeconvolutor engine = (EngineThrashDeconvolutor)deisotopingController.EngineStation.Engines[0];
            engine.Run = runGo;

            engine.Run.DeconToolsPeakList = new clsPeak[runGo.PeakList.Count];

            for(int i=0;i<runGo.PeakList.Count;i++)
            {
                var tempPeak = runGo.PeakList[i];
                engine.Run.DeconToolsPeakList[i] = new clsPeak();
                engine.Run.DeconToolsPeakList[i].mdbl_FWHM = tempPeak.Width;
                engine.Run.DeconToolsPeakList[i].mdbl_SN = 20;
                engine.Run.DeconToolsPeakList[i].mdbl_intensity = tempPeak.Height;
                engine.Run.DeconToolsPeakList[i].mdbl_mz = tempPeak.XValue;
            }
            
            GoThrashEngine.DeconvoluteWithEngine(scanNumber, results, runGo, engine);


            List<ProcessedPeak> peakResults  = new List<ProcessedPeak>();
            foreach (IsotopeObject peak in results.ResultsFromRunConverted)
            {
                ProcessedPeak peakSimple = new ProcessedPeak();
                peakSimple.XValue = peak.MonoIsotopicMass;
                peakSimple.Height = peak.IsotopeList[0].Height;
                peakSimple.Charge = peak.Charge;
                peakSimple.Width = peak.IsotopeList[0].Width;
                peakSimple.Background = peak.IsotopeList[0].Background;
                peakSimple.LocalSignalToNoise = peak.IsotopeList[0].LocalSignalToNoise;
                peakResults.Add(peakSimple);

            }

            resultsOut = results.ResultsFromRunConverted;
            //PrecursorMonoIsotopicPeaks = newController.MonoIsotopicMasses;
            //PrecursorScanChargeStates = newController.ChargeStates;

            //}
            //});

            return peakResults;


            //List<ProcessedPeak> discoveredPeakList = new List<ProcessedPeak>();
            //foreach (IsotopeObject isos in objectInsideThread.ResultsFromRunConverted)
            //{
            //    ProcessedPeak monoPeak = new ProcessedPeak();
            //    monoPeak.XValue = isos.MonoIsotopicMass;
            //    monoPeak.Height = isos.IsotopeList[0].Height;
            //    monoPeak.ScanNumber = scanNumber;
                
            //    discoveredPeakList.Add(monoPeak);
            //}
            
            //return discoveredPeakList;
        }

        /// <summary>
        /// Uses DeconTools to load data
        /// </summary>
        /// <param name="scanNumber"></param>
        /// <returns></returns>
        private List<PNNLOmics.Data.XYData> GetData(int scanNumber)
        {
            m_run.MinLCScan = scanNumber;
            m_run.MaxLCScan = scanNumber;

            try
            {
                bool GetMSMSDataAlso = true;
                int scansToSum = 1;

                //Console.WriteLine("LoadingData...");

                m_run.ScanSetCollection.Create(m_run, m_run.MinLCScan, m_run.MaxLCScan, scansToSum, 1, GetMSMSDataAlso);
                //m_run.ScanSetCollection = ScanSetCollection.Create(m_run, m_run.MinScan, m_run.MaxScan, scansToSum, 1, GetMSMSDataAlso);

                m_run.CurrentScanSet = m_run.ScanSetCollection.ScanSetList[0];

                GoSpectraGenerator.GenerateMS(m_run);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            List<PNNLOmics.Data.XYData> newList;
            ConvertXYData.RunXYDataToOmicsXYData(m_run, out newList);
            m_run.ResultCollection.Run.XYData.Xvalues = null;
            m_run.ResultCollection.Run.XYData.Yvalues = null;
            m_run.ScanSetCollection.ScanSetList = null;
            return newList;
        }

        //clear out anotherlist
        private static void AnotherListDispose(ref List<ProcessedPeak> anotherList)
        {
            for (int p = 0; p < anotherList.Count; p++)
            {
                anotherList[p] = null;
            }
            anotherList = null;
        }

        #endregion
        
    }
}
