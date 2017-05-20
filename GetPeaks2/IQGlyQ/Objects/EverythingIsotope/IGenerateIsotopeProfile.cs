using IQ.Backend.ProcessingTasks.TheorFeatureGenerator;
using Run32.Backend.Core;

namespace IQGlyQ.Objects.EverythingIsotope
{
    public interface IGenerateIsotopeProfile
    {
        IsotopicProfile Generator(string empiricalFormula, int chargeState);
    }

    /// <summary>
    /// optional isotopegeneration modes are listed here
    /// </summary>
    public enum IsotopeGenerationModes
    {
        Simple,
        Blended,
    }

    /// <summary>
    /// here is where we make fitscore calcuators of different types
    /// </summary>
    public static class IsotopeGeneratorFactory
    {
        public static IGenerateIsotopeProfile Build(IsotopeProfileParameters parameters)
        {
            IGenerateIsotopeProfile profileCalcuator;
            switch (parameters.ProfileType)
            {
                case IsotopeGenerationModes.Simple:
                    {
                        profileCalcuator = new IsotopeProfileSimple((ParametersSimpleIsotope)parameters);
                    }
                    break;
                case IsotopeGenerationModes.Blended:
                    {
                        profileCalcuator = new IsotopeProfileBlended((ParametersBlendedIsotope)parameters);
                    }
                    break;
               
                default:
                    {
                        profileCalcuator = new IsotopeProfileSimple((ParametersSimpleIsotope)parameters);
                    }
                    break;
            }

            return profileCalcuator;
        }

        /// <summary>
        /// a base class for all fit score parameters.  each new fit score needs to inherit these and must have an enumerated type
        /// </summary>
        public abstract class IsotopeProfileParameters
        {
            internal IsotopeGenerationModes ProfileType { get; set; }
           
            internal ITheorFeatureGenerator TheorFeatureGen { get; set; }
        }
    }
}
