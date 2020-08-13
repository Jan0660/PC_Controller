using System;
using System.Collections.Generic;
using System.Text;
using static Communication.Main;
namespace Communication
{
    /// <summary>
    /// This is a class to hold constant properties about the machine the server app is running on. 
    /// </summary>
    public static class PCInformation
    {
        public static bool IsPortableDevice;
        public static ulong InstalledRam;
        // is ran as administrator
        public static bool IsElevatedTask;
        public static int CompatibilityVersion;

        /// <summary>
        /// Update all of the information.
        /// </summary>
        public static void UpdateAll()
        {
            string str = SendAndReceive("IS_MOBILE_DEVICE");
            IsPortableDevice = Codes.FromString(str).ToBool();
            str = SendAndReceive("GET_RAM_INSTALLED");
            InstalledRam = Convert.ToUInt64(str);

            IsElevatedTask = Codes.FromString(Request("IS_ELEVATED_TASK")).ToBool();

            CompatibilityVersion = int.Parse(Request("GET_COMPATIBILITY"));
        }
    }
}
