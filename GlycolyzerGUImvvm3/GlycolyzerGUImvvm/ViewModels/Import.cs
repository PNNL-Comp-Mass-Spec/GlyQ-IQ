using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlycolyzerGUImvvm.Models;
using System.IO;
using GlycolyzerGUImvvm.Views;
using System.Threading;
using System.ComponentModel;

namespace GlycolyzerGUImvvm.ViewModels
{
    public class Import
    {
        public static ParameterModel parameterModel_Input = new ParameterModel();

        public static void ImportParameters(string path)
        {
            System.Xml.Serialization.XmlSerializer pReader = new System.Xml.Serialization.XmlSerializer(typeof(ParameterModel));
            System.IO.StreamReader pFile = new System.IO.StreamReader(path);
            parameterModel_Input = (ParameterModel)pReader.Deserialize(pFile);
            pFile.Close();
        }

        public static void ImportXMLFiles(string type)
        {
            string readFolder = "";

            if (type == "auto")
                readFolder = "D:\\GlycolyzerSubDir";
            else if (type == "customLoad")
                readFolder = FolderBrowse.folderBrowse("read");

            if (Directory.Exists(readFolder))
            {
                PopUpWorker loadWorkerObject = new PopUpWorker();
                Thread loadWorkerThread = new Thread(loadWorkerObject.DoLoadWork);

                loadWorkerThread.SetApartmentState(ApartmentState.STA);
                loadWorkerThread.Start();
                while (!loadWorkerThread.IsAlive) ;
                Thread.Sleep(1);

                if (File.Exists(readFolder + "\\OmniFinderGM.xml"))
                {
                    System.Xml.Serialization.XmlSerializer omniFinderGMReader = new System.Xml.Serialization.XmlSerializer(typeof(OmniFinderGMModel));
                    System.IO.StreamReader omniFinderGMFile = new System.IO.StreamReader(readFolder + "\\OmniFinderGM.xml");
                    App.omniFinderGMModel_Save = (OmniFinderGMModel)omniFinderGMReader.Deserialize(omniFinderGMFile);
                    omniFinderGMFile.Close();
                }

                if (File.Exists(readFolder + "\\GlycanMaker.xml"))
                {
                    System.Xml.Serialization.XmlSerializer glycanMakerReader = new System.Xml.Serialization.XmlSerializer(typeof(GlycanMakerModel));
                    System.IO.StreamReader glycanMakerFile = new System.IO.StreamReader(readFolder + "\\GlycanMaker.xml");
                    App.glycanMakerModel_Save = (GlycanMakerModel)glycanMakerReader.Deserialize(glycanMakerFile);
                    glycanMakerFile.Close();
                }

                if (File.Exists(readFolder + "\\OmniFinderGMRanges.xml"))
                {
                    System.Xml.Serialization.XmlSerializer omniFinderGMRangesReader = new System.Xml.Serialization.XmlSerializer(typeof(RangesModel));
                    System.IO.StreamReader omniFinderGMRangesFile = new System.IO.StreamReader(readFolder + "\\OmniFinderGMRanges.xml");
                    App.omniFinderGMRangesModel_Save = (RangesModel)omniFinderGMRangesReader.Deserialize(omniFinderGMRangesFile);
                    omniFinderGMRangesFile.Close();
                }

                if (File.Exists(readFolder + "\\OmniFinderParameter.xml"))
                {
                    System.Xml.Serialization.XmlSerializer omniFinderParameterReader = new System.Xml.Serialization.XmlSerializer(typeof(OmniFinderModel));
                    System.IO.StreamReader omniFinderParameterFile = new System.IO.StreamReader(readFolder + "\\OmniFinderParameter.xml");
                    App.omniFinderModel_Save = (OmniFinderModel)omniFinderParameterReader.Deserialize(omniFinderParameterFile);
                    omniFinderParameterFile.Close();
                }

                if (File.Exists(readFolder + "\\Libraries.xml"))
                {
                    System.Xml.Serialization.XmlSerializer librariesReader = new System.Xml.Serialization.XmlSerializer(typeof(LibrariesModel));
                    System.IO.StreamReader librariesFile = new System.IO.StreamReader(readFolder + "\\Libraries.xml");
                    App.librariesModel_Save = (LibrariesModel)librariesReader.Deserialize(librariesFile);
                    librariesFile.Close();
                }

                if (File.Exists(readFolder + "\\Folders.xml"))
                {
                    System.Xml.Serialization.XmlSerializer folderReader = new System.Xml.Serialization.XmlSerializer(typeof(FolderModel));
                    System.IO.StreamReader folderFile = new System.IO.StreamReader(readFolder + "\\Folders.xml");
                    App.folderModel_Save = (FolderModel)folderReader.Deserialize(folderFile);
                    folderFile.Close();
                }

                if (File.Exists(readFolder + "\\ExtraScienceParameter.xml"))
                {
                    System.Xml.Serialization.XmlSerializer extraScienceParameterReader = new System.Xml.Serialization.XmlSerializer(typeof(ExtraScienceParameterModel));
                    System.IO.StreamReader extraScienceParameterFile = new System.IO.StreamReader(readFolder + "\\ExtraScienceParameter.xml");
                    App.extraScienceParameterModel_Save = (ExtraScienceParameterModel)extraScienceParameterReader.Deserialize(extraScienceParameterFile);
                    extraScienceParameterFile.Close();
                }

                if (File.Exists(readFolder + "\\ParameterRanges.xml"))
                {
                    System.Xml.Serialization.XmlSerializer parameterRangesReader = new System.Xml.Serialization.XmlSerializer(typeof(RangesModel));
                    System.IO.StreamReader parameterRangesFile = new System.IO.StreamReader(readFolder + "\\ParameterRanges.xml");
                    App.parameterRangesModel_Save = (RangesModel)parameterRangesReader.Deserialize(parameterRangesFile);
                    parameterRangesFile.Close();
                }

                loadWorkerObject.RequestStop();
            }

            ParameterViewModel.RunChangedAction("AutoFileName");
        }
    }
}
