using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Objects;

namespace GetPeaks_DLL.Objects.ResultsObjects
{
    public class GlycanResult
    {
        public double GlycanHitsExperimentalMass { get; set; }//all masses from Hits (averaged)
        public double GlycanHitsExperimentalAbundance { get; set; }//all masses from Hits (summed)
        public double GlycanHitsExperimentalChargeMin { get; set; }//all masses from Hits 
        public double GlycanHitsExperimentalChargeMax { get; set; }//all masses from Hits 
        public double GlycanHitsExperimentalScanMin { get; set; }//all masses from Hits 
        public double GlycanHitsExperimentalScanMax { get; set; }//all masses from Hits 
        public double GlycanHitsExperimentalNumberOfIsomers { get; set; }//this is adjusted based on multiple hits to library and is close to correct

        public double GlycanHitsLibraryExactMass { get; set; }//exact mass from library
        public int GlycanHitsIndexFeature { get; set; }//index of coresponding glycan features for each hit
        public int GlycanHitsIndexOmniFinder { get; set; }//index of coresponding glycan compositions for each hit
        public FeatureAbstract GlycanHitsExperimentalFeature { get; set; }//features associated with hits
        public OmnifinderExactMassObject GlycanHitsComposition { get; set; }//exact mass from library and composition

        //combine information from GlycanHitsComposition to make a string of compositions
        public string GlycanElementalFormula { get; set; }
        public Isomer GlycanPolyIsomer { get; set; }
        public IsomerGlycanIndexes GlycanPolyIsomerIndex { get; set; }
        
        
        
        public GlycanResult()
        {
            GlycanHitsExperimentalMass = 0;
            GlycanHitsLibraryExactMass = 0;
            GlycanHitsExperimentalChargeMin = -1;
            GlycanHitsExperimentalChargeMax = -1;
            GlycanHitsExperimentalScanMin = -1;
            GlycanHitsExperimentalScanMax = -1;
            GlycanHitsExperimentalNumberOfIsomers = 0;

            GlycanHitsIndexFeature = -1;
            GlycanHitsIndexOmniFinder = -1;
            //GlycanHitsExperimentalFeature = new FeatureAbstract();
            GlycanHitsComposition = new OmnifinderExactMassObject();
            GlycanPolyIsomer = new Isomer();
            GlycanPolyIsomerIndex = new IsomerGlycanIndexes();
            
        }
    }   
}
