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
        public StreamReader reader;
        public StreamWriter writer;
        public string message;
        public string recieve;
        public chatClient()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            int port = CheckValidPort();
            //string serverIP = CheckServerIP();
            if (port != 0 && port > 6100 && port < 6106)
            {
                client = new TcpClient();
                try
                {
                    client.Connect(txtServerIP.Text, port);
                    Sender.RunWorkerAsync();
                    Listener.RunWorkerAsync();

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        //private string CheckServerIP()
        //{
        //    string ServerIP;
            
        //}

        private int CheckValidPort()
        {
            int port;
            if (Int32.TryParse(txtServerPort.Text, out port))
                return port;           
            else
                return 0;

        }

        private void Sender_DoWork(object sender, DoWorkEventArgs e)
        {
            string message = "";

            try
            {
                NetworkStream n = client.GetStream();

                while (!message.Equals("quit"))
                {
                    message = txtMessage.Text;
                    writer.Write(message);
                    writer.Flush();
                }

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
