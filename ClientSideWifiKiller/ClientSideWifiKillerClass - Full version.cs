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
using BasicAsyncServer;
using System.Threading;

namespace gaverProject
{
    public partial class miniOfekHelper : Form
    {

        private Socket clientSocket;
        private byte[] buffer;
        Choices commands = new Choices();

        private static void ShowErrorDialog(string message)
        {
            
            MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            try
            {
                int received = clientSocket.EndReceive(AR);

                if (received == 0)
                {
                    return;
                }


                string message = Encoding.ASCII.GetString(buffer);

                Invoke((Action)delegate
                {
                    Text = "פותח על ידי גאבר בעמ";
                });

                // Start receiving data again.
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            // Avoid Pokemon exception handling in cases like these.
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        private void ConnectCallback(IAsyncResult AR)
        {
            try
            {
                clientSocket.EndConnect(AR);
                buffer = new byte[clientSocket.ReceiveBufferSize];
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (SocketException ex)
            {
                //AutoClosingMessageBox.Show("מנסה שוב בעוד 10 דקות", "החיבור למחשב הנייד נכשל", 100);
                timer2.Start();
            }
            catch (ObjectDisposedException ex)
            {
                MessageBox.Show("2");
            }
        }

        private void SendCallback(IAsyncResult AR)
        {
            try
            {
                clientSocket.EndSend(AR);
            }
            catch (SocketException ex)
            {
                MessageBox.Show("3");
            }
            catch (ObjectDisposedException ex)
            {
                MessageBox.Show("4");
            }
        }

















        public void killSocial()
        {
            try
            {
                Process[] processSteam = Process.GetProcessesByName("Steam");

                foreach (Process process in processSteam)
                    process.Kill();
            }
            catch(Exception e) { MessageBox.Show(e.Message.ToString()); }


            try
            {
                Process[] processDiscord = Process.GetProcessesByName("Discord");

                foreach (Process process in processDiscord)
                    process.Kill();
            }
            catch (Exception e) { MessageBox.Show(e.Message.ToString()); }
        }
        public void killCsgo()
        {
            try
            {
                Process[] processSteam = Process.GetProcessesByName("csgo");

                foreach (Process process in processSteam)
                    process.Kill();
            }
            catch (Exception e) { MessageBox.Show(e.Message.ToString()); }
        }

        public void killWifi()
        {
            try
            {
                // Serialize the textBoxes text before sending.
                PersonPackage person = new PersonPackage(true, (ushort)1, "killWifi");
                byte[] buffer = person.ToByteArray();
                clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallback, null);
            }
            catch (SocketException ex)
            {
               // ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
              //  ShowErrorDialog(ex.Message);
            }
        }
        public void testConnectionToLaptop()
        {
            try
            {
                // Serialize the textBoxes text before sending.
                PersonPackage person = new PersonPackage(true, (ushort)1, "test");
                byte[] buffer = person.ToByteArray();
                clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallback, null);
            }
            catch (SocketException ex)
            {
                // ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                //  ShowErrorDialog(ex.Message);
            }
        }



        public void startSocial()
        {
            Process.Start(@"C:\Program Files (x86)\Steam\Steam.exe");
            Process.Start(@"C:\Users\ofeky\AppData\Local\Discord\Update.exe", "--processStart Discord.exe");
        }
        public void startCsgo()
        {
            Process.Start(@"steam://rungameid/730");
        }

        int messageSeconds = 2;
        int retryConnectionMinutes = 10;
        SpeechRecognitionEngine recEngine = new SpeechRecognitionEngine();
        public miniOfekHelper()
        {
            InitializeComponent();
        }
        private void connectToLaptop()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Connect to the specified host.
            var endPoint = new IPEndPoint(IPAddress.Parse("10.0.0.4"), 3333);
            clientSocket.BeginConnect(endPoint, ConnectCallback, null);
        }
        GrammarBuilder gBuilder;
        Grammar grammar;
        private void Form1_Load(object sender, EventArgs e)
        {

            try
            {
                connectToLaptop();
                //AutoClosingMessageBox.Show("בוצע חיבור למחשב הנייד", "", 1800);
            }
            catch (Exception)
            {
                //AutoClosingMessageBox.Show("מנסה שוב בעוד 10 דקות", "החיבור למחשב הנייד נכשל", 1800);
                timer2.Start();
                //ShowErrorDialog(ex.Message);
            }

            /* catch (ObjectDisposedException ex)
             {
                // ShowErrorDialog(ex.Message);
             }*/



            commands.Add(new string[] { "laptop off", "offline mode", "online mode", "kill self", "open game", "kill game", "ten status", "lite mode", "create connection", "extra mode" });
            gBuilder = new GrammarBuilder();
            gBuilder.Append(commands);
            grammar = new Grammar(gBuilder);

            recEngine.LoadGrammarAsync(grammar);
            recEngine.SetInputToDefaultAudioDevice();
            recEngine.SpeechRecognized += RecEngine_SpeechRecognized;
            recEngine.RecognizeAsync(RecognizeMode.Multiple);
        }
        bool extraMode = false;
        private void RecEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            switch (e.Result.Text)
            {
                case "laptop off":
                    label1.Text = "מכבה אינטרנט בלפטופ";
                    this.Opacity = 100;
                    killWifi();
                    timer1.Start();
                    break;
                case "create connection":
                    if (extraMode)
                    {
                        label1.Text = "מנסה להתחבר ללפטופ";
                        this.Opacity = 100;
                        connectToLaptop();
                        timer1.Start();
                    }
                    break;

                case "extra mode":
                    label1.Text = "מצב מלא";
                    extraMode = true;
                    this.Opacity = 100;
                    timer1.Start();
                    break;

                case "lite mode":
                    label1.Text = "מצב קל";
                    extraMode = false;
                    this.Opacity = 100;
                    timer1.Start();
                    break;

                case "offline mode":
                    if (extraMode)
                    {
                        label1.Text = "מכבה יישומים חברתיים";
                        killSocial();
                        this.Opacity = 100;
                        timer1.Start();
                    }
                    break;


                case "online mode":
                    if (extraMode)
                    {
                        label1.Text = "מדליק יישומים חברתיים";
                        startSocial();
                        this.Opacity = 100;
                        timer1.Start();
                    }
                    break;


                case "kill self":
                    label1.Text = "לא מאזין יותר";
                    this.Opacity = 100;
                    Application.Exit();
                    break;

                case "ten status":
                    label1.Text = "פועל ומאזין";
                    this.Opacity = 100;
                    this.ShowInTaskbar = true;
                    timer1.Start();
                    break;

                case "kill game":
                    if (extraMode)
                    {
                        label1.Text = "סוגר קונטר";
                        killCsgo();
                        this.Opacity = 100;
                        timer1.Start();
                    }
                    break;


                case "open game":
                    if (extraMode)
                    {
                        label1.Text = "פותח קונטר";
                        startCsgo();
                        this.Opacity = 100;
                        timer1.Start();
                    }
                    break;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            messageSeconds--;
            if (messageSeconds <= 0)
            {
                this.Opacity = 0;
                messageSeconds = 2;
                this.ShowInTaskbar = false;
                timer1.Stop();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            retryConnectionMinutes--;
            if(retryConnectionMinutes <= 0)
            {
                connectToLaptop();
                retryConnectionMinutes = 10;
            }
        }
    }
}
