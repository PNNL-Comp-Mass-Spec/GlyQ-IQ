using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend;
using DeconTools.Backend.Core;

namespace IQGlyQ
{
    [Serializable]
    public class FragmentTarget : TargetBase
    {
        #region Constructors

        public FragmentTarget()
            : base()
        {

        }


        public FragmentTarget(FragmentTarget copiedTarget)
            : base(copiedTarget)
        {
            this.DifferenceName = copiedTarget.DifferenceName;
            this.DifferenceID = copiedTarget.DifferenceID;
            this.RefID = copiedTarget.RefID;
            this.StartScan = copiedTarget.StartScan;
            this.StopScan = copiedTarget.StopScan;
        }

        public FragmentTarget(TargetBase copiedTarget)
            : base(copiedTarget)
        {
           
        }

        #endregion

        #region Properties

        /// <summary>
        /// the ID of the monosaccharide or other difference
        /// </summary>
        public int RefID { get; set; }

        public string DifferenceName { get; set; }
        public string DifferenceID { get; set; }

        public int StartScan { get; set; }
        public int StopScan { get; set; }

        public EnumerationError ErrorCode { get; set; }

        #endregion

        public override string GetEmpiricalFormulaFromTargetCode()
        {
            //this needs to be fixed
            return PeptideUtils.GetEmpiricalFormulaForPeptideSequence(Code);
        }

        public void CalculateMassesForIsotopicProfile(int chargeState)
        {
            if (this.IsotopicProfile == null || this.IsotopicProfile.Peaklist == null) return;

            for (int i = 0; i < this.IsotopicProfile.Peaklist.Count; i++)
            {
                double calcMZ = this.MonoIsotopicMass/chargeState + Globals.PROTON_MASS + i*1.00235/chargeState;
                this.IsotopicProfile.Peaklist[i].XValue = calcMZ;
            }

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(ID);
            sb.Append("; ");
            sb.Append(MonoIsotopicMass.ToString("0.0000"));
            sb.Append("; ");
            sb.Append(MZ.ToString("0.0000"));
            sb.Append("; ");
            sb.Append(ChargeState);
            sb.Append("; ");
            sb.Append(NormalizedElutionTime);
            sb.Append("; ");
            sb.Append(Code);

            return sb.ToString();
        }
    }
}