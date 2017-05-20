using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using System.IO;
using GetPeaks_DLL.CompareContrast;
using GetPeaks_DLL.SQLite;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.Constants;
using GetPeaks_DLL.Go_Decon_Modules;
using GetPeaks_DLL.Objects;
using DeconTools.Backend;
using PNNLOmics.Data;
using GetPeaks_DLL.Functions;

namespace GetPeaks_DLL
{
    public class CrossCorrelateResults
    {
        public void summarize(List<ElutingPeak> elutingPeaksCollection, string outputFileDestination, double crossTolerance, SimpleWorkflowParameters parameters)
        {
            #region test results

            List<ElutingPeak> synchronizedElutingPeaks = new List<ElutingPeak>();//elusting peaks that yielded a monoisotopic peak 
            //int howManyFeatures = 0;//how many eluting peaks have a monoisotopic peak somewhere in the summed spectra
            //int howManyMulipleHits = 0;//more than one monoisotopic ion in the summed spectra
            //int hundredCount = 0;//how many have a monoisotopic peak in the summed spectra and it is the correct one
            int extrapeakcount = 0;//how many lc peaks are split
            int noMonoisotopicPeaks = 0;

            foreach (ElutingPeak ePeak in elutingPeaksCollection)
            {
                if (ePeak.ID ==1)
                {
                    synchronizedElutingPeaks.Add(ePeak);
                }

            }

            #region old code off
            //foreach (ElutingPeak ePeak in elutingPeaksCollection)
            //{
            //    if (ePeak.IsosResultList.Count > 0)
            //    {
            //        howManyFeatures++;
            //    }
            //    //howManyMulipleHits += ePeak.IsosResultList.Count;

            //    if (ePeak.ID == 0)
            //    {
            //        synchronizedElutingPeaks.Add(ePeak);
            //        hundredCount++;//reasons that they are missed is that the eluting peak is an isotope and when summed and deisotoped, the mass is off and not zeroed
            //        //other reasons is that the mass is no where near a monoisotopic peak
            //    }
            //    if (ePeak.NumberOfPeaks > 1)
            //    {
            //        extrapeakcount++;
            //    }
            //}
            #endregion

            #endregion

            //int extraFeaturesInSummedSPectra = howManyMulipleHits-synchronizedElutingPeaks.Count;
            int nonFeaturePeaks = elutingPeaksCollection.Count - synchronizedElutingPeaks.Count;

            List<string> massstringList = new List<string>();
            List<string> intensityList = new List<string>();
            List<string> intensitySummedList = new List<string>();
            List<string> startScanList = new List<string>();
            List<string> scanMaxList = new List<string>();//apex scan
            List<string> endScanList = new List<string>();
            List<string> numberOfPeaks = new List<string>();
            
            Console.WriteLine("");
            foreach (ElutingPeak sPeak in synchronizedElutingPeaks)
            {
                massstringList.Add(sPeak.Mass.ToString());//currently the monoisotopic neutral mass
                intensityList.Add(sPeak.Intensity.ToString());
                intensitySummedList.Add(sPeak.SummedIntensity.ToString());
                startScanList.Add(sPeak.ScanStart.ToString());
                scanMaxList.Add(sPeak.ScanMaxIntensity.ToString());
                endScanList.Add(sPeak.ScanEnd.ToString());
                numberOfPeaks.Add(sPeak.NumberOfPeaks.ToString());

            }

            for(int i=0;i<massstringList.Count;i++)
            {
                Console.WriteLine(massstringList[i] + "," + intensityList[i]+","+scanMaxList[i]);
            }

            Console.WriteLine("");
            Console.WriteLine("there are " + elutingPeaksCollection.Count.ToString() + " eluting peaks");
            Console.WriteLine("there are " + synchronizedElutingPeaks.Count.ToString() + " features that pass the software");
            Console.WriteLine("there are " + noMonoisotopicPeaks.ToString() + " eluting peaks without spine spectra");
           
            //Console.WriteLine("there are " + extraFeaturesInSummedSPectra.ToString() + " extra hits (monoisotopic peaks showing up in summed scans)");
            //Console.WriteLine("there are " + nonFeaturePeaks.ToString() + " non feature peaks (isotopes, peaks that have no isotopes");
            Console.WriteLine("there are " + extrapeakcount.ToString() + " split LC peaks");

            //write data to file
            StringBuilder sb = new StringBuilder();
            using (StreamWriter writer = new StreamWriter(outputFileDestination))
            {
                sb = new StringBuilder();
                sb.Append("The Filename is:\t" + parameters.FileNameUsed + "\n");

                sb.Append("The Scan Range is:\t" + parameters.Part1Parameters.StartScan.ToString() + " to " + parameters.Part1Parameters.StopScan.ToString()+"\n");
                
                sb.Append("The Noise type filter is:\t" +parameters.Part1Parameters.NoiseType.ToString()+"\n");
                
                sb.Append("The Eluting Peak Noise threshold is:\t" +parameters.Part1Parameters.ElutingPeakNoiseThreshold.ToString()+"\n");
               
                sb.Append("The Eluting Peak Allignment ppm is:\t" +parameters.Part1Parameters.AllignmentToleranceInPPM.ToString()+"\n");
                
                sb.Append("The Deisotoping Threshold BR is:\t" +parameters.Part2Parameters.MSPeakDetectorPeakBR.ToString()+"\n");

                sb.Append("neutral mass\tintensity\tsummedIntensity\tstartScan\tscanMax\tendScan\tnumberofPeaks\n");
                writer.WriteLine(sb.ToString());

                for (int d = 0; d < synchronizedElutingPeaks.Count; d++)
                {
                    sb = new StringBuilder();
                    sb.Append(massstringList[d]); sb.Append("\t");
                    sb.Append(intensityList[d]); sb.Append("\t");
                    sb.Append(intensitySummedList[d]); sb.Append("\t");
                    sb.Append(startScanList[d]); sb.Append("\t");
                    sb.Append(scanMaxList[d]); sb.Append("\t");
                    sb.Append(endScanList[d]); sb.Append("\t");
                    sb.Append(numberOfPeaks[d]);

                    writer.WriteLine(sb.ToString());
                }

            }
            
        }

        //public void AssignCrossCorrelatedInformation(ElutingPeak ePeak, SimpleWorkflowParameters parameters)
        public void AssignCrossCorrelatedInformation(ElutingPeak ePeak,GoResults resultPeakIsos, SimpleWorkflowParameters parameters)
        {
 //           List<ElutingPeak> synchronizedElutingPeaks = new List<ElutingPeak>();//elusting peaks that yielded a monoisotopic peak 

            double crossTolerance = parameters.ConsistancyCrossErrorPPM;
            int noMonoisotopicPeaks = 0;

            double mTol = 0;

            double isotopeMass = 0;
            double upperMass = 0;
            double lowerMass = 0;

            //if (ePeak.IsosResultList.Count > 0)
            if (resultPeakIsos.IsosResultList.Count > 0)
            {
                //foreach (IsosResult isotope in ePeak.IsosResultList)
                foreach (IsosResult isotope in resultPeakIsos.IsosResultList)
                {
                    mTol = CompareContrast2_Old.ErrorCalculation(ePeak.Mass, crossTolerance, GetPeaks_DLL.CompareContrast.CompareContrast2_Old.ErrorType.PPM);

                    isotopeMass = isotope.IsotopicProfile.MonoPeakMZ;
                    upperMass = ePeak.Mass + mTol;
                    lowerMass = ePeak.Mass - mTol;
                    if (isotopeMass >= lowerMass && isotopeMass <= upperMass)
                    {
                        double originalMass = isotope.IsotopicProfile.MonoIsotopicMass;
                        double ppmDifference = 0;
                        ePeak.Mass = originalMass;
                        double correctedMass = CalculatedWeightedAverageMass(ePeak, originalMass, isotope.IsotopicProfile.ChargeState, ref ppmDifference);
                        if (ppmDifference < parameters.ConsistancyCrossErrorCorrectedMass)
                        {
                            ePeak.Mass = correctedMass;
                        }
                        else
                        {
                            Console.WriteLine("missed isotope " + originalMass + " " + correctedMass + " " + ppmDifference);
                            //Console.ReadKey();

                        }
                        ePeak.SummedIntensity = CalculatedPeakVolume(ePeak);
                        ePeak.ChargeState = isotope.IsotopicProfile.ChargeState;
                        ePeak.AggregateIntensity = isotope.IsotopicProfile.IntensityAggregateAdjusted;//TODO: Check this;
                        ePeak.FitScore = Convert.ToSingle(isotope.IsotopicProfile.Score);

  //                      synchronizedElutingPeaks.Add(ePeak);
                        ePeak.ID = 1;
                    }
                }
                if(ePeak.ID!=1) //no cross validation so this must be an isotope
                {
                    ePeak.ID = -1;
                }
            }
            else
            {
                ePeak.ID = -2;
                noMonoisotopicPeaks++;
            }
        }

        public void AssignCrossCorrelatedInformation(ElutingPeakOmics ePeak, GoResults resultPeakIsos, IsotopeObject isotopeStorage, SimpleWorkflowParameters parameters)
        {
            //           List<ElutingPeak> synchronizedElutingPeaks = new List<ElutingPeak>();//elusting peaks that yielded a monoisotopic peak 

            double crossTolerance = parameters.ConsistancyCrossErrorPPM;
            int noMonoisotopicPeaks = 0;

            double mTol = 0;

            double isotopeMass = 0;
            double upperMass = 0;
            double lowerMass = 0;

            //if (ePeak.IsosResultList.Count > 0)
            if (resultPeakIsos.IsosResultList.Count > 0)
            {
                //foreach (IsosResult isotope in ePeak.IsosResultList)
                foreach (IsosResult isotope in resultPeakIsos.IsosResultList)
                {
                    mTol = CompareContrast2_Old.ErrorCalculation(ePeak.Mass, crossTolerance, GetPeaks_DLL.CompareContrast.CompareContrast2_Old.ErrorType.PPM);

                    isotopeMass = isotope.IsotopicProfile.MonoPeakMZ;
                    upperMass = ePeak.Mass + mTol;
                    lowerMass = ePeak.Mass - mTol;
                    if (isotopeMass >= lowerMass && isotopeMass <= upperMass)
                    {
                        double originalMass = isotope.IsotopicProfile.MonoIsotopicMass;
                        double ppmDifference = 0;
                        ePeak.Mass = originalMass;
                        double correctedMass = CalculatedWeightedAverageMass(ePeak, originalMass, isotope.IsotopicProfile.ChargeState, ref ppmDifference);
                        if (ppmDifference < parameters.ConsistancyCrossErrorCorrectedMass)
                        {
                            ePeak.Mass = correctedMass;
                        }
                        else
                        {
                            Console.WriteLine("missed isotope " + originalMass + " " + correctedMass + " " + ppmDifference);
                            //Console.ReadKey();

                        }
                        ePeak.SummedIntensity = CalculatedPeakVolume(ePeak);
                        ePeak.ChargeState = isotope.IsotopicProfile.ChargeState;
                        ePeak.AggregateIntensity = isotope.IsotopicProfile.IntensityAggregateAdjusted;//TODO: Check this;
                        ePeak.FitScore = Convert.ToSingle(isotope.IsotopicProfile.Score);

                        isotopeStorage.MonoIsotopicMass = isotope.IsotopicProfile.Peaklist[0].XValue;
                        isotopeStorage.ExperimentMass = correctedMass;
                        foreach(MSPeak peak in isotope.IsotopicProfile.Peaklist)
                        {
                            PNNLOmics.Data.Peak newIsotopePeak = new PNNLOmics.Data.Peak();
                            newIsotopePeak.Height=peak.Height;
                            newIsotopePeak.XValue = Convert.ToSingle(peak.XValue);
                            isotopeStorage.IsotopeList.Add(newIsotopePeak);
                        }

                        //                      synchronizedElutingPeaks.Add(ePeak);
                        ePeak.ID = 1;
                    }
                }
                if (ePeak.ID != 1) //no cross validation so this must be an isotope
                {
                    ePeak.ID = -1;
                }
            }
            else
            {
                ePeak.ID = -2;
                noMonoisotopicPeaks++;
            }
        }

        //public void SimplifyElutingPeak(ref ElutingPeak ePeak)
        //{
        //    ePeak.ChromPeak = null;
        //    //ePeak.IsosResultList.Clear();
        //    ePeak.PeakList.Clear();
        //}

        private double CalculatedWeightedAverageMass(ElutingPeak ePeak, double origionalMass, int chargestate, ref double ppm)
        {
            double correctedMass = 0;
            double correctedPeakListMass = 0;
            double massProton = Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic;
            //calculate total intensity for weights;
            float totalIntensity = 0;
            for (int i = 0; i < ePeak.PeakList.Count; i++)
            {
                totalIntensity += ePeak.PeakList[i].MSPeak.Height;
            }
            List<float> weightsList = new List<float>();
            for (int i = 0; i < ePeak.PeakList.Count; i++)
            {
                correctedPeakListMass = ePeak.PeakList[i].MSPeak.XValue * chargestate - massProton * chargestate;
                weightsList.Add(ePeak.PeakList[i].MSPeak.Height / totalIntensity);
                correctedMass += weightsList[i] * correctedPeakListMass;
            }
            //double error = origionalMass - correctedMass;
            ppm = ErrorCalculator.PPMExact(correctedMass,origionalMass);
            return correctedMass;
        }

        private double CalculatedWeightedAverageMass(ElutingPeakOmics ePeak, double origionalMass, int chargestate, ref double ppm)
        {
            double correctedMass = 0;
            double correctedPeakListMass = 0;
            double massProton = Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic;
            //calculate total intensity for weights;
            double totalIntensity = 0;
            for (int i = 0; i < ePeak.PeakList.Count; i++)
            {
                totalIntensity += ePeak.PeakList[i].MSPeak.Height;
            }
            List<double> weightsList = new List<double>();
            for (int i = 0; i < ePeak.PeakList.Count; i++)
            {
                correctedPeakListMass = ePeak.PeakList[i].MSPeak.XValue * chargestate - massProton * chargestate;
                weightsList.Add(ePeak.PeakList[i].MSPeak.Height / totalIntensity);
                correctedMass += weightsList[i] * correctedPeakListMass;
            }
            //double error = origionalMass - correctedMass;
            ppm = ErrorCalculator.PPMExact(correctedMass,origionalMass);
            return correctedMass;
        }

        private double CalculatedPeakVolume(ElutingPeak ePeak)
        {
            double correctedPeakVolume = 0;
            double totalIntensity = 0;
            for (int i = 0; i < ePeak.PeakList.Count; i++)
            {
                totalIntensity += ePeak.PeakList[i].MSPeak.Height;
            }
            correctedPeakVolume = totalIntensity;
            return correctedPeakVolume;
        }

        private double CalculatedPeakVolume(ElutingPeakOmics ePeak)
        {
            double correctedPeakVolume = 0;
            double totalIntensity = 0;
            for (int i = 0; i < ePeak.PeakList.Count; i++)
            {
                totalIntensity += ePeak.PeakList[i].MSPeak.Height;
            }
            correctedPeakVolume = totalIntensity;
            return correctedPeakVolume;
        }

       
    }

    
}
