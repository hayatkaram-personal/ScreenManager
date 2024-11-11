using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ScreenManager.Presentation.VM
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        public string source;

        [ObservableProperty]
        public string destination;
        
        [ObservableProperty]
        public string processingFile;

        [ObservableProperty]
        public int progressValue;

        [ObservableProperty]
        public int progressMaxValue =100;

        [ObservableProperty]
        public Brush foregroundSource;

        [ObservableProperty]
        public Brush foregroundDest;

        [ObservableProperty]
        public int progressPercentage;

        public MainViewModel(Brush color)
        {
            foregroundSource = color;
            foregroundDest = color;
        }

    }
}
