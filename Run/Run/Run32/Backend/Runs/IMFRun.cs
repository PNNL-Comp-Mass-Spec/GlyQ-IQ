﻿using System;
using System.IO;
using DeconToolsV2.Readers;
using Run32.Backend.Core;
using Run32.Backend.Data;
using Run32.Utilities;

namespace Run32.Backend.Runs
{
    [Serializable]
    public sealed class IMFRun : DeconToolsRun
    {

        public IMFRun()
        {
            this.XYData = new XYData();
            this.MSFileType = Globals.MSFileType.PNNL_IMS;
        }

        public IMFRun(string filename)
            : this()
        {
            Check.Require(File.Exists(filename),"File does not exist");

            Filename = filename;

            string baseFilename = Path.GetFileName(Filename);
            DatasetName = baseFilename.Substring(0, baseFilename.LastIndexOf('.'));
            DataSetPath = Path.GetDirectoryName(filename);

            RawData = new clsRawData(filename, FileType.PNNL_IMS);
            MinLCScan = GetMinPossibleLCScanNum();
            MaxLCScan = GetMaxPossibleLCScanNum();
        }

        public IMFRun(string filename, int minScan, int maxScan)
            : this(filename)
        {
            this.MinLCScan = minScan;
            this.MaxLCScan = maxScan;
        }


        public override int GetMinPossibleLCScanNum()
        {
            return 0;
        }

        public override int GetMaxPossibleLCScanNum()
        {
            return GetNumMSScans() - 1;
        }


  
        public override XYData GetMassSpectrum(ScanSet scanSet, double minMZ, double maxMZ)
        {
            Check.Require(scanSet != null, "Can't get mass spectrum; inputted set of scans is null");
            Check.Require(scanSet.IndexValues.Count > 0, "Can't get mass spectrum; no scan numbers inputted");
            
            int totScans = this.GetNumMSScans();


            double[] xvals = new double[0];
            double[] yvals = new double[0];

            //if (scanSet.IndexValues.Count == 1)            //this is the case of only wanting one MS spectrum
            //{
            //    this.rawData.GetSpectrum(scanSet.IndexValues[0], ref xvals, ref yvals);
            //}
            //else
            //{
            //    int upperscan = Math.Min(scanSet.getHighestScanNumber(), this.GetNumMSScans());
            //    int lowerscan = Math.Max(scanSet.getLowestScanNumber(), 1);
            //    this.rawData.GetSummedSpectra(lowerscan, upperscan, minMZ, maxMZ, ref xvals, ref yvals);
            //}

            int upperscan = Math.Min(scanSet.getHighestScanNumber(), this.GetNumMSScans());
            int lowerscan = Math.Max(scanSet.getLowestScanNumber(), 1);
            
            //TODO:  Old DeconTools reference!! remove this
            this.RawData.GetSummedSpectra(lowerscan, upperscan, minMZ, maxMZ, ref xvals, ref yvals);

            XYData xydata=new XYData();
            xydata.Xvalues = xvals;
            xydata.Yvalues = yvals;
            return xydata;
        }

    


    }
}
