using ScreenManager.Core;
using ScreenManager.DataAccess;
using ScreenManager.Presentation;
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

        public ImageProcessingService(IXmlReaderService xmlReaderService, IImageResizerService imageResizerService)
        {
            _xmlReaderService = xmlReaderService;
            _imageResizerService = imageResizerService;
        }

        public async Task ProcessImages(string source, string destination)
        {
            var xmlFilePath = Directory.GetFiles(source, "screen_layout.xml");

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

            ImageResizer.MainViewModel.ProgressValue = 0;

            var files = Directory.GetFiles(source);
            ImageResizer.MainViewModel.ProgressMaxValue = imgSettingList.Count + files.Length;

            var directories = Directory.GetDirectories(source);
            foreach (var directory in directories)
            {
                ImageResizer.MainViewModel.ProgressMaxValue +=  Directory.GetFiles(directory).Length;
            }

            foreach (var imgDetails in imgSettingList)
            {
                foreach (var img in files)
                {
                    if (img.Contains(imgDetails.Name))
                    {
                        ImageResizer.MainViewModel.ProcessingFile ="Resizing : " + Path.GetFileName(img).ToString();
                        ImageResizer.MainViewModel.ProgressValue += 1;
                        ImageResizer.MainViewModel.ProgressPercentage = (int)((ImageResizer.MainViewModel.ProgressValue / (double)ImageResizer.MainViewModel.ProgressMaxValue) * 100);
                        
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
                        ImageResizer.MainViewModel.ProcessingFile ="Resizing : " + Path.GetFileName(file).ToString();
                        ImageResizer.MainViewModel.ProgressValue += 1;
                        ImageResizer.MainViewModel.ProgressPercentage = (int)((ImageResizer.MainViewModel.ProgressValue / (double)ImageResizer.MainViewModel.ProgressMaxValue) * 100);

                        await Task.Run(() =>  _imageResizerService.ResizeIamge(file, destFolder, resizeFb.Width, resizeFb.Height));
                    }
                }
                else if (enabledSettings.LastballEnabled && (folderName == "balls_calling" || folderName == "balls_calling_90"))
                {
                    if (!Directory.Exists(destFolder))
                        Directory.CreateDirectory(destFolder);
                    foreach (var file in dirFiles)
                    {
                        ImageResizer.MainViewModel.ProcessingFile ="Resizing : " + Path.GetFileName(file).ToString();
                        ImageResizer.MainViewModel.ProgressValue += 1;
                        ImageResizer.MainViewModel.ProgressPercentage = (int)((ImageResizer.MainViewModel.ProgressValue / (double)ImageResizer.MainViewModel.ProgressMaxValue) * 100);

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
                        ImageResizer.MainViewModel.ProcessingFile ="Resizing : " + Path.GetFileName(file).ToString();
                        ImageResizer.MainViewModel.ProgressValue += 1;
                        ImageResizer.MainViewModel.ProgressPercentage = (int)((ImageResizer.MainViewModel.ProgressValue / (double)ImageResizer.MainViewModel.ProgressMaxValue) * 100);

                        await Task.Run(() => _imageResizerService.ResizeIamge(file, destFolder, verifyCard.Width, verifyCard.Height));
                    }
                }
            }
            ImageResizer.MainViewModel.ProgressValue = ImageResizer.MainViewModel.ProgressMaxValue;
            ImageResizer.MainViewModel.ProcessingFile = "Done.";
            ImageResizer.MainViewModel.ProgressPercentage = 100;
        }
    }
}
