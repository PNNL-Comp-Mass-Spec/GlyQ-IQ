using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlycolyzerGUImvvm.Models;
using OmniFinder.Objects.BuildingBlocks;
using OmniFinder.Objects;

namespace GetPeaks_DLL.Glycolyzer.ParameterConverters
{
    public static class ConvertMonosaccharides
    {
        public static List<BuildingBlockMonoSaccharide> Convert(ParameterModel parameterModel_Input)
        {
            List<BuildingBlockMonoSaccharide> monosaccharideBlocks = new List<BuildingBlockMonoSaccharide>();

            SetDeoxyhexose(parameterModel_Input, monosaccharideBlocks);
            SetHexose(parameterModel_Input, monosaccharideBlocks);
            SetHexuronicAcid(parameterModel_Input, monosaccharideBlocks);
            SetKDN(parameterModel_Input, monosaccharideBlocks);
            SetNAcetylhexosamine(parameterModel_Input, monosaccharideBlocks);
            SetNeuraminicAcid(parameterModel_Input, monosaccharideBlocks);
            SetNGlycolylneuraminicAcid(parameterModel_Input, monosaccharideBlocks);
            SetPentose(parameterModel_Input, monosaccharideBlocks);

            return monosaccharideBlocks;
        }

        private static void SetDeoxyhexose(ParameterModel parameterModel_Input, List<BuildingBlockMonoSaccharide> monosaccharideBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedDxyHex_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinDxyHex_Int, parameterModel_Input.ParameterRangesModel_Save.MaxDxyHex_Int);
                monosaccharideBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Deoxyhexose, ranges));
            }
        }

        private static void SetHexose(ParameterModel parameterModel_Input, List<BuildingBlockMonoSaccharide> monosaccharideBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedHexose_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinHexose_Int, parameterModel_Input.ParameterRangesModel_Save.MaxHexose_Int);
                monosaccharideBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, ranges));
            }
        }

        private static void SetHexuronicAcid(ParameterModel parameterModel_Input, List<BuildingBlockMonoSaccharide> monosaccharideBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedHexA_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinHexA_Int, parameterModel_Input.ParameterRangesModel_Save.MaxHexA_Int);
                monosaccharideBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.HexuronicAcid, ranges));
            }
        }

        private static void SetKDN(ParameterModel parameterModel_Input, List<BuildingBlockMonoSaccharide> monosaccharideBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedKDN_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinKDN_Int, parameterModel_Input.ParameterRangesModel_Save.MaxKDN_Int);
                monosaccharideBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.KDN, ranges));
            }
        }

        private static void SetNAcetylhexosamine(ParameterModel parameterModel_Input, List<BuildingBlockMonoSaccharide> monosaccharideBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedHexNAc_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinHexNAc_Int, parameterModel_Input.ParameterRangesModel_Save.MaxHexNAc_Int);
                monosaccharideBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NAcetylhexosamine, ranges));
            }
        }

        private static void SetNeuraminicAcid(ParameterModel parameterModel_Input, List<BuildingBlockMonoSaccharide> monosaccharideBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedNeuAc_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinNeuAc_Int, parameterModel_Input.ParameterRangesModel_Save.MaxNeuAc_Int);
                monosaccharideBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NeuraminicAcid, ranges));
            }
        }

        private static void SetNGlycolylneuraminicAcid(ParameterModel parameterModel_Input, List<BuildingBlockMonoSaccharide> monosaccharideBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedNeuGc_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinNeuGc_Int, parameterModel_Input.ParameterRangesModel_Save.MaxNeuGc_Int);
                monosaccharideBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NGlycolylneuraminicAcid, ranges));
            }
        }

        private static void SetPentose(ParameterModel parameterModel_Input, List<BuildingBlockMonoSaccharide> monosaccharideBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedPentose_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinPentose_Int, parameterModel_Input.ParameterRangesModel_Save.MaxPentose_Int);
                monosaccharideBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Pentose, ranges));
            }
        }    
    }
}
