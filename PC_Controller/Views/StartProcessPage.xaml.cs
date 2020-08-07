using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Communication.Main;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Communication;

namespace PC_Controller.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartProcessPage : ContentPage
    {
        public StartProcessPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            SendData("START_PROCESS:" + ProcessEntry.Text);
            SendData("ARGS:" + ArgumentsEntry.Text);
            string str = ReceiveData();
            if (str == Codes.Success.ToString())
            {

            }
            else
            {
                DisplayAlert("Exception occured", str.Replace(Codes.Error.ToString(), ""), ControlPage.GetCancelButtonText());
            }
        }
    }
}