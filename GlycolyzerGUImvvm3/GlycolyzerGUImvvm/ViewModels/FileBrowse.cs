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

namespace GlycolyzerGUImvvm.ViewModels
{
    public class FileBrowse
    {
        public static string fileBrowse(string type)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            if (App.folderModel_Save.PreviousPath_String != "")
                dlg.InitialDirectory = App.folderModel_Save.PreviousPath_String;
            else
                dlg.InitialDirectory = "C:\\";

            if (type == "exe")
            {
                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".exe";
                dlg.Filter = "(*.exe)|*.exe|All files (*.*)|*.*";
            }
            else
            {
                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".xml";
                dlg.Filter = "(*.xml;*.txt)|*.xml;*.txt|All files (*.*)|*.*";
            }

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
