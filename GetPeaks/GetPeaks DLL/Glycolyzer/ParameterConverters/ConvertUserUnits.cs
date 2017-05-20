using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Objects.BuildingBlocks;
using GlycolyzerGUImvvm.Models;
using OmniFinder.Objects;
using PNNLOmics.Data.Constants.Libraries;
using PNNLOmics.Data.Constants;

namespace GetPeaks_DLL.Glycolyzer.ParameterConverters
{
    public class ConvertUserUnits
    {
        public static List<BuildingBlockUserUnit> Convert(ParameterModel parameterModel_Input)
        {
            List<BuildingBlockUserUnit> userBlocks = new List<BuildingBlockUserUnit>();

            SetUser01(parameterModel_Input, userBlocks);
            SetUser02(parameterModel_Input, userBlocks);
            SetUser03(parameterModel_Input, userBlocks);
            SetUser04(parameterModel_Input, userBlocks);
            SetUser05(parameterModel_Input, userBlocks);
            SetUser06(parameterModel_Input, userBlocks);
            SetUser07(parameterModel_Input, userBlocks);
            SetUser08(parameterModel_Input, userBlocks);
            SetUser09(parameterModel_Input, userBlocks);
            SetUser10(parameterModel_Input, userBlocks);

            return userBlocks;
        }

        public static UserUnitLibrary ConvertLibrary(ParameterModel parameterModel_Input)
        {
            UserUnitLibrary userLibrary = new UserUnitLibrary();

            //if (parameterModel_Input.OmniFinderModel_Save.CheckedUserUnit1_Bool == true)
            //{
                UserUnit user01 = new UserUnit("UserUnit01", "Usr01", parameterModel_Input.ParameterRangesModel_Save.MassUserUnit1_Double, UserUnitName.User01);
            //}

            //if (parameterModel_Input.OmniFinderModel_Save.CheckedUserUnit2_Bool == true)
            //{
                UserUnit user02 = new UserUnit("UserUnit02", "Usr02", parameterModel_Input.ParameterRangesModel_Save.MassUserUnit2_Double, UserUnitName.User02);
            //}

            //if (parameterModel_Input.OmniFinderModel_Save.CheckedUserUnit3_Bool == true)
            //{
                UserUnit user03 = new UserUnit("UserUnit03", "Usr03", parameterModel_Input.ParameterRangesModel_Save.MassUserUnit3_Double, UserUnitName.User03);
            //}
            userLibrary.SetLibrary(user01, user02, user03);// TODO this needs to be turned into a list


            return userLibrary;
        }

        private static void SetUser01(ParameterModel parameterModel_Input, List<BuildingBlockUserUnit> userBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedUserUnit1_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinUserUnit1_Int, parameterModel_Input.ParameterRangesModel_Save.MinUserUnit1_Int);
                userBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockUserUnit(PNNLOmics.Data.Constants.UserUnitName.User01, ranges));
            }
        }

        private static void SetUser02(ParameterModel parameterModel_Input, List<BuildingBlockUserUnit> userBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedUserUnit2_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinUserUnit2_Int, parameterModel_Input.ParameterRangesModel_Save.MaxUserUnit2_Int);
                userBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockUserUnit(PNNLOmics.Data.Constants.UserUnitName.User02, ranges));
            }
        }

        private static void SetUser03(ParameterModel parameterModel_Input, List<BuildingBlockUserUnit> userBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedUserUnit3_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinUserUnit3_Int, parameterModel_Input.ParameterRangesModel_Save.MaxUserUnit3_Int);
                userBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockUserUnit(PNNLOmics.Data.Constants.UserUnitName.User03, ranges));
            }
        }

        private static void SetUser04(ParameterModel parameterModel_Input, List<BuildingBlockUserUnit> userBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedUserUnit4_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinUserUnit4_Int, parameterModel_Input.ParameterRangesModel_Save.MaxUserUnit4_Int);
                userBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockUserUnit(PNNLOmics.Data.Constants.UserUnitName.User04, ranges));
            }
        }

        private static void SetUser05(ParameterModel parameterModel_Input, List<BuildingBlockUserUnit> userBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedUserUnit5_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinUserUnit5_Int, parameterModel_Input.ParameterRangesModel_Save.MaxUserUnit5_Int);
                userBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockUserUnit(PNNLOmics.Data.Constants.UserUnitName.User05, ranges));
            }
        }

        private static void SetUser06(ParameterModel parameterModel_Input, List<BuildingBlockUserUnit> userBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedUserUnit6_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinUserUnit6_Int, parameterModel_Input.ParameterRangesModel_Save.MaxUserUnit6_Int);
                userBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockUserUnit(PNNLOmics.Data.Constants.UserUnitName.User06, ranges));
            }
        }

        private static void SetUser07(ParameterModel parameterModel_Input, List<BuildingBlockUserUnit> userBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedUserUnit7_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinUserUnit7_Int, parameterModel_Input.ParameterRangesModel_Save.MaxUserUnit7_Int);
                userBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockUserUnit(PNNLOmics.Data.Constants.UserUnitName.User07, ranges));
            }
        }

        private static void SetUser08(ParameterModel parameterModel_Input, List<BuildingBlockUserUnit> userBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedUserUnit8_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinUserUnit8_Int, parameterModel_Input.ParameterRangesModel_Save.MaxUserUnit8_Int);
                userBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockUserUnit(PNNLOmics.Data.Constants.UserUnitName.User08, ranges));
            }
        }

        private static void SetUser09(ParameterModel parameterModel_Input, List<BuildingBlockUserUnit> userBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedUserUnit9_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinUserUnit9_Int, parameterModel_Input.ParameterRangesModel_Save.MaxUserUnit9_Int);
                userBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockUserUnit(PNNLOmics.Data.Constants.UserUnitName.User09, ranges));
            }
        }

        private static void SetUser10(ParameterModel parameterModel_Input, List<BuildingBlockUserUnit> userBlocks)
        {
            if (parameterModel_Input.OmniFinderModel_Save.CheckedUserUnit10_Bool == true)
            {
                RangesMinMax ranges = new RangesMinMax(parameterModel_Input.ParameterRangesModel_Save.MinUserUnit10_Int, parameterModel_Input.ParameterRangesModel_Save.MaxUserUnit10_Int);
                userBlocks.Add(new OmniFinder.Objects.BuildingBlocks.BuildingBlockUserUnit(PNNLOmics.Data.Constants.UserUnitName.User10, ranges));
            }
        }

    }
}

    
