using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQGlyQ.Enumerations;
using Run32.Backend.Core;

namespace IQGlyQ.Objects.EverythingIsotope
{
    public class UpgradeIsotopeShift: IUpgradeIsotope
    {
        public ParametersIsoShift Parameters { get; set; }

        public UpgradeOptions UpgradeType { get; set; }

        public void UpgradeMe(ref IsotopicProfile isotopeProfile)
        {
            IsotopicProfile theorIsotopicProfile = isotopeProfile;

            switch (Parameters.PenaltySwitch)
            {
                case EnumerationIsotopePenaltyMode.PointToLeft:
                    {
                        AddFirstPeak(theorIsotopicProfile);
                    }
                    break;
                case EnumerationIsotopePenaltyMode.Harmonic:
                    {
                        //if adding multiple harmonics, start from the back
                        int indexToAdd = 0; //0 will put a peak between 0 and 1
                        AddHarmonic(indexToAdd, theorIsotopicProfile);
                    }
                    break;
                case EnumerationIsotopePenaltyMode.PointToLeftAndHarmonic:
                    {
                        AddFirstPeak(theorIsotopicProfile);

                        int indexToAdd = 2; //0 will put a peak between 0 and 1.  1 might be more accurate than 2
                        AddHarmonic(indexToAdd, theorIsotopicProfile);
                    }
                    break;
            }
        }


        /// <summary>
        /// this probably does not work
        /// </summary>
        /// <param name="indexToAdd"></param>
        /// <param name="iso"></param>
        private static void AddHarmonic(int indexToAdd, IsotopicProfile iso)
        {
            if (iso.Peaklist != null && iso.Peaklist.Count > 0)
            {
                List<float> tempList = iso.AlternatePeakIntensities.ToList();

                double peakSpacing = iso.Peaklist[1].XValue - iso.Peaklist[0].XValue;
                double newMass = iso.Peaklist[indexToAdd].XValue + (peakSpacing / 2);
                //place a new point right between two peaks. this should be extended to the most abundant location
                iso.Peaklist.Insert(indexToAdd, new MSPeak(newMass, 0, 0, 0));
                tempList.Insert(indexToAdd, 0);
                iso.SetAlternatePeakIntensities(tempList.ToArray());
            }


        }

        private static void AddFirstPeak(IsotopicProfile iso)
        {
            if (iso.Peaklist != null && iso.Peaklist.Count > 0)
            {
                if (iso.Peaklist.Count == 1)
                {
                    double peakSpacing = 1;
                    double newMass = iso.Peaklist[0].XValue - (peakSpacing);
                    iso.Peaklist.Insert(0, new MSPeak(newMass, 0, 0, 0));
                }
                else
                {
                    double peakSpacing = iso.Peaklist[1].XValue - iso.Peaklist[0].XValue;
                    double newMass = iso.Peaklist[0].XValue - (peakSpacing);
                    iso.Peaklist.Insert(0, new MSPeak(newMass, 0, 0, 0));
                }

                if (iso.AlternatePeakIntensities.Length > 0)
                {
                    List<float> tempList = iso.AlternatePeakIntensities.ToList();
                    tempList.Insert(0, 0);
                    iso.SetAlternatePeakIntensities(tempList.ToArray());
                }
            }
        }

        
        UpgradeIsotopeShift()
        {
            Parameters = new ParametersIsoShift();
        }

        public UpgradeIsotopeShift(ParametersIsoShift parameters)
            : this()
        {
            Parameters = parameters;
            UpgradeType = parameters.UpgradeType;
        }
    }

    public class ParametersIsoShift : IsotopeUpgradeParameters
    {
        //extra parameters go here

        /// <summary>
        /// what sort of shifting do we want to do
        /// </summary>
        public EnumerationIsotopePenaltyMode PenaltySwitch { get; set; }

        /// <summary>
        /// how far do we want to ofset the profile.  each offset counts as a penality if a peak is present
        /// </summary>
        public int NumberOfPeaksToLeftForPenalty { get; set; }

        public ParametersIsoShift()
        {
            Initialize();
        }

        public ParametersIsoShift(EnumerationIsotopePenaltyMode penaltySwitch, int numberOfPeaksToLeftForPenalty)
            :this()
        {
            PenaltySwitch = penaltySwitch;
            NumberOfPeaksToLeftForPenalty = numberOfPeaksToLeftForPenalty;
        }

        private void Initialize()
        {
            UpgradeType = UpgradeOptions.Shift;
        }
    }
}
