using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(PC_Controller.UWP.SaveFileService))]
namespace PC_Controller.UWP
{
    public class SaveFileService : ISaveFileInterface
    {
        // copy paste go brrrr
        //https://docs.microsoft.com/en-us/windows/uwp/files/quickstart-save-a-file-with-a-picker
        public async void SaveFile(string name, byte[] bytes)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add(Path.GetExtension(name), new List<string>() { Path.GetExtension(name) });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = Path.GetFileNameWithoutExtension(name);
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                // write to file
                await Windows.Storage.FileIO.WriteBytesAsync(file, bytes);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    // was saved
                }
                else
                {
                    // couldnt be saved
                }
            }
            else
            {
                // Operation cancelled
            }
        }
    }
}
