using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;

namespace ServerWifiKiller
{
    class Program// This is the server side of the Wifi killer. It's listening on port 11000 for a certain set of bytes. And when it's recived the right set of bytes.
    {            // It's shutdown the Wifi
        #region HideAndShowConsole
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);//Here we are tracking down the console window and hiding it.

        [DllImport("Kernel32")]
        private static extern IntPtr GetConsoleWindow();

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        static IntPtr hwnd;
        #endregion

        private const int listenPort = 11000;// The port we chose although it could be any other number.
        private static void StartServer()
        {
            bool done = false;
            UdpClient listener = new UdpClient(listenPort);// Here we are setting up the port;
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);// We welcome any client ip address.
            string received_data;
            byte[] receive_byte_array;
            try
            {
                while (!done)
                {
                    Console.WriteLine("Waiting for broadcast");
                    receive_byte_array = listener.Receive(ref groupEP);
                     Console.WriteLine("Received a broadcast from {0}", groupEP.ToString());
                    received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                    if (received_data == "killWifi")
                    {
                        ShowWindow(hwnd, SW_SHOW);
                        Process.Start("ipconfig", "/release");
                        Console.WriteLine("Killed Wifi on {0}\n\n\n",DateTime.Now.ToString());
                        done = true;
                        Console.WriteLine("Press anywhere to turn wifi back on...");
                        ConsoleKeyInfo input = Console.ReadKey();
                        Process.Start("ipconfig", "/renew");
                        Console.WriteLine("Toggled Wifi On\n\n\n\n");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            listener.Close();
        }
        static void Main(string[] args)
        {
            while(true)
            {
                hwnd = GetConsoleWindow();
                ShowWindow(hwnd, SW_HIDE);
                StartServer();
            }
        }
    }
}
