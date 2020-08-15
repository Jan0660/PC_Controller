using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PC_Controller
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        ObservableCollection<Machine> machines = new ObservableCollection<Machine>();
        public ObservableCollection<Machine> Machines => machines; // completely readable
        public MainPage()
        {
            InitializeComponent();
            OnAppearing();
        }

        protected override void OnAppearing()
        {
            listView.BindingContext = new Machine();
            listView.ItemsSource = Machines;
            LoadMachines();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            SaveMachines(machines);
            base.OnDisappearing();
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Machine machine = (Machine)e.Item;
            try
            {
                ControlPage page = new ControlPage(machine.IPString);
                Navigation.PushAsync(page);
            }
            catch (Exception exc)
            {
                if (exc.Message == "Incompatible")
                {
                    DisplayAlert("Incompatible Version", "teh versions aint match up", ControlPage.GetCancelButtonText());
                }
                else
                {
                    DisplayAlert("Exception ocurred", exc.Message, ControlPage.GetCancelButtonText());
                }
            }
        }

        private void MenuItem_Clicked(object sender, EventArgs e)
        {
            // edit
            EditMachinePage page = new EditMachinePage(ref machines, (int)(sender as MenuItem).CommandParameter);
            Navigation.PushAsync(page);
        }

        private void OnDelete(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            Machine machine = item.BindingContext as Machine;
            machines.Remove(machines[machine.ID]);
            SaveMachines(machines);
            OnAppearing();
        }

        public static void SaveMachines(ObservableCollection<Machine> machines)
        {
            string MachineNames = "";
            string IPs = "";
            for (int i = 0; i < machines.Count; i++)
            {
                MachineNames += machines[i].Name + ",";
                IPs += machines[i].IPString + ",";
            }
            MachineNames = MachineNames.Remove(MachineNames.Length - 1);
            IPs = IPs.Remove(IPs.Length - 1);
            Application.Current.Properties["MachineNames"] = MachineNames;
            Application.Current.Properties["IPs"] = IPs;
            Application.Current.SavePropertiesAsync();
        }

        void LoadMachines()
        {
            try
            {
                machines = new ObservableCollection<Machine>();
                string[] MachineNames = Application.Current.Properties["MachineNames"].ToString().Split(',');
                string[] IPs = Application.Current.Properties["IPs"].ToString().Split(',');
                for (int i = 0; i < MachineNames.Length; i++)
                {
                    machines.Add(new Machine { Name = MachineNames[i], IPString = IPs[i], ID = i });
                }
            }
            catch
            {
                // take the user to the add machine screen
                ToolbarItem_Clicked(null, EventArgs.Empty);
            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            //add machine
            Machine machine = new Machine { Name = "", IPString = "", ID = machines.Count };
            machines.Add(machine);
            EditMachinePage page = new EditMachinePage(ref machines, machine.ID);
            Navigation.PushAsync(page);
        }

        private void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            SettingsPage page = new SettingsPage();
            Navigation.PushAsync(page);
        }
    }
}
