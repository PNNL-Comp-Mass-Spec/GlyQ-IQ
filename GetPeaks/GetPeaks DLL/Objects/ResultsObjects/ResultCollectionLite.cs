using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.DTO;
using DeconTools.Backend.Core;

namespace GetPeaks_DLL.Objects.ResultsObjects
{
    [Serializable]
    public class ResultCollectionLite:IDisposable
    {
        public ResultCollectionLite()
        {
            //this.run = run;
            this.ResultList = new List<IsosResultLite>();
            //this.massTagResultList = new Dictionary<MassTag, MassTagResultBase>();
            //this.scanResultList = new List<ScanResult>();
            this.MSPeakResultList = new List<MSPeakResult>();
            this.IsosResultBin = new List<IsosResultLite>(10);
            this.LogMessageList = new List<string>();
            //this.ElutingPeakCollection = new List<ElutingPeak>();
            this.MSFeatureCounter = 0;
        }

        public List<MSPeakResult> MSPeakResultList { get; set; }
        public IList<IsosResultLite> IsosResultBin { get; set; }
        public List<string> LogMessageList { get; set; }
        public List<IsosResultLite> ResultList { get; set; }
        public int MSFeatureCounter { get; set; }


        public void ClearAllResults()
        {
            this.MSPeakResultList.Clear();
            this.IsosResultBin.Clear();
            this.LogMessageList.Clear();
            this.ResultList.Clear();
            //this.ScanResultList.Clear();
        }

        public void AddIsosResult(IsosResultLite addedResult)
        {
            addedResult.MSFeatureID = MSFeatureCounter;
            MSFeatureCounter++;    // by placing it here, we make the MSFeatureID a zero-based ID, as Kevin requested in an email (Jan 20/2010)
            this.IsosResultBin.Add(addedResult);
        }

        public void Dispose()
        {
            ClearAllResults();       
        }
    }
}
