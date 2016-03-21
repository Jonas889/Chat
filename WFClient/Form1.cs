using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace WFClient
{
    public partial class chatClient : Form
    {
        private TcpClient client;
        //public StreamReader reader;
        //public StreamWriter writer;
        public Message message;
        public string recieve;
        public int port;
        public chatClient()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            port = CheckValidPort();
            //string serverIP = CheckServerIP();
            if (port != 0 && port > 6100 && port < 6106)
            {
                client = new TcpClient();
                try
                {
                    client.Connect(txtServerIP.Text, port);
                    Listener.RunWorkerAsync();

                    lblConnectionStatus.Text = "Connected";
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }

            }
        }

        //private string CheckServerIP()
        //{
        //    string ServerIP;

        //}

        private int CheckValidPort()
        {
            int testPort;
            if (Int32.TryParse(txtServerPort.Text, out testPort))
                return testPort;
            else
                return 0;

        }



        private void btnSend_Click(object sender, EventArgs e)
        {
            //message = txtUserName.Text + ": " + txtMessage.Text;
            Message sendMessage = new Message(txtUserName.Text, txtMessage.Text);
            XmlSerializer x = new XmlSerializer(sendMessage.GetType());
            try
            {
                string nwMessage = JsonConvert.SerializeObject(sendMessage);
                
                txtMessage.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Listener_DoWork(object sender, DoWorkEventArgs e)
        {
            NetworkStream n = client.GetStream();           
            while (client.Connected)
            {
                try
                {
                recieve = new BinaryReader(n).ReadString();
                }
                catch (Exception)
                {
                    lblConnectionStatus.Text = "Disconnected";
                    MessageBox.Show("Lost connection to server.");
                }
                message = JsonConvert.DeserializeObject<Message>(recieve);
                //txtChat.AppendText(message.Time + Environment.NewLine + message.Username + ": " +  message.ChatMessage + Environment.NewLine);
                txtChat.Invoke(new MethodInvoker(delegate { txtChat.Text = message.Time + Environment.NewLine + message.Username + ": " + message.ChatMessage + Environment.NewLine; }));
                recieve = "";

            }
            lblConnectionStatus.Text = "Disconnected";
        }
        #region supportingEvents
        private void txtMessage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend_Click(sender, e);
            }

        }
        #endregion

        private void Listener_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void SendMessage(string nwMessage)
        {
            NetworkStream n = client.GetStream();
            BinaryWriter writer = new BinaryWriter(n);
            writer.Write(nwMessage);
            writer.Flush();
        }
    }
}
