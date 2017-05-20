using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlycolyzerGUImvvm.Models;
using System.IO;
using GlycolyzerGUImvvm.Views;
using System.Threading;

namespace GlycolyzerGUImvvm.ViewModels
{
    public class Export
    {
        public static void ExportParameterXMLFile(String writeFile)
        {
            ParameterModel parameterModel_Save = new ParameterModel(App.omniFinderModel_Save, App.librariesModel_Save,
                App.folderModel_Save, App.extraScienceParameterModel_Save, App.parameterRangesModel_Save);

            Boolean saveFile = true;

            if (System.IO.File.Exists(writeFile))
            {
                saveFile = false;

                System.Windows.Forms.DialogResult warningResult = System.Windows.Forms.MessageBox.Show(
                    "File already exists. Do you want to overwrite it?", "Warning",
                    System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question,
                    System.Windows.Forms.MessageBoxDefaultButton.Button2);

                if (warningResult == System.Windows.Forms.DialogResult.Yes)
                    saveFile = true;
            }

            if (saveFile == true)
            {
                PopUpWorker saveWorkerObject = new PopUpWorker();
                Thread saveWorkerThread = new Thread(saveWorkerObject.DoSaveWork);

                saveWorkerThread.SetApartmentState(ApartmentState.STA);
                saveWorkerThread.Start();
                while (!saveWorkerThread.IsAlive) ;
                Thread.Sleep(1);

                System.Xml.Serialization.XmlSerializer parametersWriter = new System.Xml.Serialization.XmlSerializer(typeof(ParameterModel));
                System.IO.StreamWriter parametersFile = new System.IO.StreamWriter(writeFile);
                parametersWriter.Serialize(parametersFile, parameterModel_Save);
                parametersFile.Close();

                saveWorkerObject.RequestStop();
            }
        }

        public static void AutoExportXMLFiles()
        {
            string writeFolder = "";

            if (Directory.Exists(App.folderModel_Save.SaveAppParametersLocationFolder_String))
                writeFolder = App.folderModel_Save.SaveAppParametersLocationFolder_String;
            else
                writeFolder = FolderBrowse.folderBrowse("autoSave");

            if (Directory.Exists(writeFolder))
            {
                PopUpWorker workerObject = new PopUpWorker();
                Thread workerThread = new Thread(workerObject.DoSaveWork);

                workerThread.SetApartmentState(ApartmentState.STA);
                workerThread.Start();
                while (!workerThread.IsAlive) ;
                Thread.Sleep(1);

                System.Xml.Serialization.XmlSerializer omniFinderGMWriter = new System.Xml.Serialization.XmlSerializer(typeof(OmniFinderGMModel));
                System.IO.StreamWriter omniFinderGMFile = new System.IO.StreamWriter(writeFolder + "\\OmniFinderGM.xml");
                omniFinderGMWriter.Serialize(omniFinderGMFile, App.omniFinderGMModel_Save);
                omniFinderGMFile.Close();

                System.Xml.Serialization.XmlSerializer glycanMakerWriter = new System.Xml.Serialization.XmlSerializer(typeof(GlycanMakerModel));
                System.IO.StreamWriter glycanMakerFile = new System.IO.StreamWriter(writeFolder + "\\GlycanMaker.xml");
                glycanMakerWriter.Serialize(glycanMakerFile, App.glycanMakerModel_Save);
                glycanMakerFile.Close();

                System.Xml.Serialization.XmlSerializer omniFinderGMRangesWriter = new System.Xml.Serialization.XmlSerializer(typeof(RangesModel));
                System.IO.StreamWriter omniFinderGMRangesFile = new System.IO.StreamWriter(writeFolder + "\\OmniFinderGMRanges.xml");
                omniFinderGMRangesWriter.Serialize(omniFinderGMRangesFile, App.omniFinderGMRangesModel_Save);
                omniFinderGMRangesFile.Close();

                System.Xml.Serialization.XmlSerializer omniFinderParameterWriter = new System.Xml.Serialization.XmlSerializer(typeof(OmniFinderModel));
                System.IO.StreamWriter omniFinderParameterFile = new System.IO.StreamWriter(writeFolder + "\\OmniFinderParameter.xml");
                omniFinderParameterWriter.Serialize(omniFinderParameterFile, App.omniFinderModel_Save);
                omniFinderParameterFile.Close();

                System.Xml.Serialization.XmlSerializer librariesWriter = new System.Xml.Serialization.XmlSerializer(typeof(LibrariesModel));
                System.IO.StreamWriter librariesFile = new System.IO.StreamWriter(writeFolder + "\\Libraries.xml");
                librariesWriter.Serialize(librariesFile, App.librariesModel_Save);
                librariesFile.Close();

                System.Xml.Serialization.XmlSerializer folderWriter = new System.Xml.Serialization.XmlSerializer(typeof(FolderModel));
                System.IO.StreamWriter folderFile = new System.IO.StreamWriter(writeFolder + "\\Folders.xml");
                folderWriter.Serialize(folderFile, App.folderModel_Save);
                folderFile.Close();

                System.Xml.Serialization.XmlSerializer extraScienceParameterWriter = new System.Xml.Serialization.XmlSerializer(typeof(ExtraScienceParameterModel));
                System.IO.StreamWriter extraScienceParameterFile = new System.IO.StreamWriter(writeFolder + "\\ExtraScienceParameter.xml");
                extraScienceParameterWriter.Serialize(extraScienceParameterFile, App.extraScienceParameterModel_Save);
                extraScienceParameterFile.Close();

                System.Xml.Serialization.XmlSerializer parameterRangesWriter = new System.Xml.Serialization.XmlSerializer(typeof(RangesModel));
                System.IO.StreamWriter parameterRangesFile = new System.IO.StreamWriter(writeFolder + "\\ParameterRanges.xml");
                parameterRangesWriter.Serialize(parameterRangesFile, App.parameterRangesModel_Save);
                parameterRangesFile.Close();

                workerObject.RequestStop();
            }
        }
    }
}
