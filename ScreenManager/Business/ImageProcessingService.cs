using ScreenManager.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenManager.Business
{
    public class ImageProcessingService
    {
        private readonly IXmlReaderService _xmlReaderService;
        private readonly IImageResizerService _imageResizerService;

        public ImageProcessingService(IXmlReaderService xmlReaderService, IImageResizerService imageResizerService)
        {
            _xmlReaderService = xmlReaderService;
            _imageResizerService = imageResizerService;
        }

        public void ProcessImages(string source, string destination)
        {
            var xmlFilePath = Path.Combine(source, "E:\\Repos\\ScreenManager\\ScreenManager\\XmlTempPath\\screen_layout.xml");
            var imageInfo = _xmlReaderService.XmlParser(xmlFilePath);

            var files = Directory.GetFiles(source);

            foreach (var imgDetails in imageInfo.ImageDetails)
            {
                foreach(var img in files)
                {
                    if (img.Contains(imgDetails.Name))
                    {
                        _imageResizerService.ResizeIamge(img, destination, imgDetails.Width, imgDetails.Height);
                        break;
                    }
                }

                //var imgPath = Path.Combine(source, imgDetails.Name);
                //if(File.Exists(imgPath))
                //{
                //    _imageResizerService.ResizeIamge(imgPath, destination, imgDetails.Width, imgDetails.Height);
                //}
            }
        }
    }
}
