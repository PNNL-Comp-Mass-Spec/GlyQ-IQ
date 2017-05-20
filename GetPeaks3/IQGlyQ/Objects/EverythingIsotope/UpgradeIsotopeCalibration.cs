using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaksDllLite.Functions;
using Run64.Backend.Core;

namespace IQGlyQ.Objects.EverythingIsotope
{
    public class UpgradeIsotopeCalibration: IUpgradeIsotope
    {
        public ParametersIsoCalibration Parameters { get; set; }

        public UpgradeOptions UpgradeType { get; set; }

        public void UpgradeMe(ref IsotopicProfile isotopeProfile)
        {
            IsotopicProfile theorIsotopicProfile = isotopeProfile;

            MassCalibration.Iso(ref theorIsotopicProfile, Parameters.DeltaMassCalibrationMZ, Parameters.DeltaMassCalibrationMono);
        }

        UpgradeIsotopeCalibration()
        {
            Parameters = new ParametersIsoCalibration();
        }

        public UpgradeIsotopeCalibration(ParametersIsoCalibration parameters)
            : this()
        {
            Parameters = parameters;
            UpgradeType = parameters.UpgradeType;
        }
    }

    public class ParametersIsoCalibration : IsotopeUpgradeParameters
    {
        //extra parameters go here

        /// <summary>
        /// shifts the isotope profile at the m/z level.  works if the curvature in the data is removed
        /// </summary>
        public double DeltaMassCalibrationMZ { get; set; }

        /// <summary>
        /// shifts the isotope profile at the monoisotopic mass level
        /// </summary>
        public double DeltaMassCalibrationMono { get; set; }

        public ParametersIsoCalibration()
        {
            Initialize();
        }

        public ParametersIsoCalibration(double deltaMassCalibrationMZ, double deltaMassCalibrationMono)
            :this()
        {
            DeltaMassCalibrationMZ = deltaMassCalibrationMZ;
            DeltaMassCalibrationMono = DeltaMassCalibrationMono;
        }

        private void Initialize()
        {
            UpgradeType = UpgradeOptions.Calibrate;
        }

    }
}
