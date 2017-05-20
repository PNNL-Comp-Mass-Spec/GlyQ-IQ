using System;
using System.Collections.Generic;
using System.Linq;
using IQ.Backend.ProcessingTasks.TheorFeatureGenerator;
using Run32.Backend.Core;

namespace IQGlyQ.Objects.EverythingIsotope
{
    public class IsotopeProfileBlended : IGenerateIsotopeProfile
    {
        #region properties

        public IsotopeGenerationModes IsoType { get; set; }

        public ParametersBlendedIsotope Parameters { get; set; }

        #endregion

        #region methods

        public IsotopicProfile Generator(string empiricalFormula,int chargeState)
        {
            return GeneratorFromExisting(Parameters.localSimpleGenerator.Generator(empiricalFormula, chargeState));
        }

        public IsotopicProfile Generator(string empiricalFormula, int chargeState , ref List<int> offsets, ref List<float> relativeRatiosToMainIsotope)
        {
            
            Parameters.Offsets = offsets;
            Parameters.MixingFraction = relativeRatiosToMainIsotope;
            Parameters.AdditionalIonsToMultiplex = Parameters.Offsets.Count;
            return GeneratorFromExisting(Parameters.localSimpleGenerator.Generator(empiricalFormula, chargeState));
        }

        public IsotopicProfile GeneratorFromExisting(IsotopicProfile isotopeProfile, ref List<int> offsets, ref List<float> relativeRatiosToMainIsotope)
        {
            Parameters.Offsets = offsets;
            Parameters.MixingFraction = relativeRatiosToMainIsotope;
            Parameters.AdditionalIonsToMultiplex = Parameters.Offsets.Count;
            return GeneratorFromExisting(isotopeProfile);

        }

        public IsotopicProfile GeneratorFromExisting(IsotopicProfile isotopeProfile)
        {
            if (Parameters.AdditionalIonsToMultiplex > 0)
            {
                
                
                IsotopicProfile comboProfile = new IsotopicProfile();
                for (int j = 0; j < Parameters.AdditionalIonsToMultiplex; j++)
                {
                    int isotopeOffset = Parameters.Offsets[j];
                    float mixingFraction = Parameters.MixingFraction[j];

                    //as mixing fraction increases, you increase the contribution of the heavy lable profile
                    float pctProfile1 = 1 - mixingFraction;
                    float pctProfile2 = mixingFraction;

                    //1.  store existing datapoints that will not be added
                    List<MSPeak> comboPeakList = new List<MSPeak>();
                    for (int i = 0; i < isotopeOffset; i++)
                    {
                        MSPeak profile1Peak = isotopeProfile.Peaklist[i];
                        comboPeakList.Add(new MSPeak(profile1Peak.XValue, profile1Peak.Height * pctProfile1, profile1Peak.Width, profile1Peak.SignalToNoise));
                    }

                    //2.  add on rest of peaks that wil be an addition of intensties
                    for (int i = isotopeOffset; i < isotopeProfile.Peaklist.Count; i++)
                    {
                        MSPeak profile1Peak = isotopeProfile.Peaklist[i - isotopeOffset];
                        MSPeak profile2Peak = isotopeProfile.Peaklist[i];

                        comboPeakList.Add(new MSPeak(
                                              profile2Peak.XValue,
                                              profile2Peak.Height * pctProfile1 + profile1Peak.Height * pctProfile2,
                                              profile1Peak.Width,
                                              profile1Peak.SignalToNoise));
                    }

                    float maxHeight = comboPeakList.Max(r => r.Height); //height
                    foreach (var msPeak in comboPeakList)
                    {
                        msPeak.Height = msPeak.Height/maxHeight;
                    }

                    comboProfile = isotopeProfile.CloneIsotopicProfile();//when generatring, we don't want to establish yet
                    comboProfile.Peaklist = comboPeakList;
                }

                return comboProfile;
            }

            return isotopeProfile;//no change
        }

        #endregion

        #region constructors

        private IsotopeProfileBlended(ParametersSimpleIsotope simpleIsotopeParameters)
        {
            Parameters = new ParametersBlendedIsotope(simpleIsotopeParameters);
        }

        public IsotopeProfileBlended(ParametersBlendedIsotope parameters)
            : this(parameters.SimpleParameters)
        {
            Parameters = parameters;
            IsoType = parameters.ProfileType;
        }

        #endregion
    }

    public class ParametersBlendedIsotope : IsotopeGeneratorFactory.IsotopeProfileParameters
    {
        //extra parameters go here

        /// <summary>
        /// 2 means that 2 isotope profiles will be added together
        /// </summary>
        public int AdditionalIonsToMultiplex { get; set; }

        public List<int> Offsets { get; set; }

        /// <summary>
        /// 0-1
        /// </summary>
        public List<float> MixingFraction { get; set; }
        public ParametersSimpleIsotope SimpleParameters { get; set; }
        public IsotopeProfileSimple localSimpleGenerator { get; set; }

        public ParametersBlendedIsotope(ParametersSimpleIsotope simpleParameters)
        {
            Initialize();

            AdditionalIonsToMultiplex = 0;
            Offsets = new List<int>();
            MixingFraction = new List<float>();
            SimpleParameters = simpleParameters;
            localSimpleGenerator = new IsotopeProfileSimple(SimpleParameters);
        }

        private void Initialize()
        {
            ProfileType = IsotopeGenerationModes.Blended;
        }
    }
}
