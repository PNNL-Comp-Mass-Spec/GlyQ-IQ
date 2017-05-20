using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GlycolyzerGUImvvm.Models
{
    public class OmniFinderGMModel : ObservableObject
    {
        //Select Options
        private String selectedOption_String = "No Option Selected";

        //Sugars
        private Boolean checkedHexose_Bool = false;
        private Boolean checkedHexNAc_Bool = false;
        private Boolean checkedDxyHex_Bool = false;
        private Boolean checkedPentose_Bool = false;
        private Boolean checkedNeuAc_Bool = false;
        private Boolean checkedNeuGc_Bool = false;
        private Boolean checkedKDN_Bool = false;
        private Boolean checkedHexA_Bool = false;

        //User Units
        private Boolean checkedUserUnit1_Bool = false;
        private Boolean checkedUserUnit2_Bool = false;
        private Boolean checkedUserUnit3_Bool = false;
        private Boolean checkedUserUnit4_Bool = false;
        private Boolean checkedUserUnit5_Bool = false;
        private Boolean checkedUserUnit6_Bool = false;
        private Boolean checkedUserUnit7_Bool = false;
        private Boolean checkedUserUnit8_Bool = false;
        private Boolean checkedUserUnit9_Bool = false;
        private Boolean checkedUserUnit10_Bool = false;

        //Amino Acids
        private Boolean checkedAla_Bool = false;
        private Boolean checkedArg_Bool = false;
        private Boolean checkedAsn_Bool = false;
        private Boolean checkedAsp_Bool = false;
        private Boolean checkedCys_Bool = false;
        private Boolean checkedGln_Bool = false;
        private Boolean checkedGlu_Bool = false;
        private Boolean checkedGly_Bool = false;
        private Boolean checkedHis_Bool = false;
        private Boolean checkedIle_Bool = false;
        private Boolean checkedLeu_Bool = false;
        private Boolean checkedLys_Bool = false;
        private Boolean checkedMet_Bool = false;
        private Boolean checkedPhe_Bool = false;
        private Boolean checkedSer_Bool = false;
        private Boolean checkedThr_Bool = false;
        private Boolean checkedTrp_Bool = false;
        private Boolean checkedTyr_Bool = false;
        private Boolean checkedVal_Bool = false;
        private Boolean checkedPro_Bool = false;

        //Special
        private Boolean checkedNaH_Bool = false;
        private Boolean checkedCH3_Bool = false;
        private Boolean checkedSO3_Bool = false;
        private Boolean checkedOAcetyl_Bool = false;

        //Permethyl
        private Boolean checkedpHex_Bool = false;
        private Boolean checkedpHxNAc_Bool = false;
        private Boolean checkedpDxHex_Bool = false;
        private Boolean checkedpPntos_Bool = false;
        private Boolean checkedpNuAc_Bool = false;
        private Boolean checkedpNuGc_Bool = false;
        private Boolean checkedpKDN_Bool = false;
        private Boolean checkedpHxA_Bool = false;


        //Select Options
        public String SelectedOption_String
        {
            get { return selectedOption_String; }
            set { if (value != selectedOption_String) { selectedOption_String = value; OnPropertyChanged("selectedOption_String"); } }
        }

        //User Units
        public Boolean CheckedUserUnit1_Bool
        {
            get { return checkedUserUnit1_Bool; }
            set { if (value != checkedUserUnit1_Bool) { checkedUserUnit1_Bool = value; OnPropertyChanged("checkedUserUnit1_Bool"); } }
        }

        public Boolean CheckedUserUnit2_Bool
        {
            get { return checkedUserUnit2_Bool; }
            set { if (value != checkedUserUnit2_Bool) { checkedUserUnit2_Bool = value; OnPropertyChanged("checkedUserUnit2_Bool"); } }
        }

        public Boolean CheckedUserUnit3_Bool
        {
            get { return checkedUserUnit3_Bool; }
            set { if (value != checkedUserUnit3_Bool) { checkedUserUnit3_Bool = value; OnPropertyChanged("checkedUserUnit3_Bool"); } }
        }

        public Boolean CheckedUserUnit4_Bool
        {
            get { return checkedUserUnit4_Bool; }
            set { if (value != checkedUserUnit4_Bool) { checkedUserUnit4_Bool = value; OnPropertyChanged("checkedUserUnit4_Bool"); } }
        }

        public Boolean CheckedUserUnit5_Bool
        {
            get { return checkedUserUnit5_Bool; }
            set { if (value != checkedUserUnit5_Bool) { checkedUserUnit5_Bool = value; OnPropertyChanged("checkedUserUnit5_Bool"); } }
        }

        public Boolean CheckedUserUnit6_Bool
        {
            get { return checkedUserUnit6_Bool; }
            set { if (value != checkedUserUnit6_Bool) { checkedUserUnit6_Bool = value; OnPropertyChanged("checkedUserUnit6_Bool"); } }
        }

        public Boolean CheckedUserUnit7_Bool
        {
            get { return checkedUserUnit7_Bool; }
            set { if (value != checkedUserUnit7_Bool) { checkedUserUnit7_Bool = value; OnPropertyChanged("checkedUserUnit7_Bool"); } }
        }

        public Boolean CheckedUserUnit8_Bool
        {
            get { return checkedUserUnit8_Bool; }
            set { if (value != checkedUserUnit8_Bool) { checkedUserUnit8_Bool = value; OnPropertyChanged("checkedUserUnit8_Bool"); } }
        }

        public Boolean CheckedUserUnit9_Bool
        {
            get { return checkedUserUnit9_Bool; }
            set { if (value != checkedUserUnit9_Bool) { checkedUserUnit9_Bool = value; OnPropertyChanged("checkedUserUnit9_Bool"); } }
        }

        public Boolean CheckedUserUnit10_Bool
        {
            get { return checkedUserUnit10_Bool; }
            set { if (value != checkedUserUnit10_Bool) { checkedUserUnit10_Bool = value; OnPropertyChanged("checkedUserUnit10_Bool"); } }
        }

        //Sugars
        public Boolean CheckedHexose_Bool
        {
            get { return checkedHexose_Bool; }
            set { if (value != checkedHexose_Bool) { checkedHexose_Bool = value; OnPropertyChanged("checkedHexose_Bool"); } }
        }

        public Boolean CheckedHexNAc_Bool
        {
            get { return checkedHexNAc_Bool; }
            set { if (value != checkedHexNAc_Bool) { checkedHexNAc_Bool = value; OnPropertyChanged("checkedHexNAc_Bool"); } }
        }

        public Boolean CheckedDxyHex_Bool
        {
            get { return checkedDxyHex_Bool; }
            set { if (value != checkedDxyHex_Bool) { checkedDxyHex_Bool = value; OnPropertyChanged("checkedDxyHex_Bool"); } }
        }

        public Boolean CheckedPentose_Bool
        {
            get { return checkedPentose_Bool; }
            set { if (value != checkedPentose_Bool) { checkedPentose_Bool = value; OnPropertyChanged("checkedPentose_Bool"); } }
        }

        public Boolean CheckedNeuAc_Bool
        {
            get { return checkedNeuAc_Bool; }
            set { if (value != checkedNeuAc_Bool) { checkedNeuAc_Bool = value; OnPropertyChanged("checkedNeuAc_Bool"); } }
        }

        public Boolean CheckedNeuGc_Bool
        {
            get { return checkedNeuGc_Bool; }
            set { if (value != checkedNeuGc_Bool) { checkedNeuGc_Bool = value; OnPropertyChanged("checkedNeuGc_Bool"); } }
        }

        public Boolean CheckedKDN_Bool
        {
            get { return checkedKDN_Bool; }
            set { if (value != checkedKDN_Bool) { checkedKDN_Bool = value; OnPropertyChanged("checkedKDN_Bool"); } }
        }

        public Boolean CheckedHexA_Bool
        {
            get { return checkedHexA_Bool; }
            set { if (value != checkedHexA_Bool) { checkedHexA_Bool = value; OnPropertyChanged("checkedHexA_Bool"); } }
        }

        //Amino Acids
        public Boolean CheckedAla_Bool
        {
            get { return checkedAla_Bool; }
            set { if (value != checkedAla_Bool) { checkedAla_Bool = value; OnPropertyChanged("checkedAla_Bool"); } }
        }

        public Boolean CheckedArg_Bool
        {
            get { return checkedArg_Bool; }
            set { if (value != checkedArg_Bool) { checkedArg_Bool = value; OnPropertyChanged("checkedArg_Bool"); } }
        }

        public Boolean CheckedAsn_Bool
        {
            get { return checkedAsn_Bool; }
            set { if (value != checkedAsn_Bool) { checkedAsn_Bool = value; OnPropertyChanged("checkedAsn_Bool"); } }
        }

        public Boolean CheckedAsp_Bool
        {
            get { return checkedAsn_Bool; }
            set { if (value != checkedAsp_Bool) { checkedAsp_Bool = value; OnPropertyChanged("checkedAsp_Bool"); } }
        }

        public Boolean CheckedCys_Bool
        {
            get { return checkedCys_Bool; }
            set { if (value != checkedCys_Bool) { checkedCys_Bool = value; OnPropertyChanged("checkedCys_Bool"); } }
        }

        public Boolean CheckedGln_Bool
        {
            get { return checkedGln_Bool; }
            set { if (value != checkedGln_Bool) { checkedGln_Bool = value; OnPropertyChanged("checkedGln_Bool"); } }
        }

        public Boolean CheckedGlu_Bool
        {
            get { return checkedGlu_Bool; }
            set { if (value != checkedGlu_Bool) { checkedGlu_Bool = value; OnPropertyChanged("checkedGlu_Bool"); } }
        }

        public Boolean CheckedGly_Bool
        {
            get { return checkedGly_Bool; }
            set { if (value != checkedGly_Bool) { checkedGly_Bool = value; OnPropertyChanged("checkedGly_Bool"); } }
        }

        public Boolean CheckedHis_Bool
        {
            get { return checkedHis_Bool; }
            set { if (value != checkedHis_Bool) { checkedHis_Bool = value; OnPropertyChanged("checkedHis_Bool"); } }
        }

        public Boolean CheckedIle_Bool
        {
            get { return checkedIle_Bool; }
            set { if (value != checkedIle_Bool) { checkedIle_Bool = value; OnPropertyChanged("checkedIle_Bool"); } }
        }

        public Boolean CheckedLeu_Bool
        {
            get { return checkedLeu_Bool; }
            set { if (value != checkedLeu_Bool) { checkedLeu_Bool = value; OnPropertyChanged("checkedLeu_Bool"); } }
        }

        public Boolean CheckedLys_Bool
        {
            get { return checkedLys_Bool; }
            set { if (value != checkedLys_Bool) { checkedLys_Bool = value; OnPropertyChanged("checkedLys_Bool"); } }
        }

        public Boolean CheckedMet_Bool
        {
            get { return checkedMet_Bool; }
            set { if (value != checkedMet_Bool) { checkedMet_Bool = value; OnPropertyChanged("checkedMet_Bool"); } }
        }

        public Boolean CheckedPhe_Bool
        {
            get { return checkedPhe_Bool; }
            set { if (value != checkedPhe_Bool) { checkedPhe_Bool = value; OnPropertyChanged("checkedPhe_Bool"); } }
        }

        public Boolean CheckedSer_Bool
        {
            get { return checkedSer_Bool; }
            set { if (value != checkedSer_Bool) { checkedSer_Bool = value; OnPropertyChanged("checkedSer_Bool"); } }
        }

        public Boolean CheckedThr_Bool
        {
            get { return checkedThr_Bool; }
            set { if (value != checkedThr_Bool) { checkedThr_Bool = value; OnPropertyChanged("checkedThr_Bool"); } }
        }

        public Boolean CheckedTrp_Bool
        {
            get { return checkedTrp_Bool; }
            set { if (value != checkedTrp_Bool) { checkedTrp_Bool = value; OnPropertyChanged("checkedTrp_Bool"); } }
        }

        public Boolean CheckedTyr_Bool
        {
            get { return checkedTyr_Bool; }
            set { if (value != checkedTyr_Bool) { checkedTyr_Bool = value; OnPropertyChanged("checkedTyr_Bool"); } }
        }

        public Boolean CheckedVal_Bool
        {
            get { return checkedVal_Bool; }
            set { if (value != checkedVal_Bool) { checkedVal_Bool = value; OnPropertyChanged("checkedVal_Bool"); } }
        }

        public Boolean CheckedPro_Bool
        {
            get { return checkedPro_Bool; }
            set { if (value != checkedPro_Bool) { checkedPro_Bool = value; OnPropertyChanged("checkedPro_Bool"); } }
        }

        //Special
        public Boolean CheckedNaH_Bool
        {
            get { return checkedNaH_Bool; }
            set { if (value != checkedNaH_Bool) { checkedNaH_Bool = value; OnPropertyChanged("checkedNaH_Bool"); } }
        }

        public Boolean CheckedCH3_Bool
        {
            get { return checkedCH3_Bool; }
            set { if (value != checkedCH3_Bool) { checkedCH3_Bool = value; OnPropertyChanged("checkedCH3_Bool"); } }
        }

        public Boolean CheckedSO3_Bool
        {
            get { return checkedSO3_Bool; }
            set { if (value != checkedSO3_Bool) { checkedSO3_Bool = value; OnPropertyChanged("checkedSO3_Bool"); } }
        }

        public Boolean CheckedOAcetyl_Bool
        {
            get { return checkedOAcetyl_Bool; }
            set { if (value != checkedOAcetyl_Bool) { checkedOAcetyl_Bool = value; OnPropertyChanged("checkedOAcetyl_Bool"); } }
        }

        //Permethyl
        public Boolean CheckedpHex_Bool
        {
            get { return checkedpHex_Bool; }
            set { if (value != checkedpHex_Bool) { checkedpHex_Bool = value; OnPropertyChanged("checkedpHex_Bool"); } }
        }

        public Boolean CheckedpHxNAc_Bool
        {
            get { return checkedpHxNAc_Bool; }
            set { if (value != checkedpHxNAc_Bool) { checkedpHxNAc_Bool = value; OnPropertyChanged("checkedpHxNAc_Bool"); } }
        }

        public Boolean CheckedpDxHex_Bool
        {
            get { return checkedpDxHex_Bool; }
            set { if (value != checkedpDxHex_Bool) { checkedpDxHex_Bool = value; OnPropertyChanged("checkedpDxHex_Bool"); } }
        }

        public Boolean CheckedpPntos_Bool
        {
            get { return checkedpPntos_Bool; }
            set { if (value != checkedpPntos_Bool) { checkedpPntos_Bool = value; OnPropertyChanged("checkedpPntos_Bool"); } }
        }

        public Boolean CheckedpNuAc_Bool
        {
            get { return checkedpNuAc_Bool; }
            set { if (value != checkedpNuAc_Bool) { checkedpNuAc_Bool = value; OnPropertyChanged("checkedpNuAc_Bool"); } }
        }

        public Boolean CheckedpNuGc_Bool
        {
            get { return checkedpNuGc_Bool; }
            set { if (value != checkedpNuGc_Bool) { checkedpNuGc_Bool = value; OnPropertyChanged("checkedpNuGc_Bool"); } }
        }

        public Boolean CheckedpKDN_Bool
        {
            get { return checkedpKDN_Bool; }
            set { if (value != checkedpKDN_Bool) { checkedpKDN_Bool = value; OnPropertyChanged("checkedpKDN_Bool"); } }
        }

        public Boolean CheckedpHxA_Bool
        {
            get { return checkedpHxA_Bool; }
            set { if (value != checkedpHxA_Bool) { checkedpHxA_Bool = value; OnPropertyChanged("checkedpHxA_Bool"); } }
        }
    }
}
