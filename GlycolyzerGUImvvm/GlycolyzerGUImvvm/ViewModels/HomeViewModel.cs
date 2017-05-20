using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlycolyzerGUImvvm.Models;
using System.Windows.Input;
using GlycolyzerGUImvvm.Commands;

namespace GlycolyzerGUImvvm.ViewModels
{
    public class HomeViewModel : ObservableObject
    {
        private Models.FolderModel folderModel;

        public Models.FolderModel FolderModel
        {
            get { return folderModel; }
            set
            {
                if (value != folderModel)
                { folderModel = value; App.folderModel_Save = folderModel; this.OnPropertyChanged("folderModel"); }
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

        public HomeViewModel()
        {
            folderModel = new Models.FolderModel();

            FolderModel = App.folderModel_Save;

            ButtonCommand = new RelayCommand(new Action<object>(ButtonAction));
        }

        public void ButtonAction(object obj)
        {
            switch ((String)obj)
            {
                case "appParametersBrowse":
                    {
                        string tempPath = FolderBrowse.folderBrowse("autoSave");
                        if (tempPath != "")
                        {
                            FolderModel.SaveAppParametersLocationFolder_String = tempPath;
                            App.folderModel_Save.PreviousPath_String = FolderModel.SaveAppParametersLocationFolder_String;
                        }
                        break;
                    }
            }
        }
    }
}
