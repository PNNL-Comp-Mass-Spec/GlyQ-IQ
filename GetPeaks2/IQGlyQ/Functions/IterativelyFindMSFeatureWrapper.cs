using System.Collections.Generic;
using GetPeaksDllLite.Functions;
using IQ.Backend.ProcessingTasks.TargetedFeatureFinders;
using Run32.Backend.Core;

namespace IQGlyQ.Functions
{
    public class IterativelyFindMSFeatureWrapper
    {

        //private static IsotopicProfile IterativeFeatureFinderWrapper(FragmentIQTarget target, IqResult iQresult, IterativeTFF msfeatureFinder)
        public static IsotopicProfile IterativeFeatureFind(IsotopicProfile targetProfile, Run32.Backend.Data.XYData massSpecXyData, IterativeTFF msfeatureFinder)
        {
            //this is needed to feed into the legacy IQ2_function 
            //if we don't copy, we will lost our simppe isotope profile on targetProfile
            
            IsotopicProfile localIso = targetProfile.CloneIsotopicProfile();
            if (targetProfile.isEstablished)
            {
                localIso.EstablishAlternatvePeakIntensites(targetProfile.EstablishedMixingFraction);//keep it established

            }
            localIso.UpdatePeakListWithAlternatePeakIntensties();//this is needed to feed into the legacy IQ2_function.  
            

            IsotopicProfile isotopicProfileObserved;
            List<Peak> mspeakList;
            if (targetProfile.Peaklist[0].Height == 0)//penalty fraction case where we need extra work to populate the extra penalty spots with data if it exists
            {
                MSPeak firstPeak = targetProfile.Peaklist[0];
                List<MSPeak> clippedPeakList = new List<MSPeak>();

                for (int i = 1; i < targetProfile.Peaklist.Count; i++)
                {
                    clippedPeakList.Add(targetProfile.Peaklist[i]);
                }
                localIso.Peaklist = clippedPeakList;
                //we need to remove the first one or the wholething can fail and kill the IQ if it comes up empty.

                isotopicProfileObserved = msfeatureFinder.IterativelyFindMSFeature(massSpecXyData, localIso, out mspeakList);

                //we still need to populate thefirst peak in ObservedIsotopicProfile
                if (isotopicProfileObserved != null)
                {
                    Peak closestPeak = SelectClosest.SelectNearestPeakToCenter(mspeakList, firstPeak.XValue);
                    double howCloseAreWeInPPM = ErrorCalculator.PPMAbsolute(closestPeak.XValue, firstPeak.XValue);
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

                isotopicProfileObserved = msfeatureFinder.IterativelyFindMSFeature(massSpecXyData, localIso, out mspeakList);
            }
            return isotopicProfileObserved;
        }
    }
}
