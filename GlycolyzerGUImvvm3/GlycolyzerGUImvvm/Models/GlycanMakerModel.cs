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
    public class GlycanMakerModel : ObservableObject
    {
        //Sugars
        private int numberOfHexose_Int = 0;
        private int numberOfHexNAc_Int = 0;
        private int numberOfDeoxyhexose_Int = 0;
        private int numberOfPentose_Int = 0;
        private int numberOfNeuAc_Int = 0;
        private int numberOfNeuGc_Int = 0;
        private int numberOfKDN_Int = 0;
        private int numberOfHexA_Int = 0;

        //User Units A and B
        private int numberOfUserUnit1_Int = 0;
        private int numberOfUserUnit2_Int = 0;

        //Specials
        private int numberOfNaH_Int = 0;
        private int numberOfCH3_Int = 0;
        private int numberOfSO3_Int = 0;
        private int numberOfOAcetyl_Int = 0;

        //Amino Acids
        private int numberOfAla_Int = 0;
        private int numberOfArg_Int = 0;
        private int numberOfAsn_Int = 0;
        private int numberOfAsp_Int = 0;
        private int numberOfCys_Int = 0;
        private int numberOfGln_Int = 0;
        private int numberOfGlu_Int = 0;
        private int numberOfGly_Int = 0;
        private int numberOfHis_Int = 0;
        private int numberOfIle_Int = 0;
        private int numberOfLeu_Int = 0;
        private int numberOfLys_Int = 0;
        private int numberOfMet_Int = 0;
        private int numberOfPhe_Int = 0;
        private int numberOfSer_Int = 0;
        private int numberOfThr_Int = 0;
        private int numberOfTrp_Int = 0;
        private int numberOfTyr_Int = 0;
        private int numberOfVal_Int = 0;
        private int numberOfPro_Int = 0;

        //Permethyl
        private int numberOfpHex_Int = 0;
        private int numberOfpHxNAc_Int = 0;
        private int numberOfpDxHex_Int = 0;
        private int numberOfpPntos_Int = 0;
        private int numberOfpNuAc_Int = 0;
        private int numberOfpNuGc_Int = 0;
        private int numberOfpKDN_Int = 0;
        private int numberOfpHxA_Int = 0;

        //Charge Carrier
        private String typeOfChargeCarrier_String = "H";
        private int numberOfChargeCarrier_Int = 1;

        //Carbohydrate Type
        private String carbohydrateType_String = "Aldehyde";

        //Neutral Mass
        private Double neutralMass_Double = 0.0;

        //Mass/Charge
        private static Double massCharge_Double = 0.0;

        //Number of C, H, O, N, Na, S
        private int numberOfC_Int = 0;
        private int numberOfH_Int = 0;
        private int numberOfO_Int = 0;
        private int numberOfN_Int = 0;
        private int numberOfNa_Int = 0;
        private int numberOfS_Int = 0;


        //Sugars
        public int NumberOfHexose_Int
        {
            get { return numberOfHexose_Int; }
            set { if (value != numberOfHexose_Int) { numberOfHexose_Int = (int)value; OnPropertyChanged("numberOfHexose_Int"); } }
        }

        public int NumberOfHexNAc_Int
        {
            get { return numberOfHexNAc_Int; }
            set { if (value != numberOfHexNAc_Int) { numberOfHexNAc_Int = (int)value; OnPropertyChanged("numberOfHexNAc_Int"); } }
        }

        public int NumberOfDeoxyhexose_Int
        {
            get { return numberOfDeoxyhexose_Int; }
            set { if (value != numberOfDeoxyhexose_Int) { numberOfDeoxyhexose_Int = (int)value; OnPropertyChanged("numberOfDeoxyhexose_Int"); } }
        }

        public int NumberOfPentose_Int
        {
            get { return numberOfPentose_Int; }
            set { if (value != numberOfPentose_Int) { numberOfPentose_Int = (int)value; OnPropertyChanged("numberOfPentose_Int"); } }
        }

        public int NumberOfNeuAc_Int
        {
            get { return numberOfNeuAc_Int; }
            set { if (value != numberOfNeuAc_Int) { numberOfNeuAc_Int = (int)value; OnPropertyChanged("numberOfNeuAc_Int"); } }
        }

        public int NumberOfNeuGc_Int
        {
            get { return numberOfNeuGc_Int; }
            set { if (value != numberOfNeuGc_Int) { numberOfNeuGc_Int = (int)value; OnPropertyChanged("numberOfNeuGc_Int"); } }
        }

        public int NumberOfKDN_Int
        {
            get { return numberOfKDN_Int; }
            set { if (value != numberOfKDN_Int) { numberOfKDN_Int = (int)value; OnPropertyChanged("numberOfKDN_Int"); } }
        }

        public int NumberOfHexA_Int
        {
            get { return numberOfHexA_Int; }
            set { if (value != numberOfHexA_Int) { numberOfHexA_Int = (int)value; OnPropertyChanged("numberOfHexA_Int"); } }
        }

        //User Units A and B
        public int NumberOfUserUnit1_Int
        {
            get { return numberOfUserUnit1_Int; }
            set { if (value != numberOfUserUnit1_Int) { numberOfUserUnit1_Int = (int)value; OnPropertyChanged("numberOfUserUnit1_Int"); } }
        }

        public int NumberOfUserUnit2_Int
        {
            get { return numberOfUserUnit2_Int; }
            set { if (value != numberOfUserUnit2_Int) { numberOfUserUnit2_Int = (int)value; OnPropertyChanged("numberOfUserUnit2_Int"); } }
        }

        //Specials
        public int NumberOfNaH_Int
        {
            get { return numberOfNaH_Int; }
            set { if (value != numberOfNaH_Int) { numberOfNaH_Int = (int)value; OnPropertyChanged("numberOfNaH_Int"); } }
        }

        public int NumberOfCH3_Int
        {
            get { return numberOfCH3_Int; }
            set { if (value != numberOfCH3_Int) { numberOfCH3_Int = (int)value; OnPropertyChanged("numberOfCH3_Int"); } }
        }

        public int NumberOfSO3_Int
        {
            get { return numberOfSO3_Int; }
            set { if (value != numberOfSO3_Int) { numberOfSO3_Int = (int)value; OnPropertyChanged("numberOfSO3_Int"); } }
        }

        public int NumberOfOAcetyl_Int
        {
            get { return numberOfOAcetyl_Int; }
            set { if (value != numberOfOAcetyl_Int) { numberOfOAcetyl_Int = (int)value; OnPropertyChanged("numberOfOAcetyl_Int"); } }
        }

        //Amino Acids
        public int NumberOfAla_Int
        {
            get { return numberOfAla_Int; }
            set { if (value != numberOfAla_Int) { numberOfAla_Int = (int)value; OnPropertyChanged("numberOfAla_Int"); } }
        }

        public int NumberOfArg_Int
        {
            get { return numberOfArg_Int; }
            set { if (value != numberOfArg_Int) { numberOfArg_Int = (int)value; OnPropertyChanged("numberOfArg_Int"); } }
        }

        public int NumberOfAsn_Int
        {
            get { return numberOfAsn_Int; }
            set { if (value != numberOfAsn_Int) { numberOfAsn_Int = (int)value; OnPropertyChanged("numberOfAsn_Int"); } }
        }
        public int NumberOfAsp_Int
        {
            get { return numberOfAsp_Int; }
            set { if (value != numberOfAsp_Int) { numberOfAsp_Int = (int)value; OnPropertyChanged("numberOfAsp_Int"); } }
        }

        public int NumberOfCys_Int
        {
            get { return numberOfCys_Int; }
            set { if (value != numberOfCys_Int) { numberOfCys_Int = (int)value; OnPropertyChanged("numberOfCys_Int"); } }
        }

        public int NumberOfGln_Int
        {
            get { return numberOfGln_Int; }
            set { if (value != numberOfGln_Int) { numberOfGln_Int = (int)value; OnPropertyChanged("numberOfGln_Int"); } }
        }

        public int NumberOfGlu_Int
        {
            get { return numberOfGlu_Int; }
            set { if (value != numberOfGlu_Int) { numberOfGlu_Int = (int)value; OnPropertyChanged("numberOfGlu_Int"); } }
        }

        public int NumberOfGly_Int
        {
            get { return numberOfGly_Int; }
            set { if (value != numberOfGly_Int) { numberOfGly_Int = (int)value; OnPropertyChanged("numberOfGly_Int"); } }
        }

        public int NumberOfHis_Int
        {
            get { return numberOfHis_Int; }
            set { if (value != numberOfHis_Int) { numberOfHis_Int = (int)value; OnPropertyChanged("numberOfHis_Int"); } }
        }
        public int NumberOfIle_Int
        {
            get { return numberOfIle_Int; }
            set { if (value != numberOfIle_Int) { numberOfIle_Int = (int)value; OnPropertyChanged("numberOfIle_Int"); } }
        }

        public int NumberOfLeu_Int
        {
            get { return numberOfLeu_Int; }
            set { if (value != numberOfLeu_Int) { numberOfLeu_Int = (int)value; OnPropertyChanged("numberOfLeu_Int"); } }
        }

        public int NumberOfLys_Int
        {
            get { return numberOfLys_Int; }
            set { if (value != numberOfLys_Int) { numberOfLys_Int = (int)value; OnPropertyChanged("numberOfLys_Int"); } }
        }
        public int NumberOfMet_Int
        {
            get { return numberOfMet_Int; }
            set { if (value != numberOfMet_Int) { numberOfMet_Int = (int)value; OnPropertyChanged("numberOfMet_Int"); } }
        }

        public int NumberOfPhe_Int
        {
            get { return numberOfPhe_Int; }
            set { if (value != numberOfPhe_Int) { numberOfPhe_Int = (int)value; OnPropertyChanged("numberOfPhe_Int"); } }
        }

        public int NumberOfSer_Int
        {
            get { return numberOfSer_Int; }
            set { if (value != numberOfSer_Int) { numberOfSer_Int = (int)value; OnPropertyChanged("numberOfSer_Int"); } }
        }
        public int NumberOfThr_Int
        {
            get { return numberOfThr_Int; }
            set { if (value != numberOfThr_Int) { numberOfThr_Int = (int)value; OnPropertyChanged("numberOfThr_Int"); } }
        }

        public int NumberOfTrp_Int
        {
            get { return numberOfTrp_Int; }
            set { if (value != numberOfTrp_Int) { numberOfTrp_Int = (int)value; OnPropertyChanged("numberOfTrp_Int"); } }
        }

        public int NumberOfTyr_Int
        {
            get { return numberOfTyr_Int; }
            set { if (value != numberOfTyr_Int) { numberOfTyr_Int = (int)value; OnPropertyChanged("numberOfTyr_Int"); } }
        }
        public int NumberOfVal_Int
        {
            get { return numberOfVal_Int; }
            set { if (value != numberOfVal_Int) { numberOfVal_Int = (int)value; OnPropertyChanged("numberOfVal_Int"); } }
        }

        public int NumberOfPro_Int
        {
            get { return numberOfPro_Int; }
            set { if (value != numberOfPro_Int) { numberOfPro_Int = (int)value; OnPropertyChanged("numberOfPro_Int"); } }
        }

        //Permethyl
        public int NumberOfpHex_Int
        {
            get { return numberOfpHex_Int; }
            set { if (value != numberOfpHex_Int) { numberOfpHex_Int = (int)value; OnPropertyChanged("numberOfpHex_Int"); } }
        }

        public int NumberOfpHxNAc_Int
        {
            get { return numberOfpHxNAc_Int; }
            set { if (value != numberOfpHxNAc_Int) { numberOfpHxNAc_Int = (int)value; OnPropertyChanged("numberOfpHxNAc_Int"); } }
        }

        public int NumberOfpDxHex_Int
        {
            get { return numberOfpDxHex_Int; }
            set { if (value != numberOfpDxHex_Int) { numberOfpDxHex_Int = (int)value; OnPropertyChanged("numberOfpDxHex_Int"); } }
        }

        public int NumberOfpPntos_Int
        {
            get { return numberOfpPntos_Int; }
            set { if (value != numberOfpPntos_Int) { numberOfpPntos_Int = (int)value; OnPropertyChanged("numberOfpPntos_Int"); } }
        }

        public int NumberOfpNuAc_Int
        {
            get { return numberOfpNuAc_Int; }
            set { if (value != numberOfpNuAc_Int) { numberOfpNuAc_Int = (int)value; OnPropertyChanged("numberOfpNuAc_Int"); } }
        }

        public int NumberOfpNuGc_Int
        {
            get { return numberOfpNuGc_Int; }
            set { if (value != numberOfpNuGc_Int) { numberOfpNuGc_Int = (int)value; OnPropertyChanged("numberOfpNuGc_Int"); } }
        }

        public int NumberOfpKDN_Int
        {
            get { return numberOfpKDN_Int; }
            set { if (value != numberOfpKDN_Int) { numberOfpKDN_Int = (int)value; OnPropertyChanged("numberOfpKDN_Int"); } }
        }

        public int NumberOfpHxA_Int
        {
            get { return numberOfpHxA_Int; }
            set { if (value != numberOfpHxA_Int) { numberOfpHxA_Int = (int)value; OnPropertyChanged("numberOfpHxA_Int"); } }
        }

        //Charge Carrier
        public String TypeOfChargeCarrier_String
        {
            get { return typeOfChargeCarrier_String; }
            set { if (value != typeOfChargeCarrier_String) { typeOfChargeCarrier_String = value; OnPropertyChanged("typeOfChargeCarrier_String"); } }
        }

        public int NumberOfChargeCarrier_Int
        {
            get { return numberOfChargeCarrier_Int; }
            set { if (value != numberOfChargeCarrier_Int) { numberOfChargeCarrier_Int = (int)value; OnPropertyChanged("numberOfChargeCarrier_Int"); } }
        }

        //Carbohydrate Type
        public String CarbohydrateType_String
        {
            get { return carbohydrateType_String; }
            set { if (value != carbohydrateType_String) { carbohydrateType_String = value; OnPropertyChanged("carbohydrateType_String"); } }
        }

        //Neutral Mass
        public Double NeutralMass_Double
        {
            get { return neutralMass_Double; }
            set { if (value != neutralMass_Double) { neutralMass_Double = (Double)value; OnPropertyChanged("neutralMass_Double"); } }
        }

        //Mass/Charge
        public Double MassCharge_Double
        {
            get { return massCharge_Double; }
            set { if (value != massCharge_Double) { massCharge_Double = (Double)value; OnPropertyChanged("massCharge_Double"); } }
        }

        //Number of C, H, O, N, Na, S
        public int NumberOfC_Int
        {
            get { return numberOfC_Int; }
            set { if (value != numberOfC_Int) { numberOfC_Int = (int)value; OnPropertyChanged("numberOfC_Int"); } }
        }

        public int NumberOfH_Int
        {
            get { return numberOfH_Int; }
            set { if (value != numberOfH_Int) { numberOfH_Int = (int)value; OnPropertyChanged("numberOfH_Int"); } }
        }

        public int NumberOfO_Int
        {
            get { return numberOfO_Int; }
            set { if (value != numberOfO_Int) { numberOfO_Int = (int)value; OnPropertyChanged("numberOfO_Int"); } }
        }

        public int NumberOfN_Int
        {
            get { return numberOfN_Int; }
            set { if (value != numberOfN_Int) { numberOfN_Int = (int)value; OnPropertyChanged("numberOfN_Int"); } }
        }

        public int NumberOfNa_Int
        {
            get { return numberOfNa_Int; }
            set { if (value != numberOfNa_Int) { numberOfNa_Int = (int)value; OnPropertyChanged("numberOfNa_Int"); } }
        }

        public int NumberOfS_Int
        {
            get { return numberOfS_Int; }
            set { if (value != numberOfS_Int) { numberOfS_Int = (int)value; OnPropertyChanged("numberOfS_Int"); } }
        }
    }
}
