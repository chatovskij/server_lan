using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace server1
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerObject server; // server object

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                // get user name
                string message = GetMessage();
                userName = message;

                if (!(String.IsNullOrEmpty(userName)))
                {
                    message = userName + " joined the chat";
                    // send message about new user connection  for existing users
                    server.BroadcastMessage(message, this.Id);
                    Console.WriteLine(message);
                }
                // get message from users
                while ( !(String.IsNullOrEmpty(userName)) )
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("{0}: {1}", userName, message);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                    }
                    catch
                    {
                        message = String.Format("{0}: покинул чат", userName);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // in case exiting from cycle - close the connection
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        // reading the income message and transforming into string
        private string GetMessage()
        {
            byte[] data = new byte[64]; // buffer for receiving the data
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        // closing the connection
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
