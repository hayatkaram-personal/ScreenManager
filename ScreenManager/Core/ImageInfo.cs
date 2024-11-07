using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenManager.Core
{
    public class ImageInfo
    {
        public List<ImageDetails> ImageDetails { get; set; }
    }
    public class ImageDetails
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
