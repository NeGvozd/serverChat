using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
namespace serverChat
{
    public class Client
    {
        private string _userName;
        private Socket _handler;
        private Thread _userThread;
        public Client(Socket socket)
        {
            _handler = socket;
            _userThread = new Thread(listner);
            _userThread.IsBackground = true;
            _userThread.Start();
        }
        public string UserName
        {
            get { return _userName; }
        }
        private void listner()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int bytesRec = _handler.Receive(buffer);
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);
                    handleCommand(data);
                }
                catch { Server.EndClient(this); return; }
            }
        }
        public void End()
        {
            try
            {
                _handler.Close();
                try
                {
                    _userThread.Abort();
                }
                catch { } // г
            }
            catch (Exception exp) { Console.WriteLine("Error with end: {0}.",exp.Message); }
        }
        private void handleCommand(string data)
        {//в зависимости от тега в сообщении выполняем действия
            if (data.Contains("#setname"))
            {
                _userName = data.Split('&')[1];
                //UpdateChat();
                UpdateOnline();
                return;
            }
            if (data.Contains("#newmsg"))
            {
                string message = data.Split('&')[1];
                ChatController.AddMessage(_userName,message, false);
                return;
            }
            if (data.Contains("#personally"))
            {
                string message = data.Split('@')[1];
                ChatController.AddMessage(_userName, message, true);
                return;
            }
        }
        public void UpdateChat()
        {
            Send(ChatController.GetChat());
        }
        public void UpdateOnline()
        {
            List<string> vs = new List<string>();
            foreach(Client client in Server.Clients)
            {
                vs.Add(client.UserName);
            }
            var str = "#updateonline&" + string.Join("|", vs);
            for (int i = 0; i < Server.Clients.Count; i++)
            {
               Server.Clients[i].Send(str);
            }
        }

        public void Send(string command)
        {
            try
            {
                int bytesSent = _handler.Send(Encoding.UTF8.GetBytes(command));
                if (bytesSent > 0) Console.WriteLine("Success");
            }
            catch (Exception exp) { Console.WriteLine("Error with send command: {0}.", exp.Message); Server.EndClient(this); }
        }
    }
}
