using System.Collections.Generic;
using IQ.Backend.Core;
using Run32.Backend.Core;

namespace IQ.Workflows.FileIO
{
    public abstract class IPeakListExporter : TaskIQ
    {

        public abstract int TriggerToWriteValue { get; set; }
        public abstract int[] MSLevelsToExport { get; set; }

        public abstract void WriteOutPeaks(List<Run32.Backend.DTO.MSPeakResult> peakResultList);


        public override void Execute(ResultCollection resultList)
        {
            if (resultList.MSPeakResultList == null || resultList.MSPeakResultList.Count == 0) return;

            // check if peak results exceeds Trigger value or is the last Scan 

            int lastScanNum = resultList.Run.ScanSetCollection.ScanSetList[resultList.Run.ScanSetCollection.ScanSetList.Count - 1].PrimaryScanNumber;

            bool isLastScan=true;
            bool writeOutPeaksNoMatterWhat = false;

            //if (resultList.Run is UIMFRun)
            //{
            //    isLastScan = false;  // this doesn't matter since we are writing out the peaks no matter what.
            //    writeOutPeaksNoMatterWhat = true;
            //}
            //else
            //{
                isLastScan = (resultList.Run.CurrentScanSet.PrimaryScanNumber == lastScanNum);

            //}

            //Write out results if exceeds trigger value or is last scan
            if (resultList.MSPeakResultList.Count >= TriggerToWriteValue || isLastScan || writeOutPeaksNoMatterWhat)
            {
                WriteOutPeaks(resultList.MSPeakResultList);
                resultList.MSPeakResultList.Clear();
            }
        }
    }
}
