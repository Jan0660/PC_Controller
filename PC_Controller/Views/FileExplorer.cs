using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Communication.Main;
using Xamarin.Forms;
using System.Reflection;
using System.IO;

namespace PC_Controller
{
    public class FileExplorer : ContentPage
    {
        string directory = "C:\\";
        public FileExplorer()
        {
            // add the "above directory" arrow
            ToolbarItem item = new ToolbarItem
            {
                IconImageSource = ImageSource.FromResource("PC_Controller.up_arrow.png", typeof(FileExplorer).GetTypeInfo().Assembly),
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            item.Clicked += Item_Clicked;
            this.ToolbarItems.Add(item);
            UpdateView();
            DisplayAlert("Under construction", "Please note that the file explorer is currently a Work In Progress and key features might not work.", ControlPage.GetCancelButtonText());
        }

        private void Item_Clicked(object sender, EventArgs e)
        {
            directory = Request("GET_PARENT_OF:" + directory);
            UpdateView();
        }

        void UpdateView()
        {
            // TODO: fix this shitty GUI
            ScrollView scrollView = new ScrollView();
            StackLayout stackLayout = new StackLayout();
            scrollView.Content = stackLayout;
            Title = "File Explorer";
            stackLayout.Children.Add(new Label { Text = "Showing contents of " + directory, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start });
            stackLayout.Children.Add(new Label { Text = "", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start });
            SendData("GET_ALL_FOLDERS_IN:" + directory);
            string str = ReceiveData();
            if (str.StartsWith("*EXCEPTION:"))
            {
                stackLayout.Children.Add(new Label { Text = "Exception occurred:", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start });
                stackLayout.Children.Add(new Label { Text = str.Replace("*EXCEPTION:", ""), HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start });
            }
            else
            {
                string[] dirs = str.Split('|');
                foreach (string dir in dirs)
                {
                    Button button = new Button { Text = dir, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Start };
                    button.Clicked += Button_Clicked;
                    stackLayout.Children.Add(button);
                }
            }
            SendData("GET_ALL_FILES_IN:" + directory);
            // if there are no files and directories just pussy out
            string strr = ReceiveData();
            if (strr == "FUCKING NOTHING MAN" && str == "FUCKING NOTHING MAN")
            {
                stackLayout.Children.Add(new Label { Text = "There's nothing here bro.", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start });
            }
            else if (strr.StartsWith("*EXCEPTION:"))
            {
                stackLayout.Children.Add(new Label { Text = "Exception occurred:", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start });
                stackLayout.Children.Add(new Label { Text = strr.Replace("*EXCEPTION:", ""), HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start });
            }
            else
            {
                string[] dirs = strr.Split('|');
                foreach (string dir in dirs)
                {
                    Button button = new Button { Text = dir, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Start };
                    button.Clicked += Button_Clicked1;
                    stackLayout.Children.Add(button);
                }
            }
            Content = scrollView;
        }

        private void Button_Clicked1(object sender, EventArgs e)
        {
            // file clicked
            Button button = sender as Button;
            FileView fileView = new FileView(button.Text, directory + "\\" + button.Text);
            Navigation.PushAsync(fileView);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //directory clicked
            // figure out if the current directory is "C:\" or other
            if(directory.Length != 3)
            {
                directory += "\\" + (sender as Button).Text;
            }
            else
            {
                directory += (sender as Button).Text;
            }
            UpdateView();
        }
    }
}