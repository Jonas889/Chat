﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using WFClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Networking_server
{
    class Program
    {
         static List<string> usersConnected = new List<string>();
        static void Main(string[] args)
        {
            Server myServer = new Server();
            Thread serverThread = new Thread(myServer.Run);
            serverThread.Start();
            serverThread.Join();
        }

        public class Server
        {
            List<ClientHandler> clients = new List<ClientHandler>();
            public void Run()
            {
                TcpListener listener = new TcpListener(IPAddress.Any, 6101);
                Console.WriteLine("Server up and running, waiting for messages...");

                try
                {
                    listener.Start();

                    while (true)
                    {
                        TcpClient c = listener.AcceptTcpClient();
                        ClientHandler newClient = new ClientHandler(c, this);
                        clients.Add(newClient);
                        Console.WriteLine("New incomming connection from: " + c.Client.RemoteEndPoint.ToString());
                        Thread clientThread = new Thread(newClient.Run);
                        clientThread.Start();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (listener != null)
                        listener.Stop();
                }
            }

            public void Broadcast(ClientHandler client, string message)
            {
                foreach (ClientHandler tmpClient in clients)
                {
                    //StreamWriter writer = new StreamWriter(tmpClient.tcpclient.GetStream());
                    if (clients.Count > 1)
                    {
                        NetworkStream n = tmpClient.tcpclient.GetStream();
                        BinaryWriter w = new BinaryWriter(n);
                        w.Write(message);
                        w.Flush();
                    }
                    else if (clients.Count() == 1)
                    {
                        NetworkStream n = tmpClient.tcpclient.GetStream();
                        BinaryWriter w = new BinaryWriter(n);
                        w.Write(message);
                        w.Write("Sorry, you are alone...");
                        w.Flush();
                    }
                }
            }

            public void DisconnectClient(ClientHandler client)
            {
                clients.Remove(client);
                Console.WriteLine($"Client {client.userName} has left the building...");
                Broadcast(client, $"Client {client.userName} has left the building...");
            }
        }

        public class ClientHandler
        {
            public TcpClient tcpclient;
            private Server myServer;
            public string userName;
            public ClientHandler(TcpClient c, Server server)
            {
                tcpclient = c;
                this.myServer = server;
            }

            public void Run()
            {
                try
                {
                    string message = "";
                    while (tcpclient.Connected)
                    {
                        NetworkStream n = tcpclient.GetStream();
                        message = new BinaryReader(n).ReadString();
                        Message deSerializedMessage = JsonConvert.DeserializeObject<Message>(message);
                        if (deSerializedMessage.ChatMessage.Contains("#¤%start%¤#"))
                        {
                            userName = deSerializedMessage.Username;
                        }
                        foreach (var c in usersConnected)
                        {
                            deSerializedMessage.Users.Add(c);
                        }
                        myServer.Broadcast(this, message);
                        Console.WriteLine(message);
                    }

                    myServer.DisconnectClient(this);
                    tcpclient.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myServer.DisconnectClient(this);
                    tcpclient.Close();
                }
            }
        }
    }
}
