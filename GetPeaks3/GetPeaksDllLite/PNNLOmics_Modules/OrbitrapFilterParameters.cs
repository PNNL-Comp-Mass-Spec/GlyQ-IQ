using System;
using System.Collections.Generic;

namespace GetPeaksDllLite.PNNLOmics_Modules
{
    [Serializable]
    public class OrbitrapFilterParameters : IDisposable
    {
        public OrbitrapFilterParameters()
        {
            this.DeltaMassTollerancePPM = 500;//ppm
            //this.massNeutron = Constants.SubAtomicParticles[SubAtomicParticleName.Neutron].MassMonoIsotopic;//this.massNeutron =  = 1.0013;//TODO update this 1/2
            this.massNeutron = 1.00235; //horn
            this.ExtraSigmaFactor = 1;
            
            this.ChargeStateList = new List<int>();
            this.ChargeStateList.Add(1);
            this.ChargeStateList.Add(2);
            this.ChargeStateList.Add(3);
            this.ChargeStateList.Add(4);

            this.PeakSkipList = new List<int>();
            this.PeakSkipList.Add(1);
            this.PeakSkipList.Add(2);
            this.PeakSkipList.Add(3);
            this.PeakSkipList.Add(4);

            Dictionary<string, double> averagine = new Dictionary<string, double>();
            averagine.Add("C", 4.9384);//peptide
            averagine.Add("H", 7.7583);
            averagine.Add("N", 1.3577);
            averagine.Add("O", 1.4773);
            averagine.Add("S", 0.0417);
            this.LowMassFilterAveragine = averagine;
        }

        /// <summary>
        /// which charge states to consider for decharging
        /// </summary>
        public List<int> ChargeStateList { get; set; }

        /// <summary>
        /// difference between peak[i] and peak[i+?].  this allows for interfearing ions to be between peaks in an isotope cluster
        /// </summary>
        public List<int> PeakSkipList { get; set; }

        /// <summary>
        /// mass difference between peaks.  Theoretical or experimental...
        /// </summary>
        public double massNeutron { get; set; }

        /// <summary>
        /// Should we allow peaks without isotopes
        /// </summary>
        public bool withLowMassesAllowed { get; set; }

        /// <summary>
        /// Averagine used for identiftyinhg peaks without isotopes
        /// </summary>
        public Dictionary<string, double> LowMassFilterAveragine { get; set; }

        /// <summary>
        /// up the level for lowmass filtering by this factor.  ExtraSigmaFactor=1 for thermo's cuttoff
        /// </summary>
        public double ExtraSigmaFactor { get; set; }

        public double DeltaMassTollerancePPM { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            this.ChargeStateList = null;
            this.PeakSkipList = null;
        }

        #endregion
    }
}
