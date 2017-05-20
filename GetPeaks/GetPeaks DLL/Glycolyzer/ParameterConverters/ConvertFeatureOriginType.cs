using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Glycolyzer.Enumerations;

namespace GetPeaks_DLL.Glycolyzer.ParameterConverters
{
    public static class ConvertFeatureOriginType
    {
        public static FeatureOriginType Convert(string featureTypeName)
        {
            FeatureOriginType result = FeatureOriginType.Viper;

            switch (featureTypeName)
            {
                case "IMS":
                    {
                        result = FeatureOriginType.IMS;
                    }
                    break;
                case "Multialign":
                    {
                        result = FeatureOriginType.Multialign;
                    }
                    break;
                case "Viper":
                    {
                        result = FeatureOriginType.Viper;
                    }
                    break;
               
            }

            return result;
        }

    }
}
