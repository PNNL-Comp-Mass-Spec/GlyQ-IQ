using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Objects.Enumerations;
using GlycolyzerGUImvvm.Models;

namespace GetPeaks_DLL.Glycolyzer.ParameterConverters
{
    public static class ConvertAdduct
    {
        public static Adducts Convert(string adductTypeString)
        {
            Adducts result = Adducts.Neutral;

            switch (adductTypeString)
            {
                case "DeProtonated":
                    {
                        result = Adducts.DeProtonated;
                    }
                    break;
                case "H":
                    {
                        result = Adducts.H;
                    }
                    break;
                case "K":
                    {
                        result = Adducts.K;
                    }
                    break;
                case "Monoisotopic":
                    {
                        result = Adducts.Monoisotopic;
                    }
                    break;
                case "Na":
                    {
                        result = Adducts.Na;
                    }
                    break;
                case "Neutral":
                    {
                        result = Adducts.Neutral;
                    }
                    break;
                case "NH4":
                    {
                        result = Adducts.NH4;
                    }
                    break;
                case "UserDefined":
                    {
                        result = Adducts.UserDefined;
                    }
                    break;
                case "Water":
                    {
                        result = Adducts.Water;
                    }
                    break;
            }

            return result;
        }
    }
}
