using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.DataParsing;

namespace Server
{
    interface IDataTranslator
    {
        void StartTranslate();
        void StopTranslate();
    }


    abstract class DataTranslator<T> : IDataTranslator, IDisposable
    {
        protected static readonly object _pipeLock = new object();
        protected TimeSpan _interval;
        protected CashedDataParser<T> _data;
        protected Timer _timer;
        protected List<Client> _clients;

        public DataTranslator(CashedDataParser<T> data, TimeSpan interval, List<Client> clients)
        {
            _clients = clients;
            _data = data;
            _interval = interval;
            _timer = new Timer(Translate, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void StartTranslate()
        {
            _timer.Change(0, (int)_interval.TotalMilliseconds);
        }

        public void StopTranslate()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        protected abstract void Translate(object state);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
            }
        }
    }

}

