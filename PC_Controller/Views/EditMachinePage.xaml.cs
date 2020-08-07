using Communication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PC_Controller
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditMachinePage : ContentPage
    {
        ObservableCollection<Machine> machines;
        int ID;
        public EditMachinePage(ref ObservableCollection<Machine> machines, int ID)
        {
            this.machines = machines;
            this.ID = ID;
            InitializeComponent();
            NameEntry.Text = machines[ID].Name;
            IPEntry.Text = machines[ID].IPString;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (NameEntry.Text.Contains(",") | IPEntry.Text.Contains(","))
            {
                DisplayAlert("Don't.", "Sorry but the machine name can't contain \",\".", ControlPage.GetCancelButtonText());
            }
            machines[ID].Name = NameEntry.Text;
            machines[ID].IPString = IPEntry.Text;
            MainPage.SaveMachines(machines);
            Navigation.PopAsync();
        }
    }
}