using ScreenManager.Core;
using System.Xml.Linq;

namespace ScreenManager.DataAccess
{
    public class XmlReaderService : IXmlReaderService
    {
        public ImageInfo XmlParser(string filePath)
        {

            var document = XDocument.Load(filePath);

            var imageInfo = new ImageInfo();
            imageInfo.ImageDetails = new List<ImageDetails>();

            var themeLayout = new ThemeLayout();

            foreach (var elementNode in document.Descendants("element"))
            {
                var element = new Element
                {
                    Name = elementNode.Attribute("name")?.Value
                };

                foreach (var settingNode in elementNode.Elements("setting"))
                {
                    var setting = new ThemeSetting
                    {
                        Name = settingNode.Attribute("name")?.Value,
                        Value = settingNode.Value
                    };

                    if (setting.Name == "background")
                    {
                        element.ImageName = setting.Value;
                        element.ImageSetting.Name = settingNode.Value;
                    }
                    else if (setting.Name == "width")
                    {
                        element.ImageSetting.Width = int.Parse(setting.Value);
                    }
                    else if (setting.Name == "height")
                    {
                        element.ImageSetting.Height = int.Parse(setting.Value);
                    }

                    element.Settings.Add(setting);                    
                }

                themeLayout.Elements.Add(element);
            }

            var imageDetailList = themeLayout.Elements.Select(d => d.ImageSetting).ToList()
                .Where(x => x.Name != null && x.Width > 0).ToList();

            foreach (var imgSetting in imageDetailList)
            {
                imageInfo.ImageDetails.Add(new ImageDetails
                {
                    Name = imgSetting.Name,
                    Width = imgSetting.Width,
                    Height = imgSetting.Height
                });
            }

            return imageInfo;
        }
    }

}
