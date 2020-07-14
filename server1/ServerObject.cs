using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace server1
{
    public class ServerObject
    {
        static TcpListener tcpListener; // server for listening
        List<ClientObject> clients = new List<ClientObject>(); // all connections

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            // get closing connections with help of id
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            // and delete it from the connection list
            if (client != null)
                clients.Remove(client);
        }
        // listening of incoming messages
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Server is started. Await connections...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // send messages of existing users
        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id) // is client id != id of sender
                {
                    clients[i].Stream.Write(data, 0, data.Length); //tx data
                }
            }
        }
        // disconnect all users
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //server stop 

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //client disconnect
            }
            Environment.Exit(0); //end process
        }
    }
}
