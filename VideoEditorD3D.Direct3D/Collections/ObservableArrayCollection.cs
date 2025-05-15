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
    ReaderWriterLockSlim _lock;

    public ObservableArrayCollection()
    {
        _items = new T[4]; // Begint met een kleine array
        _count = 0;
        _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
    }

    public int Count => _count;

    public bool IsReadOnly => false;

    // Add method om items toe te voegen
    public void Add(T item)
    {
        try
        {
            _lock.EnterWriteLock();

            if (_count == _items.Length)
            {
                Array.Resize(ref _items, _items.Length * 2); // Vergroot de array bij behoefte
            }

            if (item is IComparable)
            {
                IComparable cmp = (IComparable)item;
                int i = _count - 1;
                while (i >= 0 && ((IComparable)_items[i]).CompareTo(cmp) > 0)
                {
                    _items[i + 1] = _items[i];
                    i--;
                }
                _items[i + 1] = item;
                _count++;
            }
            else
            {
                _items[_count++] = item;
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }

        Added?.Invoke(this, item); // Trigger de Added event
        Changed?.Invoke(this, item); // Trigger de Changed event
    }

    public void Sort()
    {
        try
        {
            _lock.EnterWriteLock();

            Array.Sort(_items, 0, _count);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    // Clear method om de collectie leeg te maken
    public void Clear()
    {
        try
        {
            _lock.EnterWriteLock();

            _count = 0; // We maken de collectie leeg door het aantal te resetten
            Array.Clear(_items, 0, _items.Length); // Reset de array

            Cleared?.Invoke(this, new EventArgs()); // Trigger de Cleared event
            Changed?.Invoke(this, null); // Trigger de Changed event
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    // Check of een item in de collectie zit
    public bool Contains(T item)
    {
        try
        {
            _lock.EnterReadLock();

            for (int i = 0; i < _count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_items[i], item))
                {
                    return true;
                }
            }
            return false;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    // Kopieer items naar een array
    public void CopyTo(T[] array, int arrayIndex)
    {
        try
        {
            _lock.EnterReadLock();

            Array.Copy(_items, 0, array, arrayIndex, _count);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    // Verwijder een item
    public bool Remove(T item)
    {
        try
        {
            _lock.EnterWriteLock();

            for (int i = 0; i < _count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_items[i], item))
                {
                    // Verschuif de overige items om het item te verwijderen
                    for (int j = i; j < _count - 1; j++)
                    {
                        _items[j] = _items[j + 1];
                    }
                    _items[--_count] = default!; // Reset het verwijderde item

                    Removed?.Invoke(this, item); // Trigger de Removed event
                    Changed?.Invoke(this, item); // Trigger de Changed event
                    return true;
                }
            }
            return false;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    // Enumerator voor itereren over de collectie
    public IEnumerator<T> GetEnumerator()
    {
        try
        {
            _lock.EnterReadLock();

            for (int i = 0; i < _count; i++)
            {
                yield return _items[i];
            }
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    // Non-generic enumerator
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
