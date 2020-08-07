using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xamarin.Forms;
using Plugin;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System.IO;
using static Communication.Main;
namespace PC_Controller
{
    public class FileView : ContentPage
    {
        string name;
        string path;
        public FileView(string fileName, string filePath)
        {
            name = fileName;
            path = filePath;
            ToolbarItem item = new ToolbarItem
            {
                Text = "Download",
                //IconImageSource = ImageSource.FromResource("PC_Controller.Download_icon.png", typeof(FileExplorer).GetTypeInfo().Assembly),
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            item.Clicked += Item_Clicked;
            this.ToolbarItems.Add(item);
            ScrollView scroll = new ScrollView();
            StackLayout stack = new StackLayout();
            scroll.Content = stack;

            stack.Children.Add(new Label { Text = filePath, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start });

            Content = scroll;
        }

        private void Item_Clicked(object sender, EventArgs e)
        {
            SendData("GET_FILE:" + path);
            byte[] bytes = ReceiveBytes(1);
            //string pathToSave = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), name);
            string pathToSave = "/storage/emulated/0/" + name;
            File.WriteAllBytes(pathToSave, bytes);
        }
    }
}