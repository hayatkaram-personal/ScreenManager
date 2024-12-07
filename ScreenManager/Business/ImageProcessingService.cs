using ScreenManager.Core;
using ScreenManager.DataAccess;
using ScreenManager.Presentation.VM;
using System.Drawing;
using System.IO;

namespace ScreenManager.Business
{
    public class ImageProcessingService
    {
        private readonly IXmlReaderService _xmlReaderService;
        private readonly IImageResizerService _imageResizerService;

        private const string _fbResize = "fb_numbers_75";
        private const string _lastballResize = "lastball_image";
        private const string _verifyResize = "verify_card_cells";

        private MainViewModel mainViewModel = MainViewModel.Instance;

        public ImageProcessingService(IXmlReaderService xmlReaderService, IImageResizerService imageResizerService)
        {
            _xmlReaderService = xmlReaderService;
            _imageResizerService = imageResizerService;
        }

        /// <summary>
        /// Function processing image. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public async Task<bool> ProcessImages(string source, string destination)
        {
            try
            {

                var xmlFilePath = Directory.GetFiles(source, "screen_layout.xml");
                if (xmlFilePath.Length == 0)
                    return false;

                var themeLayout = _xmlReaderService.XmlParser(xmlFilePath[0].ToString());

                var imageDetailList = themeLayout.Elements.Select(d => d.ImageSetting).ToList()
        .Where(x => x.Name != null && x.Width > 0).ToList();

                var imgSettingList = new List<ImageSetting>();

                foreach (var imgSetting in imageDetailList)
                {
                    imgSettingList.Add(new ImageSetting
                    {
                        Name = imgSetting.Name,
                        Width = imgSetting.Width,
                        Height = imgSetting.Height
                    });
                }

                mainViewModel.ProgressValue = 0;

                var files = Directory.GetFiles(source);
                var maxProgressValue = imgSettingList.Count;

                var directories = Directory.GetDirectories(source);
                foreach (var directory in directories)
                {
                    maxProgressValue += Directory.GetFiles(directory).Length;
                }
                mainViewModel.ProgressMaxValue = maxProgressValue;

                var random = new Random();
                foreach (var imgDetails in imgSettingList)
                {
                    foreach (var img in files)
                    {
                        if (img.Contains(imgDetails.Name))
                        {
                            ProgressBar("Resizing : ", "Resizing", Path.GetFileName(img).ToString(), random.Next(0, 30), 1, 0, 0, 0, 0);
                            await Task.Run(() => _imageResizerService.ResizeIamge(img, destination, imgDetails.Width, imgDetails.Height));
                            break;
                        }
                    }
                }

                var enabledSettings = new EnabledSettings();

                enabledSettings = themeLayout.Elements.Select(d => d.EnabledSettings).FirstOrDefault();

                var resizeFb = themeLayout.Elements.Where(d => d.Name == _fbResize).Select(x => x.ImageSetting).FirstOrDefault();
                var lastball = themeLayout.Elements.Where(d => d.Name == _lastballResize).Select(x => x.ImageSetting).FirstOrDefault();
                var verifyCard = themeLayout.Elements.Where(d => d.Name == _verifyResize).Select(x => x.ImageSetting).FirstOrDefault();

                foreach (var directory in directories)
                {
                    var dirFiles = Directory.GetFiles(directory);
                    var folderName = Path.GetFileName(directory);
                    var destFolder = Path.Combine(destination, folderName);

                    if (Directory.Exists(destFolder))
                    {
                        Directory.Delete(destFolder, true);
                    }

                    if (enabledSettings.FbEnabled && (folderName == "balls_called" || folderName == "balls_uncalled" || folderName == "balls_last" ||
                            folderName == "hotballs_called" || folderName == "hotballs_uncalled"))
                    {
                        if (!Directory.Exists(destFolder))
                            Directory.CreateDirectory(destFolder);

                        foreach (var file in dirFiles)
                        {
                            ProgressBar("Resizing : ", "Resizing", Path.GetFileName(file).ToString(), random.Next(0, 30), 1, 0, 0, 0, 0);

                            await Task.Run(() => _imageResizerService.ResizeIamge(file, destFolder, resizeFb.Width, resizeFb.Height));
                        }
                    }
                    else if (enabledSettings.LastballEnabled && (folderName == "balls_calling" || folderName == "balls_calling_90"))
                    {
                        if (!Directory.Exists(destFolder))
                            Directory.CreateDirectory(destFolder);
                        foreach (var file in dirFiles)
                        {
                            ProgressBar("Resizing : ", "Resizing", Path.GetFileName(file).ToString(), random.Next(0, 30), 1, 0, 0, 0, 0);

                            await Task.Run(() => _imageResizerService.ResizeIamge(file, destFolder, lastball.Width, lastball.Height));
                        }
                    }
                    else if (enabledSettings.VerifyEnabled && (folderName == "verify_called" || folderName == "verify_uncalled" || folderName == "verify_lastnumbers" ||
                            folderName == "verify_hotballs_called" || folderName == "verify_hotballs_uncalled"))
                    {
                        if (!Directory.Exists(destFolder))
                            Directory.CreateDirectory(destFolder);
                        foreach (var file in dirFiles)
                        {
                            ProgressBar("Resizing : ", "Resizing", Path.GetFileName(file).ToString(), random.Next(0, 30), 1, 100, 0, 0, 0);

                            await Task.Run(() => _imageResizerService.ResizeIamge(file, destFolder, verifyCard.Width, verifyCard.Height));
                        }
                    }

                }
                mainViewModel.ProcessingFile = "Done.";
                mainViewModel.ProgressPercentage = 100;
                mainViewModel.ProgressMaxValue = 100;

                await CopyRemainingImages(source, destination);
                await VerifyProcessedImages(destination, imgSettingList, enabledSettings, resizeFb, lastball, verifyCard);

                mainViewModel.ProcessingFile = "Done.";
                mainViewModel.ProgressPercentage = 100;
                mainViewModel.ProgressMaxValue = 100;

                mainViewModel.ProcessingInfo = "Done.";
                mainViewModel.ProgressValueMainProgressbar = 100;
                mainViewModel.ProgressPercentageMainProgressBar = 100;
                mainViewModel.ProgressMaxValueMainProgressbar = 100;
                return true;

            }
            catch (Exception ex)
            {
                Logs.Logs.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// Copy remaining images from source to destination
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destinationFolder"></param>
        /// <returns></returns>
        private async Task CopyRemainingImages(string sourceFolder, string destinationFolder)
        {
            try
            {
                mainViewModel.ProgressValue = 0;
                mainViewModel.ProgressPercentage = 0;
                mainViewModel.ProgressMaxValue = 100;

                //Copy mainfolder files
                await CopyImages(sourceFolder, destinationFolder);

                //Copy subfolder files
                var subDirectories = Directory.GetDirectories(sourceFolder);

                foreach (var drctry in subDirectories)
                {
                    var directoryName = Path.GetFileName(drctry);
                    var destPath = Path.Combine(destinationFolder, directoryName);
                    if (!Directory.Exists(destPath))
                        Directory.CreateDirectory(destPath);
                    await CopyImages(sourceFolder + "\\" + directoryName, destPath);
                }
                mainViewModel.ProgressValue = 100;
                mainViewModel.ProgressPercentage = 100;
                mainViewModel.ProgressMaxValue = 100;
            }

            catch (Exception ex)
            {
                Logs.Logs.LogError(ex);
            }

        }
        public async Task CopyImages(string sourceFolder, string destinationFolder)
        {
            try
            {
                if (sourceFolder == null || Directory.GetFiles(sourceFolder).Length == 0)
                    return;
                var sourceImg = Directory.GetFiles(sourceFolder);
                var destFolder = Directory.GetFiles(destinationFolder);

                var random = new Random();
                var fileCount = 0;
                foreach (var srcImg in sourceImg)
                {
                    var imgName = Path.GetFileName(srcImg);
                    var destPath = Path.Combine(destinationFolder, imgName);
                    if (!File.Exists(destPath))
                    {
                        fileCount++;
                    }
                }

                mainViewModel.ProgressMaxValue += fileCount;

                foreach (var srcImg in sourceImg)
                {
                    var imgName = Path.GetFileName(srcImg);
                    var destPath = Path.Combine(destinationFolder, imgName);
                    if (!File.Exists(destPath))
                    {
                        try
                        {
                            ProgressBar("Copy remaining files : ", "Copy remaining files", Path.GetFileName(srcImg).ToString(), 0, 1, 0, 0, 0, 0);
                            await Task.Delay(50);
                            await Task.Run(() => File.Copy(srcImg, destPath, overwrite: false));
                        }
                        catch (Exception ex)
                        {
                            Logs.Logs.LogError(ex);
                            //throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Logs.LogError(ex);
            }
        }

        /// <summary>
        /// Re-Verify all the images that are resized
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="imageSettings"></param>
        /// <param name="enabledSettings"></param>
        /// <param name="resizeFb"></param>
        /// <param name="lastball"></param>
        /// <param name="verifyCard"></param>
        /// <returns></returns>
        public async Task VerifyProcessedImages(string directory, List<ImageSetting> imageSettings, EnabledSettings enabledSettings, ImageSetting resizeFb, ImageSetting lastball, ImageSetting verifyCard)
        {
            try
            {
                mainViewModel.ProgressValue = 0;
                mainViewModel.ProgressPercentage = 0;
                mainViewModel.ProgressMaxValue = 100;
                await Task.Delay(50);
                var sourceImg = Directory.GetFiles(directory);
                var subDirectories = Directory.GetDirectories(directory);
                foreach (var img in imageSettings)
                {
                    foreach (var srcImg in sourceImg)
                    {
                        if (!string.IsNullOrWhiteSpace(img.Name) && srcImg.Contains(img.Name))
                        {
                            var (width, height) = GetImageDimensions(srcImg);
                            if (width != img.Width && height != img.Height)
                            {
                                ProgressBar("Verify resized images : ", "Verify resized images", Path.GetFileName(srcImg).ToString(), 0, 15, 0, 0, 0, 0);
                                await Task.Delay(100);
                                await _imageResizerService.ReSizeRemainingImages(srcImg, directory, img.Width, img.Height);
                                break;
                            }
                        }
                    }
                }

                foreach (var subDirectory in subDirectories)
                {
                    var dirFiles = Directory.GetFiles(subDirectory);
                    var folderName = Path.GetFileName(subDirectory);

                    if (enabledSettings.FbEnabled && (folderName == "balls_called" || folderName == "balls_uncalled" || folderName == "balls_last" ||
                            folderName == "hotballs_called" || folderName == "hotballs_uncalled"))
                    {
                        foreach (var file in dirFiles)
                        {
                            var (width, height) = GetImageDimensions(file);
                            if (width != resizeFb.Width && height != resizeFb.Height)
                                await _imageResizerService.ReSizeRemainingImages(file, directory, resizeFb.Width, resizeFb.Height);
                        }
                    }
                    else if (enabledSettings.LastballEnabled && (folderName == "balls_calling" || folderName == "balls_calling_90"))
                    {
                        foreach (var file in dirFiles)
                        {
                            var (width, height) = GetImageDimensions(file);
                            if (width != lastball.Width && height != lastball.Height)
                                await _imageResizerService.ReSizeRemainingImages(file, directory, lastball.Width, lastball.Height);
                        }
                    }
                    else if (enabledSettings.VerifyEnabled && (folderName == "verify_called" || folderName == "verify_uncalled" || folderName == "verify_lastnumbers" ||
                            folderName == "verify_hotballs_called" || folderName == "verify_hotballs_uncalled"))
                    {
                        foreach (var file in dirFiles)
                        {
                            var (width, height) = GetImageDimensions(file);
                            if (width != verifyCard.Width && height != verifyCard.Height)
                                await _imageResizerService.ReSizeRemainingImages(file, directory, verifyCard.Width, verifyCard.Height);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Logs.LogError(ex);
            }
        }
        public (int width, int height) GetImageDimensions(string filePath)
        {
            using (var image = Image.FromFile(filePath))
            {
                return (image.Width, image.Height);
            }
        }

        private void ProgressBar(string messageMain, string message, string file, int valueMain, int value, int maxValueMain, int maxValue, double perMain, double per)
        {
            try
            {
                mainViewModel.ProgressValue += value;
                mainViewModel.ProcessingFile = message + ": " + Path.GetFileName(file).ToString();
                if ((int)((mainViewModel.ProgressValue / (double)mainViewModel.ProgressMaxValue) * 100) > 97)
                    mainViewModel.ProgressPercentage = 95;
                else
                    mainViewModel.ProgressPercentage = (int)((mainViewModel.ProgressValue / (double)mainViewModel.ProgressMaxValue) * 100);

                if (messageMain.Contains("Resizing"))
                {
                    valueMain = (int)(mainViewModel.ProgressPercentage / 2);
                }
                else if (messageMain.Contains("Copy"))
                {
                    valueMain = mainViewModel.ProgressValueMainProgressbar;
                    if (mainViewModel.ProgressPercentage > 10 && mainViewModel.ProgressPercentage < 15)
                        valueMain += 1;
                    else if (mainViewModel.ProgressPercentage > 30 && mainViewModel.ProgressPercentage < 35)
                        valueMain += 1;
                    else if (mainViewModel.ProgressPercentage > 50 && mainViewModel.ProgressPercentage < 55)
                        valueMain += 1;
                    else if (mainViewModel.ProgressPercentage > 90 && mainViewModel.ProgressPercentage < 100)
                        valueMain += 1;
                    if (valueMain >= 90)
                    {
                        valueMain = 90;
                    }
                }
                else if (messageMain.Contains("Verify"))
                {
                    valueMain = mainViewModel.ProgressValueMainProgressbar;
                    if (valueMain < 99)
                        valueMain += 1;
                }
                mainViewModel.ProgressValueMainProgressbar = valueMain;
                mainViewModel.ProcessingInfo = messageMain;// + ": " + Path.GetFileName(file).ToString();
                mainViewModel.ProgressPercentageMainProgressBar = (int)((mainViewModel.ProgressValueMainProgressbar / (double)mainViewModel.ProgressMaxValueMainProgressbar) * 100);
            }
            catch (Exception ex)
            {
                Logs.Logs.LogError(ex);
            }
        }
    }
}
