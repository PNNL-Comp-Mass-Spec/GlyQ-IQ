using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using DeconTools.Backend.Core;
using GetPeaks_DLL.SQLite;
using PNNLOmics.Data;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Functions;
using MemoryOverloadProfilierX86;
using GetPeaks_DLL.Objects.ResultsObjects;

namespace GetPeaks_DLL.Go_Decon_Modules
{
    public class GoTHRASH:IDisposable
    {
        /// <summary>
        /// Place to hold monoisotopicMasses
        /// </summary>
        public List<ProcessedPeak> MonoIsotopicMasses { get; set; }

        /// <summary>
        /// Place to hold charge states
        /// </summary>
        public List<int> ChargeStates { get; set; }

        /// <summary>
        /// Simple constructor
        /// </summary>
        public GoTHRASH()
        {
            MonoIsotopicMasses = new List<ProcessedPeak>();
            ChargeStates = new List<int>();
        }

        /// <summary>
        /// This is the old version
        /// Analyze Eluting Peaks to find Monoisotopic Peaks.  This is better because it does not use Run
        /// </summary>
        /// <param name="discoveredPeaks"></param>
        /// <param name="XYDataIn"></param>
        /// <param name="fileIn">data file name</param>
        /// <param name="Parameters"></param>
        /// <param name="numberOfHits"></param>
        /// <param name="transformer2"></param>
        /// <param name="dataFileIterator"></param>
        public void MultiThread(List<ProcessedPeak> discoveredPeaks, List<XYData> XYDataIn, InputOutputFileName fileIn, SimpleWorkflowParameters Parameters, ref int numberOfHits, TransformerObject transformer2, ref int dataFileIterator)//Analyze Eluting Peaks to find Monoisotopic peaks vis decon tools
        {
            int threadName = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Profiler newProfiler = new Profiler();
            
            #region inside

            //create SQLiteDatabase for dumping infor from threads
            DatabaseLayer newDataBaseLayer = new DatabaseLayer();
            bool didThisWork = false;
            string localSqlFileName = transformer2.SQLiteTranformerFolderPath + @"\" + transformer2.SQLiteTranformerFileName + @".db";
            if (fileIn.OutputSQLFileName != null)
            {
                didThisWork = newDataBaseLayer.CreateFeatrueLiteTableInformation(fileIn.OutputSQLFileName);
            }
            else
            {
                Console.WriteLine("Need a location for the database");
                Console.ReadKey();

            }

            if (didThisWork == true)
            {
                //Console.WriteLine("TableInfoCreated");//just for printing
            }
            else
            {
                transformer2.ErrorLog.Add("this did not work, GoTHRASH MultiThread");
            }

            #region inside
            #region setup

            //int peakCount = 0;
            int totalPeakCount = discoveredPeaks.Count;

            List<Run> listofRuns = new List<Run>();

            #endregion

            int limitThreadCount = discoveredPeaks.Count;
            //limitThreadCount = 10;
            float limitIntensity = (float)(discoveredPeaks[0].Height / Parameters.Part2Parameters.DynamicRangeToOne);

            int numberOfElements = discoveredPeaks.Count;
            int numberOfChunks = Parameters.Part2Parameters.MemoryDivider.NumberOfBlocks;
            int rank = Parameters.Part2Parameters.MemoryDivider.BlockNumber;

            //pull these out front
                    
            #region inside
                        
            #region inside

            //passes memory 3-21-11
                            
            //Profiler newProfiler = new Profiler();

            //newProfiler.printMemory("++pre controllerB " + threadName);
            //newProfiler.printMemory("++pre controllerB " + threadName);
            //TODO dispose GoDeconToolsControllerB

            SimpleWorkflowParameters controllerBParameters = ObjectCopier.Clone<SimpleWorkflowParameters>(Parameters);
            GoDeconToolsControllerB newDeconToolsPart2 = new GoDeconToolsControllerB(controllerBParameters, DeconTools.Backend.Globals.MSFileType.Undefined);
            ScanSumSelectSwitch summingMethod = controllerBParameters.SummingMethod;

            //passes memory 3-21-11
                                
            #region inside

            Run runGo = GoCreateRun.CreateRun(fileIn);

            ConvertProcessedPeakListToResultCollectionPeakList(discoveredPeaks, runGo, 50);

            ConvertXYDataToResultsCollectionXYData(XYDataIn, runGo);


            //passes memory 3-21-11
                            
            #region full loop

            //passed memory 3-21-11
                                                    
            #region inside

            //passed memory 3-21-11
                                                        
            #region inside
                                                     
            #region -GoDeconToolsControllerB- run part 2  to deisotope the the spine now that we have the correct XYData

            //newProfiler.printMemory("RUN DECONTOOLS FX (Create isoOutput)" + threadName);
            //lock (deconvolutionLock)//some times the memory goes up inside this lock

            ResultCollectionLite deconvolutionOutput = new ResultCollectionLite();

            if (runGo.ResultCollection.Run.PeakList.Count > 2)//2 points will crash rapid.  we need atleast 3
            {
                float[] xvals;
                float[] yvals;
                DeconToolsV2.Peaks.clsPeak[] mspeakList;
                DeconToolsV2.HornTransform.clsHornTransformResults[] transformResults2;
                GoTransformPrep transformPrep = new GoTransformPrep();
                //transformPrep.PrepDeconvolutor(runGo, transformer2, out xvals, out yvals, out mspeakList, out transformResults2);
                transformPrep.PrepDeconvolutor(runGo, out xvals, out yvals, out mspeakList, out transformResults2);

                Console.WriteLine("running Thrash...");
                float backgroundIntensity = 0;
                float minPeptideIntensity = 0;

                bool fixThrash = true;
                if (fixThrash)
                {
                    //for (int i = 0; i < mspeakList.Length; i++)
                    //{
                    //    mspeakList[i].mdbl_FWHM = 0.014;
                    //}
                    transformer2.TransformEngine.TransformParameters.PeptideMinBackgroundRatio = 0;
                }
                
            
            transformer2.TransformEngine.PerformTransform(backgroundIntensity, minPeptideIntensity, ref xvals, ref yvals, ref mspeakList, ref transformResults2);

                if (transformResults2.Length == 0)
                {
                    Console.WriteLine("zero isotopes");
                    //Console.ReadKey();
                }
                deconvolutionOutput = new ResultCollectionLite();
                deconvolutionOutput = transformPrep.FormatResults(transformResults2, mspeakList, transformer2);

                //deconvolutionOutput.Dispose();
                transformResults2 = null;
            }

                                                                        
            runGo.ResultCollection.Run.DeconToolsPeakList = null;

            //clean up run now!
            runGo.XYData.Xvalues = null;
            runGo.XYData.Yvalues = null;
            runGo.XYData = null;
            runGo.PeakList = null;
            runGo.Dispose();
            runGo = null;

            int y = 4; int hg = y * 7;

            #region inside
            //IsotopeObject isotopeStorage = new IsotopeObject();

                                                                            
            //isotopeOutput.Dispose();
            ////if (ePeak.ID == 1)//write to disk
            bool writeToDisk = true;
            if (writeToDisk)//write to disk
            {

                //                            discoveredPeaks[e].ID = 1;
                lock (transformer2.DatabaseLock)//some times the memory goes up inside this lock
                {
                    #region inside

                    bool isSuccessfull = false;

                    numberOfHits++;
                    //printMemory("baseline before write");

                    using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(fileIn.OutputSQLFileName))
                    {    
                        for(int t=0;t<deconvolutionOutput.IsosResultBin.Count;t++)
                        {
                            IsosResultLite isos = deconvolutionOutput.IsosResultBin[t];

                            ProcessedPeak newMonoPeak = ConvertIsosResultLiteToProcessedPeak(isos);
                            MonoIsotopicMasses.Add(newMonoPeak);
                            ChargeStates.Add(isos.IsotopicProfile.ChargeState);
                                                                                            
                            IsotopeObject isosToWrite = ConvertIsosResultsBinToIsotopeObject(isos);

                            isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageToSQLite_Old(isosToWrite);

                            if (isSuccessfull)
                            {
                                //Console.WriteLine("++Data Stored-Mass: " + IsosToWrite.MonoIsotopicMass + " Charge: " + Math.Round(1.00235/(IsosToWrite.IsotopeList[1].XValue-IsosToWrite.IsotopeList[0].XValue)));
                                Console.WriteLine("++Data Stored-Mass: ");
                            }
                            else
                            {
                                Console.WriteLine("Data Not Stored");
                                transformer2.ErrorLog.Add("data not stored, EluringPeakFinderPart2");
                            }
                        }

                        Profiler memoryCheck = new Profiler();
                        memoryCheck.printMemory("baseline during write");

                        //ePeak = null;
                        //resultPeak = null;
                    }
                    #endregion
                }
                //printMemory("baseline after write");
            }
            else
            {
                //this is ok because not all peaks should not be writen, only the good ones
                //transformer2.ErrorLog.Add("resultpeakID is not 1, ElutingPeakPart2");
            }

            #endregion

            #endregion
                                                            
            #endregion
                                                        

            //clean up run
            newProfiler.printMemory("CanWeDeleteRun? " + threadName);
            //runGo.Dispose();
            newProfiler.printMemory("Run is disposed " + threadName);
            runGo = null;
            newProfiler.printMemory("Run is Null " + threadName);
            #endregion

            int k = 5;
            k++;

            #endregion
                            


            #endregion
                                

            int kh = 3; int ge = kh * 4;
                            
            #endregion
                        
            #endregion
                    
            #endregion
                
            newDataBaseLayer = null;

            #endregion
            
            newProfiler.printMemory("All done!");
            Console.WriteLine("End Thread #" + threadName.ToString());
        }

        private static ProcessedPeak ConvertIsosResultLiteToProcessedPeak(IsosResultLite isos)
        {
            ProcessedPeak newMonoPeak = new ProcessedPeak();
            newMonoPeak.Height = isos.IsotopicProfile.OriginalIntensity;
            newMonoPeak.XValue = isos.IsotopicProfile.MonoIsotopicMass;
            newMonoPeak.Width = isos.IsotopicProfile.Peaklist[0].Width;
            newMonoPeak.SignalToNoiseGlobal = isos.IsotopicProfile.Peaklist[0].SignalToNoise;

            return newMonoPeak;
        }

        private static IsotopeObject ConvertIsosResultsBinToIsotopeObject(IsosResultLite isos)
        {
            IsotopeObject IsosToWrite = new IsotopeObject();
            IsosToWrite.MonoIsotopicMass = isos.IsotopicProfile.MonoIsotopicMass;
            IsosToWrite.ExperimentMass = isos.IsotopicProfile.MonoPeakMZ;

            //first peak
            MSPeak isosPeak = isos.IsotopicProfile.Peaklist[0];         
            //Peak newPeak = new Peak();
            PNNLOmics.Data.Peak newPeak = new PNNLOmics.Data.Peak();
            newPeak.Height = isosPeak.Height;
            newPeak.LocalSignalToNoise = isosPeak.SignalToNoise;
            newPeak.Width = isosPeak.Width;
            newPeak.XValue = isosPeak.XValue;

            IsosToWrite.IsotopeMassString += isosPeak.XValue.ToString();
            IsosToWrite.IsotopeIntensityString += isosPeak.Height.ToString();

            IsosToWrite.IsotopeList.Add(newPeak);

            //next peaks
            if (isos.IsotopicProfile.Peaklist.Count > 1)
            {
                for (int i = 1; i < isos.IsotopicProfile.Peaklist.Count; i++)
                {
                    isosPeak = isos.IsotopicProfile.Peaklist[i];

                    newPeak = new PNNLOmics.Data.Peak();
                    //newPeak = new Peak();
                    newPeak.Height = isosPeak.Height;
                    newPeak.LocalSignalToNoise = isosPeak.SignalToNoise;
                    newPeak.Width = isosPeak.Width;
                    newPeak.XValue = isosPeak.XValue;

                    IsosToWrite.IsotopeMassString += "," + isosPeak.XValue.ToString();
                    IsosToWrite.IsotopeIntensityString += "," + isosPeak.Height.ToString();

                    IsosToWrite.IsotopeList.Add(newPeak);
                }
            }
            return IsosToWrite;
        }

        /// <summary>
        /// Converts XYData
        /// </summary>
        /// <param name="XYDataIn"></param>
        /// <param name="runGo"></param>
        private static void ConvertXYDataToResultsCollectionXYData(List<XYData> XYDataIn, Run runGo)
        {
            runGo.ResultCollection.Run.XYData.Xvalues = new double[XYDataIn.Count];
            runGo.ResultCollection.Run.XYData.Yvalues = new double[XYDataIn.Count];
            for (int i = 0; i < XYDataIn.Count; i++)
            {
                runGo.ResultCollection.Run.XYData.Xvalues[i] = XYDataIn[i].X;
                runGo.ResultCollection.Run.XYData.Yvalues[i] = XYDataIn[i].Y;
            }
        }

        private static void ConvertProcessedPeakListToResultCollectionPeakList(List<ProcessedPeak> processedPeakList, Run runGo, float defaultSN)
        {
            //runGo.ResultCollection.Run.PeakList = new List<IPeak>();//for decon tools?
            runGo.ResultCollection.Run.PeakList = new List<DeconTools.Backend.Core.Peak>();//for decon tools?
            DeconToolsV2.Peaks.clsPeak[] peaklist = new DeconToolsV2.Peaks.clsPeak[processedPeakList.Count];//for rapid
            for (int i = 0; i < processedPeakList.Count; i++)
            {
                //for decono tools
                MSPeak newMSPeak = ProcessedPeakToMSPeak(processedPeakList, i);
                if (newMSPeak.SignalToNoise == 0)
                {
                    newMSPeak.SignalToNoise = defaultSN;
                    
                }
                if(processedPeakList[i].SignalToNoiseGlobal==0)
                {
                    processedPeakList[i].SignalToNoiseGlobal = defaultSN;
                }
                runGo.ResultCollection.Run.PeakList.Add(newMSPeak);

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
            runGo.ResultCollection.Run.DeconToolsPeakList = peaklist;//for rapid
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

        private static void ProcessedPeakListTomdbl(List<ProcessedPeak> processedPeakList, DeconToolsV2.Peaks.clsPeak[] peaklist, int i)
        {
            peaklist[i] = new DeconToolsV2.Peaks.clsPeak();
            peaklist[i].mdbl_intensity = processedPeakList[i].Height;
            peaklist[i].mdbl_mz = processedPeakList[i].XValue;
            peaklist[i].mdbl_FWHM = processedPeakList[i].Width;
            peaklist[i].mdbl_SN = Convert.ToSingle(processedPeakList[i].SignalToNoiseGlobal);
        }

        #region IDisposable Members

        public void Dispose()
        {
            //this = null
        }

        #endregion
    }
}
