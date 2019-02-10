using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace serverChat
{
    public static class Server
    {
        public static List<Client> Clients = new List<Client>();
        public static void NewClient(Socket handle)
        {
            try
            {
                Client newClient = new Client(handle);
                Clients.Add(newClient);
                Console.WriteLine("New client connected: {0}", handle.RemoteEndPoint);
            }
            catch (Exception exp) { Console.WriteLine("Error with addNewClient: {0}.",exp.Message); }
        }
        public static void EndClient(Client client)
        {
            try
            {
                client.End();
                Clients.Remove(client);
                Console.WriteLine("User {0} has been disconnected.", client.UserName);
                try
                {
                    int countUsers = Clients.Count;
                    for (int i = 0; i < countUsers; i++)
                    {
                        Clients[i].UpdateOnline();
                    }
                }
                catch (Exception exp) { Console.WriteLine("Error with updateOnline: {0}.", exp.Message); }
            }
            catch (Exception exp) { Console.WriteLine("Error with endClient: {0}.",exp.Message); }
        }
        public static void UpdateAllChats(string privateName)
        {
            if (string.IsNullOrWhiteSpace(privateName))
            {
                try
                {
                    int countUsers = Clients.Count;
                    for (int i = 0; i < countUsers; i++)
                    {
                        Clients[i].UpdateChat();
                    }
                }
                catch (Exception exp) { Console.WriteLine("Error with updateALLChats: {0}.", exp.Message); }
            }
            else
            {
                try
                {
                    int countUsers = Clients.Count;
                    for (int i = 0; i < countUsers; i++)
                    {
                        if (Clients[i].UserName == privateName)
                        Clients[i].UpdateChat();
                    }
                }
                catch (Exception exp) { Console.WriteLine("Error with updateALLChats: {0}.", exp.Message); }
            }
        }
        
    }
}
