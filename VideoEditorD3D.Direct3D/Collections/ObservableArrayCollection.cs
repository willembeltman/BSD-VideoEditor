using System.Collections;
using System.Collections.Immutable;

namespace VideoEditorD3D.Direct3D.Collections;

public class ObservableArrayCollection<T> : ICollection<T>
    where T : class
{
    public event EventHandler<T?>? Changed;
    public event EventHandler? Cleared;
    public event EventHandler<T>? Added;
    public event EventHandler<T>? Removed;

    private ImmutableArray<T> _items;

    public ObservableArrayCollection()
    {
        _items = ImmutableArray<T>.Empty;
    }

    public int Count => _items.Length;

    public bool IsReadOnly => false;

    // Add method om items toe te voegen
    public void Add(T item)
    {
        _items = _items.Add(item); // Voeg het item toe aan de array

        Added?.Invoke(this, item); // Trigger de Added event
        Changed?.Invoke(this, item); // Trigger de Changed event
    }

    // Verwijder een item
    public bool Remove(T item)
    {
        var contains = _items.Contains(item);
        if (contains)
        {
            _items = _items.Remove(item); // Verwijder het item uit de array
            Removed?.Invoke(this, item); // Trigger de Removed event
            Changed?.Invoke(this, item); // Trigger de Changed event
        }
        return contains;
    }

    public void Sort()
    {
        _items = _items.Sort(); // Sorteer de array
    }

    // Clear method om de collectie leeg te maken
    public void Clear()
    {
        _items = _items.Clear(); // Maak de array leeg

        Cleared?.Invoke(this, new EventArgs()); // Trigger de Cleared event
        Changed?.Invoke(this, null); // Trigger de Changed event
    }

    // Check of een item in de collectie zit
    public bool Contains(T item)
    {
        return _items.Contains(item); 
    }

    // Kopieer items naar een array
    public void CopyTo(T[] array, int arrayIndex)
    {
        _items.CopyTo(array, arrayIndex);
    }

    // Enumerator voor itereren over de collectie
    public IEnumerator<T> GetEnumerator()
    {
        var temp = _items;
        for (int i = 0; i < temp.Length; i++)
        {
            yield return temp[i];
        }
    }

    // Non-generic enumerator
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
