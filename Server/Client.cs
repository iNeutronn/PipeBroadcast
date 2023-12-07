using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Client : IDisposable
    {
        public NamedPipeServerStream _pipeServer { get; }
        private ClientManager clientsmanager;
        private Guid _id;
        private Task _clientCommands;
        private bool _isSubscribedToWeather;
        private bool _isSubscribedToShares;
        private bool _isSubscribedToCurrency;

        public bool IsSubscribedToWeather { get { return _isSubscribedToWeather; } }
        public bool IsSubscribedToShares  { get { return _isSubscribedToShares; } }
        public bool IsSubscribedToCurrency { get { return _isSubscribedToCurrency; } }


        public event EventHandler<string> ClientCommandReceived;

        public Client(Guid id, ClientManager manager)
        {
            _isSubscribedToWeather = false;
            _isSubscribedToShares = false;
            _isSubscribedToCurrency = false;
            clientsmanager = manager;
            _id = id;
            _pipeServer = new NamedPipeServerStream("pipe" + _id.ToString(), PipeDirection.InOut, 10, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
        }

        public void ListenClient() 
        {
            _clientCommands = Task.Run(async () =>
            {
                _pipeServer.WaitForConnection();
                if (_pipeServer == null)
                    throw new InvalidOperationException("NamedPipeServerStream not initialized.");

                byte[] buffer = new byte[256]; 

                while (true)
                {
                    int bytesRead = await _pipeServer.ReadAsync(buffer, 0, buffer.Length);

                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (!string.IsNullOrEmpty(receivedData))
                    {
                        CommandProcessing(receivedData);

                        OnClientCommandReceived(receivedData);
                    }    
                }
            });
        }

        private void OnClientCommandReceived(string receivedData)
        {
            ClientCommandReceived?.Invoke(this, receivedData);
        }

        private void CommandProcessing(string command)
        {
            switch (command)
            {
                case "quit":
                    SendAnswer("OK");
                    //clientsmanager.RemoveClient(this);
                    Dispose();     // Dispose не викличеться 2 рази патерн Dispose правильно написаний? 
                    break;
                case "SubscribToShares":
                    _isSubscribedToShares = true;
                    SendAnswer("OK");
                    break;
                case "SubscribToWeather":
                    _isSubscribedToWeather = true;
                    SendAnswer("OK");
                    break;
                case "SubscribToCurrency":
                    _isSubscribedToCurrency = true;
                    SendAnswer("OK");
                    break;
                case "UnSubscribToShares":
                    _isSubscribedToShares = false;
                    SendAnswer("OK");
                    break;
                case "UnSubscribToWeather":
                    _isSubscribedToWeather = false;
                    SendAnswer("OK");
                    break;
                case "UnSubscribToCurrency":
                    _isSubscribedToCurrency = false;
                    SendAnswer("OK");
                    break;
                default:
                    SendAnswer("ERR");
                    break;
            }
        }

        public void SendAnswer(string answer)
        {
            byte[] ServerAnswer = Encoding.UTF8.GetBytes(answer);
            _pipeServer.Write(ServerAnswer, 0, ServerAnswer.Length);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pipeServer?.Dispose();
                _clientCommands?.Dispose();
            }
        }

        ~Client()
        {
            Dispose(false);
        }
    }
}
