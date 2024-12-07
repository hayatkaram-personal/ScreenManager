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
        private MainViewModel mainViewModel = MainViewModel.Instance;

        public ImageResizer()
        {
            InitializeComponent();

            this.DataContext = mainViewModel;
        }

        /// <summary>
        /// Use to load source folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSource_Click(object sender, RoutedEventArgs e)
        {
            var source = new OpenFolderDialog();
            var result = source.ShowDialog();
            if (result == true)
            {
                mainViewModel.ForegroundSource = Brushes.Black;
                mainViewModel.Source = source.FolderName;
            }
        }

        /// <summary>
        /// Use to load the destination folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDestination_Click(object sender, RoutedEventArgs e)
        {
            var dest = new OpenFolderDialog();
            var result = dest.ShowDialog();
            if (result == true)
            {
                mainViewModel.ForegroundDest = Brushes.Black;
                mainViewModel.Destination = dest.FolderName;
            }
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
