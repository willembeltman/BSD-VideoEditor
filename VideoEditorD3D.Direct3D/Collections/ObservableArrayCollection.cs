using System.Collections;

namespace VideoEditorD3D.Direct3D.Collections;

public class ObservableArrayCollection<T> : ICollection<T>
    where T : class
{
    public event EventHandler<T?>? Changed;
    public event EventHandler? Cleared;
    public event EventHandler<T>? Added;
    public event EventHandler<T>? Removed;

    private T[] _items;
    private int _count;

    public ObservableArrayCollection()
    {
        _items = new T[4]; // Begint met een kleine array
        _count = 0;
    }

    public int Count => _count;

    public bool IsReadOnly => false;

    // Add method om items toe te voegen
    public void Add(T item)
    {
        if (_count == _items.Length)
        {
            Array.Resize(ref _items, _items.Length * 2); // Vergroot de array bij behoefte
        }
        _items[_count++] = item;

        Added?.Invoke(this, item); // Trigger de Added event
        Changed?.Invoke(this, item); // Trigger de Changed event
    }

    // Clear method om de collectie leeg te maken
    public void Clear()
    {
        _count = 0; // We maken de collectie leeg door het aantal te resetten
        Array.Clear(_items, 0, _items.Length); // Reset de array
        
        Cleared?.Invoke(this, new EventArgs()); // Trigger de Cleared event
        Changed?.Invoke(this, null); // Trigger de Changed event
    }

    // Check of een item in de collectie zit
    public bool Contains(T item)
    {
        for (int i = 0; i < _count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(_items[i], item))
            {
                return true;
            }
        }
        return false;
    }

    // Kopieer items naar een array
    public void CopyTo(T[] array, int arrayIndex)
    {
        Array.Copy(_items, 0, array, arrayIndex, _count);
    }

    // Verwijder een item
    public bool Remove(T item)
    {
        for (int i = 0; i < _count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(_items[i], item))
            {
                // Verschuif de overige items om het item te verwijderen
                for (int j = i; j < _count - 1; j++)
                {
                    _items[j] = _items[j + 1];
                }
                _items[--_count] = default; // Reset het verwijderde item

                Removed?.Invoke(this, item); // Trigger de Removed event
                Changed?.Invoke(this, item); // Trigger de Changed event
                return true;
            }
        }
        return false;
    }

    // Enumerator voor itereren over de collectie
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
        {
            yield return _items[i];
        }
    }

    // Non-generic enumerator
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
