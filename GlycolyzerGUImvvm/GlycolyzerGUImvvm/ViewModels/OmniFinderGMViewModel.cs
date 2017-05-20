using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GlycolyzerGUImvvm.Commands;
using OmniFinder.Objects;
using OmniFinder.Objects.BuildingBlocks;
using PNNLOmics.Data.Constants;
using PNNLOmics.Data.Constants.Libraries;
using OmniFinder;
using GlycolyzerGUImvvm.Models;
using System.IO;
using OmniFinder.Objects.Enumerations;

namespace GlycolyzerGUImvvm.ViewModels
{
    public class OmniFinderGMViewModel : ObservableObject
    {
        private Models.OmniFinderGMModel omniFinderGMModel;

        public Models.OmniFinderGMModel OmniFinderGMModel
        {
            get { return omniFinderGMModel; }
            set 
            {
                if (value != omniFinderGMModel)
                { omniFinderGMModel = value; App.omniFinderGMModel_Save = omniFinderGMModel; this.OnPropertyChanged("omniFinderGMModel"); }
                 
            }
        }

        private Models.GlycanMakerModel glycanMakerModel;

        public Models.GlycanMakerModel GlycanMakerModel
        {
            get { return glycanMakerModel; }
            set
            {
                if (value != glycanMakerModel)
                { glycanMakerModel = value; App.glycanMakerModel_Save = glycanMakerModel; this.OnPropertyChanged("glycanMakerModel"); }
            }
        }

        private Models.RangesModel omniFinderGMRangesModel;

        public Models.RangesModel OmniFinderGMRangesModel
        {
            get { return omniFinderGMRangesModel; }
            set
            {
                if (value != omniFinderGMRangesModel)
                { omniFinderGMRangesModel = value; App.omniFinderGMRangesModel_Save = omniFinderGMRangesModel; this.OnPropertyChanged("rangesModel"); }
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

        public OmniFinderGMViewModel()
        {
            omniFinderGMModel = new Models.OmniFinderGMModel();
            glycanMakerModel = new Models.GlycanMakerModel();
            omniFinderGMRangesModel = new Models.RangesModel();

            OmniFinderGMModel = App.omniFinderGMModel_Save;
            GlycanMakerModel = App.glycanMakerModel_Save;
            OmniFinderGMRangesModel = App.omniFinderGMRangesModel_Save;

            ButtonCommand = new RelayCommand(new Action<object>(NavigateToPage));
        }

        public static void SelectedOption_Changed()
        {
            switch (App.omniFinderGMModel_Save.SelectedOption_String)
            {
                case "N Glycans":
                    {
                        App.omniFinderGMModel_Save.CheckedHexose_Bool = true;
                        App.omniFinderGMModel_Save.CheckedHexNAc_Bool = true;
                        App.omniFinderGMModel_Save.CheckedDxyHex_Bool = true;
                        App.omniFinderGMModel_Save.CheckedNeuAc_Bool = true;
                        App.omniFinderGMModel_Save.CheckedNaH_Bool = true;
                        break;
                    }
                case "Amino Acids":
                    {
                        App.omniFinderGMModel_Save.CheckedAla_Bool = true;
                        App.omniFinderGMModel_Save.CheckedArg_Bool = true;
                        App.omniFinderGMModel_Save.CheckedAsn_Bool = true;
                        App.omniFinderGMModel_Save.CheckedAsp_Bool = true;
                        App.omniFinderGMModel_Save.CheckedCys_Bool = true;
                        App.omniFinderGMModel_Save.CheckedGln_Bool = true;
                        App.omniFinderGMModel_Save.CheckedGlu_Bool = true;
                        App.omniFinderGMModel_Save.CheckedGly_Bool = true;
                        App.omniFinderGMModel_Save.CheckedHis_Bool = true;
                        App.omniFinderGMModel_Save.CheckedIle_Bool = true;
                        App.omniFinderGMModel_Save.CheckedLeu_Bool = true;
                        App.omniFinderGMModel_Save.CheckedLys_Bool = true;
                        App.omniFinderGMModel_Save.CheckedMet_Bool = true;
                        App.omniFinderGMModel_Save.CheckedPhe_Bool = true;
                        App.omniFinderGMModel_Save.CheckedSer_Bool = true;
                        App.omniFinderGMModel_Save.CheckedThr_Bool = true;
                        App.omniFinderGMModel_Save.CheckedTrp_Bool = true;
                        App.omniFinderGMModel_Save.CheckedTyr_Bool = true;
                        App.omniFinderGMModel_Save.CheckedVal_Bool = true;
                        App.omniFinderGMModel_Save.CheckedPro_Bool = true;
                        break;
                    }
                case "No Option Selected":
                    {
                        App.omniFinderGMModel_Save.CheckedHexose_Bool = false;
                        App.omniFinderGMModel_Save.CheckedHexNAc_Bool = false;
                        App.omniFinderGMModel_Save.CheckedDxyHex_Bool = false;
                        App.omniFinderGMModel_Save.CheckedNeuAc_Bool = false;
                        App.omniFinderGMModel_Save.CheckedNaH_Bool = false;
                            
                        App.omniFinderGMModel_Save.CheckedAla_Bool = false;
                        App.omniFinderGMModel_Save.CheckedArg_Bool = false;
                        App.omniFinderGMModel_Save.CheckedAsn_Bool = false;
                        App.omniFinderGMModel_Save.CheckedAsp_Bool = false;
                        App.omniFinderGMModel_Save.CheckedCys_Bool = false;
                        App.omniFinderGMModel_Save.CheckedGln_Bool = false;
                        App.omniFinderGMModel_Save.CheckedGlu_Bool = false;
                        App.omniFinderGMModel_Save.CheckedGly_Bool = false;
                        App.omniFinderGMModel_Save.CheckedHis_Bool = false;
                        App.omniFinderGMModel_Save.CheckedIle_Bool = false;
                        App.omniFinderGMModel_Save.CheckedLeu_Bool = false;
                        App.omniFinderGMModel_Save.CheckedLys_Bool = false;
                        App.omniFinderGMModel_Save.CheckedMet_Bool = false;
                        App.omniFinderGMModel_Save.CheckedPhe_Bool = false;
                        App.omniFinderGMModel_Save.CheckedSer_Bool = false;
                        App.omniFinderGMModel_Save.CheckedThr_Bool = false;
                        App.omniFinderGMModel_Save.CheckedTrp_Bool = false;
                        App.omniFinderGMModel_Save.CheckedTyr_Bool = false;
                        App.omniFinderGMModel_Save.CheckedVal_Bool = false;
                        App.omniFinderGMModel_Save.CheckedPro_Bool = false;
                        break;
                    }
            }
        }
     
        public void NavigateToPage(object obj)
        {
            switch ((String)obj)
            {
                case "reset":
                    {
                        #region reset
                        //Select Options
                        OmniFinderGMModel.SelectedOption_String = "No Option Selected";


                        //Charge Carrier
                        GlycanMakerModel.NumberOfChargeCarrier_Int = 1;
                        GlycanMakerModel.TypeOfChargeCarrier_String = "H";


                        //Carbohydrate Type
                        GlycanMakerModel.CarbohydrateType_String = "Aldehyde";


                        //Special
                        OmniFinderGMModel.CheckedNaH_Bool = false;
                        OmniFinderGMModel.CheckedCH3_Bool = false;
                        OmniFinderGMModel.CheckedSO3_Bool = false;
                        OmniFinderGMModel.CheckedOAcetyl_Bool = false;

                        GlycanMakerModel.NumberOfNaH_Int = 0;
                        GlycanMakerModel.NumberOfCH3_Int = 0;
                        GlycanMakerModel.NumberOfSO3_Int = 0;
                        GlycanMakerModel.NumberOfOAcetyl_Int = 0;


                        //Sugars
                        OmniFinderGMModel.CheckedHexose_Bool = false;
                        OmniFinderGMModel.CheckedHexNAc_Bool = false;
                        OmniFinderGMModel.CheckedDxyHex_Bool = false;
                        OmniFinderGMModel.CheckedPentose_Bool = false;
                        OmniFinderGMModel.CheckedNeuAc_Bool = false;
                        OmniFinderGMModel.CheckedNeuGc_Bool = false;
                        OmniFinderGMModel.CheckedKDN_Bool = false;
                        OmniFinderGMModel.CheckedHexA_Bool = false;

                        GlycanMakerModel.NumberOfHexose_Int = 0;
                        GlycanMakerModel.NumberOfHexNAc_Int = 0;
                        GlycanMakerModel.NumberOfDeoxyhexose_Int = 0;
                        GlycanMakerModel.NumberOfPentose_Int = 0;
                        GlycanMakerModel.NumberOfNeuAc_Int = 0;
                        GlycanMakerModel.NumberOfNeuGc_Int = 0;
                        GlycanMakerModel.NumberOfKDN_Int = 0;
                        GlycanMakerModel.NumberOfHexA_Int = 0;


                        //User Units
                        OmniFinderGMModel.CheckedUserUnit1_Bool = false;
                        OmniFinderGMModel.CheckedUserUnit2_Bool = false;

                        GlycanMakerModel.NumberOfUserUnit1_Int = 0;
                        GlycanMakerModel.NumberOfUserUnit2_Int = 0;
                        OmniFinderGMRangesModel.MassUserUnit1_Double = 0.0;
                        OmniFinderGMRangesModel.MassUserUnit2_Double = 0.0;


                        //Amino Acids
                        OmniFinderGMModel.CheckedAla_Bool = false;
                        OmniFinderGMModel.CheckedArg_Bool = false;
                        OmniFinderGMModel.CheckedAsn_Bool = false;
                        OmniFinderGMModel.CheckedAsp_Bool = false;
                        OmniFinderGMModel.CheckedCys_Bool = false;
                        OmniFinderGMModel.CheckedGln_Bool = false;
                        OmniFinderGMModel.CheckedGlu_Bool = false;
                        OmniFinderGMModel.CheckedGly_Bool = false;
                        OmniFinderGMModel.CheckedHis_Bool = false;
                        OmniFinderGMModel.CheckedIle_Bool = false;
                        OmniFinderGMModel.CheckedLeu_Bool = false;
                        OmniFinderGMModel.CheckedLys_Bool = false;
                        OmniFinderGMModel.CheckedMet_Bool = false;
                        OmniFinderGMModel.CheckedPhe_Bool = false;
                        OmniFinderGMModel.CheckedSer_Bool = false;
                        OmniFinderGMModel.CheckedThr_Bool = false;
                        OmniFinderGMModel.CheckedTrp_Bool = false;
                        OmniFinderGMModel.CheckedTyr_Bool = false;
                        OmniFinderGMModel.CheckedVal_Bool = false;
                        OmniFinderGMModel.CheckedPro_Bool = false;

                        GlycanMakerModel.NumberOfAla_Int = 0;
                        GlycanMakerModel.NumberOfArg_Int = 0;
                        GlycanMakerModel.NumberOfAsn_Int = 0;
                        GlycanMakerModel.NumberOfAsp_Int = 0;
                        GlycanMakerModel.NumberOfCys_Int = 0;
                        GlycanMakerModel.NumberOfGln_Int = 0;
                        GlycanMakerModel.NumberOfGlu_Int = 0;
                        GlycanMakerModel.NumberOfGly_Int = 0;
                        GlycanMakerModel.NumberOfHis_Int = 0;
                        GlycanMakerModel.NumberOfIle_Int = 0;
                        GlycanMakerModel.NumberOfLeu_Int = 0;
                        GlycanMakerModel.NumberOfLys_Int = 0;
                        GlycanMakerModel.NumberOfMet_Int = 0;
                        GlycanMakerModel.NumberOfPhe_Int = 0;
                        GlycanMakerModel.NumberOfSer_Int = 0;
                        GlycanMakerModel.NumberOfThr_Int = 0;
                        GlycanMakerModel.NumberOfTrp_Int = 0;
                        GlycanMakerModel.NumberOfTyr_Int = 0;
                        GlycanMakerModel.NumberOfVal_Int = 0;
                        GlycanMakerModel.NumberOfPro_Int = 0;


                        //Permethyl
                        OmniFinderGMModel.CheckedpHex_Bool = false;
                        OmniFinderGMModel.CheckedpHxNAc_Bool = false;
                        OmniFinderGMModel.CheckedpDxHex_Bool = false;
                        OmniFinderGMModel.CheckedpPntos_Bool = false;
                        OmniFinderGMModel.CheckedpNuAc_Bool = false;
                        OmniFinderGMModel.CheckedpNuGc_Bool = false;
                        OmniFinderGMModel.CheckedpKDN_Bool = false;
                        OmniFinderGMModel.CheckedpHxA_Bool = false;

                        GlycanMakerModel.NumberOfpHex_Int = 0;
                        GlycanMakerModel.NumberOfpHxNAc_Int = 0;
                        GlycanMakerModel.NumberOfpDxHex_Int = 0;
                        GlycanMakerModel.NumberOfpPntos_Int = 0;
                        GlycanMakerModel.NumberOfpNuAc_Int = 0;
                        GlycanMakerModel.NumberOfpNuGc_Int = 0;
                        GlycanMakerModel.NumberOfpKDN_Int = 0;
                        GlycanMakerModel.NumberOfpHxA_Int = 0;


                        //Neutral Mass, Mass Charge, C, H, O, N, Na Results
                        GlycanMakerModel.NeutralMass_Double = 0.0;
                        GlycanMakerModel.MassCharge_Double = 0.0;
                        GlycanMakerModel.NumberOfC_Int = 0;
                        GlycanMakerModel.NumberOfH_Int = 0;
                        GlycanMakerModel.NumberOfO_Int = 0;
                        GlycanMakerModel.NumberOfN_Int = 0;
                        GlycanMakerModel.NumberOfNa_Int = 0;
                        #endregion
                        break;
                    }
                case "calculateMass":
                    {
                        #region Calculate Mass
                        GlycanMakerObject setMeUp = new GlycanMakerObject();

                        if (GlycanMakerModel.NumberOfChargeCarrier_Int == 0)
                            setMeUp.Charge = 1;
                        else
                            setMeUp.Charge = GlycanMakerModel.NumberOfChargeCarrier_Int;
                        setMeUp.MassTollerance = 0; //place holder since we do not need it for the glycan maker

                        //Type of Charge Carrier
                        switch (GlycanMakerModel.TypeOfChargeCarrier_String)
                        {
                            case "H":
                                {
                                    setMeUp.ChargeCarryingAdduct = Adducts.H;
                                    break;
                                }
                            case "Na":
                                {
                                    setMeUp.ChargeCarryingAdduct = Adducts.Na;
                                    break;
                                }
                            case "K":
                                {
                                    setMeUp.ChargeCarryingAdduct = Adducts.K;
                                    break;
                                }
                            case "-H":
                                {
                                    setMeUp.ChargeCarryingAdduct = Adducts.DeProtonated;
                                    break;
                                }
                            case "NH4":
                                {
                                    setMeUp.ChargeCarryingAdduct = Adducts.NH4;
                                    break;
                                }
                            case "Water":
                                {
                                    setMeUp.ChargeCarryingAdduct = Adducts.Water;
                                    break;
                                }
                            case "Neutral":
                                {
                                    setMeUp.ChargeCarryingAdduct = Adducts.Neutral;
                                    break;
                                }
                            case "UserA":
                                {
                                    setMeUp.ChargeCarryingAdduct = Adducts.UserDefined;
                                    break;
                                }
                            case "UserB":
                                {
                                    setMeUp.ChargeCarryingAdduct = Adducts.UserDefined;
                                    break;
                                }
                        }

                        //Carbohydrate Type
                        switch (GlycanMakerModel.CarbohydrateType_String)
                        {
                            case "Aldehyde":
                                {
                                    setMeUp.CarbohydrateType = CarbType.Aldehyde;
                                    break;
                                }
                            case "Alditol":
                                {
                                    setMeUp.CarbohydrateType = CarbType.Alditol;
                                    break;
                                }
                            case "Fragment":
                                {
                                    setMeUp.CarbohydrateType = CarbType.Fragment;
                                    break;
                                }
                        }

                        //Sugars
                        BuildingBlock hexoseBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.Hexose,
                            new RangesMinMax(GlycanMakerModel.NumberOfHexose_Int, GlycanMakerModel.NumberOfHexose_Int));
                        BuildingBlock deoxyhexoseBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.Deoxyhexose,
                            new RangesMinMax(GlycanMakerModel.NumberOfDeoxyhexose_Int, GlycanMakerModel.NumberOfDeoxyhexose_Int));
                        BuildingBlock hexNAcBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.HexuronicAcid,
                            new RangesMinMax(GlycanMakerModel.NumberOfHexNAc_Int, GlycanMakerModel.NumberOfHexNAc_Int));
                        BuildingBlock pentoseBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.Pentose,
                            new RangesMinMax(GlycanMakerModel.NumberOfPentose_Int, GlycanMakerModel.NumberOfPentose_Int));
                        BuildingBlock neuAcBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.NeuraminicAcid,
                            new RangesMinMax(GlycanMakerModel.NumberOfNeuAc_Int, GlycanMakerModel.NumberOfNeuAc_Int));
                        BuildingBlock neuGcBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.NGlycolylneuraminicAcid,
                            new RangesMinMax(GlycanMakerModel.NumberOfNeuGc_Int, GlycanMakerModel.NumberOfNeuGc_Int));
                        BuildingBlock kdnBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.KDN,
                            new RangesMinMax(GlycanMakerModel.NumberOfKDN_Int, GlycanMakerModel.NumberOfKDN_Int));
                        BuildingBlock hexABlock = new BuildingBlockMonoSaccharide(MonosaccharideName.HexuronicAcid,
                            new RangesMinMax(GlycanMakerModel.NumberOfHexA_Int, GlycanMakerModel.NumberOfHexA_Int));

                        //User Units
                        double mass1 = OmniFinderGMRangesModel.MassUserUnit1_Double;
                        double mass2 = OmniFinderGMRangesModel.MassUserUnit2_Double;

                        UserUnitLibrary myLibrary = new UserUnitLibrary();
                        UserUnit unit1 = new UserUnit("user1", "u1", mass1, UserUnitName.User01);
                        UserUnit unit2 = new UserUnit("user2", "u2", mass2, UserUnitName.User02);
                        UserUnit unit3 = new UserUnit("user3", "u3", 0, UserUnitName.User03);
                        myLibrary.SetLibrary(unit1, unit2, unit3);
                        setMeUp.OmniFinderParameter.UserUnitLibrary = myLibrary;

                        BuildingBlock user1Block = new BuildingBlockUserUnit(UserUnitName.User01,
                            new RangesMinMax(GlycanMakerModel.NumberOfUserUnit1_Int, GlycanMakerModel.NumberOfUserUnit1_Int));
                        BuildingBlock user2Block = new BuildingBlockUserUnit(UserUnitName.User02,
                            new RangesMinMax(GlycanMakerModel.NumberOfUserUnit2_Int, GlycanMakerModel.NumberOfUserUnit2_Int));

                        //Special
                        BuildingBlock naHBlock = new BuildingBlockMiscellaneousMatter(MiscellaneousMatterName.NaMinusH,
                            new RangesMinMax(GlycanMakerModel.NumberOfNaH_Int, GlycanMakerModel.NumberOfNaH_Int));
                        BuildingBlock cH3Block = new BuildingBlockMiscellaneousMatter(MiscellaneousMatterName.Methyl,
                            new RangesMinMax(GlycanMakerModel.NumberOfCH3_Int, GlycanMakerModel.NumberOfCH3_Int));
                        BuildingBlock sO3Block = new BuildingBlockMiscellaneousMatter(MiscellaneousMatterName.Sulfate,
                            new RangesMinMax(GlycanMakerModel.NumberOfSO3_Int, GlycanMakerModel.NumberOfSO3_Int));
                        BuildingBlock oAcetylBlock = new BuildingBlockMiscellaneousMatter(MiscellaneousMatterName.OAcetyl,
                            new RangesMinMax(GlycanMakerModel.NumberOfOAcetyl_Int, GlycanMakerModel.NumberOfOAcetyl_Int));

                        //Amino Acids
                        BuildingBlock alaBlock = new BuildingBlockAminoAcid(AminoAcidName.Alanine,
                            new RangesMinMax(GlycanMakerModel.NumberOfAla_Int, GlycanMakerModel.NumberOfAla_Int));
                        BuildingBlock argBlock = new BuildingBlockAminoAcid(AminoAcidName.Arginine,
                            new RangesMinMax(GlycanMakerModel.NumberOfArg_Int, GlycanMakerModel.NumberOfArg_Int));
                        BuildingBlock asnBlock = new BuildingBlockAminoAcid(AminoAcidName.Asparagine,
                            new RangesMinMax(GlycanMakerModel.NumberOfAsn_Int, GlycanMakerModel.NumberOfAsn_Int));
                        BuildingBlock aspBlock = new BuildingBlockAminoAcid(AminoAcidName.AsparticAcid,
                            new RangesMinMax(GlycanMakerModel.NumberOfAsp_Int, GlycanMakerModel.NumberOfAsp_Int));
                        BuildingBlock cysBlock = new BuildingBlockAminoAcid(AminoAcidName.Cysteine,
                            new RangesMinMax(GlycanMakerModel.NumberOfCys_Int, GlycanMakerModel.NumberOfCys_Int));
                        BuildingBlock glnBlock = new BuildingBlockAminoAcid(AminoAcidName.Glutamine,
                            new RangesMinMax(GlycanMakerModel.NumberOfGln_Int, GlycanMakerModel.NumberOfGln_Int));
                        BuildingBlock gluBlock = new BuildingBlockAminoAcid(AminoAcidName.GlutamicAcid,
                            new RangesMinMax(GlycanMakerModel.NumberOfGlu_Int, GlycanMakerModel.NumberOfGlu_Int));
                        BuildingBlock glyBlock = new BuildingBlockAminoAcid(AminoAcidName.Glycine,
                            new RangesMinMax(GlycanMakerModel.NumberOfGly_Int, GlycanMakerModel.NumberOfGly_Int));
                        BuildingBlock hisBlock = new BuildingBlockAminoAcid(AminoAcidName.Histidine,
                            new RangesMinMax(GlycanMakerModel.NumberOfHis_Int, GlycanMakerModel.NumberOfHis_Int));
                        BuildingBlock ileBlock = new BuildingBlockAminoAcid(AminoAcidName.Isoleucine,
                            new RangesMinMax(GlycanMakerModel.NumberOfIle_Int, GlycanMakerModel.NumberOfIle_Int));
                        BuildingBlock leuBlock = new BuildingBlockAminoAcid(AminoAcidName.Leucine,
                            new RangesMinMax(GlycanMakerModel.NumberOfLeu_Int, GlycanMakerModel.NumberOfLeu_Int));
                        BuildingBlock lysBlock = new BuildingBlockAminoAcid(AminoAcidName.Lysine,
                            new RangesMinMax(GlycanMakerModel.NumberOfLys_Int, GlycanMakerModel.NumberOfLys_Int));
                        BuildingBlock metBlock = new BuildingBlockAminoAcid(AminoAcidName.Methionine,
                            new RangesMinMax(GlycanMakerModel.NumberOfMet_Int, GlycanMakerModel.NumberOfMet_Int));
                        BuildingBlock pheBlock = new BuildingBlockAminoAcid(AminoAcidName.Phenylalanine,
                            new RangesMinMax(GlycanMakerModel.NumberOfPhe_Int, GlycanMakerModel.NumberOfPhe_Int));
                        BuildingBlock serBlock = new BuildingBlockAminoAcid(AminoAcidName.Serine,
                            new RangesMinMax(GlycanMakerModel.NumberOfSer_Int, GlycanMakerModel.NumberOfSer_Int));
                        BuildingBlock thrBlock = new BuildingBlockAminoAcid(AminoAcidName.Threonine,
                            new RangesMinMax(GlycanMakerModel.NumberOfThr_Int, GlycanMakerModel.NumberOfThr_Int));
                        BuildingBlock trpBlock = new BuildingBlockAminoAcid(AminoAcidName.Tryptophan,
                            new RangesMinMax(GlycanMakerModel.NumberOfTrp_Int, GlycanMakerModel.NumberOfTrp_Int));
                        BuildingBlock tyrBlock = new BuildingBlockAminoAcid(AminoAcidName.Tyrosine,
                            new RangesMinMax(GlycanMakerModel.NumberOfTyr_Int, GlycanMakerModel.NumberOfTyr_Int));
                        BuildingBlock valBlock = new BuildingBlockAminoAcid(AminoAcidName.Valine,
                            new RangesMinMax(GlycanMakerModel.NumberOfVal_Int, GlycanMakerModel.NumberOfVal_Int));
                        BuildingBlock proBlock = new BuildingBlockAminoAcid(AminoAcidName.Proline,
                            new RangesMinMax(GlycanMakerModel.NumberOfPro_Int, GlycanMakerModel.NumberOfPro_Int));

                        //Permethyl


                        //Add Sugars, User Units, Specials, Amino Acids to SetMeUp
                        setMeUp.LegoBuildingBlocks.Add(hexoseBlock);
                        setMeUp.LegoBuildingBlocks.Add(deoxyhexoseBlock);
                        setMeUp.LegoBuildingBlocks.Add(hexNAcBlock);
                        setMeUp.LegoBuildingBlocks.Add(pentoseBlock);
                        setMeUp.LegoBuildingBlocks.Add(neuAcBlock);
                        setMeUp.LegoBuildingBlocks.Add(neuGcBlock);
                        setMeUp.LegoBuildingBlocks.Add(kdnBlock);
                        setMeUp.LegoBuildingBlocks.Add(hexABlock);

                        setMeUp.LegoBuildingBlocks.Add(user1Block);
                        setMeUp.LegoBuildingBlocks.Add(user2Block);

                        setMeUp.LegoBuildingBlocks.Add(naHBlock);
                        setMeUp.LegoBuildingBlocks.Add(cH3Block);
                        setMeUp.LegoBuildingBlocks.Add(sO3Block);
                        setMeUp.LegoBuildingBlocks.Add(oAcetylBlock);

                        setMeUp.LegoBuildingBlocks.Add(alaBlock);
                        setMeUp.LegoBuildingBlocks.Add(argBlock);
                        setMeUp.LegoBuildingBlocks.Add(asnBlock);
                        setMeUp.LegoBuildingBlocks.Add(aspBlock);
                        setMeUp.LegoBuildingBlocks.Add(cysBlock);
                        setMeUp.LegoBuildingBlocks.Add(glnBlock);
                        setMeUp.LegoBuildingBlocks.Add(gluBlock);
                        setMeUp.LegoBuildingBlocks.Add(glyBlock);
                        setMeUp.LegoBuildingBlocks.Add(hisBlock);
                        setMeUp.LegoBuildingBlocks.Add(ileBlock);
                        setMeUp.LegoBuildingBlocks.Add(leuBlock);
                        setMeUp.LegoBuildingBlocks.Add(lysBlock);
                        setMeUp.LegoBuildingBlocks.Add(metBlock);
                        setMeUp.LegoBuildingBlocks.Add(pheBlock);
                        setMeUp.LegoBuildingBlocks.Add(serBlock);
                        setMeUp.LegoBuildingBlocks.Add(thrBlock);
                        setMeUp.LegoBuildingBlocks.Add(trpBlock);
                        setMeUp.LegoBuildingBlocks.Add(tyrBlock);
                        setMeUp.LegoBuildingBlocks.Add(valBlock);
                        setMeUp.LegoBuildingBlocks.Add(proBlock);

                        GlycanMakerObject inputForGlycanMaker = setMeUp;
                        GlycanMakerOutput results = GlycanMakerController.CalculateMass(inputForGlycanMaker, inputForGlycanMaker.Charge);

                        //Neutral Mass get, set, output
                        GlycanMakerModel.NeutralMass_Double = Convert.ToDouble(results.MassNeutral);

                        //Mass/Charge get, set, output
                        GlycanMakerModel.MassCharge_Double = Convert.ToDouble(results.MassToCharge);

                        //C, H, O, N, Na
                        
                        #endregion
                        break;
                    }
            }
        }
    }
}
