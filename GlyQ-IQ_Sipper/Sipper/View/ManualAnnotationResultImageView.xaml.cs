using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DeconTools.Workflows.Backend;
using Sipper.Model;
using Sipper.ViewModel;

namespace Sipper.View
{
    /// <summary>
    /// Interaction logic for ManualAnnotationResultImageView.xaml
    /// </summary>
    public partial class ManualAnnotationResultImageView : Window
    {
        public ManualAnnotationResultImageView(Project project = null)
        {
            InitializeComponent();

            if (project == null)
            {
                project = new Project();
            }

            ViewModel = new ManualViewingWithoutRawDataViewModel(project.ResultRepository, project.FileInputs);

            DataContext = ViewModel;




        }


        public ManualViewingWithoutRawDataViewModel ViewModel { get; set; }


        private void FileDropHandler(object sender, DragEventArgs e)
        {
            DataObject dataObject = e.Data as DataObject;

            if (dataObject.ContainsFileDropList())
            {


                var fileNamesStringCollection = dataObject.GetFileDropList();
                StringBuilder bd = new StringBuilder();


                var fileNames = fileNamesStringCollection.Cast<string>().ToList();

                ViewModel.FileInputs.CreateFileLinkages(fileNames);


            }
        }


        private void txtResultsFilePath_DragOver(object sender, DragEventArgs e)
        {
            bool dropEnabled = true;

            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];

                foreach (string filename in filenames)
                {
                    if (System.IO.Path.GetExtension(filename).ToUpperInvariant() != ".TXT")
                    {
                        dropEnabled = false;
                        break;
                    }
                }
            }
            else
            {
                dropEnabled = false;
            }


            if (!dropEnabled)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }


        private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;


            var firstAddedItem = e.AddedItems[0];

            if (firstAddedItem is ResultWithImageInfo)
            {
                ViewModel.CurrentResult = (ResultWithImageInfo)e.AddedItems[0];
            }

        }

        private void btnCreateImagesClick(object sender, RoutedEventArgs e)
        {
            ViewModel.CreateImages();
        }



        private void btnSaveResultsClick(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveResults();
        }

        private void ValidationCodeListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

       

        private void btnOpenHtmlReport_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenHTMLReport();
        }

        private void btnGenerateHtmlReport_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GenerateHTMLReport();
        }

        private void btnUpdateAnnotationsWithAutomaticFilters_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveResults();
            ViewModel.UpdateAnnotationsUsingAutomaticFilter();
        }

        private void ExecuteSetAnnotationToYesCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (ViewModel.CurrentResult!=null)
            {
                ViewModel.CurrentResult.Result.ValidationCode = ValidationCode.Yes;
                
                   
                listViewMain.SelectedItem = ViewModel.CurrentResult;

                listViewMain.Items.Refresh();

                



            }
            

        }

        public void CanExecuteCustomCommand(object sender, 
            CanExecuteRoutedEventArgs e) 
        { 
            e.CanExecute = true; 
        }

     
      
    }
}
