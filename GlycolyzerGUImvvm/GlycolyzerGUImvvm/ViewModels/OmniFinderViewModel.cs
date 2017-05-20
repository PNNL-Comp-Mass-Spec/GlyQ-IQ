using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GlycolyzerGUImvvm.Commands;
using GlycolyzerGUImvvm.Models;

namespace GlycolyzerGUImvvm.ViewModels
{
    class OmniFinderViewModel : ObservableObject
    {
        private Models.OmniFinderModel omniFinderModel;

        public Models.OmniFinderModel OmniFinderModel
        {
            get { return omniFinderModel; }
            set
            {
                if (value != omniFinderModel)
                { omniFinderModel = value; App.omniFinderModel_Save = omniFinderModel; this.OnPropertyChanged("omniFinderModel"); }
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

        public OmniFinderViewModel()
        {
            omniFinderModel = new Models.OmniFinderModel();
            OmniFinderModel = App.omniFinderModel_Save;

            ButtonCommand = new RelayCommand(new Action<object>(ButtonAction));
        }

        public static void SelectedOption_Changed()
        {
            switch (App.omniFinderModel_Save.SelectedOption_String)
            {
                case "N Glycans":
                    {
                        App.omniFinderModel_Save.CheckedHexose_Bool = true;
                        App.omniFinderModel_Save.CheckedHexNAc_Bool = true;
                        App.omniFinderModel_Save.CheckedDxyHex_Bool = true;
                        App.omniFinderModel_Save.CheckedNeuAc_Bool = true;
                        App.omniFinderModel_Save.CheckedNaH_Bool = true;
                        break;
                    }
                case "Amino Acids":
                    {
                        App.omniFinderModel_Save.CheckedAla_Bool = true;
                        App.omniFinderModel_Save.CheckedArg_Bool = true;
                        App.omniFinderModel_Save.CheckedAsn_Bool = true;
                        App.omniFinderModel_Save.CheckedAsp_Bool = true;
                        App.omniFinderModel_Save.CheckedCys_Bool = true;
                        App.omniFinderModel_Save.CheckedGln_Bool = true;
                        App.omniFinderModel_Save.CheckedGlu_Bool = true;
                        App.omniFinderModel_Save.CheckedGly_Bool = true;
                        App.omniFinderModel_Save.CheckedHis_Bool = true;
                        App.omniFinderModel_Save.CheckedIle_Bool = true;
                        App.omniFinderModel_Save.CheckedLeu_Bool = true;
                        App.omniFinderModel_Save.CheckedLys_Bool = true;
                        App.omniFinderModel_Save.CheckedMet_Bool = true;
                        App.omniFinderModel_Save.CheckedPhe_Bool = true;
                        App.omniFinderModel_Save.CheckedSer_Bool = true;
                        App.omniFinderModel_Save.CheckedThr_Bool = true;
                        App.omniFinderModel_Save.CheckedTrp_Bool = true;
                        App.omniFinderModel_Save.CheckedTyr_Bool = true;
                        App.omniFinderModel_Save.CheckedVal_Bool = true;
                        App.omniFinderModel_Save.CheckedPro_Bool = true;
                        break;
                    }
                case "No Option Selected":
                    {
                        App.omniFinderModel_Save.CheckedHexose_Bool = false;
                        App.omniFinderModel_Save.CheckedHexNAc_Bool = false;
                        App.omniFinderModel_Save.CheckedDxyHex_Bool = false;
                        App.omniFinderModel_Save.CheckedNeuAc_Bool = false;
                        App.omniFinderModel_Save.CheckedNaH_Bool = false;

                        App.omniFinderModel_Save.CheckedAla_Bool = false;
                        App.omniFinderModel_Save.CheckedArg_Bool = false;
                        App.omniFinderModel_Save.CheckedAsn_Bool = false;
                        App.omniFinderModel_Save.CheckedAsp_Bool = false;
                        App.omniFinderModel_Save.CheckedCys_Bool = false;
                        App.omniFinderModel_Save.CheckedGln_Bool = false;
                        App.omniFinderModel_Save.CheckedGlu_Bool = false;
                        App.omniFinderModel_Save.CheckedGly_Bool = false;
                        App.omniFinderModel_Save.CheckedHis_Bool = false;
                        App.omniFinderModel_Save.CheckedIle_Bool = false;
                        App.omniFinderModel_Save.CheckedLeu_Bool = false;
                        App.omniFinderModel_Save.CheckedLys_Bool = false;
                        App.omniFinderModel_Save.CheckedMet_Bool = false;
                        App.omniFinderModel_Save.CheckedPhe_Bool = false;
                        App.omniFinderModel_Save.CheckedSer_Bool = false;
                        App.omniFinderModel_Save.CheckedThr_Bool = false;
                        App.omniFinderModel_Save.CheckedTrp_Bool = false;
                        App.omniFinderModel_Save.CheckedTyr_Bool = false;
                        App.omniFinderModel_Save.CheckedVal_Bool = false;
                        App.omniFinderModel_Save.CheckedPro_Bool = false;
                        break;
                    }
            }
        }

        public void ButtonAction(object obj)
        {
            switch ((String)obj)
            {
                case "clearAll":
                    {
                        #region Clear All
                        //Select Options
                        OmniFinderModel.SelectedOption_String = "No Option Selected";

                        //Special
                        OmniFinderModel.CheckedNaH_Bool = false;
                        OmniFinderModel.CheckedCH3_Bool = false;
                        OmniFinderModel.CheckedSO3_Bool = false;
                        OmniFinderModel.CheckedOAcetyl_Bool = false;

                        //Sugars
                        OmniFinderModel.CheckedHexose_Bool = false;
                        OmniFinderModel.CheckedHexNAc_Bool = false;
                        OmniFinderModel.CheckedDxyHex_Bool = false;
                        OmniFinderModel.CheckedPentose_Bool = false;
                        OmniFinderModel.CheckedNeuAc_Bool = false;
                        OmniFinderModel.CheckedNeuGc_Bool = false;
                        OmniFinderModel.CheckedKDN_Bool = false;
                        OmniFinderModel.CheckedHexA_Bool = false;

                        //User Number
                        OmniFinderModel.ParameterNumberOfUserUnits_Int = 0;

                        //Amino Acids
                        OmniFinderModel.CheckedAla_Bool = false;
                        OmniFinderModel.CheckedArg_Bool = false;
                        OmniFinderModel.CheckedAsn_Bool = false;
                        OmniFinderModel.CheckedAsp_Bool = false;
                        OmniFinderModel.CheckedCys_Bool = false;
                        OmniFinderModel.CheckedGln_Bool = false;
                        OmniFinderModel.CheckedGlu_Bool = false;
                        OmniFinderModel.CheckedGly_Bool = false;
                        OmniFinderModel.CheckedHis_Bool = false;
                        OmniFinderModel.CheckedIle_Bool = false;
                        OmniFinderModel.CheckedLeu_Bool = false;
                        OmniFinderModel.CheckedLys_Bool = false;
                        OmniFinderModel.CheckedMet_Bool = false;
                        OmniFinderModel.CheckedPhe_Bool = false;
                        OmniFinderModel.CheckedSer_Bool = false;
                        OmniFinderModel.CheckedThr_Bool = false;
                        OmniFinderModel.CheckedTrp_Bool = false;
                        OmniFinderModel.CheckedTyr_Bool = false;
                        OmniFinderModel.CheckedVal_Bool = false;
                        OmniFinderModel.CheckedPro_Bool = false;

                        //Permethyl
                        OmniFinderModel.CheckedpHex_Bool = false;
                        OmniFinderModel.CheckedpHxNAc_Bool = false;
                        OmniFinderModel.CheckedpDxHex_Bool = false;
                        OmniFinderModel.CheckedpPntos_Bool = false;
                        OmniFinderModel.CheckedpNuAc_Bool = false;
                        OmniFinderModel.CheckedpNuGc_Bool = false;
                        OmniFinderModel.CheckedpKDN_Bool = false;
                        OmniFinderModel.CheckedpHxA_Bool = false;
                        #endregion
                        break;
                    }
            }
        }
    }
}
