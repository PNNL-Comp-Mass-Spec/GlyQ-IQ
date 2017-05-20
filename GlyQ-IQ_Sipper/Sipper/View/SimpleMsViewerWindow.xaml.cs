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
using DeconTools.Backend;
using DeconTools.Backend.Core;
using Sipper.Model;
using Sipper.ViewModel;

namespace Sipper.View
{
    /// <summary>
    /// Interaction logic for SimpleMsViewerWindow.xaml
    /// </summary>
    public partial class SimpleMsViewerWindow : Window
    {
        public SimpleMsViewerWindow()
            : this(null)
        {
            
        }

        public SimpleMsViewerWindow(Project sipperProject)
        {
            InitializeComponent();

            if (sipperProject==null)sipperProject=new Project();

            ViewModel = new SimpleMsViewerViewModel(sipperProject.Run);
            DataContext = ViewModel;

        }


        public SimpleMsViewerViewModel ViewModel { get; set; }


        private void btnNavigateUpClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null || ViewModel.Run == null) return;

            ViewModel.NavigateToNextMS1MassSpectrum(Globals.ScanSelectionMode.ASCENDING);

        }


        private void btnNavigateDownClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null || ViewModel.Run == null) return;

            ViewModel.NavigateToNextMS1MassSpectrum(Globals.ScanSelectionMode.DESCENDING);
        }

        private void NavigateToSpecificScanEvent(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null || ViewModel.Run == null) return;

            int currentScan;
            if (Int32.TryParse(txtCurrentScan.Text, out currentScan))
            {
                if (currentScan == ViewModel.CurrentLcScan) return;

                ViewModel.CurrentLcScan = currentScan;
            }
            else
            {
                return;
            }

            ViewModel.NavigateToNextMS1MassSpectrum(Globals.ScanSelectionMode.CLOSEST);

        }


        private void TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ViewModel == null || ViewModel.Run == null) return;

            if (e.Delta > 0)
            {
                ViewModel.NavigateToNextMS1MassSpectrum();
            }
            else
            {
                ViewModel.NavigateToNextMS1MassSpectrum(Globals.ScanSelectionMode.DESCENDING);
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ViewModel == null || ViewModel.Run == null) return;

            

            ViewModel.CurrentLcScan = (int)e.NewValue;
            ViewModel.NavigateToNextMS1MassSpectrum(Globals.ScanSelectionMode.CLOSEST);

        }

        private void MsGraphMinMouseWheelEvent(object sender, MouseWheelEventArgs e)
        {
            if (ViewModel == null || ViewModel.Run == null) return;

            if (e.Delta > 0)
            {
                ViewModel.MSGraphMinX = ViewModel.MSGraphMinX + 1;
            }
            else
            {
                ViewModel.MSGraphMinX = ViewModel.MSGraphMinX - 1;
            }


        }


        private void MsGraphMaxMouseWheelEvent(object sender, MouseWheelEventArgs e)
        {
            if (ViewModel == null || ViewModel.Run == null) return;

            if (e.Delta > 0)
            {
                ViewModel.MSGraphMaxX = ViewModel.MSGraphMaxX + 1;
            }
            else
            {
                ViewModel.MSGraphMaxX = ViewModel.MSGraphMaxX - 1;
            }

        }


        private void btnOpenDataset_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "RAW Files (*.raw)|*.raw|UIMF (*.uimf)|*.uimf|All Files (*.*)|*.*";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                ViewModel.LoadRun(filename);

                slider.Minimum = ViewModel.MinLcScan;
                slider.Maximum = ViewModel.MaxLcScan;
            }

        }

        private void txtNumMsSummed_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ViewModel == null || ViewModel.Run == null) return;

            if (e.Delta > 0)
            {
                ViewModel.NumMSScansToSum = ViewModel.NumMSScansToSum + 2;
            }
            else
            {
                ViewModel.NumMSScansToSum = ViewModel.NumMSScansToSum - 2;
            }


        }

        private void msPeaksDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null || ViewModel.Run == null) return;

            if (e.AddedItems.Count > 0)
            {
                ViewModel.SelectedPeak = (Peak) e.AddedItems[0];
            }
           
            
        }

        private void btnReCreatePeaksFile_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null || ViewModel.Run == null) return;

           
            ViewModel.LoadPeaksUsingBackgroundWorker(true);
            
        }
        



    }
}
