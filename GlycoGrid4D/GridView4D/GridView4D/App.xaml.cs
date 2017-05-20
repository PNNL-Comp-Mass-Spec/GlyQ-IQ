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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;

namespace GridView4D
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //list of folder references and their content
        public static List<List<List<String>>> data;
        //list of folder names containing lists of files in each folder
        public static List<List<String>> folderList;
        //list of the factors, min, max
        public static List<String> factorsList;
        //list to hold all of grids to coordinate zoom and scroll
        public static List<GridWindow> gridWindowList;
        //scale to track zoom
        public static int scale = 1;
        //folder path
        public static String recentFolderPath = "";
        //changes between multiple squares view and layers view
        public static Boolean multSquares;
        //makes colors be read from files only once
        public static Boolean readColors;
        //index of color to put on top
        public static int mainColor;
        //index of color to put on top
        public static String mainFile;
        //instance of BackgroundColorProperty class
        public static List<BackgroundColorProperty> backgroundColorProperty;
        //instance of FontSizeProperty class
        public static FontSizeProperty fontSizeProperty;
        //keep track of original textsize
        public static int originalFontSize = 1;
        //keeps index of size the saved image should be
        public static int saveSizeIndex = 0;

        //nested class to bind color of textboxes to change automatically
        public class BackgroundColorProperty : INotifyPropertyChanged
        {
            //list of which colors have been added to each coordinate set
            private SolidColorBrush fileSCB = new SolidColorBrush();

            /// <summary>
            /// updates in real-time
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            protected void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

            public SolidColorBrush FileSCB
            {
                get { return fileSCB; }
                set { fileSCB = (SolidColorBrush)value; NotifyPropertyChanged("FileSCB"); }
            }
        }

        //nested class to bind font size of textboxes to change automatically
        public class FontSizeProperty : INotifyPropertyChanged
        {
            //list of which colors have been added to each coordinate set
            private int fontSizeProp = 1;
            private int smallFontSizeProp = 1;

            /// <summary>
            /// updates in real-time
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            protected void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

            public int FontSizeProp
            {
                get { return fontSizeProp; }
                set { fontSizeProp = (int)value; NotifyPropertyChanged("FontSizeProp"); }
            }

            public int SmallFontSizeProp
            {
                get { return smallFontSizeProp; }
                set { smallFontSizeProp = (int)value; NotifyPropertyChanged("SmallFontSizeProp"); }
            }
        }

        public App()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(this.OnApplicationExit); 
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            try
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\appdata.txt");

                // Create a file to write to. 
                using (System.IO.StreamWriter sw = System.IO.File.CreateText(
                    AppDomain.CurrentDomain.BaseDirectory + "\\appdata.txt"))
                {
                    sw.WriteLine(recentFolderPath);
                }

            }
            catch (IOException e1)
            {
                // Inform the user that an error occurred.
                MessageBox.Show("Error:" + e1.ToString());
            }

        }
    }
}
