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
        private NamedPipeServerStream _pipeServer;
        private int _id;
        private bool _IsSubscribedToWeater = false;
        private bool _IsSubscribedToShares = false;
        private bool _IsSubscribedToCurency = false;
        private Task _clientCommands;

        public Client(string id)
        {
            _pipeServer = new NamedPipeServerStream("pipe" + id, PipeDirection.InOut, 10, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            _pipeServer.WaitForConnection();
            ListenClient();
        }

        public void ListenClient()
        {
            // Перевірка, чи пайпа ініційована
            if (_pipeServer == null)
                throw new InvalidOperationException("NamedPipeServerStream not initialized.");
            
            // Прослуховування пайпи
            _clientCommands = Task.Run(async () =>
            {
                byte[] buffer = new byte[256]; // Розмір буфера для зчитування даних

                while (true)
                {
                    // Очікуємо, доки прийдуть дані з пайпи
                    int bytesRead = await _pipeServer.ReadAsync(buffer, 0, buffer.Length);

                    // Якщо дані отримані, конвертуємо їх у рядок
                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (!string.IsNullOrEmpty(receivedData))
                        // Обробка отриманих даних
                        CommandProcessing(receivedData);
                }
            });
        }

        private void CommandProcessing(string command)
        {
            switch (command)
            {
                case "quit":
                    Dispose();
                    break;
                case "SubscribToShares":
                    _IsSubscribedToShares = true;
                    break;
                case "SubscribToWeather":
                    _IsSubscribedToWeater = true;
                    break;
                case "SubscribToCurrency":
                    _IsSubscribedToCurency = true;
                    break;
                case "UnSubscribToShares":
                    _IsSubscribedToShares = false;
                    break;
                case "UnSubscribToWeather":
                    _IsSubscribedToWeater = false;
                    break;
                case "UnSubscribToCurrency":
                    _IsSubscribedToCurency = false;
                    break;
                default:
                    break;
            }
        }

        // Реалізація інтерфейсу IDisposable
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
