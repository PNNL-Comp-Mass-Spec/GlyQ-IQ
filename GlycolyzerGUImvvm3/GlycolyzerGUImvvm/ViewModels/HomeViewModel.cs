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
