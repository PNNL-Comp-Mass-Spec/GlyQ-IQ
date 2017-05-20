using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace GetPeaks_DLL.Isotopes
{
    public class IsotopeProfile
    {
        public IsotopeProfile()
        {
            this.IsotopeIntensityList = new List<double>();
            this.Charge = 1;
            this.Averagine = new Averagine();
            this.IsotopePattern = new TheoryIsotope();
        }
        
        /// <summary>
        /// generated intensities for each isotope
        /// </summary>
        public List<double> IsotopeIntensityList { get; set; }

        /// <summary>
        /// charge used to create mass spacing
        /// </summary>
        public int Charge { get; set; }

        /// <summary>
        /// elemental composition of averagine
        /// </summary>
        private Averagine Averagine { get; set; }

        /// <summary>
        /// where the intensities are generated
        /// </summary>
        private TheoryIsotope IsotopePattern { get; set; }

        public void GenerateProfile(double mass, Dictionary<string, double> averagineDictionary)
        {
            Averagine.isoC = averagineDictionary["C"];
            Averagine.isoH = averagineDictionary["H"];
            Averagine.isoN = averagineDictionary["N"];
            Averagine.isoO = averagineDictionary["O"];
            Averagine.isoS = averagineDictionary["S"];
            
            Averagine = Averagine.AveragineSetup(AveragineType.Custom);
            
   
            List<Peak> results = new List<Peak>();
            results = IsotopePattern.TheoryIsotopePoissonFX(mass, Averagine);

            foreach (Peak iPeak in results)
            {
                this.IsotopeIntensityList.Add(iPeak.Height);
            }
        }
    }
}
