using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlycolyzerGUImvvm.ViewModels
{
    public class FileBrowse
    {
        public static string fileBrowse()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            if (App.folderModel_Save.PreviousPath_String != "")
                dlg.InitialDirectory = App.folderModel_Save.PreviousPath_String;
            else
                dlg.InitialDirectory = "C:\\";

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".xml";
            dlg.Filter = "(*.xml;*.txt)|*.xml;*.txt|All files (*.*)|*.*";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document
                return dlg.FileName;
            }
            else
                return "";
        }
    }
}
