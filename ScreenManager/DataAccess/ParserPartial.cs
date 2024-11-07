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
        public ImageSetting ImageSetting { get; set; } = new ImageSetting();
        public List<ThemeSetting> Settings { get; set; } = new List<ThemeSetting>();
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
