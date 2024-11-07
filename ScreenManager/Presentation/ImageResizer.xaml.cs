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

namespace ScreenManager.Presentation
{
    /// <summary>
    /// Interaction logic for ImageResizer.xaml
    /// </summary>
    public partial class ImageResizer : Window
    {
        public string SourcePath { get; set; }
        public ImageResizer()
        {
            InitializeComponent();
        }

        private void btnSource_Click(object sender, RoutedEventArgs e)
        {
            var source = new OpenFolderDialog();
            //var source = new OpenFileDialog();
            var result = source.ShowDialog();
            if (result == true)
            {
                txtSource.Text = source.FolderName;
            }
        }

        private void btnDestination_Click(object sender, RoutedEventArgs e)
        {
            var dest = new OpenFolderDialog();
            var result = dest.ShowDialog();
            if (result == true)
            {
                txtDestination.Text = dest.FolderName;
            }      
        }

        private void btnResize_Click(object sender, RoutedEventArgs e)
        {
            var xmlReader = new XmlReaderService();
           // string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XmlTempPath", "screen_layout.xml");
            var imgDetails = xmlReader.XmlParser("E:\\Repos\\ScreenManager\\ScreenManager\\XmlTempPath\\screen_layout.xml");

            var xmlReaderService = new XmlReaderService();
            var imgResizerService = new ImageResizerService();
            var imgProcessingService = new ImageProcessingService(xmlReaderService, imgResizerService);

            imgProcessingService.ProcessImages(txtSource.Text, txtDestination.Text);


            //var imgResize = new ImageResizerService();
           // imgResize.ResizeIamge(txtSource.Text, txtDestination.Text, imgDetails.ImageDetails.Select(d => d.Width).FirstOrDefault(), imgDetails.ImageDetails.Select(d => d.Height).FirstOrDefault());
        }
    }
}
