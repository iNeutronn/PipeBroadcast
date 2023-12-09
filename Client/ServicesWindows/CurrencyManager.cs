using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Server.DataParsing.DataObjects.Curency;

namespace Client
{
    public class CurrencyManager : IEnumerable<CurencyRate>, INotifyCollectionChanged 
    { 
        private List<CurencyRate> _rates = new List<CurencyRate>();

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public IEnumerator<CurencyRate> GetEnumerator()
        {
            return _rates.GetEnumerator();
        }

        public void SetNewInfo(CurencyRate[] rates)
        {
            _rates = rates.ToList();
            System.Windows.Application.Current.Dispatcher.Invoke(() => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
