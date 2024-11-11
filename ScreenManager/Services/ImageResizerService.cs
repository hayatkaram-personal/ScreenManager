using System.Drawing;
using System.IO;

namespace ScreenManager.Services
{
    public class ImageResizerService : Core.IImageResizerService
    {
        public async Task ResizeIamge(string srcImgPath, string destImgPath, int width, int height)
        {
            if (!File.Exists(srcImgPath))
                throw new FileNotFoundException("Image file not found.");
            var tempPath = destImgPath + "\\TempFolder"; //Path.Combine(destImgPath + ");

            if (!Directory.Exists(tempPath ))
                Directory.CreateDirectory(tempPath);

            tempPath = Path.Combine(tempPath, Path.GetFileName(srcImgPath));
            File.Copy(srcImgPath, tempPath, true);

            var size = new Size(width, height);
            using (var image = Image.FromFile(tempPath))
            {
                using (var resizedImage = new Bitmap(image, size))
                {
                    var outputPath = Path.Combine(destImgPath, Path.GetFileName(srcImgPath));
                    resizedImage.Save(outputPath);

                }
            }
            Directory.Delete(destImgPath + "\\TempFolder", true);

        }
    }
}
