using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace gaverProject
{
    public partial class WifiKiller : Form
    {
        //This is the client of the wifi killer.
        //It's goal is to send a command the server in order to kill the wifi.
        int messageSeconds = 2;// The number of seconds the message will appear when a voice command executed.
        Choices commands = new Choices();

        private static void ShowErrorDialog(string message)
        {
            MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void killWifi()
        {// This function creates connection to the server. And send the string killWifi.
            Boolean done = false;
            Boolean exception_thrown = false;
            Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
            ProtocolType.Udp);

            IPAddress send_to_address = IPAddress.Parse("10.0.0.4");//The ip address of the server

            IPEndPoint sending_end_point = new IPEndPoint(send_to_address, 11000);//The port in the server thats listening is 11000.
            

            while (!done)
            {
                string text_to_send = "killWifi";//The server is waiting for this word to kill the WIFI service.
                if (text_to_send.Length == 0)
                {
                    done = true;
                }
                else
                {

                    byte[] send_buffer = Encoding.ASCII.GetBytes(text_to_send);

                    try
                    {
                        sending_socket.SendTo(send_buffer, sending_end_point);
                    }
                    catch (Exception send_exception)
                    {
                        exception_thrown = true;
                        ShowErrorDialog(send_exception.Message);
                    }
                    if (exception_thrown != false)
                    {
                        exception_thrown = false;
                        ShowErrorDialog("The exception indicates the message was not sent.");
                    }
                    else
                    {
                        done = true;
                    }
                }
            }
        }


        
        SpeechRecognitionEngine recEngine = new SpeechRecognitionEngine();
        public WifiKiller()
        {
            InitializeComponent();
        }
        GrammarBuilder gBuilder;
        Grammar grammar;
        private void gaver_Load(object sender, EventArgs e)
        {
            commands.Add(new string[] { "laptop off","ten status" });//Initiziling commands.
            gBuilder = new GrammarBuilder();
            gBuilder.Append(commands);
            grammar = new Grammar(gBuilder);

            recEngine.LoadGrammarAsync(grammar);
            recEngine.SetInputToDefaultAudioDevice();
            recEngine.SpeechRecognized += RecEngine_SpeechRecognized;
            recEngine.RecognizeAsync(RecognizeMode.Multiple);
        }
        private void RecEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            switch (e.Result.Text)
            {
                case "laptop off":
                    label1.Text = "מכבה אינטרנט בלפטופ";// "מכבה אינטרנט בלפטופ" means in hebrew - toggling off internet on the laptop.
                    this.Opacity = 100;
                    killWifi();
                    timer.Start();
                    break;
                case "ten status":// "תן סטאטוס" means in hebrew - report status.
                    label1.Text = "פועל ומאזין";// "פועל ומאזין" means in hebrew - on and listening.
                    this.Opacity = 100;
                    this.ShowInTaskbar = true;
                    timer.Start();
                    break;

            }
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            messageSeconds--;
            if (messageSeconds <= 0)
            {
                this.Opacity = 0;
                messageSeconds = 2;
                this.ShowInTaskbar = false;
                timer.Stop();
            }
        }
    }
}
