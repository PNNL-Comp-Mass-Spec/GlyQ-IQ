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
using System.IO;
using System.Threading;
using System.ComponentModel;

namespace GridView4D
{
    /// <summary>
    /// Interaction logic for GridWindow.xaml
    /// </summary>
    public partial class GridWindow : Window
    {
        #region main attributes
        //layout
        ScrollViewer sViewer;
        Grid windowGrid;
        Grid grid;
        Grid gridF1;
        Grid gridF2;
        Grid gridTop;
        Grid gridLeft;
        Grid gridData;
        Grid gridLegend;

        //data
        List<List<List<String>>> dataList;
        List<List<String>> data;
        List<int> factorsIndexList;
        List<String> folder;
        //flag to tell if all-in-one window or multiple windows
        Boolean multipleWindows;
        //get selected factors, min, max
        List<String> factors;
        //keeps track of what section each new textbox in 1st factor should be in
        int c1;
        //keeps track of what section each new textbox in 2nd factor should be in
        int c2;
        //keeps track of what section each new textbox in 3rd factor should be in
        int c3;
        //keeps track of what section each new textbox in 4th factor should be in
        int c4;
        //list of file names
        List<String> fileList;
        //if there are multiple windows the color index bound to needs to be incremented higher
        int bypassColors;

        //controls
        //so handlers can be removed
        Button save;
        Button changeColor;
        Button changeView;

        //event handler to remove dynamically added handler
        MouseButtonEventHandler mouseEvent;
        #endregion

        //constructor for multiple windows
        public GridWindow(List<List<String>> datap, List<int> factorsIndexListp, List<String> folderp)
        {
            InitializeComponent();

            setStart(null, datap, factorsIndexListp, folderp);

            createGrid();
        }

        //constructor for one window
        public GridWindow(List<List<List<String>>> dataListp, List<int> factorsIndexListp, List<String> folderp)
        {
            InitializeComponent();

            setStart(dataListp, null, factorsIndexListp, folderp);

            createGrid();
        }

        //instantiate variables
        private void setStart(List<List<List<String>>> dataListp, List<List<String>> datap,
            List<int> factorsIndexListp, List<String> folderp)
        {
            dataList = dataListp;
            data = datap;
            factorsIndexList = factorsIndexListp;
            folder = folderp;
            multipleWindows = true;
            factors = new List<String>();
            c1 = 0;
            c2 = 0;
            c3 = 0;
            c4 = 0;
            App.scale = 1;
            App.multSquares = true;
            App.readColors = true;
            App.saveSizeIndex = 0;
            App.originalFontSize = 1;
            mouseEvent = new MouseButtonEventHandler(mouseDown);
            fileList = new List<String>();
            if (multipleWindows)
                bypassColors = App.backgroundColorProperty.Count;
            else
                bypassColors = 0;
        }

        //build window design
        private void createGrid()
        {
            windowGrid = new Grid();
            grid = new Grid();

            #region scrollViewer
            //declare scroll viewer
            sViewer = new ScrollViewer();

            //set scrollviewer attributes
            sViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            sViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            #endregion

            #region flags to check if one window or multiple are needed
            //flag to tell if all-in-one window or multiple windows
            if (data == null)
                multipleWindows = false;

            //flag to tell if data was sent
            Boolean dataSent = true;
            if (multipleWindows)
            {
                if (data.Count == 0)
                    dataSent = false;
            }
            else
            {
                if (dataList.Count == 0)
                    dataSent = false;
            }
            #endregion

            if (!dataSent)
            {
                #region no files warning
                //if the textfile is empty, only add warning textbox to grid
                TextBox warning = new TextBox();
                warning.Text = "No data in file";
                windowGrid.Children.Add(warning);

                //set height and width
                this.Height = 450;
                this.Width = 450;
                #endregion
            }
            else
            {
                #region window size
                //set minwidth and minheight of page
                this.MinHeight = 350;
                this.MinWidth = 350;

                //set height and width
                this.Height = 890;
                this.Width = 1070;
                #endregion

                #region grid declarations/attributes
                //declare mini grids
                gridF1 = new Grid();
                gridF2 = new Grid();
                gridTop = new Grid();
                gridLeft = new Grid();
                gridData = new Grid();
                gridLegend = new Grid();

                //place grids
                //3rd row and 2nd column
                Grid.SetRow(gridF1, 2);
                Grid.SetColumn(gridF1, 1);
                //3rd column and 2nd row
                Grid.SetRow(gridF2, 1);
                Grid.SetColumn(gridF2, 2);
                //3rd column and 1st row
                Grid.SetRow(gridTop, 0);
                Grid.SetColumn(gridTop, 2);
                //3rd row and 1st column
                Grid.SetRow(gridLeft, 2);
                Grid.SetColumn(gridLeft, 0);
                //3rd row and column
                Grid.SetRow(gridData, 2);
                Grid.SetColumn(gridData, 2);
                //3rd row and 4th column
                Grid.SetRow(gridLegend, 2);
                Grid.SetColumn(gridLegend, 3);

                //make grids visible
                windowGrid.ShowGridLines = true;
                grid.ShowGridLines = true;
                gridF1.ShowGridLines = true;
                gridF2.ShowGridLines = true;
                gridLeft.ShowGridLines = true;
                gridTop.ShowGridLines = true;
                gridData.ShowGridLines = true;

                //graph grid settings
                grid.Height = 800;
                grid.Width = 1040;
                //Background needs to be set for mouseleftbuttondown event
                //to be handled by mousedown event handler
                grid.Background = new SolidColorBrush(Colors.White);
                //to make columns and rows equal
                Grid.SetIsSharedSizeScope(grid, true);

                //main grid
                //white to give white background when saved as png
                windowGrid.Background = new SolidColorBrush(Colors.White);
                #endregion

                #region main and graph grid columns and rows
                //graph grid
                //first separate titles of factors from main gridData
                buildGrid.addRow(grid, 2, 40);
                buildGrid.addRow(grid, 1, 0);

                buildGrid.addCol(grid, 2, 40);
                buildGrid.addCol(grid, 1, 0);
                
                //for key
                buildGrid.addCol(grid, 1, 200);

                //main grid
                //row for toolbar and graph grid
                buildGrid.addRow(windowGrid, 1, 30);
                buildGrid.addRow(windowGrid, 1, 0);

                //column for graph grid and key
                buildGrid.addCol(windowGrid, 1, 0);
                #endregion

                #region toolBar
                ToolBarTray toolBarTray = new ToolBarTray();
                ToolBar toolBar = new ToolBar();

                //save bitmap (png) button
                save = new Button();
                save.Content = "Save";
                save.Click += new RoutedEventHandler(saveButton_Click);

                //change color of text file button
                changeColor = new Button();
                changeColor.Content = "Change File Color";
                changeColor.Click += new RoutedEventHandler(changeColorButton_Click);

                //change view of collisions
                changeView = new Button();
                changeView.Content = "Change Grid View";
                changeView.Click += new RoutedEventHandler(changeViewButton_Click);

                //add buttons to toolbar
                toolBar.Items.Add(save);
                toolBar.Items.Add(changeColor);
                toolBar.Items.Add(changeView);

                //add toolbar to tray
                toolBar.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                toolBarTray.ToolBars.Add(toolBar);
                Grid.SetRow(toolBarTray, 0);
                windowGrid.Children.Add(toolBarTray);
                #endregion

                #region minigrids - titles
                //add titles of factors
                //get selected factors, min, max
                char[] delimiterChars = { ',' };
                for (int i = 0; i < factorsIndexList.Count; i++)
                {
                    string[] factorsTemp = App.factorsList[factorsIndexList[i]].Split(delimiterChars);
                    for (int f = 0; f < factorsTemp.Length; f++)
                        factors.Add(factorsTemp[f].Trim());
                }

                //flag to add title of factor to first textbox
                Boolean first = true;

                if (factorsIndexList.Count > 0)
                {
                    //1st factor goes in 2nd column on the left
                    for (int i = Convert.ToInt32(factors[1]); i <= Convert.ToInt32(factors[2]); i++)
                    {
                        //add row
                        buildGrid.addRow(gridF1, 1, 0);

                        //also add row to 1st column
                        buildGrid.addRow(gridLeft, 1, 0);

                        //also add row to main gridData if less than 3 factors
                        if (factorsIndexList.Count < 3)
                            buildGrid.addRow(gridData, 1, 0);

                        //textbox to hold titles
                        TextBox factor1 = new TextBox();
                        if (first)
                        {
                            factor1.Text = i.ToString() + "\n" + factors[0];
                            factor1.TextWrapping = TextWrapping.Wrap;
                            first = false;
                        }
                        else
                            factor1.Text = i.ToString();
                        
                        factor1.IsReadOnly = true;

                        //add title textboxes to grid
                        Grid.SetRow(factor1, c1);
                        c1++;
                        gridF1.Children.Add(factor1);

                        //add bold grid lines
                        buildGrid.addHoriz(gridF1, c1, 1);
                        buildGrid.addHoriz(gridLeft, c1, 1);
                    }
                }

                //reset first flag for factor title
                first = true;

                if (factorsIndexList.Count > 1)
                {
                    //2nd factor goes in 2nd row on top
                    for (int i = Convert.ToInt32(factors[4]); i <= Convert.ToInt32(factors[5]); i++)
                    {
                        //add row
                        buildGrid.addCol(gridF2, 1, 0);

                        //also add column to 1st row
                        buildGrid.addCol(gridTop, 1, 0);

                        //also add column to main gridData if less than 4 factors
                        if (factorsIndexList.Count < 4)
                            buildGrid.addCol(gridData, 1, 0);

                        //textbox to hold titles
                        TextBox factor2 = new TextBox();
                        if (first)
                        {
                            factor2.Text = i.ToString() + "\n" + factors[3];
                            factor2.TextWrapping = TextWrapping.Wrap;
                            first = false;
                        }
                        else
                            factor2.Text = i.ToString();
                        factor2.IsReadOnly = true;

                        //add title textboxes to grid
                        Grid.SetColumn(factor2, c2);
                        c2++;
                        gridF2.Children.Add(factor2);

                        //add bold grid lines
                        buildGrid.addVert(gridF2, c2, 1);
                        buildGrid.addVert(gridTop, c2, 1);
                    }
                }

                //reset first flag for factor title
                first = true;

                if (factorsIndexList.Count > 2)
                {
                    //creates multiple of 1st column grid (3rd factor) - 1 for each title in 2nd column
                    for (int f = 0; f < c1; f++)
                    {
                        //make multiple 1st column grids
                        Grid gridF3 = new Grid();
                        gridF3.ShowGridLines = true;

                        //reset count of cell
                        c3 = 0;

                        //3rd factor goes in 1st column on left
                        for (int i = Convert.ToInt32(factors[7]); i <= Convert.ToInt32(factors[8]); i++)
                        {
                            buildGrid.addRow(gridF3, 1, 0);

                            //also add row to main gridData
                            buildGrid.addRow(gridData, 1, 0);

                            //textbox to hold titles
                            TextBox factor3 = new TextBox();
                            if (first)
                            {
                                //add textbox with title name
                                TextBox titleTemp = new TextBox();
                                titleTemp.Text = factors[6];
                                titleTemp.IsReadOnly = true;
                                titleTemp.TextWrapping = TextWrapping.Wrap;
                                Grid.SetRow(titleTemp, 1);
                                Grid.SetColumn(titleTemp, 0);
                                grid.Children.Add(titleTemp);

                                factor3.Text = i.ToString();
                                first = false;
                            }
                            
                            factor3.Text = i.ToString();
                            factor3.IsReadOnly = true;

                            //add title textboxes to grid
                            Grid.SetRow(factor3, c3);
                            c3++;
                            gridF3.Children.Add(factor3);
                        }

                        //then add a gridF3 to each row
                        Grid.SetRow(gridF3, f);
                        gridLeft.Children.Add(gridF3);
                        buildGrid.addHoriz(gridLeft, c1, 1);
                    }
                }

                //reset first flag for factor title
                first = true;

                if (factorsIndexList.Count > 3)
                {
                    //creates multiple of 1st column grid (4th factor) - 1 for each title in 2nd column
                    for (int f = 0; f < c2; f++)
                    {
                        //make multiple 1st row grids
                        Grid gridF4 = new Grid();
                        gridF4.ShowGridLines = true;

                        //reset count of cell
                        c4 = 0;

                        //4th factor goes in 1st row on top
                        for (int i = Convert.ToInt32(factors[10]); i <= Convert.ToInt32(factors[11]); i++)
                        {
                            buildGrid.addCol(gridF4, 1, 0);

                            //also add column to main gridData
                            buildGrid.addCol(gridData, 1, 0);

                            //textbox to hold titles
                            TextBox factor4 = new TextBox();
                            if (first)
                            {
                                //add textbox with title name
                                TextBox titleTemp = new TextBox();
                                titleTemp.Text = factors[9];
                                titleTemp.IsReadOnly = true;
                                titleTemp.TextWrapping = TextWrapping.Wrap;
                                Grid.SetRow(titleTemp, 0);
                                Grid.SetColumn(titleTemp, 1);
                                grid.Children.Add(titleTemp);

                                factor4.Text = i.ToString();
                                first = false;
                            }
                            
                            factor4.Text = i.ToString();
                            factor4.IsReadOnly = true;

                            //add title textboxes to grid
                            Grid.SetColumn(factor4, c4);
                            c4++;
                            gridF4.Children.Add(factor4);
                        }

                        //then add a gridF4 to each column
                        Grid.SetColumn(gridF4, f);
                        gridTop.Children.Add(gridF4);
                        buildGrid.addVert(gridTop, c2, 1);
                    }
                }

                //add all mini grids to graph grid
                grid.Children.Add(gridF1);
                grid.Children.Add(gridF2);
                grid.Children.Add(gridTop);
                grid.Children.Add(gridLeft);
                grid.Children.Add(gridData);
                #endregion

                #region text binding base set font size
                //set start height of font
                App.originalFontSize = Convert.ToInt32(Math.Truncate(
                                                (double) 750 / gridData.RowDefinitions.Count / 2));
                App.fontSizeProperty.FontSizeProp = App.originalFontSize;
                App.fontSizeProperty.SmallFontSizeProp = App.originalFontSize / 2;
                #endregion

                #region read data
                readData();
                #endregion

                #region bold grid lines
                //Add bold grid lines
                //(grid, number of lines, number of lines to skip)
                // # of lines to skip = 1 if skip none
                if (factorsIndexList.Count > 3)
                {
                    buildGrid.addVert(gridData, c2, c4);
                    buildGrid.addHoriz(gridData, c1, c3);
                }
                else if (factorsIndexList.Count > 2)
                {
                    buildGrid.addVert(gridData, c2, 1);
                    buildGrid.addHoriz(gridData, c1, c3);
                } 
                else if (factorsIndexList.Count > 1)
                {
                    buildGrid.addVert(gridData, c2, 1);
                    buildGrid.addHoriz(gridData, c1, 1);
                } 
                else if (factorsIndexList.Count > 0)
                {
                    buildGrid.addHoriz(gridData, c1, 1);
                }
                #endregion

                #region legend
                //legend goes furthest to right in 4th column
                //keeps track of what section each new textbox in legend should be in
                int legendRow = 0;

                //first textbox in legend is title
                //add a row
                buildGrid.addRow(gridLegend, 1, 40);

                //textbox to hold legend title
                TextBox legend = new TextBox();
                legend.Text = "Legend : " + folder[0];
                for(int g = 1; g < folder.Count; g++)
                    legend.Text += ", " + folder[g];
                legend.TextWrapping = TextWrapping.Wrap;
                legend.TextAlignment = TextAlignment.Center;
                legend.BorderThickness = new Thickness(2);
                legend.BorderBrush = new SolidColorBrush(Colors.Black);
                legend.IsReadOnly = true;

                //add textboxe to grid
                Grid.SetRow(legend, legendRow);
                legendRow++;
                gridLegend.Children.Add(legend);

                //Legend is filled with each file name and coresponding color
                //keep track of number of files in each folder
                int numFilesInFolder = 0;
                //iterate for number of folders
                // 1 - multiple windows
                // >1 - all-in-one window
                for (int w = 0; w < folder.Count; w++)
                {
                    //holds file
                    List<List<String>> dataListTemp = new List<List<String>>();
                    if (multipleWindows)
                        dataListTemp = data;
                    else
                        dataListTemp = dataList[w];

                    for (int i = 0; i < dataListTemp.Count; i++)
                    {
                        //iterate through each data list
                        List<String> dataTemp = dataListTemp[i];
                        fileList.Add(dataTemp[0]);

                        //add two rows
                        buildGrid.addRow(gridLegend, 1, 40);
                        buildGrid.addRow(gridLegend, 1, 30);

                        //textbox to hold file names
                        TextBox legend1 = new TextBox();
                        legend1.Text = dataTemp[0];
                        legend1.TextWrapping = TextWrapping.Wrap;
                        legend1.IsReadOnly = true;
                        legend1.BorderBrush = new SolidColorBrush(Colors.Transparent);

                        //textbox to hold color
                        TextBox legend2 = new TextBox();
                        legend2.IsReadOnly = true;
                        //bind background of textbox to color in list
                        //source uses a FilesSCB class
                        Binding b = new Binding("FileSCB");
                        b.Source = App.backgroundColorProperty[i + numFilesInFolder + bypassColors];
                        b.Mode = BindingMode.TwoWay;
                        //Attach the binding to the target.
                        legend2.SetBinding(TextBox.BackgroundProperty, b);

                        //add textboxes to grid
                        Grid.SetRow(legend1, legendRow);
                        legendRow++;
                        Grid.SetRow(legend2, legendRow);
                        legendRow++;
                        gridLegend.Children.Add(legend1);
                        gridLegend.Children.Add(legend2);
                    }
                    //keep track of number of files in each folder
                    numFilesInFolder += dataListTemp.Count;
                }

                //add legend to grid
                //windowGrid.Children.Add(gridLegend);
                grid.Children.Add(gridLegend);
                #endregion

                #region zoom and scroll and tooltip popup
                //add click event to zoom in
                //add click event to GridPage and grid
                this.MouseDown += mouseEvent;
                grid.MouseDown += mouseEvent;

                //add size change event to window to stretch grid
                this.SizeChanged += new System.Windows.SizeChangedEventHandler(window_SizeChanged);

                //add event to scroll bar to make all grids scroll
                sViewer.ScrollChanged += new ScrollChangedEventHandler(scrollChanged);
                #endregion
            }

            #region put content together and put on screen
            //add grid to scrollviewer
            //add scrollvieweer to maingrid
            sViewer.Content = grid;
            Grid.SetRow(sViewer, 1);
            Grid.SetColumn(sViewer, 0);
            windowGrid.Children.Add(sViewer);

            //put scrollViewer with grid on screen
            this.Content = windowGrid;
            #endregion
        }

        //read files and draw coordinates to grid
        public void readData()
        {
            #region lists
            //Lists of coordinates already used to able layers when needed
            List<int> xCoord = new List<int>();
            List<int> yCoord = new List<int>();

            //list of how many times each color has been used at each coordinate
            // 0 - count for layers
            // >0 - corresponds to other lists Others[0] = numUsed[1]
            List<List<int>> numUsed = new List<List<int>>();

            //list of files to make sure collisions in same file aren't color changed
            List<List<String>> fileNameList = new List<List<String>>();

            //list of files to make sure collisions in same file aren't color changed
            List<Grid> miniGridList = new List<Grid>();

            //Lists of number of columns in each minigrid
            List<int> numCols = new List<int>();

            //Lists of indexes of colors for each minigrid
            List<List<int>> colorIndex = new List<List<int>>();

            //list of tooltips to add info to
            List<ToolTip> toolTipList = new List<ToolTip>();
            #endregion

            //int to track number of files in previous folder
            int numFilesInFolder = 0;

            //iterate for number of folders
            // 1 - multiple windows
            // >1 - all-in-one window (sometimes also 1)
            for (int w = 0; w < folder.Count; w++)
            {
                //holds file
                List<List<String>> dataListTemp = new List<List<String>>();
                if (multipleWindows)
                    dataListTemp = data;
                else
                    dataListTemp = dataList[w];

                //iterate through files (data lists)
                for (int f = 0; f < dataListTemp.Count; f++)
                {
                    //holds single list
                    List<String> dataTemp = dataListTemp[f];

                    //add used color to array for legend
                    if (App.readColors)
                    {
                        App.backgroundColorProperty.Add(new App.BackgroundColorProperty());
                        App.backgroundColorProperty[f + numFilesInFolder + bypassColors].FileSCB = 
                            new SolidColorBrush((Color)ColorConverter.ConvertFromString(dataTemp[1]));
                    }

                    //Begin reading data from file
                    //0 - folder + file name
                    //1 - color for file
                    for (int i = 2; i < dataTemp.Count; i++)
                    {
                        #region check if coordinates are within graph
                        //separate out data (Ex: 1, 2, 3, 4)
                        char[] delimiterChars1 = { ',' };
                        string[] dataRead = dataTemp[i].Split(delimiterChars1);
                        for (int p = 0; p < dataRead.Length; p++)
                            dataRead[p] = dataRead[p].Trim();

                        //flag used to check if the data is within the graph
                        //SK we need to check if too large or too small for range
                        Boolean inGraph = VerifyIfInGraph(dataRead);

                        #endregion

                        if (inGraph)
                        {
                            #region get column and row
                            //get coordinates
                            //c3 is from counting the number of rows in each part of the 1st column
                            //c4 is from counting the number of columns in each part of the 1st row
                            //keeps numbers to be used in calculating position on graph
                            int num1 = 0;
                            int num2 = 0;
                            int num3 = 0;
                            int num4 = 0;
                            int mult3 = 1;
                            int mult4 = 1;
                            if (factorsIndexList.Count > 0)
                            {
                                if (!(Convert.ToInt32(factors[1]) == 0))
                                    num1 = Convert.ToInt32(dataRead[factorsIndexList[0]]) - Convert.ToInt32(factors[1]);
                                else
                                    num1 = Convert.ToInt32(dataRead[factorsIndexList[0]]);
                            }
                            if (factorsIndexList.Count > 1)
                            {
                                if (!(Convert.ToInt32(factors[4]) == 0))
                                    num2 = Convert.ToInt32(dataRead[factorsIndexList[1]]) - Convert.ToInt32(factors[4]);
                                else
                                    num2 = Convert.ToInt32(dataRead[factorsIndexList[1]]);
                            }
                            if (factorsIndexList.Count > 2)
                            {
                                if (!(Convert.ToInt32(factors[7]) == 0))
                                    num3 = Convert.ToInt32(dataRead[factorsIndexList[2]]) - Convert.ToInt32(factors[7]);
                                else
                                    num3 = Convert.ToInt32(dataRead[factorsIndexList[2]]);
                                mult3 = c3;
                            }
                            if (factorsIndexList.Count > 3)
                            {
                                if (!(Convert.ToInt32(factors[10]) == 0))
                                    num4 = Convert.ToInt32(dataRead[factorsIndexList[3]]) - Convert.ToInt32(factors[10]);
                                else
                                    num4 = Convert.ToInt32(dataRead[factorsIndexList[3]]);
                                mult4 = c4;
                            }

                            int col = num2 * mult4 + num4;
                            int row = num1 * mult3 + num3;
                            #endregion

                            //flag if coordinates already in use
                            Boolean used = false;

                            //check if coordinates already filled
                            for (int g = 0; g < xCoord.Count; g++)
                            {
                                //enter number if coordinates already being used
                                if (xCoord[g] == col && yCoord[g] == row)
                                {
                                    #region setup for collision coordinates
                                    //use grid to separate colors
                                    miniGridList[g].Children.Clear();
                                    miniGridList[g].ColumnDefinitions.Clear();
                                    miniGridList[g].RowDefinitions.Clear();

                                    //empty textbox for tooltip
                                    TextBox textBoxDataTT = new TextBox();
                                    textBoxDataTT.IsReadOnly = true;
                                    textBoxDataTT.Background = new SolidColorBrush(Colors.Transparent);

                                    //box color
                                    //same color if same file
                                    //flag to check if same file
                                    Boolean sameFile = false;
                                    Boolean mainFilePresent = false;
                                    int FN = 0;
                                    while (!sameFile && FN < fileNameList[g].Count)
                                    {
                                        if (!App.multSquares && (fileNameList[g][FN].Equals(App.mainFile)
                                            || dataTemp[0].Equals(App.mainFile)))
                                            mainFilePresent = true;

                                        if (fileNameList[g][FN].Equals(dataTemp[0]))
                                        {
                                            sameFile = true;
                                        }
                                        else
                                            FN++;
                                    }
                                    #endregion

                                    if (!sameFile)
                                    {
                                        if (App.multSquares || !mainFilePresent)
                                        {
                                            #region view colisions as multiple mini-squares
                                            //add file name to list to know which coordinates for which file
                                            fileNameList[g].Add(dataTemp[0]);

                                            //keep track of new point on coordinates
                                            numUsed[g].Add(1);

                                            //add color index to list to remake minigrids
                                            colorIndex[g].Add(f + numFilesInFolder);

                                            if (fileNameList[g].Count % 2 == 0)
                                            {
                                                buildGrid.addRow(miniGridList[g], 2, 0);
                                                buildGrid.addCol(miniGridList[g], fileNameList[g].Count / 2, 0);
                                                numCols[g] = (fileNameList[g].Count / 2);
                                            }
                                            else
                                            {
                                                buildGrid.addCol(miniGridList[g], fileNameList[g].Count, 0);
                                                numCols[g] = (fileNameList[g].Count);
                                            }

                                            //iterate through each color
                                            for (int c = 0; c < fileNameList[g].Count; c++)
                                            {
                                                //use textbox to make square
                                                TextBox textBoxData = new TextBox();
                                                textBoxData.IsReadOnly = true;
                                                textBoxData.TextAlignment = TextAlignment.Center;
                                                textBoxData.Foreground = new SolidColorBrush(Colors.Black);
                                                //keep track of new point on coordinates
                                                textBoxData.Text = numUsed[g][c+1].ToString();

                                                //bind fontsize of textbox to fit
                                                //source uses a FontSizeProperty class
                                                Binding bf = new Binding("SmallFontSizeProp");
                                                bf.Source = App.fontSizeProperty;
                                                bf.Mode = BindingMode.TwoWay;
                                                //Attach the binding to the target.
                                                textBoxData.SetBinding(TextBox.FontSizeProperty, bf);

                                                //bind background of textbox to color in list
                                                //source uses a BackgroundColorProperty class
                                                Binding b = new Binding("FileSCB");
                                                b.Source = App.backgroundColorProperty[colorIndex[g][c] + bypassColors];
                                                b.Mode = BindingMode.TwoWay;
                                                //Attach the binding to the target.
                                                textBoxData.SetBinding(TextBox.BackgroundProperty, b);

                                                if (c > numCols[g] - 1)
                                                {
                                                    Grid.SetColumn(textBoxData, c - numCols[g]);
                                                    Grid.SetRow(textBoxData, 1);
                                                }
                                                else
                                                {
                                                    Grid.SetColumn(textBoxData, c);
                                                    Grid.SetRow(textBoxData, 0);
                                                }

                                                miniGridList[g].Children.Add(textBoxData);
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            #region view colisions with best on top
                                            //add file name to list to know which coordinates for which file
                                            fileNameList[g][0] = App.mainFile;

                                            //use textbox to make square
                                            TextBox textBoxData = new TextBox();
                                            textBoxData.IsReadOnly = true;
                                            textBoxData.TextAlignment = TextAlignment.Center;
                                            textBoxData.Foreground = new SolidColorBrush(Colors.Black);

                                            //keep track of new point on coordinates
                                            if (dataTemp[0].Equals(App.mainFile))
                                                numUsed[g][0]++;
                                            textBoxData.Text = numUsed[g][0].ToString();

                                            //bind fontsize of textbox to fit
                                            //source uses a FontSizeProperty class
                                            Binding bf = new Binding("FontSizeProp");
                                            bf.Source = App.fontSizeProperty;
                                            bf.Mode = BindingMode.TwoWay;
                                            //Attach the binding to the target.
                                            textBoxData.SetBinding(TextBox.FontSizeProperty, bf);

                                            //bind background of textbox to color in list
                                            //source uses a BackgroundColorProperty class
                                            Binding b = new Binding("FileSCB");
                                            b.Source = App.backgroundColorProperty[App.mainColor + bypassColors];
                                            b.Mode = BindingMode.TwoWay;
                                            //Attach the binding to the target.
                                            textBoxData.SetBinding(TextBox.BackgroundProperty, b);

                                            miniGridList[g].Children.Add(textBoxData);
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        //same textfile with multiple sharing a cell
                                        if (fileNameList[g].Count > 1)
                                        {
                                            if (App.multSquares || !mainFilePresent)
                                            {
                                                #region same textfile on used coordinates multiple squares
                                                //keep track of new point on coordinates
                                                numUsed[g][FN+1]++;

                                                if (fileNameList[g].Count % 2 == 0)
                                                {
                                                    buildGrid.addRow(miniGridList[g], 2, 0);
                                                    buildGrid.addCol(miniGridList[g], fileNameList[g].Count / 2, 0);
                                                    numCols[g] = (fileNameList[g].Count / 2);
                                                }
                                                else
                                                {
                                                    buildGrid.addCol(miniGridList[g], fileNameList[g].Count, 0);
                                                    numCols[g] = (fileNameList[g].Count);
                                                }

                                                //iterate through each color
                                                for (int c = 0; c < fileNameList[g].Count; c++)
                                                {
                                                    //use textbox to make square
                                                    TextBox textBoxData = new TextBox();
                                                    textBoxData.IsReadOnly = true;
                                                    textBoxData.TextAlignment = TextAlignment.Center;
                                                    textBoxData.Foreground = new SolidColorBrush(Colors.Black);
                                                    //keep track of new point on coordinates
                                                    textBoxData.Text = numUsed[g][c+1].ToString();

                                                    //bind fontsize of textbox to fit
                                                    //source uses a FontSizeProperty class
                                                    Binding bf = new Binding("SmallFontSizeProp");
                                                    bf.Source = App.fontSizeProperty;
                                                    bf.Mode = BindingMode.TwoWay;
                                                    //Attach the binding to the target.
                                                    textBoxData.SetBinding(TextBox.FontSizeProperty, bf);

                                                    //bind background of textbox to color in list
                                                    //source uses a BackgroundColorProperty class
                                                    Binding b = new Binding("FileSCB");
                                                    b.Source = App.backgroundColorProperty[colorIndex[g][c] + bypassColors];
                                                    b.Mode = BindingMode.TwoWay;
                                                    //Attach the binding to the target.
                                                    textBoxData.SetBinding(TextBox.BackgroundProperty, b);

                                                    if (c > numCols[g] - 1)
                                                    {
                                                        Grid.SetColumn(textBoxData, c - numCols[g]);
                                                        Grid.SetRow(textBoxData, 1);
                                                    }
                                                    else
                                                    {
                                                        Grid.SetColumn(textBoxData, c);
                                                        Grid.SetRow(textBoxData, 0);
                                                    }

                                                    miniGridList[g].Children.Add(textBoxData);
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                #region view colisions with best on top
                                                //use textbox to make square
                                                TextBox textBoxData = new TextBox();
                                                textBoxData.IsReadOnly = true;
                                                textBoxData.TextAlignment = TextAlignment.Center;
                                                textBoxData.Foreground = new SolidColorBrush(Colors.Black);

                                                //keep track of new point on coordinates
                                                if (dataTemp[0].Equals(App.mainFile))
                                                    numUsed[g][0]++;
                                                textBoxData.Text = numUsed[g][0].ToString();

                                                //bind fontsize of textbox to fit
                                                //source uses a FontSizeProperty class
                                                Binding bf = new Binding("FontSizeProp");
                                                bf.Source = App.fontSizeProperty;
                                                bf.Mode = BindingMode.TwoWay;
                                                //Attach the binding to the target.
                                                textBoxData.SetBinding(TextBox.FontSizeProperty, bf);

                                                //bind background of textbox to color in list
                                                //source uses a BackgroundColorProperty class
                                                Binding b = new Binding("FileSCB");
                                                b.Source = App.backgroundColorProperty[App.mainColor + bypassColors];
                                                b.Mode = BindingMode.TwoWay;
                                                //Attach the binding to the target.
                                                textBoxData.SetBinding(TextBox.BackgroundProperty, b);

                                                miniGridList[g].Children.Add(textBoxData);
                                                #endregion
                                            }
                                        }
                                        else //same text file with only one text file in cell
                                        {
                                            #region same textfile on used coordinates
                                            //use textbox to make square
                                            TextBox textBoxData = new TextBox();
                                            textBoxData.IsReadOnly = true;
                                            textBoxData.TextAlignment = TextAlignment.Center;
                                            textBoxData.Foreground = new SolidColorBrush(Colors.Black);

                                            //keep track of new point on coordinates
                                            if (!App.multSquares && mainFilePresent)
                                            {
                                                if (dataTemp[0].Equals(App.mainFile))
                                                    numUsed[g][0]++;
                                                textBoxData.Text = numUsed[g][0].ToString();
                                            }
                                            else
                                            {
                                                numUsed[g][FN + 1]++;
                                                textBoxData.Text = numUsed[g][FN + 1].ToString();
                                            }

                                            //bind fontsize of textbox to fit
                                            //source uses a FontSizeProperty class
                                            Binding bf = new Binding("FontSizeProp");
                                            bf.Source = App.fontSizeProperty;
                                            bf.Mode = BindingMode.TwoWay;
                                            //Attach the binding to the target.
                                            textBoxData.SetBinding(TextBox.FontSizeProperty, bf);

                                            //bind background of textbox to color in list
                                            //source uses a BackgroundColorProperty class
                                            Binding b = new Binding("FileSCB");
                                            if (!App.multSquares && mainFilePresent)
                                                b.Source = App.backgroundColorProperty[App.mainColor + bypassColors];
                                            else
                                                b.Source = App.backgroundColorProperty[f + numFilesInFolder + bypassColors];
                                            b.Mode = BindingMode.TwoWay;
                                            //Attach the binding to the target.
                                            textBoxData.SetBinding(TextBox.BackgroundProperty, b);

                                            Grid.SetColumn(textBoxData, 0);
                                            Grid.SetRow(textBoxData, 0);

                                            miniGridList[g].Children.Add(textBoxData);
                                            #endregion
                                        }
                                    }

                                    #region used coordinate tooltip
                                    //add tool tip to show folder name and coordinates when hoover over
                                    //check for which coordinates to display
                                    //add new info to old
                                    if (factorsIndexList.Count > 3)
                                    {
                                        toolTipList[g].Content += "\n" + folder[w] + " - " +
                                            dataRead[factorsIndexList[0]] + ", " +
                                            dataRead[factorsIndexList[1]] + ", " +
                                            dataRead[factorsIndexList[2]] + ", " +
                                            dataRead[factorsIndexList[3]];
                                    }
                                    else if (factorsIndexList.Count > 2)
                                    {
                                        toolTipList[g].Content += "\n" + folder[w] + " - " +
                                            dataRead[factorsIndexList[0]] + ", " +
                                            dataRead[factorsIndexList[1]] + ", " +
                                            dataRead[factorsIndexList[2]];
                                    }
                                    else if (factorsIndexList.Count > 1)
                                    {
                                        toolTipList[g].Content += "\n" + folder[w] + " - " +
                                            dataRead[factorsIndexList[0]] + ", " +
                                            dataRead[factorsIndexList[1]];
                                    }
                                    else if (factorsIndexList.Count > 0)
                                    {
                                        toolTipList[g].Content += "\n" + folder[w] + " - " +
                                            dataRead[factorsIndexList[0]];
                                    }
                                    textBoxDataTT.ToolTip = toolTipList[g];

                                    //event handler to allow zoom capabilities
                                    textBoxDataTT.PreviewMouseDown += mouseEvent;

                                    //add to specific coordinates
                                    Grid.SetColumn(textBoxDataTT, col);
                                    Grid.SetRow(textBoxDataTT, row);

                                    //add to grid
                                    gridData.Children.Add(textBoxDataTT);
                                    used = true;
                                    #endregion
                                }
                            }

                            #region new coordinates
                            //if coordinates not used fill in whole box
                            if (!used)
                            {
                                //minigrid in each square
                                Grid miniGrid = new Grid();

                                //use textbox to make square
                                TextBox textBoxData = new TextBox();
                                textBoxData.IsReadOnly = true;

                                //add tool tip to show folder name and coordinates when hoover over
                                ToolTip tt = new ToolTip();
                                //check for which coordinates to display
                                if (factorsIndexList.Count > 3)
                                {
                                    tt.Content = "Folder - " + factors[0] + ", " + factors[3] +
                                        ", " + factors[6] + ", " + factors[9] + "\n";
                                    tt.Content += folder[w] + " - " + dataRead[factorsIndexList[0]] + ", " +
                                        dataRead[factorsIndexList[1]] + ", " +
                                        dataRead[factorsIndexList[2]] + ", " +
                                        dataRead[factorsIndexList[3]];
                                }
                                else if (factorsIndexList.Count > 2)
                                {
                                    tt.Content = "Folder - " + factors[0] + ", " + factors[3] +
                                        ", " + factors[6] + "\n";
                                    tt.Content += folder[w] + " - " + dataRead[factorsIndexList[0]] + ", " +
                                        dataRead[factorsIndexList[1]] + ", " +
                                        dataRead[factorsIndexList[2]];
                                }
                                else if (factorsIndexList.Count > 1)
                                {
                                    tt.Content = "Folder - " + factors[0] + ", " + factors[3] + "\n";
                                    tt.Content += folder[w] + " - " + dataRead[factorsIndexList[0]] + ", " +
                                        dataRead[factorsIndexList[1]];
                                }
                                else if (factorsIndexList.Count > 0)
                                {
                                    tt.Content = "Folder - " + factors[0] + "\n";
                                    tt.Content += folder[w] + " - " + dataRead[factorsIndexList[0]];
                                }
                                textBoxData.ToolTip = tt;
                                toolTipList.Add(tt);

                                //box color
                                //bind background of textbox to color in list
                                //source uses a BackgroundColorProperty class
                                Binding b = new Binding("FileSCB");
                                b.Source = App.backgroundColorProperty[f + numFilesInFolder + bypassColors];
                                b.Mode = BindingMode.TwoWay;
                                //Attach the binding to the target.
                                textBoxData.SetBinding(TextBox.BackgroundProperty, b);

                                //event handler to allow zoom capabilities
                                textBoxData.PreviewMouseDown += mouseEvent;

                                //add to specific coordinates
                                Grid.SetColumn(textBoxData, col);
                                Grid.SetRow(textBoxData, row);
                                Grid.SetColumn(miniGrid, col);
                                Grid.SetRow(miniGrid, row);

                                //add to grid
                                gridData.Children.Add(textBoxData);
                                gridData.Children.Add(miniGrid);

                                //add used coordinates and color to list
                                xCoord.Add(col);
                                yCoord.Add(row);
                                numUsed.Add(new List<int>());
                                numUsed[numUsed.Count - 1].Add(0);
                                numUsed[numUsed.Count - 1].Add(1);
                                if (!App.multSquares && dataTemp[0].Equals(App.mainFile))
                                    numUsed[numUsed.Count - 1][0]++;
                                //add file name to list to know which coordinates for which file
                                fileNameList.Add(new List<String>());
                                fileNameList[fileNameList.Count - 1].Add(dataTemp[0]);
                                //add minigrid to list
                                miniGridList.Add(miniGrid);
                                //make 1 numCol variable for each minigrid
                                numCols.Add(1);
                                //add color index to list to remake minigrids
                                colorIndex.Add(new List<int>());
                                colorIndex[colorIndex.Count - 1].Add(f + numFilesInFolder);
                            }
                            #endregion
                        }
                    }
                }

                //int to track number of files in previous folder
                numFilesInFolder += dataListTemp.Count;
            }

            if (App.readColors)
                App.readColors = false;
        }

        private bool VerifyIfInGraph(string[] dataRead)
        {
            bool inGraph = true;

            int dataRead0 = Convert.ToInt32(dataRead[factorsIndexList[0]]);
            int dataRead1 = Convert.ToInt32(dataRead[factorsIndexList[1]]);
            int dataRead2 = Convert.ToInt32(dataRead[factorsIndexList[2]]);
            int dataRead3 = Convert.ToInt32(dataRead[factorsIndexList[3]]);

            int factorMin0 = Convert.ToInt32(factors[1]); int factorMax0 = Convert.ToInt32(factors[2]);
            int factorMin1 = Convert.ToInt32(factors[4]); int factorMax1 = Convert.ToInt32(factors[5]);
            int factorMin2 = Convert.ToInt32(factors[7]); int factorMax2 = Convert.ToInt32(factors[8]);
            int factorMin3 = Convert.ToInt32(factors[10]); int factorMax3 = Convert.ToInt32(factors[11]);

            switch (factorsIndexList.Count-1)
            {
                case 0:
                    {
                        if (dataRead0 > factorMax0 || dataRead0 < factorMin0) inGraph = false;
                    }
                    break;
                case 1:
                    {
                        if (dataRead0 > factorMax0 || dataRead0 < factorMin0) inGraph = false;
                        if (dataRead1 > factorMax1 || dataRead1 < factorMin1) inGraph = false;
                    }
                    break;
                case 2:
                    {
                        if (dataRead0 > factorMax0 || dataRead0 < factorMin0) inGraph = false;
                        if (dataRead1 > factorMax1 || dataRead1 < factorMin1) inGraph = false;
                        if (dataRead2 > factorMax2 || dataRead2 < factorMin2) inGraph = false;
                    }
                    break;
                case 3:
                    {
                        if (dataRead0 > factorMax0 || dataRead0 < factorMin0) inGraph = false;
                        if (dataRead1 > factorMax1 || dataRead1 < factorMin1) inGraph = false;
                        if (dataRead2 > factorMax2 || dataRead2 < factorMin2) inGraph = false;
                        if (dataRead3 > factorMax3 || dataRead3 < factorMin3) inGraph = false;
                    }
                    break;
                default:
                    {
                        inGraph = false;
                    }
                    break;
            }

            //if (factorsIndexList.Count > 0 && factorMax1 - dataRead1  < 0 && factorMin1 - dataRead1 > 0)
            //    inGraph = false;
            
            
            //if (factorsIndexList.Count > 1 && factorMax2- dataRead2 < 0 && factorMin2 - dataRead2 > 0)
            //if (factorsIndexList.Count > 1 && ((Convert.ToInt32(dataRead[factorsIndexList[1]]) - Convert.ToInt32(factors[5])) > 0) && (Convert.ToInt32(dataRead[factorsIndexList[0]]) - Convert.ToInt32(factors[1]) >= 0))
            //    inGraph = false;
            
            
            //if (factorsIndexList.Count > 2 && ((Convert.ToInt32(dataRead[factorsIndexList[2]]) - Convert.ToInt32(factors[8])) > 0))
            //    inGraph = false;
            //if (factorsIndexList.Count > 3 && ((Convert.ToInt32(dataRead[factorsIndexList[3]]) - Convert.ToInt32(factors[11])) > 0))
            //    inGraph = false;

            return inGraph;
        }

        //catch whena click happens
        private void mouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                //zoom to specific spot
                // Get the x and y coordinates of the mouse pointer.
                System.Windows.Point position = e.GetPosition(App.gridWindowList[0]);

                //change scroll offset
                App.gridWindowList[0].sViewer.ScrollToHorizontalOffset(0);
                App.gridWindowList[0].sViewer.ScrollToVerticalOffset(0);
                //App.gridWindowList[0].sViewer.ScrollToHorizontalOffset(position.X * App.scale + 80);
                //App.gridWindowList[0].sViewer.ScrollToVerticalOffset(position.Y * App.scale + 80);

                zoomIn();
                e.Handled = true;
            }
            else
            {
                zoomOut();
                e.Handled = true;
            }
        }

        //zoom in on right click
        private void zoomIn()
        {
            //itterate through all grids to make all zoom equally
            for (int i = 0; i < App.gridWindowList.Count; i++)
            {
                //zoom in by resizing grids
                App.gridWindowList[i].MinHeight = App.gridWindowList[i].ActualHeight / (2 * App.scale);
                App.gridWindowList[i].MinWidth = App.gridWindowList[i].ActualWidth / (2 * App.scale);
                App.gridWindowList[i].grid.MinHeight = App.gridWindowList[i].grid.ActualHeight * 2;
                App.gridWindowList[i].grid.MinWidth = App.gridWindowList[i].grid.ActualWidth * 2;
            }
            App.fontSizeProperty.FontSizeProp *= 2;
            App.fontSizeProperty.SmallFontSizeProp *= 2;
            App.scale++;
        }

        //zoom out on left click unless completely zoomed-out already
        private void zoomOut()
        {
            //itterate through all grids to make all zoom equally
            for (int i = 0; i < App.gridWindowList.Count; i++)
            {
                //zoom out by resizing grid
                if (App.scale > 1)
                {
                    if (App.scale == 2)
                    {
                        App.gridWindowList[i].grid.MinHeight = 0;
                        App.gridWindowList[i].grid.MinWidth = 0;
                    }
                    else
                    {
                        App.gridWindowList[i].grid.MinHeight = App.gridWindowList[i].grid.ActualHeight / 2;
                        App.gridWindowList[i].grid.MinWidth = App.gridWindowList[i].grid.ActualWidth / 2;
                    }
                }
            }
            
            if (App.scale > 1)
            {
                App.fontSizeProperty.FontSizeProp /= 2;
                App.fontSizeProperty.SmallFontSizeProp /= 2;
                App.scale--;
            }
        }

        //change windows so they all scroll the same when multiple are up
        private void scrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //itterate through all grids to make all scroll at the same time
            for (int i = 0; i < App.gridWindowList.Count; i++)
            {
                ScrollViewer sTemp = (ScrollViewer)sender;
                if (sTemp != App.gridWindowList[i].sViewer)
                {
                    if (!(e.HorizontalOffset == App.gridWindowList[i].sViewer.HorizontalOffset))
                        App.gridWindowList[i].sViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
                    if (!(e.VerticalOffset == App.gridWindowList[i].sViewer.VerticalOffset))
                        App.gridWindowList[i].sViewer.ScrollToVerticalOffset(e.VerticalOffset);
                }
            }
            e.Handled = true;
        }

        //save a .png of the grid and legend
        void saveButton_Click(Object sender, RoutedEventArgs e)
        {
            //create SaveFileDialog
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            //Configure save file dialog box
            // Default file name
            dlg.FileName = "Document"; 
            // Default file extension
            dlg.DefaultExt = ".jpg"; 
            // Filter files by extension
            dlg.Filter = "(.jpg)|*.jpg";
            
            
            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results 
            if (result == true)
            {
                //gets what size the saved image should be
                SaveSizeWindow saveSizeWindow = new SaveSizeWindow();
                saveSizeWindow.ShowDialog();

                //create saving pop up window running on separate thread
                PopUpWorker saveWorkerObject = new PopUpWorker();
                Thread saveWorkerThread = new Thread(saveWorkerObject.DoSaveWork);
                saveWorkerObject.RequestStart();

                saveWorkerThread.SetApartmentState(ApartmentState.STA);
                saveWorkerThread.Start();
                while (!saveWorkerThread.IsAlive) ;
                Thread.Sleep(1);

                RenderTargetBitmap targetBitmap;

                //if user wants large picture
                if (App.saveSizeIndex == 1)
                {
                    //zoom in to get visible numbers on saved png
                    zoomIn();
                    this.Height += 800;
                    this.Width += 1040;
                    zoomIn();
                    this.Height += 800;
                    this.Width += 1040;

                    //Get the selected file name and save bitmap there
                    // render this grid's visual tree to the RenderTargetBitmap
                    targetBitmap = new RenderTargetBitmap((int)(this.grid.ActualWidth + 180) * 4,
                            (int)(this.grid.ActualHeight + 150) * 4, 400d, 400d, PixelFormats.Pbgra32);
                }
                else
                {

                    //Get the selected file name and save bitmap there
                    // render this grid's visual tree to the RenderTargetBitmap
                    targetBitmap = new RenderTargetBitmap((int)(this.grid.ActualWidth + 50) * 4,
                            (int)(this.grid.ActualHeight + 50) * 4, 400d, 400d, PixelFormats.Pbgra32);
                }

                targetBitmap.Render(this.grid);

                // add the RenderTargetBitmap to a Bitmapencoder
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(targetBitmap));

                try
                {
                    // save file to disk
                    using (FileStream fs = File.Open(dlg.FileName, FileMode.OpenOrCreate))
                    {
                        encoder.Save(fs);
                    }
                }
                catch (Exception ex)
                {
                }

                //if user chose large picture
                if (App.saveSizeIndex == 1)
                {
                    //set height and width to original
                    zoomOut();
                    this.Height = 890;
                    this.Width = 1070;
                    zoomOut();
                }

                //stop save pop up window thread
                saveWorkerObject.RequestStop();
            }
        }

        //change a file's color
        void changeColorButton_Click(Object sender, RoutedEventArgs e)
        {
            //instance of ChangeColorWindow
            ChangeColorWindow changeColorWindow = new ChangeColorWindow(fileList, bypassColors);
            //open changeColorWindow
            changeColorWindow.ShowDialog();
        }

        //change whether a certain color is on top or multiple colors share a cell during a collision
        void changeViewButton_Click(Object sender, RoutedEventArgs e)
        {
            //instance of ChangeViewWindow
            ChangeViewWindow changeViewWindow = new ChangeViewWindow(this, fileList);
            //open changeViewWindow
            changeViewWindow.ShowDialog();
        }

        private void window_SizeChanged(object sender, System.EventArgs e)
        {
            grid.Height = this.ActualHeight - 90;
            grid.Width = this.ActualWidth - 30;
        }

        //clear objects and handlers to stop memory leaks
        protected void onExit(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            //remove handlers to let objects die
            sViewer.ScrollChanged -= new ScrollChangedEventHandler(scrollChanged);
            save.Click -= new RoutedEventHandler(saveButton_Click);
            changeColor.Click -= new RoutedEventHandler(changeColorButton_Click);
            changeView.Click -= new RoutedEventHandler(changeViewButton_Click);
            ResetSubscriptions();
        }

        private void ResetSubscriptions() { mouseEvent = null; }
    }
}
