using ScreenManager.Core;
using ScreenManager.DataAccess;
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

        public void ProcessImages(string source, string destination)
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "\\XmlTempPath\\screen_layout.xml";
            var xmlFilePath = Path.Combine(source, "E:\\Repos\\ScreenManager\\ScreenManager\\XmlTempPath\\screen_layout.xml");

            var themeLayout = _xmlReaderService.XmlParser(xmlFilePath);
            //var imageInfo = new ImageInfo();
            //imageInfo.ImageDetails = new List<ImageDetails>();

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


            var files = Directory.GetFiles(source);

            foreach (var imgDetails in imgSettingList)
            {
                foreach (var img in files)
                {
                    if (img.Contains(imgDetails.Name))
                    {
                        _imageResizerService.ResizeIamge(img, destination, imgDetails.Width, imgDetails.Height);
                        break;
                    }
                }
            }


            var enabledSettings = new EnabledSettings();

            enabledSettings = themeLayout.Elements.Select(d => d.EnabledSettings).FirstOrDefault();

            var resizeFb = themeLayout.Elements.Where(d => d.Name == _fbResize).Select(x => x.ImageSetting).FirstOrDefault();
            var lastball = themeLayout.Elements.Where(d => d.Name == _lastballResize).Select(x => x.ImageSetting).FirstOrDefault();
            var verifyCard = themeLayout.Elements.Where(d => d.Name == _verifyResize).Select(x => x.ImageSetting).FirstOrDefault();

            var directories = Directory.GetDirectories(source);
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
                        _imageResizerService.ResizeIamge(file, destFolder, resizeFb.Width, resizeFb.Height);
                    }
                }
                else if (enabledSettings.LastballEnabled && (folderName == "balls_calling" || folderName == "balls_calling_90"))
                {
                    if (!Directory.Exists(destFolder))
                        Directory.CreateDirectory(destFolder);
                    foreach (var file in dirFiles)
                    {
                        _imageResizerService.ResizeIamge(file, destFolder, lastball.Width, lastball.Height);
                    }
                }
                else if (enabledSettings.VerifyEnabled && (folderName == "verify_called" || folderName == "verify_uncalled" || folderName == "verify_lastnumbers" ||
                        folderName == "verify_hotballs_called" || folderName == "verify_hotballs_uncalled"))
                {
                    if (!Directory.Exists(destFolder))
                        Directory.CreateDirectory(destFolder);
                    foreach (var file in dirFiles)
                    {
                        _imageResizerService.ResizeIamge(file, destFolder, verifyCard.Width, verifyCard.Height);
                    }
                }
            }
        }
    }
}
