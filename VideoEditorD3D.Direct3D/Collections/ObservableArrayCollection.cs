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

    public ImmutableArray<T> CurrentArray;

    public ObservableArrayCollection()
    {
        CurrentArray = ImmutableArray<T>.Empty;
    }

    public int Count => CurrentArray.Length;

    public bool IsReadOnly => false;

    // Add method om items toe te voegen
    public void Add(T item)
    {
        Added?.Invoke(this, item); // Trigger de Added event
        Changed?.Invoke(this, item); // Trigger de Changed event
        CurrentArray = CurrentArray.Add(item); // Voeg het item toe aan de array
    }

    // Verwijder een item
    public bool Remove(T item)
    {
        var contains = CurrentArray.Contains(item);
        if (contains)
        {
            CurrentArray = CurrentArray.Remove(item); // Verwijder het item uit de array
            Removed?.Invoke(this, item); // Trigger de Removed event
            Changed?.Invoke(this, item); // Trigger de Changed event
        }
        return contains;
    }

    public void Sort()
    {
        CurrentArray = CurrentArray.Sort(); // Sorteer de array
    }

    // Clear method om de collectie leeg te maken
    public void Clear()
    {
        CurrentArray = CurrentArray.Clear(); // Maak de array leeg

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
        CurrentArray.CopyTo(array, arrayIndex);
    }

    // Enumerator voor itereren over de collectie
    public IEnumerator<T> GetEnumerator()
    {
        var temp = CurrentArray;
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
