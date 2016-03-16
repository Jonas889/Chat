using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFClient
{
    public partial class chatClient : Form
    {
        private TcpClient client;
        //public StreamReader reader;
        //public StreamWriter writer;
        public string message;
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
            message = txtMessage.Text;
            try
            {
                NetworkStream n = client.GetStream();
                BinaryWriter writer = new BinaryWriter(n);
                writer.Write(message);
                //writer = new StreamWriter(client.GetStream());
                //writer.Write(message);
                writer.Flush();
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
                recieve = new BinaryReader(n).ReadString();
                //recieve = reader.ReadLine();
                txtChat.AppendText(txtUserName.Text + ": " + recieve + Environment.NewLine);
                recieve = "";

            }
        }
    }
}
