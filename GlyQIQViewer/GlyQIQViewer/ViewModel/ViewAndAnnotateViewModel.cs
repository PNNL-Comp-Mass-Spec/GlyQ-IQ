using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using IQ.Backend.Core;
using IQ.Backend.ProcessingTasks;
using IQ.Backend.ProcessingTasks.MSGenerators;
using IQ.Workflows;
using IQ.Workflows.FileIO;
using IQ.Workflows.FileIO.DTO;
using IQ.Workflows.FileIO.Importers;
using IQ.Workflows.WorkFlowParameters;
using IQ.Workflows.WorkFlowPile;
using IQGlyQ;
using OxyPlot;
using OxyPlot.Axes;
using Run32.Backend;
using Run32.Backend.Core;
using Run32.Backend.Runs;
using Run32.Utilities;
using Sipper.Model;
using Run32.Backend.Data;
using TheorXYDataCalculationUtilities = DeconTools.Backend.Utilities.IsotopeDistributionCalculation.TheorXYDataCalculationUtilities;

namespace Sipper.ViewModel
{
    public delegate void AllDataLoadedAndReadyEventHandler(object sender, EventArgs e);

    public class ViewAndAnnotateViewModel : ViewModelBase
    {
        private const double DefaultMsPeakWidth = 0.01;
        private readonly ScanSetFactory _scanSetFactory = new ScanSetFactory();
        private int _currentLcScan;
        private readonly TargetedResultRepository _resultRepositorySource;
        private BackgroundWorker _backgroundWorker;
        private string _peaksFilename;
        private MSGenerator _msGenerator;
        private string GlyQIQParameterFileName;
        
        #region Constructors

        public ViewAndAnnotateViewModel()
        {
            FileInputs = new FileInputsViewModel();
            _resultRepositorySource = new TargetedResultRepository();
            Results = new ObservableCollection<SipperLcmsFeatureTargetedResultDTO>();
            var workflowParameters = new SipperTargetedWorkflowParameters();
            //Workflow = new SipperTargetedWorkflow(workflowParameters);
            Workflow = new GlyQIQTargetedWorkflow(workflowParameters, GlyQIQParameterFileName);
            ChromGraphXWindowWidth = 600;
            MsGraphMinX = 400;
            MsGraphMaxX = 1400;

            ShowFileAndResultsList = true;
            MassSpecVisibleWindowWidth = 15;


        }

        public ViewAndAnnotateViewModel(FileInputsInfo fileInputs)
            : this()
        {
            FileInputs = new FileInputsViewModel(fileInputs);
            FileInputs.PropertyChanged += FileInputsPropertyChanged;


            LoadParameters();

            //load ms workflowparameters
            LoadMSParameters();

            GlyQIQParameterFileName = fileInputs.MsWorkflowParameterFilePath;

            UpdateGraphRelatedProperties();

        }

        public ViewAndAnnotateViewModel(TargetedResultRepository resultRepository, FileInputsInfo fileInputs = null)
            : this(fileInputs)
        {
            _resultRepositorySource = resultRepository;
            SetResults(_resultRepositorySource.Results);

            if (IsAllDataReady)
            {
                OnAllDataLoadedAndReady(new EventArgs());
            }

        }
        
        #endregion

        #region Event-related

        public event AllDataLoadedAndReadyEventHandler AllDataLoadedAndReadyEvent;

        public void OnAllDataLoadedAndReady(EventArgs e)
        {
            AllDataLoadedAndReadyEventHandler handler = AllDataLoadedAndReadyEvent;
            if (handler != null) handler(this, e);
        }
        
        private void OnYAxisChange(object sender, AxisChangedEventArgs e)
        {
            LinearAxis yAxis = sender as LinearAxis;

            // No need to update anything if the minimum is already <= 0
            if (yAxis.ActualMinimum <= 0) return;

            // Set the minimum to 0 and refresh the plot
            yAxis.Zoom(0, yAxis.ActualMaximum);
            yAxis.PlotModel.RefreshPlot(true);
        }

        void FileInputsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ParameterFilePath":
                    LoadParameters();
                    break;
                case "MSParameterFilePath":
                    LoadMSParameters();
                    break;
                case "DatasetPath":
                    LoadRun(FileInputs.DatasetPath);
                    break;
                case "TargetsFilePath":
                    LoadResults(FileInputs.TargetsFilePath);
                    break;
            }

            if (IsAllDataReady)
            {
                OnAllDataLoadedAndReady(new EventArgs());
            }




        }


        #endregion

        #region Properties

        private Run _run;
        public Run Run
        {
            get { return _run; }
            set
            {
                _run = value;
                Workflow.Run = _run;
                OnPropertyChanged("DatasetFilePath");
                OnPropertyChanged("RunStatusText");

                if (IsAllDataReady)
                {
                    OnAllDataLoadedAndReady(new EventArgs());
                }
            }
        }

        public string RunStatusText
        {
            get
            {
                if (Run == null)
                {
                    return "Not loaded.";
                }
                else
                {
                    return "LOADED.";
                }
            }
        }


        private double _massSpecVisibleWindowWidth;
        public double MassSpecVisibleWindowWidth
        {
            get { return _massSpecVisibleWindowWidth; }
            set
            {
                _massSpecVisibleWindowWidth = value;
                OnPropertyChanged("MassSpecVisibleWindowWidth");
            }
        }

        public FileInputsViewModel FileInputs { get; private set; }

        public ValidationCode CurrentResultValidationCode
        {
            get
            {
                if (_currentResult == null)
                {
                    return ValidationCode.None;
                }
                else
                {
                    return _currentResult.ValidationCode;
                }
            }
            set
            {
                if (_currentResult == null)
                {
                    return;
                }

                _currentResult.ValidationCode = value;
                OnPropertyChanged("CurrentResultValidationCode");
            }
        }

        public string DatasetFilePath
        {
            get
            {
                return FileInputs.DatasetPath;
            }

            set
            {
                FileInputs.DatasetPath = value;
            }

        }

        public ObservableCollection<SipperLcmsFeatureTargetedResultDTO> Results { get; set; }

        private PlotModel _theorIsoPlot;
        public PlotModel TheorIsoPlot
        {
            get { return _theorIsoPlot; }
            set
            {
                _theorIsoPlot = value;
                OnPropertyChanged("TheorIsoPlot");
            }
        }

        private PlotModel _observedIsoPlot;
        public PlotModel ObservedIsoPlot
        {
            get { return _observedIsoPlot; }
            set
            {
                _observedIsoPlot = value;
                OnPropertyChanged("ObservedIsoPlot");
            }
        }

        private PlotModel _chromatogramPlot;
        public PlotModel ChromatogramPlot
        {
            get { return _chromatogramPlot; }
            set
            {
                _chromatogramPlot = value;
                OnPropertyChanged("ChromatogramPlot");
            }
        }

        private PlotModel _chromCorrelationPlot;
        public PlotModel ChromCorrelationPlot
        {
            get
            {
                return _chromCorrelationPlot;
            }
            set
            {
                _chromCorrelationPlot = value;
                OnPropertyChanged("ChromCorrelationPlot");
            }
        }

        private SipperLcmsFeatureTargetedResultDTO _currentResult;
        public SipperLcmsFeatureTargetedResultDTO CurrentResult
        {
            get { return _currentResult; }
            set
            {
                //check if we moved on to a different dataset

                _currentResult = value;
                OnPropertyChanged("CurrentResult");
                OnPropertyChanged("CurrentResultValidationCode");
            }
        }

        public string ChromTitleText
        {
            get
            {
                if (CurrentResult == null) return String.Empty;

                return "XIC m/z " + CurrentResult.MonoMZ.ToString("0.0000");

            }
        }

        private string _targetsFileStatusText;
        public string TargetsFileStatusText
        {
            get { return _targetsFileStatusText; }
            set
            {
                if (value == TargetsFileStatusText) return;
                _targetsFileStatusText = value;
                OnPropertyChanged("TargetsFileStatusText");
            }
        }

        private string _targetFilterString;
        public string TargetFilterString
        {
            get { return _targetFilterString; }
            set
            {
                _targetFilterString = value;

                FilterTargets();
                OnPropertyChanged("TargetFilterString");
            }
        }

        private string _parameterFileStatusText;
        public string ParameterFileStatusText
        {
            get { return _parameterFileStatusText; }
            set
            {
                if (value == ParameterFileStatusText) return;
                _parameterFileStatusText = value;
                OnPropertyChanged("ParameterFileStatusText");

            }
        }

        private string _MSparameterFileStatusText;
        public string MSParameterFileStatusText
        {
            get { return _MSparameterFileStatusText; }
            set
            {
                if (value == MSParameterFileStatusText) return;
                _MSparameterFileStatusText = value;
                OnPropertyChanged("MSParameterFileStatusText");

            }
        }

        //private SipperTargetedWorkflow _workflow;
        //public SipperTargetedWorkflow Workflow

        private GlyQIQTargetedWorkflow _workflow;
        public GlyQIQTargetedWorkflow Workflow
        {
            get { return _workflow; }
            set
            {
                _workflow = value;
                OnPropertyChanged("Workflow");
            }
        }

        private string _generalStatusMessage;
        public string GeneralStatusMessage
        {
            get { return _generalStatusMessage; }
            set
            {
                _generalStatusMessage = value;
                { OnPropertyChanged("GeneralStatusMessage"); }

            }
        }

        public string WorkflowStatusMessage
        {
            get
            {
                if (Workflow != null)
                {
                    return Workflow.WorkflowStatusMessage;
                }
                return String.Empty;
            }

        }

        public string PeptideSequence
        {
            get
            {
                if (Workflow != null && Workflow.Result != null)
                {
                    return Workflow.Result.Target.Code;
                }
                return String.Empty;

            }




        }

        private XYData _chromXyData;
        public XYData ChromXyData
        {
            get { return _chromXyData; }
            set
            {
                _chromXyData = value;
                OnPropertyChanged("ChromXyData");
            }
        }

        private XYData _massSpecXyData;
        public XYData MassSpecXyData
        {
            get { return _massSpecXyData; }
            set
            {
                _massSpecXyData = value;
                OnPropertyChanged("MassSpecXyData");
            }
        }


        private XYData _subtractedMassSpecXyData;
        public XYData SubtractedMassSpecXYData
        {
            get { return _subtractedMassSpecXyData; }
            set { _subtractedMassSpecXyData = value; }
        }
        
        private XYData _chromCorrXyData;
        public XYData ChromCorrXYData
        {
            get { return _chromCorrXyData; }
            set { _chromCorrXyData = value; }
        }

        public XYData RatioLogsXyData { get; set; }

        public XYData RatioXyData { get; set; }

        public double ChromGraphMaxX { get; set; }

        public double ChromGraphMinX { get; set; }

        public double ChromGraphXWindowWidth { get; set; }

        private double _msGraphMaxX;
        public double MsGraphMaxX
        {
            get { return _msGraphMaxX; }
            set
            {
                _msGraphMaxX = value;
                OnPropertyChanged("MsGraphMaxX");
            }
        }

        private double _msGraphMinX;
        public double MsGraphMinX
        {
            get { return _msGraphMinX; }
            set
            {
                _msGraphMinX = value;
                OnPropertyChanged("MsGraphMinX");
            }
        }

        public float MsGraphMaxY { get; set; }

        public int MinLcScan
        {
            get
            {
                if (Run == null) return 1;
                return Run.MinLCScan;
            }

        }

        public int MaxLcScan
        {
            get
            {
                if (Run == null) return 1;
                return Run.MaxLCScan;
            }

        }

        public int CurrentLcScan
        {
            get { return _currentLcScan; }
            set
            {
                _currentLcScan = value;
                OnPropertyChanged("CurrentLcScan");
            }
        }


        public XYData LabelDistributionXyData { get; set; }

        private XYData _theorProfileXyData;
        public XYData TheorProfileXyData
        {
            get { return _theorProfileXyData; }
            set
            {
                _theorProfileXyData = value;
                OnPropertyChanged("TheorProfileXyData");
            }
        }

        private int _percentProgress;
        /// <summary>
        /// Data for peak loading progress bar
        /// </summary>
        public int PercentProgress
        {
            get { return _percentProgress; }
            set
            {
                _percentProgress = value;
                OnPropertyChanged("PercentProgress");
            }
        }

        private bool _showFileAndResultsList;
        public bool ShowFileAndResultsList
        {
            get { return _showFileAndResultsList; }
            set { _showFileAndResultsList = value;
            OnPropertyChanged("ShowFileAndResultsList");
            }
        }

        protected bool IsParametersLoaded { get; set; }
        protected bool IsMSParametersLoaded { get; set; }
        protected bool IsRunLoaded
        {
            get
            {
                return Run != null
                    && Run.PeakList != null && Run.PeakList.Count > 0;
            }
        }

        protected bool IsResultsLoaded
        {
            get { return Results != null && Results.Count > 0; }
        }

        public bool IsAllDataReady
        {
            get { return (IsParametersLoaded && IsResultsLoaded && IsRunLoaded && IsMSParametersLoaded); }
        }


        #endregion

        #region Public Methods
      

        public void NavigateToNextMs1MassSpectrum(Globals.ScanSelectionMode selectionMode = Globals.ScanSelectionMode.ASCENDING)
        {
            if (Run == null) return;

            if (Workflow == null) return;

            var workflowParameters = (TargetedWorkflowParameters)Workflow.WorkflowParameters;

            int nextPossibleMs;
            if (selectionMode == Globals.ScanSelectionMode.DESCENDING)
            {
                nextPossibleMs = CurrentLcScan - 1;
            }
            else
            {
                nextPossibleMs = CurrentLcScan + 1;
            }

            CurrentLcScan = Run.GetClosestMSScan(nextPossibleMs, selectionMode);

            if (_msGenerator == null)
            {
                _msGenerator = MSGeneratorFactory.CreateMSGenerator(Run.MSFileType);

            }


            var currentScanSet = _scanSetFactory.CreateScanSet(Run, CurrentLcScan, workflowParameters.NumMSScansToSum);
            MassSpecXyData = _msGenerator.GenerateMS(Run, currentScanSet);

            if (MassSpecXyData!=null)
            {
                MassSpecXyData = MassSpecXyData.TrimData(MsGraphMinX - 20, MsGraphMaxX + 20);
            }
            
            CreateMsPlotForScanByScanAnalysis(currentScanSet);
        }

        public void ExecuteWorkflow()
        {
            if (Run == null) return;

            GeneralStatusMessage = ".......Pre Workflow";

            SetCurrentWorkflowTarget(CurrentResult);

            

            Workflow.Execute();

            MsGraphMinX = Workflow.Result.Target.MZ - 1.75;
            MsGraphMaxX = Workflow.Result.Target.MZ + MassSpecVisibleWindowWidth;

            CreateChromatogramPlot();
            CreateTheorIsotopicProfilePlot();
            CreateChromCorrPlot();
            CreateObservedIsotopicProfilePlot();

            if (Workflow.Success)
            {
                TargetedWorkflowParameters workflowParameters = (TargetedWorkflowParameters)Workflow.WorkflowParameters;
                CurrentLcScan = Workflow.Result.GetScanNum();
            }



            //UpdateGraphRelatedProperties();



            OnPropertyChanged("WorkflowStatusMessage");
            OnPropertyChanged("PeptideSequence");
        }

        public void LoadRun(string fileOrFolderPath)
        {
            if (Run != null)
            {
                Run.Close();
                Run = null;
                GC.Collect();
            }

            try
            {
                Run = new RunFactory().CreateRun(fileOrFolderPath);
            }
            catch (Exception ex)
            {
                GeneralStatusMessage = ex.Message;
            }

            OnPropertyChanged("RunStatusText");
            OnPropertyChanged("DatasetFilePath");

            if (Run != null)
            {
                LoadPeaksUsingBackgroundWorker();
                FileInputs.DatasetParentFolder = Run.DataSetPath;
            }
        }

        private void LoadPeaksUsingBackgroundWorker()
        {
            if (Run == null) return;

            if (_backgroundWorker != null && _backgroundWorker.IsBusy)
            {
                GeneralStatusMessage = "Patience please. Already busy...";
                return;

            }

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorkerCompleted;
            _backgroundWorker.ProgressChanged += BackgroundWorkerProgressChanged;
            _backgroundWorker.DoWork += BackgroundWorkerDoWork;

            _backgroundWorker.RunWorkerAsync();


        }

        private void LoadPeaks()
        {
            try
            {
                _peaksFilename = this.Run.DataSetPath + "\\" + this.Run.DatasetName + "_peaks.txt";

                if (!File.Exists(_peaksFilename))
                {
                    GeneralStatusMessage =
                        "Creating chromatogram data (_peaks.txt file); this is only done once. It takes 1 - 5 min .......";
                    var deconParam = (TargetedWorkflowParameters)Workflow.WorkflowParameters;

                    var peakCreationParameters = new PeakDetectAndExportWorkflowParameters();
                    peakCreationParameters.PeakBR = deconParam.ChromGenSourceDataPeakBR;
                    peakCreationParameters.PeakFitType = Globals.PeakFitType.QUADRATIC;
                    peakCreationParameters.SigNoiseThreshold = deconParam.ChromGenSourceDataSigNoise;

                    //SK added because PeakDetectAndExportWorkflow likely lives somewhereelse
                    peakCreationParameters.LCScanMin = Run.MinLCScan;
                    peakCreationParameters.LCScanMax = Run.MaxLCScan;

                    var peakCreator = new PeakDetectAndExportWorkflow(Run, peakCreationParameters, _backgroundWorker);
                    peakCreator.Execute();
                }
            }
            catch (Exception ex)
            {
                GeneralStatusMessage = ex.Message;
                return;
            }

            GeneralStatusMessage = "Loading chromatogram data (_peaks.txt file) .......";
            try
            {
                PeakImporterFromText peakImporter = new PeakImporterFromText(_peaksFilename, _backgroundWorker);
                peakImporter.ImportPeaks(this.Run.ResultCollection.MSPeakResultList);
            }
            catch (Exception ex)
            {
                GeneralStatusMessage = ex.Message;
                return;
                //throw new ApplicationException("Peaks failed to load. Maybe the details below will help... \n\n" + ex.Message + "\nStacktrace: " + ex.StackTrace, ex);
            }

            if (Run.ResultCollection.MSPeakResultList != null && Run.ResultCollection.MSPeakResultList.Count > 0)
            {
                int numPeaksLoaded = Run.ResultCollection.MSPeakResultList.Count;
                GeneralStatusMessage = "Chromatogram data LOADED. (# peaks= " + numPeaksLoaded + ")";
            }
            else
            {
                GeneralStatusMessage = "No Chromatogram data!!! Check your _peaks.txt file for correct format.";
            }

        }

        void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;

            LoadPeaks();


            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void BackgroundWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                GeneralStatusMessage = "Cancelled";
            }
            else if (e.Error != null)
            {
                GeneralStatusMessage = "Error loading peaks. Contact a good friend.";
            }
            else
            {
                PercentProgress = 100;
            }
        }

        private void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PercentProgress = e.ProgressPercentage;
        }
  
        public void LoadParameters()
        {
            IsParametersLoaded = false;

            if (String.IsNullOrEmpty(FileInputs.ParameterFilePath))
            {
                ParameterFileStatusText = "None loaded; using defaults";

                Workflow.WorkflowParameters = new SipperTargetedWorkflowParameters();

            }
            else
            {
                FileInfo fileInfo = new FileInfo(FileInputs.ParameterFilePath);

                if (fileInfo.Exists)
                {
                    Workflow.WorkflowParameters.LoadParameters(FileInputs.ParameterFilePath);
                    ParameterFileStatusText = fileInfo.Name + " LOADED";



                    IsParametersLoaded = true;
                }
                else
                {
                    Workflow.WorkflowParameters = new SipperTargetedWorkflowParameters();
                    ParameterFileStatusText = "None loaded; using defaults";
                }


            }

            Workflow.IsWorkflowInitialized = false;    //important... forces workflow to be reinitialized with new parameters




        }

        public void LoadMSParameters()
        {
            IsMSParametersLoaded = false;

            if (String.IsNullOrEmpty(FileInputs.MsParameterFilePath))
            {
                MSParameterFileStatusText = "None loaded; using defaults";

                Workflow.ProcessingParameters = new FragmentedTargetedPeakProcessingParameters();

            }
            else
            {
                FileInfo fileInfo = new FileInfo(FileInputs.MsParameterFilePath);

                if (fileInfo.Exists)
                {
                    if (Workflow.ProcessingParameters == null)
                    {
                        Workflow.ProcessingParameters = new FragmentedTargetedPeakProcessingParameters();
                    }

                    Workflow.ProcessingParameters.SetParameters(FileInputs.MsParameterFilePath);
                    
                    MSParameterFileStatusText = fileInfo.Name + " LOADED";



                    IsMSParametersLoaded = true;
                }
                else
                {
                    Workflow.ProcessingParameters = new FragmentedTargetedPeakProcessingParameters();
                    MSParameterFileStatusText = "None loaded; using defaults";
                }


            }

            Workflow.IsWorkflowInitialized = false;    //important... forces workflow to be reinitialized with new parameters




        }

        public void LoadResults(string resultFile)
        {

            _resultRepositorySource.Results.Clear();

            FileInfo fileInfo = new FileInfo(resultFile);

            if (fileInfo.Exists)
            {
                SipperResultFromTextImporter importer = new SipperResultFromTextImporter(resultFile);
                var tempResults = importer.Import();

                _resultRepositorySource.Results.AddRange(tempResults.Results);
            }

            SetResults(_resultRepositorySource.Results);


        }
        
        public void SaveResults()
        {
            try
            {
                var exporter = new SipperResultToLcmsFeatureExporter(FileInputs.ResultsSaveFilePath);
                exporter.ExportResults(Results);
            }
            catch (Exception ex)
            {
                GeneralStatusMessage = "Error saving results. Error message: " + ex.Message;
                throw;
            }

            GeneralStatusMessage = "Results saved to: " + Path.GetFileName(FileInputs.ResultsSaveFilePath);


        }

        public void CopyMsDataToClipboard()
        {
            if (MassSpecXyData == null || MassSpecXyData.Xvalues == null || MassSpecXyData.Xvalues.Length == 0) return;
            CopyXyDataToClipboard(MassSpecXyData.Xvalues, MassSpecXyData.Yvalues);
        }

        public void CopyChromatogramToClipboard()
        {
            if (ChromXyData == null || ChromXyData.Xvalues == null || ChromXyData.Xvalues.Length == 0) return;
            CopyXyDataToClipboard(ChromXyData.Xvalues, ChromXyData.Yvalues);
        }

        public void CopyTheorMsToClipboard()
        {
            if (TheorProfileXyData == null || TheorProfileXyData.Xvalues == null || TheorProfileXyData.Xvalues.Length == 0) return;
            CopyXyDataToClipboard(TheorProfileXyData.Xvalues, TheorProfileXyData.Yvalues);
        }

        #endregion

        #region Private Methods

        private void CreateChromatogramPlot()
        {
            var centerScan = Workflow.Result.Target.ScanLCTarget;
            ChromGraphMinX = centerScan - ChromGraphXWindowWidth / 2;
            ChromGraphMaxX = centerScan + ChromGraphXWindowWidth / 2;

            XYData xydata = new XYData();
            if (Workflow.ChromatogramXYData == null)
            {
                xydata.Xvalues = Workflow.ChromatogramXYData == null ? new double[] { 1, Run.MaxLCScan } : Workflow.ChromatogramXYData.Xvalues;
                xydata.Yvalues = Workflow.ChromatogramXYData == null ? new double[] { 0, 0 } : Workflow.ChromatogramXYData.Yvalues;
            }
            else
            {
                xydata.Xvalues = Workflow.ChromatogramXYData.Xvalues;
                xydata.Yvalues = Workflow.ChromatogramXYData.Yvalues;
            }

            string graphTitle = "TargetID=" + Workflow.Result.Target.ID + "; m/z " +
                                  Workflow.Result.Target.MZ.ToString("0.0000") + "; z=" +
                                  Workflow.Result.Target.ChargeState;


            PlotModel plotModel = new PlotModel(graphTitle);
            plotModel.TitleFontSize = 11;
            plotModel.Padding = new OxyThickness(0);
            plotModel.PlotMargins = new OxyThickness(0);
            plotModel.PlotAreaBorderThickness = 0;

            var series = new OxyPlot.Series.LineSeries();
            series.MarkerSize = 1;
            series.Color = OxyColors.Black;
            for (int i = 0; i < xydata.Xvalues.Length; i++)
            {
                series.Points.Add(new DataPoint(xydata.Xvalues[i], xydata.Yvalues[i]));
            }

            var xAxis = new LinearAxis(AxisPosition.Bottom, "scan");
            xAxis.Minimum = ChromGraphMinX;
            xAxis.Maximum = ChromGraphMaxX;
            
            var yAxis = new LinearAxis(AxisPosition.Left, "Intensity");
            yAxis.Minimum = 0;
            yAxis.AbsoluteMinimum = 0;


            var maxY = xydata.getMaxY();
            yAxis.Maximum = maxY + maxY * 0.05;
            yAxis.AxisChanged += OnYAxisChange;

            xAxis.AxislineStyle = LineStyle.Solid;
            xAxis.AxislineThickness = 1;
            yAxis.AxislineStyle = LineStyle.Solid;
            yAxis.AxislineThickness = 1;
            yAxis.FontSize = 12;

            plotModel.Series.Add(series);
            plotModel.Axes.Add(yAxis);
            plotModel.Axes.Add(xAxis);

            ChromatogramPlot = plotModel;
        }

        private void CreateMsPlotForScanByScanAnalysis(ScanSet scanSet)
        {
            XYData xydata = new XYData();
            xydata.Xvalues = MassSpecXyData == null ? new double[] { 400, 1500 } : MassSpecXyData.Xvalues;
            xydata.Yvalues = MassSpecXyData == null ? new double[] { 0, 0 } : MassSpecXyData.Yvalues;

            string msGraphTitle = "Observed MS - Scan: " + scanSet;

            MsGraphMaxY = (float)xydata.getMaxY(MsGraphMinX, MsGraphMaxX);


            PlotModel plotModel = new PlotModel(msGraphTitle);
            plotModel.TitleFontSize = 11;
            plotModel.Padding = new OxyThickness(0);
            plotModel.PlotMargins = new OxyThickness(0);
            plotModel.PlotAreaBorderThickness = 0;



            var series = new OxyPlot.Series.LineSeries();
            series.MarkerSize = 1;
            series.Color = OxyColors.Black;
            for (int i = 0; i < xydata.Xvalues.Length; i++)
            {
                series.Points.Add(new DataPoint(xydata.Xvalues[i], xydata.Yvalues[i]));
            }

            var xAxis = new LinearAxis(AxisPosition.Bottom, "m/z");
            xAxis.Minimum = MsGraphMinX;
            xAxis.Maximum = MsGraphMaxX;
            
            var yAxis = new LinearAxis(AxisPosition.Left, "Intensity");
            yAxis.Minimum = 0;
            yAxis.AbsoluteMinimum = 0;
            yAxis.Maximum = MsGraphMaxY + MsGraphMaxY * 0.05;
            //yAxis.Maximum = maxIntensity + (maxIntensity * .05);
            //yAxis.AbsoluteMaximum = maxIntensity + (maxIntensity * .05);
            yAxis.AxisChanged += OnYAxisChange;
            yAxis.StringFormat = "0.0E0";
            yAxis.FontSize = 12;


            xAxis.AxislineStyle = LineStyle.Solid;
            xAxis.AxislineThickness = 1;
            yAxis.AxislineStyle = LineStyle.Solid;
            yAxis.AxislineThickness = 1;

            plotModel.Series.Add(series);
            plotModel.Axes.Add(yAxis);
            plotModel.Axes.Add(xAxis);

            ObservedIsoPlot = plotModel;


        }

        private void CreateObservedIsotopicProfilePlot()
        {
            XYData xydata = new XYData();

            if (Workflow.MassSpectrumXYData == null)
            {
                xydata.Xvalues = Workflow.MassSpectrumXYData == null ? new double[] { 400, 1500 } : Workflow.MassSpectrumXYData.Xvalues;
                xydata.Yvalues = Workflow.MassSpectrumXYData == null ? new double[] { 0, 0 } : Workflow.MassSpectrumXYData.Yvalues;
            }
            else
            {
                xydata.Xvalues = Workflow.MassSpectrumXYData.Xvalues;
                xydata.Yvalues = Workflow.MassSpectrumXYData.Yvalues;

                xydata = xydata.TrimData(Workflow.Result.Target.MZ - 100, Workflow.Result.Target.MZ + 100);
            }

            if (Workflow.Result.IsotopicProfile != null)
            {
                MsGraphMaxY = Workflow.Result.IsotopicProfile.getMostIntensePeak().Height;
            }
            else
            {
                MsGraphMaxY = (float)xydata.getMaxY();
            }

            string msGraphTitle = Workflow.Result.Target.Code + "; m/z " +
                                  Workflow.Result.Target.MZ.ToString("0.0000") + "; z=" +
                                  Workflow.Result.Target.ChargeState;


            PlotModel plotModel = new PlotModel(msGraphTitle);
            plotModel.TitleFontSize = 11;
            plotModel.Padding = new OxyThickness(0);
            plotModel.PlotMargins = new OxyThickness(0);
            plotModel.PlotAreaBorderThickness = 0;



            var series = new OxyPlot.Series.LineSeries();
            series.MarkerSize = 1;
            series.Color = OxyColors.Black;
            for (int i = 0; i < xydata.Xvalues.Length; i++)
            {
                series.Points.Add(new DataPoint(xydata.Xvalues[i], xydata.Yvalues[i]));
            }

            var xAxis = new LinearAxis(AxisPosition.Bottom, "m/z");
            xAxis.Minimum = MsGraphMinX;
            xAxis.Maximum = MsGraphMaxX;
            

            var yAxis = new LinearAxis(AxisPosition.Left, "Intensity");
            yAxis.Minimum = 0;
            yAxis.AbsoluteMinimum = 0;
            yAxis.Maximum = MsGraphMaxY + MsGraphMaxY * 0.05;
            yAxis.StringFormat = "0.0E0";
            //yAxis.Maximum = maxIntensity + (maxIntensity * .05);
            //yAxis.AbsoluteMaximum = maxIntensity + (maxIntensity * .05);
            yAxis.AxisChanged += OnYAxisChange;
            //yAxis.UseSuperExponentialFormat = true;
            yAxis.FontSize = 12;

            xAxis.AxislineStyle = LineStyle.Solid;
            xAxis.AxislineThickness = 1;
            yAxis.AxislineStyle = LineStyle.Solid;
            yAxis.AxislineThickness = 1;

            plotModel.Series.Add(series);
            plotModel.Axes.Add(yAxis);
            plotModel.Axes.Add(xAxis);

            ObservedIsoPlot = plotModel;


        }

        private void CreateTheorIsotopicProfilePlot()
        {
            var theorProfileAligned = Workflow.Result.Target.IsotopicProfile.CloneIsotopicProfile();
            double fwhm;
            if (Workflow.Result.IsotopicProfile != null)
            {

                fwhm = Workflow.Result.IsotopicProfile.GetFWHM();
                IsotopicProfileUtilities.AlignTwoIsotopicProfiles(Workflow.Result.IsotopicProfile, theorProfileAligned);

                if (Workflow.SubtractedIso != null && Workflow.SubtractedIso.Peaklist.Count > 0)
                {
                    SubtractedMassSpecXYData = TheorXYDataCalculationUtilities.GetTheoreticalIsotopicProfileXYData(Workflow.SubtractedIso, fwhm);
                }
                else
                {
                    SubtractedMassSpecXYData = new XYData
                    {
                        Xvalues = new double[] { 400, 500, 600 },
                        Yvalues = new double[] { 0, 0, 0 }
                    };
                }
            }
            else
            {
                fwhm = DefaultMsPeakWidth;
            }

            TheorProfileXyData = TheorXYDataCalculationUtilities.GetTheoreticalIsotopicProfileXYData(Workflow.Result.Target.IsotopicProfile, fwhm);

            XYData xydata = new XYData();
            xydata.Xvalues = TheorProfileXyData.Xvalues;
            xydata.Yvalues = TheorProfileXyData.Yvalues;

            //scale to 100;
            for (int i = 0; i < xydata.Yvalues.Length; i++)
            {
                xydata.Yvalues[i] = xydata.Yvalues[i]*100;
            }



            string msGraphTitle = "Theoretical MS - m/z " +
                                  Workflow.Result.Target.MZ.ToString("0.0000") + "; z=" +
                                  Workflow.Result.Target.ChargeState;


            PlotModel plotModel = new PlotModel(msGraphTitle);
            plotModel.TitleFontSize = 11;
            plotModel.Padding = new OxyThickness(0);


            plotModel.PlotMargins = new OxyThickness(50, 0, 0, 0);
            plotModel.PlotAreaBorderThickness = 0;

            var series = new OxyPlot.Series.LineSeries();
            series.MarkerSize = 1;
            series.Color = OxyColors.Black;
            for (int i = 0; i < xydata.Xvalues.Length; i++)
            {
                series.Points.Add(new DataPoint(xydata.Xvalues[i], xydata.Yvalues[i]));
            }

            var xAxis = new LinearAxis(AxisPosition.Bottom, "m/z");
            xAxis.Minimum = MsGraphMinX;
            xAxis.Maximum = MsGraphMaxX;
            
            var yAxis = new LinearAxis(AxisPosition.Left, "Intensity");
            yAxis.Minimum = 0;
            yAxis.AbsoluteMinimum = 0;
            yAxis.Maximum = 105;
            yAxis.AbsoluteMaximum = 105;
            yAxis.StringFormat = "0.0E0";
            yAxis.FontSize = 12;

            //yAxis.Maximum = maxIntensity + (maxIntensity * .05);
            //yAxis.AbsoluteMaximum = maxIntensity + (maxIntensity * .05);
            yAxis.AxisChanged += OnYAxisChange;
            //yAxis.UseSuperExponentialFormat = true;

            


            xAxis.AxislineStyle = LineStyle.Solid;
            xAxis.AxislineThickness = 1;
            yAxis.AxislineStyle = LineStyle.Solid;
            yAxis.AxislineThickness = 1;


            plotModel.Series.Add(series);
            plotModel.Axes.Add(yAxis);
            plotModel.Axes.Add(xAxis);


            TheorIsoPlot = plotModel;


        }

        private void CreateChromCorrPlot()
        {
            ChromCorrXYData = new XYData();
            ChromCorrXYData.Xvalues = Workflow.ChromCorrelationRSquaredVals == null ? new double[] { 0, 1, 2, 3, 4 } : Workflow.ChromCorrelationRSquaredVals.Xvalues;
            ChromCorrXYData.Yvalues = Workflow.ChromCorrelationRSquaredVals == null ? new double[] { 0, 0, 0, 0, 0 } : Workflow.ChromCorrelationRSquaredVals.Yvalues;

            XYData xydata = new XYData();
            xydata.Xvalues = ChromCorrXYData.Xvalues;
            xydata.Yvalues = ChromCorrXYData.Yvalues;

            string graphTitle = "Isotope peak correlation data";
            PlotModel plotModel = new PlotModel(graphTitle);
            plotModel.TitleFontSize = 11;
            plotModel.Padding = new OxyThickness(0);
            plotModel.PlotMargins = new OxyThickness(0);
            plotModel.PlotAreaBorderThickness = 0;

            var series = new OxyPlot.Series.LineSeries();
            series.MarkerSize = 3;
            series.MarkerType = MarkerType.Square;
            series.MarkerStrokeThickness = 1;
            series.MarkerFill = OxyColors.DarkRed;
            series.MarkerStroke = OxyColors.Black;

            double yAxisMultiplier = 100;

            series.Color = OxyColors.Black;
            for (int i = 0; i < xydata.Xvalues.Length; i++)
            {
                series.Points.Add(new DataPoint(xydata.Xvalues[i], xydata.Yvalues[i] * yAxisMultiplier));
            }

            var xAxis = new LinearAxis(AxisPosition.Bottom, "isotopic peak #");


            var yAxis = new LinearAxis(AxisPosition.Left, "correlation");

            xAxis.AxislineStyle = LineStyle.Solid;
            xAxis.AxislineThickness = 1;
            yAxis.AxislineStyle = LineStyle.Solid;
            yAxis.AxislineThickness = 1;

            xAxis.Minimum = MsGraphMinX;
            xAxis.Maximum = MsGraphMaxX;

            xAxis.FontSize = 12;
            xAxis.MajorStep = 1;
            xAxis.ShowMinorTicks = false;
            xAxis.MinorStep = 0.1;
            
            yAxis.FontSize = 12;
            yAxis.Minimum = 0;
            yAxis.AbsoluteMinimum = 0;
            yAxis.Maximum = 1.02 * yAxisMultiplier;
            yAxis.AbsoluteMaximum = 1.02 * yAxisMultiplier;
            yAxis.AxisChanged += OnYAxisChange;
            yAxis.StringFormat = "0.0E0";

            plotModel.Series.Add(series);
            plotModel.Axes.Add(yAxis);
            plotModel.Axes.Add(xAxis);

            ChromCorrelationPlot = plotModel;
        }

        private void UpdateGraphRelatedProperties()
        {
            ChromXyData = new XYData();
            ChromXyData.Xvalues = Workflow.ChromatogramXYData == null ? new double[] { 0, 1, 2, 3, 4 } : Workflow.ChromatogramXYData.Xvalues;
            ChromXyData.Yvalues = Workflow.ChromatogramXYData == null ? new double[] { 0, 1, 2, 3, 4 } : Workflow.ChromatogramXYData.Yvalues;

            //MassSpecXYData = new XYData();
            //MassSpecXYData.Xvalues = Workflow.MassSpectrumXYData == null ? new double[] { 0, 1, 2, 3, 4 } : Workflow.MassSpectrumXYData.Xvalues;
            //MassSpecXYData.Yvalues = Workflow.MassSpectrumXYData == null ? new double[] { 0, 1, 2, 3, 4 } : Workflow.MassSpectrumXYData.Yvalues;


            RatioXyData = new XYData();
            RatioXyData.Xvalues = Workflow.RatioVals == null ? new double[] { 0, 1, 2, 3, 4 } : Workflow.RatioVals.Xvalues;
            RatioXyData.Yvalues = Workflow.RatioVals == null ? new double[] { 0, 0, 0, 0, 0 } : Workflow.RatioVals.Yvalues;


            RatioLogsXyData = new XYData();
            RatioLogsXyData.Xvalues = new double[] { 0, 1, 2, 3, 4 };
            RatioLogsXyData.Yvalues = new double[] { 0, 0, 0, 0, 0 };


            LabelDistributionXyData = new XYData();
            if (CurrentResult != null && CurrentResult.LabelDistributionVals != null && CurrentResult.LabelDistributionVals.Length > 0)
            {
                //var xvals = ratioData.Peaklist.Select((p, i) => new { peak = p, index = i }).Select(n => (double)n.index).ToList();

                LabelDistributionXyData.Xvalues = CurrentResult.LabelDistributionVals.Select((value, index) => new { index }).Select(n => (double)n.index).ToArray();
                LabelDistributionXyData.Yvalues = CurrentResult.LabelDistributionVals;
            }
            else
            {
                LabelDistributionXyData.Xvalues = new double[] { 0, 1, 2, 3 };
                LabelDistributionXyData.Yvalues = new double[] { 0, 0, 0, 0 };
            }



            //if (CurrentResultInfo != null)
            //{
            //    MSGraphMinX = CurrentResultInfo.MonoMZ - 1.75;
            //    MSGraphMaxX = CurrentResultInfo.MonoMZ + MassSpecVisibleWindowWidth;
            //}
            //else
            //{
            //    MSGraphMinX = MassSpecXYData.Xvalues.Min();
            //    MSGraphMaxX = MassSpecXYData.Xvalues.Max();
            //}






        }
        
        private void SetCurrentWorkflowTarget(SipperLcmsFeatureTargetedResultDTO result)
        {
            TargetBase target = new LcmsFeatureTarget();
            target.ChargeState = (short)result.ChargeState;
            target.ChargeStateTargets.Add(target.ChargeState);
            target.ElutionTimeUnit = Globals.ElutionTimeUnit.ScanNum;
            target.EmpiricalFormula = result.EmpiricalFormula;
            target.ID = (int)result.TargetID;


            target.IsotopicProfile = null;   //workflow will determine this

            target.MZ = result.MonoMZ;
            target.MonoIsotopicMass = result.MonoMass;
            target.ScanLCTarget = result.ScanLC;

            Run.CurrentMassTag = target;



        }
        
        private void CopyXyDataToClipboard(double[] xvals, double[] yvals)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            int maxLength = 0;
            if (xvals.Length == 0 || yvals.Length == 0) return;

            if (xvals.Length >= yvals.Length) maxLength = yvals.Length;
            else maxLength = xvals.Length;

            for (int i = 0; i < maxLength; i++)
            {
                stringBuilder.Append(xvals[i]);
                stringBuilder.Append("\t");
                stringBuilder.Append(yvals[i]);
                stringBuilder.Append(Environment.NewLine);
            }

            if (stringBuilder.ToString().Length == 0) return;

            Clipboard.SetText(stringBuilder.ToString());
            GeneralStatusMessage = "Data copied to clipboard";

        }

        private string DetermineDelimiterInString(string targetFilterString)
        {
            string[] delimitersToCheck = new string[] { "\t", ",", " ", Environment.NewLine };
            string mostFrequentDelim = string.Empty;

            int maxCount = int.MinValue;

            foreach (var delim in delimitersToCheck)
            {

                string[] tempStringArray = new[] { delim };

                var count = targetFilterString.Split(tempStringArray, StringSplitOptions.RemoveEmptyEntries).Length - 1;

                if (count > maxCount)
                {
                    mostFrequentDelim = delim;
                    maxCount = count;
                }

            }

            return mostFrequentDelim;
        }

        private void SetResults(IEnumerable<TargetedResultDTO> resultsToSet)
        {
            var query = (from n in resultsToSet select (SipperLcmsFeatureTargetedResultDTO)n);

            Results.Clear();
            foreach (var resultDto in query)
            {
                Results.Add(resultDto);
            }

            TargetsFileStatusText = "Viewing " + Results.Count + " results/targets";
        }

        private void FilterTargets()
        {
            //determine delimiter of TargetFilterString, if any
            if (string.IsNullOrEmpty(TargetFilterString))
            {
                SetResults(_resultRepositorySource.Results);
                return;
            }

            char[] delimitersToCheck = new char[] { '\t', ',', '\n', ' ' };

            var trimmedFilterString = TargetFilterString.Trim(delimitersToCheck);

            string delimiter = DetermineDelimiterInString(trimmedFilterString);

            List<string> filterList = new List<string>();
            if (!string.IsNullOrEmpty(delimiter))
            {
                var parsedFilterStringArray = trimmedFilterString.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
                filterList.AddRange(parsedFilterStringArray);

            }
            else
            {
                filterList.Add(trimmedFilterString);
            }



            var filteredResults = new List<TargetedResultDTO>();
            foreach (var filter in filterList)
            {
                int myInt;
                bool isNumerical = int.TryParse(filter, out myInt);



                if (isNumerical)
                {
                    filteredResults.AddRange(_resultRepositorySource.Results.Where(p => p.TargetID.ToString().StartsWith(myInt.ToString())));
                }
                else
                {
                    filteredResults.AddRange(_resultRepositorySource.Results.Where(p => p.Code.Contains(filter)));
                }

            }

            SetResults(filteredResults);





            //split string

            //determine if number or letters

            //if number, filter based on targetID;  if letters, filter based on code
        }

        #endregion

  
    }
}
