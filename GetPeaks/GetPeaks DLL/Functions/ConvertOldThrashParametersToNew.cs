using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend;
using DeconTools.Backend.Parameters;
using GetPeaks_DLL.Go_Decon_Modules;

namespace GetPeaks_DLL.Functions
{
    public static class ConvertOldThrashParametersToNew
    {
        public static void OldToNew(ParametersTHRASH transformerParameterSetup, ref ThrashParameters newVersion)
        {
            //convert parameterfile new
            //parameters.HornTransformParameters = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters;
            newVersion.AbsolutePeptideIntensity = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.AbsolutePeptideIntensity;
            newVersion.AveragineFormula = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.AveragineFormula;
            newVersion.ChargeCarrierMass = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.CCMass;
            newVersion.CheckAllPatternsAgainstChargeState1 = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.CheckAllPatternsAgainstCharge1;
            newVersion.CompleteFit = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.CompleteFit;

            //transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.DeleteIntensityThreshold;
            //transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.DetectPeaksOnlyWithNoDeconvolution;
            //transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.ElementIsotopeComposition;
            //transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.IsActualMonoMZUsed;
            newVersion.IsO16O18Data = false;
            newVersion.IsotopicProfileFitType = Globals.IsotopicProfileFitType.AREA;
            newVersion.LeftFitStringencyFactor = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.LeftFitStringencyFactor;
            newVersion.MaxCharge = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.MaxCharge;
            newVersion.MaxFit = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.MaxFit;
            newVersion.MaxMass = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.MaxMW;
            newVersion.MinIntensityForDeletion = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.MinIntensityForScore;
            newVersion.MinMSFeatureToBackgroundRatio = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.MinS2N;
            newVersion.NumPeaksForShoulder = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.NumPeaksForShoulder;
            newVersion.NumPeaksUsedInAbundance = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.NumPeaksUsedInAbundance;
            newVersion.RightFitStringencyFactor = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.RightFitStringencyFactor;
            newVersion.TagFormula = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.TagFormula;
            newVersion.UseAbsoluteIntensity = transformerParameterSetup.DeisotopingParametersThrash.DeconThrashParameters.UseAbsolutePeptideIntensity;


        }
    }
}
