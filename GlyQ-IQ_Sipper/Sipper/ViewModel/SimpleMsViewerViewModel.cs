using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using DeconTools.Backend;
using DeconTools.Backend.Core;
using DeconTools.Backend.Data;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Backend.ProcessingTasks.MSGenerators;
using DeconTools.Backend.ProcessingTasks.PeakDetectors;
using DeconTools.Backend.ProcessingTasks.ZeroFillers;
using DeconTools.Backend.Runs;
using DeconTools.Backend.Workflows;
using DeconTools.Workflows.Backend.Core;
using OxyPlot;
using OxyPlot.Axes;

namespace Sipper.ViewModel
{
    public class SimpleMsViewerViewModel : ViewModelBase
    {

        private bool _xAxisIsChangedInternally;
        private bool _isInternalPeakListUpdate;

        private MSGenerator _msGenerator;
        private ScanSetFactory _scanSetFactory = new ScanSetFactory();
        private BackgroundWorker _backgroundWorker;
        private string _peaksFilename;
        PeakChromatogramGenerator _peakChromatogramGenerator;
        private bool _recreatePeaksFile;

        #region Constructors


        public SimpleMsViewerViewModel()
            : this(null)
        {

        }


        public SimpleMsViewerViewModel(Run run)
        {
            this.Run = run;

            PeakDetector = new DeconToolsPeakDetectorV2();
            _peakChromatogramGenerator = new PeakChromatogramGenerator();
            Peaks = new List<Peak>();

            //the order matters here. See the properties.
            MSGraphMaxX = 1500;
            MSGraphMinX = 400;
            ChromToleranceInPpm = 20;

            ChromSourcePeakDetectorSigNoise = 2;
            ChromSourcePeakDetectorPeakBr = 3;

            NumMSScansToSum = 1;
            ShowMsMsSpectra = false;
            
            NavigateToNextMS1MassSpectrum();

        }


        #endregion

        #region Properties

        public double ChromToleranceInPpm { get; set; }

        private bool _showMsMsSpectra;
        public bool ShowMsMsSpectra
        {
            get { return _showMsMsSpectra; }
            set
            {
                _showMsMsSpectra = value;
                OnPropertyChanged("ShowMsMsSpectra");
            }
        }


        public DeconToolsPeakDetectorV2 PeakDetector { get; set; }

        private double _chromSourcePeakDetectorPeakBr;
        public double ChromSourcePeakDetectorPeakBr
        {
            get { return _chromSourcePeakDetectorPeakBr; }
            set
            {
                _chromSourcePeakDetectorPeakBr = value;
                OnPropertyChanged("ChromSourcePeakDetectorPeakBr");
            }
        }


        private double _chromSourcePeakDetectorSigNoise;
        public double ChromSourcePeakDetectorSigNoise
        {
            get { return _chromSourcePeakDetectorSigNoise; }
            set
            {
                _chromSourcePeakDetectorSigNoise = value;
                OnPropertyChanged("ChromSourcePeakDetectorSigNoise");
            }
        }



        private List<Peak> _peaks;
        public List<Peak> Peaks
        {
            get { return _peaks; }
            set
            {
                _peaks = value;
                OnPropertyChanged("Peaks");
            }
        }


        private DeconTools.Backend.Core.Run _run;
        public DeconTools.Backend.Core.Run Run
        {
            get { return _run; }
            set
            {
                _run = value;

            }
        }

        int _currentLcScan;
        public int CurrentLcScan
        {
            get
            {
                return _currentLcScan;
            }
            set
            {
                _currentLcScan = value;
                OnPropertyChanged("CurrentLcScan");
            }
        }

        private ScanSet _currentScanSet;
        public ScanSet CurrentScanSet
        {
            get { return _currentScanSet; }
            set { _currentScanSet = value; }
        }


        private Peak _selectedPeak;
        public Peak SelectedPeak
        {
            get { return _selectedPeak; }
            set
            {
                _selectedPeak = value;


                CreateChromatogram();

            }
        }


        public int MinLcScan
        {
            get
            {
                if (_run == null) return 1;
                return _run.MinLCScan;
            }

        }

        public int MaxLcScan
        {
            get
            {
                if (_run == null) return 1;
                return _run.MaxLCScan;
            }

        }


        private XYData _massSpecXYData;
        public XYData MassSpecXYData
        {
            get { return _massSpecXYData; }
            set
            {
                _massSpecXYData = value;
                OnPropertyChanged("MassSpecXYData");
            }
        }

        private int _numMsScansToSum;
        public int NumMSScansToSum
        {
            get
            {
                return _numMsScansToSum;
            }
            set
            {
                bool isEvenNumber = value % 2 == 0;
                if (isEvenNumber)
                {
                    bool tryingToSumMore = value > _numMsScansToSum;
                    if (tryingToSumMore)
                    {
                        value++;
                    }
                    else
                    {
                        value--;
                    }
                }

                if (value < 1)
                {
                    value = 1;
                }

                _numMsScansToSum = value;
                OnPropertyChanged("NumMSScansToSum");

                if (!_xAxisIsChangedInternally)
                {
                    NavigateToNextMS1MassSpectrum(Globals.ScanSelectionMode.CLOSEST);
                }
            }
        }


        private double _msGraphMaxX;
        public double MSGraphMaxX
        {
            get { return _msGraphMaxX; }
            set
            {
                if (value <= _msGraphMinX)
                {
                    value = _msGraphMinX + 0.001;
                }

                _msGraphMaxX = value;
                OnPropertyChanged("MSGraphMaxX");

                if (!_xAxisIsChangedInternally)
                {
                    NavigateToNextMS1MassSpectrum(Globals.ScanSelectionMode.CLOSEST);
                }
            }
        }

        private double _msGraphMinX;
        public double MSGraphMinX
        {
            get { return _msGraphMinX; }
            set
            {
                if (value >= _msGraphMaxX)
                {
                    value = _msGraphMaxX - 0.001;
                }

                _msGraphMinX = value;
                OnPropertyChanged("MSGraphMinX");

                if (!_xAxisIsChangedInternally)
                {
                    NavigateToNextMS1MassSpectrum(Globals.ScanSelectionMode.CLOSEST);
                }

            }
        }


        public string DatasetName
        {
            get
            {
                if (Run == null) return "";
                return Run.DatasetName;
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

        string _generalStatusMessage;
        public string GeneralStatusMessage
        {
            get
            {
                return _generalStatusMessage;
            }
            set
            {
                _generalStatusMessage = value;
                OnPropertyChanged("GeneralStatusMessage");
            }
        }


        private int _percentProgress;
        public int PercentProgress
        {
            get { return _percentProgress; }
            set
            {
                _percentProgress = value;
                OnPropertyChanged("PercentProgress");
            }
        }

        protected XYData ChromXyData { get; set; }


        #endregion

        #region Public Methods

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

            if (Run != null)
            {
                LoadPeaksUsingBackgroundWorker();

            }

            if (Run != null)
            {
                NavigateToNextMS1MassSpectrum();
            }

            OnPropertyChanged("DatasetName");
        }


        public void LoadPeaksUsingBackgroundWorker(bool recreatePeaksFile = false)
        {

            _recreatePeaksFile = recreatePeaksFile;

            if (Run == null) return;

            if (_backgroundWorker != null && _backgroundWorker.IsBusy)
            {
                GeneralStatusMessage = "Busy...";
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




        public void NavigateToNextMS1MassSpectrum(Globals.ScanSelectionMode selectionMode = Globals.ScanSelectionMode.ASCENDING)
        {
            if (Run == null) return;

            int nextPossibleMs;
            if (selectionMode == Globals.ScanSelectionMode.DESCENDING)
            {
                nextPossibleMs = CurrentLcScan - 1;
            }
            else if (selectionMode == Globals.ScanSelectionMode.ASCENDING)
            {
                nextPossibleMs = CurrentLcScan + 1;
            }
            else
            {
                nextPossibleMs = CurrentLcScan;
            }

            if (!ShowMsMsSpectra)
            {
                CurrentLcScan = Run.GetClosestMSScan(nextPossibleMs, selectionMode);  
            }
            else
            {
                CurrentLcScan = nextPossibleMs;
            }
            

            if (_msGenerator == null)
            {
                _msGenerator = MSGeneratorFactory.CreateMSGenerator(Run.MSFileType);

            }


            CurrentScanSet = _scanSetFactory.CreateScanSet(Run, CurrentLcScan, NumMSScansToSum);
            MassSpecXYData = _msGenerator.GenerateMS(Run, CurrentScanSet);

           

            Peaks = new List<Peak>();
            if (MassSpecXYData != null)
            {
                if (Run.IsDataCentroided(CurrentLcScan))
                {

                    MassSpecXYData = ZeroFillCentroidData(MassSpecXYData);
                }


                //Trim the viewable mass spectrum, but leave some data so user can pan to the right and left
                MassSpecXYData = MassSpecXYData.TrimData(MSGraphMinX - 20, MSGraphMaxX + 20);

                //Use only the data within the viewing area for peak detection
                var xydataForPeakDetector = MassSpecXYData.TrimData(MSGraphMinX, MSGraphMaxX);
                Peaks = PeakDetector.FindPeaks(xydataForPeakDetector.Xvalues, xydataForPeakDetector.Yvalues);

            }

            CreateMSPlotForScanByScanAnalysis();

            _isInternalPeakListUpdate = true;
            SelectedPeak = Peaks.OrderByDescending(p => p.Height).FirstOrDefault();  //this triggers an XIC
            _isInternalPeakListUpdate = false;

        }

        private XYData ZeroFillCentroidData(XYData massSpecXyData)
        {
            List<double> newXValues = new List<double>();
            List<double> newYValues = new List<double>();


            for (int i = 0; i < massSpecXyData.Xvalues.Length; i++)
            {
                var currentXVal = massSpecXyData.Xvalues[i];
                var currentYVal = massSpecXyData.Yvalues[i];

                double zeroFillDistance=0.005;
                double newXValBefore = currentXVal - zeroFillDistance;
                double newXValAfter = currentXVal + zeroFillDistance;

                newXValues.Add(newXValBefore);
                newYValues.Add(0);

                newXValues.Add(currentXVal);
                newYValues.Add(currentYVal);

                newXValues.Add(newXValAfter);
                newYValues.Add(0);

            }

            return new XYData {Xvalues = newXValues.ToArray(), Yvalues = newYValues.ToArray()};
        }

        private void CreateChromatogram()
        {
            bool canGenerateChrom = Run != null && Run.ResultCollection.MSPeakResultList != null &&
                                    Run.ResultCollection.MSPeakResultList.Count > 0 && Peaks != null && Peaks.Count > 0
                                    && SelectedPeak != null;

            if (!canGenerateChrom) return;

            double scanWindowWidth = 600;
            int lowerScan = (int)Math.Round(Math.Max(MinLcScan, CurrentLcScan - scanWindowWidth / 2));
            int upperScan = (int)Math.Round(Math.Min(MaxLcScan, CurrentLcScan + scanWindowWidth / 2));

            ChromXyData = _peakChromatogramGenerator.GenerateChromatogram(Run, lowerScan, upperScan,
                                                                          SelectedPeak.XValue, ChromToleranceInPpm);

            if (ChromXyData == null)
            {
                ChromXyData = new XYData();
                ChromXyData.Xvalues = new double[] { lowerScan, upperScan };
                ChromXyData.Yvalues = new double[] { 0, 0 };

            }

            var maxY = (float)ChromXyData.getMaxY();


            string graphTitle = "XIC for most intense peak (m/z " + SelectedPeak.XValue.ToString("0.000") + ")";

            PlotModel plotModel = new PlotModel(graphTitle);
            plotModel.TitleFontSize = 9;
            plotModel.Padding = new OxyThickness(0);
            plotModel.PlotMargins = new OxyThickness(0);
            plotModel.PlotAreaBorderThickness = 0;

            var series = new OxyPlot.Series.LineSeries();
            series.MarkerSize = 1;
            series.Color = OxyColors.Black;
            for (int i = 0; i < ChromXyData.Xvalues.Length; i++)
            {
                series.Points.Add(new DataPoint(ChromXyData.Xvalues[i], ChromXyData.Yvalues[i]));
            }

            var xAxis = new LinearAxis(AxisPosition.Bottom, "scan");
            xAxis.Minimum = lowerScan;
            xAxis.Maximum = upperScan;

            var yAxis = new LinearAxis(AxisPosition.Left, "Intensity");
            yAxis.Minimum = 0;
            yAxis.AbsoluteMinimum = 0;
            yAxis.Maximum = maxY + maxY * 0.05;
            yAxis.AxisChanged += OnYAxisChange;

            xAxis.AxislineStyle = LineStyle.Solid;
            xAxis.AxislineThickness = 1;
            yAxis.AxislineStyle = LineStyle.Solid;
            yAxis.AxislineThickness = 1;

            plotModel.Series.Add(series);
            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);


            ChromatogramPlot = plotModel;




        }



        #endregion

        #region Private Methods


        private void CreateMSPlotForScanByScanAnalysis()
        {
            XYData xydata = new XYData();
            xydata.Xvalues = MassSpecXYData == null ? new double[] { 400, 1500 } : MassSpecXYData.Xvalues;
            xydata.Yvalues = MassSpecXYData == null ? new double[] { 0, 0 } : MassSpecXYData.Yvalues;

            string msGraphTitle = "Observed MS - Scan: " + (CurrentScanSet == null ? "" : CurrentScanSet.ToString());

            var maxY = (float)xydata.getMaxY(MSGraphMinX, MSGraphMaxX);


            PlotModel plotModel = new PlotModel(msGraphTitle);
            plotModel.TitleFontSize = 11;
            plotModel.Padding = new OxyThickness(0);
            plotModel.PlotMargins = new OxyThickness(0);
            plotModel.PlotAreaBorderThickness = 0;

            plotModel.MouseDown += MouseButtonDown;

            var series = new OxyPlot.Series.LineSeries();
            series.MarkerSize = 1;
            series.Color = OxyColors.Black;
            for (int i = 0; i < xydata.Xvalues.Length; i++)
            {
                series.Points.Add(new DataPoint(xydata.Xvalues[i], xydata.Yvalues[i]));
            }

            var xAxis = new LinearAxis(AxisPosition.Bottom, "m/z");
            xAxis.Minimum = MSGraphMinX;
            xAxis.Maximum = MSGraphMaxX;

            var yAxis = new LinearAxis(AxisPosition.Left, "Intensity");
            yAxis.Minimum = 0;
            yAxis.AbsoluteMinimum = 0;
            yAxis.Maximum = maxY + maxY * 0.05;
            //yAxis.Maximum = maxIntensity + (maxIntensity * .05);
            //yAxis.AbsoluteMaximum = maxIntensity + (maxIntensity * .05);
            yAxis.AxisChanged += OnYAxisChange;

            xAxis.AxisChanged += OnXAxisChange;


            xAxis.AxislineStyle = LineStyle.Solid;
            xAxis.AxislineThickness = 1;
            yAxis.AxislineStyle = LineStyle.Solid;
            yAxis.AxislineThickness = 1;

            plotModel.Series.Add(series);
            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);


            ObservedIsoPlot = plotModel;






        }

        private void LoadPeaks()
        {
            try
            {
                _peaksFilename = this.Run.DataSetPath + "\\" + this.Run.DatasetName + "_peaks.txt";

                if (_recreatePeaksFile || !File.Exists(_peaksFilename))
                {
                    _recreatePeaksFile = false;

                    if (File.Exists(_peaksFilename)) File.Delete(_peaksFilename);

                    GeneralStatusMessage =
                        "Creating chromatogram data (_peaks.txt file); this is only done once. It takes 1 - 5 min .......";

                    var peakCreationParameters = new PeakDetectAndExportWorkflowParameters();
                    peakCreationParameters.PeakBR = ChromSourcePeakDetectorPeakBr;
                    peakCreationParameters.PeakFitType = Globals.PeakFitType.QUADRATIC;
                    peakCreationParameters.SigNoiseThreshold = ChromSourcePeakDetectorSigNoise;

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



        private void OnXAxisChange(object sender, AxisChangedEventArgs e)
        {
            LinearAxis axis = sender as LinearAxis;

            _xAxisIsChangedInternally = true;

            MSGraphMinX = axis.ActualMinimum;
            MSGraphMaxX = axis.ActualMaximum;

            _xAxisIsChangedInternally = false;
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


        private void MouseButtonDown(object sender, OxyMouseEventArgs e)
        {
            var plot = ObservedIsoPlot;

            if (e.ChangedButton == OxyMouseButton.Left)
            {
                var position = e.Position;

                var series = plot.GetSeriesFromPoint(position, 10);
                if (series != null)
                {
                    var hitResult = series.GetNearestPoint(position, true);

                    if (hitResult != null && hitResult.DataPoint != null)
                    {
                        var datapoint = hitResult.DataPoint;

                        SelectedPeak = new Peak(datapoint.X, (float)datapoint.Y, 0);

                        GeneralStatusMessage = "Selected point = " + datapoint.X.ToString("0.000") + ", " +
                                       datapoint.Y.ToString("0.000");
                    }
                }
            }

        }



        #endregion

    }
}
