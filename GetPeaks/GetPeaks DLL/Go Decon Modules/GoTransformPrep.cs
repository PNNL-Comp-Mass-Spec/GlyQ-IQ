using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using DeconTools.Backend;
using DeconTools.Utilities;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Objects.ResultsObjects;
using DeconToolsV2.HornTransform;

namespace GetPeaks_DLL.Go_Decon_Modules
{
    public class GoTransformPrep
    {
        //public void PrepDeconvolutor(Run run, TransformerObject transformer2, out float[] xvals, out float[] yvals, out DeconToolsV2.Peaks.clsPeak[] mspeakList, out DeconToolsV2.HornTransform.clsHornTransformResults[] transformResults2)
        public void PrepDeconvolutor(Run run, out float[] xvals, out float[] yvals, out DeconToolsV2.Peaks.clsPeak[] mspeakList, out DeconToolsV2.HornTransform.clsHornTransformResults[] transformResults2)        
        {

            xvals = new float[1];
            yvals = new float[1];
            //run.ResultCollection.Run.XYData.GetXYValuesAsSingles(ref xvals, ref yvals);
            GetXYValuesAsSingles(run, ref xvals, ref yvals);

            mspeakList = new DeconToolsV2.Peaks.clsPeak[run.ResultCollection.Run.DeconToolsPeakList.Length];
            for (int i = 0; i < run.ResultCollection.Run.DeconToolsPeakList.Length; i++)
            {
                DeconToolsV2.Peaks.clsPeak sPeak = run.ResultCollection.Run.DeconToolsPeakList[i];
                DeconToolsV2.Peaks.clsPeak newPeak = new DeconToolsV2.Peaks.clsPeak();
                newPeak.mdbl_FWHM = sPeak.mdbl_FWHM;
                newPeak.mdbl_intensity = sPeak.mdbl_intensity;
                newPeak.mdbl_mz = sPeak.mdbl_mz;
                newPeak.mdbl_SN = sPeak.mdbl_SN;
                //newPeak.mint_data_index = sPeak.mint_data_index;
                //newPeak.mint_peak_index = sPeak.mint_peak_index;
                mspeakList[i] = newPeak;
            }
            
            transformResults2 = new DeconToolsV2.HornTransform.clsHornTransformResults[0];
            //transformer2.TransformParameters = loadDeconEngineHornParameters();//pulled out front

            run.ResultCollection.IsosResultBin.Clear();
        }

        private void GetXYValuesAsSingles(Run run, ref float[] xvals, ref float[] yvals)
        {
            //NOTE going from double to single variables could result in a loss of information

            {
                xvals = new float[run.ResultCollection.Run.XYData.Xvalues.Length];
                yvals = new float[run.ResultCollection.Run.XYData.Yvalues.Length];

                for (int i = 0; i < run.ResultCollection.Run.XYData.Xvalues.Length; i++)
                {
                    xvals[i] = Convert.ToSingle(run.ResultCollection.Run.XYData.Xvalues[i]);
                    yvals[i] = Convert.ToSingle(run.ResultCollection.Run.XYData.Yvalues[i]);
                }
            }
        }

        public ResultCollectionLite FormatResults(List<DeconToolsV2.HornTransform.clsHornTransformResults> transformResults, DeconToolsV2.Peaks.clsPeak[] mspeakList, TransformerObject transformer2)
        {
            ResultCollectionLite resultList = new ResultCollectionLite();

            int NumIsotopicProfiles = 0;   //reset to 0;

            foreach (DeconToolsV2.HornTransform.clsHornTransformResults hornResult in transformResults)
            {
                //IsosResult result = resultList.CreateIsosResult();
                IsosResultLite result = new IsosResultLite();
                IsotopicProfile profile = result.IsotopicProfile;
                
                profile.AverageMass = hornResult.mdbl_average_mw;
                profile.ChargeState = hornResult.mshort_cs;
                profile.MonoIsotopicMass = hornResult.mdbl_mono_mw;
                profile.Score = hornResult.mdbl_fit;
                profile.MostAbundantIsotopeMass = hornResult.mdbl_most_intense_mw;

                GetIsotopicProfile(hornResult.marr_isotope_peak_indices, mspeakList, ref profile);

                int NumPeaksUsedInAbundance = transformer2.TransformEngine.TransformParameters.NumPeaksUsedInAbundance;
                if (NumPeaksUsedInAbundance == 1)  // fyi... this is typical
                {
                    profile.IntensityAggregateAdjusted = hornResult.mint_abundance;
                }
                else
                {
                    profile.IntensityAggregateAdjusted = sumPeaks(profile, NumPeaksUsedInAbundance, hornResult.mint_abundance, NumPeaksUsedInAbundance);
                }

                profile.MonoPlusTwoAbundance = profile.GetMonoPlusTwoAbundance();
                profile.MonoPeakMZ = profile.GetMZ();

                result.IsotopicProfile = profile;
                result.ChargeState = profile.ChargeState;
                CombineDeconResults(ref resultList, result, DeconResultComboMode.simplyAddIt);
                //resultList.IsosResultBin.Add(result);
                //resultList.ResultList.Add(result);
                NumIsotopicProfiles++;
            }

            return resultList;
        }


        public ResultCollectionLite FormatResults(List<DeconToolsV2.HornTransform.clsHornTransformResults> transformResults, DeconToolsV2.Peaks.clsPeak[] mspeakList, HornTransformParameters parameters, ResultCollection resultListDecon)
        {
            ResultCollectionLite resultList = new ResultCollectionLite();

            int NumIsotopicProfiles = 0;   //reset to 0;

            for (int i = 0; i < transformResults.Count; i++)
            {
                DeconToolsV2.HornTransform.clsHornTransformResults hornResult = transformResults[i];
                IsosResult isosInternal = resultListDecon.IsosResultBin[i];

                //IsosResult result = resultList.CreateIsosResult();
                IsosResultLite result = new IsosResultLite();
                IsotopicProfile profile = result.IsotopicProfile;

                profile.AverageMass = hornResult.mdbl_average_mw;
                profile.ChargeState = hornResult.mshort_cs;
                profile.MonoIsotopicMass = hornResult.mdbl_mono_mw;
                profile.Score = hornResult.mdbl_fit;
                profile.MostAbundantIsotopeMass = hornResult.mdbl_most_intense_mw;

                //GetIsotopicProfile(hornResult.marr_isotope_peak_indices, mspeakList, ref profile);
                GetIsotopicProfile(ref profile, isosInternal);

                int NumPeaksUsedInAbundance = parameters.NumPeaksUsedInAbundance;
                if (NumPeaksUsedInAbundance == 1) // fyi... this is typical
                {
                    profile.IntensityAggregateAdjusted = hornResult.mint_abundance;
                }
                else
                {
                    profile.IntensityAggregateAdjusted = sumPeaks(profile, NumPeaksUsedInAbundance, hornResult.mint_abundance,
                                                          NumPeaksUsedInAbundance);
                }

                profile.MonoPlusTwoAbundance = profile.GetMonoPlusTwoAbundance();
                profile.MonoPeakMZ = profile.GetMZ();

                result.IsotopicProfile = profile;
                result.ChargeState = profile.ChargeState;
                result.FitScore = hornResult.mdbl_fit;
                CombineDeconResults(ref resultList, result, DeconResultComboMode.simplyAddIt);

                

                NumIsotopicProfiles++;
            }

            return resultList;
        }

        public ResultCollectionLite FormatResults(DeconToolsV2.HornTransform.clsHornTransformResults[] transformResults, DeconToolsV2.Peaks.clsPeak[] mspeakList, TransformerObject transformer2)
        {
            ResultCollectionLite resultList = new ResultCollectionLite();

            int NumIsotopicProfiles = 0;   //reset to 0;

            foreach (DeconToolsV2.HornTransform.clsHornTransformResults hornResult in transformResults)
            {
                //IsosResult result = resultList.CreateIsosResult();
                IsosResultLite result = new IsosResultLite();
                IsotopicProfile profile = result.IsotopicProfile;

                profile.AverageMass = hornResult.mdbl_average_mw;
                profile.ChargeState = hornResult.mshort_cs;
                profile.MonoIsotopicMass = hornResult.mdbl_mono_mw;
                profile.Score = hornResult.mdbl_fit;
                profile.MostAbundantIsotopeMass = hornResult.mdbl_most_intense_mw;

                GetIsotopicProfile(hornResult.marr_isotope_peak_indices, mspeakList, ref profile);

                int NumPeaksUsedInAbundance = transformer2.TransformEngine.TransformParameters.NumPeaksUsedInAbundance;
                if (NumPeaksUsedInAbundance == 1)  // fyi... this is typical
                {
                    profile.IntensityAggregateAdjusted = hornResult.mint_abundance;
                }
                else
                {
                    profile.IntensityAggregateAdjusted = sumPeaks(profile, NumPeaksUsedInAbundance, hornResult.mint_abundance, NumPeaksUsedInAbundance);
                }

                profile.MonoPlusTwoAbundance = profile.GetMonoPlusTwoAbundance();
                profile.MonoPeakMZ = profile.GetMZ();

                result.IsotopicProfile = profile;
                result.ChargeState = profile.ChargeState;
                result.FitScore = hornResult.mdbl_fit;

                CombineDeconResults(ref resultList, result, DeconResultComboMode.simplyAddIt);
                //resultList.IsosResultBin.Add(result);
                //resultList.ResultList.Add(result);
                NumIsotopicProfiles++;
            }

            return resultList;
        }

        //TODO pull this out so we can use it publicly
        public List<TransformerObject> PreparArmyOfTransformers(GoTransformParameters transformerParameterSetup, int numberOfThreads, InputOutputFileName newFile, Object databaseLockMulti)
        {
            List<TransformerObject> transformerList = new List<TransformerObject>();

            for (int i = 0; i < numberOfThreads; i++)
            {
                string SQLiteFileName = "SimpleWorkFlowPart2Multi_" + Convert.ToString(i); ;
                string SQLiteFolder = newFile.OutputPath;

                TransformerObject newTransformer = new TransformerObject(transformerParameterSetup, SQLiteFileName, SQLiteFolder, databaseLockMulti);

                transformerList.Add(newTransformer);
            }

            return transformerList;
        }

        private void GetIsotopicProfile(int[] peakIndexList, DeconToolsV2.Peaks.clsPeak[] peakdata, ref IsotopicProfile profile)
        {
            if (peakIndexList == null || peakIndexList.Length == 0) return;
            DeconToolsV2.Peaks.clsPeak deconMonopeak = lookupPeak(peakIndexList[0], peakdata);

            MSPeak monoPeak = convertDeconPeakToMSPeak(deconMonopeak);
            profile.Peaklist.Add(monoPeak);

            if (peakIndexList.Length == 1) return;           //only one peak in the DeconEngine's profile    

            for (int i = 1; i < peakIndexList.Length; i++)     //start with second peak and add each peak to profile
            {
                DeconToolsV2.Peaks.clsPeak deconPeak = lookupPeak(peakIndexList[i], peakdata);
                MSPeak peakToBeAdded = convertDeconPeakToMSPeak(deconPeak);
                profile.Peaklist.Add(peakToBeAdded);
            }
        }

        private void GetIsotopicProfile(ref IsotopicProfile profile, IsosResult isosInternal)
        {
            if (isosInternal.IsotopicProfile.Peaklist == null || isosInternal.IsotopicProfile.Peaklist.Count == 0) return;

            List<MSPeak> peaklist = isosInternal.IsotopicProfile.Peaklist;

            for (int i = 0; i < peaklist.Count; i++)
            {
                MSPeak peakToBeAdded = new MSPeak();
                peakToBeAdded.XValue = peaklist[i].XValue;
                peakToBeAdded.Width = peaklist[i].Width;
                peakToBeAdded.SignalToNoise = peaklist[i].SignalToNoise;
                peakToBeAdded.MSFeatureID = peaklist[i].MSFeatureID;
                peakToBeAdded.Height = peaklist[i].Height;
                peakToBeAdded.DataIndex = peaklist[i].DataIndex;
                //convertDeconPeakToMSPeak(peaklist[i]);
                profile.Peaklist.Add(peakToBeAdded);
            }
        }

        private DeconToolsV2.Peaks.clsPeak lookupPeak(int index, DeconToolsV2.Peaks.clsPeak[] peakdata)
        {
            return peakdata[index];
        }

        private MSPeak convertDeconPeakToMSPeak(DeconToolsV2.Peaks.clsPeak deconPeak)
        {
            MSPeak peak = new MSPeak();
            peak.XValue = deconPeak.mdbl_mz;
            peak.Width = (float)deconPeak.mdbl_FWHM;
            peak.SignalToNoise = (float)deconPeak.mdbl_SN;
            peak.Height = (int)deconPeak.mdbl_intensity;

            return peak;
        }

        private double sumPeaks(IsotopicProfile profile, int NumPeaksUsedInAbundance, int defaultVal, int numPeaksUsedInAbundance)
        {
            if (profile.Peaklist == null || profile.Peaklist.Count == 0) return defaultVal;
            List<float> peakListIntensities = new List<float>();
            foreach (MSPeak peak in profile.Peaklist)
            {
                peakListIntensities.Add(peak.Height);

            }
            peakListIntensities.Sort();
            peakListIntensities.Reverse();    // i know... this isn't the best way to do this!
            double summedIntensities = 0;

            for (int i = 0; i < peakListIntensities.Count; i++)
            {
                if (i < numPeaksUsedInAbundance)
                {
                    summedIntensities += peakListIntensities[i];
                }
            }

            return summedIntensities;
        }

        private void CombineDeconResults(ref ResultCollectionLite baseResultList, IsosResultLite addedResult, DeconResultComboMode comboMode)
        {
            Check.Require(baseResultList != null, "Deconvolutor problem. Can't combine results. Base resultList is null.");
            Check.Require(addedResult != null, "Deconvolutor problem. Can't combine results. Added IsosResult is null.");

            switch (comboMode)
            {
                case DeconResultComboMode.simplyAddIt:
                    baseResultList.AddIsosResult(addedResult);
                    break;
                case DeconResultComboMode.addItIfUnique:

                    //retrieve IsosResults for CurrentScanSet
                    //TODO: next line might be a time bottleneck! needs checking
                    //List<IsosResult> scanSetIsosResults = ResultCollection.getIsosResultsForCurrentScanSet(baseResultList);

                    //search isosResults for a (monoPeak = addedResult's monoPeak) AND chargeState = addedResult's chargeState 
                    bool testExistante = DoesResultExist(addedResult, baseResultList);

                    if (testExistante)
                    {
                        //do nothing...  isotopic profile already exists
                    }
                    else
                    {
                        baseResultList.AddIsosResult(addedResult);
                    }
                    break;
                case DeconResultComboMode.addAndReplaceIfOneDaltonErrorDetected:
                    throw new NotImplementedException("add and replace isotopic profile mode not yet supported");
                default:
                    break;
            }
        }

        private static bool DoesResultExist(IsosResultLite addedResult, ResultCollectionLite resultsList)
        {
            bool testExistance = true;

            MSPeak addedMonoPeak;
            MSPeak baseMonoPeak;

            foreach (IsosResultLite result in resultsList.IsosResultBin)
            {
                addedMonoPeak = addedResult.IsotopicProfile.Peaklist[0];
                baseMonoPeak = result.IsotopicProfile.Peaklist[0];


                if (addedResult.IsotopicProfile.ChargeState == result.IsotopicProfile.ChargeState
                    && addedMonoPeak.XValue == baseMonoPeak.XValue)
                {
                    testExistance = true;   //found a match
                    return testExistance;
                }
            }
            //didn't find a matching monoisotopic peak
            testExistance = false;
            return testExistance;
        }

        private enum DeconResultComboMode
        {
            simplyAddIt,
            addItIfUnique,
            addAndReplaceIfOneDaltonErrorDetected
        }

        private bool doesResultExist(IList<IsosResult> scanSetIsosResults, IsosResult addedResult)
        {
            MSPeak addedMonoPeak;
            MSPeak baseMonoPeak;

            foreach (IsosResult result in scanSetIsosResults)
            {
                addedMonoPeak = addedResult.IsotopicProfile.Peaklist[0];
                baseMonoPeak = result.IsotopicProfile.Peaklist[0];


                if (addedResult.IsotopicProfile.ChargeState == result.IsotopicProfile.ChargeState
                    && addedMonoPeak.XValue == baseMonoPeak.XValue)
                {
                    return true;   //found a match
                }
            }
            //didn't find a matching monoisotopic peak
            return false;
        }


        public List<clsHornTransformResults> ResultsBinToHornList(int scan, ResultCollection resultList)
        {
            List<clsHornTransformResults> copiedResults = new List<clsHornTransformResults>();

            foreach (IsosResult iResult in resultList.IsosResultBin)
            {
                DeconToolsV2.HornTransform.clsHornTransformResults hornResult = new clsHornTransformResults();
                MSPeak newPeak = new MSPeak(iResult.IsotopicProfile.MonoIsotopicMass,
                                            Convert.ToSingle(iResult.IsotopicProfile.IntensityAggregateAdjusted),
                                            iResult.IsotopicProfile.Peaklist[0].Width, iResult.IsotopicProfile.Peaklist[0].SignalToNoise);

                hornResult.mdbl_mono_mw = newPeak.XValue;
                hornResult.mint_mono_intensity = (int)newPeak.Height;
                hornResult.mdbl_fwhm = newPeak.Width;
                hornResult.mdbl_sn = newPeak.SignalToNoise;
                hornResult.mdbl_mz = iResult.IsotopicProfile.Peaklist[0].XValue; //mz from first peak in peak list
                hornResult.mint_abundance = Convert.ToInt32(iResult.IsotopicProfile.Peaklist[0].Height);
                if (iResult.IsotopicProfile.Peaklist.Count > 2)
                {
                    hornResult.mint_iplus2_intensity = Convert.ToInt32(iResult.IsotopicProfile.Peaklist[2].Height);
                }
                else
                {
                    hornResult.mint_iplus2_intensity = 0;
                }

                hornResult.mdbl_average_mw = iResult.IsotopicProfile.AverageMass;
                hornResult.mdbl_fit = iResult.IsotopicProfile.Score;
                hornResult.mdbl_fwhm = iResult.IsotopicProfile.GetFWHM();
                hornResult.mint_num_isotopes_observed = iResult.IsotopicProfile.Peaklist.Count;
                hornResult.mint_scan_num = scan;
                hornResult.mshort_cs = Convert.ToSByte(iResult.IsotopicProfile.ChargeState);

                double mz = (iResult.IsotopicProfile.MonoIsotopicMass +
                             iResult.IsotopicProfile.ChargeState * 1.0072764) /
                            iResult.IsotopicProfile.ChargeState;

                hornResult.mdbl_most_intense_mw = mz; //calculated mz
                hornResult.mdbl_delta_mz = mz - hornResult.mdbl_mz;

                copiedResults.Add(hornResult);
            }

            return copiedResults;
        }
    }
}
