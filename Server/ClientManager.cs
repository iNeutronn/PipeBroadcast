using System;
using System.Collections;
using System.IO.Pipes;
using System.Text;
using Server.DataParsing;
using Server.DataParsing.DataObjects.Curency;
using Server.DataParsing.DataObjects.Shares;
using Server.DataParsing.DataObjects.Weather;
using Server.DataTranslators;

namespace Server
{
    internal class ClientManager : IEnumerable<Client>, IDisposable
    {
        private List<Client> _clients;
        private int _idCounter;
        private Task _idThread;
        private Dictionary<Client, IDataTranslator[]> _clientTranslators;
        //private IDataTranslator[] _translators;
        private bool _isRunning = true;

        public ClientManager()
        {
            _clients = new List<Client>();
            _idCounter = 0;
            _idThread = Task.Run(() => idThreadWork());
            _clientTranslators = new Dictionary<Client, IDataTranslator[]>();
            //_translators = new IDataTranslator[]
            //{
            //   new SharesTranslator(new SharesDataParser(), TimeSpan.FromSeconds(100000) , _clients),
            //   new CurrencyTranslator(new CurencyDataParser(), new TimeSpan(0,3,0) , _clients),
            //   new WetherTranslator(new WeatherDataParser(), TimeSpan.FromSeconds(10) , _clients)
            //};

            //foreach (var translator in _translators)
            //    translator.StartTranslate();
        }

        public IEnumerator<Client> GetEnumerator()
        {
            return ((IEnumerable<Client>)_clients).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_clients).GetEnumerator();
        }

        public void RemoveClient(Client client)
        {
            _clients.Remove(client);
            foreach (var translator in _clientTranslators[client])
            {
                translator.StopTranslate();
                if (translator is IDisposable disposableTranslator)
                {
                     disposableTranslator.Dispose();
                }
            }
        }

        public void ClearClients()
        {
            foreach (var client in _clients)
            {
                RemoveClient(client);
            }
        }

        public void AddClient(Client client)
        {
            _clients.Add(client);
            _clientTranslators[client] = new IDataTranslator[]
            {
                new SharesTranslator(new SharesDataParser(), TimeSpan.FromSeconds(100000) , client),
                new CurrencyTranslator(new CurencyDataParser(), new TimeSpan(0,3,0) , client),
                new WetherTranslator(new WeatherDataParser(), TimeSpan.FromSeconds(10) , client)
            };
        }

        private async Task idThreadWork()
        {
            using (NamedPipeServerStream idPipe = new NamedPipeServerStream("idPipe", PipeDirection.InOut, 10, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                while (_isRunning)
                {
                    await idPipe.WaitForConnectionAsync();
                    //Console.WriteLine("IsConnected");

                    Guid clientId = Guid.NewGuid();
                    //Console.WriteLine("Id = " + clientId);
                    Client client = new Client(clientId, this);

                    ++_idCounter;

                    byte[] clientIdBytes = clientId.ToByteArray();
                    //Console.WriteLine(clientIdBytes);
                    //Console.WriteLine(clientIdBytes.Length);

                    idPipe.Write(clientIdBytes, 0, clientIdBytes.Length);



                    //Console.WriteLine("IsWrite");
                    byte[] confirmation = new byte[1];
                    // int bytesRead = await idPipe.ReadAsync(confirmation, 0, confirmation.Length);
                    idPipe.Read(confirmation, 0, confirmation.Length);
                    Console.WriteLine(confirmation.ToString());

                    client.ListenClient();

                    AddClient(client);

                    client.ClientCommandReceived += Client_ClientCommandReceived;

                    idPipe.Disconnect();
                }
            }

        }

        private void Client_ClientCommandReceived(object? sender, string e)
        {
            Client client = (Client)sender;

            if (e == "SubscribToShares")
            {
                foreach(var translator in _clientTranslators[client])
                {
                    if(translator is SharesTranslator)
                    {
                        translator.StartTranslate();
                    }
                }
            }
            else if (e == "SubscribToWeather")
            {
                foreach (var translator in _clientTranslators[client])
                {
                    if (translator is WetherTranslator)
                    {
                        translator.StartTranslate();
                    }
                }
            }
            else if (e == "SubscribToCurrency")
            {
                foreach (var translator in _clientTranslators[client])
                {
                    if (translator is CurrencyTranslator)
                    {
                        translator.StartTranslate();
                    }
                }
            }
            else if (e == "UnSubscribToShares")
            {
                foreach (var translator in _clientTranslators[client])
                {
                    if (translator is SharesTranslator)
                    {
                        translator.StopTranslate();
                    }
                }
            }
            else if (e == "UnSubscribToWeather")
            {
                foreach (var translator in _clientTranslators[client])
                {
                    if (translator is WetherTranslator)
                    {
                        translator.StopTranslate();
                    }
                }
            }
            else if (e == "UnSubscribToCurrency")
            {
                foreach (var translator in _clientTranslators[client])
                {
                    if (translator is CurrencyTranslator)
                    {
                        translator.StopTranslate();
                    }
                }
            }
            else if (e == "quit")
            {
                RemoveClient(client);
            }
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
                _isRunning = false;
                _idThread.Wait();
                _idThread.Dispose();
                ClearClients();
                //foreach (var translator in _translators)
                //{
                //    translator.StopTranslate();
                //    if (translator is IDisposable disposableTranslator)
                //    {
                //        disposableTranslator.Dispose();
                //    }
                //}
            }
        }
    }
}