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
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace ScreenManager.Presentation
{
    /// <summary>
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class Loading : Window
    {
        public Loading()
        {
            InitializeComponent();
            //var imageUri = new Uri(""pack://application:,,,/ScreenManager;component/Images/loading-gif.gif""); 
            //  var image = new BitmapImage(imageUri);
            // ImageBehavior.SetAnimatedSource(GifImage, image);
        }
    }
}
