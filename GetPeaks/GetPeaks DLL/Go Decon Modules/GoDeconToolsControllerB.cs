using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using DeconTools.Backend.Utilities;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Backend;
using System.Threading;
using DeconTools.Backend.DTO;
using PNNLOmics.Algorithms.PeakDetection;
using PNNLOmics.Data;
using GetPeaks_DLL.Common_Switches;
using GetPeaks_DLL.PNNLOmics_Modules;
using GetPeaks_DLL.Go_Decon_Modules;
using MemoryOverloadProfilierX86;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Objects.ResultsObjects;

namespace GetPeaks_DLL
{
    public class GoDeconToolsControllerB:IDisposable
    {
        //public MSGeneratorFactory msgenFactory { get; set; }//= new MSGeneratorFactory();
        //public Task msGenerator { get; set; }
        ///public Task msGenerator { get; set; }
        public SimpleWorkflowParameters Parameters { get; set; }
        public DeconToolsPeakDetector MSPeakDetector { get; set; }
        public Deconvolutor Deconvolutor { get; set; }
        //public Task Deconvolutor { get; set; }

        public void Close()
        {
            //this.msgenFactory = null;
            ///this.msGenerator = null;
            //this.Parameters = null;
            this.MSPeakDetector = null;
            //this.Deconvolutor.Cleanup();//does nothing
            //It is impossible to delete the deconengine once it is made. You need to go to C++ world and it is still hard
            this.Deconvolutor = null;
            this.Parameters.Dispose();
            this.Parameters = null;
        }

        public void Dispose()
        {
            this.Close();
        }

        GoDeconToolsControllerB()
        {
            //this.msgenFactory = new MSGeneratorFactory();
        }

        public GoDeconToolsControllerB(SimpleWorkflowParameters parameters,DeconTools.Backend.Globals.MSFileType fileType  )
            : this()
        {
            GOinitializeTasks(parameters, fileType);
        }

        private void GOinitializeTasks(SimpleWorkflowParameters parameters, DeconTools.Backend.Globals.MSFileType MSFileType)
        {
            ///this.msGenerator = msgenFactory.CreateMSGenerator(MSFileType);//pases memory 3-21-11
            this.Parameters = parameters;//pases memory 3-21-11v
            this.MSPeakDetector = new DeconToolsPeakDetector(parameters.Part2Parameters.MSPeakDetectorPeakBR, parameters.Part2Parameters.MSPeakDetectorSigNoise, Globals.PeakFitType.QUADRATIC, false);//pases memory 3-21-11
            this.MSPeakDetector.PeaksAreStored = true;
            switch (parameters.Part2Parameters.DeconvolutionType)
            {
                case DeconvolutionType.Rapid:
                    {
                        this.Deconvolutor = new RapidDeconvolutor();
                    }
                    break;
                case DeconvolutionType.Thrash:
                    {
                        HornDeconvolutor decon = new HornDeconvolutor();
                        this.Deconvolutor = decon;
                    }
                    break;
                default:
                    {
                    }
                    break;
            }            
        }

        //we need to sort the peaks and check to see if they are assigned as isotopes
        /// <summary>
        /// This is the module decon tools that converts a XYData from a spectra to a IsosResults
        /// </summary>
        /// <param name="run">This Run needs to have the XYData populated allowing it to be decoupled from the disk</param>
        /// <param name="ePeak">eluting peak of interest</param>
        /// <param name="parameters"></param>
        public GoResults GODeconToolsFX(Run run, SimpleWorkflowParameters parameters, TransformerObject transformer2)
        {
            GoResults resultPeakIsos = new GoResults();

            ResultCollectionLite deconvolutionOutput = new ResultCollectionLite();

            //Profiler newProfiler = new Profiler();
            //TODO: make sure XYdata>0
            if (run.XYData.Xvalues.Length > 0)
            {
                try
                {
                    if (3 == 3)//ok 3-23
                    {
                        #region inside
                        //Monitor.Enter(myLockObject);
                        Parameters = parameters;

                        //           GoDeconToolsControllerB controller = new GoDeconToolsControllerB(run, Parameters);

                        // in general we want the full spectra so that we can do more operations on it
                        int e = run.XYData.Xvalues.Length;

                        #region possible trim data.  this is currently off because we want all the data
                        //int featureWindowBefore = 4;
                        //int featureWindow = 7;
                        //List<Dictionary<string, XYData>> piledupData = new List<Dictionary<string, XYData>>();

                        //string name = Thread.CurrentThread.Name;
                        //XYData localXYData = new XYData();
                        //localXYData = run.XYData.TrimData(ePeak.PeakList[0].MSPeak.XValue - featureWindowBefore, ePeak.PeakList[0].MSPeak.XValue + featureWindow);
                        //Dictionary<string, XYData> newItem = new Dictionary<string, XYData>();
                        //newItem.Add(name, localXYData);
                        //piledupData.Add(newItem);
                        #endregion

                        #region set up XYdata for rapid.  trim data is not needed because we want related peaks (charge state, Na+ etc.)
                        
                        run.ResultCollection.Run.XYData = run.XYData;//for rapid
                        //run.ResultCollection.Run.XYData = run.XYData.TrimData(ePeak.PeakList[0].MSPeak.XValue - featureWindowBefore, ePeak.PeakList[0].MSPeak.XValue + featureWindow);//for rapid

                        #endregion
                        
                        #region peak detection
                    bool OmicsPeakDecection = true;
                    //setup outside loop so we can use the data
                    //Console.WriteLine("startPeakdetection");
                       
                            
                        PeakThresholderParameters parametersThreshold;
                        List<ProcessedPeak> thresholdedData = new List<ProcessedPeak>();
                        List<ProcessedPeak> processedPeakList = new List<ProcessedPeak>();

                        if (OmicsPeakDecection)
                        {
                            //Monitor.Enter(myLockObject);
                            //Console.WriteLine("thread : " + Thread.CurrentThread.Name + " has entered the Omics peak detection");
                            ///input: run.ResultCollection.Run.XYData
                            #region OmicsPeakDetection
                            //1.  extract XYData
                            List<double> tempXvalues = new List<double>();
                            List<double> tempYValues = new List<double>();
                            tempXvalues = run.ResultCollection.Run.XYData.Xvalues.ToList();
                            tempYValues = run.ResultCollection.Run.XYData.Yvalues.ToList();

                            List<PNNLOmics.Data.XYData> newData = new List<PNNLOmics.Data.XYData>();
                            for (int i = 0; i < run.ResultCollection.Run.XYData.Xvalues.Length; i++)
                            {
                                PNNLOmics.Data.XYData newPoint = new PNNLOmics.Data.XYData((float)tempXvalues[i], (float)tempYValues[i]);
                                newData.Add(newPoint);
                            }

                            bool isModule = true;
                            if (isModule)
                            {
                                //1.  discover peaks
                                PeakCentroider newPeakCentroider = new PeakCentroider();
                                //                      newPeakCentroider.Parameters.ScanNumber = 0;//scan number max?
                                newPeakCentroider.Parameters.FWHMPeakFitType = SwitchPeakFitType.setPeakFitType(Parameters.Part2Parameters.PeakFitType);

                                List<ProcessedPeak> discoveredPeakList = new List<ProcessedPeak>();
                                discoveredPeakList = newPeakCentroider.DiscoverPeaks(newData);

                                //2. threshold data
                                parametersThreshold = new PeakThresholderParameters();
                                parametersThreshold.SignalToShoulderCuttoff = (float)Parameters.Part2Parameters.MSPeakDetectorSigNoise;
                                parametersThreshold.DataNoiseType = Parameters.Part2Parameters.NoiseType;
                                parametersThreshold.isDataThresholded = parameters.Part2Parameters.isDataThresholded;
                                SwitchThreshold newSwitchThreshold = new SwitchThreshold();
                                newSwitchThreshold.Parameters = parametersThreshold;
                                newSwitchThreshold.ParametersOrbitrap = parameters.Part2Parameters.ParametersOrbitrap;
                                processedPeakList = newSwitchThreshold.ThresholdNow(ref discoveredPeakList);


                                discoveredPeakList = null;
                                //processedPeakList = null;
                                parametersThreshold = null;

                            }
                            else
                            {
                                #region else
                                GetPeaksPeakDetectorParameters parametersGetPeaksPeakDetector = new GetPeaksPeakDetectorParameters();
                                //  parametersGetPeaksPeakDetector.CentroidParameters.ScanNumber = 0;//scan number max?
                                parametersGetPeaksPeakDetector.ThresholdParameters.SignalToShoulderCuttoff = (float)(this.Parameters.Part2Parameters.MSPeakDetectorSigNoise);/// /5.1635
                                parametersGetPeaksPeakDetector.CentroidParameters.FWHMPeakFitType = SwitchPeakFitType.setPeakFitType(Parameters.Part2Parameters.PeakFitType);

                                processedPeakList = GetPeaksPeakDetector.GetProcessedPeaks(newData, parametersGetPeaksPeakDetector, ThresholdType.AveragePlusSigma);
                                #endregion
                            }

                            //4. save data to Decon Tools
                            //  real                    run.ResultCollection.Run.PeakList = new List<IPeak>();
                            //List<PNNLOmics.Data.XYData> omicsList = parametersThreshold.ThresholdedPeakData;
                            //List<PNNLOmics.Data.XYData> omicsList = parametersThreshold.ThresholdedPeakData;
                            // real                     run.ResultCollection.PeakCounter = run.ResultCollection.MSPeakResultList.Count + processedPeakList.Count;
                            //run.ResultCollection.PeakCounter = run.ResultCollection.MSPeakResultList.Count + omicsList.Count;

                            #region vestigial print stuff
                            //List<double> printMasses = new List<double>();
                            //List<double> printInt = new List<double>();
                            //List<double> prinFWHM = new List<double>();
                            //string masses = "";
                            //string intensity = "";
                            //string FWHM = "";
                            #endregion

                            //run.ResultCollection.Run.PeakList = new List<IPeak>();//for decon tools?
                            run.ResultCollection.Run.PeakList = new List<DeconTools.Backend.Core.Peak>();//for decon tools?
                            DeconToolsV2.Peaks.clsPeak[] peaklist = new DeconToolsV2.Peaks.clsPeak[processedPeakList.Count];//for rapid

                            for (int i = 0; i < processedPeakList.Count; i++)
                            {
                                //for decono tools
                                MSPeak newMSPeak = ProcessedPeakToMSPeak(processedPeakList, i);
                                run.ResultCollection.Run.PeakList.Add(newMSPeak);

                                //for rapid
                                ProcessedPeakListTomdbl(processedPeakList, peaklist, i);



                                #region vestigial print stuff
                                ////printMasses.Add(omicsList[i].X);
                                ////printInt.Add(omicsList[i].Y);
                                ////prinFWHM.Add(newMSPeak.Width);

                                ////masses += printMasses[i].ToString() + ",";
                                ////intensity += printInt[i].ToString() + ",";
                                ////FWHM += prinFWHM[i].ToString() + ",";
                                #endregion


                                ////update mspeakresults
                                //MSPeakResult newMSPeakResult = new MSPeakResult();
                                //newMSPeakResult.ChromID = -1;
                                //newMSPeakResult.Frame_num = -1;
                                //newMSPeakResult.MSPeak = newMSPeak;
                                //newMSPeakResult.PeakID = i;
                                //newMSPeakResult.Scan_num = processedPeakList[i].ScanNumber;
                                //run.ResultCollection.MSPeakResultList.Add(newMSPeakResult);
                            }
                            run.ResultCollection.Run.DeconToolsPeakList = peaklist;//for rapid

                            #endregion
                            ///output: parametersThreshold.ThresholdedPeakData etc.
                            ///output: parametersThreshold.ThresholdedPeakSNShoulder
                            ///output: run.ResultCollection.PeakCounter
                            ///output: run.ResultCollection.MSPeakResultList
                            ///output: run.ResultCollection.Run.PeakList 

                            //Console.WriteLine("thread : " + Thread.CurrentThread.Name + " has left the Omics peak detection");
                            //Monitor.Exit(myLockObject);

                            tempXvalues = null;
                            tempYValues = null;
                        }
                        else
                        {
                            ///Input: run.ResultCollection.run.XYData
                            #region MSPeakDetector + SN values
                            //parametersThreshold = new PeakThresholderParameters();
                            //parametersPeakDetector = new PeakDetectorParameters();
                            run.ResultCollection.Run.PeakList = null;
                            this.MSPeakDetector.Execute(run.ResultCollection);
                            //controller.MSPeakDetector.Execute(run.ResultCollection);
                            for (int i = 0; i < run.ResultCollection.MSPeakResultList.Count; i++)
                            {
                                //parametersThreshold.ThresholdedPeakSNShoulder.Add(run.ResultCollection.MSPeakResultList[i].MSPeak.SN);
                                //This did not convert  
                                //parametersThreshold.ThresholdedPeakSNShoulder.Add(run.ResultCollection.MSPeakResultList[i].MSPeak.SN);
                            }

                            #region vestigial print stuff
                            //List<double> printMasses = new List<double>();
                            //List<double> printInt = new List<double>();
                            //List<double> prinFWHM = new List<double>();
                            //string masses2 = "";
                            //string intensity2 = "";
                            //string FWHM2 = "";


                            //for (int i = 0; i < run.ResultCollection.Run.PeakList.Count; i++)
                            //{
                            //    printMasses.Add(run.ResultCollection.Run.PeakList[i].XValue);

                            //    printInt.Add(run.ResultCollection.Run.PeakList[i].Height);

                            //    prinFWHM.Add(run.ResultCollection.Run.PeakList[i].Width);

                            //    masses2 += printMasses[i].ToString() + ",";
                            //    intensity2 += printInt[i].ToString() + ",";
                            //    FWHM2 += prinFWHM[i].ToString() + ",";
                            //}
                            #endregion

                            #endregion
                            ///output: run.ResultCollection.PeakCounter
                            ///output: run.ResultCollection.MSPeakResultList
                            ///output: run.ResultCollection.Run.PeakList
                            ///output: run.ResultCollection.Run.DeconToolsPeakList
                            ///output: parametersThreshold.ThresholdedPeakSNShoulder
                        }
                    #endregion

                        ///input:  run.ResultCollection.Run.PeakList
                        #region setup data for Rapid off
                        //Console.WriteLine("thread : " + Thread.CurrentThread.Name + " has entered the Decon Peaklist section" + run.ResultCollection.Run.PeakList.Count.ToString());
                        //DeconToolsV2.Peaks.clsPeak[] peaklist = new DeconToolsV2.Peaks.clsPeak[run.ResultCollection.Run.PeakList.Count];
                        //for (int i = 0; i < peaklist.Length; i++)

                        #region old convert to rapid
                        //DeconToolsV2.Peaks.clsPeak[] peaklist = new DeconToolsV2.Peaks.clsPeak[run.ResultCollection.Run.PeakList.Count];
                        //for (int i = 0; i < peaklist.Length; i++)
                        //{
                        //    peaklist[i] = new DeconToolsV2.Peaks.clsPeak();

                        //    peaklist[i].mdbl_intensity = run.ResultCollection.Run.PeakList[i].Height;
                        //    peaklist[i].mdbl_mz = run.ResultCollection.Run.PeakList[i].XValue;
                        //    peaklist[i].mdbl_FWHM = run.ResultCollection.Run.PeakList[i].Width;

                        //    if (OmicsPeakDecection)
                        //    {
                        //        peaklist[i].mdbl_SN = processedPeakList[i].SignalToNoiseGlobal;
                        //    }
                        //    else
                        //    {
                        //        //peaklist[i].mdbl_SN = processedPeakList[i].SignalToNoiseLocalMinima;

                        //        peaklist[i].mdbl_SN = run.ResultCollection.MSPeakResultList[run.ResultCollection.MSPeakResultList.Count - peaklist.Length + i].MSPeak.SN;
                        //    }

                        //}
                        //run.ResultCollection.Run.DeconToolsPeakList = peaklist;//for rapid
                        #endregion
                        #endregion
                        ///output:  run.ResultCollection.Run.DeconToolsPeakList

                        #region old deconvolutor off

                        //clear out XYData
                        //run.XYData = new DeconTools.Backend.XYData();


                        //Console.WriteLine("start deconvolution");
                        //if (run.ResultCollection.Run.PeakList.Count > 200000)//2 points will crash rapid.  we need atleast 3// this is toggles out so we go below
                        //{
                        //    #region deconvolutor off because of 200000
                        //    //Console.WriteLine("thread : " + Thread.CurrentThread.Name + " has entered the deconvolutor");

                        //    switch (parameters.Part2Parameters.DeconvolutionType)
                        //    {
                        //        case DeconvolutionType.Rapid:
                        //            {
                        //                this.Deconvolutor = new RapidDeconvolutor();
                        //                //controller.Deconvolutor = new RapidDeconvolutor();
                        //            }
                        //            break;
                        //        case DeconvolutionType.Thrash:
                        //            {
                        //                HornDeconvolutor decon = new HornDeconvolutor();
                        //                this.Deconvolutor = decon;
                        //                //controller.Deconvolutor = new HornDeconvolutor();
                        //            }
                        //            break;
                        //    }
                        //    #endregion
                        //}
                        #endregion
                        if (4 == 4)
                        {
                            //Profiler newProfiler = new Profiler();

                            //newProfiler.printMemory("Inside ControllerB ");
                            //Console.ReadKey();

                            //TODO we need to find everything that changes in runGo
                            if (run.ResultCollection.Run.PeakList.Count > 2)//2 points will crash rapid.  we need atleast 3
                            {
                                //newProfiler.printMemory("Before ");
                                //this.Deconvolutor.deconvolute(run.ResultCollection);
                                

                                float[] xvals;
                                float[] yvals;
                                DeconToolsV2.Peaks.clsPeak[] mspeakList;
                                DeconToolsV2.HornTransform.clsHornTransformResults[] transformResults2;
                                GoTransformPrep transformPrep = new GoTransformPrep();
                                //transformPrep.PrepDeconvolutor(run, transformer2, out xvals, out yvals, out mspeakList, out transformResults2);
                                transformPrep.PrepDeconvolutor(run, out xvals, out yvals, out mspeakList, out transformResults2);

                                float backgroundIntensity = 0;
                                float minPeptideIntensity = 0;
                                transformer2.TransformEngine.PerformTransform(backgroundIntensity, minPeptideIntensity, ref xvals, ref yvals, ref mspeakList, ref transformResults2);

                                //if (transformResults2.Length == 0)
                                //{
                                //    Console.WriteLine("zero isotopes");
                                //    Console.ReadKey();
                                //}

                                deconvolutionOutput = transformPrep.FormatResults(transformResults2, mspeakList, transformer2);
                                //deconvolutionOutput.Dispose();
                                transformResults2 = null;
                                
                                //this.Deconvolutor.deconvolute(ref xvals, ref yvals, backgroundIntensity, minPeptideIntensity, ref mspeakList);
                              
                                //this.Deconvolutor.cleanup();
                                //this.Deconvolutor = null;
                                //newProfiler.printMemory("After ");
                            }

                            //    //controller.Deconvolutor.deconvolute(run.ResultCollection);
                            //newProfiler.printMemory("After Deconvolute ");
                            //Console.ReadKey();
                            //    //Console.WriteLine("thread : " + Thread.CurrentThread.Name + " has left the deconvolutor");
                            //    //before we assigne the isotope we need to check the mass accuracy to the mad int peak
                            //    //Console.WriteLine("leave deconvolution");

                            //    //double error = 0;
                            //    //TODO:  move elsewhe Probably to GO accelerate just after decontoolsFX runs
                            //    //this should all go somethwere else so that the controllerB runs and returns as is.  we can sort the isosResults later
                            //    // Acquire the lock.

                            
                            //}
      //                      run.ResultCollection.Run.DeconToolsPeakList = null;
                            //newProfiler.printMemory("After Deconvolute ");
      //                      run.ResultCollection.Run.PeakList.Clear();
                            //newProfiler.printMemory("After Deconvolute ");
     //                       run.ResultCollection.Run.PeakList = null;
                            //newProfiler.printMemory("After Deconvolute ");
                            //TODO clear our decon results here!!!!!3-23-11
                            //real                    peaklist = null;
                            //real                    run.ResultCollection.Run.DeconToolsPeakList = null;

                            //clean up run now!
                            run.XYData.Xvalues = null;
                            run.XYData.Yvalues = null;
                            //newProfiler.printMemory("After Deconvolute ");
                            run.XYData = null;
                            //newProfiler.printMemory("After Deconvolute ");
                            run.PeakList = null;
                            //newProfiler.printMemory("After Near end ");
                            //run.ResultCollection.Run.DeconToolsPeakList = null;
                        }
                        processedPeakList = null;
                        thresholdedData = null;
                        
                         
                        #endregion
                    }
                        
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                Console.WriteLine("XYData Missing");
            }

            foreach (IsosResultLite isos in deconvolutionOutput.IsosResultBin)
            {
                StandardIsosResult newIsos = new StandardIsosResult();
                IsotopicProfile newIsotopicProfilePeakList = ObjectCopier.Clone<IsotopicProfile>(isos.IsotopicProfile);
                newIsos.IsotopicProfile = newIsotopicProfilePeakList;
                resultPeakIsos.IsosResultList.Add(newIsos);
                isos.IsotopicProfile.Peaklist.Clear();
                isos.IsotopicProfile.Peaklist = null;
                isos.IsotopicProfile = null;
            }
            deconvolutionOutput.Dispose();


            //newProfiler.printMemory("++after results bin ");
            //this.Dispose();
            return resultPeakIsos;
        }

       

       

        private static void ProcessedPeakListTomdbl(List<ProcessedPeak> processedPeakList, DeconToolsV2.Peaks.clsPeak[] peaklist, int i)
        {
            peaklist[i] = new DeconToolsV2.Peaks.clsPeak();
            peaklist[i].mdbl_intensity = processedPeakList[i].Height;
            peaklist[i].mdbl_mz = processedPeakList[i].XValue;
            peaklist[i].mdbl_FWHM = processedPeakList[i].Width;
            peaklist[i].mdbl_SN = Convert.ToSingle(processedPeakList[i].SignalToNoiseGlobal);
        }

        private static DeconTools.Backend.Core.MSPeak ProcessedPeakToMSPeak(List<ProcessedPeak> processedPeakList, int i)
        {
            DeconTools.Backend.Core.MSPeak newMSPeak = new DeconTools.Backend.Core.MSPeak();
            newMSPeak.XValue = processedPeakList[i].XValue;
            //newMSPeak.Height = (float)thresholdedData[i].Intensity;
            //TODO check this height = intensity
            newMSPeak.Height = Convert.ToSingle(processedPeakList[i].Height);
            newMSPeak.Width = processedPeakList[i].Width;
            newMSPeak.SignalToNoise = Convert.ToSingle(processedPeakList[i].SignalToNoiseGlobal);
            newMSPeak.DataIndex = processedPeakList[i].ScanNumber; //save scan here
            return newMSPeak;
        }  
    }
}
