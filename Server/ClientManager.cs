using System;
using System.Collections;
using System.IO.Pipes;
using System.Text;
using Server.DataParsing;
using Server.DataParsing.DataObjects.Curency;
using Server.DataParsing.DataObjects.Shares;
using Server.DataTranslators;
using Server.DataParsing.DataObjects.Weather;

namespace Server
{
    internal class ClientManager : IEnumerable<Client>, IDisposable
    {
        private List<Client> _clients;
        private int _idCounter;
        private Task _idThread;
        private Dictionary<Client, IDataTranslator[]> _clientTranslators;
        private bool _isRunning = true;

        public ClientManager()
        {
            _clients = new List<Client>();
            _idCounter = 0;
            _idThread = Task.Run(() => idThreadWork());
            _clientTranslators = new Dictionary<Client, IDataTranslator[]>();
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
                new SharesTranslator(new SharesDataParser(TimeSpan.FromMinutes(5)), TimeSpan.FromMinutes(5) , client),
                new CurrencyTranslator(new CurencyDataParser(TimeSpan.FromMinutes(5)), TimeSpan.FromMinutes(5) , client),
                new WetherTranslator(new WeatherDataParser(TimeSpan.FromMinutes(5)), TimeSpan.FromMinutes(5) , client)
            };
        }

        private async Task idThreadWork()
        {
            using (NamedPipeServerStream idPipe = new NamedPipeServerStream("idPipe", PipeDirection.InOut, 10, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                while (_isRunning)
                {
                    await idPipe.WaitForConnectionAsync();
                    Console.WriteLine("IsConnected");

                    Guid clientId = Guid.NewGuid();
                    Console.WriteLine("new Id = " + clientId);
                    Client client = new (clientId);

                    ++_idCounter;

                    byte[] clientIdBytes = clientId.ToByteArray();


                    idPipe.Write(clientIdBytes, 0, clientIdBytes.Length);



                    
                    byte[] confirmation = new byte[1];
                    idPipe.Read(confirmation, 0, confirmation.Length);
                    Console.WriteLine(confirmation.ToString());

                    client.StartListenClient();

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
                //foreach (var translators in _clientTranslators.Values)
                //{
                //    foreach (var translator in translators)
                //    {
                //        translator.StopTranslate();
                //        if (translator is IDisposable disposableTranslator)
                //        {
                //            disposableTranslator.Dispose();
                //        }
                //    }
                //}
            }
        }
    }
}