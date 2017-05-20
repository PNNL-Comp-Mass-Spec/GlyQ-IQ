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
    /// Interaction logic for ChangeViewWindow.xaml
    /// </summary>
    public partial class ChangeViewWindow : Window
    {
        GridWindow gridWindow;
        List<String> fileList;

        public ChangeViewWindow(GridWindow gridWindowp, List<String> fileListp)
        {
            InitializeComponent();

            fileList = fileListp;

            gridWindow = gridWindowp;

            topFileListBox.Items.Clear();

            //add listBoxTopFiles to 2nd column to keep individual seleections
            listBoxTopFiles.Foreground = new SolidColorBrush(Colors.Transparent);
            listBoxTopFiles.Background = new SolidColorBrush(Colors.Transparent);
            SolidColorBrush scb = new SolidColorBrush(Colors.Blue);
            scb.Opacity = 0.25;
            listBoxTopFiles.Resources.Add(SystemColors.HighlightBrushKey, scb);
            listBoxTopFiles.Resources.Add(SystemColors.HighlightTextBrushKey, new SolidColorBrush(Colors.Transparent));
            listBoxTopFiles.Resources.Add(SystemColors.ControlBrushKey, scb);
            listBoxTopFiles.Resources.Add(SystemColors.ControlTextBrushKey, new SolidColorBrush(Colors.Transparent));

            //add file names to listbox
            int i = 0;
            while (i < fileList.Count)
            {
                topFileListBox.Items.Add(fileList[i]);
                listBoxTopFiles.Items.Add(i);
                i++;
            }
            listBoxTopFiles.Items.Add(i);

            //add change grid back to multiple square view option
            topFileListBox.Items.Add("Multi-color collisions");
        }

        private void doneSelectingButton_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxTopFiles.SelectedIndex == -1)
                MessageBox.Show("No option selected");
            else
            {
                if (listBoxTopFiles.Items.Count - 1 == listBoxTopFiles.Items.IndexOf(listBoxTopFiles.SelectedItem))
                    App.multSquares = true;
                else
                {
                    App.mainColor = listBoxTopFiles.Items.IndexOf(listBoxTopFiles.SelectedItem);
                    App.mainFile = fileList[listBoxTopFiles.Items.IndexOf(listBoxTopFiles.SelectedItem)];
                    App.multSquares = false;
                }

                gridWindow.readData();

                doneSelectingButton.Click -= doneSelectingButton_Click;
                this.Close();
            }
        }
    }
}
