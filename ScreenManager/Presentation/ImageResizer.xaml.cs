using Microsoft.Win32;
using ScreenManager.DataAccess;
using ScreenManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using ScreenManager.Business;
using ScreenManager.Helper;
using WpfAnimatedGif;
using ScreenManager.Presentation.VM;

namespace ScreenManager.Presentation
{
    /// <summary>
    /// Interaction logic for ImageResizer.xaml
    /// </summary>
    public partial class ImageResizer : Window
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public string processingFile { get; set; }
        public static MainViewModel MainViewModel { get; set; } = new MainViewModel(Brushes.Black);
        public ImageResizer()
        {
            InitializeComponent();

            this.DataContext = MainViewModel;
        }

        private void btnSource_Click(object sender, RoutedEventArgs e)
        {
            var source = new OpenFolderDialog();
            //var source = new OpenFileDialog();
            var result = source.ShowDialog();
            if (result == true)
            {
                MainViewModel.ForegroundSource = Brushes.Black;
                MainViewModel.Source = source.FolderName;
                // MainViewModel.ProcessingFile = source.FolderName;
                Source = source.FolderName;
            }
        }

        private void btnDestination_Click(object sender, RoutedEventArgs e)
        {
            var dest = new OpenFolderDialog();
            var result = dest.ShowDialog();
            if (result == true)
            {
                MainViewModel.ForegroundDest = Brushes.Black;
                MainViewModel.Destination = dest.FolderName;
                Destination = dest.FolderName;
            }
        }

        private async void btnResize_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MainViewModel.Source) || MainViewModel.Source == "Please select source folder*")
            {
                MainViewModel.Source = "Please select source folder*";
                MainViewModel.ForegroundSource = Brushes.Red;
                return;
            }
            else if(string.IsNullOrWhiteSpace(MainViewModel.Destination) || MainViewModel.Destination == "Please select destination folder*")
            {
                MainViewModel.Destination = "Please select destination folder*";
                MainViewModel.ForegroundDest = Brushes.Red;
                return;
            }

            btnSource.IsEnabled = false;
            btnDestination.IsEnabled = false;
            btnResize.IsEnabled = false;

            var xmlReader = new XmlReaderService();

            var xmlReaderService = new XmlReaderService();
            var imgResizerService = new ImageResizerService();
            var imgProcessingService = new ImageProcessingService(xmlReaderService, imgResizerService);

            await imgProcessingService.ProcessImages(Source, Destination);
            // loader.Close();
            Notification.Notify(this, "Resized successfully", NotificationType.Success);

            btnSource.IsEnabled = true;
            btnDestination.IsEnabled = true;
            btnResize.IsEnabled = true;
        }

        private void bdCross_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
