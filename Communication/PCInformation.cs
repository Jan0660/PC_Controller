using System;
using System.Collections.Generic;
using System.Text;
using static Communication.Main;
namespace Communication
{
    public static class PCInformation
    {
        public static bool IsPortableDevice;
        public static ulong InstalledRam; 

        public static void UpdateAll()
        {
            string str = SendAndReceive("IS_MOBILE_DEVICE");
            if(str == "Yee")
            {
                IsPortableDevice = true;
            }
            else
            {
                IsPortableDevice = false;
            }
            str = SendAndReceive("GET_RAM_INSTALLED");
            InstalledRam = Convert.ToUInt64(str);
        }
    }
}
