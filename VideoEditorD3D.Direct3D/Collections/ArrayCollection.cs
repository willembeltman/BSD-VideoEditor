using System.Collections;

namespace VideoEditorD3D.Direct3D.Collections;

public class ArrayCollection<T> : ICollection<T>
    where T : class
{
    public T[] CurrentArray;

    public ArrayCollection(int capacity = 4)
    {
        CurrentArray = new T[capacity];
    }

    public virtual bool IsReadOnly => false;

    public virtual int Count { get; set; }

    private void Resize(int newSize)
    {
        var newArray = new T[newSize];
        Array.Copy(CurrentArray, newArray, Count);
        CurrentArray = newArray;
    }

    // Add method om items toe te voegen
    public virtual void Add(T item)
    {
        // Resize als array vol is
        if (Count == CurrentArray.Length)
        {
            Resize(CurrentArray.Length * 2);
        }

        CurrentArray[Count++] = item;
    }

    // Verwijder een item
    public virtual bool Remove(T item)
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

        return true;
    }

    public virtual int IndexOf(T item)
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
    public virtual void Clear()
    {
        CurrentArray = []; // Maak de array leeg
    }

    // Check of een item in de collectie zit
    public virtual bool Contains(T item)
    {
        return CurrentArray.Contains(item);
    }

    // Kopieer items naar een array
    public virtual void CopyTo(T[] array, int arrayIndex)
    {
        for (int i = 0; i < Count; i++)
        {
            array[arrayIndex + i] = CurrentArray[i];
        }
    }

    // Enumerator voor itereren over de collectie
    public virtual IEnumerator<T> GetEnumerator()
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
    public virtual T this[int index]
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
        }
    }
}