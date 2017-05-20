using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GlycolyzerGUImvvm.ViewModels
{
    public class FolderBrowse
    {
        public static string folderBrowse(String dlgDescription)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            if (App.folderModel_Save.PreviousPath_String != "")
                dlg.SelectedPath = App.folderModel_Save.PreviousPath_String;
            else
                dlg.SelectedPath = "C:\\";

            if (dlgDescription == "autoSave")
                dlg.Description = "Select the document folder to save this session's application parameters to.";
            else if (dlgDescription == "read")
                dlg.Description = "Select the document folder to read previous input from.";
            else if (dlgDescription == "outputFolder")
                dlg.Description = "Select the document folder to save output to.";
            else if (dlgDescription == "saveFolder")
                dlg.Description = "Select the document folder to save the parameter file to.";

            System.Windows.Forms.DialogResult result = dlg.ShowDialog();

            if (result.ToString() == "OK")
                return dlg.SelectedPath;
            else
                return "";
        }
    }
}
