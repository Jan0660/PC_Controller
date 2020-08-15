using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PC_Controller.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Communication.Main;
namespace PC_Controller.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilePreviewPage : ContentPage
    {
        Models.File file;
        public FilePreviewPage(Models.File file)
        {
            this.file = file;
            InitializeComponent();
            Title = file.Name;
            InfoLabel.Text = $@"Path: {file.FullName}
Size: { file.Size} bytes";
            Preview(false);
        }

        void Preview(bool confirm)
        {
            string extension = Path.GetExtension(file.FullName);
            switch (extension)
            {
                case ".png":
                case ".jpg":
                case ".bmp":
                    if (file.Size < Settings.MaxAutomaticImagePreviewSize | confirm)
                    {
                        ViewImage();
                    }
                    else
                    {
                        PreviewConfirmLayout.IsVisible = true;
                    }
                    break;
                case ".txt":
                    if(file.Size < Settings.MaxAutomaticTextPreviewSize | confirm)
                    {
                        PreviewLabel.IsVisible = true;
                        PreviewLayout.IsVisible = true;
                        SendData("GET_FILE:" + file.FullName);
                        if (Settings.ViewTextFilesInLabel)
                        {
                            PreviewLabel.IsVisible = true;
                            PreviewLabel.Text = Encoding.ASCII.GetString(ReceiveBytes());
                        }
                        else
                        {
                            PreviewEditor.IsVisible = true;
                            PreviewEditor.Text = Encoding.ASCII.GetString(ReceiveBytes());
                        }
                    }
                    else
                    {
                        PreviewConfirmLayout.IsVisible = true;
                    }
                    break;
            }
        }

        void ViewImage()
        {
            SendData("GET_FILE:" + file.FullName);
            byte[] bytes = ReceiveAvailableBytes(1);
            PreviewLayout.IsVisible = true;
            PreviewImage.IsVisible = true;
            // stolen from https://forums.xamarin.com/discussion/19853/load-image-form-byte-array
            PreviewImage.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //confirm preview
            PreviewConfirmLayout.IsVisible = false;
            Preview(true);
        }
    }
}