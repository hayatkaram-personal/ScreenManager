using ScreenManager.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;


namespace ScreenManager.Services
{
    public class ImageResizerService : Core.IImageResizerService
    {
        public void ResizeIamge(string srcImgPath,string destImgPath, int width, int height)
        {
            if (!File.Exists(srcImgPath))
                throw new FileNotFoundException("Image file not found.");

            var size = new Size(width, height);
            using (var image = Image.FromFile(srcImgPath))
            {
                using (var resizedImage = new Bitmap(image, size))
                {                    
                    var outputPath = Path.Combine(destImgPath , Path.GetFileName(srcImgPath));

                    resizedImage.Save(outputPath);
                }
            }
        }
    }
}
