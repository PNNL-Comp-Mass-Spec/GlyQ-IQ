/* Written by Myanna Harris
* for the Department of Energy (PNNL, Richland, WA)
* Battelle Memorial Institute
* E-mail: Myanna.Harris@pnnl.gov
* Website: http://omics.pnl.gov/software
* -----------------------------------------------------
* 
 * Notice: This computer software was prepared by Battelle Memorial Institute,
* hereinafter the Contractor, under Contract No. DE-AC05-76RL0 1830 with the
* Department of Energy (DOE).  All rights in the computer software are reserved
* by DOE on behalf of the United States Government and the Contractor as
* provided in the Contract.
* 
 * NEITHER THE GOVERNMENT NOR THE CONTRACTOR MAKES ANY WARRANTY, EXPRESS OR
* IMPLIED, OR ASSUMES ANY LIABILITY FOR THE USE OF THIS SOFTWARE.
* 
 * This notice including this sentence must appear on any copies of this computer
* software.
* -----------------------------------------------------*/

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
    public class GUIExport
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
                    System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning,
                    System.Windows.Forms.MessageBoxDefaultButton.Button2);

                if (warningResult == System.Windows.Forms.DialogResult.Yes)
                    saveFile = true;
            }

            if (saveFile == true)
            {
                PopUpWorker saveWorkerObject = new PopUpWorker();
                Thread saveWorkerThread = new Thread(saveWorkerObject.DoSaveWork);
                saveWorkerObject.RequestStart();

                saveWorkerThread.SetApartmentState(ApartmentState.STA);
                saveWorkerThread.Start();
                while (!saveWorkerThread.IsAlive) ;
                Thread.Sleep(1);

                System.Xml.Serialization.XmlSerializer parametersWriter = new System.Xml.Serialization.XmlSerializer(typeof(ParameterModel));
                System.IO.StreamWriter parametersFile = new System.IO.StreamWriter(writeFile);
                parametersWriter.Serialize(parametersFile, parameterModel_Save);
                parametersFile.Close();

                saveWorkerObject.RequestStop();

                PopUpWorker saveWorkerObject1 = new PopUpWorker();
                Thread saveWorkerThread1 = new Thread(saveWorkerObject1.FinishSaveWork);
                saveWorkerObject1.RequestStart();

                saveWorkerThread1.SetApartmentState(ApartmentState.STA);
                saveWorkerThread1.Start();
                while (!saveWorkerThread1.IsAlive) ;
                Thread.Sleep(1000);

                saveWorkerObject1.RequestStop();
            }
        }

        public static void ExportXMLParameterFiles(String type)
        {
            string writeFolder = "";

            if (type == "autoSave")
            {
                writeFolder = "C:\\GlycolyzerApplicationFolder";
                if(!Directory.Exists(writeFolder))
                {
                    System.IO.Directory.CreateDirectory(writeFolder);
                }
            }
            else if (type == "customSave")
            {
                writeFolder = App.folderModel_Save.SaveAppParametersLocationFolder_String;

                if (File.Exists(writeFolder + "\\OmniFinderGM.xml") && File.Exists(writeFolder + "\\GlycanMaker.xml")
                    && File.Exists(writeFolder + "\\OmniFinderGMRanges.xml") && File.Exists(writeFolder + "\\OmniFinderParameter.xml")
                    && File.Exists(writeFolder + "\\Libraries.xml") && File.Exists(writeFolder + "\\Folders.xml")
                    && File.Exists(writeFolder + "\\ExtraScienceParameter.xml") && File.Exists(writeFolder + "\\ParameterRanges.xml"))
                {
                    System.Windows.Forms.DialogResult warningResult = System.Windows.Forms.MessageBox.Show(
                        "Files already exist. Do you want to overwrite them?", "Warning",
                        System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning,
                        System.Windows.Forms.MessageBoxDefaultButton.Button2);

                    if (warningResult == System.Windows.Forms.DialogResult.No)
                        writeFolder = "";
                }
            }

            if (Directory.Exists(writeFolder))
            {
                PopUpWorker saveWorkerObject = new PopUpWorker();
                Thread saveWorkerThread = new Thread(saveWorkerObject.DoSaveWork);
                saveWorkerObject.RequestStart();

                saveWorkerThread.SetApartmentState(ApartmentState.STA);
                saveWorkerThread.Start();
                while (!saveWorkerThread.IsAlive) ;
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

                saveWorkerObject.RequestStop();

                PopUpWorker saveWorkerObject1 = new PopUpWorker();
                Thread saveWorkerThread1 = new Thread(saveWorkerObject1.FinishSaveWork);
                saveWorkerObject1.RequestStart();

                saveWorkerThread1.SetApartmentState(ApartmentState.STA);
                saveWorkerThread1.Start();
                while (!saveWorkerThread1.IsAlive) ;
                Thread.Sleep(1000);

                saveWorkerObject1.RequestStop();
            }
        }
    }
}
