using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Objects.BuildingBlocks;
using GlycolyzerGUImvvm.Models;
using OmniFinder.Objects;

namespace GetPeaks_DLL.Glycolyzer.ParameterConverters
{
    public static class ConvertMiscellaneousMatter
    {
        public static List<BuildingBlockMiscellaneousMatter> Convert(ParameterModel parameterModel_Input)
        {
            List<BuildingBlockMiscellaneousMatter> miscellaneousBlocks = new List<BuildingBlockMiscellaneousMatter>();

            SetMethyl(parameterModel_Input, miscellaneousBlocks);
            SetNaMinusH(parameterModel_Input, miscellaneousBlocks);
            SetOAcetyl(parameterModel_Input, miscellaneousBlocks);
            SetSulfate(parameterModel_Input, miscellaneousBlocks);

            return miscellaneousBlocks;
        }

        private static void SetMethyl(ParameterModel parameterModel_Input, List<BuildingBlockMiscellaneousMatter> miscellaneousBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedCH3_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinCH3_Int, parameterModel_Input.ParameterRangesModel_Save.MaxCH3_Int);
                miscellaneousBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockMiscellaneousMatter(PNNLOmics.Data.Constants.MiscellaneousMatterName.Methyl, ranges));
            }
        }

        private static void SetNaMinusH(ParameterModel parameterModel_Input, List<BuildingBlockMiscellaneousMatter> miscellaneousBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedNaH_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinNaH_Int, parameterModel_Input.ParameterRangesModel_Save.MaxNaH_Int);
                miscellaneousBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockMiscellaneousMatter(PNNLOmics.Data.Constants.MiscellaneousMatterName.NaMinusH, ranges));
            }
        }

        private static void SetOAcetyl(ParameterModel parameterModel_Input, List<BuildingBlockMiscellaneousMatter> miscellaneousBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedOAcetyl_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinOAcetyl_Int, parameterModel_Input.ParameterRangesModel_Save.MaxOAcetyl_Int);
                miscellaneousBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockMiscellaneousMatter(PNNLOmics.Data.Constants.MiscellaneousMatterName.OAcetyl, ranges));
            }
        }

        private static void SetSulfate(ParameterModel parameterModel_Input, List<BuildingBlockMiscellaneousMatter> miscellaneousBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedSO3_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinSO3_Int, parameterModel_Input.ParameterRangesModel_Save.MaxSO3_Int);
                miscellaneousBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockMiscellaneousMatter(PNNLOmics.Data.Constants.MiscellaneousMatterName.Sulfate, ranges));
            }
        }

    }
}
