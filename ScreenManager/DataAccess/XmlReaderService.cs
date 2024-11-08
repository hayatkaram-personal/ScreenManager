using ScreenManager.Core;
using System.Xml.Linq;

namespace ScreenManager.DataAccess
{
    public class XmlReaderService : IXmlReaderService
    {
        public ThemeLayout XmlParser(string filePath)
        {
            var document = XDocument.Load(filePath);

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

                    if(setting.Name == "fb_enabled")
                    {
                        element.EnabledSettings.FbEnabled = bool.Parse(setting.Value);
                    }
                    else if (setting.Name == "lastball_enabled")
                    {
                        element.EnabledSettings.LastballEnabled = bool.Parse(setting.Value);
                    }
                    else if(setting.Name == "verify_enabled")
                    {
                        element.EnabledSettings.VerifyEnabled = bool.Parse(setting.Value);
                    }
                    else if (setting.Name == "background")
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

            return themeLayout;
        }
    }

}
