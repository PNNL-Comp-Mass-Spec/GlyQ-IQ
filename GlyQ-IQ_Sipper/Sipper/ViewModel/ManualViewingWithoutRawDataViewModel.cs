using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using DeconTools.Workflows.Backend;
using DeconTools.Workflows.Backend.FileIO;
using DeconTools.Workflows.Backend.Results;
using Sipper.Model;

namespace Sipper.ViewModel
{
    public class ManualViewingWithoutRawDataViewModel : ViewModelBase
    {

        private TargetedResultRepository _resultRepositorySource;
        private List<string> _imageFilePaths;
        private FileInputsInfo _fileInputsInfo;

        #region Constructors

        public ManualViewingWithoutRawDataViewModel(FileInputsInfo fileInputs = null)
        {
            Results = new ObservableCollection<ResultWithImageInfo>();

            _fileInputsInfo = fileInputs;
            FileInputs = new FileInputsViewModel(fileInputs);

            FileInputs.PropertyChanged += FileInputsPropertyChanged;
            _resultRepositorySource = new TargetedResultRepository();

        }

        public ManualViewingWithoutRawDataViewModel(TargetedResultRepository resultRepository, FileInputsInfo fileInputs = null)
            : this(fileInputs)
        {
            _resultRepositorySource = resultRepository;
            GetImageFileReferences(FileInputs.ResultImagesFolderPath);
            SetResults();

        }


        #endregion


        void FileInputsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "TargetsFilePath":
                    LoadResults(FileInputs.TargetsFilePath);
                    break;
                case "ResultImagesFolderPath":
                    GetImageFileReferences(FileInputs.ResultImagesFolderPath);
                    SetResults();
                    break;


            }
        }


        #region Properties
        public ObservableCollection<ResultWithImageInfo> Results { get; set; }


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

        private string _resultImagesStatusText;
        public string ResultImagesStatusText
        {
            get { return _resultImagesStatusText; }
            set
            {
                if (value == _resultImagesStatusText) return;
                _resultImagesStatusText = value;
                OnPropertyChanged("ResultImagesStatusText");
            }
        }


        public FileInputsViewModel FileInputs { get; private set; }


       


        private ResultWithImageInfo _currentResult;
        public ResultWithImageInfo CurrentResult
        {
            get
            {
                return _currentResult;
            }
            set
            {
                if (value == _currentResult) return;
                _currentResult = value;
                OnPropertyChanged("CurrentResult");
            }
        }

        private void GetImageFileReferences(string resultImagesFolderPath)
        {
            IsImageFilesLoaded = false;

            if (String.IsNullOrEmpty(resultImagesFolderPath))
            {
                ResultImagesStatusText =  "0 images loaded";
            }
            else
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(resultImagesFolderPath);

                if (directoryInfo.Exists)
                {
                    _imageFilePaths = directoryInfo.GetFiles("*.png", SearchOption.AllDirectories).Select(p => p.FullName).ToList();
                    ResultImagesStatusText = _imageFilePaths.Count + " images loaded";
                    
                    if (_imageFilePaths.Count>0)
                    {
                        IsImageFilesLoaded = true;  
                    }
                    
                }
                else
                {
                    ResultImagesStatusText = "0 images loaded";
                }

            }

         

           
            


        }

        protected bool IsImageFilesLoaded { get; set; }

        #endregion

        #region Public Methods

        public void CreateImages()
        {
            if (string.IsNullOrEmpty(FileInputs.DatasetParentFolder))
            {
                GeneralStatusMessage = "Cannot create images. Dataset directory has not been defined. Please enter a path to the directory that contains the dataset.";
                return;
            }

            var imageOutputter = new ResultImageOutputter(_fileInputsInfo);
            

            imageOutputter.Execute();
        }


        public void OpenHTMLReport()
        {
            if (IsImageFilesLoaded && _resultRepositorySource.Results.Count > 0)
            {
                string expectedHtmlFilepath = FileInputs.ResultImagesFolderPath + Path.DirectorySeparatorChar +
                                              "0_index.html";

                if (File.Exists(expectedHtmlFilepath))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(expectedHtmlFilepath);
                    }
                    catch (Exception ex)
                    {
                        GeneralStatusMessage = ex.Message;
                    }
                    
                }

            }


        }


        public void GenerateHTMLReport()
        {
            if (IsImageFilesLoaded && _resultRepositorySource.Results.Count>0)
            {
                
                HTMLReportGenerator reportGenerator = new HTMLReportGenerator(Results, _fileInputsInfo);

                try
                {
                    reportGenerator.GenerateHTMLReport();
                    GeneralStatusMessage = "HTML report was generated! See output folder.";

                }
                catch (Exception ex)
                {
                    GeneralStatusMessage = ex.Message;

                }

            }
            else
            {
                GeneralStatusMessage=  "Results not ready yet. Check results and file/folder paths.";
            }
        }


        //TODO: code duplication here
        public void LoadResults(string resultFile)
        {
            GeneralStatusMessage = "";

            _resultRepositorySource.Results.Clear();

            FileInfo fileInfo = new FileInfo(resultFile);

            if (fileInfo.Exists)
            {
                SipperResultFromTextImporter importer = new SipperResultFromTextImporter(resultFile);
                var tempResults = importer.Import();

                _resultRepositorySource.Results.AddRange(tempResults.Results);
            }

            SetResults();

        }

        public void SetResults()
        {
            GeneralStatusMessage = "";

            if (_resultRepositorySource == null) return;

            var query = (from n in _resultRepositorySource.Results select (SipperLcmsFeatureTargetedResultDTO)n);

            Results.Clear();
            foreach (var resultDto in query)
            {
                ResultWithImageInfo resultWithImageInfo = new ResultWithImageInfo(resultDto);



                Results.Add(resultWithImageInfo);
            }

            TargetsFileStatusText = Results.Count + " loaded.";

            MapResultsToImages();

            GenerateHTMLReport();

            


        }

        private void MapResultsToImages()
        {
            if (String.IsNullOrEmpty(FileInputs.ResultImagesFolderPath)) return;

            foreach (var result in Results)
            {
                //string baseFileName = FileInputs.ResultImagesFolderPath + Path.DirectorySeparatorChar +
                //                 result.Result.DatasetName + "_ID" + result.Result.TargetID;


                string baseFileName = result.Result.DatasetName + "_ID" + result.Result.TargetID;


                var targetImages = (from n in _imageFilePaths where n.Contains(baseFileName) select n).ToList();


                string expectedMSImage = targetImages.FirstOrDefault(p => p.Contains("_MS.png"));
                string expectedChromImageFilename = targetImages.FirstOrDefault(p => p.Contains("_chrom.png"));
                string expectedTheorMSImageFilename = targetImages.FirstOrDefault(p => p.Contains("_theorMS.png"));


                result.MSImageFilePath = expectedMSImage??"";
                result.ChromImageFilePath = expectedChromImageFilename??"";
                result.TheorMSImageFilePath = expectedTheorMSImageFilename??"";
            }


        }

        public void SaveResults()
        {

            if (FileInputs.ResultsSaveFilePath == null) return;

            try
            {
                var exporter = new SipperResultToLcmsFeatureExporter(FileInputs.ResultsSaveFilePath);
                exporter.ExportResults(_resultRepositorySource.Results);
            }
            catch (Exception ex)
            {
                GeneralStatusMessage = "Error saving results. Error message: " + ex.Message;
                return;
            }

            GeneralStatusMessage = "Results saved to: " + Path.GetFileName(FileInputs.ResultsSaveFilePath);


        }

        
        public void UpdateAnnotationsUsingAutomaticFilter()
        {
            foreach (SipperLcmsFeatureTargetedResultDTO result in _resultRepositorySource.Results)
            {
                ResultFilteringUtilities.ApplyFilteringScheme2(result);

                result.ValidationCode = ValidationCode.None;

                if (result.PassesFilter)
                {
                    result.ValidationCode = ValidationCode.Yes;
                }
            }
        }




        #endregion

        #region Private Methods

        #endregion

     
    }
}
