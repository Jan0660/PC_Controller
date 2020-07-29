using Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Communication.Main;
namespace PC_Controller
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ControlPage : ContentPage
    {
        string[] CancelButtonTexts = { "FUCK!", "SHIT", "WHAT A PIECE OF SHIT", "FUCK YOU JAN_6006 FOR MAKING SHITTY APP", "FUCK YOU!", "PISS", "DAMNIT", "This is utterly fucking retarded."
                , "GOD DAMNIT", "OH YOU ASS", "y tho" };
        Random random = new Random();

        public ControlPage()
        {
            InitializeComponent();
            Controller.Connect();
            PCInformation.UpdateAll();
            if (!PCInformation.IsPortableDevice)
                PowerInfo.IsVisible = false;
            PCInfo1.Text = "Hostname: " + SendAndReceive("GET_HOSTNAME") + Environment.NewLine + "Local IP address: " + SendAndReceive("GET_LOCAL_IP");
            Device.StartTimer(TimeSpan.FromMilliseconds(250), () =>
            {
                PCInfo2.Text = "CPU usage: " + SendAndReceive("GET_CPU_USAGE") + "%";
                PCInfo2.Text += Environment.NewLine + "Free memory: " + ConvertBytesToMegabytes(Convert.ToUInt64(SendAndReceive("GET_RAM_FREE"))).ToString() + "MB";
                PCInfo2.Text += Environment.NewLine + "Installed memory: " + ConvertBytesToMegabytes(PCInformation.InstalledRam).ToString() + "MB";
                if (PCInformation.IsPortableDevice == true)
                {
                    PowerInfo.Text = "Battery status: " + SendAndReceive("GET_BATTERY_PERCENTAGE") + "%";
                    string str = SendAndReceive("GET_POWERLINE_STATUS");
                    if(str == "Online")
                    {
                        PowerInfo.Text += Environment.NewLine + "Plugged In.";
                    }
                    
                }
                return true; // True = Repeat again, False = Stop the timer
            });
        }

        static double ConvertBytesToMegabytes(ulong bytes)
        {
            return Math.Round((bytes / 1024f) / 1024f);
        }

        public string GetCancelButtonText()
        {
            return CancelButtonTexts[random.Next(CancelButtonTexts.Length)];
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                string str = SendAndReceive("TEST");
                await DisplayAlert("i guess it gone ok", str, "OK BOOMER");
            }
            catch (Exception exc)
            {
                await DisplayAlert("Exception thrown", exc.Message, GetCancelButtonText());
            }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            try
            {
                string str = SendAndReceive("LOGOFF");
            }
            catch (Exception exc)
            {
                await DisplayAlert("Exception thrown", exc.Message, GetCancelButtonText());
            }
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            try
            {
                string str = SendAndReceive("SHUTDOWN");
            }
            catch (Exception exc)
            {
                await DisplayAlert("Exception thrown", exc.Message, GetCancelButtonText());
            }
        }

        private void Button_Clicked_3(object sender, EventArgs e)
        {
            SerialControlPage page = new SerialControlPage();
            Navigation.PushAsync(page);
        }
    }
}