using System;
using System.Text;
using IQ.Workflows.Core;
using IQGlyQ.Objects;

namespace IQGlyQ
{
    [Serializable]
    public class FragmentIQTarget : IqTarget
    {
        #region Constructors

        public FragmentIQTarget()
            : base()
        {
            ScanInfo = new ScanObject(0,0);
            //this.Error = ErrorEnumeration.NoError;
        }

        public FragmentIQTarget(FragmentIQTarget copiedTarget)
            : base(copiedTarget)
        {
            this.DifferenceName = copiedTarget.DifferenceName;
            this.DifferenceID = copiedTarget.DifferenceID;
            this.RefID = copiedTarget.RefID;
            this.ScanInfo = copiedTarget.ScanInfo;
            this.ScanLCTarget = copiedTarget.ScanLCTarget;
            //this.IsotopicProfile = copiedTarget.IsotopicProfile;
            //this.Error = ErrorEnumeration.NoError;

        }


        public FragmentIQTarget(IqTarget copiedTarget)
            : base(copiedTarget)
        {
            ScanInfo = new ScanObject(0,0);
            //this.Error = ErrorEnumeration.NoError;
            //FragmentIQTarget newTarget = new FragmentIQTarget(copiedTarget, includeRecursion);
        }

        #endregion

        #region Properties

        /// <summary>
        /// the ID of the monosaccharide or other difference
        /// </summary>
        public int RefID { get; set; }

        public string DifferenceName { get; set; }
        public string DifferenceID { get; set; }

        //public int StartScan { get; set; }
        //public int StopScan { get; set; }
        public int ScanLCTarget { get; set; }

        public ScanObject ScanInfo { get; set; }
        //public IsotopicProfile IsotopicProfile { get; set; }// now on iq feature

        //public ErrorEnumeration Error { get; set; }

        //public IqTarget IqTargetBase { get; set; }

        #endregion

        //public override string GetEmpiricalFormulaFromTargetCode()
        //{
        //    //this needs to be fixed
        //    return PeptideUtils.GetEmpiricalFormulaForPeptideSequence(Code);
        //}

        //public void CalculateMassesForIsotopicProfile(int chargeState)
        //{
        //    if (this.IsotopicProfile == null || this.IsotopicProfile.Peaklist == null) return;

        //    for (int i = 0; i < this.IsotopicProfile.Peaklist.Count; i++)
        //    {
        //        double calcMZ = this.IsotopicProfile.MonoIsotopicMass/chargeState + Globals.PROTON_MASS + i*1.00235/chargeState;
        //        this.IsotopicProfile.Peaklist[i].XValue = calcMZ;
        //    }

        //}

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(ID);
            sb.Append("; ");
            //sb.Append(MonoIsotopicMass.ToString("0.0000"));
            //sb.Append("; ");
           
            //sb.Append(MZ.ToString("0.0000"));
            //sb.Append("; ");
            sb.Append(ChargeState);
            sb.Append("; ");
            //sb.Append(NormalizedElutionTime);
            //sb.Append("; ");
            sb.Append(Code);

            return sb.ToString();
        }
    }
}