﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

using IQ.Backend.Core;
using IQ.Backend.Utilities.IsotopeDistributionCalculation;
using IQ.Workflows.Core;
using IQ.Workflows.FileIO;
using IQ.Workflows.FileIO.DTO;
using IQ.Workflows.FileIO.Importers;
using IQ.Workflows.Utilities;
using IQ.Workflows.WorkFlowParameters;

using Run32.Backend;
using Run32.Backend.Core;
using Run32.Backend.Runs;
using Run32.Utilities;

namespace IQ.Workflows.WorkFlowPile
{
    public abstract class TargetedWorkflowExecutor : WorkflowBase
    {
        protected IsotopicDistributionCalculator IsotopicDistributionCalculator = IsotopicDistributionCalculator.Instance;
            

        protected string _loggingFileName;
        protected string _resultsFolder;
        protected TargetedResultRepository ResultRepository;


        protected List<long> MassTagIDsinTargets = new List<long>();

        protected WorkflowParameters _workflowParameters;

        protected BackgroundWorker _backgroundWorker;
        private TargetedWorkflowExecutorProgressInfo _progressInfo = new TargetedWorkflowExecutorProgressInfo();

        #region Constructors
        public TargetedWorkflowExecutor(WorkflowExecutorBaseParameters parameters, string datasetPath, BackgroundWorker backgroundWorker = null)
        {
            this.DatasetPath = datasetPath;

            _backgroundWorker = backgroundWorker;

            this.WorkflowParameters = parameters;

            MsgfFdrScoreCutoff = 0.1;

            ResultRepository = new TargetedResultRepository();
            InitializeWorkflow();
        }

        public TargetedWorkflowExecutor(WorkflowExecutorBaseParameters workflowExecutorParameters, TargetedWorkflow targetedWorkflow,string datasetPath, BackgroundWorker backgroundWorker = null )
        {
            this.DatasetPath = datasetPath;

            _backgroundWorker = backgroundWorker;

            this.WorkflowParameters = workflowExecutorParameters;

            TargetedWorkflow = targetedWorkflow;

            ResultRepository = new TargetedResultRepository();
            InitializeWorkflow();
        }



		public TargetedWorkflowExecutor(WorkflowExecutorBaseParameters workflowExecutorParameters, WorkflowParameters workflowParameters, string datasetPath, BackgroundWorker backgroundWorker = null)
		{
			this.DatasetPath = datasetPath;

			_backgroundWorker = backgroundWorker;

			this.WorkflowParameters = workflowExecutorParameters;
			_workflowParameters = workflowParameters;

			ResultRepository = new TargetedResultRepository();
			InitializeWorkflow();
		}

        public TargetedWorkflowExecutor(WorkflowExecutorBaseParameters parameters, Run run, BackgroundWorker backgroundWorker = null)
        {
            Run = run;

            if (Run != null) DatasetPath = Run.DataSetPath;

            _backgroundWorker = backgroundWorker;

            this.WorkflowParameters = parameters;

            ResultRepository = new TargetedResultRepository();
            InitializeWorkflow();
        }


        public override void InitializeWorkflow()
        {
            if (string.IsNullOrEmpty(ExecutorParameters.ResultsFolder))
            {
                _resultsFolder = RunUtilities.GetDatasetParentFolder(DatasetPath);
            }
            else
            {
                _resultsFolder = getResultsFolder(ExecutorParameters.ResultsFolder);
            }

            if (ExecutorParameters.TargetedAlignmentIsPerformed)
            {
                MassTagsForTargetedAlignment = GetMassTagTargets(ExecutorParameters.TargetsUsedForAlignmentFilePath); 
            }
           

            bool targetsFilePathIsEmpty = (String.IsNullOrEmpty(ExecutorParameters.TargetsFilePath));

            string currentTargetsFilePath;

            if (targetsFilePathIsEmpty)
            {
                currentTargetsFilePath = TryFindTargetsForCurrentDataset();   //check for a _targets file specifically associated with dataset
            }
            else
            {
                currentTargetsFilePath = ExecutorParameters.TargetsFilePath;
            }

            Targets = CreateTargets(ExecutorParameters.TargetType, currentTargetsFilePath);

            Check.Ensure(Targets != null && Targets.TargetList.Count > 0,
                         "Target massTags is empty. Check the path to the massTag data file.");


            if (ExecutorParameters.TargetType == GlobalsWorkFlow.TargetType.LcmsFeature)
            {
                UpdateTargetMissingInfo();
            }

            if (TargetedWorkflow==null)
            {
                if (_workflowParameters == null)
                {
                    _workflowParameters = WorkflowParameters.CreateParameters(ExecutorParameters.WorkflowParameterFile);
                    _workflowParameters.LoadParameters(ExecutorParameters.WorkflowParameterFile);
                }
                TargetedWorkflow = TargetedWorkflow.CreateWorkflow(_workflowParameters);
            }
            else
            {
                _workflowParameters = TargetedWorkflow.WorkflowParameters;
            }
          
        	if (ExecutorParameters.TargetedAlignmentIsPerformed)
            {
                if (string.IsNullOrEmpty(ExecutorParameters.TargetedAlignmentWorkflowParameterFile))
                {
                    throw new FileNotFoundException(
                        "Cannot initialize workflow. TargetedAlignment is requested but TargetedAlignmentWorkflowParameter file is not found. Check path for the 'TargetedAlignmentWorkflowParameterFile' ");
                }


                TargetedAlignmentWorkflowParameters = new TargetedAlignerWorkflowParameters();
                TargetedAlignmentWorkflowParameters.LoadParameters(ExecutorParameters.TargetedAlignmentWorkflowParameterFile);

            }

            
        }



        #endregion

        #region Properties

        public string DatasetPath { get; set; }

        public TargetCollection MassTagsForTargetedAlignment { get; set; }

        public TargetCollection Targets { get; set; }

        public override WorkflowParameters WorkflowParameters
        {
            get
            {
                return ExecutorParameters;
            }
            set
            {
                ExecutorParameters = value as WorkflowExecutorBaseParameters;
            }
        }

        public WorkflowExecutorBaseParameters ExecutorParameters { get; set; }

        public TargetedAlignerWorkflowParameters TargetedAlignmentWorkflowParameters { get; set; }

        //public TargetedAlignerWorkflow TargetedAlignmentWorkflow { get; set; }

        public TargetedWorkflow TargetedWorkflow { get; set; }

        /// <summary>
        /// These are database targets that are used for lookup when working on peak-matched LcmsFeatures
        /// </summary>
        public TargetCollection MassTagsForReference { get; set; }


        public double MsgfFdrScoreCutoff { get; set; }

        //added for sipper compatibility
        public bool RunIsDisposed { get; set; }

        #endregion

     

        #region Public Methods

        protected virtual void UpdateTargetMissingInfo()
        {

            bool canUseReferenceMassTags = MassTagsForReference != null && MassTagsForReference.TargetList.Count > 0;

            List<int> massTagIDsAvailableForLookup = new List<int>();

            if (canUseReferenceMassTags)
            {
                massTagIDsAvailableForLookup = MassTagsForReference.TargetList.Select(p => p.ID).ToList();
            }




            foreach (LcmsFeatureTarget target in Targets.TargetList)
            {
                bool isMissingMonoMass = target.MonoIsotopicMass <= 0;

                if (String.IsNullOrEmpty(target.EmpiricalFormula))
                {
                    if (canUseReferenceMassTags && massTagIDsAvailableForLookup.Contains(target.FeatureToMassTagID))
                    {

                        var mt = MassTagsForReference.TargetList.First(p => p.ID == target.FeatureToMassTagID);

                        //in DMS, Sequest will put an 'X' when it can't differentiate 'I' and 'L'
                        //  see:   \\gigasax\DMS_Parameter_Files\Sequest\sequest_ETD_N14_NE.params
                        //To create the theoretical isotopic profile, we will change the 'X' to 'L'
                        if (mt.Code.Contains("X"))
                        {
                            mt.Code = mt.Code.Replace('X', 'L');
                            mt.EmpiricalFormula = mt.GetEmpiricalFormulaFromTargetCode();
                        }

                        target.Code = mt.Code;
                        target.EmpiricalFormula = mt.EmpiricalFormula;
                    }
                    else if (!String.IsNullOrEmpty(target.Code))
                    {
                        //Create empirical formula based on code. Assume it is an unmodified peptide

                        if (target.Code.Contains("X"))
                        {
                            target.Code = target.Code.Replace('X', 'L');
                        }

                        target.EmpiricalFormula = new PeptideUtils().GetEmpiricalFormulaForPeptideSequence(target.Code);

                    }
                    else
                    {
                        if (isMissingMonoMass)
                        {
                            throw new ApplicationException(
                                "Trying to prepare target list, but Target is missing both the 'Code' and the Monoisotopic Mass. One or the other is needed.");
                        }
                        target.Code = "AVERAGINE";
                        target.EmpiricalFormula =
                            IsotopicDistributionCalculator.GetAveragineFormulaAsString(target.MonoIsotopicMass,false);
                    }
                }


                if (isMissingMonoMass)
                {
                    target.MonoIsotopicMass =
                        EmpiricalFormulaUtilities.GetMonoisotopicMassFromEmpiricalFormula(target.EmpiricalFormula);



                    target.MZ = target.MonoIsotopicMass / target.ChargeState + Globals.PROTON_MASS;
                }



            }
        }




        protected string TryFindTargetsForCurrentDataset()
        {
            string expectedTargetsFileBase = ExecutorParameters.TargetsBaseFolder + Path.DirectorySeparatorChar +
                                             RunUtilities.GetDatasetName(DatasetPath);

            string expectedTargetsFile1 = expectedTargetsFileBase + "_targets.txt";
            string expectedTargetsFile2 = expectedTargetsFileBase + "_LCMSFeatures.txt";
            string expectedTargetsFile3 = expectedTargetsFileBase + "_MSGFplus.tsv";


            if (File.Exists(expectedTargetsFile1))
            {
                return expectedTargetsFile1;
            }

            if (File.Exists(expectedTargetsFile2))
            {
                return expectedTargetsFile2;
            }

            if (File.Exists(expectedTargetsFile3))
            {
                return expectedTargetsFile3;
            }



            return String.Empty;
        }


        public override void Execute()
        {
            try
            {
                SetupLogging();

                SetupAlignment();

                ReportGeneralProgress("Started Processing....");
                ReportGeneralProgress("Dataset = " + DatasetPath);
                ReportGeneralProgress("Parameters:" + "\n" + _workflowParameters.ToStringWithDetails());


                if (!RunIsInitialized)
                {
                    //create Run; load _peaks data; do alignment if desired
                    InitializeRun(DatasetPath);
                }

                ExecutePreProcessingHook();

                ProcessDataset();

                ExecutePostProcessingHook();

                ExportData();



                HandleAlignmentInfoFiles();
                FinalizeRun();



            }
            catch (Exception ex)
            {
                ReportGeneralProgress("--------------------------------------------------------------");
                ReportGeneralProgress("-------------------   ERROR    -------------------------------");
                ReportGeneralProgress("--------------------------------------------------------------");

                try
                {
                    FinalizeRun();

                }
                catch
                {
                }

                ReportGeneralProgress(ex.Message);
                ReportGeneralProgress(ex.StackTrace);


            }

        }

        protected void SetupAlignment()
        {
            if (string.IsNullOrEmpty(ExecutorParameters.AlignmentInfoFolder))
            {
                //
                return;
            }

            if (!Directory.Exists(ExecutorParameters.AlignmentInfoFolder))
            {
                Directory.CreateDirectory(ExecutorParameters.AlignmentInfoFolder);
            }
        }


        protected virtual void SetupLogging()
        {
            string loggingFolder;

            if (string.IsNullOrEmpty(ExecutorParameters.LoggingFolder))
            {
                loggingFolder = RunUtilities.GetDatasetParentFolder(DatasetPath);
            }
            else
            {
                loggingFolder = ExecutorParameters.LoggingFolder;
            }

            try
            {
                if (!Directory.Exists(loggingFolder))
                {
                    Directory.CreateDirectory(loggingFolder);
                }
            }
            catch (Exception ex)
            {
                throw new System.IO.IOException("Trying to set up logging folder but there was a critical error. Details:\n\n" + ex.Message, ex);
            }
           

            _loggingFileName = loggingFolder + "\\" + RunUtilities.GetDatasetName(DatasetPath) + "_log.txt";
        }


        public List<TargetedResultDTO> GetResults()
        {
            return ResultRepository.Results;
        }



        public bool RunIsInitialized
        {
            get
            {
                if (Run == null || Run.ResultCollection.MSPeakResultList.Count == 0)
                {
                    return false;
                }

                if (!DatasetPath.Contains(Run.DataSetPath))
                {
                    return false;
                }

                return true;
            }
        }


        public void ProcessDataset()
        {


            //apply mass calibration and NET alignment from .txt files, if they exist
            PerformAlignment();


            bool runIsNotAligned = (!Run.MassIsAligned && !Run.NETIsAligned);     //if one of these two is aligned, the run is considered to be aligned

            //Perform targeted alignment if 1) run is not aligned  2) parameters permit it
            
            this.TargetedWorkflow.Run = Run;


            ResultRepository.Results.Clear();

            int mtCounter = 0;
            int totalTargets = Targets.TargetList.Count;

            ReportGeneralProgress("Processing...", 0);

            foreach (var massTag in this.Targets.TargetList)
            {
                mtCounter++;

#if DEBUG
				System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

#endif


                Run.CurrentMassTag = massTag;
                try
                {
                    this.TargetedWorkflow.Execute();
                    ResultRepository.AddResult(this.TargetedWorkflow.Result);

                }
                catch (Exception ex)
                {
                    string errorString = "Error on MT\t" + massTag.ID + "\tchargeState\t" + massTag.ChargeState + "\t" + ex.Message + "\t" + ex.StackTrace;
                    ReportGeneralProgress(errorString, mtCounter);

                    throw;
                }

#if DEBUG
                stopwatch.Stop();
                Console.WriteLine(massTag.ID + "\tprocessing time = " + stopwatch.ElapsedMilliseconds);

#endif

                string progressString = "Percent complete = " + ((double)mtCounter / totalTargets * 100.0).ToString("0.0") + "\tTarget " + mtCounter + " of " + totalTargets;


                if (_backgroundWorker != null)
                {
                    if (_backgroundWorker.CancellationPending)
                    {
                        return;
                    }
                }

                ReportProcessingProgress(progressString, mtCounter);


            }

            ReportGeneralProgress("---- PROCESSING COMPLETE ---------------", 100);





        }


        protected virtual void ExportData()
        {
            string outputFileName = GetOutputFileName();
            backupResultsFileIfNecessary(Run.DatasetName, outputFileName);

            TargetedResultToTextExporter exporter = TargetedResultToTextExporter.CreateExporter(this._workflowParameters, outputFileName);
            exporter.ExportResults(ResultRepository.Results);
        }



        /// <summary>
        /// This hook allows inheriting class to execute post processing methods. e.g. see TopDownTargetedWorkflowExecutor
        /// </summary>
        protected virtual void ExecutePostProcessingHook() { }

        /// <summary>
        /// This hook allows inheriting class to execute pre processing methods.
        /// </summary>
        protected virtual void ExecutePreProcessingHook() { }


        protected virtual string GetOutputFileName()
        {
            return _resultsFolder + Path.DirectorySeparatorChar + Run.DatasetName + "_results.txt";
        }

        #endregion

        #region Private Methods
        protected string getResultsFolder(string folder)
        {
            string outputFolder;

            if (string.IsNullOrEmpty(folder))
            {
                outputFolder = DatasetPath + Path.DirectorySeparatorChar + "Results";
            }
            else
            {
                outputFolder = folder;
            }


            DirectoryInfo dirinfo = new DirectoryInfo(outputFolder);

            if (!dirinfo.Exists)
            {
                dirinfo.Create();
            }

            return dirinfo.FullName;

        }

        protected TargetCollection GetMassTagTargets(string massTagFileName)
        {
            return GetMassTagTargets(massTagFileName, new List<int>());
        }

        protected TargetCollection GetMassTagTargets(string massTagFileName, List<int> targetIDsToFilterOn)
        {
            if (String.IsNullOrEmpty(massTagFileName) || !File.Exists(massTagFileName))
            {
                return new TargetCollection();
            }


            if (massTagFileName.ToLower().Contains("_msgfplus.tsv"))
            {
                BasicIqTargetImporter iqTargetImporter = new BasicIqTargetImporter(massTagFileName);
                var iqTargets = iqTargetImporter.Import();

                IqTargetUtilities targetUtilities = new IqTargetUtilities();
                TargetCollection targetCollection = new TargetCollection();
                targetCollection.TargetList = new List<TargetBase>();

                foreach (var iqTarget in iqTargets)
                {
                    if (iqTarget.QualityScore > MsgfFdrScoreCutoff) continue;
                    targetUtilities.UpdateTargetMissingInfo(iqTarget);

                    TargetBase oldStyleTarget = new PeptideTarget();
                    oldStyleTarget.ChargeState = (short)iqTarget.ChargeState;
                    oldStyleTarget.Code = iqTarget.Code;
                    oldStyleTarget.EmpiricalFormula = iqTarget.EmpiricalFormula;
                    oldStyleTarget.ID = iqTarget.ID;
                    oldStyleTarget.MZ = iqTarget.MZTheor;
                    oldStyleTarget.MonoIsotopicMass = iqTarget.MonoMassTheor;
                    oldStyleTarget.ScanLCTarget = iqTarget.ScanLC;
                    oldStyleTarget.NormalizedElutionTime = (float)iqTarget.ElutionTimeTheor;

                    oldStyleTarget.ElutionTimeUnit = Globals.ElutionTimeUnit.ScanNum;
                    targetCollection.TargetList.Add(oldStyleTarget);
                }

                return targetCollection;
            }


            MassTagFromTextFileImporter importer = new MassTagFromTextFileImporter(massTagFileName);
            return importer.Import(targetIDsToFilterOn);
        }

        protected string getLogFileName(string folderPath)
        {
            string logfilename = "";

            DirectoryInfo logfolderPath = new DirectoryInfo(folderPath);

            if (!logfolderPath.Exists) logfolderPath.Create();

            logfilename = logfolderPath.FullName + Path.DirectorySeparatorChar + "logfile_" + DateTime.Now.Year.ToString() + "_" +
                DateTime.Now.Month.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Day.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Ticks.ToString() + ".txt";

            return logfilename;

        }

        protected List<string> getListDatasetPaths(string fileContainingDatasetPaths)
        {
            List<string> datasetPathList = new List<string>();

            using (StreamReader sr = new StreamReader(fileContainingDatasetPaths))
            {

                while (sr.Peek() != -1)
                {
                    datasetPathList.Add(sr.ReadLine());

                }
                sr.Close();
            }

            return datasetPathList;

        }

        protected void ReportGeneralProgress(string generalProgressString, int progressPercent = 0)
        {
            if (_backgroundWorker == null)
            {
                Console.WriteLine(DateTime.Now + "\t" + generalProgressString);
            }
            else
            {
                _progressInfo.ProgressInfoString = generalProgressString;
                _progressInfo.IsGeneralProgress = true;
                _backgroundWorker.ReportProgress(progressPercent, _progressInfo);
            }

            writeToLogFile(DateTime.Now + "\t" + generalProgressString);
        }


        protected void ReportProcessingProgress(string reportString, int progressCounter)
        {

            if (_backgroundWorker == null)
            {
                if (progressCounter % 100 == 0)
                {
                    Console.WriteLine(DateTime.Now + "\t" + reportString);
                }

            }
            else
            {
                int progressPercent = (int)(progressCounter * 100 / (double)Targets.TargetList.Count);

                _progressInfo.ProgressInfoString = reportString;
                _progressInfo.IsGeneralProgress = false;
                _progressInfo.Result = TargetedWorkflow.Result;
                _progressInfo.Time = DateTime.Now;

                _progressInfo.ChromatogramXYData = new Run32.Backend.Data.XYData();
                _progressInfo.ChromatogramXYData.Xvalues = TargetedWorkflow.ChromatogramXYData.Xvalues;
                _progressInfo.ChromatogramXYData.Yvalues = TargetedWorkflow.ChromatogramXYData.Yvalues;

                _progressInfo.MassSpectrumXYData = new Run32.Backend.Data.XYData();
                _progressInfo.MassSpectrumXYData.Xvalues = TargetedWorkflow.MassSpectrumXYData.Xvalues;
                _progressInfo.MassSpectrumXYData.Yvalues = TargetedWorkflow.MassSpectrumXYData.Yvalues;

                _backgroundWorker.ReportProgress(progressPercent, _progressInfo);
            }

            if (progressCounter % 100 == 0)
            {
                writeToLogFile(DateTime.Now + "\t" + reportString);
            }

        }

        protected void writeToLogFile(string stringToWrite)
        {

            if (!string.IsNullOrEmpty(this._loggingFileName))
            {
                using (StreamWriter sw = new StreamWriter(new System.IO.FileStream(this._loggingFileName, System.IO.FileMode.Append,
                              System.IO.FileAccess.Write, System.IO.FileShare.Read)))
                {
                    sw.AutoFlush = true;
                    sw.WriteLine(stringToWrite);
                    sw.Flush();

                }
            }

        }

        protected void HandleAlignmentInfoFiles()
        {
            FileAttributes attr = File.GetAttributes(Run.Filename);


            FileInfo[] datasetRelatedFiles;

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Run.Filename);
                datasetRelatedFiles = dirInfo.GetFiles(Run.DatasetName + "*.txt");



            }
            else
            {
                FileInfo fi = new FileInfo(Run.Filename);
                DirectoryInfo dirInfo = fi.Directory;
                datasetRelatedFiles = dirInfo.GetFiles(Run.DatasetName + "*.txt");

            }

            foreach (var file in datasetRelatedFiles)
            {
                if (file.Name.Contains("_alignedFeatures") || file.Name.Contains("_MZAlignment") || file.Name.Contains("_NETAlignment"))
                {
                    bool allowOverwrite = false;

                    string targetCopiedFilename = ExecutorParameters.AlignmentInfoFolder + Path.DirectorySeparatorChar + file.Name;

                    //upload alignment data only if it doesn't already exist
                    if (!File.Exists(targetCopiedFilename))
                    {
                        file.CopyTo(ExecutorParameters.AlignmentInfoFolder + Path.DirectorySeparatorChar + file.Name, allowOverwrite);
                    }

                    if (this.ExecutorParameters.CopyRawFileLocal)
                    {
                        file.Delete();       //if things were copied locally, we are going to delete anything created. 
                    }

                }

            }


        }

        public void InitializeRun(string dataset)
        {
            string runFilename;


            if (this.ExecutorParameters.CopyRawFileLocal)
            {
                ReportGeneralProgress("Started copying raw data to local folder: " + this.ExecutorParameters.FolderPathForCopiedRawDataset);

                FileAttributes attr = File.GetAttributes(dataset);

                DirectoryInfo sourceDirInfo;
                DirectoryInfo targetDirInfo;
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    sourceDirInfo = new DirectoryInfo(dataset);
                    runFilename = this.ExecutorParameters.FolderPathForCopiedRawDataset + Path.DirectorySeparatorChar + sourceDirInfo.Name;
                    targetDirInfo = new DirectoryInfo(runFilename);
                    FileUtilities.CopyAll(sourceDirInfo, targetDirInfo);
                    ReportGeneralProgress("Copying complete.");
                }
                else
                {
                    FileInfo fileinfo = new FileInfo(dataset);
                    sourceDirInfo = fileinfo.Directory;
                    runFilename = this.ExecutorParameters.FolderPathForCopiedRawDataset + Path.DirectorySeparatorChar + Path.GetFileName(dataset);

                    targetDirInfo = new DirectoryInfo(this.ExecutorParameters.FolderPathForCopiedRawDataset);

                    if (!File.Exists(runFilename))
                    {
                        FileUtilities.CopyAll(fileinfo, targetDirInfo);
                        ReportGeneralProgress("Copying complete.");
                    }
                    else
                    {
                        ReportGeneralProgress("Datafile already exists on local drive. Using existing datafile.");

                    }

                }

            }
            else
            {
                runFilename = dataset;
            }

            //create Run
            RunFactory rf = new RunFactory();
            Run = rf.CreateRun(runFilename);

            bool runInstantiationFailed = (Run == null);
            if (runInstantiationFailed)
            {
                ReportGeneralProgress("Run initialization FAILED. Likely a filename problem. Or missing manufacturer .dlls");
                return;
            }
            else
            {
                ReportGeneralProgress("Run initialized successfully.");
            }


            //Retrieve alignment data if it exists
            CopyAlignmentInfoIfExists();




            //check and load chrom source data (_peaks.txt)
            bool peaksFileExists = CheckForPeaksFile();
            if (!peaksFileExists)
            {
                ReportGeneralProgress("Creating _Peaks.txt file for extracted ion chromatogram (XIC) source data ... takes 1-5 minutes");

                CreatePeaksForChromSourceData();
                ReportGeneralProgress("Done creating _Peaks.txt file");
            }
            else
            {
                ReportGeneralProgress("Using existing _Peaks.txt file");
            }


            ReportGeneralProgress("Peak loading started...");


            string baseFileName;
            baseFileName = this.Run.DataSetPath + "\\" + this.Run.DatasetName;

            string possibleFilename1 = baseFileName + "_peaks.txt";

            if (File.Exists(possibleFilename1))
            {
                //create background worker so that updates don't go out to console.
                //BackgroundWorker bw = new BackgroundWorker();
                //bw.WorkerSupportsCancellation = true;
                //bw.WorkerReportsProgress = true;

                //TODO: keep an eye on errors connected to background worker here.
                PeakImporterFromText peakImporter = new PeakImporterFromText(possibleFilename1, _backgroundWorker);

                peakImporter.ImportPeaks(this.Run.ResultCollection.MSPeakResultList);
            }
            else
            {
                ReportGeneralProgress("CRITICAL FAILURE. Chrom source data (_peaks.txt) file not loaded.");
                return;
            }

			// Grab the primary LC Scan numbers if they are not already filled out
			if(!Run.PrimaryLcScanNumbers.Any())
			{
				Run.PrimaryLcScanNumbers = RunUtilities.FindPrimaryLcScanNumbers(this.Run.ResultCollection.MSPeakResultList);
			}

            ReportGeneralProgress("Peak Loading complete.");
            return;
        }

        private void CopyAlignmentInfoIfExists()
        {
            if (String.IsNullOrEmpty(ExecutorParameters.AlignmentInfoFolder)) return;

            DirectoryInfo dirInfo = new DirectoryInfo(ExecutorParameters.AlignmentInfoFolder);

            if (dirInfo.Exists)
            {

                FileInfo[] datasetRelatedFiles = dirInfo.GetFiles(Run.DatasetName + "*.txt");

                foreach (var file in datasetRelatedFiles)
                {
                    if (file.Name.ToLower() == Run.DatasetName.ToLower() + "_mzalignment.txt" || file.Name.ToLower() == Run.DatasetName.ToLower() + "_netalignment.txt")
                    {
                        string targetFileName = Run.DataSetPath + Path.DirectorySeparatorChar + file.Name;
                        if (!File.Exists(targetFileName))
                        {
                            file.CopyTo(Run.DataSetPath + Path.DirectorySeparatorChar + file.Name, true);
                        }
                    }


                }


            }
        }

        protected void PerformAlignment()
        {
//            if (string.IsNullOrEmpty(ExecutorParameters.AlignmentInfoFolder))
//            {
//                RunUtilities.AlignRunUsingAlignmentInfoInFiles(Run);
//            }
//            else
//            {
//                RunUtilities.AlignRunUsingAlignmentInfoInFiles(Run, ExecutorParameters.AlignmentInfoFolder);
//            }





            if (Run.MassIsAligned)
            {
                ReportGeneralProgress("Run has been mass aligned");
            }
            else
            {
                ReportGeneralProgress("FYI - Run has NOT been mass aligned.");
            }

            if (Run.NETIsAligned)
            {
                ReportGeneralProgress("Run has been NET aligned using info in either the _NETAlignment.txt file or the _UMCs.txt file");
            }
            else
            {
                ReportGeneralProgress("Warning - Run has NOT been NET aligned.");
            }
        }

        private void CreatePeaksForChromSourceData()
        {
            PeakDetectAndExportWorkflowParameters parameters = new PeakDetectAndExportWorkflowParameters();
            TargetedWorkflowParameters deconParam = (TargetedWorkflowParameters)this._workflowParameters;

            parameters.PeakBR = deconParam.ChromGenSourceDataPeakBR;
            parameters.PeakFitType = Globals.PeakFitType.QUADRATIC;
            parameters.SigNoiseThreshold = deconParam.ChromGenSourceDataSigNoise;
        	parameters.ProcessMSMS = deconParam.ProcessMsMs;
            PeakDetectAndExportWorkflow peakCreator = new PeakDetectAndExportWorkflow(this.Run, parameters, _backgroundWorker);
            peakCreator.Execute();
        }

        private bool CheckForPeaksFile()
        {
            string baseFileName;
            baseFileName = this.Run.DataSetPath + "\\" + this.Run.DatasetName;

            string possibleFilename1 = baseFileName + "_peaks.txt";

            if (File.Exists(possibleFilename1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected TargetCollection CreateTargets(GlobalsWorkFlow.TargetType targetType, string targetFilePath)
        {
            if (string.IsNullOrEmpty(targetFilePath)) return null;

            switch (targetType)
            {
                case GlobalsWorkFlow.TargetType.LcmsFeature:
                    return GetLcmsFeatureTargets(targetFilePath);

                case GlobalsWorkFlow.TargetType.DatabaseTarget:
                    return GetMassTagTargets(targetFilePath);

                default:
                    throw new ArgumentOutOfRangeException("targetType");
            }
        }


        protected virtual TargetCollection GetLcmsFeatureTargets(string targetsFilePath)
        {
            if (targetsFilePath.ToLower().Contains("_msgfplus.tsv"))
            {
                BasicIqTargetImporter iqTargetImporter = new BasicIqTargetImporter(targetsFilePath);
                var iqTargets = iqTargetImporter.Import();

                IqTargetUtilities targetUtilities = new IqTargetUtilities();
                TargetCollection targetCollection = new TargetCollection();
                targetCollection.TargetList = new List<TargetBase>();

                foreach (var iqTarget in iqTargets)
                {
                    if (iqTarget.QualityScore > MsgfFdrScoreCutoff) continue;
                    targetUtilities.UpdateTargetMissingInfo(iqTarget);

                    TargetBase oldStyleTarget = new LcmsFeatureTarget();
                    oldStyleTarget.ChargeState = (short)iqTarget.ChargeState;
                    oldStyleTarget.Code = iqTarget.Code;
                    oldStyleTarget.EmpiricalFormula = iqTarget.EmpiricalFormula;
                    oldStyleTarget.ID = iqTarget.ID;
                    oldStyleTarget.MZ = iqTarget.MZTheor;
                    oldStyleTarget.MonoIsotopicMass = iqTarget.MonoMassTheor;
                    oldStyleTarget.ScanLCTarget = iqTarget.ScanLC;
                    oldStyleTarget.NormalizedElutionTime = (float)iqTarget.ElutionTimeTheor;

                    oldStyleTarget.ElutionTimeUnit = Globals.ElutionTimeUnit.ScanNum;
                    targetCollection.TargetList.Add(oldStyleTarget);
                }

                return targetCollection;
            }
            
            LcmsTargetFromFeaturesFileImporter importer =
               new LcmsTargetFromFeaturesFileImporter(targetsFilePath);

            var lcmsTargetCollection = importer.Import();
            return lcmsTargetCollection;
        }

        protected virtual TargetedResultToTextExporter createExporter(string outputFileName)
        {
            throw new NotImplementedException();
        }


        private void cleanUpLocalFiles()
        {
            throw new NotImplementedException();
        }

        protected void FinalizeRun()
        {

            string runfileName = Run.Filename;
            string datasetName = Run.DatasetName;

            Run.Close();
            Run = null;
            GC.Collect();



            if (this.ExecutorParameters.CopyRawFileLocal && this.ExecutorParameters.DeleteLocalDatasetAfterProcessing)
            {
                FileAttributes attr = File.GetAttributes(runfileName);

                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(runfileName);
                    dirInfo.Delete(true);
                }
                else
                {
                    FileInfo fileinfo = new FileInfo(runfileName);

                    string fileSuffix = fileinfo.Extension;

                    DirectoryInfo dirInfo = fileinfo.Directory;

                    string expectedPeaksFile = dirInfo.FullName + Path.DirectorySeparatorChar + datasetName + "_peaks.txt";

                    if (File.Exists(expectedPeaksFile))
                    {
                        //File.Delete(expectedPeaksFile);
                    }

                    FileInfo[] allRawDataFiles = dirInfo.GetFiles("*" + fileSuffix);
                    if (allRawDataFiles.Count() > 35)
                    {
                        foreach (var file in allRawDataFiles)
                        {
                            try
                            {
                                file.Delete();
                            }
                            catch (Exception)
                            {

                            }


                        }
                    }


                }




            }




        }

        protected void backupResultsFileIfNecessary(string datasetName, string outputFileName)
        {
            FileInfo outputFileInfo = new FileInfo(outputFileName);


            if (outputFileInfo.Exists)
            {
                string backupFolder = this._resultsFolder + Path.DirectorySeparatorChar + "Backup";
                DirectoryInfo backupFolderInfo = new DirectoryInfo(backupFolder);

                if (!backupFolderInfo.Exists)
                {
                    backupFolderInfo.Create();
                }

                string backupFilename = backupFolderInfo.FullName + Path.DirectorySeparatorChar + datasetName + "_results.txt";
                outputFileInfo.CopyTo(backupFilename, true);

                outputFileInfo.Delete();

            }
        }
        #endregion

    }
}
