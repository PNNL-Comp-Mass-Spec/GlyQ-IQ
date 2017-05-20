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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GridView4D
{
    /// <summary>
    /// Interaction logic for CreateFilesWindow.xaml
    /// </summary>
    public partial class CreateFilesWindow : Window
    {
        //keep information for multiple additions
        //factors - Red,1,3,Blue,4,7
        //factorName,min,max
        List<String> factors;
        //subfolder names
        List<String> subFolders;
        //files - fileName,fileName2
        List<String> files;
        //coordinates - coordinateString, coordinateString2
        List<String> coordinates;

        public CreateFilesWindow()
        {
            factors = new List<String>();
            subFolders = new List<String>();
            files = new List<String>();
            coordinates = new List<String>();

            InitializeComponent();
        }

        private void addFactorButton_Click(object sender, RoutedEventArgs e)
        {
            Button from = (Button)sender;

            switch(from.Name)
            {
                case "browseButton":
                    browse();
                    break;
                case "addFactorButton":
                    if (directoryTextBox.Text.Trim().Length != 0 && factorNameTextBox.Text.Trim().Length != 0
                        && factorMinTextBox.Text.Trim().Length != 0 && factorMaxTextBox.Text.Trim().Length != 0)
                        addFactor();
                    else
                        MessageBox.Show("No directory selected or Empty TextBox");
                    break;
                case "addFolderButton":
                    if (directoryTextBox.Text.Trim().Length != 0 && subFolderTextBox.Text.Trim().Length != 0)
                        addFolder();
                    else
                        MessageBox.Show("No directory selected");
                    break;
                case "addFileButton":
                    if (subFolderTextBox.Text.Trim().Length != 0 && subFolders.Count != 0 &&
                        subFolderTextBox.Text == subFolders[subFolders.Count-1]
                        && fileNameTextBox.Text.Trim().Length != 0)
                        addFile();
                    else
                        MessageBox.Show("No folder selected or Folder Not Added or Empty TextBox");
                    break;
                case "addCoordinatesButton":
                    if (fileNameTextBox.Text.Trim().Length != 0 && files.Count != 0 && 
                        fileNameTextBox.Text == files[files.Count - 1] &&
                        coordinatesTextBox.Text.Trim().Length != 0)
                        addCoordinates();
                    else
                        MessageBox.Show("No file selected or File Not Added or Empty TextBox");
                    break;
                case "doneButton":
                    if (directoryTextBox.Text.Trim().Length != 0 && mainFolderTextBox.Text.Trim().Length != 0)
                    {
                        save();
                        this.Close();
                    }
                    else
                        MessageBox.Show("Information missing");
                    break;
                case "cancelButton":
                    this.Close();
                    break;
            }
        }

        private void browse()
        {
            //create FolderBrowserDialog
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            //select starting path
            dlg.SelectedPath = "C:\\";

            //display FolderBrowserDialog
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();

            //Get the selected file name and display in a TextBox 
            directoryTextBox.Text = dlg.SelectedPath;
        }

        private void addFactor()
        {
            factors.Add(factorNameTextBox.Text);
            factors.Add(factorMinTextBox.Text);
            factors.Add(factorMaxTextBox.Text);
        }

        private void addFolder()
        {
            subFolders.Add(subFolderTextBox.Text);
        }

        private void addFile()
        {
            files.Add((subFolders.Count-1).ToString());
            files.Add(fileNameTextBox.Text);
        }

        private void addCoordinates()
        {
            coordinates.Add((files.Count - 2).ToString());
            coordinates.Add(coordinatesTextBox.Text);
        }

        private void save()
        {
            // Specify a name for your top-level folder. 
            String folderName = System.IO.Path.Combine(directoryTextBox.Text, mainFolderTextBox.Text);

            //make main Directory
            System.IO.Directory.CreateDirectory(folderName);

            //add factor file
            // Use Combine again to add the file name to the path.
            String factorPathString = System.IO.Path.Combine(folderName, "Factors.txt");

            // Check that the file doesn't already exist.
            // DANGER: System.IO.File.Create will overwrite the file if it already exists.
            if (!System.IO.File.Exists(factorPathString))
            {
                // Create a file to write to. 
                using (System.IO.StreamWriter sw = System.IO.File.CreateText(factorPathString))
                {
                    //iterate through each coordinate string in each file
                    for (int p = 0; p < factors.Count; p+=3 )
                    {
                        sw.WriteLine(factors[p] + "," + factors[p+1] + "," + factors[p+2]);
                    }
                }
            }
            else
            {
                Console.WriteLine("File \"{0}\" already exists.", "Factors.txt");
                return;
            }

            //add data folders
            //int to keep track of file iteration
            int f = 0;

            //int to keep track of coordinate string iteration
            int c = 0;

            //iterate through each subfolder
            for (int i = 0; i < subFolders.Count; i++)
            {
                // To create a string that specifies the path to a subfolder under your  
                // top-level folder, add a name for the subfolder to folderName. 
                String folderPathString = System.IO.Path.Combine(folderName, subFolders[i]);

                //make sub-Directory
                System.IO.Directory.CreateDirectory(folderPathString);

                //stop iteration when reach files from other folder
                Boolean stopFile = false;

                //iterate through each file in each subfolder
                while (f < files.Count && !stopFile)
                {
                    if (Convert.ToInt32(files[f]) == i)
                    {
                        // Use Combine again to add the file name to the path.
                        String filePathString = System.IO.Path.Combine(folderPathString, files[f+1]);

                        // Check that the file doesn't already exist.
                        // DANGER: System.IO.File.Create will overwrite the file if it already exists.
                        if (!System.IO.File.Exists(filePathString))
                        {
                            // Create a file to write to. 
                            using (System.IO.StreamWriter sw = System.IO.File.CreateText(filePathString + ".txt"))
                            {
                                Boolean stopCoord = false;

                                //iterate through each coordinate string in each file
                                while (c < coordinates.Count && !stopCoord)
                                {
                                    if (Convert.ToInt32(coordinates[c]) == f)
                                    {
                                        sw.WriteLine(coordinates[c+1]);
                                        c += 2;
                                    }
                                    else
                                        stopCoord = true;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("File \"{0}\" already exists.", files[f]);
                            return;
                        }

                        f+=2;
                    }
                    else
                        stopFile = true;
                }
            }
        }
    }
}
