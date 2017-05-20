using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GlycolyzerGUImvvm.Commands;
using GlycolyzerGUImvvm.Models;

namespace GlycolyzerGUImvvm.ViewModels
{
    public class RangesViewModel : ObservableObject
    {
        private Models.RangesModel rangesModel;

        public Models.RangesModel RangesModel
        {
            get { return rangesModel; }
            set 
            {
                if (value != rangesModel)
                {
                    rangesModel = value;

                    if (App.initializingFlagsModel.ParameterRangesSave_InitializeFlag)
                    {
                        App.parameterRangesModel_Save = rangesModel;
                    }
                    else if (App.initializingFlagsModel.OmniFinderGMRangesSave_InitializeFlag)
                    {
                        App.omniFinderGMRangesModel_Save = rangesModel;
                    }

                    this.OnPropertyChanged("rangesModel");
                }
            }
        }

        private ICommand m_ButtonCommand;
        public ICommand ButtonCommand
        {
            get
            {
                return m_ButtonCommand;
            }
            set
            {
                m_ButtonCommand = value;
            }
        }

        public RangesViewModel()
        {
            rangesModel = new Models.RangesModel();

            if (App.initializingFlagsModel.ParameterRangesSave_InitializeFlag)
            {
                RangesModel = App.parameterRangesModel_Save;
                CheckUserUnits();
            }
            else if (App.initializingFlagsModel.OmniFinderGMRangesSave_InitializeFlag)
            {
                RangesModel = App.omniFinderGMRangesModel_Save;
            }

            ButtonCommand = new RelayCommand(new Action<object>(SetMinMax));
        }

        public void SetMinMax(object obj)
        {
            switch ((String)obj)
            {
                    //Sugars(8)
                case "hexose_Button":
                    {
                        RangesModel.MinHexose_Int = 3;
                        RangesModel.MaxHexose_Int = 12;
                        break;
                    }
                case "hexNAc_Button":
                    {
                        RangesModel.MinHexNAc_Int = 2;
                        RangesModel.MaxHexNAc_Int = 9;
                        break;
                    }
                case "dxyHex_Button":
                    {
                        RangesModel.MinDxyHex_Int = 0;
                        RangesModel.MaxDxyHex_Int = 5;
                        break;
                    }
                case "pentose_Button":
                    {
                        RangesModel.MinPentose_Int = 0;
                        RangesModel.MaxPentose_Int = 1;
                        break;
                    }
                case "neuAc_Button":
                    {
                        RangesModel.MinNeuAc_Int = 0;
                        RangesModel.MaxNeuAc_Int = 5;
                        break;
                    }
                case "neuGc_Button":
                    {
                        RangesModel.MinNeuGc_Int = 0;
                        RangesModel.MaxNeuGc_Int = 4;
                        break;
                    }
                case "kDN_Button":
                    {
                        RangesModel.MinKDN_Int = 0;
                        RangesModel.MaxKDN_Int = 1;
                        break;
                    }
                case "hexA_Button":
                    {
                        RangesModel.MinHexA_Int = 0;
                        RangesModel.MaxHexA_Int = 1;
                        break;
                    }

                    //User Units(10)
                case "userUnit_Button1":
                    {
                        RangesModel.MinUserUnit1_Int = 0;
                        RangesModel.MaxUserUnit1_Int = 1;
                        break;
                    }
                case "userUnit_Button2":
                    {
                        RangesModel.MinUserUnit2_Int = 0;
                        RangesModel.MaxUserUnit2_Int = 1;
                        break;
                    }
                case "userUnit_Button3":
                    {
                        RangesModel.MinUserUnit3_Int = 0;
                        RangesModel.MaxUserUnit3_Int = 1;
                        break;
                    }
                case "userUnit_Button4":
                    {
                        RangesModel.MinUserUnit4_Int = 0;
                        RangesModel.MaxUserUnit4_Int = 1;
                        break;
                    }
                case "userUnit_Button5":
                    {
                        RangesModel.MinUserUnit5_Int = 0;
                        RangesModel.MaxUserUnit5_Int = 1;
                        break;
                    }
                case "userUnit_Button6":
                    {
                        RangesModel.MinUserUnit6_Int = 0;
                        RangesModel.MaxUserUnit6_Int = 1;
                        break;
                    }
                case "userUnit_Button7":
                    {
                        RangesModel.MinUserUnit7_Int = 0;
                        RangesModel.MaxUserUnit7_Int = 1;
                        break;
                    }
                case "userUnit_Button8":
                    {
                        RangesModel.MinUserUnit8_Int = 0;
                        RangesModel.MaxUserUnit8_Int = 1;
                        break;
                    }
                case "userUnit_Button9":
                    {
                        RangesModel.MinUserUnit9_Int = 0;
                        RangesModel.MaxUserUnit9_Int = 1;
                        break;
                    }
                case "userUnit_Button10":
                    {
                        RangesModel.MinUserUnit10_Int = 0;
                        RangesModel.MaxUserUnit10_Int = 1;
                        break;
                    }

                    //Specials(4)
                case "naH_Button":
                    {
                        RangesModel.MinNaH_Int = 0;
                        RangesModel.MaxNaH_Int = 4;
                        break;
                    }
                case "cH3_Button":
                    {
                        RangesModel.MinCH3_Int = 0;
                        RangesModel.MaxCH3_Int = 4;
                        break;
                    }
                case "sO3_Button":
                    {
                        RangesModel.MinSO3_Int = 0;
                        RangesModel.MaxSO3_Int = 1;
                        break;
                    }
                case "oAcetyl_Button":
                    {
                        RangesModel.MinOAcetyl_Int = 0;
                        RangesModel.MaxOAcetyl_Int = 1;
                        break;
                    }

                    //Amino Acids(20)
                case "ala_Button":
                    {
                        RangesModel.MinAla_Int = 0;
                        RangesModel.MaxAla_Int = 1;
                        break;
                    }
                case "arg_Button":
                    {
                        RangesModel.MinArg_Int = 0;
                        RangesModel.MaxArg_Int = 1;
                        break;
                    }
                case "asn_Button":
                    {
                        RangesModel.MinAsn_Int = 0;
                        RangesModel.MaxAsn_Int = 1;
                        break;
                    }
                case "asp_Button":
                    {
                        RangesModel.MinAsp_Int = 0;
                        RangesModel.MaxAsp_Int = 1;
                        break;
                    }
                case "cys_Button":
                    {
                        RangesModel.MinCys_Int = 0;
                        RangesModel.MaxCys_Int = 1;
                        break;
                    }
                case "gln_Button":
                    {
                        RangesModel.MinGln_Int = 0;
                        RangesModel.MaxGln_Int = 1;
                        break;
                    }
                case "glu_Button":
                    {
                        RangesModel.MinGlu_Int = 0;
                        RangesModel.MaxGlu_Int = 1;
                        break;
                    }
                case "gly_Button":
                    {
                        RangesModel.MinGly_Int = 0;
                        RangesModel.MaxGly_Int = 1;
                        break;
                    }
                case "his_Button":
                    {
                        RangesModel.MinHis_Int = 0;
                        RangesModel.MaxHis_Int = 1;
                        break;
                    }
                case "ile_Button":
                    {
                        RangesModel.MinIle_Int = 0;
                        RangesModel.MaxIle_Int = 1;
                        break;
                    }
                case "leu_Button":
                    {
                        RangesModel.MinLeu_Int = 0;
                        RangesModel.MaxLeu_Int = 1;
                        break;
                    }
                case "lys_Button":
                    {
                        RangesModel.MinLys_Int = 0;
                        RangesModel.MaxLys_Int = 1;
                        break;
                    }
                case "met_Button":
                    {
                        RangesModel.MinMet_Int = 0;
                        RangesModel.MaxMet_Int = 1;
                        break;
                    }
                case "phe_Button":
                    {
                        RangesModel.MinPhe_Int = 0;
                        RangesModel.MaxPhe_Int = 1;
                        break;
                    }
                case "ser_Button":
                    {
                        RangesModel.MinSer_Int = 0;
                        RangesModel.MaxSer_Int = 1;
                        break;
                    }
                case "thr_Button":
                    {
                        RangesModel.MinThr_Int = 0;
                        RangesModel.MaxThr_Int = 1;
                        break;
                    }
                case "trp_Button":
                    {
                        RangesModel.MinTrp_Int = 0;
                        RangesModel.MaxTrp_Int = 1;
                        break;
                    }
                case "tyr_Button":
                    {
                        RangesModel.MinTyr_Int = 0;
                        RangesModel.MaxTyr_Int = 1;
                        break;
                    }
                case "val_Button":
                    {
                        RangesModel.MinVal_Int = 0;
                        RangesModel.MaxVal_Int = 1;
                        break;
                    }
                case "pro_Button":
                    {
                        RangesModel.MinPro_Int = 0;
                        RangesModel.MaxPro_Int = 1;
                        break;
                    }

                    //Permethyl(8)
                case "pHex_Button":
                    {
                        RangesModel.MinpHex_Int = 3;
                        RangesModel.MaxpHex_Int = 12;
                        break;
                    }
                case "pHxNAc_Button":
                    {
                        RangesModel.MinpHxNAc_Int = 2;
                        RangesModel.MaxpHxNAc_Int = 9;
                        break;
                    }
                case "pDxHex_Button":
                    {
                        RangesModel.MinpDxHex_Int = 0;
                        RangesModel.MaxpDxHex_Int = 5;
                        break;
                    }
                case "pPntos_Button":
                    {
                        RangesModel.MinpPntos_Int = 0;
                        RangesModel.MaxpPntos_Int = 1;
                        break;
                    }
                case "pNuAc_Button":
                    {
                        RangesModel.MinpNuAc_Int = 0;
                        RangesModel.MaxpNuAc_Int = 4;
                        break;
                    }
                case "pNuGc_Button":
                    {
                        RangesModel.MinpNuGc_Int = 0;
                        RangesModel.MaxpNuGc_Int = 4;
                        break;
                    }
                case "pKDN_Button":
                    {
                        RangesModel.MinpKDN_Int = 0;
                        RangesModel.MaxpKDN_Int = 1;
                        break;
                    }
                case "pHxA_Button":
                    {
                        RangesModel.MinpHxA_Int = 0;
                        RangesModel.MaxpHxA_Int = 1;
                        break;
                    }
            }
        }

        private void CheckUserUnits()
        {
            switch (App.omniFinderModel_Save.ParameterNumberOfUserUnits_Int)
            {
                case 1:
                    {
                        App.omniFinderModel_Save.CheckedUserUnit1_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit2_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit3_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit4_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit5_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit6_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit7_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit8_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit9_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit10_Bool = false;
                        break;
                    }
                case 2:
                    {
                        App.omniFinderModel_Save.CheckedUserUnit1_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit2_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit3_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit4_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit5_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit6_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit7_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit8_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit9_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit10_Bool = false;
                        break;
                    }
                case 3:
                    {
                        App.omniFinderModel_Save.CheckedUserUnit1_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit2_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit3_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit4_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit5_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit6_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit7_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit8_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit9_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit10_Bool = false;
                        break;
                    }
                case 4:
                    {
                        App.omniFinderModel_Save.CheckedUserUnit1_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit2_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit3_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit4_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit5_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit6_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit7_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit8_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit9_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit10_Bool = false;
                        break;
                    }
                case 5:
                    {
                        App.omniFinderModel_Save.CheckedUserUnit1_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit2_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit3_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit4_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit5_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit6_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit7_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit8_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit9_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit10_Bool = false;
                        break;
                    }
                case 6:
                    {
                        App.omniFinderModel_Save.CheckedUserUnit1_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit2_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit3_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit4_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit5_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit6_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit7_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit8_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit9_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit10_Bool = false;
                        break;
                    }
                case 7:
                    {
                        App.omniFinderModel_Save.CheckedUserUnit1_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit2_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit3_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit4_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit5_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit6_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit7_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit8_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit9_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit10_Bool = false;
                        break;
                    }
                case 8:
                    {
                        App.omniFinderModel_Save.CheckedUserUnit1_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit2_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit3_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit4_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit5_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit6_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit7_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit8_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit9_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit10_Bool = false;
                        break;
                    }
                case 9:
                    {
                        App.omniFinderModel_Save.CheckedUserUnit1_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit2_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit3_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit4_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit5_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit6_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit7_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit8_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit9_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit10_Bool = false;
                        break;
                    }
                case 10:
                    {
                        App.omniFinderModel_Save.CheckedUserUnit1_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit2_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit3_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit4_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit5_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit6_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit7_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit8_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit9_Bool = true;
                        App.omniFinderModel_Save.CheckedUserUnit10_Bool = true;
                        break;
                    }
                default:
                    {
                        App.omniFinderModel_Save.CheckedUserUnit1_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit2_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit3_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit4_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit5_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit6_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit7_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit8_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit9_Bool = false;
                        App.omniFinderModel_Save.CheckedUserUnit10_Bool = false;
                        break;
                    }
            }
        }
    }
}