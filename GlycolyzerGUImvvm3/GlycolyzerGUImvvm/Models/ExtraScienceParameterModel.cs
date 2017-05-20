/* Written by Myanna Harris
* for the Department of Energy (PNNL, Richland, WA)
* Battelle Memorial Institute
* E-mail: Myanna.Harris@pnnl.gov
* Website: http://omics.pnl.gov/software
* -----------------------------------------------------
* 
 * Notice: This computer software was prepared by Battelle Memorial Institute,
* hereinafter the Contractor, under Contract No. DE-AC05-76RL0 1830 with the
* Department of Energy (DOE).  All rights in the computer software are reserved
* by DOE on behalf of the United States Government and the Contractor as
* provided in the Contract.
* 
 * NEITHER THE GOVERNMENT NOR THE CONTRACTOR MAKES ANY WARRANTY, EXPRESS OR
* IMPLIED, OR ASSUMES ANY LIABILITY FOR THE USE OF THIS SOFTWARE.
* 
 * This notice including this sentence must appear on any copies of this computer
* software.
* -----------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlycolyzerGUImvvm.Models
{
    public class ExtraScienceParameterModel : ObservableObject
    {
        private int numberOfChargesExtraParameter_Int = 1;
        private Double mzToleranceExtraParameter_Double = 0.0;
        private String mzToleranceTypeExtraParameter_String = "ppm";
        private String carbohydrateTypeExtraParameter_String = "Aldehyde";
        private String chargeCarryingSpeciesExtraParameter_String = "H";
        private String featureOriginTypeExtraParameter_String = "Multialign";


        public int NumberOfChargesExtraParameter_Int
        {
            get { return numberOfChargesExtraParameter_Int; }
            set { if (value != numberOfChargesExtraParameter_Int) { numberOfChargesExtraParameter_Int = (int)value; OnPropertyChanged("numberOfChargesExtraParameter_Int"); } }
        }

        public Double MzToleranceExtraParameter_Double
        {
            get { return mzToleranceExtraParameter_Double; }
            set { if (value != mzToleranceExtraParameter_Double) { mzToleranceExtraParameter_Double = (Double)value; OnPropertyChanged("mzToleranceExtraParameter_Double"); } }
        }

        public String MzToleranceTypeExtraParameter_String
        {
            get { return mzToleranceTypeExtraParameter_String; }
            set { if (value != mzToleranceTypeExtraParameter_String) { mzToleranceTypeExtraParameter_String = value; OnPropertyChanged("mzToleranceTypeExtraParameter_String"); } }
        }

        public String CarbohydrateTypeExtraParameter_String
        {
            get { return carbohydrateTypeExtraParameter_String; }
            set { if (value != carbohydrateTypeExtraParameter_String) { carbohydrateTypeExtraParameter_String = value; OnPropertyChanged("carbohydrateTypeExtraParameter_String"); } }
        }

        public String ChargeCarryingSpeciesExtraParameter_String
        {
            get { return chargeCarryingSpeciesExtraParameter_String; }
            set { if (value != chargeCarryingSpeciesExtraParameter_String) { chargeCarryingSpeciesExtraParameter_String = value; OnPropertyChanged("chargeCarryingSpeciesExtraParameter_String"); } }
        }

        public String FeatureOriginTypeExtraParameter_String
        {
            get { return featureOriginTypeExtraParameter_String; }
            set { if (value != featureOriginTypeExtraParameter_String) { featureOriginTypeExtraParameter_String = value; OnPropertyChanged("featureOriginTypeExtraParameter_String"); } }
        }
    }
}
