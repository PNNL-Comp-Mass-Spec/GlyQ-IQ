using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQGlyQ.Enumerations;

namespace IQGlyQ.UnitTesting
{
    public class IQ_UnitTest_SetStrings
    {
        public static void Set(out string testFile, out string peaksTestFile, out string resultsFolder, out string targetsFile, out string executorParameterFile, out string factorsFile, out string loggingFolder, out string pathFragmentParameters, out string filefolderPath, EnumerationDataset thisDataset, EnumerationIsPic isPic)
        {
            resultsFolder = null;
            executorParameterFile = null;
            factorsFile = "Factors_L10.txt";
            filefolderPath = @"F:\ScottK\WorkingParameters";
            switch (isPic)
            {
                case EnumerationIsPic.IsPic:
                    {
                        resultsFolder = @"F:\ScottK\Results";
                        executorParameterFile = @"F:\ScottK\WorkingParameters\L_RunFile.xml";
                        filefolderPath = @"F:\ScottK\WorkingParameters";
                    }
                    break;
                case EnumerationIsPic.IsNotPic:
                    {
                        //resultsFolder = @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\Unlabelled\Results";
                        //resultsFolder = @"E:\ScottK\IQ\RunFiles";
                        resultsFolder = @"E:\ScottK\WorkingResults";
                        filefolderPath = @"E:\ScottK\IQ\RunFiles";

                        //executorParameterFile = @"E:\ScottK\IQ\RunFiles\SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Work HM UnitTest.xml";
                        executorParameterFile = @"E:\ScottK\IQ\RunFiles\SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Work HM Test.xml";
                        //executorParameterFile = @"E:\ScottK\IQ\RunFiles\L_10RunFile.xml";//basic dataset run

                        //executorParameterFile = @"E:\ScottK\IQ\RunFiles\L_10RunFile453.xml";//1Da shift

                        switch (thisDataset)
                        {
                            case EnumerationDataset.SPINExactiveMuddiman:
                                {
                                    executorParameterFile = @"E:\ScottK\IQ\RunFiles\L_10RunFileMuddiman.xml";
                                }
                                break;
                        }
                    }
                    break;
            }

            loggingFolder = resultsFolder;
            factorsFile = filefolderPath + @"\\" + factorsFile;

            //string testFile = UnitTesting2.FileRefs.RawDataMSFiles.OrbitrapStdFile1;
            //testFile = @"\\protoapps\UserData\Slysz\DeconTools_TestFiles" + "\\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
            testFile = @"D:\Csharp\ConosleApps\LocalServer\IQ\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";

            //peaksTestFile = @"\\protoapps\UserData\Slysz\DeconTools_TestFiles\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18_scans5500-6500_peaks.txt";
            peaksTestFile = @"D:\Csharp\ConosleApps\LocalServer\IQ\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18_scans5500-6500_peaks.txt";

            //targetsFile = @"\\protoapps\UserData\Slysz\Data\MassTags\QCShew_Formic_MassTags_Bin10_all.txt";
            targetsFile = @"D:\Csharp\ConosleApps\LocalServer\IQ\QCShew_Formic_MassTags_Bin10_all.txt";

            pathFragmentParameters = @"E:\ScottK\IQ\RunFiles\FragmentedTargetedWorkflowParameters_Velos_DH.txt";

            switch (thisDataset)
            {
                case EnumerationDataset.SPINExactive:
                    {
                        switch (isPic)
                        {
                            case EnumerationIsPic.IsPic:
                                {
                                    testFile = @"F:\ScottK\RawData\Gly09_SN130_4Mar13_Cheetah_C14_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";
                                    pathFragmentParameters = @"E:\ScottK\IQ\RunFiles\FragmentedTargetedWorkflowParameters_Velos_DH.txt";
                                }
                                break;
                            case EnumerationIsPic.IsNotPic:
                                {
                                    peaksTestFile = @"S:\Gly09_SN130_4Mar13_Cheetah_C14_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar_peaks.txt";
                                    testFile = @" E:\PNNL Data\2013_02_18 SPIN Exactive04\Gly09_SN130_4Mar13_Cheetah_C14_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";
                                    pathFragmentParameters = @"E:\ScottK\IQ\RunFiles\FragmentedTargetedWorkflowParameters_SpinExactive.txt";
                                    //testFile = @" T:\Raw\Gly09_SN130_4Mar13_Cheetah_C14_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";
                                }
                                break;
                        }
                    }
                    break;
                case EnumerationDataset.SPINExactiveMuddiman:
                    {
                        

                        switch (isPic)
                        {
                            case EnumerationIsPic.IsPic:
                                {
                                    //A 126-131
                                    //testFile = @"F:\ScottK\RawData\Gly09_SN126131_6Mar13_Cheetah_C18_50cm_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";

                                    //B 125-128
                                    //testFile = @"F:\ScottK\RawData\Gly09_SN125128_7Mar13_Cheetah_C18_50cm_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";

                                    //C 127-132
                                    testFile = @"F:\ScottK\RawData\Gly09_SN127132_6Mar13_Cheetah_C18_50cm_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";
                                    pathFragmentParameters = @"E:\ScottK\IQ\RunFiles\FragmentedTargetedWorkflowParameters_Velos_DH.txt";
                                }
                                break;
                            case EnumerationIsPic.IsNotPic:
                                {
                                    peaksTestFile = @"S:\Gly09_SN130_4Mar13_Cheetah_C14_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar_peaks.txt";
                                    pathFragmentParameters = @"E:\ScottK\IQ\RunFiles\FragmentedTargetedWorkflowParameters_Velos_DH.txt";
                                    //A 126-131
                                    testFile = @" E:\PNNL Data\2013_02_18 SPIN Exactive04\Muddiman\Gly09_SN126131_6Mar13_Cheetah_C18_50cm_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";

                                    //B 125-128
                                    //testFile = @" E:\PNNL Data\2013_02_18 SPIN Exactive04\Muddiman\Gly09_SN125128_7Mar13_Cheetah_C18_50cm_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";

                                    //C 127-132
                                    //testFile = @" E:\PNNL Data\2013_02_18 SPIN Exactive04\Muddiman\Gly09_SN127132_6Mar13_Cheetah_C18_50cm_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";

                                    //testFile = @" T:\Raw\Gly09_SN130_4Mar13_Cheetah_C14_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";
                                }
                                break;
                        }
                    }
                    break;
                case EnumerationDataset.SN123R8:
                    {
                        switch (isPic)
                        {
                            case EnumerationIsPic.IsPic:
                                {
                                    testFile = @"F:\ScottK\RawData\Gly09_Velos3_Jaguar_230nL30_C15_SN123_3X_01Jan13_R8.raw";
                                    pathFragmentParameters = @"E:\ScottK\IQ\RunFiles\FragmentedTargetedWorkflowParameters_Velos_DH.txt";
                                }
                                break;
                            case EnumerationIsPic.IsNotPic:
                                {
                                    testFile = @" E:\PNNL Data\2012_12_24 Velos 3\Gly09_Velos3_Jaguar_230nL30_C15_SN123_3X_01Jan13_R8.raw";
                                    peaksTestFile = @" E:\PNNL Data\2012_12_24 Velos 3\Gly09_Velos3_Jaguar_230nL30_C15_SN123_3X_01Jan13_R8_peaks.txt";
                                    pathFragmentParameters = @"E:\ScottK\IQ\RunFiles\FragmentedTargetedWorkflowParameters_Velos_DH.txt";
                                }
                                break;
                        }
                    }
                    break;
                case EnumerationDataset.Diabetes:
                    {
                        switch (isPic)
                        {
                            case EnumerationIsPic.IsPic:
                                {
                                    testFile = @"F:\ScottK\RawData\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
                                    pathFragmentParameters = @"E:\ScottK\IQ\RunFiles\FragmentedTargetedWorkflowParameters_Velos_DH.txt";
                                }
                                break;
                            case EnumerationIsPic.IsNotPic:
                                {
                                    peaksTestFile = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_peaks.txt";
                                    peaksTestFile = @"S:\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_peaks.txt";
                                    pathFragmentParameters = @"E:\ScottK\IQ\RunFiles\FragmentedTargetedWorkflowParameters_Velos_DH.txt";

                                    //home
                                    testFile = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";

                                    //work ram disk
                                    //testFile = @"Y:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
                                }
                                break;
                        }
                    }
                    break;
                case EnumerationDataset.IMS:
                    {
                        switch (isPic)
                        {
                            case EnumerationIsPic.IsPic:
                                {
                                    testFile = @"F:\ScottK\RawData\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
                                    pathFragmentParameters = @"E:\ScottK\IQ\RunFiles\FragmentedTargetedWorkflowParameters_Velos_DH.txt";
                                }
                                break;
                            case EnumerationIsPic.IsNotPic:
                                {
                                    peaksTestFile = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_peaks.txt";
                                    peaksTestFile = @"S:\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_peaks.txt";
                                    pathFragmentParameters = @"E:\ScottK\IQ\RunFiles\FragmentedTargetedWorkflowParameters_Velos_DH.txt";
                                    //home
                                    testFile = @"D:\Csharp\ConosleApps\LocalServer\UIMF\Gly09_SN130_8Mar13_Cheetah_C14_220nL_IMS6_2700V_130C_Multi__BC.uimf";

                                    //work ram disk
                                    //testFile = @"Y:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
                                }
                                break;
                        }
                    }
                    break;
                default:
                    {
                        
                        Console.WriteLine("Missing Selection Utilitiles");
                        Console.ReadKey();
                    }
                    break;
            }

        }
    }
}
