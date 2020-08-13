using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PC_Controller.Models
{
    public class FilesList : ObservableCollection<File>
    {
        public string Heading { get; set; }
        public ObservableCollection<File> Files => this;
    }
}
