using System;
using System.Collections.Generic;
using System.Text;

namespace PC_Controller
{
    public interface ISaveFileInterface
    {
        void SaveFile(string name, byte[] bytes);
    }
}
