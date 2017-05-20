using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FieldOffice
{
    public class Datasets
    {
        public static void SetData(out List<string> parametersFilesglyQIQconsoleOperatingParameters_WithEnding, out List<string> dataFilesNoEnding, out List<string> parametersFilesiQParameterFile_WithEnding, out string fieldOfficeSeries)
        {
            parametersFilesglyQIQconsoleOperatingParameters_WithEnding = new List<string>();
            dataFilesNoEnding = new List<string>();
            parametersFilesiQParameterFile_WithEnding = new List<string>();
            fieldOfficeSeries = "FO";

            bool isEcoli = false;
            if (isEcoli)
            {
                fieldOfficeSeries = "F_Ecoli_V10_PsaLac";


                dataFilesNoEnding.Add("ChrR_Ox_Dig_Control_24Sep09_Draco_09-05-04_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_Velos_H000.txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add("Default.txt");
                dataFilesNoEnding.Add("Peptide_Merc_08_2Feb11_Sphinx_11-01-17_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_Velos_H000.txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add("Default.txt");

                //big one
                //dataFilesNoEnding.Add("Peptide_Merc_06_2Feb11_Sphinx_11-01-17_1");


                
                //parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add("GlyQIQ_Params_Velos4_Ecoli_L10PSA000.txt");

                
                //dataFilesNoEnding.Add("Peptide_Merc_08_2Feb11_Sphinx_11-01-17_1");
            }

            bool isSmallFile = false;
            if (isSmallFile)
            {
                dataFilesNoEnding.Add("D60A_1");
                parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_Velos_H.txt");
                parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add("GlyQIQ_Params_Velos_SN129_L10PSA.txt");
            }

            //Environment.Exit(0);

            bool isStandardComparision = true;
            if (isStandardComparision)
            {

                fieldOfficeSeries = "FStd_V12_NPSA";
                //fieldOfficeSeries = "F_Std_V9_NoLac";
                //fieldOfficeSeries = "F_Std_V9_Psa";
                //fieldOfficeSeries = "F_Std_V9_PsaLac";
                //fieldOfficeSeries = "F_Std_V10h_PsaLac";
                //fieldOfficeSeries = "F_Std_V10s_HMF";
                //fieldOfficeSeries = "F_TestingMedium0384D2";
                //fieldOfficeSeries = "F_F_Deception2";
                //fieldOfficeSeries = "F_Clay_V10_HM_Deception1b";
                //fieldOfficeSeries = "F_Std_V9_Local";//Submitted to GlyQ-IQ

                bool withVelosOrigional = false;
                bool withVelos4 = false;//for GlyQ-IQ paper part 1 
                bool withSPINNelu = true; //glyqIQ Part 2
                bool withSPINHoly = false;//glyqIQ Part 2
                bool exactiveESI138 = false;
                bool isSPINSerum = false;
                if (withVelosOrigional)
                {
                    dataFilesNoEnding.Add("V_SN129_1");
                    parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_Velos_H.txt");
                    parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add("GlyQIQ_Params_Velos_SN129_L10PSA.txt");
                }
                
                if (withVelos4)
                {
                    fieldOfficeSeries = "F_HSum9_s5";
                    //slightly better
                    //dataFilesNoEnding.Add("ESI_SN136_21Dec13_3X_230nL_C15_2530_325C_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_Velos4_136_H.txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add("GlyQIQ_Params_Velos4_SN136_L10PSA.txt");
                    //dataFilesNoEnding.Add("ESI_SN136_21Dec_C15_2530_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_Velos4_136_H.txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add("GlyQIQ_Params_Velos4_SN136_L10PSA.txt");
                    //matched to spin
                    //dataFilesNoEnding.Add("ESI_SN138_21Dec13_3X_230nL_C15_2530_325C_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_Velos4_138_H.txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add("GlyQIQ_Params_Velos4_SN138_L10PSA.txt");
                    dataFilesNoEnding.Add("ESI_SN138_21Dec_C15_2530_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_Velos4_138_H.txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add("GlyQIQ_Params_Velos4_SN138_L10PSA.txt");
                }
                
                if (withSPINNelu)
                {
                    dataFilesNoEnding.Add("S_SN129_1");
                    //parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_Spin_HAVG000.txt");//nope +1.5
                    parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_Spin_HAVG.txt");//yup +0.25
                    //parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_Spin_HAVG002.txt");//nope -1
                    parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add("Default.txt");
                    //parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add("GlyQIQ_Params_SPIN_SN129_L10PSA.txt");
                }
                
                if (withSPINHoly)
                {

                    dataFilesNoEnding.Add("SPIN_SN138_16Dec13_C15_1");//dataFilesNoEnding.Add("SPIN_SN138_16Dec13_40uL_230nL_140C_300mL_22T_C15_1");
                    parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_Spin_HAVG_HolyCellC127.txt");
                    parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add("Default.txt");
                    //parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add("GlyQIQ_Params_SPIN_SN138_L10PSAC127.txt");
                }

                if (exactiveESI138)
                {
                    string defaultiQParameterFile_WithEnding = "FragmentedParameters_ESI_Exactive_003.txt";
                    string defaultglyQIQconsoleOperatingParameters_WithEnding = "Default.txt";
                    //250C
                    dataFilesNoEnding.Add("ESI_SN138_25Nov13_3X_230nL_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);

                }

                if (isSPINSerum)
                {

                    string defaultiQParameterFile_WithEnding = "FragmentedParameters_Spin_HAVG_HolyCellC127.txt";
                    string defaultglyQIQconsoleOperatingParameters_WithEnding = "Default.txt";

                    //dataFilesNoEnding.Add("SPIN_SN136_07Dec13_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SN136_07Dec13_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SN137_07Dec13_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SN137_07Dec13_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SN138_16Dec13_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SN139_05Dec13_120C_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SN139_07Dec13_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SN139_07Dec13_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_SN139_16Dec13_R1_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_SN139_16Dec13_R2_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SN140_07Dec13_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SN140_07Dec13_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_SN140_16Dec13_R1_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_SN140_16Dec13_R2_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_SN140_16Dec13_R3_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);

                }
            }

            bool isAnt = false;
            if (isAnt)
            {
                fieldOfficeSeries = "F_AntV4";


                string defaultiQParameterFile_WithEnding = "FragmentedParameters_Velos_H_Ant.txt";
                string defaultglyQIQconsoleOperatingParameters_WithEnding = "GlyQIQ_Params_Velos_SN129_LAnt.txt";
                //string defaultglyQIQconsoleOperatingParameters_WithEnding = "GlyQIQ_Params_Velos_SN129_L10PSA.txt";

                dataFilesNoEnding.Add("Gly09_Velos3_Jaguar_200nL_C12_AntB1_3X_25Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                dataFilesNoEnding.Add("Gly09_Velos3_Jaguar_200nL_C12_AntB2_3X_25Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                dataFilesNoEnding.Add("Gly09_Velos3_Jaguar_200nL_C12_AntB3_3X_25Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                dataFilesNoEnding.Add("Gly09_Velos3_Jaguar_200nL_C12_AntM4_3X_25Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);

                dataFilesNoEnding.Add("Gly09_Velos3_Jaguar_200nL_C13_AntM1_3X_25Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                dataFilesNoEnding.Add("Gly09_Velos3_Jaguar_200nL_C13_AntM2_3X_26Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                dataFilesNoEnding.Add("Gly09_Velos3_Jaguar_200nL_C13_AntM3_3X_25Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                dataFilesNoEnding.Add("Gly09_Velos3_Jaguar_200nL_C13_AntM5_3X_25Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                dataFilesNoEnding.Add("Gly09_Velos3_Jaguar_200nL_C13_AntT1_3X_25Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                dataFilesNoEnding.Add("Gly09_Velos3_Jaguar_200nL_C13_AntT2_3X_25Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                dataFilesNoEnding.Add("Gly09_Velos3_Jaguar_200nL_C13_AntT3_3X_25Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                
                //removed as bad
                //dataFilesNoEnding.Add("Gly09_Velos3_Jaguar_200nL_C12_AntB4_3X_27Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("Gly09_Velos3_Jaguar_200nL_C12_AntM2_3X_25Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                
            }

             bool isColominicAcid = false;// and VWF
            if (isColominicAcid)
            {
                //fieldOfficeSeries = "F_CA_V7_000v3";

                bool velosESI = false;
                bool exactiveSPIN = false;
                bool exactiveSPIN2 = false;
                bool exactiveESI = false;
                bool velosMatch = true;
                string defaultiQParameterFile_WithEnding;
                string defaultglyQIQconsoleOperatingParameters_WithEnding;

                //Velos ESI
                if (velosESI)
                {
                    fieldOfficeSeries = "F_CA_V9N004v1";
                    defaultiQParameterFile_WithEnding = "FragmentedParameters_Velos4_CA_H_N004.txt";
                    defaultglyQIQconsoleOperatingParameters_WithEnding = "GlyQIQ_Params_Velos4_CA_L10CA_N004.txt";

                    //bad//dataFilesNoEnding.Add("ESI_ColominicAcid_21Dec13_3X_230nL_C16_2530_325C_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("ESI_ColominicAcid_22Dec13_3X_230nL_C16_2530_325C_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("ESI_CAcid_22Dec13_Velos_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                }

                //Exactive SPIN
                if (exactiveSPIN)
                {
                    fieldOfficeSeries = "F_CA_V11P007vS100D2";
                    defaultiQParameterFile_WithEnding = "FragmentedParameters_SPIN_Exactive_CA007.txt";
                    //defaultglyQIQconsoleOperatingParameters_WithEnding = "GlyQIQ_Params_Exact_SPIN_CA_L10CA007.txt";
                    defaultglyQIQconsoleOperatingParameters_WithEnding = "Default.txt";

                    //dataFilesNoEnding.Add("SPIN_CAcid660uM_18Dec13_40uL_230nL30_140C_300mL_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_CAcid_18Dec_L30_140C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_CAcid660uM_19Dec13_L28_140C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    
                    
                    //dataFilesNoEnding.Add("SPIN_CAcid660uM_18Dec13_40uL_230nL_140C_300mL_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                }


                if (exactiveESI)
                {
                    fieldOfficeSeries = "F_CA_V10P006vS100V1";
                    defaultiQParameterFile_WithEnding = "FragmentedParameters_ESI_Exactive_CA006.txt";
                    //defaultglyQIQconsoleOperatingParameters_WithEnding = "GlyQIQ_Params_Exact_ESI_CA_L10CA006.txt";
                    defaultglyQIQconsoleOperatingParameters_WithEnding = "Default.txt";

                    //dataFilesNoEnding.Add("ESI_CAcid_L28d_150C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("ESI_CAcid_L28d_200C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("ESI_CAcid_L28d_250C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("ESI_CAcid_L28d_300C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    
                    
                }

                if (exactiveSPIN2)
                {
                    string cal = "000";
                    fieldOfficeSeries = "F_CA_V12h" + cal + "vS100DT";
                    //defaultiQParameterFile_WithEnding = "FragmentedParameters_ESI_Exactive_CA" + cal + @".txt";
                    defaultiQParameterFile_WithEnding = "FragmentedParameters_SPIN_Exactive_CA" + cal + @".txt";
                    defaultglyQIQconsoleOperatingParameters_WithEnding = "Default.txt";

                    //dataFilesNoEnding.Add("ESI_05Mar14_150C_055V_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("ESI_05Mar14_150C_190V_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    
                    dataFilesNoEnding.Add("ESI_05Mar14_200C_055V_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("ESI_05Mar14_200C_190_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    
                    //dataFilesNoEnding.Add("ESI_05Mar14_300C_055V_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("ESI_05Mar14_300C_190V_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);

                    //dataFilesNoEnding.Add("ESI_05Mar14_300C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);



                   
                }

                if (velosMatch)
                {
                    string cal2 = "_N004";
                    fieldOfficeSeries = "F_V12" + cal2;
                    //defaultiQParameterFile_WithEnding = "FragmentedParameters_ESI_Exactive_CA" + cal2 + @".txt";
                    defaultiQParameterFile_WithEnding = "FragmentedParameters_SPIN_Velos_SN" + cal2 + @".txt";
                    defaultglyQIQconsoleOperatingParameters_WithEnding = "Default.txt";

                    //group1 5ppm
                    //string calA = "004";
                    //dataFilesNoEnding.Add("SPINV_SN142_14Mar14_60uL_230nL_120C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calA + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //string calB = "006";
                    //dataFilesNoEnding.Add("SPINV_SN143_14Mar14_60uL_230nL_120C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calB + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //string calC = "006";
                    //dataFilesNoEnding.Add("SPINV_SN148_14Mar14_60uL_230nL_120C_C15_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calC + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    
                    //group2
                    //string calD = "N005";
                    //dataFilesNoEnding.Add("ESI_SN142_12Mar14_60uL_230nL_150C_190V_C16_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calD + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);

                    //group3
                    //string calE = "N001";
                    //dataFilesNoEnding.Add("SPINV_SN144_16Mar14_60uL_230nL_120C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_SN145_16Mar14_60uL_230nL_120C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_SN150_16Mar14_60uL_230nL_120C_C15_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_SN149_16Mar14_60uL_230nL_120C_C15_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_SN146_16Mar14_60uL_230nL_120C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_SN147_16Mar14_60uL_230nL_120C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_SN142_17Mar14_60uL_230nL_120C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_SN144_17Mar14_60uL_230nL_120C_C15_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_SN144_17Mar14_60uL_230nL_120C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_SN151_17Mar14_60uL_230nL_120C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);

                    string calE = "N001";
                    string calF = "003";
                    string calG = "N004";
                    string calH = "001";
                    //dataFilesNoEnding.Add("SPINV_VWF04A_17Mar14_25uL_230nL_120C_C15_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_VWF10A_17Mar14_25uL_230nL_120C_C15_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calF + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);

                    
                    //1 N001
                    //dataFilesNoEnding.Add("SPINV_VWF10A_18Mar14_25uL_230nL_120C_C15_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    
                    //2 N001
                    //dataFilesNoEnding.Add("SPINV_VWF04A_18Mar14_25uL_230nL_120C_C15_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                  
                    //3 N004
                    //dataFilesNoEnding.Add("SPINV_VWF04B_19Mar14_10uL_230nL_120C_C15_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calG + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);

                    //4 N004
                    //dataFilesNoEnding.Add("SPINV_VWF10B_19Mar14_10uL_230nL_120C_C15_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calG + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);


                    //dataFilesNoEnding.Add("SPINV_Blank03_18Mar14_60uL_230nL_120C_C15_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calG + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_Blank04_19Mar14_60uL_230nL_120C_C15_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calG + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_SN144_18Mar14_60uL_230nL_120C_C15_1_Blank"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calG + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);



                    //dataFilesNoEnding.Add("SPINV_SN144_18Mar14_60uL_230nL_120C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_SN146_18Mar14_60uL_230nL_120C_C16_2_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_SN146_19Mar14_60uL_230nL_120C_C16_3_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_SN144_18Mar14_60uL_230nL_120C_C16_1a"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);

                    //dataFilesNoEnding.Add("Velos_SN150_21Mar14_60uL_230nL_120C_C16_3_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calG + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_SN150_20Mar14_60uL_230nL_120C_C16_3_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calG + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    
                   //done
                    //dataFilesNoEnding.Add("SPINV_Cell10-3_19Mar14_60uL_230nL_120C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calG + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_SN146_16Mar14_60uL_230nL_120C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calG + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPINV_SN144_16Mar14_60uL_230nL_120C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calG + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);

                    dataFilesNoEnding.Add("SPINV_SN142_17Mar14_60uL_230nL_120C_C16_1"); parametersFilesiQParameterFile_WithEnding.Add("FragmentedParameters_SPIN_Velos_SN" + calE + @".txt"); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    
                    
                }
                
            }

            bool isDiabetes = false;
            if (isDiabetes)
            {
                string cal2 = "_LxCa";
                fieldOfficeSeries = "F_DB" + cal2;

                //iQParameterFile_WithEnding = "FragmentedParameters_Velos_DH.txt";
                //string glyQIQconsoleOperatingParameters_WithEnding = "GlyQIQ_Diabetes_Parameters_PICFS_Velos_SN129_L10PSA.txt";
                //glyQIQconsoleOperatingParameters_WithEnding = "GlyQIQ_Params_Velos_SNX_L10PSA_DH.txt";

                string defaultiQParameterFile_WithEnding = "FragmentedParameters_Velos_DH.txt";
                string defaultglyQIQconsoleOperatingParameters_WithEnding = "GlyQIQ_Params_Velos_SNX_L10PSA_DH.txt";

                //dataFilesNoEnding.Add("C14_DB02_30Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                dataFilesNoEnding.Add("C14_DB04_30Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("C14_DB06_31Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("C14_DB08_31Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("C14_DB10_31Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);

                //dataFilesNoEnding.Add("C14_DB12_01Jan13_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("C14_DB14_01Jan13_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("C14_DB16_01Jan13_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("C14_DB18_01Jan13_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("C14_DB20_01Jan13_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);

                //dataFilesNoEnding.Add("C15_DB01_30Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("C15_DB03_30Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("C15_DB05_30Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("C15_DB07_31Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("C15_DB09_31Dec12_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);

                //dataFilesNoEnding.Add("C15_DB11_01Jan13_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("C15_DB13_01Jan13_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("C15_DB15_01Jan13_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("C15_DB17_01Jan13_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("C15_DB19_01Jan13_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);

                //dataFilesNoEnding.Add("SN111SN114_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("SN112SN115_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("SN113SN116_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("SN117SN120_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("SN118SN121_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("SN119SN122_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
            
            }


            bool isCellLines = false;
            if (isCellLines)
            {
               
                //string defaultglyQIQconsoleOperatingParameters_WithEnding = "GlyQIQ_Params_ESI_Exactive_L10.txt";
                string defaultiQParameterFile_WithEnding = "FragmentedParameters_ESI_Exactive.txt";
                string defaultglyQIQconsoleOperatingParameters_WithEnding = "Default.txt";
                fieldOfficeSeries = "F_Cell_PO4";

                bool cal1 = false;
                if(cal1)
                {
                    //defaultglyQIQconsoleOperatingParameters_WithEnding = "GlyQIQ_Params_SPIN_Exactive_L10_C1.txt";
                    defaultiQParameterFile_WithEnding = "FragmentedParameters_SPIN_Exactive_C015.txt";
                    fieldOfficeSeries = "F_Cell015_HM_V1";//015 is best

                    //dataFilesNoEnding.Add("SPIN_SN136_07Dec13_40uL_230nL_140C_800mL_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SN136_07Dec13_40uL_230nL_140C_800mL_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SN137_07Dec13_40uL_230nL_140C_800mL_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SN137_07Dec13_40uL_230nL_140C_800mL_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SN139_07Dec13_40uL_230nL_140C_800ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SN139_07Dec13_40uL_230nL_140C_800ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SN140_07Dec13_40uL_230nL_140C_800mL_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SN140_07Dec13_40uL_230nL_140C_800mL_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);

                    //dataFilesNoEnding.Add("SPIN_Cell_Norm-1_07Dec13_160uL_230nL_140C_800ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //    //dataFilesNoEnding.Add("SPIN_Cell_ACN-0A_08Dec13_160uL_230nL_140C_800ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //    //dataFilesNoEnding.Add("SPIN_Cell_ACN-0B_08Dec13_160uL_230nL_140C_800ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_Norm-2_07Dec13_160uL_230nL_140C_800ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);             
                    //dataFilesNoEnding.Add("SPIN_Cell_30mM-1_08Dec13_160uL_230nL_140C_800ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_50mM-1_08Dec13_160uL_230nL_140C_800ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_00mM-1_08Dec13_160uL_230nL_140C_800ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_05mM-1_08Dec13_160uL_230nL_140C_800ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_10mM-1_08Dec13_160uL_230nL_140C_800ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_Norm-2_08Dec13_160uL_230nL_140C_800ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                                          
                    

                }

                bool cal2 = false;
                if (cal2)
                {
                    //defaultglyQIQconsoleOperatingParameters_WithEnding = "GlyQIQ_Params_SPIN_Exactive_L10_C22.txt";
                    defaultiQParameterFile_WithEnding = "FragmentedParameters_SPIN_Exactive_C22.txt";
                    fieldOfficeSeries = "F_CellLinesC22";

                    dataFilesNoEnding.Add("SPIN_Cell_00mM-1_09Dec13_160uL_230nL_140C_800ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_Cell_05mM-1_09Dec13_160uL_230nL_140C_800ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_Cell_10mM-1_09Dec13_160uL_230nL_140C_800ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_Cell_30mM-1_09Dec13_160uL_230nL_140C_800ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_Cell_50mM-1_09Dec13_160uL_230nL_140C_800ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add(""); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                
                }

                
                
                    
                bool cal3 = true;
                if (cal3)
                {
                   // defaultglyQIQconsoleOperatingParameters_WithEnding = "GlyQIQ_Params_SPIN_Exactive_L10_C127.txt";
                    defaultiQParameterFile_WithEnding = "FragmentedParameters_Spin_HAVG_HolyCellC127.txt";
                    fieldOfficeSeries = "F_Cell0127_PO4";//015 is best

                    #region long file names
                    //dataFilesNoEnding.Add("SPIN_Cell_00mM-1_15Dec13_160uL_230nL_140C_300ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_00mM-2_12Dec13_160uL_230nL_140C_300ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_00mM-3_13Dec13_160uL_230nL_140C_300ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_00mM-4_14Dec13_160uL_230nL_140C_300ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_00mM-5_15Dec13_160uL_230nL_140C_300ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_05mM-1_15Dec13_160uL_230nL_140C_300ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_05mM-2_12Dec13_160uL_230nL_140C_300ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_05mM-3_13Dec13_160uL_230nL_140C_300ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_05mM-4_14Dec13_160uL_230nL_140C_300ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_05mM-5_15Dec13_160uL_230nL_140C_300ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_10mM-1_15Dec13_160uL_230nL_140C_300ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_10mM-2_12Dec13_160uL_230nL_140C_300ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_10mM-3_13Dec13_160uL_230nL_140C_300ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_10mM-4_14Dec13_160uL_230nL_140C_300ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_10mM-5_15Dec13_160uL_230nL_140C_300ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_30mM-1_15Dec13_160uL_230nL_140C_300ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_30mM-2_12Dec13_160uL_230nL_140C_300ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_30mM-3_13Dec13_160uL_230nL_140C_300ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_30mM-4_14Dec13_160uL_230nL_140C_300ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_30mM-5_15Dec13_160uL_230nL_140C_300ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_50mM-1_15Dec13_160uL_230nL_140C_300ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_50mM-2_12Dec13_160uL_230nL_140C_300ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_50mM-3_13Dec13_160uL_230nL_140C_300ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_50mM-4_14Dec13_160uL_230nL_140C_300ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_50mM-5_15Dec13_160uL_230nL_140C_300ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_Norm-2_07Dec13_160uL_230nL_140C_800ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_Norm-2_08Dec13_160uL_230nL_140C_800ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_Norm-2_12Dec13_160uL_230nL_140C_300ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_Norm-2_14Dec13_160uL_230nL_140C_300ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_Norm-2_15Dec13_160uL_230nL_140C_300ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_00mM-1_08Dec13_160uL_230nL_140C_800ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_05mM-1_09Dec13_160uL_230nL_140C_800ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_10mM-1_09Dec13_160uL_230nL_140C_800ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_30mM-1_09Dec13_160uL_230nL_140C_800ml_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_Cell_50mM-1_09Dec13_160uL_230nL_140C_800ml_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    #endregion

                    //dataFilesNoEnding.Add("Cell_00mM_1_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_00mM_2_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_00mM_3_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_00mM_4_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_00mM_5_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_05mM_1_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_05mM_2_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_05mM_3_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_05mM_4_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_05mM_5_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_10mM_1_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_10mM_2_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_10mM_3_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_10mM_4_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_10mM_5_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_30mM_1_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_30mM_2_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_30mM_3_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_30mM_4_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_30mM_5_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_50mM_1_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_50mM_2_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_50mM_3_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_50mM_4_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_50mM_5_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_Norm2_12_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_Norm2_14_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("Cell_Norm2_15_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    
                }

                bool cal4 = false;
                if (cal4)
                {
                    //defaultglyQIQconsoleOperatingParameters_WithEnding = "GlyQIQ_Params_SPIN_Exactive_006.txt";
                    defaultiQParameterFile_WithEnding = "FragmentedParameters_SPIN_Exactive_006.txt";
                    fieldOfficeSeries = "F_Cell_V10_006";

                    //dataFilesNoEnding.Add("SPIN_HCC38_17Dec13_40uL_230nL_140C_300mL_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_HCC38_17Dec13_40uL_230nL_140C_300mL_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_MCF7_17Dec13_40uL_230nL_140C_300mL_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_MCF7_17Dec13_40uL_230nL_140C_300mL_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SKBR3_17Dec13_40uL_230nL_140C_300mL_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SKBR3_17Dec13_40uL_230nL_140C_300mL_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SOM149_17Dec13_40uL_230nL_140C_300mL_22T_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    //dataFilesNoEnding.Add("SPIN_SOM149_17Dec13_40uL_230nL_140C_300mL_22T_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);

                    dataFilesNoEnding.Add("SPIN_HCC38_17Dec_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_HCC38_17Dec_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_MCF7_17Dec_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_MCF7_17Dec_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_SKBR3_17Dec_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_SKBR3_17Dec_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_SOM149_17Dec_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                    dataFilesNoEnding.Add("SPIN_SOM149_17Dec_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                }

                //dataFilesNoEnding.Add("ESI_Cell_Norm-1_26Nov13_40uL_230nL_C14_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("ESI_Cell_Norm-1_01Dec13_80uL_230nL_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("ESI_SN138_TimingFromVINJA_C16_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("ESI_SN138_28Nov13_3X_230nL_C14_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);
                //dataFilesNoEnding.Add("ESI_SN138_25Nov13_3X_230nL_C15_1"); parametersFilesiQParameterFile_WithEnding.Add(defaultiQParameterFile_WithEnding); parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(defaultglyQIQconsoleOperatingParameters_WithEnding);


            }

            if (dataFilesNoEnding.Count == 0)
            {
                Console.WriteLine("Missing fileName");
                Console.ReadKey();
            }

            if (parametersFilesiQParameterFile_WithEnding.Count == 0)
            {
                Console.WriteLine("Missing iQParameterFile");
                Console.ReadKey();
            }

            if (parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Count == 0)
            {
                Console.WriteLine("Missing glyQIQconsoleOperatingParameters");
                Console.ReadKey();
            }

            if (dataFilesNoEnding.Count != parametersFilesiQParameterFile_WithEnding.Count || dataFilesNoEnding.Count != parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Count)
            {
                Console.WriteLine("Missing Something");
                Console.ReadKey();
            }
        }
    }
}
