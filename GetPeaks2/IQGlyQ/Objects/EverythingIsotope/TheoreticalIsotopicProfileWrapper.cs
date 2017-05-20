using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GetPeaksDllLite.Functions;
using IQ.Backend.ProcessingTasks.TheorFeatureGenerator;
using IQ.Workflows.Core;
using IQGlyQ.Enumerations;
using Run32.Backend.Core;

namespace IQGlyQ.Objects.EverythingIsotope
{
    //factory
    public class TheoreticalIsotopicProfileWrapper
    {
        private IUpgradeIsotope Calibrate { get; set; }
        private IUpgradeIsotope Shifter { get; set; }

        public TheoreticalIsotopicProfileWrapper(ParametersIsoCalibration calibrateParameters, ParametersIsoShift shiftParameters)
        {
            Calibrate = new UpgradeIsotopeCalibration(calibrateParameters);
            Shifter = new UpgradeIsotopeShift(shiftParameters);
        }

        public TheoreticalIsotopicProfileWrapper()
        {

        }

        //public void Generate(ref IGenerateIsotopeProfile TheorFeatureGenV2, IqTarget target, EnumerationIsotopicProfileMode switchOption, double deltaMassCalibrationMZ, double deltaMassCalibrationMono, bool toMassCalibrate, EnumerationIsotopePenaltyMode penaltySwitch)
        public void Generate(ref IGenerateIsotopeProfile TheorFeatureGenV2, IqTarget target, EnumerationIsotopicProfileMode switchOption, bool toMassCalibrate, bool toShift)
        {
            Generate(ref TheorFeatureGenV2, target, switchOption, toMassCalibrate, toShift, 0.5f);
        }

        public void Generate(ref IGenerateIsotopeProfile TheorFeatureGenV2, IqTarget target, EnumerationIsotopicProfileMode switchOption,bool toMassCalibrate, bool toShift, float mixingFraction)
        {
            
            //step 1 is generated a non calibrated non modified iso
            CreateNewProfile(TheorFeatureGenV2, target, switchOption, mixingFraction);

            IsotopicProfile theorIsotopicProfile = target.TheorIsotopicProfile;

            //step 2 calibrate
            if (toMassCalibrate){Calibrate.UpgradeMe(ref theorIsotopicProfile);}
            
            //step 3 introduce penalty peaks
            if (toShift){Shifter.UpgradeMe(ref theorIsotopicProfile);}
        }

        private static void CreateNewProfile(IGenerateIsotopeProfile TheorFeatureGenV2, IqTarget target, EnumerationIsotopicProfileMode switchOption, float mixingFractionIn)
        {
            switch (switchOption)
            {
                case EnumerationIsotopicProfileMode.H:
                    {
                        //old
                        //target.TheorIsotopicProfile = GenerateSimpleOld(ref TheorFeatureGen,target.EmpiricalFormula,target.ChargeState);


                        //new
                        //var simpleParameters = new ParametersSimpleIsotope(TheorFeatureGen);
                        //IGenerateIsotopeProfile TheorFeatureGenV2 = new IsotopeProfileSimple(simpleParameters);
                        target.TheorIsotopicProfile = TheorFeatureGenV2.Generator(target.EmpiricalFormula, target.ChargeState);
                        target.TheorIsotopicProfile.AbundanceC13asFraction = target.TheorIsotopicProfile.Peaklist[1].Height;
                        target.TheorIsotopicProfile.AbundanceC14asFraction = target.TheorIsotopicProfile.Peaklist[2].Height;
                        target.TheorIsotopicProfile.AbundanceC15asFraction = target.TheorIsotopicProfile.Peaklist[2].Height;
                        target.TheorIsotopicProfile.RefreshAlternatePeakIntenstiesFromPeakList();
                        
                    }
                    break;

                case EnumerationIsotopicProfileMode.DH:
                    {
                        //old
                        //target.TheorIsotopicProfile = GenerateSimpleOld(ref TheorFeatureGen,target.EmpiricalFormula,target.ChargeState);
                        //target.TheorIsotopicProfile = GenerateCombinedIsotopicProfile(target.TheorIsotopicProfile, 1, 1);

                        //new
                        IsotopeProfileBlended localGenerator = (IsotopeProfileBlended)TheorFeatureGenV2;

                        //1.  store simple distiribution as the fullprofile and store these intensities in the peaklist
                        List<int> offsetsSimple = new int[] { 0 }.ToList();
                        List<float> mixingFractionSimple = new float[] { 0 }.ToList();

                        target.TheorIsotopicProfile = localGenerator.Generator(target.EmpiricalFormula, target.ChargeState, ref offsetsSimple, ref mixingFractionSimple);
                        
                        //2.  store d/h blended as intensities only
                        //as mixing fraction increases, you increase the contribution of the heavy lable profile
                        //float defaultMixingFraction = 0.5f;//50:D 50:H
                        float mixingFractiontoUse = mixingFractionIn;
                        int defaultOffset = 1;//1 Neutron

                        List<int> offsets = new int[] { defaultOffset }.ToList();
                        List<float> mixingFraction = new float[] { mixingFractiontoUse }.ToList();

                        IsotopicProfile tempProfile = localGenerator.Generator(target.EmpiricalFormula, target.ChargeState, ref offsets, ref mixingFraction);
                        target.TheorIsotopicProfile.AbundanceC13asFraction = target.TheorIsotopicProfile.Peaklist[1].Height;
                        target.TheorIsotopicProfile.AbundanceC14asFraction = target.TheorIsotopicProfile.Peaklist[2].Height;
                        target.TheorIsotopicProfile.AbundanceC15asFraction = target.TheorIsotopicProfile.Peaklist[3].Height;
                        target.TheorIsotopicProfile.SetAlternatePeakIntensities(tempProfile.Peaklist);
                    }
                    break;
                default:
                    {
                        //target.TheorIsotopicProfile = GenerateSimpleOld(ref TheorFeatureGen,target.EmpiricalFormula,target.ChargeState);
                        target.TheorIsotopicProfile = TheorFeatureGenV2.Generator(target.EmpiricalFormula, target.ChargeState);
                        target.TheorIsotopicProfile.AbundanceC13asFraction = target.TheorIsotopicProfile.Peaklist[1].Height;
                        target.TheorIsotopicProfile.AbundanceC14asFraction = target.TheorIsotopicProfile.Peaklist[2].Height;
                        target.TheorIsotopicProfile.AbundanceC15asFraction = target.TheorIsotopicProfile.Peaklist[2].Height;
                        target.TheorIsotopicProfile.RefreshAlternatePeakIntenstiesFromPeakList();
                        Console.WriteLine("Missing Selection Utilitiles in CreateNewProfile");
                        System.Threading.Thread.Sleep(3000);
                    }
                    break;
            }
        }

        public static IsotopicProfile GenerateSimpleOld(ref ITheorFeatureGenerator theorFeatureGen, string empiricalFormula, int chargeState)
        {
            IsotopicProfile iso = null;
            if (empiricalFormula != null)
            {
                iso = theorFeatureGen.GenerateTheorProfile(empiricalFormula, chargeState);
            }
            return iso;
        }

        public static IsotopicProfile Add1DaPointsToIso(IsotopicProfile iso, int numberOfPoints)
        {
            double deltaX = 0;
            float zeroPointAbundance = 0.00f;

            if (iso != null && iso.Peaklist.Count > 1)
            {
                deltaX = iso.Peaklist[1].XValue - iso.Peaklist[0].XValue;

                for (int i = 0; i < numberOfPoints; i++)
                {
                    iso.Peaklist.Insert(0, new MSPeak(iso.Peaklist[0].XValue - deltaX, zeroPointAbundance, 0, 0));
                }
            }

            if (iso.AlternatePeakIntensities.Length > 0)
            {
                List<float> tempList = iso.AlternatePeakIntensities.ToList();

                for (int i = 0; i < numberOfPoints; i++)
                {
                    tempList.Insert(0, zeroPointAbundance);
                }

                iso.SetAlternatePeakIntensities(tempList.ToArray());
            }

            return iso;
        }


        //this is the old way I think
        public static IsotopicProfile GenerateCombinedIsotopicProfile(IsotopicProfile iso, int isotopeOffset, float ratioProfile2)
        {
            IsotopicProfile comboProfile = new IsotopicProfile();

            List<MSPeak> comboPeakList = new List<MSPeak>();
            for (int i = 0; i < isotopeOffset; i++)
            {
                MSPeak profile1Peak = iso.Peaklist[i];
                comboPeakList.Add(new MSPeak(profile1Peak.XValue, profile1Peak.Height, profile1Peak.Width, profile1Peak.SignalToNoise));
            }

            for (int i = isotopeOffset; i < iso.Peaklist.Count; i++)
            {
                MSPeak profile1Peak = iso.Peaklist[i - isotopeOffset];
                MSPeak profile2Peak = iso.Peaklist[i];

                comboPeakList.Add(new MSPeak(
                                      profile2Peak.XValue,
                                      profile1Peak.Height + ratioProfile2 * profile2Peak.Height,
                                      profile1Peak.Width,
                                      profile1Peak.SignalToNoise));
            }

            float maxHeight = comboPeakList.Max(r => r.Height); //height
            foreach (var msPeak in comboPeakList)
            {
                msPeak.Height = msPeak.Height / maxHeight;
            }

            comboProfile = iso.CloneIsotopicProfile();//when generatring, we do not want to establish yet
           
            comboProfile.Peaklist = comboPeakList;

            return comboProfile;
        }
    }
}
