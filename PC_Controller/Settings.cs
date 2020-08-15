using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PC_Controller
{
    public static class Settings
    {
        public static int MaxAutomaticImagePreviewSize;
        public static int MaxAutomaticTextPreviewSize;
        public static bool ViewTextFilesInLabel;
        public static void Load()
        {
            try
            {
                if ((string)App.Current.Properties["LastVersion"] != Xamarin.Essentials.VersionTracking.CurrentBuild)
                {
                    SetDefaults();
                    App.Current.Properties["LastVersion"] = Xamarin.Essentials.VersionTracking.CurrentBuild;
                }
            }
            catch
            {
                SetDefaults();
                App.Current.Properties["LastVersion"] = Xamarin.Essentials.VersionTracking.CurrentBuild;
            }
            MaxAutomaticImagePreviewSize = (int)App.Current.Properties["MaxAutomaticImagePreviewSize"];
            MaxAutomaticTextPreviewSize = (int)App.Current.Properties["MaxAutomaticTextPreviewSize"];
            ViewTextFilesInLabel = (bool)App.Current.Properties["ViewTextFilesInLabel"];
        }

        public static void SetDefaults()
        {
            App.Current.Properties["MaxAutomaticImagePreviewSize"] = 400000;
            App.Current.Properties["MaxAutomaticTextPreviewSize"] = 10000;
            App.Current.Properties["ViewTextFilesInLabel"] = false;
        }

        public static void Save()
        {
            App.Current.Properties["MaxAutomaticImagePreviewSize"] = MaxAutomaticImagePreviewSize;
            App.Current.Properties["MaxAutomaticTextPreviewSize"] = MaxAutomaticTextPreviewSize;
            App.Current.Properties["ViewTextFilesInLabel"] = ViewTextFilesInLabel;
            App.Current.SavePropertiesAsync();
        }
    }
}
