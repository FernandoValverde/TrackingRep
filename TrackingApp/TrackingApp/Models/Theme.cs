using SQLite;

namespace TrackingApp.Models
{
    public class Theme
    {
        [PrimaryKey]
        public int ThemeId { get; set; }

        public string MainBarColor { get; set; }

        public string SecondaryBarColor { get; set; }

        public string BackgroundColor { get; set; }

        public string BarTextColor { get; set; }
    }
}
