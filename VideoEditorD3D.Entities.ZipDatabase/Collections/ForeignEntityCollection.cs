using System.Collections;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;

namespace VideoEditorD3D.Entities.ZipDatabase.Collections;

public class ForeignEntityCollection<TForeign, TPrimary> : ICollection<TForeign>
    where TForeign : IEntity
    where TPrimary : IEntity
{
    private readonly DbSet<TForeign> DbSet;
    private readonly Func<TForeign, TPrimary, bool> WhereHasForeignKey;
    private readonly Action<TForeign, TPrimary> SetForeignKey;
    private readonly TPrimary Primary;

    public ForeignEntityCollection(
        DbSet<TForeign> dbSet, 
        TPrimary primary, 
        Func<TForeign, TPrimary, bool> whereHasForeignKey, 
        Action<TForeign, TPrimary> setForeignKey)
    {
        DbSet = dbSet;
        WhereHasForeignKey = whereHasForeignKey;
        SetForeignKey = setForeignKey;
        Primary = primary;
    }

    public int Count => DbSet.Count(foreign => WhereHasForeignKey(foreign, Primary));
    public bool IsReadOnly => false;

    public void Add(TForeign item)
    {
        SetForeignKey(item, Primary);
        DbSet.Add(item);
    }

    public void CopyTo(TForeign[] array, int arrayIndex)
    {
        foreach (var item in DbSet.Where(a => WhereHasForeignKey(a, Primary)))
        {
            if (arrayIndex >= array.Length) throw new ArgumentException("Target array too small");
            array[arrayIndex++] = item;
        }
    }
    public bool Remove(TForeign item)
    {
        if (!DbSet.Any(a => WhereHasForeignKey(a, Primary) && a.Id == item.Id)) return false;
        return DbSet.Remove(item);
    }

    public void Clear()
    {
        var list = DbSet.Where(a => WhereHasForeignKey(a, Primary)).ToArray();
        DbSet.RemoveRange(list);
    }

    public bool Contains(TForeign item)
    {
        return DbSet.Any(a => WhereHasForeignKey(a, Primary) && a.Id == item.Id);
    }

    public IEnumerator<TForeign> GetEnumerator()
    {
        foreach (var item in DbSet.Where(a => WhereHasForeignKey(a, Primary)))
        {
            yield return item;
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}