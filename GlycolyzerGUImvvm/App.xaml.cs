using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using GlycolyzerGUImvvm.Views;
using GlycolyzerGUImvvm.ViewModels;
using System.Windows.Resources;
using System.Windows.Controls;
using GlycolyzerGUImvvm.Models;
using System.IO;

namespace GlycolyzerGUImvvm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static InitializingFlagsModel initializingFlagsModel;

        public static ParameterPage parameterPage;
        public static OmniFinderGMPage omniFinderGMPage;

        public static OmniFinderGMModel omniFinderGMModel_Save = new OmniFinderGMModel();
        public static GlycanMakerModel glycanMakerModel_Save = new GlycanMakerModel();
        public static RangesModel omniFinderGMRangesModel_Save = new RangesModel();

        public static OmniFinderModel omniFinderModel_Save = new OmniFinderModel();
        public static LibrariesModel librariesModel_Save = new LibrariesModel();
        public static FolderModel folderModel_Save = new FolderModel();
        public static ExtraScienceParameterModel extraScienceParameterModel_Save = new ExtraScienceParameterModel();
        public static RangesModel parameterRangesModel_Save = new RangesModel();

        public static String rangesBGColor;


        protected override void OnStartup(StartupEventArgs e) 
        { 
            base.OnStartup(e);
            initializingFlagsModel = new InitializingFlagsModel();
            AutoReadXMLFile();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            GetDefaultAddress();
            AutoSaveXMLFile();
        }

        private void AutoReadXMLFile()
        {
            string readMainFile = "D:\\GlycolyzerXMLFile.xml";

            string readFromFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(readMainFile), "GlycolyzerSubDir");
            readFromFolder = System.IO.Path.Combine(readFromFolder, "Glycolyzer");

            if (File.Exists(readFromFolder + "OmniFinderGM.xml"))
            {
                System.Xml.Serialization.XmlSerializer omniFinderGMReader = new System.Xml.Serialization.XmlSerializer(typeof(OmniFinderGMModel));
                System.IO.StreamReader omniFinderGMFile = new System.IO.StreamReader(readFromFolder + "OmniFinderGM.xml");
                App.omniFinderGMModel_Save = (OmniFinderGMModel)omniFinderGMReader.Deserialize(omniFinderGMFile);
                omniFinderGMFile.Close();
            }

            if (File.Exists(readFromFolder + "GlycanMaker.xml"))
            {
                System.Xml.Serialization.XmlSerializer glycanMakerReader = new System.Xml.Serialization.XmlSerializer(typeof(GlycanMakerModel));
                System.IO.StreamReader glycanMakerFile = new System.IO.StreamReader(readFromFolder + "GlycanMaker.xml");
                App.glycanMakerModel_Save = (GlycanMakerModel)glycanMakerReader.Deserialize(glycanMakerFile);
                glycanMakerFile.Close();
            }

            if (File.Exists(readFromFolder + "OmniFinderGMRanges.xml"))
            {
                System.Xml.Serialization.XmlSerializer omniFinderGMRangesReader = new System.Xml.Serialization.XmlSerializer(typeof(RangesModel));
                System.IO.StreamReader omniFinderGMRangesFile = new System.IO.StreamReader(readFromFolder + "OmniFinderGMRanges.xml");
                App.omniFinderGMRangesModel_Save = (RangesModel)omniFinderGMRangesReader.Deserialize(omniFinderGMRangesFile);
                omniFinderGMRangesFile.Close();
            }

            if (File.Exists(readFromFolder + "OmniFinderParameter.xml"))
            {
                System.Xml.Serialization.XmlSerializer omniFinderParameterReader = new System.Xml.Serialization.XmlSerializer(typeof(OmniFinderModel));
                System.IO.StreamReader omniFinderParameterFile = new System.IO.StreamReader(readFromFolder + "OmniFinderParameter.xml");
                App.omniFinderModel_Save = (OmniFinderModel)omniFinderParameterReader.Deserialize(omniFinderParameterFile);
                omniFinderParameterFile.Close();
            }

            if (File.Exists(readFromFolder + "Libraries.xml"))
            {
                System.Xml.Serialization.XmlSerializer librariesReader = new System.Xml.Serialization.XmlSerializer(typeof(LibrariesModel));
                System.IO.StreamReader librariesFile = new System.IO.StreamReader(readFromFolder + "Libraries.xml");
                App.librariesModel_Save = (LibrariesModel)librariesReader.Deserialize(librariesFile);
                librariesFile.Close();
            }

            if (File.Exists(readFromFolder + "Folders.xml"))
            {
                System.Xml.Serialization.XmlSerializer folderReader = new System.Xml.Serialization.XmlSerializer(typeof(FolderModel));
                System.IO.StreamReader folderFile = new System.IO.StreamReader(readFromFolder + "Folders.xml");
                App.folderModel_Save = (FolderModel)folderReader.Deserialize(folderFile);
                folderFile.Close();
            }

            if (File.Exists(readFromFolder + "ExtraScienceParameter.xml"))
            {
                System.Xml.Serialization.XmlSerializer extraScienceParameterReader = new System.Xml.Serialization.XmlSerializer(typeof(ExtraScienceParameterModel));
                System.IO.StreamReader extraScienceParameterFile = new System.IO.StreamReader(readFromFolder + "ExtraScienceParameter.xml");
                App.extraScienceParameterModel_Save = (ExtraScienceParameterModel)extraScienceParameterReader.Deserialize(extraScienceParameterFile);
                extraScienceParameterFile.Close();
            }

            if (File.Exists(readFromFolder + "ParameterRanges.xml"))
            {
                System.Xml.Serialization.XmlSerializer parameterRangesReader = new System.Xml.Serialization.XmlSerializer(typeof(RangesModel));
                System.IO.StreamReader parameterRangesFile = new System.IO.StreamReader(readFromFolder + "ParameterRanges.xml");
                App.parameterRangesModel_Save = (RangesModel)parameterRangesReader.Deserialize(parameterRangesFile);
                parameterRangesFile.Close();
            }
        }

        private void GetDefaultAddress()
        {
            switch (librariesModel_Save.ChosenDefaultLibrary_String)
            {
                case "NLinked_Alditol" :
                    {
                        //librariesModel_Save.AddressDefaultLibrary_String = ;
                        break;
                    }
                case "NLinked_Alditol_2ndIsotope" :
                    {
                        //librariesModel_Save.AddressDefaultLibrary_String = ;
                        break;
                    }
                    case "NLinked_Aldehyde" :
                    {
                        //librariesModel_Save.AddressDefaultLibrary_String = ;
                        break;
                    }
                    case "Cell_Alditol" :
                    {
                        //librariesModel_Save.AddressDefaultLibrary_String = ;
                        break;
                    }
                    case "Cell_Alditol_V2" :
                    {
                        //librariesModel_Save.AddressDefaultLibrary_String = ;
                        break;
                    }
                    case "Cell_Alditol_Vmini" :
                    {
                        //librariesModel_Save.AddressDefaultLibrary_String = ;
                        break;
                    }
                    case "Ant_Alditol" :
                    {
                        //librariesModel_Save.AddressDefaultLibrary_String = ;
                        break;
                    }
                    case "NonCalibrated" :
                    {
                        //librariesModel_Save.AddressDefaultLibrary_String = ;
                        break;
                    }
                    case "NLinked_Alditol_PolySA" :
                    {
                        //librariesModel_Save.AddressDefaultLibrary_String = ;
                        break;
                    }
                    case "NLinked_Alditol8" :
                    {
                        //librariesModel_Save.AddressDefaultLibrary_String = ;
                        break;
                    }
                    case "NLinked_Alditol9" :
                    {
                        //librariesModel_Save.AddressDefaultLibrary_String = ;
                        break;
                    }
                    case "Hexose" :
                    {
                        //librariesModel_Save.AddressDefaultLibrary_String = ;
                        break;
                    }
            }
        }

        private void AutoSaveXMLFile()
        {
            string writeFile = "D:\\GlycolyzerXMLFile.xml";

            string writeFolder = System.IO.Path.GetDirectoryName(writeFile);

            string newPath = System.IO.Path.Combine(writeFolder, "GlycolyzerSubDir");
            System.IO.Directory.CreateDirectory(newPath);
            string newFileName = "Glycolyzer";
            newPath = System.IO.Path.Combine(newPath, newFileName);

            //if (!System.IO.File.Exists(newPath)){}

            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(String));
            System.IO.StreamWriter file = new System.IO.StreamWriter(writeFile);
            writer.Serialize(file, "Individual Files in GlycolyzerSubDir Folder");
            file.Close();

            System.Xml.Serialization.XmlSerializer omniFinderGMWriter = new System.Xml.Serialization.XmlSerializer(typeof(OmniFinderGMModel));
            System.IO.StreamWriter omniFinderGMFile = new System.IO.StreamWriter(newPath + "OmniFinderGM.xml");
            omniFinderGMWriter.Serialize(omniFinderGMFile, App.omniFinderGMModel_Save);
            omniFinderGMFile.Close();

            System.Xml.Serialization.XmlSerializer glycanMakerWriter = new System.Xml.Serialization.XmlSerializer(typeof(GlycanMakerModel));
            System.IO.StreamWriter glycanMakerFile = new System.IO.StreamWriter(newPath + "GlycanMaker.xml");
            glycanMakerWriter.Serialize(glycanMakerFile, App.glycanMakerModel_Save);
            glycanMakerFile.Close();

            System.Xml.Serialization.XmlSerializer omniFinderGMRangesWriter = new System.Xml.Serialization.XmlSerializer(typeof(RangesModel));
            System.IO.StreamWriter omniFinderGMRangesFile = new System.IO.StreamWriter(newPath + "OmniFinderGMRanges.xml");
            omniFinderGMRangesWriter.Serialize(omniFinderGMRangesFile, App.omniFinderGMRangesModel_Save);
            omniFinderGMRangesFile.Close();

            System.Xml.Serialization.XmlSerializer omniFinderParameterWriter = new System.Xml.Serialization.XmlSerializer(typeof(OmniFinderModel));
            System.IO.StreamWriter omniFinderParameterFile = new System.IO.StreamWriter(newPath + "OmniFinderParameter.xml");
            omniFinderParameterWriter.Serialize(omniFinderParameterFile, App.omniFinderModel_Save);
            omniFinderParameterFile.Close();

            System.Xml.Serialization.XmlSerializer librariesWriter = new System.Xml.Serialization.XmlSerializer(typeof(LibrariesModel));
            System.IO.StreamWriter librariesFile = new System.IO.StreamWriter(newPath + "Libraries.xml");
            librariesWriter.Serialize(librariesFile, App.librariesModel_Save);
            librariesFile.Close();

            System.Xml.Serialization.XmlSerializer folderWriter = new System.Xml.Serialization.XmlSerializer(typeof(FolderModel));
            System.IO.StreamWriter folderFile = new System.IO.StreamWriter(newPath + "Folders.xml");
            folderWriter.Serialize(folderFile, App.folderModel_Save);
            folderFile.Close();

            System.Xml.Serialization.XmlSerializer extraScienceParameterWriter = new System.Xml.Serialization.XmlSerializer(typeof(ExtraScienceParameterModel));
            System.IO.StreamWriter extraScienceParameterFile = new System.IO.StreamWriter(newPath + "ExtraScienceParameter.xml");
            extraScienceParameterWriter.Serialize(extraScienceParameterFile, App.extraScienceParameterModel_Save);
            extraScienceParameterFile.Close();

            System.Xml.Serialization.XmlSerializer parameterRangesWriter = new System.Xml.Serialization.XmlSerializer(typeof(RangesModel));
            System.IO.StreamWriter parameterRangesFile = new System.IO.StreamWriter(newPath + "ParameterRanges.xml");
            parameterRangesWriter.Serialize(parameterRangesFile, App.parameterRangesModel_Save);
            parameterRangesFile.Close();
        }
    }
}
