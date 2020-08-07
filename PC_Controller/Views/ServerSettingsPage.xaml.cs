using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Communication.Main;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PC_Controller.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServerSettingsPage : ContentPage
    {
        public ServerSettingsPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //show/hide console window
            SendData("SHOW_HIDE_WINDOW");
        }
    }
}