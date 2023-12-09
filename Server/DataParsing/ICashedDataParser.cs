using System.Net;

namespace Server.DataParsing
{

    internal abstract class CashedDataParser<T>
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        protected TimeSpan TimeOut;
        protected DateTime LastUpdate;
        protected T CachedData;

        public T GetData()
        {
            if ((DateTime.Now - LastUpdate > TimeOut) || CachedData == null)
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

        protected CashedDataParser(TimeSpan timeOut)
        {
            TimeOut = timeOut;
            LastUpdate = DateTime.MinValue;
        }

    }
}
