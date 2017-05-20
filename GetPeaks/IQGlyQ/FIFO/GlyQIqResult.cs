using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQGlyQ.FIFO
{
    public class GlyQIqResult
    {
        public string TargetID { get; set; }
        public string Code { get; set; }
        public string EmpiricalFormula { get; set; }
        public string ChargeState { get; set; }
        public string MonomassTheor { get; set; }
        public string MZTheor { get; set; }
        public string ElutionTimeTheor { get; set; }
        public string MonoMassObs { get; set; }
        public string MZObs { get; set; }
        public string ElutionTimeObs { get; set; }
        public string ChromPeaksWithinTolerance { get; set; }
        public string Scan { get; set; }
        public string Abundance { get; set; }
        public string IsoFitScore { get; set; }
        public string InterferenceScore { get; set; }
        public string ScanNum { get; set; }
        public string FragmentCharge { get; set; }
        public string ParentCharge { get; set; }
        public string CorrelationScore { get; set; }
        public string IsIntact { get; set; }
        public string IsAntiCorrelated { get; set; }
        public string Name { get; set; }
        public string FragmentArea { get; set; }
        public string ParentArea { get; set; }
        public string Fragment_LM_R2 { get; set; }
        public string FragmentStart { get; set; }
        public string FragmentStop { get; set; }
        public string ParentStart { get; set; }
        public string ParentStop { get; set; }
        public string FragmentError { get; set; }
        public string ParentError { get; set; }
        public string GlobalChargeStateMin { get; set; }
        public string GlobalChargeStateMax { get; set; }
        public string GlobalAggregateAbundance { get; set; }
        public string GlobalIntactCount { get; set; }
        public string GlobalFutureTargetsCount { get; set; }
        public string GlobalResult { get; set; }
        public string ParentOrChild { get; set; }
        public string FinalDecision { get; set; }
        public string AverageMonoMass { get; set; }
        public string PPM { get; set; }
        public string TargetType { get; set; }
        public string IsoFit { get; set; }
        public string InterferenceValue { get; set; }
        public string ChargeCorrelation { get; set; }

        public GlyQIqResult()
        {
        }

        public GlyQIqResult(string[] resultWords)
        {
            if(resultWords.Length==45)
            {
                TargetID = resultWords[0];
                Code = resultWords[1];
                EmpiricalFormula = resultWords[2];
                ChargeState = resultWords[3];
                MonomassTheor = resultWords[4];
                MZTheor = resultWords[5];
                ElutionTimeTheor = resultWords[6];
                MonoMassObs = resultWords[7];
                MZObs = resultWords[8];
                ElutionTimeObs = resultWords[9];
                ChromPeaksWithinTolerance = resultWords[10];
                Scan = resultWords[11];
                Abundance = resultWords[12];
                IsoFitScore = resultWords[13];
                InterferenceScore = resultWords[14];
                ScanNum = resultWords[15];
                FragmentCharge = resultWords[16];
                ParentCharge = resultWords[17];
                CorrelationScore = resultWords[18];
                IsIntact = resultWords[19];
                IsAntiCorrelated = resultWords[20];
                Name = resultWords[21];
                FragmentArea = resultWords[22];
                ParentArea = resultWords[23];
                Fragment_LM_R2 = resultWords[24];
                FragmentStart = resultWords[25];
                FragmentStop = resultWords[26];
                ParentStart = resultWords[27];
                ParentStop = resultWords[28];
                FragmentError = resultWords[29];
                ParentError = resultWords[30];
                GlobalChargeStateMin = resultWords[31];
                GlobalChargeStateMax = resultWords[32];
                GlobalAggregateAbundance = resultWords[33];
                GlobalIntactCount = resultWords[34];
                GlobalFutureTargetsCount = resultWords[35];
                GlobalResult = resultWords[36];
                ParentOrChild = resultWords[37];
                FinalDecision = resultWords[38];
                AverageMonoMass = resultWords[39];
                PPM = resultWords[40];
                TargetType = resultWords[41];
                IsoFit = resultWords[42];
                InterferenceValue = resultWords[43];
                ChargeCorrelation = resultWords[44];

            }
            else
            {
                Console.WriteLine("MissingColumn");
                Console.ReadKey();

            }
        }

        public string GlyQIqResultToString(string delim="\t")
        {
            string resultAsString = "";

            resultAsString += TargetID;resultAsString += delim;
            resultAsString += Code;resultAsString += delim;
            resultAsString += EmpiricalFormula;resultAsString += delim;
            resultAsString += ChargeState;resultAsString += delim;
            resultAsString += MonomassTheor;resultAsString += delim;
            resultAsString += MZTheor;resultAsString += delim;
            resultAsString += ElutionTimeTheor;resultAsString += delim;
            resultAsString += MonoMassObs;resultAsString += delim;
            resultAsString += MZObs;resultAsString += delim;
            resultAsString += ElutionTimeObs;resultAsString += delim;
            resultAsString += ChromPeaksWithinTolerance;resultAsString += delim;
            resultAsString += Scan;resultAsString += delim;
            resultAsString += Abundance;resultAsString += delim;
            resultAsString += IsoFitScore;resultAsString += delim;
            resultAsString += InterferenceScore;resultAsString += delim;
            resultAsString += ScanNum;resultAsString += delim;
            resultAsString += FragmentCharge;resultAsString += delim;
            resultAsString += ParentCharge;resultAsString += delim;
            resultAsString += CorrelationScore;resultAsString += delim;
            resultAsString += IsIntact;resultAsString += delim;
            resultAsString += IsAntiCorrelated;resultAsString += delim;
            resultAsString += Name;resultAsString += delim;
            resultAsString += FragmentArea;resultAsString += delim;
            resultAsString += ParentArea;resultAsString += delim;
            resultAsString += Fragment_LM_R2;resultAsString += delim;
            resultAsString += FragmentStart;resultAsString += delim;
            resultAsString += FragmentStop;resultAsString += delim;
            resultAsString += ParentStart;resultAsString += delim;
            resultAsString += ParentStop;resultAsString += delim;
            resultAsString += FragmentError;resultAsString += delim;
            resultAsString += ParentError;resultAsString += delim;
            resultAsString += GlobalChargeStateMin;resultAsString += delim;
            resultAsString += GlobalChargeStateMax;resultAsString += delim;
            resultAsString += GlobalAggregateAbundance;resultAsString += delim;
            resultAsString += GlobalIntactCount;resultAsString += delim;
            resultAsString += GlobalFutureTargetsCount;resultAsString += delim;
            resultAsString += GlobalResult;resultAsString += delim;
            resultAsString += ParentOrChild;resultAsString += delim;
            resultAsString += FinalDecision;resultAsString += delim;
            resultAsString += AverageMonoMass;resultAsString += delim;
            resultAsString += PPM;resultAsString += delim;
            resultAsString += TargetType;resultAsString += delim;
            resultAsString += IsoFit;resultAsString += delim;
            resultAsString += InterferenceValue += delim;
            resultAsString += ChargeCorrelation;//NO DELIM

            return resultAsString;
        }
    }
}
