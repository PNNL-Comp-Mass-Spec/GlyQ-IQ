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
using System.Threading;

namespace GridView4D
{
    /// <summary>
    /// Interaction logic for ListPage.xaml
    /// </summary>
    public partial class ListPage : Page
    {
        ListBox listBoxFolders;
        ListBox listBoxFiles;
        ListBox listBoxFactors;
        ListBox listBoxOrderFactors;
        //list filled with empty strings on top of files listbox to allow doubling of file names
        ListBox listBoxTopFiles;
        //list to keep track of what is in the files listbox
        List<String> filesBoxStr;
        //list of selected folder indexes
        List<int> sIndex;
        //fators list
        List<String> factorsStr;
        //change factor description
        string[] text = { "YPrim", "XPrim", "YSec", "XSec" };

        public ListPage()
        {
            InitializeComponent();

            #region main grid add controls
            //main grid
            Grid grid = new Grid();
            grid.ShowGridLines = true;

            //1st column for folders to select
            //2nd column for files to select
            //3rd column for factors to select
            //4th column for buttons to open grid pages
            buildGrid.addCol(grid, 4, 0);

            //1st row for titles
            //2nd row for controls
            buildGrid.addRow(grid, 1, 30);
            buildGrid.addRow(grid, 1, 0);

            //title labels
            Label folderLabel = new Label();
            folderLabel.Content = "Folders";
            Grid.SetColumn(folderLabel, 0);
            Grid.SetRow(folderLabel, 0);

            Label fileLabel = new Label();
            fileLabel.Content = "Files";
            Grid.SetColumn(fileLabel, 1);
            Grid.SetRow(fileLabel, 0);

            Label factorLabel = new Label();
            factorLabel.Content = "Factors";
            Grid.SetColumn(factorLabel, 2);
            Grid.SetRow(factorLabel, 0);

            Label buttonLabel = new Label();
            buttonLabel.Content = "Grid Type";
            Grid.SetColumn(buttonLabel, 3);
            Grid.SetRow(buttonLabel, 0);

            //list of selected folder indexes
            sIndex = new List<int>();

            //add folders list box to 1st column
            listBoxFolders = new ListBox();
            listBoxFolders.SelectionMode = SelectionMode.Extended;
            listBoxFolders.SelectionChanged += new SelectionChangedEventHandler(listBoxFolders_SelectedIndexChanged);
            Grid.SetColumn(listBoxFolders, 0);
            Grid.SetRow(listBoxFolders, 1);

            //add folder names to listbox
            for (int i = 0; i < App.folderList.Count; i++)
                listBoxFolders.Items.Add(App.folderList[i][0]);

            //add files list box to 2nd column
            listBoxFiles = new ListBox();
            listBoxFiles.SelectionMode = SelectionMode.Extended;
            Grid.SetColumn(listBoxFiles, 1);
            Grid.SetRow(listBoxFiles, 1);

            //add listBoxTopFiles to 2nd column to keep individual seleections
            listBoxTopFiles = new ListBox();
            listBoxTopFiles.SelectionMode = SelectionMode.Extended;
            listBoxTopFiles.Foreground = new SolidColorBrush(Colors.Transparent);
            listBoxTopFiles.Background = new SolidColorBrush(Colors.Transparent);
            SolidColorBrush scb = new SolidColorBrush(Colors.Blue);
            scb.Opacity = 0.25;
            listBoxTopFiles.Resources.Add(SystemColors.HighlightBrushKey, scb);
            listBoxTopFiles.Resources.Add(SystemColors.HighlightTextBrushKey, new SolidColorBrush(Colors.Transparent));
            listBoxTopFiles.Resources.Add(SystemColors.ControlBrushKey, scb);
            listBoxTopFiles.Resources.Add(SystemColors.ControlTextBrushKey, new SolidColorBrush(Colors.Transparent));
            filesBoxStr = new List<String>();
            Grid.SetColumn(listBoxTopFiles, 1);
            Grid.SetRow(listBoxTopFiles, 1);

            //add factors list box to 3rd column
            listBoxFactors = new ListBox();
            listBoxFactors.Resources.Add(SystemColors.HighlightBrushKey, scb);
            listBoxFactors.Resources.Add(SystemColors.ControlBrushKey, scb);
            listBoxFactors.Background = new SolidColorBrush(Colors.Transparent);
            listBoxFactors.SelectionMode = SelectionMode.Extended;
            factorsStr = new List<String>();
            listBoxFactors.SelectionChanged += new SelectionChangedEventHandler(listBoxFactors_SelectedIndexChanged);
            Grid.SetColumn(listBoxFactors, 2);
            Grid.SetRow(listBoxFactors, 1);

            //add listbx behind factors listbox to show selection order
            listBoxOrderFactors = new ListBox();
            Grid.SetColumn(listBoxOrderFactors, 2);
            Grid.SetRow(listBoxOrderFactors, 1);

            //add factors to list box
            char[] delimiterChars = { ',' };
            for (int i = 0; i < App.factorsList.Count; i++)
            {
                string[] factorsTemp = App.factorsList[i].Split(delimiterChars);
                listBoxFactors.Items.Add(factorsTemp[0]);
                listBoxOrderFactors.Items.Add(factorsTemp[0]);
                factorsStr.Add(factorsTemp[0]);
            }

            //add button to 2nd column - compbines to make one graph
            Button gridButtonOne = new Button();
            Grid.SetColumn(gridButtonOne, 3);
            Grid.SetRow(gridButtonOne, 1);
            gridButtonOne.Content = "One Grid";
            gridButtonOne.Style = (Style)FindResource("buttonStyle");
            gridButtonOne.Margin = new Thickness(0,0,0,80);
            gridButtonOne.Click += new RoutedEventHandler(this.gridButton_Click);

            //add button to 2nd column - makes multiple graphs
            Button gridButtonMult = new Button();
            Grid.SetColumn(gridButtonMult, 3);
            Grid.SetRow(gridButtonMult, 1);
            gridButtonMult.Name = "gridButtonAll";
            gridButtonMult.Content = "Separate Grids";
            gridButtonMult.Style = (Style)FindResource("buttonStyle");
            gridButtonMult.Margin = new Thickness(0, 80, 0, 0);
            gridButtonMult.Click += new RoutedEventHandler(this.gridButton_Click);

            //add controls to grid
            grid.Children.Add(folderLabel);
            grid.Children.Add(fileLabel);
            grid.Children.Add(factorLabel);
            grid.Children.Add(buttonLabel);
            grid.Children.Add(listBoxFolders);
            grid.Children.Add(listBoxFiles);
            grid.Children.Add(listBoxTopFiles);
            grid.Children.Add(listBoxOrderFactors);
            grid.Children.Add(listBoxFactors);
            grid.Children.Add(gridButtonOne);
            grid.Children.Add(gridButtonMult);

            //put grid in window
            this.Content = grid;
            #endregion
        }

        //change files based on selected folders
        private void listBoxFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxFiles.Items.Clear();
            listBoxTopFiles.Items.Clear();
            filesBoxStr.Clear();

            sIndex.Clear();

            //add file names to listbox
            foreach (Object selectedItem in listBoxFolders.SelectedItems)
            {
                sIndex.Add(listBoxFolders.Items.IndexOf(selectedItem));
            }

            //order list from lowest (0) to highest #
            //bubble sort
            Boolean done = false;
            while (!done)
            {
                Boolean noChanges = true;
                for (int s = 1; s < sIndex.Count; s++)
                {
                    if (sIndex[s - 1] > sIndex[s])
                    {
                        int temp = sIndex[s - 1];
                        sIndex[s - 1] = sIndex[s];
                        sIndex[s] = temp;

                        noChanges = false;
                    }
                }

                if (noChanges)
                    done = true;
            }

            //int to track incrementing of invisible listbox items
            int incListBox = 0;

            //output files in order - files from folder 0 first
            for (int s = 0; s < sIndex.Count; s++)
            {
                for (int i = 1; i < App.folderList[sIndex[s]].Count; i++)
                {
                    listBoxFiles.Items.Add(App.folderList[sIndex[s]][i]);
                    listBoxTopFiles.Items.Add((incListBox + i).ToString());
                    filesBoxStr.Add(App.folderList[sIndex[s]][0] + " " + App.folderList[sIndex[s]][i]);
                }
                //int to track incrementing of invisible listbox items
                incListBox += App.folderList[sIndex[s]].Count-1;
            }
        }

        //number selected factors in order of selection so user can "remap" factors
        private void listBoxFactors_SelectedIndexChanged(object sender, EventArgs e)
        {
            //count through different text options
            //reset for different selection
            int index = 0;

            //clear old text
            listBoxOrderFactors.Items.Clear();
            
            //reset base options
            for (int i = 0; i < factorsStr.Count; i++)
            {
                listBoxOrderFactors.Items.Add(factorsStr[i]);
            }

            foreach (String selectedItem in listBoxFactors.SelectedItems)
            {
                //change listbox item text
                listBoxOrderFactors.Items[listBoxFactors.Items.IndexOf(selectedItem)] = 
                    factorsStr[listBoxFactors.Items.IndexOf(selectedItem)] + " " + text[index];
                //increment through text order tags
                index++;
                if (index > 3)
                    index = 0;
            }
        }

        //open grid/s
        void gridButton_Click(Object sender, RoutedEventArgs e)
        {
            //clear universal lists
            App.backgroundColorProperty.Clear();
            App.gridWindowList.Clear();

            Button from = (Button)sender;
            if (from.Name == "gridButtonAll")
            {
                //get the indexes of the selected factors
                List<int> factorsIndexList = new List<int>();
                foreach (Object selectedFactor in listBoxFactors.SelectedItems)
                {
                    factorsIndexList.Add(listBoxFactors.Items.IndexOf(selectedFactor));

                    //separate out data (Ex: 1, 2, 3, 4)
                    char[] delimiterChars1 = { ',' };
                    string[] dataRead = App.data[0][0][2].Split(delimiterChars1);
                    if ((listBoxFactors.Items.IndexOf(selectedFactor)) > (dataRead.Length-1))
                    {
                        MessageBox.Show("Files must contain the same number of coordinates as there are factors");
                        return;
                    }
                }

                if (factorsIndexList.Count > 4 || factorsIndexList.Count < 1)
                    MessageBox.Show("1 to 4 factors need to be selected.");
                else
                {
                    //create loading pop up window running on separate thread
                    /*PopUpWorker loadWorkerObject = new PopUpWorker();
                    Thread loadWorkerThread = new Thread(loadWorkerObject.DoLoadWork);
                    loadWorkerObject.RequestStart();

                    loadWorkerThread.SetApartmentState(ApartmentState.STA);
                    loadWorkerThread.Start();
                    while (!loadWorkerThread.IsAlive) ;
                    Thread.Sleep(1);*/

                    //separate windows
                    //keeps previous folder file counts
                    int numFolderFiles = 0;
                    //save files in order - files from folder 0 first
                    for (int s = 0; s < sIndex.Count; s++)
                    {
                        //list out folder names
                        List<String> folder = new List<String>();
                        folder.Add(App.folderList[sIndex[s]][0]);

                        //only get selected files
                        List<List<String>> tempFileDataList = new List<List<String>>();

                        //get the files selected using listBoxTopFiles
                        foreach (Object selectedItem2 in listBoxTopFiles.SelectedItems)
                        {
                            if ((listBoxTopFiles.Items.IndexOf(selectedItem2) - numFolderFiles >= 0) &&
                                !(listBoxTopFiles.Items.IndexOf(selectedItem2) - numFolderFiles >
                                App.data[sIndex[s]].Count - 1)
                                && App.data[sIndex[s]]
                                [listBoxTopFiles.Items.IndexOf(selectedItem2) - numFolderFiles]
                                [0].Equals(filesBoxStr[listBoxTopFiles.Items.IndexOf(selectedItem2)]))
                            {
                                tempFileDataList.Add(App.data[sIndex[s]]
                                    [listBoxTopFiles.Items.IndexOf(selectedItem2) - numFolderFiles]);
                            }
                        }

                        //increment number of folders to skip over
                        numFolderFiles += App.data[sIndex[s]].Count;

                        //instance of GridWindow
                        //send list of files in selected folder and the selected factors
                        GridWindow gridWindow = new GridWindow(tempFileDataList, factorsIndexList,
                            folder);
                        //add GridWindow to list to coordinate zoom and scroll
                        App.gridWindowList.Add(gridWindow);

                        //stop load pop up window thread
                        //loadWorkerObject.RequestStop();

                        //open grid page
                        gridWindow.Show();
                    }
                }
            }
            else
            {
                //create loading pop up window running on separate thread
                /*PopUpWorker loadWorkerObject = new PopUpWorker();
                Thread loadWorkerThread = new Thread(loadWorkerObject.DoLoadWork);
                loadWorkerObject.RequestStart();

                loadWorkerThread.SetApartmentState(ApartmentState.STA);
                loadWorkerThread.Start();
                while (!loadWorkerThread.IsAlive) ;
                Thread.Sleep(1);*/

                //one window
                //get the indexes of the selected factors
                List<int> factorsIndexList = new List<int>();
                foreach (Object selectedFactor in listBoxFactors.SelectedItems)
                {
                    factorsIndexList.Add(listBoxFactors.Items.IndexOf(selectedFactor));

                    //separate out data (Ex: 1, 2, 3, 4)
                    char[] delimiterChars1 = { ',' };
                    string[] dataRead = App.data[0][0][2].Split(delimiterChars1);
                    if ((listBoxFactors.Items.IndexOf(selectedFactor)) > (dataRead.Length-1))
                    {
                        MessageBox.Show("Files must contain the same number of coordinates as there are factors");
                        return;
                    }
                }

                if (factorsIndexList.Count > 4 || factorsIndexList.Count < 1)
                    MessageBox.Show("1 to 4 factors need to be selected.");
                else
                {
                    //list out folder names
                    List<String> folder = new List<String>();

                    //get the folders and data selected
                    List<List<List<String>>> dataList = new List<List<List<String>>>();
                    //keeps previous folder file counts
                    int numFolderFiles = 0;
                    //save files in order - files from folder 0 first
                    for (int s = 0; s < sIndex.Count; s++)
                    {
                        //only get selected files
                        List<List<String>> tempFileDataList = new List<List<String>>();

                        //get the files selected using listBoxTopFiles
                        foreach (Object selectedItem2 in listBoxTopFiles.SelectedItems)
                        {
                            if ((listBoxTopFiles.Items.IndexOf(selectedItem2) - numFolderFiles >= 0) &&
                                !(listBoxTopFiles.Items.IndexOf(selectedItem2) - numFolderFiles >
                                App.data[sIndex[s]].Count - 1) &&
                                App.data[sIndex[s]]
                                [listBoxTopFiles.Items.IndexOf(selectedItem2) - numFolderFiles]
                                [0].Equals(filesBoxStr[listBoxTopFiles.Items.IndexOf(selectedItem2)]))
                            {
                                tempFileDataList.Add(App.data[sIndex[s]]
                                    [listBoxTopFiles.Items.IndexOf(selectedItem2) - numFolderFiles]);
                            }
                        }

                        //increment number of folders to skip over
                        numFolderFiles += App.data[sIndex[s]].Count;

                        dataList.Add(tempFileDataList);
                        folder.Add(App.folderList[sIndex[s]][0]);
                    }

                    //instance of GridWindow - send list of selected folders and factors
                    GridWindow gridWindow = new GridWindow(dataList, factorsIndexList, folder);
                    //add GridWindow to list to coordinate zoom and scroll
                    App.gridWindowList.Add(gridWindow);

                    //stop load pop up window thread
                    //loadWorkerObject.RequestStop();

                    //open grid page
                    gridWindow.ShowDialog();
                }
            }
        }
    }
}
