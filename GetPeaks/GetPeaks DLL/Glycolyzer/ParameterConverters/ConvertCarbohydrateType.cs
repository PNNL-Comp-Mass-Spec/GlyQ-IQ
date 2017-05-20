using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Objects.Enumerations;
using GlycolyzerGUImvvm.Models;

namespace GetPeaks_DLL.Glycolyzer.ParameterConverters
{
    public static class ConvertCarbohydrateType
    {
        public static CarbType Convert(string carbohydrateTypeString)
        {
            CarbType result = CarbType.Aldehyde;

            switch (carbohydrateTypeString)
            {
                case "Aldehyde":
                    {
                        result = CarbType.Aldehyde;
                    }
                    break;
                case "Alditol":
                    {
                        result = CarbType.Alditol;
                    }
                    break;
                case "Fragment":
                    {
                        result = CarbType.Fragment;
                    }
                    break;
            }

            return result;
        }
    }
}
