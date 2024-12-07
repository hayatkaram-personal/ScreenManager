namespace ScreenManager.DataAccess
{
    /// <summary>
    /// Class used to read the setting tags in xml
    /// </summary>
    public class ThemeSetting
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// This class is used to read data of top element node
    /// </summary>
    public class Element
    {
        public string Name { get; set; }
        public string ImageName { get; set; }
        public EnabledSettings EnabledSettings { get; set; } = new EnabledSettings();
        public ImageSetting ImageSetting { get; set; } = new ImageSetting();
        public List<ThemeSetting> Settings { get; set; } = new List<ThemeSetting>();
    }

    /// <summary>
    /// Class used to store properties used to check which folders to resize
    /// </summary>
    public class EnabledSettings
    {
        public bool FbEnabled { get; set; }
        public bool LastballEnabled { get; set; }
        public bool VerifyEnabled { get; set; }
    }

    /// <summary>
    /// Class use to store image name and its resolution
    /// </summary>
    public class ImageSetting
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    /// <summary>
    /// Class use to store objects of whole xml
    /// </summary>
    public class ThemeLayout
    {
        public List<Element> Elements { get; set; } = new List<Element>();
    }
}
