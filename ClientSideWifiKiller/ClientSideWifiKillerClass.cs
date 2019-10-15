using System;
using System.Linq;
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
        int calledSeconds = 10;// The number of seconds the program is listening for your voice command.
        Choices commands = new Choices();

        private static void ShowErrorDialog(string message)
        {
            MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void sendToServer(string text_to_send)
        {// This function creates connection to the server. And send the string killWifi.
            Boolean done = false;
            Boolean exception_thrown = false;
            Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
            ProtocolType.Udp);
            IPAddress send_to_address = Dns.GetHostEntry("LAPTOP-K41IRKQN").AddressList.Where(o => o.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).First();//The ip address of the server

            IPEndPoint sending_end_point = new IPEndPoint(send_to_address, 11000);//The port in the server thats listening is 11000.
            

            while (!done)
            {
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
            commands.Add(new string[] { "kill wifi", "computer", "stop" });//Initiziling commands.
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
                case "kill wifi":
                    if(programCalled)
                    {
                        label.Text = "מכבה אינטרנט בלפטופ";// "מכבה אינטרנט בלפטופ" means in hebrew - toggling off internet on the laptop.
                        this.Opacity = 100;
                        sendToServer("killWifi");
                        messageTimer.Start();
                        programCalled = false;
                    }
                    break;
                case "computer":// "ekshev" means in hebrew - report status.
                    label.Text = "דבר אליי";// "דבר אליי" means in hebrew - listening.
                    programCalled = true;
                    calledSeconds = 10;
                    this.Opacity = 100;
                    messageTimer.Start();
                    calledTimer.Start();
                    break;
                case "stop":// "teetabad" means in hebrew - kill yourself.
                    if (programCalled)
                    {
                        label.Text = "מתאבד";// "מתאבד" means in hebrew - killing myself.
                        //messageTimer.Start();
                        this.Opacity = 100;
                        killSelfTimer.Start();
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
                messageTimer.Stop();
            }
        }

        private void calledTimer_Tick(object sender, EventArgs e)
        {
            calledSeconds--;
            if(calledSeconds <=0)
            {
                programCalled = false;
                calledTimer.Stop();
            }
        }

        private void killSelfTimer_Tick(object sender, EventArgs e)
        {
            messageSeconds--;
            if (messageSeconds <= 0)
            {
                Application.Exit();
            }
        }
    }
}
