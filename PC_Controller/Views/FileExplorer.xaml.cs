using PC_Controller.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PC_Controller.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Communication.Main;
using Communication;
using System.Xml;
using Plugin.FilePicker.Abstractions;
using Plugin.FilePicker;
using System.IO;
using File = PC_Controller.Models.File;
using Xamarin.Essentials;

namespace PC_Controller.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FileExplorer : ContentPage
    {
        FilesList files = new FilesList();
        FilesList folders = new FilesList();
        ObservableCollection<FilesList> filesList = new ObservableCollection<FilesList>();
        ObservableCollection<FilesList> FilesList => filesList;
        string directory = "C:\\";
        public FileExplorer()
        {
            InitializeComponent();
            Update();
        }

        void Update()
        {
            label.Text = "Showing contents of " + directory;
            LoadFolders();
            listView.BindingContext = new File();
            listView.ItemsSource = FilesList;
        }

        void LoadFolders()
        {
            folders = new FilesList() { Heading = "Folders" };
            SendData("GET_ALL_FOLDERS_IN:" + directory);
            string str = ReceiveData();
            string[] dirs = str.Split('|');
            foreach (string dir in dirs)
            {
                folders.Add(new File() { Name = dir, FullName = GetFullName(directory, dir), FileType = FileType.Folder });
            }
            files = new FilesList() { Heading = "Files" };
            SendData("GET_ALL_FILES_IN:" + directory);
            str = ReceiveData();
            dirs = str.Split('|');
            foreach (string dir in dirs)
            {
                files.Add(new File() { Name = dir, FullName = GetFullName(directory, dir), FileType = FileType.File });
            }
            filesList = new ObservableCollection<FilesList>();
            filesList.Add(folders);
            filesList.Add(files);
        }

        string GetFullName(string path, string name)
        {
            if (path.EndsWith("\\"))
            {
                return path + name;
            }
            else
            {
                return path + "\\" + name;
            }
        }

        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            //delete
            File file = (sender as MenuItem).BindingContext as File;
            if (await DisplayAlert("U sure 'bout that?", "Do you really want to delete this " + file.FileType.ToString() + "?", "Yes", "No") == false)
                return;
            string response;
            if (file.FileType == FileType.File)
            {
                response = SendAndReceive("DELETE_FILE:" + file.FullName);
            }
            else
            {
                response = SendAndReceive("DELETE_FOLDER:" + file.FullName);
            }
            if (response != Codes.Success.ToString())
            {
                await DisplayAlert("Exception ocurred", response.Remove(0, Codes.Error.ToString().Length), ControlPage.GetCancelButtonText());
            }
            Update();
        }

        private async void MenuItem_Clicked_1(object sender, EventArgs e)
        {
            //rename
            File file = (sender as MenuItem).BindingContext as File;
            string newName = await DisplayPromptAsync("Rename " + file.FileType.ToString(), "New name");
            string response;
            if (file.FileType == FileType.File)
            {
                response = SendAndReceive("MOVE_FILE:" + file.FullName + "|" + GetFullName(directory, newName));
            }
            else
            {
                response = SendAndReceive("MOVE_FOLDER:" + file.FullName + "|" + GetFullName(directory, newName));
            }
            if (response != Codes.Success.ToString())
            {
                await DisplayAlert("Exception ocurred", response.Remove(0, Codes.Error.ToString().Length), ControlPage.GetCancelButtonText());
            }
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            File file = e.Item as File;
            if (file.FileType == FileType.Folder)
            {
                directory = file.FullName;
                Update();
            }
            else
            {
                FilePreviewPage page = new FilePreviewPage(file);
                Navigation.PushAsync(page);
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Add", "Cancel", null, "Create folder", "Upload file");
            switch (action)
            {
                case "Create folder":
                    string name = await DisplayPromptAsync("Create folder", "Folder name:");
                    string response = Request("CREATE_FOLDER:" + GetFullName(directory, name));
                    if (response != Codes.Success.ToString())
                    {
                        await DisplayAlert("Exception ocurred", response.Remove(Codes.Error.ToString().Length), ControlPage.GetCancelButtonText());
                    }
                    Update();
                    break;
                case "Upload file":
                    try
                    {
                        App.DontDisconnect = true;
                        FileData fileData = await CrossFilePicker.Current.PickFile();
                        App.DontDisconnect = false;
                        if (fileData == null)
                            return; // user canceled file picking
                        if (Codes.FromString(Request("FILE_EXISTS:" + GetFullName(directory, fileData.FileName))).ToBool())
                        {
                            if (!await DisplayAlert("File already exists", "A file with the same name already exists in this folder, do you wish to overwrite it?", "Overwrite", "Cancel"))
                            {
                                return;
                            }
                        }
                        SendData("UPLOAD_FILE_TO:" + GetFullName(directory, fileData.FileName));
                        if(ReceiveData() != Codes.True.ToString())
                        {
                            return;
                        }
                        socket.Send(fileData.DataArray);
                        string str = ReceiveData();
                        if (str != Codes.Success.ToString())
                        {
                            await DisplayAlert("Exception ocurred", str.Remove(0, "Error".Length), "ok");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Exception choosing file: " + ex.ToString());
                    }
                    break;
                default:
                    //await DisplayAlert("Feature not implemented", "Sorry, but this feature is not yet implemented.", ControlPage.GetCancelButtonText());
                    break;
            }
            Update();
        }

        private void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            //refresh
            Update();
        }

        private async void MenuItem_Clicked_2(object sender, EventArgs e)
        {
            //Download
            File file = (sender as MenuItem).BindingContext as File;
            if (file.FileType == FileType.File)
            {
                SendData("GET_FILE:" + file.FullName);
                byte[] bytes = ReceiveAvailableBytes(1);
                //System.IO.File.WriteAllBytes(localPath, bytes);
                ISaveFileInterface service = DependencyService.Get<ISaveFileInterface>();
                service.SaveFile(file.Name, bytes);
            }
            else
            {
                DisplayAlert("sorry not sorry", "Sorry but you can't download entire folders yet.", "ree");
            }
        }
    }
}