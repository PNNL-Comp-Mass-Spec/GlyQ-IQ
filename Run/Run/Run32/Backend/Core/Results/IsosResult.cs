﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Run32.Backend.Core.Results
{
    [Serializable]
    public abstract class IsosResult
    {
        public int MSFeatureID { get; set; }
        public IList<ResultFlag> Flags = new List<ResultFlag>();

        public Run Run { get; set; }

        public ScanSet ScanSet { get; set; }

        public IsotopicProfile IsotopicProfile { get; set; }

        public double InterferenceScore { get; set; }

        public double IntensityAggregate { get; set; }


        public override string ToString()
        {
            if (this.IsotopicProfile == null) 
                return base.ToString();

            string delim = "; ";

            StringBuilder sb = new StringBuilder();
            sb.Append(this.MSFeatureID);
            sb.Append(delim);
            sb.Append(this.ScanSet.PrimaryScanNumber);
            sb.Append(delim);

            sb.Append(this.IsotopicProfile.MonoPeakMZ.ToString("0.00000"));
            sb.Append(delim);
            sb.Append(this.IsotopicProfile.ChargeState);
            sb.Append(delim);
            sb.Append(this.IntensityAggregate);
            sb.Append(delim);
            sb.Append(this.IsotopicProfile.Score.ToString("0.0000"));
            sb.Append(delim);
            sb.Append(this.InterferenceScore.ToString("0.0000"));

            return sb.ToString();
        }
        
        public void Display()
        {
            Console.WriteLine(this.ToString());

        }


    }
}
