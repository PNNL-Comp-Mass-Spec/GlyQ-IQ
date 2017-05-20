using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Algorithms.PeakDetection;
using GetPeaks_DLL.CompareContrast;
using PNNLOmics.Data.Constants;
using GetPeaks_DLL.Isotopes;

namespace GetPeaks_DLL.PNNLOmics_Modules
{
    [Serializable]
    public class OrbitrapThreshold
    {
        /// <summary>
        /// Gets or sets the peak centroider parameters.
        /// </summary>
        public PeakThresholderParameters Parameters { get; set; }

        /// <summary>
        /// Gets or sets the peak centroider parameters.
        /// </summary>
        public OrbitrapFilterParameters ParametersOrbitrap { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrbitrapThreshold()
        {
            this.Parameters = new PeakThresholderParameters();
            this.ParametersOrbitrap = new OrbitrapFilterParameters();
        }

        public List<ProcessedPeak> ApplyThreshold(ref List<ProcessedPeak> peakList)
        {
            List<ProcessedPeak> ResultListThresholded = new List<ProcessedPeak>();

            //initialize data
            List<int> acceptedData = new List<int>(new int[peakList.Count]);

            //we need two connections.  one for each directions.  this will get the before and after connections
            List<int> acceptedConnectionDataIndex1 = new List<int>(new int[peakList.Count]);
            List<int> acceptedConnectionDataIndex2 = new List<int>(new int[peakList.Count]);

            //TODO update this 1/2
            //double massNeutron = Constants.SubAtomicParticles[SubAtomicParticleName.Neutron].MassMonoIsotopic;
            //ParametersOrbitrap.massNeutron = 1.0013;

            //newSwitchThreshold.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;// this is an important parameter
            //newSwitchThreshold.ParametersOrbitrap.massNeutron = 1.002149286;//SN09// this is an important parameter
            //1.00235 derived from weighted average of CHNOS.  Horn, Zubarev, McLafferty, anal chem, 2000
            acceptedData = this.ApplyThresholdDirectoin1(ref peakList, ref acceptedData, ref acceptedConnectionDataIndex1, ParametersOrbitrap.massNeutron);//at i+x, (i+X)-(i)

            acceptedData = this.ApplyThresholdDirectoin2(ref peakList, ref acceptedData, ref acceptedConnectionDataIndex2, ParametersOrbitrap.massNeutron);//at i, (i+X)-(i)

            if (this.ParametersOrbitrap.withLowMassesAllowed == true)
            {
                acceptedData = this.AllowLowMassMonoisotopes(ref peakList, ref acceptedData, ref acceptedConnectionDataIndex1, ref acceptedConnectionDataIndex2, this.ParametersOrbitrap.LowMassFilterAveragine);
            }

            for (int i = 0; i < peakList.Count; i++)
            {
                if (acceptedData[i] > 0)
                {
                    ResultListThresholded.Add(peakList[i]);
                    //Console.WriteLine(peakList[i].XValue.ToString());
                }
            }

            return ResultListThresholded;
        }


        //case AveragineType.Peptide:
        //            AveragineIN.isoC = 4.9384;
        //            AveragineIN.isoH = 7.7583;
        //            AveragineIN.isoO = 1.4773;
        //            AveragineIN.isoN = 1.3577;
        //            AveragineIN.isoS = 0.0417;
        //            break;
        //        case AveragineType.Glycan://HNFS
        //            AveragineIN.isoC = 31;
        //            AveragineIN.isoH = 50;
        //            AveragineIN.isoO = 22;
        //            AveragineIN.isoN = 2;
        //            AveragineIN.isoS = 0;
        //            break;
        //        default:
        //            AveragineIN.isoC = 4.9384;
        //            AveragineIN.isoH = 7.7583;
        //            AveragineIN.isoO = 1.4773;
        //            AveragineIN.isoN = 1.3577;
        //            AveragineIN.isoS = 0.0417;
        //            break;


        /// <summary>
        /// Filter the orbitrap data via charge state since the noise level is not provided.  Look for possible mass isotopes in + direction
        /// </summary>
        /// <param name="peakList">Input peaks we want to threshold</param>
        /// <param name="acceptedData">list of data that pass threshold</param>
        /// <param name="massNeutron"> mass of a neutron</param>
        /// <returns>Peaks containing an isotope</returns>

        private List<int> ApplyThresholdDirectoin1(ref List<ProcessedPeak> peakList, ref List<int> acceptedData, ref List<int> acceptedConnectionDataIndex, double massNeutron)
        {
            List<ProcessedPeak> ResultListThresholded = new List<ProcessedPeak>();
            int peakListCount = peakList.Count;

            double tolleranceIN = ParametersOrbitrap.DeltaMassTollerancePPM;//ppm
            double mTol = 0;

            double difference = 0;
            double highbound = 0;
            double lowbound = 0;

            //forward direction, blanks are at  the beginning of the data
            for (int i = 0; i < peakListCount; i++)
            {
                int peakSkip = 1;

                do
                {
                    int startoffset = peakSkip;//start at row 1 (not row 0) because we need to do row1-row0
                    int stopBeforeEnd = peakListCount - peakSkip;
                    if (i >= startoffset)
                    {
                        difference = peakList[i].XValue - peakList[i - peakSkip].XValue;
                        mTol = CompareContrast.CompareContrast2_Old.ErrorCalculation(difference, tolleranceIN, GetPeaks_DLL.CompareContrast.CompareContrast2_Old.ErrorType.PPM);
                        if (difference < massNeutron + mTol)//since +1 is the largest difference of interest
                        {
                            foreach (int chargeState in ParametersOrbitrap.ChargeStateList)//cycle through charge states
                            {
                                highbound = (double)(massNeutron / (double)chargeState) + mTol;
                                lowbound = (double)(massNeutron / (double)chargeState) - mTol;
                                if (difference >= lowbound && difference <= highbound)
                                {
                                    acceptedData[i]++;
                                    acceptedConnectionDataIndex[i] = i - peakSkip;
                                }
                            }
                        }
                    }

                    if (i > peakSkip)//do not increpent if we are at the beginnig of the spectra where the peakList[i-peakSkip] will fail due to index
                    {
                        peakSkip++;
                    }
                    else
                    {
                        break;
                    }
                }
                while (difference < massNeutron + mTol);//+1 charge is the largest window.  it would be better to use each charge state though
            }
            return acceptedData;
        }

        private List<int> ApplyThresholdDirectoin2(ref List<ProcessedPeak> peakList, ref List<int> acceptedData, ref List<int> acceptedConnectionDataIndex2, double massNeutron)
        {
            List<ProcessedPeak> ResultListThresholded = new List<ProcessedPeak>();
            int peakListCount = peakList.Count;

            double tolleranceIN = ParametersOrbitrap.DeltaMassTollerancePPM;//ppm
            double mTol = 0;

            double difference = 0;
            double highbound = 0;
            double lowbound = 0;

            //forward direction, blanks are at  the beginning of the data
            for (int i = 0; i < peakListCount; i++)
            {
                int peakSkip = 1;

                do
                {
                    //int startoffset = 0;//include row 0 in this case
                    int stopBeforeEnd = peakListCount - peakSkip;//stop before end
                    if (i < stopBeforeEnd)
                    {
                        difference = peakList[i + peakSkip].XValue - peakList[i].XValue;
                        mTol = CompareContrast.CompareContrast2_Old.ErrorCalculation(difference, tolleranceIN, GetPeaks_DLL.CompareContrast.CompareContrast2_Old.ErrorType.PPM);
                        if (difference < massNeutron + mTol)//since +1 is the largest difference of interest
                        {
                            foreach (int chargeState in ParametersOrbitrap.ChargeStateList)//cycle through charge states
                            {
                                highbound = (double)(massNeutron / (double)chargeState) + mTol;
                                lowbound = (double)(massNeutron / (double)chargeState) - mTol;
                                if (difference >= lowbound && difference <= highbound)
                                {
                                    acceptedData[i]++;
                                    acceptedConnectionDataIndex2[i] = i + peakSkip;
                                }
                            }
                        }
                    }

                    if (i < stopBeforeEnd)//do not increment if we are at the beginnig of the spectra where the peakList[i-peakSkip] will fail due to index
                    {
                        peakSkip++;
                    }
                    else
                    {
                        break;
                    }
                }
                while (difference < massNeutron + mTol);//+1 charge is the largest window.  it would be better to use each charge state though
            }

            return acceptedData;
        }

        /// <summary>
        /// allow masses in which the C13 is not present because it would be in the noise
        /// </summary>
        /// <param name="peakList">Input peaks we want to threshold</param>
        /// <param name="acceptedData">list of data that pass threshold</param>
        /// <returns>Peaks containing an isotope</returns>
        private List<int> AllowLowMassMonoisotopes(ref List<ProcessedPeak> peakList, ref List<int> acceptedData, ref List<int> acceptedConnectionDataIndex1, ref List<int> acceptedConnectionDataIndex2, Dictionary<string, double> averagine)
        {

            double ratioC13toC12;
            List<double> myticalC13List = new List<double>();
            List<double> theoryRatioList = new List<double>();

            //find the last of the chain for 1 points.  multiple points would be a better estimate 
            int count = 0;//how much good data we have
            for (int i = 0; i < peakList.Count; i++)
            {
                if (acceptedData[i] > 0)
                {
                    count++;
                }
            }

            int j = 0;
            int n = 0;
            int r = 0;
            double threshold1 = 0;
            double threshold2 = 0;
            double threshold3 = 0;


            if (count > 0)
            {
                //for count==1
                for (int i = 0; i < peakList.Count; i++)
                {
                    if (acceptedData[i] > 0)//for the first accepted data.  accept the last of the string as the threshold since it will be just above the noise level
                    {
                        j = i;
                        //find the last of the chain for 1 points.  multiple points would be a better estimate

                        if (acceptedConnectionDataIndex2[j] > 0)
                        {
                            while (acceptedConnectionDataIndex2[j] > 0)
                            {
                                j = acceptedConnectionDataIndex2[j];
                                threshold1 = peakList[j].Height;
                            }
                            
                            break;
                        }
                    }
                }

                //bonus for better acuracy of noise
                if (count > 1 && j < acceptedData.Count)//we want more than one point that is free from clusters
                {
                    for (int k = j + 1; k < peakList.Count; k++)
                    {
                        if (acceptedData[k] > 0)//for the first accepted data.  accept the last of the string as the threshold since it will be just above the noise level
                        {
                            n = k;
                            //find the last of the chain for 1 points.  multiple points would be a better estimate

                            if (acceptedConnectionDataIndex2[n] > 0)
                            {
                                while (acceptedConnectionDataIndex2[n] > 0)
                                {
                                    n = acceptedConnectionDataIndex2[n];
                                    threshold2 = peakList[n].Height;
                                }
                                
                                break;
                            }
                        }
                    }
                }

                if (count > 2 && j < acceptedData.Count)
                {
                    for (int m = n + 1; m < peakList.Count; m++)
                    {
                        if (acceptedData[m] > 0)//for the first accepted data.  accept the last of the string as the threshold since it will be just above the noise level
                        {
                            r = m;
                            //find the last of the chain for 1 points.  multiple points would be a better estimate

                            if (acceptedConnectionDataIndex2[r] > 0)
                            {
                                while (acceptedConnectionDataIndex2[r] > 0)
                                {
                                    r = acceptedConnectionDataIndex2[r];
                                }
                                threshold3 = peakList[r].Height;
                                break;
                            }
                        }
                    }
                }
                

                //find min value oflowest isotope
                double minThreshold = threshold1;
                if(threshold2< minThreshold)
                {
                    minThreshold = threshold2;
                }
                if (threshold3 < minThreshold)
                {
                    minThreshold = threshold3;
                }

                //now that we know the noise level, we can check the "non" accepted peaks for the C13 test

                //theory
                for (int i = 0; i < peakList.Count; i++)
                {
                    IsotopeProfile newProfile = new IsotopeProfile();
                    newProfile.GenerateProfile(peakList[i].XValue, averagine);
                    ratioC13toC12 = newProfile.IsotopeIntensityList[1] / newProfile.IsotopeIntensityList[0];
                    theoryRatioList.Add(ratioC13toC12);
                }

                //experimental
                double myticalC13 = 0;
                for (int i = 0; i < peakList.Count; i++)
                {
                    if (acceptedData[i] == 0)//for the first accepted data.  accept the last of the string as the threshold since it will be just above the noise level
                    {
                        myticalC13 = peakList[i].Height * theoryRatioList[i];
                        myticalC13List.Add(myticalC13); 
                    }
                    else
                    {
                        myticalC13List.Add(0);
                    }
                }
    
                //test experimantal vs theory
                count = 0;
                for (int i = 0; i < peakList.Count; i++)
                {
                    if (acceptedData[i] == 0)//for the first accepted data.  accept the last of the string as the threshold since it will be just above the noise level
                    {
                        if (myticalC13List[i] < minThreshold / ParametersOrbitrap.ExtraSigmaFactor)//by shrinking the minthreshold, it is harder for ions to pass
                        {
                            acceptedData[i] = 3;
                            count++;
                        }
                    }
                }
            }

            return acceptedData;
        }
    }
}
