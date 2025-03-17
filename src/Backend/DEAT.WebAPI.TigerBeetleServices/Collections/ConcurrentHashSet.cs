using System.Collections;
using System.Collections.Concurrent;

namespace DEAT.WebAPI.TigerBeetleServices.Collections;

#nullable enable

/// <summary>
/// Represents a thread-safe hash set.
/// </summary>
public class ConcurrentHashSet<T> : ICollection<T> where T : notnull
{
    private readonly ConcurrentDictionary<T, byte> _dictionary;

    public ConcurrentHashSet()
    {
        _dictionary = new ConcurrentDictionary<T, byte>();
    }

    public int Count => _dictionary.Count;

    public bool IsReadOnly => false;

    public bool Add(T item) => _dictionary.TryAdd(item, 0);

    public void Clear() => _dictionary.Clear();

    public bool Contains(T item) => _dictionary.ContainsKey(item);

    public void CopyTo(T[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count) throw new ArgumentException("Destination array is not long enough");

        var items = _dictionary.Keys.ToArray();
        Array.Copy(items, 0, array, arrayIndex, items.Length);
    }

    public IEnumerator<T> GetEnumerator() => _dictionary.Keys.GetEnumerator();

    public bool Remove(T item) => _dictionary.TryRemove(item, out _);

    public T[] ToArray() => _dictionary.Keys.ToArray();

    void ICollection<T>.Add(T item)
    {
        Add(item);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
} 