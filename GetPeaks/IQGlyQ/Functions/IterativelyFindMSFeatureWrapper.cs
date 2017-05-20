using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks.TargetedFeatureFinders;

namespace IQGlyQ.Functions
{
    public class IterativelyFindMSFeatureWrapper
    {

        //private static IsotopicProfile IterativeFeatureFinderWrapper(FragmentIQTarget target, IqResult iQresult, IterativeTFF msfeatureFinder)
        public static IsotopicProfile IterativeFeatureFind(IsotopicProfile targetProfile, DeconTools.Backend.XYData massSpecXyData, IterativeTFF msfeatureFinder)
        {
            IsotopicProfile isotopicProfileObserved;
            List<Peak> mspeakList;
            if (targetProfile.Peaklist[0].Height == 0)
            {
                MSPeak firstPeak = targetProfile.Peaklist[0];
                IsotopicProfile clippedTheoryIso = targetProfile.CloneIsotopicProfile();
                List<MSPeak> clippedPeakList = new List<MSPeak>();

                for (int i = 1; i < targetProfile.Peaklist.Count; i++)
                {
                    clippedPeakList.Add(targetProfile.Peaklist[i]);
                }
                clippedTheoryIso.Peaklist = clippedPeakList;
                //we need to remove the first one or the wholething can fail and kill the IQ if it comes up empty.

                isotopicProfileObserved = msfeatureFinder.IterativelyFindMSFeature(massSpecXyData, clippedTheoryIso, out mspeakList);

                //we still need to populate thefirst peak in ObservedIsotopicProfile
                if (isotopicProfileObserved != null)
                {
                    Peak closestPeak = SelectClosest.SelectNearestPeakToCenter(mspeakList, firstPeak.XValue);
                    double howCloseAreWeInPPM = GetPeaks_DLL.Functions.ErrorCalculator.PPMAbsolute(closestPeak.XValue, firstPeak.XValue);
                    if (howCloseAreWeInPPM < msfeatureFinder.ToleranceInPPM)
                    {
                        isotopicProfileObserved.Peaklist.Insert(0, new MSPeak(closestPeak.XValue, closestPeak.Height, closestPeak.Width, 1));
                    }
                    else
                    {
                        isotopicProfileObserved.Peaklist.Insert(0, new MSPeak(firstPeak.XValue, 0, 0, 1));
                    }
                }
            }
            else
            {
                //iQresult.ObservedIsotopicProfile = _msfeatureFinder.IterativelyFindMSFeature(iQresult.IqResultDetail.MassSpectrum, target.TheorIsotopicProfile, out mspeakList);
                isotopicProfileObserved = msfeatureFinder.IterativelyFindMSFeature(massSpecXyData, targetProfile, out mspeakList);
            }
            return isotopicProfileObserved;
        }
    }
}
