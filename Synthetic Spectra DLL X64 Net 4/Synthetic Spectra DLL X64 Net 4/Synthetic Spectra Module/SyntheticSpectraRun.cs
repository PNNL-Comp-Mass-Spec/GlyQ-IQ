using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using Synthetic_Spectra_DLL_X64_Net_4.Framework;
using Synthetic_Spectra_DLL_X64_Net_4.LoadFile;

namespace Synthetic_Spectra_DLL_X64_Net_4.Synthetic_Spectra_Module
{
    public class SyntheticSpectraRun<T, U> 
        where T : struct
        where U : struct
    {
        public DiskFileList NoiseFileBox { get; set; }
        public List<XYDataGeneric<T, U>> NoiseSpectra { get; set; }

        public DiskFileList MZParameterFileBox { get; set; }
        public SyntheticSpectraParameters MZParameters { get; set; }

        public DiskFileList TheoryFileBox { get; set; }
        public List<XYDataGeneric<T, U>> MonoIsotopicPeakList { get; set; }
        public List<int> TheoryChargeStateList {get;set;}

        public DiskFileList LCParametersFileBox { get; set; }
        public List<XYDataGeneric<int,int>> LCParametersRTSigma {get;set;}

        public SyntheticSpectraRun(string fileSource)
        { 
            //this will be loaded from File List text file;
            //New Listimporter  //creates an instance of Importer to work with
            FileListImporter loadList = new FileListImporter(fileSource);

            List<string> filesList = new List<string>(); 
            //Load table of contents file containing other files to load
            loadList.ImportFileList(filesList);


            this.MZParameterFileBox = new DiskFileList();
            this.TheoryFileBox = new DiskFileList();
            this.NoiseFileBox = new DiskFileList();
            this.LCParametersFileBox = new DiskFileList();

            this.MZParameterFileBox.FileName = filesList[0];
            this.MZParameterFileBox.FileKey = SyntheticSpectraFileType.MZParameters;

            this.TheoryFileBox.FileName =filesList[1];
            this.TheoryFileBox.FileKey = SyntheticSpectraFileType.TheoryData;

            this.NoiseFileBox.FileName = filesList[2];
            this.NoiseFileBox.FileKey = SyntheticSpectraFileType.Noise;

            this.LCParametersFileBox.FileName = filesList[3];
            this.LCParametersFileBox.FileKey = SyntheticSpectraFileType.LCParameters;
        }

        public SyntheticSpectraRun()
        {
            this.MZParameterFileBox = new DiskFileList();
            this.TheoryFileBox = new DiskFileList();
            this.NoiseFileBox = new DiskFileList();
            this.LCParametersFileBox = new DiskFileList();

            this.MZParameterFileBox.FileName = "";
            this.MZParameterFileBox.FileKey = SyntheticSpectraFileType.MZParameters;

            this.TheoryFileBox.FileName = "";
            this.TheoryFileBox.FileKey = SyntheticSpectraFileType.TheoryData;

            this.NoiseFileBox.FileName = "";
            this.NoiseFileBox.FileKey = SyntheticSpectraFileType.Noise;

            this.LCParametersFileBox.FileName = "";
            this.LCParametersFileBox.FileKey = SyntheticSpectraFileType.LCParameters;
        }
    }
}
