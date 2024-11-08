namespace ScreenManager.DataAccess
{
    public class ThemeSetting
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Element
    {
        public string Name { get; set; }
        public string ImageName { get; set; }
        public EnabledSettings EnabledSettings { get; set; } = new EnabledSettings();
        public ImageSetting ImageSetting { get; set; } = new ImageSetting();
        public List<ThemeSetting> Settings { get; set; } = new List<ThemeSetting>();
    }

    public class EnabledSettings
    {
        public bool FbEnabled { get; set; }
        public bool LastballEnabled { get; set; }
        public bool VerifyEnabled { get; set; }
    }

    public class ImageSetting
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class ThemeLayout
    {
        public List<Element> Elements { get; set; } = new List<Element>();
    }
}
