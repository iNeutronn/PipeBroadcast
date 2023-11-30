using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Client
    {
        private NamedPipeServerStream _pipeServer;
        private int _id;
        private bool _IsSubscribedToWeater;
        private bool _IsSubscribedToShares;
        private bool _IsSubscribedToCurency;
    }
}
