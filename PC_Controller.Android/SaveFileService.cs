using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(PC_Controller.Droid.SaveFileService))]
namespace PC_Controller.Droid
{
    class SaveFileService : ISaveFileInterface
    {
        public async void SaveFile(string name, byte[] bytes)
        {
            // /storage/emulated/0
            try
            {
                Directory.CreateDirectory("/storage/emulated/0/PC Controller downloads");
            }
            catch { }
            var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status == PermissionStatus.Granted)
            {
                File.WriteAllBytes("/storage/emulated/0/PC Controller downloads/" + name, bytes);
            }
            else
            {
                status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }
            if (status == PermissionStatus.Granted)
            {
                File.WriteAllBytes("/storage/emulated/0/PC Controller downloads/" + name, bytes);
            }
        }
    }
}