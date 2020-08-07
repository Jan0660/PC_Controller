using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Communication.Main;
namespace PC_Controller
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SerialControlPage : ContentPage
    {
        public SerialControlPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            SendData("OPEN_SERIAL_PORT:" + portEntry.Text);
            SendData(baudRateEntry.Text);
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            SendData("SEND_SERIAL_DATA:" + dataEntry.Text);
        }
    }
}