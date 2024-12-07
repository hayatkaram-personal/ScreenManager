using System.IO;
using System.Windows.Threading;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ScreenManager.Services
{
    public class ImageResizerService : Core.IImageResizerService
    {
        /// <summary>
        /// Function used to resize image and copy it in destination folder
        /// </summary>
        /// <param name="srcImgPath"></param>
        /// <param name="destImgPath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public async Task ResizeIamge(string srcImgPath, string destImgPath, int width, int height)
        {
            if (!File.Exists(srcImgPath))
                throw new FileNotFoundException("Image file not found.");
            var tempPath = destImgPath + "\\TempFolder"; //Path.Combine(destImgPath + ");

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            tempPath = Path.Combine(tempPath, Path.GetFileName(srcImgPath));

            try
            {
                using (var srcStream = File.OpenRead(srcImgPath))
                using (var dstStream = File.Create(tempPath))
                {
                    await srcStream.CopyToAsync(dstStream);
                }
                var size = new System.Drawing.Size(width, height);
                using (var image = System.Drawing.Image.FromFile(tempPath))
                using (var resizedImage = new System.Drawing.Bitmap(image, size))
                {
                    var outputPath = Path.Combine(destImgPath, Path.GetFileName(tempPath));
                    resizedImage.Save(outputPath);
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (Directory.Exists(destImgPath + "\\TempFolder"))
                    Directory.Delete(destImgPath + "\\TempFolder", true);
            }

            //File.Copy(srcImgPath, tempPath, true);

            ////var size = new Size(width, height);
            //using (var image = Image.FromFile(tempPath))
            //{
            //    using (var resizedImage = new Bitmap(image, size))
            //    {
            //        var outputPath = Path.Combine(destImgPath, Path.GetFileName(srcImgPath));
            //        resizedImage.Save(outputPath);

            //    }
            //}
            //Directory.Delete(destImgPath + "\\TempFolder", true);

        }
        public async Task ReSizeRemainingImages(string src, string dest, int width, int height)
        {
            try
            {
                using (var image = await Image.LoadAsync(src))
                {
                    // Resize the image
                    image.Mutate(x => x.Resize(width, height));

                    // Save the resized image
                    var outputPath = Path.Combine(dest, Path.GetFileName(src));
                    await image.SaveAsync(outputPath);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
