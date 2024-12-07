using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenManager.Business;
using ScreenManager.DataAccess;
using ScreenManager.Helper;
using ScreenManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ScreenManager.Presentation.VM
{
    public partial class MainViewModel : ObservableObject
    {
        /// <summary>
        /// Property bind to Source folder
        /// </summary>
        [ObservableProperty]
        public string source;

        //Property bind to Destination folder
        [ObservableProperty]
        public string destination;

        //Property used to display files in UI that are currently processed
        [ObservableProperty]
        public string processingFile;

        [ObservableProperty]
        public string processingInfo;

        //Property used to display current progress 
        [ObservableProperty]
        public int progressValue;

        [ObservableProperty]
        public int progressValueMainProgressbar;

        //Property bind to progressbar max value
        [ObservableProperty]
        public int progressMaxValue = 100;

        [ObservableProperty]
        public int progressMaxValueMainProgressbar = 100;

        //Source textbox text color. used to change this in case of error
        [ObservableProperty]
        public Brush foregroundSource;

        //Destination textbox text color used when error occurs
        [ObservableProperty]
        public Brush foregroundDest;

        //Property used to display current progress percentage
        [ObservableProperty]
        public int progressPercentage;

        [ObservableProperty]
        public int progressPercentageMainProgressBar;

        //Property use to enable/disable button
        [ObservableProperty]
        public bool isEnabled = true;

        private MainViewModel(Brush color)
        {
            foregroundSource = color;
            foregroundDest = color;
        }

        private static MainViewModel _instance;

        /// <summary>
        /// Single instance for VM to maintain UI consistant updation
        /// </summary>
        public static MainViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MainViewModel(Brushes.Black);
                return _instance;
            }
        }

        /// <summary>
        /// Image resize starts here
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        public async void Resize(object obj)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Source) || Source == "Please select source folder*")
                {
                    Source = "Please select source folder*";
                    ForegroundSource = Brushes.Red;
                    return;
                }
                else if (string.IsNullOrWhiteSpace(Destination))
                {
                    Destination = Source + "\\Destination";
                    if (Directory.Exists(Destination))
                        Directory.Delete(Destination, true);
                    Directory.CreateDirectory(Destination);
                }

                IsEnabled = false; ;

                var xmlReader = new XmlReaderService();

                var xmlReaderService = new XmlReaderService();
                var imgResizerService = new ImageResizerService();
                var imgProcessingService = new ImageProcessingService(xmlReaderService, imgResizerService);

                //Send image for resizing
                var result = await imgProcessingService.ProcessImages(Source, Destination);
                if (result == false)
                {
                    if (obj is Window window)
                        Notification.Notify(window, "Xml file not found", NotificationType.Error);
                }
                else
                {
                    if (obj is Window window)
                        Notification.Notify(window, "Resized successfully", NotificationType.Success);
                }

                IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logs.Logs.LogError(ex);
            }
        }
    }
}
