using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPhotos.Collections;
public interface IItemsProvider<T> where T : class
{
    int Count
    {
        get;
    }

    /// <summary>
    /// Fetches a range of items.
    /// </summary>
    /// <param name="startIndex">The start index.</param>
    /// <param name="count">The number of items to fetch.</param>
    /// <returns></returns>
    Task<IList<T>> Fetch(int startIndex, uint count);

    T Fetch(int index);

    void LoadData(T item);
}
