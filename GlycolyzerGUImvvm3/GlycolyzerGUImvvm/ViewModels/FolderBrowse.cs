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
