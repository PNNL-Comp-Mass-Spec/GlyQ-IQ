using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using PNNLOmics.Data.Constants;

namespace GetPeaks_DLL.Functions
{
    public static class MassCalibration
    {
        public static void MassAndMono(ref double massToCharge, ref double monoisootpicMass, int chargeState, double calibrationMz, double deltaMassCalibrationMono)
        {
            //monoisootpicMass += calibrationMZ * chargeState;
            //massToCharge += calibrationMZ;
            monoisootpicMass += deltaMassCalibrationMono;

            massToCharge = (monoisootpicMass + Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic * chargeState) / chargeState;
            massToCharge += calibrationMz;
        }

        public static void ConvertMonoToCalMonoAndMZ(double nonCalibratedMono, int charge, double calibrationOffsetMZ, out double calibratedTheoreticalMono, out double calibratedTheoreticalMZ)
        {
            double massProton = Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic;
            calibratedTheoreticalMZ = ConvertMonoToMz.Execute(nonCalibratedMono, charge, massProton) + calibrationOffsetMZ;
            calibratedTheoreticalMono = ConvertMzToMono.Execute(calibratedTheoreticalMZ, charge, massProton);
        }

        public static void PeakListMZ(ref List<MSPeak> peaksMZ, double calibrationMZ)
        {
            foreach (var peak in peaksMZ)
            {
                peak.XValue += calibrationMZ;
            }
        }

        public static void PeakListMono(ref List<MSPeak> peaksMZ, double deltaMassCalibrationMono, int chargeState)
        {
            foreach (var peak in peaksMZ)
            {
                peak.XValue += (deltaMassCalibrationMono / chargeState);
            }
        }

        public static void Iso(ref IsotopicProfile iso, double calibrationMZ, double deltaMassCalibrationMono)
        {
            double massToCharge = iso.MonoPeakMZ;
            double monoisotopicMass = iso.MonoIsotopicMass;
            MassAndMono(ref massToCharge, ref monoisotopicMass, iso.ChargeState, calibrationMZ, deltaMassCalibrationMono);
            iso.MonoPeakMZ = massToCharge;
            iso.MonoIsotopicMass = monoisotopicMass;

            iso.MostAbundantIsotopeMass += calibrationMZ;

            List<MSPeak> currentPeakList = iso.Peaklist;
            PeakListMono(ref currentPeakList, deltaMassCalibrationMono, iso.ChargeState);

            
            PeakListMZ(ref currentPeakList, calibrationMZ);
        }
    }
}
