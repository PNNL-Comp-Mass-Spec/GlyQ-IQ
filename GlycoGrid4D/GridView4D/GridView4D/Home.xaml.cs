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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections;

namespace GridView4D
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            App.data = new List<List<List<String>>>();
            App.folderList = new List<List<String>>();
            App.factorsList = new List<String>();
            App.gridWindowList = new List<GridWindow>();
            App.backgroundColorProperty = new List<App.BackgroundColorProperty>();
            App.fontSizeProperty = new App.FontSizeProperty();
            App.multSquares = true;
            App.mainColor = 0;
            
            try
            {
                using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "\\appdata.txt"))
                {
                    App.recentFolderPath = sr.ReadLine();
                }
            }
            catch (Exception e)
            {
            }

            InitializeComponent();

            browseTextBox.Text = App.recentFolderPath;
        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            //create FolderBrowserDialog
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            //select starting path
            dlg.SelectedPath = App.recentFolderPath;

            //display FolderBrowserDialog
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();

            //Get the selected file name, save it, and display in a TextBox
            App.recentFolderPath = dlg.SelectedPath;
            browseTextBox.Text = App.recentFolderPath;
        }

        public Boolean readTextFiles(String folder, int num)
        {
            try
            {
                //add file of factors to factorsList
                //get file name
                string[] files2 = Directory.GetFiles(folder);
                String fileNameTemp = (System.IO.Path.GetFileName(files2[0])).Substring(0,7);

                if (!(files2.Length == 0) && fileNameTemp.Equals("Factors"))
                {
                    //reads file
                    for (int i = 0; i < files2.Length; i++)
                    {
                        using (StreamReader sr = new StreamReader(files2[i]))
                        {
                            String line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                //adds each line separately to string list
                                App.factorsList.Add(line);
                            }
                        }
                    }

                    //add folder info to data list
                    //gets each folder path
                    String[] subdirs = Directory.GetDirectories(folder);

                    //iterate through each folder
                    for (int f = 0; f < subdirs.Length; f++)
                    {
                        //gets each folder path
                        String folderTemp = subdirs[f];

                        //add folder name to display in legend - data[0] cooresponds with folderList[0]
                        List<String> filesTemp = new List<String>();
                        filesTemp.Add(System.IO.Path.GetFileName(folderTemp));

                        //temporary holder
                        List<List<String>> dataHolderTemp = new List<List<String>>();

                        //gets all files names
                        string[] files = Directory.GetFiles(folderTemp);

                        //reads files
                        for (int i = 0; i < files.Length; i++)
                        {
                            using (StreamReader sr = new StreamReader(@files[i]))
                            {
                                //temporary holders
                                String line;
                                List<String> dataTemp = new List<String>();
                                //add file name to keep track of which files to display
                                filesTemp.Add(System.IO.Path.GetFileName(files[i]));
                                //add file name to display in legend and keep track of collisions
                                dataTemp.Add(filesTemp[0] + " " + System.IO.Path.GetFileName(files[i]));
                                //add each line of text - factors line then data lines
                                while ((line = sr.ReadLine()) != null)
                                {
                                    //1st line is color
                                    //adds each line separately to string list
                                    dataTemp.Add(line);
                                }
                                //add to temporary 2nd level holder
                                dataHolderTemp.Add(dataTemp);
                            }
                        }
                        //data - holds list of the folders in the root folder
                        //dataHolderTemp - holds list of the files in each folder
                        //dataTemp - holds list of the data in each file
                        //0 - folder + file name
                        //1 - color for file
                        //add dataHolderTemp to data
                        App.data.Add(dataHolderTemp);

                        //add list of files
                        // 0 - folder name
                        // >0 - file names
                        App.folderList.Add(filesTemp);
                    }

                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
                return false;
            }
        }

        private void listButton_Click(object sender, RoutedEventArgs e)
        {
            //flag to check factors file exists
            Boolean factors = true;

            //reads in textfile using method
            if (browseTextBox.Text.Trim().Length != 0)
                factors = readTextFiles(browseTextBox.Text, 1);

            if (factors)
            {
                //instance of ListPage
                ListPage listPage = new ListPage();
                //navigates to list page
                this.NavigationService.Navigate(listPage);
            }
            else
                MessageBox.Show("No factors file present");
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            //instance of CreateFilesWindow
            CreateFilesWindow createFilesWindow = new CreateFilesWindow();
            //open createFilesWindow
            createFilesWindow.Show();
        }
    }
}
