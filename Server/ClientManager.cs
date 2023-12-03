using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class ClientManager : IEnumerable<Client>
    {
        private List<Client> _clients;
        private int _idCounter;
        private Task _idThread;


        public ClientManager()
        {
            _clients = new List<Client>();
            _idCounter = 0;
            _idThread = Task.Run(() => idThreadWork());
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
                while (true)
                {
                    await idPipe.WaitForConnectionAsync();

                    Guid clientId = Guid.NewGuid();

                    _clients.Add(new Client(clientId.ToString()));
                    ++_idCounter;

                    byte[] clientIdBytes = clientId.ToByteArray();

                    idPipe.Write(clientIdBytes, 0, clientIdBytes.Length);

                    byte[] confirmation = new byte[1];
                    int bytesRead = await idPipe.ReadAsync(confirmation, 0, confirmation.Length);

                    idPipe.Disconnect();
                }
            }
        }


    }
}
