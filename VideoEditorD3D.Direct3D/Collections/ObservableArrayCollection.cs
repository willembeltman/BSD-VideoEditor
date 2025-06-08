using System.Collections;
using System.Collections.Immutable;

namespace VideoEditorD3D.Direct3D.Collections;

public class ObservableArrayCollection<T> : ArrayCollection<T>
    where T : class
{
    public event EventHandler<T?>? Changed;
    public event EventHandler? Cleared;
    public event EventHandler<T>? Added;
    public event EventHandler<T>? Removed;

    public ObservableArrayCollection(int capacity = 4) : base(capacity)
    {
    }

    // Add method om items toe te voegen
    public override void Add(T item)
    {
        base.Add(item);

        Added?.Invoke(this, item); // Trigger de Added event
        Changed?.Invoke(this, item); // Trigger de Changed event
    }

    // Verwijder een item
    public override bool Remove(T item)
    {
        var result = base.Remove(item);

        Removed?.Invoke(this, item); // Trigger de Removed event
        Changed?.Invoke(this, item); // Trigger de Changed event

        return result;
    }

    // Clear method om de collectie leeg te maken
    public override void Clear()
    {
        foreach (var item in CurrentArray)
        {
            Removed?.Invoke(this, item); // Trigger de Removed event
            Changed?.Invoke(this, item); // Trigger de Changed event
        }

        base.Clear();

        Cleared?.Invoke(this, new EventArgs()); // Trigger de Cleared event
    }
}
