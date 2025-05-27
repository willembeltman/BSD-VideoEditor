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

    public T[] CurrentArray;

    public ObservableArrayCollection(int capacity = 4)
    {
        CurrentArray = new T[capacity];
    }


    public bool IsReadOnly => false;

    public int Count { get; set; }

    private void Resize(int newSize)
    {
        var newArray = new T[newSize];
        Array.Copy(CurrentArray, newArray, Count);
        CurrentArray = newArray;
    }

    // Add method om items toe te voegen
    public void Add(T item)
    {
        Added?.Invoke(this, item); // Trigger de Added event
        Changed?.Invoke(this, item); // Trigger de Changed event

        // Resize als array vol is
        if (Count == CurrentArray.Length)
        {
            Resize(CurrentArray.Length * 2);
        }

        CurrentArray[Count++] = item;
    }

    // Verwijder een item
    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index == -1) return false;

        // Verschuif items na index 1 plek naar links
        for (int i = index; i < Count - 1; i++)
        {
            CurrentArray[i] = CurrentArray[i + 1];
        }

        Count--;
        CurrentArray[Count] = default!; // Referentie verwijderen voor GC

        Removed?.Invoke(this, item); // Trigger de Removed event
        Changed?.Invoke(this, item); // Trigger de Changed event

        return true;
    }

    private int IndexOf(T item)
    {
        var comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < Count; i++)
        {
            if (comparer.Equals(CurrentArray[i], item))
                return i;
        }
        return -1;
    }

    // Clear method om de collectie leeg te maken
    public void Clear()
    {
        CurrentArray = []; // Maak de array leeg

        Cleared?.Invoke(this, new EventArgs()); // Trigger de Cleared event
        Changed?.Invoke(this, null); // Trigger de Changed event
    }

    // Check of een item in de collectie zit
    public bool Contains(T item)
    {
        return CurrentArray.Contains(item);
    }

    // Kopieer items naar een array
    public void CopyTo(T[] array, int arrayIndex)
    {
        for (int i = 0; i < Count; i++)
        {
            array[arrayIndex + i] = CurrentArray[i];
        }
    }

    // Enumerator voor itereren over de collectie
    public IEnumerator<T> GetEnumerator()
    {
        var temp = CurrentArray;
        for (int i = 0; i < Count; i++)
        {
            yield return temp[i];
        }
    }

    // Non-generic enumerator
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // Optioneel: indexer voor toegang
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
            return CurrentArray[index];
        }
        set
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
            CurrentArray[index] = value;
            Changed?.Invoke(this, value);
        }
    }
}
