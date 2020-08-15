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
using PC_Controller.Views;
namespace PC_Controller
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ControlPage : ContentPage
    {
        static string[] CancelButtonTexts = { "FUCK!", "SHIT", "WHAT A PIECE OF SHIT", "FUCK YOU JAN_6006 FOR MAKING SHITTY APP", "FUCK YOU!", "PISS", "DAMNIT", "This is utterly fucking retarded."
, "GOD DAMNIT", "OH YOU ASS", "y tho", "please do not consider suicide", "Yanderedev coding happened",
"Xamarin forms happened", "Microsoft happened", "Jan0660 coding happened", "Jan6006 coding happened",
"REEEEEEEEEEEEEEEEE"};
        static Random random = new Random();

        public ControlPage(string IPString)
        {
            InitializeComponent();
            Connect(IPString);
            PCInformation.UpdateAll();
            if (PCInformation.CompatibilityVersion != App.CompatibilityVersion)
            {
                SendData("DISCONNECT");
                throw new Exception("Incompatible");
            }
            if (!PCInformation.IsPortableDevice)
                PowerInfo.IsVisible = false;
            PCInfo1.Text = "Hostname: " + SendAndReceive("GET_HOSTNAME") + Environment.NewLine + "Local IP address: " + SendAndReceive("GET_LOCAL_IP");
            Device.StartTimer(TimeSpan.FromMilliseconds(250), () =>
            {
                try
                {
                    // only update when this page is the currently viewed page
                    if (Navigation.NavigationStack.Last() == this)
                    {
                        PCInfo2.Text = "CPU usage: " + Request("GET_CPU_USAGE") + "%";
                        PCInfo2.Text += Environment.NewLine + "Free memory: " + ConvertBytesToMegabytes(SendAndReceive("GET_RAM_FREE")).ToString() + "MB";
                        PCInfo2.Text += Environment.NewLine + "Installed memory: " + ConvertBytesToMegabytes(PCInformation.InstalledRam).ToString() + "MB";
                        if (PCInformation.IsPortableDevice == true)
                        {
                            PowerInfo.Text = "Battery status: " + SendAndReceive("GET_BATTERY_PERCENTAGE") + "%";
                            string str = SendAndReceive("GET_POWERLINE_STATUS");
                            if (str == "Online")
                            {
                                PowerInfo.Text += Environment.NewLine + "Plugged In.";
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    System.Diagnostics.Debug.WriteLine("Exception thrown at the ControlPage timer at: " + exc.StackTrace + " / Message: " + exc.Message);
                    if (exc.Message == "The socket has been shut down")
                    {
                        return false;
                    }
                    return true;
                }
                return true; // True = Repeat again, False = Stop the timer
            });
        }

        static double ConvertBytesToMegabytes(ulong bytes)
        {
            try
            {
                return Math.Round((bytes / 1024f) / 1024f);
            }
            catch
            {
                return 0.0;
            }
        }

        public virtual double ConvertBytesToMegabytes(string str)
        {
            try
            {
                return ConvertBytesToMegabytes(Convert.ToUInt64(str));
            }
            catch
            {
                return 0.0;
            }
        }

        public static string GetCancelButtonText()
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

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            // disconnect
            // i have to remove the NavigationPage BackButton because you CANT FUCKING INTERCEPT IT
            // https://github.com/xamarin/Xamarin.Forms/issues/1944 this issue about it has been open for 2+ fucking years
            SendData("DISCONNECT");
            base.OnBackButtonPressed();
            Navigation.PopAsync();
        }

        private void Button_Clicked_5(object sender, EventArgs e)
        {
            // control clicked
            ServerSettingsPage page = new ServerSettingsPage();
            Navigation.PushAsync(page);
        }

        private void Button_Clicked_6(object sender, EventArgs e)
        {
            //start process
            StartProcessPage page = new StartProcessPage();
            Navigation.PushAsync(page);
        }

        private void Button_Clicked_7(object sender, EventArgs e)
        {
            Views.FileExplorer page = new Views.FileExplorer();
            Navigation.PushAsync(page);
        }

        private void Button_Clicked_4(object sender, EventArgs e)
        {
            TrollToysPage page = new TrollToysPage();
            Navigation.PushAsync(page);
        }
    }
}