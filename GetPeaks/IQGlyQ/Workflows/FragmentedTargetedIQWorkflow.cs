using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend;
using DeconTools.Backend.ProcessingTasks.Smoothers;
using DeconTools.Utilities;
using DeconTools.Workflows.Backend.Core;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks.TheorFeatureGenerator;
using DeconTools.Backend.ProcessingTasks.TargetedFeatureFinders;
using DeconTools.Backend.ProcessingTasks.ChromatogramProcessing;
using DeconTools.Backend.ProcessingTasks.MSGenerators;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Workflows.Backend.Core.ChromPeakSelection;
using GetPeaks_DLL.Functions;
using IQGlyQ.Exporter;
using IQGlyQ.Functions;
using IQGlyQ.Objects;
using IQGlyQ.Processors;
using IQGlyQ.Results;
using IQGlyQ.TargetGenerators;
using IQGlyQ.Tasks;
using PNNLOmics.Algorithms.PeakDetection;
using GetPeaks_DLL.DataFIFO;
using PNNLOmics.Data;
using Peak = DeconTools.Backend.Core.Peak;
using XYData = PNNLOmics.Data.XYData;

namespace IQGlyQ
{
    public class FragmentedTargetedIQWorkflow : IqWorkflow

    {
        private DeuteratedQuantifierTask _quant;

        //private RemoveInsourceFragmentation _removeInsource;
        //private RemoveInsourceFragmentationV2 _removeInsource;
        private RemoveInsourceFragmentationIQ _removeInsource;

        private ConvertResultToChildren _convertResults;

        private SmartChromPeakSelector _chromPeakSelectorDeuterated;

        //private MSGenerator _msGenerator;

        private Processors.ProcessorChromatogram _LCProcessor;

        private Processors.ProcessorMassSpectra _MSProcessor;

        public override TargetedWorkflowParameters WorkflowParameters { get; set; }

        public List<TargetedResultBase> PileOfResults { get; set; }

        #region Constructors

        public FragmentedTargetedIQWorkflow(Run run, FragmentedTargetedWorkflowParametersIQ parameters)
            : base(run, parameters)
        {
            parameters.MSParameters.MsGeneratorParameters = new MSGeneratorParameters(run.MSFileType);

            parameters.MSParameters.MsGeneratorParameters.MsFileType = run.MSFileType;

            

            //_msGenerator = MSGeneratorFactory.CreateMSGenerator(run.MSFileType);
            PileOfResults = new List<TargetedResultBase>();
            //WorkflowParameters = null;
            InitializeWorkflow();

        }

        public FragmentedTargetedIQWorkflow(FragmentedTargetedWorkflowParametersIQ parameters)
            : base(parameters)
        {
            //Fragments = parameters.Fragments;
            PileOfResults = new List<TargetedResultBase>();
            //WorkflowParameters = null;
            InitializeWorkflow();
        }

        //public override TargetedWorkflowParameters WorkflowParameters
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        #endregion

   
        #region IWorkflow Members


        //protected override void ExecutePostWorkflowHook()
        //{
        //    base.ExecutePostWorkflowHook();
        //    ExecuteTask(_quant);
        //}

        #endregion

        public override void InitializeWorkflow()
        {
            Check.Require(this.Run != null, "Run is null");

            base.DoPreInitialization();

            base.DoMainInitialization();

            base.DoPostInitialization();
            
            base.DoPostInitialization();

            FragmentedTargetedWorkflowParametersIQ tempParameters = (FragmentedTargetedWorkflowParametersIQ)this.WorkflowParameters;

            //1.  LC
            //start smart section (old)
            SmartChromPeakSelectorParameters smartchrompeakSelectorParams = new SmartChromPeakSelectorParameters();
            smartchrompeakSelectorParams.MSFeatureFinderType = DeconTools.Backend.Globals.TargetedFeatureFinderType.ITERATIVE;
            smartchrompeakSelectorParams.MSPeakDetectorPeakBR = this.WorkflowParameters.MSPeakDetectorPeakBR;
            smartchrompeakSelectorParams.MSPeakDetectorSigNoiseThresh = this.WorkflowParameters.MSPeakDetectorSigNoise;
            smartchrompeakSelectorParams.MSToleranceInPPM = this.WorkflowParameters.MSToleranceInPPM;
            smartchrompeakSelectorParams.NETTolerance = (float)this.WorkflowParameters.ChromNETTolerance;
            smartchrompeakSelectorParams.NumScansToSum = this.WorkflowParameters.NumMSScansToSum;
            smartchrompeakSelectorParams.NumChromPeaksAllowed = 20;//changed from 10 to 20 because it will fail if ther are more than this number
            smartchrompeakSelectorParams.IterativeTffMinRelIntensityForPeakInclusion = 0.5;
            smartchrompeakSelectorParams.MultipleHighQualityMatchesAreAllowed = this.WorkflowParameters.MultipleHighQualityMatchesAreAllowed;

            //deuterium (old)
            _chromPeakSelectorDeuterated = new SmartChromPeakSelector(smartchrompeakSelectorParams);
            //_chromPeakSelectorDeuterated.TargetedMSFeatureFinder = _msfeatureFinder;
            _chromPeakSelectorDeuterated.IsotopicProfileType = DeconTools.Backend.Globals.IsotopicProfileType.UNLABELLED;

            _LCProcessor = new ProcessorChromatogram(tempParameters.LCParameters);

            //2.  Mass Spec
            IterativeTFFParameters _iterativeTFFParameters = new IterativeTFFParameters();
            _iterativeTFFParameters.ToleranceInPPM = this.WorkflowParameters.MSToleranceInPPM;
            _iterativeTFFParameters.IsotopicProfileType = DeconTools.Backend.Globals.IsotopicProfileType.UNLABELLED;
            _iterativeTFFParameters.RequiresMonoIsotopicPeak = true;

            MsfeatureFinder = new IterativeTFF(_iterativeTFFParameters);
            //_msfeatureFinder = new O16O18TargetedIterativeFeatureFinder(_iterativeTFFParameters);

            tempParameters.MSParameters.Engine_msGenerator = MSGeneratorFactory.CreateMSGenerator(tempParameters.MSParameters.MsGeneratorParameters.MsFileType);
            //_msGenerator = MSGeneratorFactory.CreateMSGenerator(this.Run.MSFileType);
            _MSProcessor = new ProcessorMassSpectra(tempParameters.MSParameters);
            
            //3.  Isotope
            TheorFeatureGen = new JoshTheorFeatureGenerator(tempParameters.IsotopeProfileType, tempParameters.IsotopeLabelingEfficiency, tempParameters.IsotopeLowPeakCuttoff, tempParameters.MolarMixingFractionOfH);

            //TODO targeted result base AddLabelledIso is not set up yet

            //4.  deuterium
            double minValue = 1;
            //_quant = new DeuteratedQuantifierTask(minValue, WorkflowParametersV2.IsotopeLabelingEfficiency);
            _quant = new DeuteratedQuantifierTask(minValue, tempParameters.IsotopeLabelingEfficiency);

            
            //5.  remove insource
            //_removeInsource = new RemoveInsourceFragmentationIQ(tempParameters, tempParameters.MSParameters.Engine_msGenerator, this.Run, _LCProcessor, _MSProcessor);
            _removeInsource = new RemoveInsourceFragmentationIQ(tempParameters, this.Run, _LCProcessor, _MSProcessor);
   

            _convertResults = new ConvertResultToChildren();

            //default
            IsWorkflowInitialized = true;
        }

        public override void Execute(IqResult result)
        {
            
            Console.WriteLine("Start Workflows... (FragmentedTargetedIQWorkflow) on " + result.Target.Code);
            
            FragmentedTargetedWorkflowParametersIQ tempParameters = (FragmentedTargetedWorkflowParametersIQ)this.WorkflowParameters;

            if (result.Target.ChargeState >= tempParameters.ChargeStateMin &&  result.Target.ChargeState <= tempParameters.ChargeStateMax)
            {

                //Console.ReadKey();
                
                Utiliites.TherortIsotopeicProfileWrapper(ref TheorFeatureGen, result.Target, tempParameters.MSParameters.IsoParameters.IsotopeProfileMode, tempParameters.MSParameters.IsoParameters.DeltaMassCalibrationMZ, tempParameters.MSParameters.IsoParameters.DeltaMassCalibrationMono, tempParameters.MSParameters.IsoParameters.ToMassCalibrate, tempParameters.MSParameters.IsoParameters.PenaltyMode);

                //result.IqResultDetail.Chromatogram = ChromGen.GenerateChromatogram(Run, result.Target.TheorIsotopicProfile, result.Target.ElutionTimeTheor);
                //result.IqResultDetail.Chromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(Run, result.Target.TheorIsotopicProfile, result.Target.ElutionTimeTheor);//old that does not have smart ppm tolerences

                //step 1:  this is a first pass at generating a chromatogram with input tolerance
                double massToExtract = result.Target.TheorIsotopicProfile.getMostIntensePeak().XValue;
                //double chromTollerencePPM = tempParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM;//this is the parameter default ppm
                tempParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM = tempParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPMInitial;
                _LCProcessor = new ProcessorChromatogram(tempParameters.LCParameters);//input parameter base lcProcessor.  initialized
                //tempParameters.LCParameters.Engine_PeakChromGenerator = new PeakChromatogramGenerator(chromTollerencePPM);

                //result.IqResultDetail.Chromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(Run, massToExtract, chromTollerencePPM);//full range general EIC generation
                result.IqResultDetail.Chromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(Run, massToExtract);//Latest, full range general EIC generation with default parameters

                //GetPeaks_DLL.DataFIFO.IXYDataWriter writer = new DataXYDataWriter();
                //writer.WriteDeconXYDataDeconTools(result.IqResultDetail.Chromatogram, @"E:\ScottK\ToPic\Results_Gly09_SN133_26Feb13_Cheetah_C14_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M1mbar\chrom");

                //step 2: Peak detect chromatogram
                _LCProcessor.id = result.Target.ID;
                _LCProcessor.ChargeState = result.Target.ChargeState;
                ScanObject scans = new ScanObject(Run.MinLCScan, Run.MaxLCScan);

                Console.WriteLine("Pre LC Processor... Press Key");
                //Console.ReadKey();

                result.ChromPeakList = _LCProcessor.Execute(result.IqResultDetail.Chromatogram, scans, tempParameters.LCParameters.ProcessLcChromatogram);

                Console.WriteLine("Post LCProcessor... LC Processor is working");
                ProcessingParametersChromatogram targetSpecificLCParameters;

                //setp 3 select most abundant peak to find peak scan number to learn from
                if (result.ChromPeakList.Count > 0)
                {
                    result.ChromPeakList = result.ChromPeakList.OrderByDescending(n => n.Height).ToList();
                    Peak tallestPeak = result.ChromPeakList.FirstOrDefault();
                    int approximateScanCentroidFromTallestPeak = Convert.ToInt32(tallestPeak.XValue);

                    //step 4:  select top peak so we can select a scan to learn from

                    ScanSet scanSelected = SelectClosest.SelectClosestScanSetToScan(Run, approximateScanCentroidFromTallestPeak);

                    Console.WriteLine("Pre MS Processor... Press Key");

                    DeconTools.Backend.XYData spectraAtMaxEICPeak = _MSProcessor.DeconMSGeneratorWrapper(Run, scanSelected);
                    List<ProcessedPeak> msPeaks = _MSProcessor.Execute(spectraAtMaxEICPeak, EnumerationMassSpectraProcessing.OmicsCentroid_Only);


                    //step 5:  select best peak in mass spectra by subtracting the theoretical from all peaks, sort and take the closest
                    Console.WriteLine("Post MS Processor... MS Processor is working");

                    double chromTollerencePPMCalculted;
                    if (msPeaks.Count > 0)
                    {
                        ProcessedPeak closestMsPeakToTheoretical = SelectClosest.ClosestMsPeakToTheoretical(massToExtract, msPeaks);

                        //Console.WriteLine("Post MS Processor... 1");

                        double calculatedWidth = closestMsPeakToTheoretical.Width;
                        double partialWidth = calculatedWidth / tempParameters.LCParameters.ParametersChromGenerator.AutoSelectEICAt_X_partOfPeakWidth;//2 will be half max
                        chromTollerencePPMCalculted = partialWidth / massToExtract * 1000000;
                        //this is a local PPM to extract
                    }
                    else
                    {
                        //this is the case where there are not peaks in the spectra.  therfore, don't change the chrom ppm.
                        chromTollerencePPMCalculted = tempParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM;
                    }

                    if(chromTollerencePPMCalculted>tempParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPMMax)
                    {
                        chromTollerencePPMCalculted = tempParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPMMax;//prevents running away with super wide xic
                    }
                    //Console.WriteLine("Post MS Processor...2");

                    //step 6.  this is updated as a LCprocessor to use throughout the current target
                    targetSpecificLCParameters = tempParameters.LCParameters;
                    targetSpecificLCParameters.ParametersChromGenerator.ChromToleranceInPPM = chromTollerencePPMCalculted;
                    _LCProcessor = new ProcessorChromatogram(targetSpecificLCParameters);//this LC processor should be used for the rest of the target

                    //Console.WriteLine("Post MS Processor... 3");

                    result.IqResultDetail.Chromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(Run, massToExtract);//full range general EIC generation

                    //Console.WriteLine("Post MS Processor...4");
                    //step 6.  redo peaks
                    result.ChromPeakList = _LCProcessor.Execute(result.IqResultDetail.Chromatogram, scans, tempParameters.LCParameters.ProcessLcChromatogram);

                }


                bool printEachTargetLC = false;
                Console.WriteLine(Environment.NewLine + "Just before LC write to disk... toggle set to " + printEachTargetLC);
                if (printEachTargetLC && result.Target.ChargeState == 1 || printEachTargetLC && result.Target.ChargeState == 2)
                {
                    PrintSmoothedEICForEachTarget(result);
                    PrintPeakListForEachTarget(result.ChromPeakList, result);
                }
                Console.WriteLine("Just after LC write to disk...");

               
                
                #region calculate elution times here

                //foreach (ChromPeak chromPeak in result.ChromPeakList)
                //{
                //    double time = Run.GetTime(Convert.ToInt32(chromPeak.XValue));
                //    string scanInfo = Run.GetScanInfo(Convert.ToInt32(chromPeak.XValue));//0 because we are only allowing one scan through
                //    chromPeak.NETValue = ParseThermoScanInfo.ExtractMass(scanInfo) + time;
                //}

                List<Peak> chromPeakList = result.ChromPeakList;
                ProcessorChromatogram.CalculateElutionTimes(Run, ref chromPeakList);

                #endregion

                //ChromPeakDetector.CalculateElutionTimes(Run, result.ChromPeakList);

                result.IqResultDetail.ChromPeakQualityData = ChromPeakAnalyzer.GetChromPeakQualityData(Run, result.Target, result.ChromPeakList);

                //bool filterOutFlagged = result.Target.TheorIsotopicProfile.GetIndexOfMostIntensePeak() == 0;

                Console.WriteLine("switch at charge " + result.Target.ChargeState);
                //ExecuteTask(_chromPeakSelectorDeuterated);//origional
                //ChromPeakSelected = Result.ChromPeakSelected;

                //this is where we have several candidates for the feature.  some are peaks and some are isomers.  
                //we need to go into each of these scans and see if there is a glycan larger.  if there is, remove it from the list and save as new target for export
                //if we export the new targets we don't have to deal with recursion

                //ExecuteTask(_removeInsource);

                switch (tempParameters.GenerationDirection)
                {
                    case EnumerationParentOrChild.ChildrenOnly:
                        {
                            _removeInsource.CleanData(Run, result, EnumerationParentOrChild.ChildrenOnly);
                        }
                        break;
                    case EnumerationParentOrChild.ParentsOnly:
                        {
                            _removeInsource.CleanData(Run, result, EnumerationParentOrChild.ParentsOnly);
                        }
                        break;
                    case EnumerationParentOrChild.ParentsAndChildren://2 of 3
                        {
                            //if (massToExtract > 860 && massToExtract < 870)//charge 2
                            //{
                            Console.WriteLine("Clean up data...");
                            _removeInsource.CleanData(Run, result, EnumerationParentOrChild.ChildrenOnly);//this looks for smaller fragments to validate
                            _removeInsource.CleanData(Run, result, EnumerationParentOrChild.ParentsOnly);//this looks for larger ions as future targets
                            Console.WriteLine("Done Cleaning up data...");
                            //}
                        }
                        break;
                }

                Console.WriteLine("convert result");

                _convertResults.Convert(result);

                Console.WriteLine("post convert result");
                //this is where we set up the standard iq results

                IqGlyQResult glyResult = (IqGlyQResult)result;
                List<IqResult> children = glyResult.ChildResults().ToList();

                foreach (IqResult child in children)
                {
                     IqGlyQResult glyChild = (IqGlyQResult)child;
                     Console.WriteLine("The time is " + glyChild.ToChild.ElutionTime + " at scan " + glyChild.ToChild.Scan + " decide " + glyChild.ToChild.FinalDecision);
                }

                foreach (IqResult child in children)
                {
                    ////rest of IQ
                    if (result.IqResultDetail.ChromPeakQualityData != null && result.IqResultDetail.ChromPeakQualityData.Count > 0)
                    {
                        Console.WriteLine("PostProccessing info adding. Peaks:  " + result.IqResultDetail.ChromPeakQualityData.Count);
                        
                        child.ChromPeakSelected = result.IqResultDetail.ChromPeakQualityData[glyResult.ToChild.ChromPeakQualityIndex].Peak;

                        //result.ChromPeakSelected = ChromPeakSelector.SelectBestPeak(result.IqResultDetail.ChromPeakQualityData, filterOutFlagged);
                        IqGlyQResult glyChild = (IqGlyQResult)child;

                        Console.WriteLine("Scan " +child.ChromPeakSelected.XValue);
                        child.ChromPeakSelected.XValue = glyChild.ToChild.TargetFragment.ScanLCTarget;
                        child.LCScanSetSelected = ChromPeakUtilities.GetLCScanSetForChromPeak(child.ChromPeakSelected, Run, WorkflowParameters.NumMSScansToSum);//this works ok so far
                        //child.LCScanSetSelected = new ScanSet(child.LCScanSetSelected.PrimaryScanNumber);

                        child.IqResultDetail.MassSpectrum = _MSProcessor.DeconMSGeneratorWrapper(Run, child.LCScanSetSelected);
                        //child.IqResultDetail.MassSpectrum = MSGenerator.GenerateMS(Run, child.LCScanSetSelected);

                        TrimData(child.IqResultDetail.MassSpectrum, child.Target.MZTheor, MsLeftTrimAmount, MsRightTrimAmount);//this currently trims nothing

                        //List<Peak> mspeakList;
                        //child.ObservedIsotopicProfile = MsfeatureFinder.IterativelyFindMSFeature(child.IqResultDetail.MassSpectrum, child.Target.TheorIsotopicProfile, out mspeakList);
                        if (glyChild.ToChild.FragmentObservedIsotopeProfile != null)
                        {
                            child.ObservedIsotopicProfile = glyChild.ToChild.FragmentObservedIsotopeProfile;
                        }
                        else
                        {
                            child.ObservedIsotopicProfile = IterativelyFindMSFeatureWrapper.IterativeFeatureFind(child.Target.TheorIsotopicProfile, child.IqResultDetail.MassSpectrum, MsfeatureFinder);
                        }

                        //child.FitScore = FitScoreCalc.CalculateFitScore(child.Target.TheorIsotopicProfile, child.ObservedIsotopicProfile, child.IqResultDetail.MassSpectrum);

                        //child.InterferenceScore = InterferenceScorer.GetInterferenceScore(child.ObservedIsotopicProfile, mspeakList);

                        child.MonoMassObs = child.ObservedIsotopicProfile == null ? 0 : child.ObservedIsotopicProfile.MonoIsotopicMass;

                        child.MZObs = child.ObservedIsotopicProfile == null ? 0 : child.ObservedIsotopicProfile.MonoPeakMZ;

                        double elutionTime = child.ChromPeakSelected == null ? 0d : ((ChromPeak)child.ChromPeakSelected).NETValue;
                        elutionTime = glyChild.ToChild.ElutionTime;

                        child.ElutionTimeObs = elutionTime;

                        //glyResult.ToChild.FragmentObservedIsotopeProfile.Score = child.ObservedIsotopicProfile.Score;
                        child.FitScore = child.ObservedIsotopicProfile.Score;

                        if(child.ObservedIsotopicProfile.Peaklist==null)
                        {
                            child.ObservedIsotopicProfile = null;
                        }

                        child.Abundance = GetAbundance(child);


                        //_quant.Execute(child);


                       

                        
                        

                        
                    }
                    
                }

                 #region clean up results
                
                foreach (IqResult child in children)
                {
                    IqGlyQResult glyChild = (IqGlyQResult)child;
                    if (glyChild.IqResultDetail != null && glyChild.IqResultDetail.MassSpectrum != null)
                    {
                        glyChild.IqResultDetail.MassSpectrum.SetXYValues(new double[0], new float[0]);
                    }

                    if (glyChild.Target.TheorIsotopicProfile != null)
                    {
                        glyChild.Target.TheorIsotopicProfile.Peaklist = new List<MSPeak>();
                    }
                }

                 #endregion
            }
            else
            {
                Console.WriteLine("ChargeState is too high.  Charge is greater than " + tempParameters.ChargeStateMax);
            }



            Console.WriteLine("Done with Fragmented Targeted Workflow");
        }

        private void PrintSmoothedEICForEachTarget(IqResult result)
        {
            StringListToDisk writer = new StringListToDisk();
            List<string> dataToWrite = new List<string>();
            string path = @"F:\LC_" + result.Target.Code + "c" + result.Target.ChargeState + ".txt";
            List<XYData> omicsRaw = ConvertXYData.DeconXYDataToOmicsXYData(result.IqResultDetail.Chromatogram);
            List<XYData> chromForWriting = ProcessorChromatogram.SmoothWrapper(omicsRaw);

            dataToWrite.Add(Run.MinLCScan + "," + 0); //first point
            if (chromForWriting != null)
            {
                foreach (var Peak in chromForWriting)
                {
                    if (Peak.X != Run.MinLCScan || Peak.X != Run.MaxLCScan)
                        //dont add first or last point so we don't have duplicates
                    {
                        dataToWrite.Add(Peak.X + "," + Peak.Y);
                    }
                }
            }
            dataToWrite.Add(Run.MaxLCScan + "," + 0); //last point

            writer.toDiskStringList(path, dataToWrite);
        }

        private void PrintPeakListForEachTarget(List<Peak> chromPeakList, IqResult result)
        {
            StringListToDisk writer = new StringListToDisk();
            List<string> dataToWrite = new List<string>();
            string path = @"F:\LC_" + result.Target.Code + "c" + result.Target.ChargeState +"p" + ".txt";
            

            dataToWrite.Add(Run.MinLCScan + "," + 0); //first point
            foreach (var Peak in chromPeakList)
            {
                if (Peak.XValue != Run.MinLCScan || Peak.XValue != Run.MaxLCScan)
                //dont add first or last point so we don't have duplicates
                {
                    dataToWrite.Add(Peak.XValue + "," + Peak.Height);
                }
            }
            dataToWrite.Add(Run.MaxLCScan + "," + 0); //last point

            writer.toDiskStringList(path, dataToWrite);
        }


        //this is good
        protected override IqResult CreateIQResult(IqTarget target)
        {
            IqResult result = new IqGlyQResult(target);
            return result;
        }

        public override DeconTools.Workflows.Backend.FileIO.ResultExporter GetResultExporter()
        {
            throw new NotImplementedException();
        }

        //protected override DeconTools.Backend.Globals.ResultType GetResultType()
        //{
        //    return DeconTools.Backend.Globals.ResultType.DEUTERATED_TARGETED_RESULT;
        //}

       





        //this is good

        public override DeconTools.Workflows.Backend.FileIO.ResultExporter CreateExporter()
        {
            InSourceFragmentExporter exporter = new InSourceFragmentExporter();

            return exporter;

        }
    }
}

