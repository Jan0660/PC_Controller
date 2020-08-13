using System;
using System.Collections.Generic;
using System.Net.Cache;
using System.Text;
using static Communication.Main;
namespace PC_Controller.Models
{
    public enum FileType { Folder, File };

    // this is used for files and folders.
    public class File
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public FileType FileType { get; set; }

        private long size = -1;
        public long Size
        {
            get
            {
                if (size == -1)
                {
                    string str = Request("GET_FILE_SIZE:" + FullName);
                    if (str.StartsWith(Communication.Codes.Error.ToString()))
                    {
                        return size;
                    }
                    size = Convert.ToInt64(str);
                }
                return size;
            }
            set
            {
                size = value;
            }
        }
    }
}
