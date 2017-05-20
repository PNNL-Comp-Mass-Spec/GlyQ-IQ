using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Constants;
using OmniFinder.Objects;
using OmniFinder;
using OmniFinder.Objects.Enumerations;
using OmniFinder.Functions;
using OmniFinder.Objects.BuildingBlocks;

namespace Omni_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            OmniFinderController newController = new OmniFinderController();

            OmniFinderParameters parameters = new OmniFinderParameters();

            parameters.BuildingBlocksMonosacchcarides.Add(new BuildingBlockMonoSaccharide(MonosaccharideName.Hexose, new RangesMinMax(3, 5)));
            parameters.BuildingBlocksMonosacchcarides.Add(new BuildingBlockMonoSaccharide(MonosaccharideName.NAcetylhexosamine, new RangesMinMax(2, 5)));
            parameters.BuildingBlocksMonosacchcarides.Add(new BuildingBlockMonoSaccharide(MonosaccharideName.Deoxyhexose, new RangesMinMax(0, 1)));
            //parameters.BuildingBlocksMonosacchcarides.Add(new BuildingBlockMonoSaccharide(MonosaccharideName.Pentose, new RangesMinMax(0, 1)));
            //parameters.BuildingBlocksMonosacchcarides.Add(new BuildingBlockMonoSaccharide(MonosaccharideName.NeuraminicAcid, new RangesMinMax(0, 1)));
            //parameters.BuildingBlocksMonosacchcarides.Add(new BuildingBlockMonoSaccharide(MonosaccharideName.NGlycolylneuraminicAcid, new RangesMinMax(0, 1)));
            //parameters.BuildingBlocksMonosacchcarides.Add(new BuildingBlockMonoSaccharide(MonosaccharideName.KDN, new RangesMinMax(0, 1)));
            //parameters.BuildingBlocksMonosacchcarides.Add(new BuildingBlockMonoSaccharide(MonosaccharideName.HexuronicAcid, new RangesMinMax(0, 1)));
            
            //parameters.BuildingBlocksAminoAcids.Add(new BuildingBlockAminoAcid(AminoAcidName.Asparagine, new RangesMinMax(0, 1)));
            
            parameters.ChargeCarryingAdduct = Adducts.Monoisotopic;

            parameters.CarbohydrateType = CarbType.Aldehyde;

            OmniFinderOutput results = newController.FindCompositions(parameters);
            
            Console.WriteLine(results.MassAndComposition[1].MassExact.ToString());        
        }
    }
}
