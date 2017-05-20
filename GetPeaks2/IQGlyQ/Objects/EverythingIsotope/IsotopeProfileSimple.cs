using System.Collections.Generic;
using IQ.Backend.ProcessingTasks.TheorFeatureGenerator;
using Run32.Backend.Core;

namespace IQGlyQ.Objects.EverythingIsotope
{
    public class IsotopeProfileSimple : IGenerateIsotopeProfile
    {
        #region properties

        public IsotopeGenerationModes IsoType { get; set; }

        public ParametersSimpleIsotope Parameters { get; set; }

        //private ITheorFeatureGenerator TheorFeatureGen { get; set; }
        
        #endregion

        #region methods

        public IsotopicProfile Generator(string empiricalFormula, int chargeState)
        {
            return GenerateSimpleWrapper(empiricalFormula, chargeState);
        }

        #endregion

        #region constructors

        private IsotopeProfileSimple(ITheorFeatureGenerator baseGenerator)
        {
            Parameters = new ParametersSimpleIsotope(baseGenerator);
            
        }

        public IsotopeProfileSimple(ParametersSimpleIsotope parameters)
            : this(parameters.TheorFeatureGen)
        {
            Parameters = parameters;

            IsoType = parameters.ProfileType;
        }

        #endregion

        private IsotopicProfile GenerateSimpleWrapper(string empiricalFormula, int chargeState)
        {
            IsotopicProfile iso = null;
            if (empiricalFormula != null)
            {
                iso = Parameters.TheorFeatureGen.GenerateTheorProfile(empiricalFormula, chargeState);
            }
            return iso;
        }
    }


    

    public class ParametersSimpleIsotope : IsotopeGeneratorFactory.IsotopeProfileParameters
    {
        //extra parameters go here


        public ParametersSimpleIsotope(ITheorFeatureGenerator theorFeatureGen)
        {
            Initialize();

            TheorFeatureGen = theorFeatureGen;

        }

        private void Initialize()
        {
            ProfileType = IsotopeGenerationModes.Simple;
            //ITheorFeatureGenerator TheorFeatureGen = new JoshTheorFeatureGenerator();
        }
    }
}
