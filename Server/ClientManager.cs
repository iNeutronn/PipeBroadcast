﻿using System;
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
        private IDataTranslator[] _translators;
        private bool _isRunning = true;

        public ClientManager()
        {
            _clients = new List<Client>();
            _idCounter = 0;
            _idThread = Task.Run(() => idThreadWork());
            _translators = new IDataTranslator[]
            {
               new SharesTranslator(new SharesDataParser(), new TimeSpan(0,1,0) , _clients),
               new CurrencyTranslator(new CurencyDataParser(), new TimeSpan(0,3,0) , _clients),
               new WetherTranslator(new WeatherDataParser(), new TimeSpan(0,5,0) , _clients)
            };

            foreach(var translator in _translators)
                translator.StartTranslate();   
        }

        public IEnumerator<Client> GetEnumerator()
        {
            return ((IEnumerable<Client>)_clients).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_clients).GetEnumerator();
        }

        private async Task idThreadWork()
        {
            using (NamedPipeServerStream idPipe = new NamedPipeServerStream("idPipe", PipeDirection.InOut, 10, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            {
                while (_isRunning)
                {
                    await idPipe.WaitForConnectionAsync();
                    //Console.WriteLine("IsConnected");

                    Guid clientId = Guid.NewGuid();
                    //Console.WriteLine("Id = " + clientId);
                    _clients.Add(new Client(clientId.ToString()));
                    ++_idCounter;

                    byte[] clientIdBytes = Encoding.UTF8.GetBytes(clientId.ToString());
                    //Console.WriteLine(clientIdBytes);
                    //Console.WriteLine(clientIdBytes.Length);

                    idPipe.Write(clientIdBytes, 0, clientIdBytes.Length);
                    _clients[^1].ListenClient();
                    //Console.WriteLine("IsWrite");
                    byte[] confirmation = new byte[1];
                    int bytesRead = await idPipe.ReadAsync(confirmation, 0, confirmation.Length);
                    Console.WriteLine(confirmation.ToString());
                    idPipe.Disconnect();
                }
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
                _clients.Clear();
                foreach (var translator in _translators)
                 {
                     translator.StopTranslate();
                     if (translator is IDisposable disposableTranslator)
                     {
                         disposableTranslator.Dispose();
                     }
                 }
             }
        }
    }
}