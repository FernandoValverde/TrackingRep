using Foundation;
using TrackingApp.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(TrackingApp.iOS.Version_iOS))]
namespace TrackingApp.iOS
{
    public class Version_iOS : IAppVersion
    {
        public string GetVersion()
        {
            return NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
        }
        public int GetBuild()
        {
            return int.Parse(NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString());
        }
    }
}