namespace VideoEditorD3D.FF.Types;

public readonly struct Fps
{
    public Fps()
    {
        Base = 25;
        Divider = 1;
    }

    public Fps(long @base, long divider)
    {
        Base = @base;
        Divider = divider;
    }

    public long Base { get; }
    public long Divider { get; }

    public double Value
    {
        get
        {
            return Convert.ToDouble(Base) / Divider;
        }
    }

    public static bool operator ==(Fps p1, Fps p2)
    {
        return p1.Equals(p2);
    }
    public static bool operator !=(Fps p1, Fps p2)
    {
        return !p1.Equals(p2);
    }
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (!(obj is Fps)) return false;

        var other = obj as Fps?;
        if (other == null) return false;
        if (Base != other.Value.Base) return false;
        if (Divider != other.Value.Divider) return false;
        return true;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Base, Divider);
    }

    public override string ToString()
    {
        return $"{Value}";
    }

    public static bool TryParse(string? value, out Fps result)
    {
        result = new Fps();

        if (value == null) return false;

        var list = value.Split(['/'], StringSplitOptions.RemoveEmptyEntries);

        if (list.Length != 2) return false;
        if (!long.TryParse(list[0], out var @base)) return false;
        if (!long.TryParse(list[1], out var divider)) return false;

        result = new Fps(@base, divider);
        return true;
    }

    public static implicit operator double(Fps windowsScaling)
    {
        return windowsScaling.Value;
    }
}

