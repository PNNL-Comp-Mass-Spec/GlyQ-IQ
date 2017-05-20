using System;
using System.Collections.Generic;
using System.Text;
using IQGlyQ.Results;
using IQ_X64.Workflows.Core;
using IQ_X64.Workflows.FileIO;
using Run64.Backend.Core;

namespace IQGlyQ.Exporter
{
    public class InSourceFragmentExporter : ResultExporter
    {
        public bool ToPrintDH { get; set; }
        public int NumberOfPenaltyIons { get; set; }
        public int NumberOfIsotopesToPrint { get; set; }

        public override string GetResultAsString(IqResult result, bool includeHeader = false)
        {
            string delim = "\t";

            //we need to set the lcscanset selelected

            //IQ
            string baseOutput = base.GetResultAsString(result, includeHeader);

            //GlyQ
            IqGlyQResult castResult = (IqGlyQResult)result;

            string addOnString = AddOnString(castResult, includeHeader);
            string resultString = "";

            string DHstring = "";
            if (ToPrintDH)
            {
                DHstring = AddOnStringDH(castResult, NumberOfIsotopesToPrint, includeHeader);
                resultString = baseOutput + delim + addOnString + delim + DHstring;
            }
            else
            {
                resultString = baseOutput + delim + addOnString;
            }




            return resultString;
        }

        public override string GetHeader()
        {
            string delim = "\t";

            //IQ
            string baseOutput = base.GetHeader();

            //GlyQ
            string addOnString = AddOnHeaderString();

            string resultHeader = "";
            if (ToPrintDH)
            {
                string addonHeadderDH = AddOnHeaderStringDH(NumberOfPenaltyIons, NumberOfIsotopesToPrint);
                resultHeader = baseOutput + delim + addOnString + delim + addonHeadderDH;
            }
            else
            {
                resultHeader = baseOutput + delim + addOnString;
            }


            return resultHeader;
        }

        private string AddOnString(IqGlyQResult result, bool includeHeader = false)
        {
            StringBuilder sb = new StringBuilder();

            string scanSetString;

            if (result.LCScanSetSelected != null)
            {
                scanSetString = result.ToChild.Scan.ToString();
            }
            else
            {
                scanSetString = "0";
            }

            string delim = "\t";


            if (includeHeader)
            {
                string header = GetHeader();
                sb.Append(header);
                sb.Append(Environment.NewLine);

            }


            sb.Append(result.ToChild.Primary_Target.ScanLCTarget);
            sb.Append(delim);
            sb.Append(result.ToChild.Primary_Target.ChargeState);
            sb.Append(delim);
            sb.Append(result.ToChild.TargetParent.ChargeState);
            sb.Append(delim);
            sb.Append(result.ToChild.CorrelationScore);
            sb.Append(delim);
            sb.Append(result.ToChild.IsIntact);
            sb.Append(delim);
            sb.Append(result.ToChild.IsAntiCorrelated);
            sb.Append(delim);
            sb.Append(result.ToChild.TargetParent.DifferenceName);
            sb.Append(delim);
            sb.Append(result.ToChild.FragmentFitAbundance);
            sb.Append(delim);
            sb.Append(result.ToChild.ParentFitAbundance);
            sb.Append(delim);
            sb.Append(result.ToChild.LMOptimizationCorrelationRsquared);
            sb.Append(delim);
            sb.Append(result.ToChild.Primary_Target.ScanInfo.Start);
            sb.Append(delim);
            sb.Append(result.ToChild.Primary_Target.ScanInfo.Stop);
            sb.Append(delim);
            sb.Append(result.ToChild.TargetParent.ScanInfo.Start);
            sb.Append(delim);
            sb.Append(result.ToChild.TargetParent.ScanInfo.Stop);
            sb.Append(delim);
            sb.Append(result.ToChild.Error);
            sb.Append(delim);
            sb.Append(result.ToChild.Error);//needs to be fixed
            sb.Append(delim);
            sb.Append(result.ToChild.GlobalChargeStateMin);
            sb.Append(delim);
            sb.Append(result.ToChild.GlobalChargeStateMax);
            sb.Append(delim);
            sb.Append(result.ToChild.GlobalAggregateAbundance);
            sb.Append(delim);
            sb.Append(result.ToChild.GlobalIntactCount);
            sb.Append(delim);
            sb.Append(result.ToChild.GlobalFutureTargetsCount);
            sb.Append(delim);
            sb.Append(result.ToChild.GlobalResult);
            sb.Append(delim);
            sb.Append(result.ToChild.TypeOfResultParentOrChildDifferenceApproach);
            sb.Append(delim);
            sb.Append(result.ToChild.FinalDecision);
            sb.Append(delim);
            sb.Append(result.ToChild.AverageMonoMass);
            sb.Append(delim);
            sb.Append(result.ToChild.PPMError);
            sb.Append(delim);
            sb.Append(result.ToChild.TypeOfResultTargetOrModifiedTarget);
            sb.Append(delim);
            sb.Append(result.ToChild.Primary_Observed_IsotopeProfile.Score);
            sb.Append(delim);
            sb.Append(result.ToChild.InterfearenceScore);
            sb.Append(delim);
            sb.Append(result.ToChild.ChargeStateCorrelation);

            string outString = sb.ToString();
            return outString;
        }

        private string AddOnHeaderString()
        {
            StringBuilder sb = new StringBuilder();

            string delim = "\t";

            sb.Append("ScanNum");
            sb.Append(delim);
            sb.Append("FragmentCharge");
            sb.Append(delim);
            sb.Append("ParentCharge");
            sb.Append(delim);
            sb.Append("CorrelationScore");
            sb.Append(delim);
            sb.Append("IsIntact");
            sb.Append(delim);
            sb.Append("IsAntiCorrelated");
            sb.Append(delim);
            sb.Append("Name");
            sb.Append(delim);
            sb.Append("FragmentArea");
            sb.Append(delim);
            sb.Append("ParentArea");
            sb.Append(delim);
            sb.Append("Fragment_LM_R2");
            sb.Append(delim);
            sb.Append("FragmentStart");
            sb.Append(delim);
            sb.Append("FragmentStop");
            sb.Append(delim);
            sb.Append("ParentStart");
            sb.Append(delim);
            sb.Append("ParentStop");
            sb.Append(delim);
            sb.Append("FragmentError");
            sb.Append(delim);
            sb.Append("ParentError");

            sb.Append(delim);
            sb.Append("GlobalChargeStateMin");
            sb.Append(delim);
            sb.Append("GlobalChargeStateMax");
            sb.Append(delim);
            sb.Append("GlobalAggregateAbundance");
            sb.Append(delim);
            sb.Append("GlobalIntactCount");
            sb.Append(delim);
            sb.Append("GlobalFutureTargetsCount");
            sb.Append(delim);
            sb.Append("GlobalResult");
            sb.Append(delim);
            sb.Append("ParentOrChild");
            sb.Append(delim);
            sb.Append("FinalDecision");
            sb.Append(delim);
            sb.Append("AverageMonoMass");
            sb.Append(delim);
            sb.Append("PPM");
            sb.Append(delim);
            sb.Append("Type");
            sb.Append(delim);
            sb.Append("IsoFit");
            sb.Append(delim);
            sb.Append("InterferenceValue");
            sb.Append(delim);
            sb.Append("ChargeStateCorrelation");

            string outString = sb.ToString();
            return outString;
        }

        /// <summary>
        /// it is important to either append data OR 0.  missing one will mess the whole thing up
        /// </summary>
        /// <param name="result"></param>
        /// <param name="numberOfIsotopesToPrint"></param>
        /// <param name="includeHeader"></param>
        /// <returns></returns>
        private string AddOnStringDH(IqGlyQResult result, int numberOfIsotopesToPrint, bool includeHeader = false)
        {
            StringBuilder Sb = new StringBuilder();
            StringBuilderV2 sb = new StringBuilderV2("\t");
            string scanSetString;

            if (result.LCScanSetSelected != null)
            {
                scanSetString = result.ToChild.Scan.ToString();
            }
            else
            {
                scanSetString = "0";
            }

            string delim = "\t";


            if (includeHeader)
            {
                string header = GetHeader();
                Sb.Append(header);
                Sb.Append(Environment.NewLine);

            }

            int precategories = 3;

            //sbLength=0

            if (result.ToChild.Primary_Observed_IsotopeProfile.Peaklist != null && result.ToChild.Primary_Observed_IsotopeProfile.Peaklist.Count > 0)
            {
                List<MSPeak> localPeakList = result.ToChild.Primary_Observed_IsotopeProfile.Peaklist;
                float[] localTheoreticalPeakList = result.ToChild.Primary_Target.TheorIsotopicProfile.AlternatePeakIntensities;
                int numberOfPeaksDetected = localPeakList.Count;

                double mixingFraction = result.ToChild.Primary_Target.TheorIsotopicProfile.EstablishedMixingFraction;
                sb.Append(mixingFraction);
                //sb.Append(delim);

                if (mixingFraction > 0)
                {
                    sb.Append(mixingFraction / (1 - mixingFraction));
                }
                else
                {
                    sb.Append(-1);
                }
                //sb.Append(delim);

                sb.Append(result.ToChild.Primary_Target.TheorIsotopicProfile.AbundanceC13asFraction);
                //sb.Append(delim);


                //sbLength = 3

                //print observed and normalized observed
                float maxObservedAbundance;
                AppendObserved(numberOfIsotopesToPrint, numberOfPeaksDetected, ref sb, delim, localPeakList, out maxObservedAbundance);

                //sb.Append(delim);

                //sbLength = 19 (3 + numberOfIsotopesToPrint + numberOfIsotopesToPrint)

                AppendTheoretical(numberOfIsotopesToPrint, ref sb, delim, localTheoreticalPeakList, numberOfPeaksDetected);

                //1 because of penalty scoring

                sb.Append(0);

                //sbLength = 28 (3 + numberOfIsotopesToPrint + numberOfIsotopesToPrint + numberOfIsotopesToPrint)

                AppendDH(result, localPeakList, maxObservedAbundance, delim, ref sb);

                //sb.Append(delim);

                //sbLenght = 36 (3 + numberOfIsotopesToPrint + numberOfIsotopesToPrint + numberOfIsotopesToPrint H4+1+D(4-1)

                float C12 = 1;
                sb.Append(C12);//seperate the observed and the theoryt with c13
                //sb.Append(delim);

                float C13 = result.ToChild.Primary_Target.TheorIsotopicProfile.AbundanceC13asFraction;
                sb.Append(C13);//seperate the observed and the theoryt with c13
                //sb.Append(delim);

                float C14 = result.ToChild.Primary_Target.TheorIsotopicProfile.AbundanceC14asFraction;
                sb.Append(C14);//seperate the observed and the theoryt with c13
                //sb.Append(delim);

                float C15 = result.ToChild.Primary_Target.TheorIsotopicProfile.AbundanceC14asFraction;
                sb.Append(C15);//seperate the observed and the theoryt with c13
                //sb.Append(delim);

                //sbLenght = 36 (3 + numberOfIsotopesToPrint + numberOfIsotopesToPrint + 1 + numberOfIsotopesToPrint H4+1+D(4-1) 4Simple

            }
            else
            {
                for (int i = 0; i < precategories + numberOfIsotopesToPrint * 3 + 1 + (4 + 1 + 3) + 4; i++)
                {
                    sb.Append(-1);
                    //sb.Append(delim);
                }
                //last line
                //sb.Append(-1);
            }

            //sbLength = 39

            Sb.Append(sb.ReturnLine());
            string outString = Sb.ToString();
            return outString;
        }

        private class StringBuilderV2
        {
            public List<string> Words { get; set; }
            private string Deliminator { get; set; }

            public StringBuilderV2(string deliminator)
            {
                Deliminator = deliminator;
                Words = new List<string>();
            }

            public void Append(Object item)
            {
                Words.Add(item.ToString());
            }

            public string ReturnLine()
            {
                return String.Join(Deliminator, Words);
            }

        }

        private static void AppendDH(IqGlyQResult result, List<MSPeak> localPeakList, float maxObservedAbundance, string delim, ref StringBuilderV2 sb)
        {
            float HMono = localPeakList[1].Height / maxObservedAbundance;

            float H12 = 1 * HMono;
            sb.Append(H12); //seperate the observed and the theoryt with c13
            //sb.Append(delim);

            float H13 = result.ToChild.Primary_Target.TheorIsotopicProfile.AbundanceC13asFraction * HMono;
            sb.Append(H13); //seperate the observed and the theoryt with c13
            //sb.Append(delim);

            float H14 = result.ToChild.Primary_Target.TheorIsotopicProfile.AbundanceC14asFraction * HMono;
            sb.Append(H14); //seperate the observed and the theoryt with c13
            //sb.Append(delim);

            float H15 = result.ToChild.Primary_Target.TheorIsotopicProfile.AbundanceC15asFraction * HMono;
            sb.Append(H15); //seperate the observed and the theoryt with c13
            //sb.Append(delim);

            sb.Append(0);
            //sb.Append(delim);

            //sbLength = 32

            if (localPeakList.Count > 2)
            {
                float D12 = localPeakList[2].Height / maxObservedAbundance - H13;
                sb.Append(D12);
            } //seperate the observed and the theoryt with c13
            else
            {
                sb.Append(0);
            }
            //sb.Append(delim);

            if (localPeakList.Count > 3)
            {
                float D13 = localPeakList[3].Height / maxObservedAbundance - H14;
                sb.Append(D13);
            } //seperate the observed and the theoryt with c13
            else
            {
                sb.Append(0);
            }
            //sb.Append(delim);

            if (localPeakList.Count > 4)
            {
                float D14 = localPeakList[4].Height / maxObservedAbundance - H15;
                sb.Append(D14);
            } //seperate the observed and the theoryt with c13
            else
            {
                sb.Append(0);
            }

        }


        private string AddOnHeaderStringDH(int numberOfPenaltyIons, int numberOfIsotopeToPrint)
        {
            StringBuilder sb = new StringBuilder();

            string delim = "\t";

            sb.Append("MixingFraction");
            sb.Append(delim);

            sb.Append("DH");
            sb.Append(delim);

            sb.Append("C13");
            sb.Append(delim);

            AppendObservedHedders1Set(numberOfPenaltyIons, numberOfIsotopeToPrint, delim, sb);

            sb.Append(delim);

            AppendObservedHedders1Set(numberOfPenaltyIons, numberOfIsotopeToPrint, delim, sb);

            sb.Append(delim);

            AppendTheoryHedders1Set(numberOfPenaltyIons, numberOfIsotopeToPrint, delim, sb);

            sb.Append(delim);
            sb.Append("Zero");
            sb.Append(delim);

            AppendDHHeader(delim, sb);

            sb.Append(delim);

            sb.Append("C12");
            sb.Append(delim);
            sb.Append("C13");
            sb.Append(delim);
            sb.Append("C14");
            sb.Append(delim);
            sb.Append("C15");
            //sb.Append(delim);



            string outString = sb.ToString();
            return outString;
        }

        private static void AppendDHHeader(string delim, StringBuilder sb)
        {
            sb.Append("H12");
            sb.Append(delim);
            sb.Append("H13");
            sb.Append(delim);
            sb.Append("H14");
            sb.Append(delim);
            sb.Append("H15");
            sb.Append(delim);

            sb.Append("Zero");
            sb.Append(delim);

            sb.Append("D12");
            sb.Append(delim);
            sb.Append("D13");
            sb.Append(delim);
            sb.Append("D14");
        }


        private static void AppendTheoretical(int numberOfIsotopesToPrint, ref StringBuilderV2 sb, string delim, float[] localTheoreticalPeakList, int numberOfPeaksDetected)
        {

            for (int i = 0; i < numberOfIsotopesToPrint; i++)
            {
                if (numberOfPeaksDetected > i)
                {
                    if (i < localTheoreticalPeakList.Length)
                    {
                        sb.Append(localTheoreticalPeakList[i]);
                    }
                    else
                    {
                        sb.Append(0);
                    }
                }
                else
                {
                    sb.Append(0);
                }
                //sb.Append(delim);
            }
            //last line
            //if (numberOfPeaksDetected > numberOfIsotopesToPrint && localTheoreticalPeakList.Length > 0 && numberOfIsotopesToPrint < localTheoreticalPeakList.Length)
            //{
            //    sb.Append(localTheoreticalPeakList[numberOfIsotopesToPrint]);
            //}
            //else
            //{
            //    sb.Append(0);
            //}
        }

        private static void AppendObserved(int numberOfIsotopesToPrint, int numberOfPeaksDetected, ref StringBuilderV2 sb, string delim, List<MSPeak> localPeakList, out float maxObserved)
        {
            maxObserved = 0;

            for (int i = 0; i < numberOfIsotopesToPrint; i++)//numberOfIsotopesToPrint = 8 isotopes for low mass is good
            {
                if (localPeakList != null && i < localPeakList.Count && localPeakList.Count > 0)
                {
                    if (numberOfPeaksDetected > i)
                    {
                        sb.Append(localPeakList[i].Height);
                        if (localPeakList[i].Height > maxObserved)
                        {
                            maxObserved = localPeakList[i].Height;
                        }
                    }
                    else
                    {
                        sb.Append(0);
                    }
                }
                else
                {
                    sb.Append(0);
                }

                //sb.Append(delim);
            }
            //last line
            //if (numberOfPeaksDetected < numberOfIsotopesToPrint && localPeakList.Count > 0 && numberOfIsotopesToPrint < localPeakList.Count)
            //{
            //    sb.Append(localPeakList[numberOfIsotopesToPrint].Height);
            //}
            //else
            //{
            //    sb.Append(0);
            //}

            //sblength = 11

            //sb.Append(delim);
            //append normalized values
            for (int i = 0; i < numberOfIsotopesToPrint; i++)
            {
                if (localPeakList != null && localPeakList.Count > 0 && i < localPeakList.Count)
                {
                    if (numberOfPeaksDetected > i)
                    {
                        sb.Append(localPeakList[i].Height / maxObserved);
                    }
                    else
                    {
                        sb.Append(0);
                    }
                }
                else
                {
                    sb.Append(0);
                }

                //sb.Append(delim);
            }
            //last line
            //if (numberOfPeaksDetected > numberOfIsotopesToPrint && localPeakList.Count > 0 && numberOfIsotopesToPrint < localPeakList.Count)
            //{
            //    sb.Append(localPeakList[numberOfIsotopesToPrint].Height);
            //}
            //else
            //{
            //    sb.Append(0);
            //}



        }

        private static void AppendTheoryHedders1Set(int numberOfPenaltyIons, int numberOfIsotopeToPrint, string delim, StringBuilder sb)
        {
            for (int i = 0; i < numberOfIsotopeToPrint - numberOfPenaltyIons; i++)
            {
                sb.Append("TheoryIso_" + i);
                sb.Append(delim);
            }
            int finalIso2 = numberOfIsotopeToPrint - numberOfPenaltyIons;
            sb.Append("TheoryIso_" + finalIso2);
        }

        private static void AppendObservedHedders1Set(int numberOfPenaltyIons, int numberOfIsotopeToPrint, string delim, StringBuilder sb)
        {
            if (numberOfPenaltyIons > 0)
            {
                for (int i = 0; i < numberOfPenaltyIons; i++)
                {
                    sb.Append("Penalty");
                    sb.Append(delim);
                }
            }

            for (int i = 0; i < numberOfIsotopeToPrint - numberOfPenaltyIons - 1; i++)
            {
                sb.Append("Iso_" + i);
                sb.Append(delim);
            }
            int finalIso = numberOfIsotopeToPrint - numberOfPenaltyIons - 1;
            sb.Append("Iso_" + finalIso);
        }
    }
}
