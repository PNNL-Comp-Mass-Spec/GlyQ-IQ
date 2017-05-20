using System.Collections.Generic;
using System.IO;
using PNNLOmics.Data.Peaks;
using Run32.Backend.Core;

namespace GetPeaksDllLite.DataFIFO
{
    public abstract class IPeakListExporter
    {
        public abstract int TriggerToWriteValue { get; set; }
        public abstract int[] MSLevelsToExport { get; set; }
        public abstract string Name { get; set; }

        public abstract void WriteOutPeaksOmics(List<ProcessedPeak> peakList, List<int> IDs);
        public abstract void WriteOutPeaksOmics(StreamWriter sw, List<ProcessedPeak> peakList, List<int> IDs);

        public void Execute(List<ProcessedPeak> resultList, List<int> IDs, Run runIn)
        {
            if (resultList == null || resultList.Count == 0) return;

            // check if peak results exceeds Trigger value or is the last Scan 

            int lastScanNum = runIn.ScanSetCollection.ScanSetList[runIn.ScanSetCollection.ScanSetList.Count - 1].PrimaryScanNumber;

            bool isLastScan = true;
            bool writeOutPeaksNoMatterWhat = false;

            //if (runIn is UIMFRun)
            //{
            //    isLastScan = false;  // this doesn't matter since we are writing out the peaks no matter what.
            //    writeOutPeaksNoMatterWhat = true;
            //}
            //else
            //{
                isLastScan = (runIn.CurrentScanSet.PrimaryScanNumber == lastScanNum);

            //}

            //Write out results if exceeds trigger value or is last scan
            if (resultList.Count >= TriggerToWriteValue || isLastScan || writeOutPeaksNoMatterWhat)
            {
                WriteOutPeaksOmics(resultList, IDs);
                resultList.Clear();
            }

        }

        public virtual void Cleanup()
        {
            return;
        }
    }
}
