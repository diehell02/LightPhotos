using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace LightPhotos.System.Collections;
public class DataVirtualizationCollection<T> : IList<T>, INotifyCollectionChanged, ISupportIncrementalLoading
{
    private readonly IItemsProvider<T> _itemsProvider;
    private readonly List<T> _items;
    private bool _busy;

    public DataVirtualizationCollection(IItemsProvider<T> itemsProvider)
    {
        _itemsProvider = itemsProvider;
        _items = new List<T>();
    }

    #region IList

    public int Count => _itemsProvider.Count;

    public bool IsReadOnly => true;

    public T this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public int IndexOf(T item) => throw new NotImplementedException();
    public void Insert(int index, T item) => throw new NotImplementedException();
    public void RemoveAt(int index) => throw new NotImplementedException();
    public void Add(T item) => throw new NotImplementedException();
    public void Clear() => throw new NotImplementedException();
    public bool Contains(T item) => throw new NotImplementedException();
    public void CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
    public bool Remove(T item) => throw new NotImplementedException();
    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>)_items;

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)_items;

    #endregion

    #region INotifyCollectionChanged

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    #endregion

    #region ISupportIncrementalLoading

    bool ISupportIncrementalLoading.HasMoreItems => _items.Count < _itemsProvider.Count;


    IAsyncOperation<LoadMoreItemsResult> ISupportIncrementalLoading.LoadMoreItemsAsync(uint count)
    {
        if (_busy)
        {
            throw new InvalidOperationException("Only one operation in flight at a time");
        }

        _busy = true;

        return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
    }

    private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
    {
        try
        {
            var list = await _itemsProvider.Fetch(_items.Count + 1, count);
            var baseIndex = _items.Count;
            _items.AddRange(list);

            // Now notify of the new items
            NotifyOfInsertedItems(baseIndex, list.Count);

            return new LoadMoreItemsResult((uint)list.Count);
        }
        finally
        {
            _busy = false;
        }
    }

    private void NotifyOfInsertedItems(int baseIndex, int count)
    {
        if (CollectionChanged == null)
        {
            return;
        }

        for (var i = 0; i < count; i++)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _items[i + baseIndex], i + baseIndex);
            CollectionChanged(this, args);
        }
    }

    #endregion
}
