using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlycolyzerGUImvvm.Models
{
    public class RangesModel : ObservableObject
    {
        #region Min
        //Min
        //Sugars
        private int minHexose_Int = 0;
        private int minHexNAc_Int = 0;
        private int minDxyHex_Int = 0;
        private int minPentose_Int = 0;
        private int minNeuAc_Int = 0;
        private int minNeuGc_Int = 0;
        private int minKDN_Int = 0;
        private int minHexA_Int = 0;

        //User Number                                  
        private int minUserUnit1_Int = 0;
        private int minUserUnit2_Int = 0;
        private int minUserUnit3_Int = 0;
        private int minUserUnit4_Int = 0;
        private int minUserUnit5_Int = 0;
        private int minUserUnit6_Int = 0;
        private int minUserUnit7_Int = 0;
        private int minUserUnit8_Int = 0;
        private int minUserUnit9_Int = 0;
        private int minUserUnit10_Int = 0;

        //Amino Acids                                  
        private int minAla_Int = 0;
        private int minArg_Int = 0;
        private int minAsn_Int = 0;
        private int minAsp_Int = 0;
        private int minCys_Int = 0;
        private int minGln_Int = 0;
        private int minGlu_Int = 0;
        private int minGly_Int = 0;
        private int minHis_Int = 0;
        private int minIle_Int = 0;
        private int minLeu_Int = 0;
        private int minLys_Int = 0;
        private int minMet_Int = 0;
        private int minPhe_Int = 0;
        private int minSer_Int = 0;
        private int minThr_Int = 0;
        private int minTrp_Int = 0;
        private int minTyr_Int = 0;
        private int minVal_Int = 0;
        private int minPro_Int = 0;

        //Special                                      
        private int minNaH_Int = 0;
        private int minCH3_Int = 0;
        private int minSO3_Int = 0;
        private int minOAcetyl_Int = 0;

        //Permethyl                                    
        private int minpHex_Int = 0;
        private int minpHxNAc_Int = 0;
        private int minpDxHex_Int = 0;
        private int minpPntos_Int = 0;
        private int minpNuAc_Int = 0;
        private int minpNuGc_Int = 0;
        private int minpKDN_Int = 0;
        private int minpHxA_Int = 0;
        #endregion

        #region Max
        //Max
        //Sugars
        private int maxHexose_Int = 0;
        private int maxHexNAc_Int = 0;
        private int maxDxyHex_Int = 0;
        private int maxPentose_Int = 0;
        private int maxNeuAc_Int = 0;
        private int maxNeuGc_Int = 0;
        private int maxKDN_Int = 0;
        private int maxHexA_Int = 0;

        //User Number
        private int maxUserUnit1_Int = 0;
        private int maxUserUnit2_Int = 0;
        private int maxUserUnit3_Int = 0;
        private int maxUserUnit4_Int = 0;
        private int maxUserUnit5_Int = 0;
        private int maxUserUnit6_Int = 0;
        private int maxUserUnit7_Int = 0;
        private int maxUserUnit8_Int = 0;
        private int maxUserUnit9_Int = 0;
        private int maxUserUnit10_Int = 0;

        //Amino Acids
        private int maxAla_Int = 0;
        private int maxArg_Int = 0;
        private int maxAsn_Int = 0;
        private int maxAsp_Int = 0;
        private int maxCys_Int = 0;
        private int maxGln_Int = 0;
        private int maxGlu_Int = 0;
        private int maxGly_Int = 0;
        private int maxHis_Int = 0;
        private int maxIle_Int = 0;
        private int maxLeu_Int = 0;
        private int maxLys_Int = 0;
        private int maxMet_Int = 0;
        private int maxPhe_Int = 0;
        private int maxSer_Int = 0;
        private int maxThr_Int = 0;
        private int maxTrp_Int = 0;
        private int maxTyr_Int = 0;
        private int maxVal_Int = 0;
        private int maxPro_Int = 0;

        //Special        
        private int maxNaH_Int = 0;
        private int maxCH3_Int = 0;
        private int maxSO3_Int = 0;
        private int maxOAcetyl_Int = 0;

        //Permethyl  
        private int maxpHex_Int = 0;
        private int maxpHxNAc_Int = 0;
        private int maxpDxHex_Int = 0;
        private int maxpPntos_Int = 0;
        private int maxpNuAc_Int = 0;
        private int maxpNuGc_Int = 0;
        private int maxpKDN_Int = 0;
        private int maxpHxA_Int = 0;
        #endregion

        #region User Unit Masses
        //User Unit Masses
        private double massUserUnit1_Double = 0.0;
        private double massUserUnit2_Double = 0.0;
        private double massUserUnit3_Double = 0.0;
        private double massUserUnit4_Double = 0.0;
        private double massUserUnit5_Double = 0.0;
        private double massUserUnit6_Double = 0.0;
        private double massUserUnit7_Double = 0.0;
        private double massUserUnit8_Double = 0.0;
        private double massUserUnit9_Double = 0.0;
        private double massUserUnit10_Double = 0.0;
        #endregion


        #region Min Methods
        //Min
        //Sugars
        public int MinHexose_Int
        {
            get { return minHexose_Int; }
            set { if (value != minHexose_Int) { minHexose_Int = value; OnPropertyChanged("minHexose_Int"); } }
        }
        public int MinHexNAc_Int
        {
            get { return minHexNAc_Int; }
            set { if (value != minHexNAc_Int) { minHexNAc_Int = value; OnPropertyChanged("minHexNAc_Int"); } }
        }
        public int MinDxyHex_Int
        {
            get { return minDxyHex_Int; }
            set { if (value != minDxyHex_Int) { minDxyHex_Int = value; OnPropertyChanged("minDxyHex_Int"); } }
        }
        public int MinPentose_Int
        {
            get { return minPentose_Int; }
            set { if (value != minPentose_Int) { minPentose_Int = value; OnPropertyChanged("minPentose_Int"); } }
        }
        public int MinNeuAc_Int
        {
            get { return minNeuAc_Int; }
            set { if (value != minNeuAc_Int) { minNeuAc_Int = value; OnPropertyChanged("minNeuAc_Int"); } }
        }
        public int MinNeuGc_Int
        {
            get { return minNeuGc_Int; }
            set { if (value != minNeuGc_Int) { minNeuGc_Int = value; OnPropertyChanged("minNeuGc_Int"); } }
        }
        public int MinKDN_Int
        {
            get { return minKDN_Int; }
            set { if (value != minKDN_Int) { minKDN_Int = value; OnPropertyChanged("minKDN_Int"); } }
        }
        public int MinHexA_Int
        {
            get { return minHexA_Int; }
            set { if (value != minHexA_Int) { minHexA_Int = value; OnPropertyChanged("minHexA_Int"); } }
        }

        //User Number
        public int MinUserUnit1_Int
        {
            get { return minUserUnit1_Int; }
            set { if (value != minUserUnit1_Int) { minUserUnit1_Int = value; OnPropertyChanged("minUserUnit1_Int"); } }
        }
        public int MinUserUnit2_Int
        {
            get { return minUserUnit2_Int; }
            set { if (value != minUserUnit2_Int) { minUserUnit2_Int = value; OnPropertyChanged("minUserUnit2_Int"); } }
        }
        public int MinUserUnit3_Int
        {
            get { return minUserUnit3_Int; }
            set { if (value != minUserUnit3_Int) { minUserUnit3_Int = value; OnPropertyChanged("minUserUnit3_Int"); } }
        }
        public int MinUserUnit4_Int
        {
            get { return minUserUnit4_Int; }
            set { if (value != minUserUnit4_Int) { minUserUnit4_Int = value; OnPropertyChanged("minUserUnit4_Int"); } }
        }
        public int MinUserUnit5_Int
        {
            get { return minUserUnit5_Int; }
            set { if (value != minUserUnit5_Int) { minUserUnit5_Int = value; OnPropertyChanged("minUserUnit5_Int"); } }
        }
        public int MinUserUnit6_Int
        {
            get { return minUserUnit6_Int; }
            set { if (value != minUserUnit6_Int) { minUserUnit6_Int = value; OnPropertyChanged("minUserUnit6_Int"); } }
        }
        public int MinUserUnit7_Int
        {
            get { return minUserUnit7_Int; }
            set { if (value != minUserUnit7_Int) { minUserUnit7_Int = value; OnPropertyChanged("minUserUnit7_Int"); } }
        }
        public int MinUserUnit8_Int
        {
            get { return minUserUnit8_Int; }
            set { if (value != minUserUnit8_Int) { minUserUnit8_Int = value; OnPropertyChanged("minUserUnit8_Int"); } }
        }
        public int MinUserUnit9_Int
        {
            get { return minUserUnit9_Int; }
            set { if (value != minUserUnit9_Int) { minUserUnit9_Int = value; OnPropertyChanged("minUserUnit9_Int"); } }
        }
        public int MinUserUnit10_Int
        {
            get { return minUserUnit10_Int; }
            set { if (value != minUserUnit10_Int) { minUserUnit10_Int = value; OnPropertyChanged("minUserUnit10_Int"); } }
        }

        //Amino Acids
        public int MinAla_Int
        {
            get { return minAla_Int; }
            set { if (value != minAla_Int) { minAla_Int = value; OnPropertyChanged("minAla_Int"); } }
        }
        public int MinArg_Int
        {
            get { return minArg_Int; }
            set { if (value != minArg_Int) { minArg_Int = value; OnPropertyChanged("minArg_Int"); } }
        }
        public int MinAsn_Int
        {
            get { return minAsn_Int; }
            set { if (value != minAsn_Int) { minAsn_Int = value; OnPropertyChanged("minAsn_Int"); } }
        }
        public int MinAsp_Int
        {
            get { return minAsp_Int; }
            set { if (value != minAsp_Int) { minAsp_Int = value; OnPropertyChanged("minAsp_Int"); } }
        }
        public int MinCys_Int
        {
            get { return minCys_Int; }
            set { if (value != minCys_Int) { minCys_Int = value; OnPropertyChanged("minCys_Int"); } }
        }
        public int MinGln_Int
        {
            get { return minGln_Int; }
            set { if (value != minGln_Int) { minGln_Int = value; OnPropertyChanged("minGln_Int"); } }
        }
        public int MinGlu_Int
        {
            get { return minGlu_Int; }
            set { if (value != minGlu_Int) { minGlu_Int = value; OnPropertyChanged("minGlu_Int"); } }
        }
        public int MinGly_Int
        {
            get { return minGly_Int; }
            set { if (value != minGly_Int) { minGly_Int = value; OnPropertyChanged("minGly_Int"); } }
        }
        public int MinHis_Int
        {
            get { return minHis_Int; }
            set { if (value != minHis_Int) { minHis_Int = value; OnPropertyChanged("minHis_Int"); } }
        }
        public int MinIle_Int
        {
            get { return minIle_Int; }
            set { if (value != minIle_Int) { minIle_Int = value; OnPropertyChanged("minIle_Int"); } }
        }
        public int MinLeu_Int
        {
            get { return minLeu_Int; }
            set { if (value != minLeu_Int) { minLeu_Int = value; OnPropertyChanged("minLeu_Int"); } }
        }
        public int MinLys_Int
        {
            get { return minLys_Int; }
            set { if (value != minLys_Int) { minLys_Int = value; OnPropertyChanged("minLys_Int"); } }
        }
        public int MinMet_Int
        {
            get { return minMet_Int; }
            set { if (value != minMet_Int) { minMet_Int = value; OnPropertyChanged("minMet_Int"); } }
        }
        public int MinPhe_Int
        {
            get { return minPhe_Int; }
            set { if (value != minPhe_Int) { minPhe_Int = value; OnPropertyChanged("minPhe_Int"); } }
        }
        public int MinSer_Int
        {
            get { return minSer_Int; }
            set { if (value != minSer_Int) { minSer_Int = value; OnPropertyChanged("minSer_Int"); } }
        }
        public int MinThr_Int
        {
            get { return minThr_Int; }
            set { if (value != minThr_Int) { minThr_Int = value; OnPropertyChanged("minThr_Int"); } }
        }
        public int MinTrp_Int
        {
            get { return minTrp_Int; }
            set { if (value != minTrp_Int) { minTrp_Int = value; OnPropertyChanged("minTrp_Int"); } }
        }
        public int MinTyr_Int
        {
            get { return minTyr_Int; }
            set { if (value != minTyr_Int) { minTyr_Int = value; OnPropertyChanged("minTyr_Int"); } }
        }
        public int MinVal_Int
        {
            get { return minVal_Int; }
            set { if (value != minVal_Int) { minVal_Int = value; OnPropertyChanged("minVal_Int"); } }
        }
        public int MinPro_Int
        {
            get { return minPro_Int; }
            set { if (value != minPro_Int) { minPro_Int = value; OnPropertyChanged("minPro_Int"); } }
        }

        //Special
        public int MinNaH_Int
        {
            get { return minNaH_Int; }
            set { if (value != minNaH_Int) { minNaH_Int = value; OnPropertyChanged("minNaH_Int"); } }
        }
        public int MinCH3_Int
        {
            get { return minCH3_Int; }
            set { if (value != minCH3_Int) { minCH3_Int = value; OnPropertyChanged("minCH3_Int"); } }
        }
        public int MinSO3_Int
        {
            get { return minSO3_Int; }
            set { if (value != minSO3_Int) { minSO3_Int = value; OnPropertyChanged("minSO3_Int"); } }
        }
        public int MinOAcetyl_Int
        {
            get { return minOAcetyl_Int; }
            set { if (value != minOAcetyl_Int) { minOAcetyl_Int = value; OnPropertyChanged("minOAcetyl_Int"); } }
        }

        //Permethyl  
        public int MinpHex_Int
        {
            get { return minpHex_Int; }
            set { if (value != minpHex_Int) { minpHex_Int = value; OnPropertyChanged("minpHex_Int"); } }
        }
        public int MinpHxNAc_Int
        {
            get { return minpHxNAc_Int; }
            set { if (value != minpHxNAc_Int) { minpHxNAc_Int = value; OnPropertyChanged("minpHxNAc_Int"); } }
        }
        public int MinpDxHex_Int
        {
            get { return minpDxHex_Int; }
            set { if (value != minpDxHex_Int) { minpDxHex_Int = value; OnPropertyChanged("minpDxHex_Int"); } }
        }
        public int MinpPntos_Int
        {
            get { return minpPntos_Int; }
            set { if (value != minpPntos_Int) { minpPntos_Int = value; OnPropertyChanged("minpPntos_Int"); } }
        }
        public int MinpNuAc_Int
        {
            get { return minpNuAc_Int; }
            set { if (value != minpNuAc_Int) { minpNuAc_Int = value; OnPropertyChanged("minpNuAc_Int"); } }
        }
        public int MinpNuGc_Int
        {
            get { return minpNuGc_Int; }
            set { if (value != minpNuGc_Int) { minpNuGc_Int = value; OnPropertyChanged("minpNuGc_Int"); } }
        }
        public int MinpKDN_Int
        {
            get { return minpKDN_Int; }
            set { if (value != minpKDN_Int) { minpKDN_Int = value; OnPropertyChanged("minpKDN_Int"); } }
        }
        public int MinpHxA_Int
        {
            get { return minpHxA_Int; }
            set { if (value != minpHxA_Int) { minpHxA_Int = value; OnPropertyChanged("minpHxA_Int"); } }
        }
        #endregion

        #region Max Methods
        //Max
        //Sugars
        public int MaxHexose_Int
        {
            get { return maxHexose_Int; }
            set { if (value != maxHexose_Int) { maxHexose_Int = value; OnPropertyChanged("maxHexose_Int"); } }
        }
        public int MaxHexNAc_Int
        {
            get { return maxHexNAc_Int; }
            set { if (value != maxHexNAc_Int) { maxHexNAc_Int = value; OnPropertyChanged("maxHexNAc_Int"); } }
        }
        public int MaxDxyHex_Int
        {
            get { return maxDxyHex_Int; }
            set { if (value != maxDxyHex_Int) { maxDxyHex_Int = value; OnPropertyChanged("maxDxyHex_Int"); } }
        }
        public int MaxPentose_Int
        {
            get { return maxPentose_Int; }
            set { if (value != maxPentose_Int) { maxPentose_Int = value; OnPropertyChanged("maxPentose_Int"); } }
        }
        public int MaxNeuAc_Int
        {
            get { return maxNeuAc_Int; }
            set { if (value != maxNeuAc_Int) { maxNeuAc_Int = value; OnPropertyChanged("maxNeuAc_Int"); } }
        }
        public int MaxNeuGc_Int
        {
            get { return maxNeuGc_Int; }
            set { if (value != maxNeuGc_Int) { maxNeuGc_Int = value; OnPropertyChanged("maxNeuGc_Int"); } }
        }
        public int MaxKDN_Int
        {
            get { return maxKDN_Int; }
            set { if (value != maxKDN_Int) { maxKDN_Int = value; OnPropertyChanged("maxKDN_Int"); } }
        }
        public int MaxHexA_Int
        {
            get { return maxHexA_Int; }
            set { if (value != maxHexA_Int) { maxHexA_Int = value; OnPropertyChanged("maxHexA_Int"); } }
        }

        //User Number
        public int MaxUserUnit1_Int
        {
            get { return maxUserUnit1_Int; }
            set { if (value != maxUserUnit1_Int) { maxUserUnit1_Int = value; OnPropertyChanged("maxUserUnit1_Int"); } }
        }
        public int MaxUserUnit2_Int
        {
            get { return maxUserUnit2_Int; }
            set { if (value != maxUserUnit2_Int) { maxUserUnit2_Int = value; OnPropertyChanged("maxUserUnit2_Int"); } }
        }
        public int MaxUserUnit3_Int
        {
            get { return maxUserUnit3_Int; }
            set { if (value != maxUserUnit3_Int) { maxUserUnit3_Int = value; OnPropertyChanged("maxUserUnit3_Int"); } }
        }
        public int MaxUserUnit4_Int
        {
            get { return maxUserUnit4_Int; }
            set { if (value != maxUserUnit4_Int) { maxUserUnit4_Int = value; OnPropertyChanged("maxUserUnit4_Int"); } }
        }
        public int MaxUserUnit5_Int
        {
            get { return maxUserUnit5_Int; }
            set { if (value != maxUserUnit5_Int) { maxUserUnit5_Int = value; OnPropertyChanged("maxUserUnit5_Int"); } }
        }
        public int MaxUserUnit6_Int
        {
            get { return maxUserUnit6_Int; }
            set { if (value != maxUserUnit6_Int) { maxUserUnit6_Int = value; OnPropertyChanged("maxUserUnit6_Int"); } }
        }
        public int MaxUserUnit7_Int
        {
            get { return maxUserUnit7_Int; }
            set { if (value != maxUserUnit7_Int) { maxUserUnit7_Int = value; OnPropertyChanged("maxUserUnit7_Int"); } }
        }
        public int MaxUserUnit8_Int
        {
            get { return maxUserUnit8_Int; }
            set { if (value != maxUserUnit8_Int) { maxUserUnit8_Int = value; OnPropertyChanged("maxUserUnit8_Int"); } }
        }
        public int MaxUserUnit9_Int
        {
            get { return maxUserUnit9_Int; }
            set { if (value != maxUserUnit9_Int) { maxUserUnit9_Int = value; OnPropertyChanged("maxUserUnit9_Int"); } }
        }
        public int MaxUserUnit10_Int
        {
            get { return maxUserUnit10_Int; }
            set { if (value != maxUserUnit10_Int) { maxUserUnit10_Int = value; OnPropertyChanged("maxUserUnit10_Int"); } }
        }

        //Amaxo Acids
        public int MaxAla_Int
        {
            get { return maxAla_Int; }
            set { if (value != maxAla_Int) { maxAla_Int = value; OnPropertyChanged("maxAla_Int"); } }
        }
        public int MaxArg_Int
        {
            get { return maxArg_Int; }
            set { if (value != maxArg_Int) { maxArg_Int = value; OnPropertyChanged("maxArg_Int"); } }
        }
        public int MaxAsn_Int
        {
            get { return maxAsn_Int; }
            set { if (value != maxAsn_Int) { maxAsn_Int = value; OnPropertyChanged("maxAsn_Int"); } }
        }
        public int MaxAsp_Int
        {
            get { return maxAsp_Int; }
            set { if (value != maxAsp_Int) { maxAsp_Int = value; OnPropertyChanged("maxAsp_Int"); } }
        }
        public int MaxCys_Int
        {
            get { return maxCys_Int; }
            set { if (value != maxCys_Int) { maxCys_Int = value; OnPropertyChanged("maxCys_Int"); } }
        }
        public int MaxGln_Int
        {
            get { return maxGln_Int; }
            set { if (value != maxGln_Int) { maxGln_Int = value; OnPropertyChanged("maxGln_Int"); } }
        }
        public int MaxGlu_Int
        {
            get { return maxGlu_Int; }
            set { if (value != maxGlu_Int) { maxGlu_Int = value; OnPropertyChanged("maxGlu_Int"); } }
        }
        public int MaxGly_Int
        {
            get { return maxGly_Int; }
            set { if (value != maxGly_Int) { maxGly_Int = value; OnPropertyChanged("maxGly_Int"); } }
        }
        public int MaxHis_Int
        {
            get { return maxHis_Int; }
            set { if (value != maxHis_Int) { maxHis_Int = value; OnPropertyChanged("maxHis_Int"); } }
        }
        public int MaxIle_Int
        {
            get { return maxIle_Int; }
            set { if (value != maxIle_Int) { maxIle_Int = value; OnPropertyChanged("maxIle_Int"); } }
        }
        public int MaxLeu_Int
        {
            get { return maxLeu_Int; }
            set { if (value != maxLeu_Int) { maxLeu_Int = value; OnPropertyChanged("maxLeu_Int"); } }
        }
        public int MaxLys_Int
        {
            get { return maxLys_Int; }
            set { if (value != maxLys_Int) { maxLys_Int = value; OnPropertyChanged("maxLys_Int"); } }
        }
        public int MaxMet_Int
        {
            get { return maxMet_Int; }
            set { if (value != maxMet_Int) { maxMet_Int = value; OnPropertyChanged("maxMet_Int"); } }
        }
        public int MaxPhe_Int
        {
            get { return maxPhe_Int; }
            set { if (value != maxPhe_Int) { maxPhe_Int = value; OnPropertyChanged("maxPhe_Int"); } }
        }
        public int MaxSer_Int
        {
            get { return maxSer_Int; }
            set { if (value != maxSer_Int) { maxSer_Int = value; OnPropertyChanged("maxSer_Int"); } }
        }
        public int MaxThr_Int
        {
            get { return maxThr_Int; }
            set { if (value != maxThr_Int) { maxThr_Int = value; OnPropertyChanged("maxThr_Int"); } }
        }
        public int MaxTrp_Int
        {
            get { return maxTrp_Int; }
            set { if (value != maxTrp_Int) { maxTrp_Int = value; OnPropertyChanged("maxTrp_Int"); } }
        }
        public int MaxTyr_Int
        {
            get { return maxTyr_Int; }
            set { if (value != maxTyr_Int) { maxTyr_Int = value; OnPropertyChanged("maxTyr_Int"); } }
        }
        public int MaxVal_Int
        {
            get { return maxVal_Int; }
            set { if (value != maxVal_Int) { maxVal_Int = value; OnPropertyChanged("maxVal_Int"); } }
        }
        public int MaxPro_Int
        {
            get { return maxPro_Int; }
            set { if (value != maxPro_Int) { maxPro_Int = value; OnPropertyChanged("maxPro_Int"); } }
        }

        //Special
        public int MaxNaH_Int
        {
            get { return maxNaH_Int; }
            set { if (value != maxNaH_Int) { maxNaH_Int = value; OnPropertyChanged("maxNaH_Int"); } }
        }
        public int MaxCH3_Int
        {
            get { return maxCH3_Int; }
            set { if (value != maxCH3_Int) { maxCH3_Int = value; OnPropertyChanged("maxCH3_Int"); } }
        }
        public int MaxSO3_Int
        {
            get { return maxSO3_Int; }
            set { if (value != maxSO3_Int) { maxSO3_Int = value; OnPropertyChanged("maxSO3_Int"); } }
        }
        public int MaxOAcetyl_Int
        {
            get { return maxOAcetyl_Int; }
            set { if (value != maxOAcetyl_Int) { maxOAcetyl_Int = value; OnPropertyChanged("maxOAcetyl_Int"); } }
        }

        //Permethyl  
        public int MaxpHex_Int
        {
            get { return maxpHex_Int; }
            set { if (value != maxpHex_Int) { maxpHex_Int = value; OnPropertyChanged("maxpHex_Int"); } }
        }
        public int MaxpHxNAc_Int
        {
            get { return maxpHxNAc_Int; }
            set { if (value != maxpHxNAc_Int) { maxpHxNAc_Int = value; OnPropertyChanged("maxpHxNAc_Int"); } }
        }
        public int MaxpDxHex_Int
        {
            get { return maxpDxHex_Int; }
            set { if (value != maxpDxHex_Int) { maxpDxHex_Int = value; OnPropertyChanged("maxpDxHex_Int"); } }
        }
        public int MaxpPntos_Int
        {
            get { return maxpPntos_Int; }
            set { if (value != maxpPntos_Int) { maxpPntos_Int = value; OnPropertyChanged("maxpPntos_Int"); } }
        }
        public int MaxpNuAc_Int
        {
            get { return maxpNuAc_Int; }
            set { if (value != maxpNuAc_Int) { maxpNuAc_Int = value; OnPropertyChanged("maxpNuAc_Int"); } }
        }
        public int MaxpNuGc_Int
        {
            get { return maxpNuGc_Int; }
            set { if (value != maxpNuGc_Int) { maxpNuGc_Int = value; OnPropertyChanged("maxpNuGc_Int"); } }
        }
        public int MaxpKDN_Int
        {
            get { return maxpKDN_Int; }
            set { if (value != maxpKDN_Int) { maxpKDN_Int = value; OnPropertyChanged("maxpKDN_Int"); } }
        }
        public int MaxpHxA_Int
        {
            get { return maxpHxA_Int; }
            set { if (value != maxpHxA_Int) { maxpHxA_Int = value; OnPropertyChanged("maxpHxA_Int"); } }
        }
        #endregion

        #region User Unit Mass Methods
        //User Unit Mass Methods
        public double MassUserUnit1_Double
        {
            get { return massUserUnit1_Double; }
            set { if (value != massUserUnit1_Double) { massUserUnit1_Double = value; OnPropertyChanged("massUserUnit1_Double"); } }
        }
        public double MassUserUnit2_Double
        {
            get { return massUserUnit2_Double; }
            set { if (value != massUserUnit2_Double) { massUserUnit2_Double = value; OnPropertyChanged("massUserUnit2_Double"); } }
        }
        public double MassUserUnit3_Double
        {
            get { return massUserUnit3_Double; }
            set { if (value != massUserUnit3_Double) { massUserUnit3_Double = value; OnPropertyChanged("massUserUnit3_Double"); } }
        }
        public double MassUserUnit4_Double
        {
            get { return massUserUnit4_Double; }
            set { if (value != massUserUnit4_Double) { massUserUnit4_Double = value; OnPropertyChanged("massUserUnit4_Double"); } }
        }
        public double MassUserUnit5_Double
        {
            get { return massUserUnit5_Double; }
            set { if (value != massUserUnit5_Double) { massUserUnit5_Double = value; OnPropertyChanged("massUserUnit5_Double"); } }
        }
        public double MassUserUnit6_Double
        {
            get { return massUserUnit6_Double; }
            set { if (value != massUserUnit6_Double) { massUserUnit6_Double = value; OnPropertyChanged("massUserUnit6_Double"); } }
        }
        public double MassUserUnit7_Double
        {
            get { return massUserUnit7_Double; }
            set { if (value != massUserUnit7_Double) { massUserUnit7_Double = value; OnPropertyChanged("massUserUnit7_Double"); } }
        }
        public double MassUserUnit8_Double
        {
            get { return massUserUnit8_Double; }
            set { if (value != massUserUnit8_Double) { massUserUnit8_Double = value; OnPropertyChanged("massUserUnit8_Double"); } }
        }
        public double MassUserUnit9_Double
        {
            get { return massUserUnit9_Double; }
            set { if (value != massUserUnit9_Double) { massUserUnit9_Double = value; OnPropertyChanged("massUserUnit9_Double"); } }
        }
        public double MassUserUnit10_Double
        {
            get { return massUserUnit10_Double; }
            set { if (value != massUserUnit10_Double) { massUserUnit10_Double = value; OnPropertyChanged("massUserUnit10_Double"); } }
        }
        #endregion
    }
}
