using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PC_Controller
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            MaxAutoImageSizeEntry.Text = Settings.MaxAutomaticImagePreviewSize.ToString();
            MaxAutoTextSizeEntry.Text = Settings.MaxAutomaticTextPreviewSize.ToString();
            TextInLabel.IsToggled = Settings.ViewTextFilesInLabel;
        }

        protected override void OnDisappearing()
        {
            Button_Clicked(null, EventArgs.Empty);
            base.OnDisappearing();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //save settings
            Settings.MaxAutomaticImagePreviewSize = int.Parse(MaxAutoImageSizeEntry.Text);
            Settings.MaxAutomaticTextPreviewSize = int.Parse(MaxAutoTextSizeEntry.Text);
            Settings.ViewTextFilesInLabel = TextInLabel.IsToggled;
            Settings.Save();
        }
    }
}