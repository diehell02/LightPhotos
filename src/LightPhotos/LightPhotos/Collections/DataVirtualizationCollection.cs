using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarSymbols;
using LightPhotos.Core.Helpers;
using LightPhotos.Core.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Data;

namespace LightPhotos.Collections;
public class DataVirtualizationCollection<T> : IList, INotifyCollectionChanged, IItemsRangeInfo
{
    private readonly IItemsProvider<T> _itemsProvider;
    private readonly T[] _items;
    private bool _busy;
    private readonly AntiShakeLimiter _antiShakeLimiter = new(TimeSpan.FromMilliseconds(500));

    public DataVirtualizationCollection(IItemsProvider<T> itemsProvider)
    {
        _itemsProvider = itemsProvider;
        _items = new T[_itemsProvider.Count];
    }

    private T FetchFromIndex(int index, bool loadData = false)
    {
        if (index >= _items.Length || _items[index] is null)
        {
            var item = _itemsProvider.Fetch(index);
            _items[index] = item;
            // Now notify of the new items
            //NotifyOfInsertedItems(index, 1);
        }
        return _items[index];
    }

    #region IList

    public bool IsFixedSize => throw new NotImplementedException();

    public bool IsReadOnly => throw new NotImplementedException();

    public int Count => _itemsProvider.Count;

    public bool IsSynchronized => throw new NotImplementedException();

    public object SyncRoot => throw new NotImplementedException();

    public object? this[int index] 
    {
        get
        {
            return FetchFromIndex(index);
        }
        set => throw new NotImplementedException(); 
    }

    public int Add(object value) => throw new NotImplementedException();
    public void Clear() => throw new NotImplementedException();
    public bool Contains(object value) => throw new NotImplementedException();
    public int IndexOf(object value) => throw new NotImplementedException();
    public void Insert(int index, object value) => throw new NotImplementedException();
    public void Remove(object value) => throw new NotImplementedException();
    public void RemoveAt(int index) => throw new NotImplementedException();
    public void CopyTo(Array array, int index) => throw new NotImplementedException();
    public IEnumerator GetEnumerator() => throw new NotImplementedException();

    #endregion

    #region INotifyCollectionChanged

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    #endregion

    //#region ISupportIncrementalLoading

    //public bool HasMoreItems => _items.Length < _itemsProvider.Count;

    //public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
    //{
    //    Log.Debug($"count:{count}");
    //    if (_busy)
    //    {
    //        throw new InvalidOperationException("Only one operation in flight at a time");
    //    }

    //    _busy = true;

    //    return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
    //}

    //private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
    //{
    //    try
    //    {
    //        var baseIndex = _items.Length;
    //        var list = await _itemsProvider.Fetch(baseIndex, count);
    //        for (var i = 0; i < list.Count; i++)
    //        {
    //            _items[baseIndex + i] = list[i];
    //        }

    //        // Now notify of the new items
    //        NotifyOfInsertedItems(baseIndex, list.Count);

    //        return new LoadMoreItemsResult((uint)list.Count);
    //    }
    //    finally
    //    {
    //        _busy = false;
    //    }
    //}

    //private void NotifyOfInsertedItems(int baseIndex, int count)
    //{
    //    if (CollectionChanged == null)
    //    {
    //        return;
    //    }

    //    for (var i = 0; i < count; i++)
    //    {
    //        var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _items[i + baseIndex], i + baseIndex);
    //        CollectionChanged(this, args);
    //    }
    //}

    //#endregion

    #region IItemsRangeInfo

    public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
    {
        Log.Debug($"visibleRange: [FirstIndex:{visibleRange.FirstIndex} LastIndex:{visibleRange.LastIndex} Length:{visibleRange.Length}]");
        foreach (var item in trackedItems)
        {
            Log.Debug($"item: [FirstIndex:{item.FirstIndex} LastIndex:{item.LastIndex} Length:{item.Length}]");
        }
        if (trackedItems.Count == 0)
        {
            return;
        }
        void LoadTrackedRange()
        {
            var trackedRange = trackedItems[0];
            for (var i = trackedRange.FirstIndex; i <= trackedRange.LastIndex; i++)
            {
                _itemsProvider.LoadData(FetchFromIndex(i));
            }
        }
        void LoadVisibleRange()
        {
            for (var i = visibleRange.FirstIndex; i <= visibleRange.LastIndex; i++)
            {
                _itemsProvider.LoadData(FetchFromIndex(i));
            }
        }
        var thread = DispatcherQueue.GetForCurrentThread();
        _antiShakeLimiter.Execute(() =>
        {
            thread.TryEnqueue(() =>
            {
                LoadTrackedRange();
            });
        });
    }

    public void Dispose() => throw new NotImplementedException();

    #endregion

    //#region ISelectionInfo

    //public void SelectRange(ItemIndexRange itemIndexRange) => throw new NotImplementedException();
    //public void DeselectRange(ItemIndexRange itemIndexRange) => throw new NotImplementedException();
    //public bool IsSelected(int index) => throw new NotImplementedException();
    //public IReadOnlyList<ItemIndexRange> GetSelectedRanges() => throw new NotImplementedException();

    //#endregion
}
