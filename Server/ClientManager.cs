using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class ClientManager : IEnumerable<Client>
    {
        private List<Client> _clients;
        private int _idCounter;

        public ClientManager()
        {
            _clients = new List<Client>();
            _idCounter = 0;
        }

        public IEnumerator<Client> GetEnumerator()
        {
            return ((IEnumerable<Client>)_clients).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_clients).GetEnumerator();
        }
    }

}
