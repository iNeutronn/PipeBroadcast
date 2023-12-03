using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataParsing
{
    internal abstract class CashedDataParser<T>
    {
        protected TimeSpan TimeOut;
        protected DateTime LastUpdate;
        protected T CachedData;
        
        public T GetData()
        {
            if (DateTime.Now - LastUpdate > TimeOut)
            {
                LastUpdate = DateTime.Now;
                return GetDataFromSource();
            }
            else
            {
                return CachedData;
            }
        }

        protected abstract T GetDataFromSource();

        public CashedDataParser(TimeSpan timeOut)
        {
            TimeOut = timeOut;
            LastUpdate = DateTime.MinValue;
        }
       
    }
}
