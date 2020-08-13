using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using static Communication.Main;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using Microsoft.VisualBasic.Devices;
using System.Windows.Forms;
using System.Security.Principal;
using Communication;
using System.IO;
using System.Threading;

namespace Server
{
    class Program
    {
        #region variables and shit
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        static bool IsWindowHidden = false;
        const int CompatibilityVersion = 1;
        // Log off
        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSDisconnectSession(IntPtr hServer, int sessionId, bool bWait);

        const int WTS_CURRENT_SESSION = -1;
        static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
        [DllImport("powrprof.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U1)]
        static extern bool GetPwrCapabilities(out SYSTEM_POWER_CAPABILITIES systemPowerCapabilites);
        static SYSTEM_POWER_CAPABILITIES systemPowerCapabilites;

        public static bool IsAdministrator =>
   new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        static SerialPort serialPort;

        static PerformanceCounter cpuCounter;
        static ComputerInfo computerInfo = new ComputerInfo();
        #endregion
        static void Main(string[] args)
        {
            GetPwrCapabilities(out systemPowerCapabilites);
            try
            {
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
                IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 255);
                Socket listeningSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listeningSocket.Bind(ipe);
                while (true)
                {
                    listeningSocket.Listen(int.MaxValue);
                    Console.WriteLine("Listening for connection");
                    Socket socket = listeningSocket.Accept();
                    // try to prevent weird shit
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, int.MaxValue);
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, int.MaxValue);
                    Communication.Main.socket = socket;
                    Console.WriteLine("Got connection");
                    while (true)
                    {
                        string In = ReceiveData();
                        Console.WriteLine("Received \"" + In + "\"");
                        // this is what's known as "readable code"
                        switch (In)
                        {
                            case "TEST":
                                SendData("Test work'd");
                                break;
                            case "DISCONNECT":
                                socket.Disconnect(false);
                                goto disconnected;
                            case "LOGOFF":
                                if (!WTSDisconnectSession(WTS_CURRENT_SERVER_HANDLE, WTS_CURRENT_SESSION, false))
                                    throw new Win32Exception();
                                SendData(Codes.Success.ToString());
                                break;
                            case "SHUTDOWN":
                                Process.Start("shutdown", "/s /t 0");
                                SendData(Codes.Success.ToString());
                                break;
                            case "SHOW_HIDE_WINDOW":
                                IntPtr handle = GetConsoleWindow();
                                if (IsWindowHidden)
                                    ShowWindow(handle, SW_SHOW);
                                else
                                    ShowWindow(handle, SW_HIDE);
                                IsWindowHidden = !IsWindowHidden;
                                break;
                            #region GET
                            case "IS_ELEVATED_TASK":
                                SendBool(IsAdministrator);
                                break;
                            case "GET_COMPATIBILITY":
                                SendData(CompatibilityVersion.ToString());
                                break;
                            case "GET_HOSTNAME":
                                SendData(Dns.GetHostName());
                                break;
                            case "GET_LOCAL_IP":
                                foreach (IPAddress ip in Dns.GetHostAddresses(Dns.GetHostName()))
                                {
                                    if (ip.MapToIPv4().ToString().StartsWith("10.0.0.") | ip.MapToIPv4().ToString().StartsWith("192.168.1."))
                                    {
                                        SendData(ip.ToString());
                                        goto IPSent;
                                    }
                                }
                                SendData("ERROR");
                                IPSent:;
                                break;
                            case "GET_RAM_FREE":
                                SendData(computerInfo.AvailablePhysicalMemory.ToString());
                                break;
                            case "GET_CPU_USAGE":
                                SendData(cpuCounter.NextValue().ToString());
                                break;
                            case "GET_RAM_INSTALLED":
                                SendData(computerInfo.TotalPhysicalMemory.ToString());
                                break;
                            case "GET_BATTERY_PERCENTAGE":
                                SendData((SystemInformation.PowerStatus.BatteryLifePercent * 100).ToString());
                                break;
                            case "GET_POWERLINE_STATUS":
                                SendData(SystemInformation.PowerStatus.PowerLineStatus.ToString());
                                break;
                            case "IS_MOBILE_DEVICE":
                                // determine if device is a notebook or desktop if the lid is present
                                // needed because SystemInformation.PowerStatus.BatteryLifePercent hangs if the computer is a desktop
                                SendBool(systemPowerCapabilites.LidPresent);
                                break;
                            #endregion
                            default:
                                if (In.StartsWith("OPEN_SERIAL_PORT:"))
                                {
                                    string stre = In.Replace("OPEN_SERIAL_PORT:", "");
                                    int baudRate = int.Parse(ReceiveData());
                                    serialPort = new SerialPort(stre, baudRate);
                                    serialPort.Open();
                                }
                                else if (In.StartsWith("SEND_SERIAL_DATA:"))
                                {
                                    serialPort.WriteLine(In.Replace("SEND_SERIAL_DATA:", ""));
                                }
                                // I have to do this shit because Directory.GetParent on android adds a slash on th start of the path and does shit
                                else if (In.StartsWith("GET_PARENT_OF:"))
                                {
                                    try
                                    {
                                        SendData(Directory.GetParent(In.Replace("GET_PARENT_OF:", "")).FullName);
                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine("Exception occured: " + exc.Message);
                                        SendData(In.Replace("GET_PARENT_OF:", ""));
                                    }
                                }
                                else if (In.StartsWith("GET_ALL_FOLDERS_IN:"))
                                {
                                    try
                                    {
                                        string[] dirs = Directory.GetDirectories(In.Replace("GET_ALL_FOLDERS_IN:", ""));
                                        string str = "";
                                        for (int i = 0; i < dirs.Length; i++)
                                        {
                                            dirs[i] = new DirectoryInfo(dirs[i]).Name;
                                            str += dirs[i] + "|";
                                        }
                                        if (str != "")
                                        {
                                            str = str.Remove(str.Length - 1);
                                            SendData(str);
                                        }
                                        else
                                        {
                                            SendData("FUCKING NOTHING MAN");
                                        }
                                    }
                                    catch (Exception exc)
                                    {
                                        if (exc.Message.EndsWith("denied."))
                                        {
                                            //access denied
                                            SendData("*EXCEPTION:" + exc.Message);
                                        }
                                    }
                                }
                                else if (In.StartsWith("GET_ALL_FILES_IN:"))
                                {
                                    try
                                    {
                                        string[] files = Directory.GetFiles(In.Replace("GET_ALL_FILES_IN:", ""));
                                        string str = "";
                                        for (int i = 0; i < files.Length; i++)
                                        {
                                            files[i] = new DirectoryInfo(files[i]).Name;
                                            str += files[i] + "|";
                                        }
                                        if (str != "")
                                        {
                                            str = str.Remove(str.Length - 1);
                                            SendData(str);
                                        }
                                        else
                                        {
                                            SendData("FUCKING NOTHING MAN");
                                        }
                                    }
                                    catch (Exception exc)
                                    {
                                        SendData("*EXCEPTION:" + exc.Message);
                                    }
                                }
                                else if (In.StartsWith("GET_FILE:"))
                                {
                                    socket.SendFile(In.Replace("GET_FILE:", ""));
                                }
                                else if (In.StartsWith("START_PROCESS:"))
                                {
                                    try
                                    {
                                        string process = In.Replace("START_PROCESS:", "");
                                        string arguments = ReceiveData().Replace("ARGS:", "");
                                        Process.Start(process, arguments);
                                        SendData(Codes.Success.ToString());
                                    }
                                    catch (Exception exc)
                                    {
                                        SendData(Codes.Error.ToString() + exc.Message);
                                    }
                                }
                                else if (In.StartsWith("CREATE_FOLDER:"))
                                {
                                    try
                                    {
                                        Directory.CreateDirectory(In.Replace("CREATE_FOLDER:", ""));
                                        SendData(Codes.Success.ToString());
                                    }
                                    catch (Exception exc)
                                    {
                                        SendData(Codes.Error.ToString() + exc.Message);
                                    }
                                }
                                else if (In.StartsWith("MOVE_FILE:"))
                                {
                                    try
                                    {
                                        string str = In.Remove(0, "MOVE_FILE:".Length);
                                        File.Move(str.Split('|')[0], str.Split('|')[1]);
                                        SendData(Codes.Success.ToString());
                                    }
                                    catch(Exception exc)
                                    {
                                        SendData(Codes.Error.ToString() + exc.Message);
                                    }
                                }
                                else if (In.StartsWith("MOVE_FOLDER:"))
                                {
                                    try
                                    {
                                        string str = In.Remove(0, "MOVE_FOLDER:".Length);
                                        Directory.Move(str.Split('|')[0], str.Split('|')[1]);
                                        SendData(Codes.Success.ToString());
                                    }
                                    catch (Exception exc)
                                    {
                                        SendData(Codes.Error.ToString() + exc.Message);
                                    }
                                }
                                else if (In.StartsWith("DELETE_FILE:"))
                                {
                                    try
                                    {
                                        string str = In.Remove(0, "DELETE_FILE:".Length);
                                        File.Delete(str);
                                        SendData(Codes.Success.ToString());
                                    }
                                    catch(Exception exc)
                                    {
                                        SendData(Codes.Error.ToString() + exc.Message);
                                    }
                                }
                                else if (In.StartsWith("DELETE_FOLDER:"))
                                {
                                    try
                                    {
                                        string str = In.Remove(0, "DELETE_FOLDER:".Length);
                                        Directory.Delete(str, true);
                                        SendData(Codes.Success.ToString());
                                    }
                                    catch (Exception exc)
                                    {
                                        SendData(Codes.Error.ToString() + exc.Message);
                                    }
                                }
                                else if (In.StartsWith("GET_FILE_SIZE:"))
                                {
                                    try
                                    {
                                        string str = In.Remove(0, "GET_FILE_SIZE:".Length);
                                        SendData(new FileInfo(str).Length.ToString());
                                    }
                                    catch (Exception exc)
                                    {
                                        SendData(Codes.Error.ToString() + exc.Message);
                                    }
                                }
                                else if (In.StartsWith("FILE_EXISTS:"))
                                {
                                    string str = In.Remove(0, "FILE_EXISTS:".Length);
                                    SendData(Codes.FromBool(File.Exists(str)).ToString());
                                }
                                else if (In.StartsWith("UPLOAD_FILE_TO:"))
                                {
                                    try
                                    {
                                        string str = In.Remove(0, "UPLOAD_FILE_TO:".Length);
                                        SendData(Codes.True.ToString());
                                        byte[] fileBytes = ReceiveAvailableBytes(1);
                                        File.WriteAllBytes(str, fileBytes);
                                        SendData(Codes.Success.ToString());
                                        Console.WriteLine("Received " + fileBytes.Length + " bytes");
                                    }
                                    catch(Exception exc)
                                    {
                                        SendData(Codes.Error.ToString() + exc.Message);
                                    }
                                }
                                else
                                {
                                    SendData(Codes.Error.ToString());
                                }
                                break;
                        }
                    }
                    disconnected:;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception thrown: " + exc.Message);
            }
            Console.WriteLine("Program ended, press any key to exit.");
            Console.ReadLine();
        }
    }

    //stolen from pinvoke.net
    public struct SYSTEM_POWER_CAPABILITIES
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool PowerButtonPresent;
        [MarshalAs(UnmanagedType.U1)]
        public bool SleepButtonPresent;
        [MarshalAs(UnmanagedType.U1)]
        public bool LidPresent;
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemS1;
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemS2;
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemS3;
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemS4;
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemS5;
        [MarshalAs(UnmanagedType.U1)]
        public bool HiberFilePresent;
        [MarshalAs(UnmanagedType.U1)]
        public bool FullWake;
        [MarshalAs(UnmanagedType.U1)]
        public bool VideoDimPresent;
        [MarshalAs(UnmanagedType.U1)]
        public bool ApmPresent;
        [MarshalAs(UnmanagedType.U1)]
        public bool UpsPresent;
        [MarshalAs(UnmanagedType.U1)]
        public bool ThermalControl;
        [MarshalAs(UnmanagedType.U1)]
        public bool ProcessorThrottle;
        public byte ProcessorMinThrottle;
        public byte ProcessorMaxThrottle;    // Also known as ProcessorThrottleScale before Windows XP
        [MarshalAs(UnmanagedType.U1)]
        public bool FastSystemS4;   // Ignore if earlier than Windows XP
        [MarshalAs(UnmanagedType.U1)]
        public bool Hiberboot;  // Ignore if earlier than Windows XP
        [MarshalAs(UnmanagedType.U1)]
        public bool WakeAlarmPresent;   // Ignore if earlier than Windows XP
        [MarshalAs(UnmanagedType.U1)]
        public bool AoAc;   // Ignore if earlier than Windows XP
        [MarshalAs(UnmanagedType.U1)]
        public bool DiskSpinDown;
        public byte HiberFileType;  // Ignore if earlier than Windows 10 (10.0.10240.0)
        [MarshalAs(UnmanagedType.U1)]
        public bool AoAcConnectivitySupported;  // Ignore if earlier than Windows 10 (10.0.10240.0)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        private readonly byte[] spare3;
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemBatteriesPresent;
        [MarshalAs(UnmanagedType.U1)]
        public bool BatteriesAreShortTerm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public BATTERY_REPORTING_SCALE[] BatteryScale;
        public SYSTEM_POWER_STATE AcOnLineWake;
        public SYSTEM_POWER_STATE SoftLidWake;
        public SYSTEM_POWER_STATE RtcWake;
        public SYSTEM_POWER_STATE MinDeviceWakeState;
        public SYSTEM_POWER_STATE DefaultLowLatencyWake;
    }

    public struct BATTERY_REPORTING_SCALE
    {
        public uint Granularity;
        public uint Capacity;
    }

    public enum SYSTEM_POWER_STATE
    {
        PowerSystemUnspecified = 0,
        PowerSystemWorking = 1,
        PowerSystemSleeping1 = 2,
        PowerSystemSleeping2 = 3,
        PowerSystemSleeping3 = 4,
        PowerSystemHibernate = 5,
        PowerSystemShutdown = 6,
        PowerSystemMaximum = 7
    }
}
