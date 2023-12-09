using Newtonsoft.Json;
using Server.DataTranslators;
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

        public void StartListenClient() 
        {
            Thread t = new(async () =>
            {
                _pipeServer.WaitForConnection();
                if (_pipeServer == null)
                    throw new InvalidOperationException("NamedPipeServerStream not initialized.");

                byte[] buffer = new byte[256];

                while (true)
                {
                    int bytesRead = await _pipeServer.ReadAsync(buffer, 0, buffer.Length);

                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    await Console.Out.WriteLineAsync("Lisener: recived " + receivedData);

                    if (!string.IsNullOrEmpty(receivedData))
                    {
                        ProcessCommand(receivedData);

                        OnClientCommandReceived(receivedData);
                    }
                }
            })
            {
                Name = "ClientListener " + _id
            };
            t.Start();
        }

        private void OnClientCommandReceived(string receivedData)
        {
            ClientCommandReceived?.Invoke(this, receivedData);
        }

        private void ProcessCommand(string command)
        {
            switch (command)
            {
                case "quit":
                    SendAnswer(new TransitionObject() { Header = "ServisData", Data = "OK" });
                    clientsmanager.RemoveClient(this);
                    Dispose();     
                    break;
                case "SubscribToShares":
                    _isSubscribedToShares = true;
                    SendAnswer(new TransitionObject() { Header = "ServisData", Data = "OK" });
                    break;
                case "SubscribToWeather":
                    _isSubscribedToWeather = true;
                    SendAnswer(new TransitionObject() { Header = "ServisData", Data = "OK" });
                    break;
                case "SubscribToCurrency":
                    _isSubscribedToCurrency = true;
                    SendAnswer(new TransitionObject() { Header = "ServisData", Data = "OK" });
                    break;
                case "UnSubscribToShares":
                    _isSubscribedToShares = false;
                    SendAnswer(new TransitionObject() { Header = "ServisData", Data = "OK" });
                    break;
                case "UnSubscribToWeather":
                    _isSubscribedToWeather = false;
                    SendAnswer(new TransitionObject() { Header = "ServisData", Data = "OK" });
                    break;
                case "UnSubscribToCurrency":
                    _isSubscribedToCurrency = false;
                    SendAnswer(new TransitionObject() { Header = "ServisData", Data = "OK" });
                    break;
                default:
                    SendAnswer(new TransitionObject() { Header = "ServisData", Data = "ERR" });
                    break;
            }
        }

        public void SendAnswer(TransitionObject transitionObject)
        {
            byte[] ServerAnswer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(transitionObject));
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
