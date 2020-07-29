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
using System.Management;
using System.Management.Instrumentation;
using Microsoft.VisualBasic.Devices;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace Server
{
    class Program
    {
        #region variables and shit
        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSDisconnectSession(IntPtr hServer, int sessionId, bool bWait);

        const int WTS_CURRENT_SESSION = -1;
        static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
        [DllImport("powrprof.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U1)]
        static extern bool GetPwrCapabilities(out SYSTEM_POWER_CAPABILITIES systemPowerCapabilites);
        static SYSTEM_POWER_CAPABILITIES systemPowerCapabilites;
        static SerialPort serialPort;

        static PerformanceCounter cpuCounter;
        static ComputerInfo computerInfo = new ComputerInfo();
        #endregion
        static void Main(string[] args)
        {
            GetPwrCapabilities(out systemPowerCapabilites);
            try
            {
                cpuCounter = new PerformanceCounter(
            "Processor",
            "% Processor Time",
            "_Total",
            true
            );
                IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 255);
                Socket listeningSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listeningSocket.Bind(ipe);
                while (true)
                {
                    listeningSocket.Listen(int.MaxValue);
                    Console.WriteLine("Listening for connection");
                    Socket socket = listeningSocket.Accept();
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, int.MaxValue);
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, int.MaxValue);
                    Communication.Main.socket = socket;
                    Console.WriteLine("Got connection");
                    while (true)
                    {
                        string In = ReceiveData();
                        Console.WriteLine("Received \"" + In + "\"");
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
                                break;
                            case "SHUTDOWN":
                                Process.Start("shutdown", "/s /t 0");
                                break;
                            case "GET_COMPATIBILITY":
                                SendData("1");
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
                            #region GET
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
                                try
                                {
                                    SendData((SystemInformation.PowerStatus.BatteryLifePercent * 100).ToString());
                                }
                                catch (Exception exc)
                                {
                                    Console.WriteLine("Exception thrown: " + exc.Message);
                                    SendData("404");
                                }
                                break;
                            case "GET_POWERLINE_STATUS":
                                SendData(SystemInformation.PowerStatus.PowerLineStatus.ToString());
                                break;
                            case "IS_MOBILE_DEVICE":
                                if (systemPowerCapabilites.LidPresent)
                                {
                                    SendData("Yee");
                                }
                                else
                                {
                                    SendData("Noo");
                                }
                                break;
                            #endregion
                            default:
                                if (In.StartsWith("OPEN_SERIAL_PORT:"))
                                {
                                    string str = In.Replace("OPEN_SERIAL_PORT:", "");
                                    int baudRate = int.Parse(ReceiveData());
                                    serialPort = new SerialPort(str, baudRate);
                                    serialPort.Open();
                                }
                                else if (In.StartsWith("SEND_SERIAL_DATA:"))
                                {
                                    serialPort.WriteLine(In.Replace("SEND_SERIAL_DATA:", ""));
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
            Console.WriteLine("Program ended");
            Console.ReadLine();
        }
    }

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
