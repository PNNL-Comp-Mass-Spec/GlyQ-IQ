using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQGlyQ.FIFO;

namespace MultiDatasetToolBox
{
    public static class FilterResults
    {
        public static List<GlyQIqResult> Standard(List<GlyQIqResult> GlyQIqResultHits, Filters onWhat)
        {
            switch(onWhat)
            {
                case Filters.Name:
                    {
                        GlyQIqResultHits = (from n in GlyQIqResultHits where n.Name == "Neuraminic Acid" || n.Name == "Deoxyhexose" select n).ToList();
                    }
                    break;
               case Filters.FinalDecision:
                    {
                        GlyQIqResultHits = (from n in GlyQIqResultHits where n.FinalDecision == "CorrectGlycan" select n).ToList();
                    }
                    break;
               case Filters.All:
                    {
                        GlyQIqResultHits = (from n in GlyQIqResultHits where n.Name == "Neuraminic Acid" || n.Name == "Deoxyhexose" select n).ToList();
                        GlyQIqResultHits = (from n in GlyQIqResultHits where n.FinalDecision == "CorrectGlycan" select n).ToList();
                    }
                    break;
               case Filters.None:
                    {

                    }
                    break;
               case Filters.MostAbundant:
                    {
                        GlyQIqResultHits = GlyQIqResultHits.OrderByDescending(n => Convert.ToDouble(n.GlobalAggregateAbundance)).ToList();
                        
                        List<GlyQIqResult> GlyQIqResultHitsTemp = new List<GlyQIqResult>();
                        GlyQIqResultHitsTemp.Add(GlyQIqResultHits.FirstOrDefault());

                        GlyQIqResultHits = GlyQIqResultHitsTemp;
                    }
                    break;
            }

            return GlyQIqResultHits;
        }

        public enum Filters
        {
            Name,
            FinalDecision,
            All,
            MostAbundant,
            None
        }

        public enum TypeOfHitsToSelect
        {
            MostAbundantAgregateAbundance,
            MostAbundantMsAbundance,
            First
        }
    }
}
