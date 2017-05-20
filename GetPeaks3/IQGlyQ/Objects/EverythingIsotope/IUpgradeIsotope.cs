using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Run64.Backend.Core;

namespace IQGlyQ.Objects.EverythingIsotope
{
    public interface IUpgradeIsotope
    {
        void UpgradeMe(ref IsotopicProfile isotopeProfile);
    }

    /// <summary>
    /// optional isotopegeneration modes are listed here
    /// </summary>
    public enum UpgradeOptions
    {
        Calibrate,
        Shift,
    }

    /// <summary>
    /// here is where we make fitscore calcuators of different types
    /// </summary>
    public static class IsotopeUpgradeFactory
    {
        public static IUpgradeIsotope Build(IsotopeUpgradeParameters parameters)
        {
            IUpgradeIsotope profileWorkShop;
            switch (parameters.UpgradeType)
            {
                case UpgradeOptions.Calibrate:
                    {
                        profileWorkShop = new UpgradeIsotopeCalibration((ParametersIsoCalibration)parameters);
                    }
                    break;
                case UpgradeOptions.Shift:
                    {
                        profileWorkShop = new UpgradeIsotopeShift((ParametersIsoShift)parameters);
                    }
                    break;
                default:
                    {
                        profileWorkShop = new UpgradeIsotopeShift((ParametersIsoShift)parameters);
                    }
                    break;
            }

            return profileWorkShop;
        }
    }

    public abstract class IsotopeUpgradeParameters
    {
        public UpgradeOptions UpgradeType { get; set; }
    }
}
