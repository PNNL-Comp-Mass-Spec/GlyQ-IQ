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
    /// Interaction logic for ChangeColorWindow.xaml
    /// </summary>
    public partial class ChangeColorWindow : Window
    {
        List<String> c;
        int bypassColors;

        public ChangeColorWindow(List<String> fileList, int bypassColorsp)
        {
            InitializeComponent();

            bypassColors = bypassColorsp;

            addItems(fileList);
        }

        private void addItems(List<String> fileList)
        {
            fileChangeListBox.Items.Clear();
            colorChangeListBox.Items.Clear();
            listBoxTopFiles.Items.Clear();

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
            for (int i = 0; i < fileList.Count; i++)
            {
                fileChangeListBox.Items.Add(fileList[i]);
                listBoxTopFiles.Items.Add(i);
            }

            c = new List<String>();
            c.Add("Blue");
            c.Add("Gold");
            c.Add("Green");
            c.Add("Magenta");
            c.Add("Orange");
            c.Add("Pink");
            c.Add("Red");
            c.Add("Yellow");

            for (int i = 0; i < c.Count; i++)
                colorChangeListBox.Items.Add(c[i]);
        }

        private void doneChangingButton_Click(object sender, RoutedEventArgs e)
        {
            Button from = (Button)sender;

            switch (from.Name)
            {
                case "doneChangingButton":
                    change();
                    break;
                case "cancelChangingButton":
                    doneChangingButton.Click -= doneChangingButton_Click;
                    cancelChangingButton.Click -= doneChangingButton_Click;
                    this.Close();
                    break;
            }
        }

        private void change()
        {
            if (listBoxTopFiles.SelectedIndex == -1)
                MessageBox.Show("No file selected");
            else if (colorChangeListBox.SelectedIndex == -1)
                MessageBox.Show("No color selected");
            else
            {
                //change colors for files
                foreach (Object selectedItem in listBoxTopFiles.SelectedItems)
                {
                    App.backgroundColorProperty[listBoxTopFiles.Items.IndexOf(selectedItem) + bypassColors].FileSCB 
                        = new SolidColorBrush((Color)ColorConverter.ConvertFromString(
                        c[colorChangeListBox.Items.IndexOf(colorChangeListBox.SelectedItem)]));
                }

                doneChangingButton.Click -= doneChangingButton_Click;
                cancelChangingButton.Click -= doneChangingButton_Click;
                this.Close();
            }
        }
    }
}
