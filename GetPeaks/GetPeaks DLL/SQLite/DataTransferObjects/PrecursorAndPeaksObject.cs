using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PNNLOmics.Data;
using YAFMS_DB.GetPeaks;

namespace GetPeaks_DLL.SQLite.DataTransferObjects
{
    public class PrecursorAndPeaksObject
    {
        public int ScanNum { get; set; }

        public int PeakNumber { get; set; }

        public double XValue { get; set; }

        public int Charge { get; set; }

        public DatabasePeakProcessedWithMZObject PrecursorPeak { get; set; }

        public List<DatabasePeakProcessedWithMZObject> TandemMonoPeakList { get; set; }

        public List<DatabasePeakProcessedObject> TandemPeakList { get; set; }

        //New stuff
        //centric
        public List<DatabasePeakCentricObject> TandemPeakCentricList { get; set; }
        //public DatabaseAttributeCentricObjectList TandemAttributeCentricList { get; set; }

        public List<DatabasePeakCentricObject> TandemMonoPeakCentricList { get; set; }

        public DatabasePeakCentricObject PrecursorCentricPeak { get; set; }



        public string levelOfPeakProcessingOnList { get; set; }
        public string levelOfPeakProcessingOnPeak { get; set; }


        public PrecursorAndPeaksObject()
        {
            TandemMonoPeakList = new List<DatabasePeakProcessedWithMZObject>();
            PrecursorPeak = new DatabasePeakProcessedWithMZObject();
            TandemPeakList = new List<DatabasePeakProcessedObject>();
            levelOfPeakProcessingOnPeak = "FileHeadder";
            levelOfPeakProcessingOnList = "Unknown";

            //new stuff
            TandemPeakCentricList = new List<DatabasePeakCentricObject>();
            TandemMonoPeakCentricList = new List<DatabasePeakCentricObject>();
            PrecursorCentricPeak = new DatabasePeakCentricObject();
            //TandemAttributeCentricList = new DatabaseAttributeCentricObjectList();
        }
    }
}
