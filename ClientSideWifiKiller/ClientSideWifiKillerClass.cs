using System;
using System.Net;
using System.Net.Sockets;
using System.Speech.Recognition;
using System.Text;
using System.Windows.Forms;

namespace gaverProject
{
    public partial class WifiKiller : Form
    {
        //This is the client of the wifi killer.
        //It's goal is to send a command the server in order to kill the wifi.
        int messageSeconds = 2;// The number of seconds the message will appear when a voice command executed.
        int calledSeconds = 10;
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
            commands.Add(new string[] { "caabe wifi", "akshev", "titabed" });//Initiziling commands.
            gBuilder = new GrammarBuilder();
            gBuilder.Append(commands);
            grammar = new Grammar(gBuilder);

            recEngine.LoadGrammarAsync(grammar);
            recEngine.SetInputToDefaultAudioDevice();
            recEngine.SpeechRecognized += RecEngine_SpeechRecognized;
            recEngine.RecognizeAsync(RecognizeMode.Multiple);
        }
        bool programCalled = false;
        private void RecEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            switch (e.Result.Text)
            {
                case "caabe wifi":
                    if(programCalled)
                    {
                        label1.Text = "מכבה אינטרנט בלפטופ";// "מכבה אינטרנט בלפטופ" means in hebrew - toggling off internet on the laptop.
                        this.Opacity = 100;
                        killWifi();
                        messageTimer.Start();
                        programCalled = false;
                    }
                    break;
                case "akshev":// "ekshev" means in hebrew - report status.
                    label1.Text = "דבר אליי";// "דבר אליי" means in hebrew - listening.
                    programCalled = true;
                    this.Opacity = 100;
                    this.ShowInTaskbar = true;
                    messageTimer.Start();
                    break;
                case "titabed":// "titabed" means in hebrew - kill yourself.
                    if (programCalled)
                    {
                        label1.Text = "מתאבד";// "מתאבד" means in hebrew - killing myself.
                        this.Opacity = 100;
                        Application.Exit();
                    }
                    break;

            }
        }
        private void messageTimer_Tick(object sender, EventArgs e)
        {
            messageSeconds--;
            if (messageSeconds <= 0)
            {

                this.Opacity = 0;
                messageSeconds = 2;
                this.ShowInTaskbar = false;
                messageTimer.Stop();
            }
        }

        private void calledTimer_Tick(object sender, EventArgs e)
        {
            calledSeconds--;
            if(calledSeconds <=0)
            {
                programCalled = false;
            }
        }
    }
}
