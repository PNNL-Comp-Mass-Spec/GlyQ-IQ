using System.Collections.Generic;
using PNNLOmics.Data;

namespace GetPeaksDllLite.CompareAndContrast
{
    public class CompareResults
    {
        public List<int> IndexListAMatch { get; set; }
        public List<int> IndexListBMatch { get; set; }
        public List<int> IndexListAandNotB { get; set; }
        public List<int> IndexListBandNotA { get; set; }
        public int extraHitCounter {get; set;}
        public int startIndex { get; set; }
		public string IndexName { get; set; }
        public double tollerance { get; set; }

        public void AddAandB(List<int> listA, List<int> listB)
		{
            this.IndexListAMatch = listA;
            this.IndexListBMatch = listB;
		}

        public void AddAandNotB(List<int> listIN)
		{
			this.IndexListAandNotB = listIN;
		}

        public void AddBandNotA(List<int> listIN)
		{
			this.IndexListBandNotA = listIN;
		}
    }

    public class ListResults
    {
        public List<XYData> XYList { get; set; }
        public List<int> XYIndexList { get; set; }

        public void AddListXY(List<XYData> ListIN)
        {
            this.XYList = ListIN;
        }

        public void AddListInt(List<int> XYIndexListIN)
        {
            this.XYIndexList = XYIndexListIN;
        }        
    }

    public class CompareResultsSet
    {
        public List<CompareResults> resultsList{ get; set;}

        public void AddResults(List<CompareResults> ListIN)
        {
            this.resultsList = ListIN;
        }
    }

    public  class LibraryIndexStartStop
    { 
        public int startIndex {get;set;}
        public int stopIndex {get;set;}

        public void SetIndex(int StartIndexIN, int StopIndexIN)
        {
            this.startIndex = StartIndexIN;
            this.stopIndex = StopIndexIN;
        }
    } 
}
