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
using System.ComponentModel;
using System.Threading;

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
        public static OmniFinderGMRangesPage omniFinderGMRangesPage;

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
            GUIImport.ImportXMLParameterFiles("auto");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            GUIExport.ExportXMLParameterFiles("autoSave");
        }

        public static void Pause()
        {
            for (int i = 0; i <= 300000000; i++)
            {
            }
        }
    }
}
