using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Objects;
using GlycolyzerGUImvvm.Models;
using OmniFinder.Objects.BuildingBlocks;

namespace GetPeaks_DLL.Glycolyzer.ParameterConverters
{
    public static class ConvertAminoAcids
    {
        public static List<BuildingBlockAminoAcid> Convert(ParameterModel parameterModel_Input)
        {
            List<BuildingBlockAminoAcid> aminoAcidBlocks = new List<BuildingBlockAminoAcid>();
            
            SetAlanine(parameterModel_Input, ref aminoAcidBlocks);
            SetAsparagine(parameterModel_Input, ref aminoAcidBlocks);
            SetArginine(parameterModel_Input, ref aminoAcidBlocks);
            SetAsparticAcid(parameterModel_Input, ref aminoAcidBlocks);
            SetCystine(parameterModel_Input, ref aminoAcidBlocks);
            SetGlutamicAcid(parameterModel_Input, ref aminoAcidBlocks);
            SetGlutamine(parameterModel_Input, ref aminoAcidBlocks);
            SetGlycine(parameterModel_Input, ref aminoAcidBlocks);
            SetHistidine(parameterModel_Input, ref aminoAcidBlocks);
            SetIsoleucine(parameterModel_Input, ref aminoAcidBlocks);
            SetLeucine(parameterModel_Input, ref aminoAcidBlocks);
            SetLysine(parameterModel_Input, ref aminoAcidBlocks);
            SetMethionine(parameterModel_Input, ref aminoAcidBlocks);
            SetPhenylalanine(parameterModel_Input, ref aminoAcidBlocks);
            SetProline(parameterModel_Input, ref aminoAcidBlocks);
            SetSerine(parameterModel_Input, ref aminoAcidBlocks);
            SetThreonine(parameterModel_Input, ref aminoAcidBlocks);
            SetTryptophan(parameterModel_Input, ref aminoAcidBlocks);
            SetTyrosine(parameterModel_Input, ref aminoAcidBlocks);
            SetValine(parameterModel_Input, ref aminoAcidBlocks);

            return aminoAcidBlocks;
        }

        private static void SetAlanine(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedAla_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinAla_Int, parameterModel_Input.ParameterRangesModel_Save.MaxAla_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Alanine, ranges));
            }
        }

        private static void SetAsparagine(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedAsn_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinAsn_Int, parameterModel_Input.ParameterRangesModel_Save.MaxAsn_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Asparagine, ranges));
            }
        }

        private static void SetArginine(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedArg_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinArg_Int, parameterModel_Input.ParameterRangesModel_Save.MaxArg_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Arginine, ranges));
            }
        }

        private static void SetAsparticAcid(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedAsp_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinAsp_Int, parameterModel_Input.ParameterRangesModel_Save.MaxAsp_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.AsparticAcid, ranges));
            }
        }

        private static void SetCystine(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedCys_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinCys_Int, parameterModel_Input.ParameterRangesModel_Save.MaxCys_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Cysteine, ranges));
            }
        }

        private static void SetGlutamicAcid(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedGlu_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinGlu_Int, parameterModel_Input.ParameterRangesModel_Save.MaxGlu_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.GlutamicAcid, ranges));
            }
        }

        private static void SetGlutamine(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedGln_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinGln_Int, parameterModel_Input.ParameterRangesModel_Save.MaxGln_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Glutamine, ranges));
            }
        }

        private static void SetGlycine(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedGly_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinGly_Int, parameterModel_Input.ParameterRangesModel_Save.MaxGly_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Glycine, ranges));
            }
        }

        private static void SetHistidine(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedHis_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinHis_Int, parameterModel_Input.ParameterRangesModel_Save.MaxHis_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Histidine, ranges));
            }
        }

        private static void SetIsoleucine(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedIle_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinIle_Int, parameterModel_Input.ParameterRangesModel_Save.MaxIle_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Isoleucine, ranges));
            }
        }

        private static void SetLeucine(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedLeu_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinLeu_Int, parameterModel_Input.ParameterRangesModel_Save.MaxLeu_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Leucine, ranges));
            }
        }

        private static void SetLysine(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedLys_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinLys_Int, parameterModel_Input.ParameterRangesModel_Save.MaxLys_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Lysine, ranges));
            }
        }

        private static void SetMethionine(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedMet_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinMet_Int, parameterModel_Input.ParameterRangesModel_Save.MinMet_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Methionine, ranges));
            }
        }

        private static void SetPhenylalanine(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedPhe_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinPhe_Int, parameterModel_Input.ParameterRangesModel_Save.MaxPhe_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Phenylalanine, ranges));
            }
        }

        private static void SetProline(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedPro_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinPro_Int, parameterModel_Input.ParameterRangesModel_Save.MaxPro_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Proline, ranges));
            }
        }

        private static void SetSerine(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedSer_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinSer_Int, parameterModel_Input.ParameterRangesModel_Save.MaxSer_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Serine, ranges));
            }
        }

        private static void SetThreonine(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedThr_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinThr_Int, parameterModel_Input.ParameterRangesModel_Save.MaxThr_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Threonine, ranges));
            }
        }

        private static void SetTryptophan(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedTrp_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinTrp_Int, parameterModel_Input.ParameterRangesModel_Save.MaxTrp_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Tryptophan, ranges));
            }
        }

        private static void SetTyrosine(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedTyr_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinTyr_Int, parameterModel_Input.ParameterRangesModel_Save.MaxTyr_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Tyrosine, ranges));
            }
        }

        private static void SetValine(ParameterModel parameterModel_Input, ref List<BuildingBlockAminoAcid> aminoAcids)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedVal_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinVal_Int, parameterModel_Input.ParameterRangesModel_Save.MaxVal_Int);
                aminoAcids.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockAminoAcid(PNNLOmics.Data.Constants.AminoAcidName.Valine, ranges));
            }
        }
    }
}
