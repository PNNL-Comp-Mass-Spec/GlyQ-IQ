using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.IO;


namespace GlycolyzerGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// Global variables for all pages to save to and manipulate
    /// </summary>
    public partial class App : Application
    {
        public static GlycanMakerVariables glycanMakerVariables = new GlycanMakerVariables();
        public static OmniFinderVariables utilitiesOmniFinderVariables = new OmniFinderVariables();
        public static OmniFinderVariables parametersOmniFinderVariables = new OmniFinderVariables();
        public static LibraryVariables libraryVariables = new LibraryVariables();
        public static ExtraScienceParameterVariables extraScienceParameterVariables = new ExtraScienceParameterVariables();
        public static InputOutputSaveFolderVariables inputOutputSaveFolderVariables = new InputOutputSaveFolderVariables();
        public static InitializePages initializePages = new InitializePages();
        public static RangesPageVariables parametersRangesPageVariables = new RangesPageVariables();
        public static RangesPageVariables utilitiesRangesPageVariables = new RangesPageVariables();

        //Initialize Pages
        public static UtilitiesSubHomePage utilitiesSubHomePage;///////////////////////
        public static ParametersSubHomePage parametersSubHomePage;
        public static GlycanMakerPage glycanMakerPage;/////////////////////////////////
        public static UtilitiesOmniFinderPage utilitiesOmniFinderPage;
        public static OmniFinderWithGlycanMaker utilitiesSpecialOmniFinderPageDesign;
        public static ParametersOmniFinderPageDesign parametersOmniFinderPageDesign;
        public static ParametersRangesPage parametersRangesPage;
        public static UtilitiesRangesPage utilitiesRangesPage;
        public static UtilitiesRangesSubHomePage utilitiesRangesSubHomePage;

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.App_Exit;
            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ReadXMLFile("");
        }

        void App_Exit(object sender, ExitEventArgs e)
        {
            ParametersSubHomePage.SaveXMLFile("");
        }

        public static void ReadXMLFile(String readMainFile)
        {
            if (readMainFile == "")
                readMainFile = "D:\\GlycolyzerXMLFile.xml";

            string readFromFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(readMainFile), "GlycolyzerSubDir");
            readFromFolder = System.IO.Path.Combine(readFromFolder, "Glycolyzer");

            if (File.Exists(readFromFolder + "ParametersRanges.xml"))
            {
                System.Xml.Serialization.XmlSerializer parameterRangesReader = new System.Xml.Serialization.XmlSerializer(typeof(App.RangesPageVariables));
                System.IO.StreamReader parameterRangesFile = new System.IO.StreamReader(readFromFolder + "ParametersRanges.xml");
                App.parametersRangesPageVariables = (App.RangesPageVariables)parameterRangesReader.Deserialize(parameterRangesFile);
                parameterRangesFile.Close();
            }

            if (File.Exists(readFromFolder + "ParametersOmniFinder.xml"))
            {
                System.Xml.Serialization.XmlSerializer omniReader = new System.Xml.Serialization.XmlSerializer(typeof(App.OmniFinderVariables));
                System.IO.StreamReader omniFile = new System.IO.StreamReader(readFromFolder + "ParametersOmniFinder.xml");
                App.parametersOmniFinderVariables = (App.OmniFinderVariables)omniReader.Deserialize(omniFile);
                omniFile.Close();
            }

            if (File.Exists(readFromFolder + "ExtraScienceParameters.xml"))
            {
                System.Xml.Serialization.XmlSerializer parametersReader = new System.Xml.Serialization.XmlSerializer(typeof(App.ExtraScienceParameterVariables));
                System.IO.StreamReader parametersFile = new System.IO.StreamReader(readFromFolder + "ExtraScienceParameters.xml");
                App.extraScienceParameterVariables = (App.ExtraScienceParameterVariables)parametersReader.Deserialize(parametersFile);
                parametersFile.Close();
            }

            if (File.Exists(readFromFolder + "InputOutputSaveFolder.xml"))
            {
                System.Xml.Serialization.XmlSerializer folderReader = new System.Xml.Serialization.XmlSerializer(typeof(App.InputOutputSaveFolderVariables));
                System.IO.StreamReader folderFile = new System.IO.StreamReader(readFromFolder + "InputOutputSaveFolder.xml");
                App.inputOutputSaveFolderVariables = (App.InputOutputSaveFolderVariables)folderReader.Deserialize(folderFile);
                folderFile.Close();
            }

            if (File.Exists(readFromFolder + "Library.xml"))
            {
                System.Xml.Serialization.XmlSerializer libraryReader = new System.Xml.Serialization.XmlSerializer(typeof(App.LibraryVariables));
                System.IO.StreamReader libraryFile = new System.IO.StreamReader(readFromFolder + "Library.xml");
                App.libraryVariables = (App.LibraryVariables)libraryReader.Deserialize(libraryFile);
                libraryFile.Close();
            }

            if (File.Exists(readFromFolder + "GlycanMaker.xml"))
            {
                System.Xml.Serialization.XmlSerializer glycanReader = new System.Xml.Serialization.XmlSerializer(typeof(App.GlycanMakerVariables));
                System.IO.StreamReader glycanFile = new System.IO.StreamReader(readFromFolder + "GlycanMaker.xml");
                App.glycanMakerVariables = (App.GlycanMakerVariables)glycanReader.Deserialize(glycanFile);
                glycanFile.Close();
            }

            if (File.Exists(readFromFolder + "UtilitiesOmniFinder.xml"))
            {
                System.Xml.Serialization.XmlSerializer utilitiesOmniReader = new System.Xml.Serialization.XmlSerializer(typeof(App.OmniFinderVariables));
                System.IO.StreamReader utilitiesOmniFile = new System.IO.StreamReader(readFromFolder + "UtilitiesOmniFinder.xml");
                App.utilitiesOmniFinderVariables = (App.OmniFinderVariables)utilitiesOmniReader.Deserialize(utilitiesOmniFile);
                utilitiesOmniFile.Close();
            }

            if (File.Exists(readFromFolder + "UtilitiesRanges.xml"))
            {
                System.Xml.Serialization.XmlSerializer utilitiesRangesReader = new System.Xml.Serialization.XmlSerializer(typeof(App.RangesPageVariables));
                System.IO.StreamReader utilitiesRangesFile = new System.IO.StreamReader(readFromFolder + "UtilitiesRanges.xml");
                App.utilitiesRangesPageVariables = (App.RangesPageVariables)utilitiesRangesReader.Deserialize(utilitiesRangesFile);
                utilitiesRangesFile.Close();
            }
        }


        /// <summary>
        /// Glycan Maker Variables
        /// </summary>
        public class GlycanMakerVariables : INotifyPropertyChanged
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
            private Double massOfUserUnit1_Double = 0.0;
            private Double massOfUserUnit2_Double = 0.0;

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

            //Number of C, H, O, N, Na
            private int numberOfC_Int = 0;
            private int numberOfH_Int = 0;
            private int numberOfO_Int = 0;
            private int numberOfN_Int = 0;
            private int numberOfNa_Int = 0;

            /// <summary>
            /// updates in real-time
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            protected void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

            //Sugars
            public int NumberOfHexose_Int
            {
                get { return numberOfHexose_Int; }
                set { numberOfHexose_Int = (int)value; NotifyPropertyChanged("numberOfHexose_Int");}
            }

            public int NumberOfHexNAc_Int
            {
                get { return numberOfHexNAc_Int; }
                set { numberOfHexNAc_Int = (int)value; NotifyPropertyChanged("numberOfHexNAc_Int"); }
            }

            public int NumberOfDeoxyhexose_Int
            {
                get { return numberOfDeoxyhexose_Int; }
                set { numberOfDeoxyhexose_Int = (int)value; NotifyPropertyChanged("numberOfDeoxyhexose_Int"); }
            }

            public int NumberOfPentose_Int
            {
                get { return numberOfPentose_Int; }
                set { numberOfPentose_Int = (int)value; NotifyPropertyChanged("numberOfPentose_Int"); }
            }

            public int NumberOfNeuAc_Int
            {
                get { return numberOfNeuAc_Int; }
                set { numberOfNeuAc_Int = (int)value; NotifyPropertyChanged("numberOfNeuAc_Int"); }
            }

            public int NumberOfNeuGc_Int
            {
                get { return numberOfNeuGc_Int; }
                set { numberOfNeuGc_Int = (int)value; NotifyPropertyChanged("numberOfNeuGc_Int"); }
            }

            public int NumberOfKDN_Int
            {
                get { return numberOfKDN_Int; }
                set { numberOfKDN_Int = (int)value; NotifyPropertyChanged("numberOfKDN_Int"); }
            }

            public int NumberOfHexA_Int
            {
                get { return numberOfHexA_Int; }
                set { numberOfHexA_Int = (int)value; NotifyPropertyChanged("numberOfHexA_Int"); }
            }
            
            //User Units A and B
            public int NumberOfUserUnit1_Int
            {
                get { return numberOfUserUnit1_Int; }
                set { numberOfUserUnit1_Int = (int)value; NotifyPropertyChanged("numberOfUserUnit1_Int"); }
            }

            public int NumberOfUserUnit2_Int
            {
                get { return numberOfUserUnit2_Int; }
                set { numberOfUserUnit2_Int = (int)value; NotifyPropertyChanged("numberOfUserUnit2_Int"); }
            }
            public Double MassOfUserUnit1_Double
            {
                get { return massOfUserUnit1_Double; }
                set { massOfUserUnit1_Double = (Double)value; NotifyPropertyChanged("massOfUserUnit1_Double"); }
            }

            public Double MassOfUserUnit2_Double
            {
                get { return massOfUserUnit2_Double; }
                set { massOfUserUnit2_Double = (Double)value; NotifyPropertyChanged("massOfUserUnit2_Double"); }
            }

            //Specials
            public int NumberOfNaH_Int
            {
                get { return numberOfNaH_Int; }
                set { numberOfNaH_Int = (int)value; NotifyPropertyChanged("numberOfNaH_Int"); }
            }

            public int NumberOfCH3_Int
            {
                get { return numberOfCH3_Int; }
                set { numberOfCH3_Int = (int)value; NotifyPropertyChanged("numberOfCH3_Int"); }
            }

            public int NumberOfSO3_Int
            {
                get { return numberOfSO3_Int; }
                set { numberOfSO3_Int = (int)value; NotifyPropertyChanged("numberOfSO3_Int"); }
            }

            public int NumberOfOAcetyl_Int
            {
                get { return numberOfOAcetyl_Int; }
                set { numberOfOAcetyl_Int = (int)value; NotifyPropertyChanged("numberOfOAcetyl_Int"); }
            }

            //Amino Acids
            public int NumberOfAla_Int
            {
                get { return numberOfAla_Int; }
                set { numberOfAla_Int = (int)value; NotifyPropertyChanged("numberOfAla_Int"); }
            }

            public int NumberOfArg_Int
            {
                get { return numberOfArg_Int; }
                set { numberOfArg_Int = (int)value; NotifyPropertyChanged("numberOfArg_Int"); }
            }

            public int NumberOfAsn_Int
            {
                get { return numberOfAsn_Int; }
                set { numberOfAsn_Int = (int)value; NotifyPropertyChanged("numberOfAsn_Int"); }
            }
            public int NumberOfAsp_Int
            {
                get { return numberOfAsp_Int; }
                set { numberOfAsp_Int = (int)value; NotifyPropertyChanged("numberOfAsp_Int"); }
            }

            public int NumberOfCys_Int
            {
                get { return numberOfCys_Int; }
                set { numberOfCys_Int = (int)value; NotifyPropertyChanged("numberOfCys_Int"); }
            }

            public int NumberOfGln_Int
            {
                get { return numberOfGln_Int; }
                set { numberOfGln_Int = (int)value; NotifyPropertyChanged("numberOfGln_Int"); }
            }
            public int NumberOfGlu_Int
            {
                get { return numberOfGlu_Int; }
                set { numberOfGlu_Int = (int)value; NotifyPropertyChanged("numberOfGlu_Int"); }
            }

            public int NumberOfGly_Int
            {
                get { return numberOfGly_Int; }
                set { numberOfGly_Int = (int)value; NotifyPropertyChanged("numberOfGly_Int"); }
            }

            public int NumberOfHis_Int
            {
                get { return numberOfHis_Int; }
                set { numberOfHis_Int = (int)value; NotifyPropertyChanged("numberOfHis_Int"); }
            }
            public int NumberOfIle_Int
            {
                get { return numberOfIle_Int; }
                set { numberOfIle_Int = (int)value; NotifyPropertyChanged("numberOfIle_Int"); }
            }

            public int NumberOfLeu_Int
            {
                get { return numberOfLeu_Int; }
                set { numberOfLeu_Int = (int)value; NotifyPropertyChanged("numberOfLeu_Int"); }
            }

            public int NumberOfLys_Int
            {
                get { return numberOfLys_Int; }
                set { numberOfLys_Int = (int)value; NotifyPropertyChanged("numberOfLys_Int"); }
            }
            public int NumberOfMet_Int
            {
                get { return numberOfMet_Int; }
                set { numberOfMet_Int = (int)value; NotifyPropertyChanged("numberOfMet_Int"); }
            }

            public int NumberOfPhe_Int
            {
                get { return numberOfPhe_Int; }
                set { numberOfPhe_Int = (int)value; NotifyPropertyChanged("numberOfPhe_Int"); }
            }

            public int NumberOfSer_Int
            {
                get { return numberOfSer_Int; }
                set { numberOfSer_Int = (int)value; NotifyPropertyChanged("numberOfSer_Int"); }
            }
            public int NumberOfThr_Int
            {
                get { return numberOfThr_Int; }
                set { numberOfThr_Int = (int)value; NotifyPropertyChanged("numberOfThr_Int"); }
            }

            public int NumberOfTrp_Int
            {
                get { return numberOfTrp_Int; }
                set { numberOfTrp_Int = (int)value; NotifyPropertyChanged("numberOfTrp_Int"); }
            }

            public int NumberOfTyr_Int
            {
                get { return numberOfTyr_Int; }
                set { numberOfTyr_Int = (int)value; NotifyPropertyChanged("numberOfTyr_Int"); }
            }
            public int NumberOfVal_Int
            {
                get { return numberOfVal_Int; }
                set { numberOfVal_Int = (int)value; NotifyPropertyChanged("numberOfVal_Int"); }
            }

            public int NumberOfPro_Int
            {
                get { return numberOfPro_Int; }
                set { numberOfPro_Int = (int)value; NotifyPropertyChanged("numberOfPro_Int"); }
            }

            //Permethyl
            public int NumberOfpHex_Int
            {
                get { return numberOfpHex_Int; }
                set { numberOfpHex_Int = (int)value; NotifyPropertyChanged("numberOfpHex_Int"); }
            }

            public int NumberOfpHxNAc_Int
            {
                get { return numberOfpHxNAc_Int; }
                set { numberOfpHxNAc_Int = (int)value; NotifyPropertyChanged("numberOfpHxNAc_Int"); }
            }

            public int NumberOfpDxHex_Int
            {
                get { return numberOfpDxHex_Int; }
                set { numberOfpDxHex_Int = (int)value; NotifyPropertyChanged("numberOfpDxHex_Int"); }
            }

            public int NumberOfpPntos_Int
            {
                get { return numberOfpPntos_Int; }
                set { numberOfpPntos_Int = (int)value; NotifyPropertyChanged("numberOfpPntos_Int"); }
            }

            public int NumberOfpNuAc_Int
            {
                get { return numberOfpNuAc_Int; }
                set { numberOfpNuAc_Int = (int)value; NotifyPropertyChanged("numberOfpNuAc_Int"); }
            }

            public int NumberOfpNuGc_Int
            {
                get { return numberOfpNuGc_Int; }
                set { numberOfpNuGc_Int = (int)value; NotifyPropertyChanged("numberOfpNuGc_Int"); }
            }

            public int NumberOfpKDN_Int
            {
                get { return numberOfpKDN_Int; }
                set { numberOfpKDN_Int = (int)value; NotifyPropertyChanged("numberOfpKDN_Int"); }
            }

            public int NumberOfpHxA_Int
            {
                get { return numberOfpHxA_Int; }
                set { numberOfpHxA_Int = (int)value; NotifyPropertyChanged("numberOfpHxA_Int"); }
            }

            //Charge Carrier
            public String TypeOfChargeCarrier_String
            {
                get { return typeOfChargeCarrier_String; }
                set { typeOfChargeCarrier_String = value; NotifyPropertyChanged("typeOfChargeCarrier_String"); }
            }

            public int NumberOfChargeCarrier_Int
            {
                get { return numberOfChargeCarrier_Int; }
                set { numberOfChargeCarrier_Int = (int)value; NotifyPropertyChanged("numberOfChargeCarrier_Int"); }
            }

            //Carbohydrate Type
            public String CarbohydrateType_String
            {
                get { return carbohydrateType_String; }
                set { carbohydrateType_String = value; NotifyPropertyChanged("carbohydrateType_String"); }
            }

            //Neutral Mass
            public Double NeutralMass_Double
            {
                get { return neutralMass_Double; }
                set { neutralMass_Double = (Double)value; NotifyPropertyChanged("neutralMass_Double"); }
            }

            //Mass/Charge
            public Double MassCharge_Double
            {
                get { return massCharge_Double; }
                set { massCharge_Double = (Double)value; NotifyPropertyChanged("massCharge_Double"); }
            }

            //Number of C, H, O, N, Na
            public int NumberOfC_Int
            {
                get { return numberOfC_Int; }
                set { numberOfC_Int = (int)value; NotifyPropertyChanged("numberOfC_Int"); }
            }

            public int NumberOfH_Int
            {
                get { return numberOfH_Int; }
                set { numberOfH_Int = (int)value; NotifyPropertyChanged("numberOfH_Int"); }
            }

            public int NumberOfO_Int
            {
                get { return numberOfO_Int; }
                set { numberOfO_Int = (int)value; NotifyPropertyChanged("numberOfO_Int"); }
            }

            public int NumberOfN_Int
            {
                get { return numberOfN_Int; }
                set { numberOfN_Int = (int)value; NotifyPropertyChanged("numberOfN_Int"); }
            }

            public int NumberOfNa_Int
            {
                get { return numberOfNa_Int; }
                set { numberOfNa_Int = (int)value; NotifyPropertyChanged("numberOfNa_Int"); }
            }
        }

        /// <summary>
        /// OmniFinder Variables
        /// </summary>
        public class OmniFinderVariables : INotifyPropertyChanged
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

            //User Number
            private int numberOfUserUnits_Int = 0;
            
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

            /// <summary>
            /// updates in real-time
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            protected void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

            //Select Options
            public String SelectedOption_String
            {
                get { return selectedOption_String; }
                set { selectedOption_String = value; NotifyPropertyChanged("selectedOption_String"); }
            }

            //User Units
            public Boolean CheckedUserUnit1_Bool
            {
                get { return checkedUserUnit1_Bool; }
                set { checkedUserUnit1_Bool = value; NotifyPropertyChanged("checkedUserUnit1_Bool"); }
            }

            public Boolean CheckedUserUnit2_Bool
            {
                get { return checkedUserUnit2_Bool; }
                set { checkedUserUnit2_Bool = value; NotifyPropertyChanged("checkedUserUnit2_Bool"); }
            }
            public Boolean CheckedUserUnit3_Bool
            {
                get { return checkedUserUnit3_Bool; }
                set { checkedUserUnit3_Bool = value; NotifyPropertyChanged("checkedUserUnit3_Bool"); }
            }

            public Boolean CheckedUserUnit4_Bool
            {
                get { return checkedUserUnit4_Bool; }
                set { checkedUserUnit4_Bool = value; NotifyPropertyChanged("checkedUserUnit4_Bool"); }
            }
            public Boolean CheckedUserUnit5_Bool
            {
                get { return checkedUserUnit5_Bool; }
                set { checkedUserUnit5_Bool = value; NotifyPropertyChanged("checkedUserUnit5_Bool"); }
            }

            public Boolean CheckedUserUnit6_Bool
            {
                get { return checkedUserUnit6_Bool; }
                set { checkedUserUnit6_Bool = value; NotifyPropertyChanged("checkedUserUnit6_Bool"); }
            }
            public Boolean CheckedUserUnit7_Bool
            {
                get { return checkedUserUnit7_Bool; }
                set { checkedUserUnit7_Bool = value; NotifyPropertyChanged("checkedUserUnit7_Bool"); }
            }

            public Boolean CheckedUserUnit8_Bool
            {
                get { return checkedUserUnit8_Bool; }
                set { checkedUserUnit8_Bool = value; NotifyPropertyChanged("checkedUserUnit8_Bool"); }
            }
            public Boolean CheckedUserUnit9_Bool
            {
                get { return checkedUserUnit9_Bool; }
                set { checkedUserUnit9_Bool = value; NotifyPropertyChanged("checkedUserUnit9_Bool"); }
            }

            public Boolean CheckedUserUnit10_Bool
            {
                get { return checkedUserUnit10_Bool; }
                set { checkedUserUnit10_Bool = value; NotifyPropertyChanged("checkedUserUnit10_Bool"); }
            }

            //Sugars
            public Boolean CheckedHexose_Bool
            {
                get { return checkedHexose_Bool; }
                set { checkedHexose_Bool = value; NotifyPropertyChanged("checkedHexose_Bool"); }
            }

            public Boolean CheckedHexNAc_Bool
            {
                get { return checkedHexNAc_Bool; }
                set { checkedHexNAc_Bool = value; NotifyPropertyChanged("checkedHexNAc_Bool"); }
            }

            public Boolean CheckedDxyHex_Bool
            {
                get { return checkedDxyHex_Bool; }
                set { checkedDxyHex_Bool = value; NotifyPropertyChanged("checkedDxyHex_Bool"); }
            }

            public Boolean CheckedPentose_Bool
            {
                get { return checkedPentose_Bool; }
                set { checkedPentose_Bool = value; NotifyPropertyChanged("checkedPentose_Bool"); }
            }

            public Boolean CheckedNeuAc_Bool
            {
                get { return checkedNeuAc_Bool; }
                set { checkedNeuAc_Bool = value; NotifyPropertyChanged("checkedNeuAc_Bool"); }
            }

            public Boolean CheckedNeuGc_Bool
            {
                get { return checkedNeuGc_Bool; }
                set { checkedNeuGc_Bool = value; NotifyPropertyChanged("checkedNeuGc_Bool"); }
            }

            public Boolean CheckedKDN_Bool
            {
                get { return checkedKDN_Bool; }
                set { checkedKDN_Bool = value; NotifyPropertyChanged("checkedKDN_Bool"); }
            }

            public Boolean CheckedHexA_Bool
            {
                get { return checkedHexA_Bool; }
                set { checkedHexA_Bool = value; NotifyPropertyChanged("checkedHexA_Bool"); }
            }

            //User Number
            public int NumberForUser_Int
            {
                get { return numberOfUserUnits_Int; }
                set { numberOfUserUnits_Int = (int)value; NotifyPropertyChanged("numberForUser_Int"); }
            }

            //Amino Acids
            public Boolean CheckedAla_Bool
            {
                get { return checkedAla_Bool; }
                set { checkedAla_Bool = value; NotifyPropertyChanged("checkedAla_Bool"); }
            }

            public Boolean CheckedArg_Bool
            {
                get { return checkedArg_Bool; }
                set { checkedArg_Bool = value; NotifyPropertyChanged("checkedArg_Bool"); }
            }

            public Boolean CheckedAsn_Bool
            {
                get { return checkedAsn_Bool; }
                set { checkedAsn_Bool = value; NotifyPropertyChanged("checkedAsn_Bool"); }
            }

            public Boolean CheckedAsp_Bool
            {
                get { return checkedAsp_Bool; }
                set { checkedAsp_Bool = value; NotifyPropertyChanged("checkedAsp_Bool"); }
            }

            public Boolean CheckedCys_Bool
            {
                get { return checkedCys_Bool; }
                set { checkedCys_Bool = value; NotifyPropertyChanged("checkedCys_Bool"); }
            }

            public Boolean CheckedGln_Bool
            {
                get { return checkedGln_Bool; }
                set { checkedGln_Bool = value; NotifyPropertyChanged("checkedGln_Bool"); }
            }

            public Boolean CheckedGlu_Bool
            {
                get { return checkedGlu_Bool; }
                set { checkedGlu_Bool = value; NotifyPropertyChanged("checkedGlu_Bool"); }
            }

            public Boolean CheckedGly_Bool
            {
                get { return checkedGly_Bool; }
                set { checkedGly_Bool = value; NotifyPropertyChanged("checkedGly_Bool"); }
            }

            public Boolean CheckedHis_Bool
            {
                get { return checkedHis_Bool; }
                set { checkedHis_Bool = value; NotifyPropertyChanged("checkedHis_Bool"); }
            }

            public Boolean CheckedIle_Bool
            {
                get { return checkedIle_Bool; }
                set { checkedIle_Bool = value; NotifyPropertyChanged("checkedIle_Bool"); }
            }

            public Boolean CheckedLeu_Bool
            {
                get { return checkedLeu_Bool; }
                set { checkedLeu_Bool = value; NotifyPropertyChanged("checkedLeu_Bool"); }
            }

            public Boolean CheckedLys_Bool
            {
                get { return checkedLys_Bool; }
                set { checkedLys_Bool = value; NotifyPropertyChanged("checkedLys_Bool"); }
            }

            public Boolean CheckedMet_Bool
            {
                get { return checkedMet_Bool; }
                set { checkedMet_Bool = value; NotifyPropertyChanged("checkedMet_Bool"); }
            }

            public Boolean CheckedPhe_Bool
            {
                get { return checkedPhe_Bool; }
                set { checkedPhe_Bool = value; NotifyPropertyChanged("checkedPhe_Bool"); }
            }

            public Boolean CheckedSer_Bool
            {
                get { return checkedSer_Bool; }
                set { checkedSer_Bool = value; NotifyPropertyChanged("checkedSer_Bool"); }
            }

            public Boolean CheckedThr_Bool
            {
                get { return checkedThr_Bool; }
                set { checkedThr_Bool = value; NotifyPropertyChanged("checkedThr_Bool"); }
            }

            public Boolean CheckedTrp_Bool
            {
                get { return checkedTrp_Bool; }
                set { checkedTrp_Bool = value; NotifyPropertyChanged("checkedTrp_Bool"); }
            }

            public Boolean CheckedTyr_Bool
            {
                get { return checkedTyr_Bool; }
                set { checkedTyr_Bool = value; NotifyPropertyChanged("checkedTyr_Bool"); }
            }

            public Boolean CheckedVal_Bool
            {
                get { return checkedVal_Bool; }
                set { checkedVal_Bool = value; NotifyPropertyChanged("checkedVal_Bool"); }
            }

            public Boolean CheckedPro_Bool
            {
                get { return checkedPro_Bool; }
                set { checkedPro_Bool = value; NotifyPropertyChanged("checkedPro_Bool"); }
            }

            //Special
            public Boolean CheckedNaH_Bool
            {
                get { return checkedNaH_Bool; }
                set { checkedNaH_Bool = value; NotifyPropertyChanged("checkedNaH_Bool"); }
            }

            public Boolean CheckedCH3_Bool
            {
                get { return checkedCH3_Bool; }
                set { checkedCH3_Bool = value; NotifyPropertyChanged("checkedCH3_Bool"); }
            }

            public Boolean CheckedSO3_Bool
            {
                get { return checkedSO3_Bool; }
                set { checkedSO3_Bool = value; NotifyPropertyChanged("checkedSO3_Bool"); }
            }

            public Boolean CheckedOAcetyl_Bool
            {
                get { return checkedOAcetyl_Bool; }
                set { checkedOAcetyl_Bool = value; NotifyPropertyChanged("checkedOAcetyl_Bool"); }
            }

            //Permethyl
            public Boolean CheckedpHex_Bool
            {
                get { return checkedpHex_Bool; }
                set { checkedpHex_Bool = value; NotifyPropertyChanged("checkedpHex_Bool"); }
            }

            public Boolean CheckedpHxNAc_Bool
            {
                get { return checkedpHxNAc_Bool; }
                set { checkedpHxNAc_Bool = value; NotifyPropertyChanged("checkedpHxNAc_Bool"); }
            }

            public Boolean CheckedpDxHex_Bool
            {
                get { return checkedpDxHex_Bool; }
                set { checkedpDxHex_Bool = value; NotifyPropertyChanged("checkedpDxHex_Bool"); }
            }

            public Boolean CheckedpPntos_Bool
            {
                get { return checkedpPntos_Bool; }
                set { checkedpPntos_Bool = value; NotifyPropertyChanged("checkedpPntos_Bool"); }
            }

            public Boolean CheckedpNuAc_Bool
            {
                get { return checkedpNuAc_Bool; }
                set { checkedpNuAc_Bool = value; NotifyPropertyChanged("checkedpNuAc_Bool"); }
            }

            public Boolean CheckedpNuGc_Bool
            {
                get { return checkedpNuGc_Bool; }
                set { checkedpNuGc_Bool = value; NotifyPropertyChanged("checkedpNuGc_Bool"); }
            }

            public Boolean CheckedpKDN_Bool
            {
                get { return checkedpKDN_Bool; }
                set { checkedpKDN_Bool = value; NotifyPropertyChanged("checkedpKDN_Bool"); }
            }

            public Boolean CheckedpHxA_Bool
            {
                get { return checkedpHxA_Bool; }
                set { checkedpHxA_Bool = value; NotifyPropertyChanged("checkedpHxA_Bool"); }
            }
        }

        /// <summary>
        /// Libraries to Upload in Parameters Variables
        /// </summary>
        public class LibraryVariables : INotifyPropertyChanged
        {
            private String chosenDefaultLibrary_String = "";//No Library Selected???????????????????????????????????????????????????????
            private String chosenCustomLibrary_String = "";

            /// <summary>
            /// updates in real-time
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            protected void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

            public String ChosenDefaultLibrary_String
            {
                get { return chosenDefaultLibrary_String; }
                set { chosenDefaultLibrary_String = value; NotifyPropertyChanged("chosenDefaultLibrary_String"); }
            }

            public String ChosenCustomLibrary_String
            {
                get { return chosenCustomLibrary_String; }
                set { chosenCustomLibrary_String = value; NotifyPropertyChanged("chosenCustomLibrary_String"); }
            }
        }

        /// <summary>
        /// Extra Science Parameter Variables
        /// </summary>
        public class ExtraScienceParameterVariables : INotifyPropertyChanged
        {
            private int numberOfChargesExtraParameter_Int = 1;
            private Double mzToleranceExtraParameter_Double = 0.0;
            private String mzToleranceTypeExtraParameter_String = "ppm";
            private String carbohydrateTypeExtraParameter_String = "Aldehyde";
            private String chargeCarryingSpeciesExtraParameter_String = "H";

            /// <summary>
            /// updates in real-time
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            protected void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

            public int NumberOfChargesExtraParameter_Int
            {
                get { return numberOfChargesExtraParameter_Int; }
                set { numberOfChargesExtraParameter_Int = (int)value; NotifyPropertyChanged("numberOfChargesExtraParameter_Int"); }
            }

            public Double MzToleranceExtraParameter_Double
            {
                get { return mzToleranceExtraParameter_Double; }
                set { mzToleranceExtraParameter_Double = (Double)value; NotifyPropertyChanged("mzToleranceExtraParameter_Double"); }
            }

            public String MzToleranceTypeExtraParameter_String
            {
                get { return mzToleranceTypeExtraParameter_String; }
                set { mzToleranceTypeExtraParameter_String = value; NotifyPropertyChanged("mzToleranceTypeExtraParameter_String"); }
            }

            public String CarbohydrateTypeExtraParameter_String
            {
                get { return carbohydrateTypeExtraParameter_String; }
                set { carbohydrateTypeExtraParameter_String = value; NotifyPropertyChanged("carbohydrateTypeExtraParameter_String"); }
            }

            public String ChargeCarryingSpeciesExtraParameter_String
            {
                get { return chargeCarryingSpeciesExtraParameter_String; }
                set { chargeCarryingSpeciesExtraParameter_String = value; NotifyPropertyChanged("chargeCarryingSpeciesExtraParameter_String"); }
            }     
        }             
                       
        /// <summary>
        /// Input/Output Data Folders and Save Location in Parameter Variables
        /// </summary>
        public class InputOutputSaveFolderVariables : INotifyPropertyChanged
        {
            private String inputDataFolder_String = "";
            private String outputDataFolder_String = "";
            private String saveLocationFolder_String = "";

            /// <summary>
            /// updates in real-time
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            protected void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

            public String InputDataFolder_String
            {
                get { return inputDataFolder_String; }
                set { inputDataFolder_String = value; NotifyPropertyChanged("inputDataFolder_String"); }
            }

            public String OutputDataFolder_String
            {
                get { return outputDataFolder_String; }
                set { outputDataFolder_String = value; NotifyPropertyChanged("outputDataFolder_String"); }
            }

            public String SaveLocationFolder_String
            {
                get { return saveLocationFolder_String; }
                set { saveLocationFolder_String = value; NotifyPropertyChanged("saveLocationFolder_String"); }
            }
        }

        /// <summary>
        /// FLags for First Initialization of Pages
        /// </summary>
        public class InitializePages : INotifyPropertyChanged
        {
            //Initialize Page Flags
            private Boolean utilities_InitializeFlag = true;
            private Boolean parameters_InitializeFlag = true;
            private Boolean glycanMaker_InitializeFlag = true;
            private Boolean omniFinder_InitializeFlag = true;
            private Boolean utilitiesOmniFinderPageDesign_InitializeFlag = true;
            private Boolean parametersOmniFinderPageDesign_InitializeFlag = true;
            private Boolean extraScienceParameters_InitializeFlag = true;
            private Boolean parametersRanges_InitializeFlag = true;
            private Boolean checkTab_InitializeFlag = false;
            private Boolean utilitiesRangesPage_InitializeFlag = true;
            private Boolean utilitiesRangesSubHomePage_InitializeFlag = true;

            /// <summary>
            /// updates in real-time
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            protected void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

            public Boolean Utilities_InitializeFlag
            {
                get { return utilities_InitializeFlag; }
                set { utilities_InitializeFlag = value; NotifyPropertyChanged("utilities_InitializeFlag"); }
            }

            public Boolean Parameters_InitializeFlag
            {
                get { return parameters_InitializeFlag; }
                set { parameters_InitializeFlag = value; NotifyPropertyChanged("parameters_InitializeFlag"); }
            }

            public Boolean GlycanMaker_InitializeFlag
            {
                get { return glycanMaker_InitializeFlag; }
                set { glycanMaker_InitializeFlag = value; NotifyPropertyChanged("glycanMaker_InitializeFlag"); }
            }

            public Boolean OmniFinder_InitializeFlag
            {
                get { return omniFinder_InitializeFlag; }
                set { omniFinder_InitializeFlag = value; NotifyPropertyChanged("omniFinder_InitializeFlag"); }
            }

            public Boolean UtilitiesOmniFinderPageDesign_InitializeFlag
            {
                get { return utilitiesOmniFinderPageDesign_InitializeFlag; }
                set { utilitiesOmniFinderPageDesign_InitializeFlag = value; NotifyPropertyChanged("omniFinderPageDesign_InitializeFlag"); }
            }

            public Boolean ParametersOmniFinderPageDesign_InitializeFlag
            {
                get { return parametersOmniFinderPageDesign_InitializeFlag; }
                set { parametersOmniFinderPageDesign_InitializeFlag = value; NotifyPropertyChanged("parametersOmniFinderPageDesign_InitializeFlag"); }
            }

            public Boolean ExtraScienceParameters_InitializeFlag
            {
                get { return extraScienceParameters_InitializeFlag; }
                set { extraScienceParameters_InitializeFlag = value; NotifyPropertyChanged("extraScienceParameters_InitializeFlag"); }
            }

            public Boolean ParametersRanges_InitializeFlag
            {
                get { return parametersRanges_InitializeFlag; }
                set { parametersRanges_InitializeFlag = value; NotifyPropertyChanged("parametersRanges_InitializeFlag"); }
            }
            public Boolean CheckTab_InitializeFlag
            {
                get { return checkTab_InitializeFlag; }
                set { checkTab_InitializeFlag = value; NotifyPropertyChanged("checkTab_InitializeFlag"); }
            }
            public Boolean UtilitiesRangesPage_InitializeFlag
            {
                get { return utilitiesRangesPage_InitializeFlag; }
                set { utilitiesRangesPage_InitializeFlag = value; NotifyPropertyChanged("utilitiesRangesPage_InitializeFlag"); }
            }
            public Boolean UtilitiesRangesSubHomePage_InitializeFlag
            {
                get { return utilitiesRangesSubHomePage_InitializeFlag; }
                set { utilitiesRangesSubHomePage_InitializeFlag = value; NotifyPropertyChanged("utilitiesRangesSubHomePage_InitializeFlag"); }
            }
        }

        /// <summary>
        /// RangesPage Variables
        /// </summary>
        public class RangesPageVariables : INotifyPropertyChanged
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

            /// <summary>
            /// updates in real-time
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            protected void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

            #region Min Methods
            //Min
            //Sugars
            public int MinHexose_Int
            {
                get { return minHexose_Int; }
                set { minHexose_Int = value; NotifyPropertyChanged("minHexose_Int"); }
            }
            public int MinHexNAc_Int
            {
                get { return minHexNAc_Int; }
                set { minHexNAc_Int = value; NotifyPropertyChanged("minHexNAc_Int"); }
            }
            public int MinDxyHex_Int
            {
                get { return minDxyHex_Int; }
                set { minDxyHex_Int = value; NotifyPropertyChanged("minDxyHex_Int"); }
            }
            public int MinPentose_Int
            {
                get { return minPentose_Int; }
                set { minPentose_Int = value; NotifyPropertyChanged("minPentose_Int"); }
            }
            public int MinNeuAc_Int
            {
                get { return minNeuAc_Int; }
                set { minNeuAc_Int = value; NotifyPropertyChanged("minNeuAc_Int"); }
            }
            public int MinNeuGc_Int
            {
                get { return minNeuGc_Int; }
                set { minNeuGc_Int = value; NotifyPropertyChanged("minNeuGc_Int"); }
            }
            public int MinKDN_Int
            {
                get { return minKDN_Int; }
                set { minKDN_Int = value; NotifyPropertyChanged("minKDN_Int"); }
            }
            public int MinHexA_Int
            {
                get { return minHexA_Int; }
                set { minHexA_Int = value; NotifyPropertyChanged("minHexA_Int"); }
            }

            //User Number
            public int MinUserUnit1_Int
            {
                get { return minUserUnit1_Int; }
                set { minUserUnit1_Int = value; NotifyPropertyChanged("minUserUnit1_Int"); }
            }
            public int MinUserUnit2_Int
            {
                get { return minUserUnit2_Int; }
                set { minUserUnit2_Int = value; NotifyPropertyChanged("minUserUnit2_Int"); }
            }
            public int MinUserUnit3_Int
            {
                get { return minUserUnit3_Int; }
                set { minUserUnit3_Int = value; NotifyPropertyChanged("minUserUnit3_Int"); }
            }
            public int MinUserUnit4_Int
            {
                get { return minUserUnit4_Int; }
                set { minUserUnit4_Int = value; NotifyPropertyChanged("minUserUnit4_Int"); }
            }
            public int MinUserUnit5_Int
            {
                get { return minUserUnit5_Int; }
                set { minUserUnit5_Int = value; NotifyPropertyChanged("minUserUnit5_Int"); }
            }
            public int MinUserUnit6_Int
            {
                get { return minUserUnit6_Int; }
                set { minUserUnit6_Int = value; NotifyPropertyChanged("minUserUnit6_Int"); }
            }
            public int MinUserUnit7_Int
            {
                get { return minUserUnit7_Int; }
                set { minUserUnit7_Int = value; NotifyPropertyChanged("minUserUnit7_Int"); }
            }
            public int MinUserUnit8_Int
            {
                get { return minUserUnit8_Int; }
                set { minUserUnit8_Int = value; NotifyPropertyChanged("minUserUnit8_Int"); }
            }
            public int MinUserUnit9_Int
            {
                get { return minUserUnit9_Int; }
                set { minUserUnit9_Int = value; NotifyPropertyChanged("minUserUnit9_Int"); }
            }
            public int MinUserUnit10_Int
            {
                get { return minUserUnit10_Int; }
                set { minUserUnit10_Int = value; NotifyPropertyChanged("minUserUnit10_Int"); }
            }

            //Amino Acids
            public int MinAla_Int
            {
                get { return minAla_Int; }
                set { minAla_Int = value; NotifyPropertyChanged("minAla_Int"); }
            }
            public int MinArg_Int
            {
                get { return minArg_Int; }
                set { minArg_Int = value; NotifyPropertyChanged("minArg_Int"); }
            }
            public int MinAsn_Int
            {
                get { return minAsn_Int; }
                set { minAsn_Int = value; NotifyPropertyChanged("minAsn_Int"); }
            }
            public int MinAsp_Int
            {
                get { return minAsp_Int; }
                set { minAsp_Int = value; NotifyPropertyChanged("minAsp_Int"); }
            }
            public int MinCys_Int
            {
                get { return minCys_Int; }
                set { minCys_Int = value; NotifyPropertyChanged("minCys_Int"); }
            }
            public int MinGln_Int
            {
                get { return minGln_Int; }
                set { minGln_Int = value; NotifyPropertyChanged("minGln_Int"); }
            }
            public int MinGlu_Int
            {
                get { return minGlu_Int; }
                set { minGlu_Int = value; NotifyPropertyChanged("minGlu_Int"); }
            }
            public int MinGly_Int
            {
                get { return minGly_Int; }
                set { minGly_Int = value; NotifyPropertyChanged("minGly_Int"); }
            }
            public int MinHis_Int
            {
                get { return minHis_Int; }
                set { minHis_Int = value; NotifyPropertyChanged("minHis_Int"); }
            }
            public int MinIle_Int
            {
                get { return minIle_Int; }
                set { minIle_Int = value; NotifyPropertyChanged("minIle_Int"); }
            }
            public int MinLeu_Int
            {
                get { return minLeu_Int; }
                set { minLeu_Int = value; NotifyPropertyChanged("minLeu_Int"); }
            }
            public int MinLys_Int
            {
                get { return minLys_Int; }
                set { minLys_Int = value; NotifyPropertyChanged("minLys_Int"); }
            }
            public int MinMet_Int
            {
                get { return minMet_Int; }
                set { minMet_Int = value; NotifyPropertyChanged("minMet_Int"); }
            }
            public int MinPhe_Int
            {
                get { return minPhe_Int; }
                set { minPhe_Int = value; NotifyPropertyChanged("minPhe_Int"); }
            }
            public int MinSer_Int
            {
                get { return minSer_Int; }
                set { minSer_Int = value; NotifyPropertyChanged("minSer_Int"); }
            }
            public int MinThr_Int
            {
                get { return minThr_Int; }
                set { minThr_Int = value; NotifyPropertyChanged("minThr_Int"); }
            }
            public int MinTrp_Int
            {
                get { return minTrp_Int; }
                set { minTrp_Int = value; NotifyPropertyChanged("minTrp_Int"); }
            }
            public int MinTyr_Int
            {
                get { return minTyr_Int; }
                set { minTyr_Int = value; NotifyPropertyChanged("minTyr_Int"); }
            }
            public int MinVal_Int
            {
                get { return minVal_Int; }
                set { minVal_Int = value; NotifyPropertyChanged("minVal_Int"); }
            }
            public int MinPro_Int
            {
                get { return minPro_Int; }
                set { minPro_Int = value; NotifyPropertyChanged("minPro_Int"); }
            }

            //Special
            public int MinNaH_Int
            {
                get { return minNaH_Int; }
                set { minNaH_Int = value; NotifyPropertyChanged("minNaH_Int"); }
            }
            public int MinCH3_Int
            {
                get { return minCH3_Int; }
                set { minCH3_Int = value; NotifyPropertyChanged("minCH3_Int"); }
            }
            public int MinSO3_Int
            {
                get { return minSO3_Int; }
                set { minSO3_Int = value; NotifyPropertyChanged("minSO3_Int"); }
            }
            public int MinOAcetyl_Int
            {
                get { return minOAcetyl_Int; }
                set { minOAcetyl_Int = value; NotifyPropertyChanged("minOAcetyl_Int"); }
            }

            //Permethyl  
            public int MinpHex_Int
            {
                get { return minpHex_Int; }
                set { minpHex_Int = value; NotifyPropertyChanged("minpHex_Int"); }
            }
            public int MinpHxNAc_Int
            {
                get { return minpHxNAc_Int; }
                set { minpHxNAc_Int = value; NotifyPropertyChanged("minpHxNAc_Int"); }
            }
            public int MinpDxHex_Int
            {
                get { return minpDxHex_Int; }
                set { minpDxHex_Int = value; NotifyPropertyChanged("minpDxHex_Int"); }
            }
            public int MinpPntos_Int
            {
                get { return minpPntos_Int; }
                set { minpPntos_Int = value; NotifyPropertyChanged("minpPntos_Int"); }
            }
            public int MinpNuAc_Int
            {
                get { return minpNuAc_Int; }
                set { minpNuAc_Int = value; NotifyPropertyChanged("minpNuAc_Int"); }
            }
            public int MinpNuGc_Int
            {
                get { return minpNuGc_Int; }
                set { minpNuGc_Int = value; NotifyPropertyChanged("minpNuGc_Int"); }
            }
            public int MinpKDN_Int
            {
                get { return minpKDN_Int; }
                set { minpKDN_Int = value; NotifyPropertyChanged("minpKDN_Int"); }
            }
            public int MinpHxA_Int
            {
                get { return minpHxA_Int; }
                set { minpHxA_Int = value; NotifyPropertyChanged("minpHxA_Int"); }
            }
            #endregion

            #region Max Methods
            //Max
            //Sugars
            public int MaxHexose_Int
            {
                get { return maxHexose_Int; }
                set { maxHexose_Int = value; NotifyPropertyChanged("maxHexose_Int"); }
            }
            public int MaxHexNAc_Int
            {
                get { return maxHexNAc_Int; }
                set { maxHexNAc_Int = value; NotifyPropertyChanged("maxHexNAc_Int"); }
            }
            public int MaxDxyHex_Int
            {
                get { return maxDxyHex_Int; }
                set { maxDxyHex_Int = value; NotifyPropertyChanged("maxDxyHex_Int"); }
            }
            public int MaxPentose_Int
            {
                get { return maxPentose_Int; }
                set { maxPentose_Int = value; NotifyPropertyChanged("maxPentose_Int"); }
            }
            public int MaxNeuAc_Int
            {
                get { return maxNeuAc_Int; }
                set { maxNeuAc_Int = value; NotifyPropertyChanged("maxNeuAc_Int"); }
            }
            public int MaxNeuGc_Int
            {
                get { return maxNeuGc_Int; }
                set { maxNeuGc_Int = value; NotifyPropertyChanged("maxNeuGc_Int"); }
            }
            public int MaxKDN_Int
            {
                get { return maxKDN_Int; }
                set { maxKDN_Int = value; NotifyPropertyChanged("maxKDN_Int"); }
            }
            public int MaxHexA_Int
            {
                get { return maxHexA_Int; }
                set { maxHexA_Int = value; NotifyPropertyChanged("maxHexA_Int"); }
            }

            //User Number
            public int MaxUserUnit1_Int
            {
                get { return maxUserUnit1_Int; }
                set { maxUserUnit1_Int = value; NotifyPropertyChanged("maxUserUnit1_Int"); }
            }
            public int MaxUserUnit2_Int
            {
                get { return maxUserUnit2_Int; }
                set { maxUserUnit2_Int = value; NotifyPropertyChanged("maxUserUnit2_Int"); }
            }
            public int MaxUserUnit3_Int
            {
                get { return maxUserUnit3_Int; }
                set { maxUserUnit3_Int = value; NotifyPropertyChanged("maxUserUnit3_Int"); }
            }
            public int MaxUserUnit4_Int
            {
                get { return maxUserUnit4_Int; }
                set { maxUserUnit4_Int = value; NotifyPropertyChanged("maxUserUnit4_Int"); }
            }
            public int MaxUserUnit5_Int
            {
                get { return maxUserUnit5_Int; }
                set { maxUserUnit5_Int = value; NotifyPropertyChanged("maxUserUnit5_Int"); }
            }
            public int MaxUserUnit6_Int
            {
                get { return maxUserUnit6_Int; }
                set { maxUserUnit6_Int = value; NotifyPropertyChanged("maxUserUnit6_Int"); }
            }
            public int MaxUserUnit7_Int
            {
                get { return maxUserUnit7_Int; }
                set { maxUserUnit7_Int = value; NotifyPropertyChanged("maxUserUnit7_Int"); }
            }
            public int MaxUserUnit8_Int
            {
                get { return maxUserUnit8_Int; }
                set { maxUserUnit8_Int = value; NotifyPropertyChanged("maxUserUnit8_Int"); }
            }
            public int MaxUserUnit9_Int
            {
                get { return maxUserUnit9_Int; }
                set { maxUserUnit9_Int = value; NotifyPropertyChanged("maxUserUnit9_Int"); }
            }
            public int MaxUserUnit10_Int
            {
                get { return maxUserUnit10_Int; }
                set { maxUserUnit10_Int = value; NotifyPropertyChanged("maxUserUnit10_Int"); }
            }

            //Amaxo Acids
            public int MaxAla_Int
            {
                get { return maxAla_Int; }
                set { maxAla_Int = value; NotifyPropertyChanged("maxAla_Int"); }
            }
            public int MaxArg_Int
            {
                get { return maxArg_Int; }
                set { maxArg_Int = value; NotifyPropertyChanged("maxArg_Int"); }
            }
            public int MaxAsn_Int
            {
                get { return maxAsn_Int; }
                set { maxAsn_Int = value; NotifyPropertyChanged("maxAsn_Int"); }
            }
            public int MaxAsp_Int
            {
                get { return maxAsp_Int; }
                set { maxAsp_Int = value; NotifyPropertyChanged("maxAsp_Int"); }
            }
            public int MaxCys_Int
            {
                get { return maxCys_Int; }
                set { maxCys_Int = value; NotifyPropertyChanged("maxCys_Int"); }
            }
            public int MaxGln_Int
            {
                get { return maxGln_Int; }
                set { maxGln_Int = value; NotifyPropertyChanged("maxGln_Int"); }
            }
            public int MaxGlu_Int
            {
                get { return maxGlu_Int; }
                set { maxGlu_Int = value; NotifyPropertyChanged("maxGlu_Int"); }
            }
            public int MaxGly_Int
            {
                get { return maxGly_Int; }
                set { maxGly_Int = value; NotifyPropertyChanged("maxGly_Int"); }
            }
            public int MaxHis_Int
            {
                get { return maxHis_Int; }
                set { maxHis_Int = value; NotifyPropertyChanged("maxHis_Int"); }
            }
            public int MaxIle_Int
            {
                get { return maxIle_Int; }
                set { maxIle_Int = value; NotifyPropertyChanged("maxIle_Int"); }
            }
            public int MaxLeu_Int
            {
                get { return maxLeu_Int; }
                set { maxLeu_Int = value; NotifyPropertyChanged("maxLeu_Int"); }
            }
            public int MaxLys_Int
            {
                get { return maxLys_Int; }
                set { maxLys_Int = value; NotifyPropertyChanged("maxLys_Int"); }
            }
            public int MaxMet_Int
            {
                get { return maxMet_Int; }
                set { maxMet_Int = value; NotifyPropertyChanged("maxMet_Int"); }
            }
            public int MaxPhe_Int
            {
                get { return maxPhe_Int; }
                set { maxPhe_Int = value; NotifyPropertyChanged("maxPhe_Int"); }
            }
            public int MaxSer_Int
            {
                get { return maxSer_Int; }
                set { maxSer_Int = value; NotifyPropertyChanged("maxSer_Int"); }
            }
            public int MaxThr_Int
            {
                get { return maxThr_Int; }
                set { maxThr_Int = value; NotifyPropertyChanged("maxThr_Int"); }
            }
            public int MaxTrp_Int
            {
                get { return maxTrp_Int; }
                set { maxTrp_Int = value; NotifyPropertyChanged("maxTrp_Int"); }
            }
            public int MaxTyr_Int
            {
                get { return maxTyr_Int; }
                set { maxTyr_Int = value; NotifyPropertyChanged("maxTyr_Int"); }
            }
            public int MaxVal_Int
            {
                get { return maxVal_Int; }
                set { maxVal_Int = value; NotifyPropertyChanged("maxVal_Int"); }
            }
            public int MaxPro_Int
            {
                get { return maxPro_Int; }
                set { maxPro_Int = value; NotifyPropertyChanged("maxPro_Int"); }
            }

            //Special
            public int MaxNaH_Int
            {
                get { return maxNaH_Int; }
                set { maxNaH_Int = value; NotifyPropertyChanged("maxNaH_Int"); }
            }
            public int MaxCH3_Int
            {
                get { return maxCH3_Int; }
                set { maxCH3_Int = value; NotifyPropertyChanged("maxCH3_Int"); }
            }
            public int MaxSO3_Int
            {
                get { return maxSO3_Int; }
                set { maxSO3_Int = value; NotifyPropertyChanged("maxSO3_Int"); }
            }
            public int MaxOAcetyl_Int
            {
                get { return maxOAcetyl_Int; }
                set { maxOAcetyl_Int = value; NotifyPropertyChanged("maxOAcetyl_Int"); }
            }

            //Permethyl  
            public int MaxpHex_Int
            {
                get { return maxpHex_Int; }
                set { maxpHex_Int = value; NotifyPropertyChanged("maxpHex_Int"); }
            }
            public int MaxpHxNAc_Int
            {
                get { return maxpHxNAc_Int; }
                set { maxpHxNAc_Int = value; NotifyPropertyChanged("maxpHxNAc_Int"); }
            }
            public int MaxpDxHex_Int
            {
                get { return maxpDxHex_Int; }
                set { maxpDxHex_Int = value; NotifyPropertyChanged("maxpDxHex_Int"); }
            }
            public int MaxpPntos_Int
            {
                get { return maxpPntos_Int; }
                set { maxpPntos_Int = value; NotifyPropertyChanged("maxpPntos_Int"); }
            }
            public int MaxpNuAc_Int
            {
                get { return maxpNuAc_Int; }
                set { maxpNuAc_Int = value; NotifyPropertyChanged("maxpNuAc_Int"); }
            }
            public int MaxpNuGc_Int
            {
                get { return maxpNuGc_Int; }
                set { maxpNuGc_Int = value; NotifyPropertyChanged("maxpNuGc_Int"); }
            }
            public int MaxpKDN_Int
            {
                get { return maxpKDN_Int; }
                set { maxpKDN_Int = value; NotifyPropertyChanged("maxpKDN_Int"); }
            }
            public int MaxpHxA_Int
            {
                get { return maxpHxA_Int; }
                set { maxpHxA_Int = value; NotifyPropertyChanged("maxpHxA_Int"); }
            }
            #endregion
        }
    }
}