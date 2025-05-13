using System.Collections;

namespace VideoEditor;

public class ConcurrentArray<T> : IEnumerable<T>
    where T : class
{
    private T[] TheArray { get; set; } = Array.Empty<T>();
    public int Length
    {
        get
        {
            int length = -1;
            lock (this)
            {
                length = TheArray.Length;
            }
            return length;
        }
    }

    public void Clear()
    {
        lock (this)
        {
            TheArray = Array.Empty<T>();
        }
    }
    public void Add(T item)
    {
        lock (this)
        {
            var newArray = new T[TheArray.Length + 1];
            Array.Copy(TheArray, newArray, TheArray.Length);
            newArray[TheArray.Length] = item;
            TheArray = newArray;
        }
    }
    public void AddRange(IEnumerable<T> list)
    {
        var Tlist2 = new List<T>(list);
        var addArray = Tlist2.ToArray();

        lock (this)
        {
            var newArray = new T[TheArray.Length + addArray.Length];
            Array.Copy(TheArray, newArray, TheArray.Length);
            Array.Copy(addArray, 0, newArray, TheArray.Length, addArray.Length);
            TheArray = newArray;
        }
    }

    public bool Remove(T item)
    {
        lock (this)
        {
            var newArray = new T[TheArray.Length - 1];
            int index = 0;
            bool found = false;

            foreach (var item2 in TheArray)
            {
                if (index >= newArray.Length)
                    break;

                if (item == item2)
                {
                    found = true;
                    continue;
                }

                if (index < newArray.Length)
                {
                    newArray[index] = item2;
                    index++;
                }
            }

            if (found)
                TheArray = newArray;

            return found;
        }
    }



    public IEnumerator<T> GetEnumerator()
    {
        T[] snapshot;
        lock (this)
        {
            snapshot = TheArray;
        }
        return ((IEnumerable<T>)snapshot).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

}

