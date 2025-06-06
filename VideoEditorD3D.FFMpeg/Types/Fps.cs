﻿namespace VideoEditorD3D.FFMpeg.Types;

public struct Fps
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

    public long Base { get; set; }
    public long Divider { get; set; }

    public double FrameTime
    {
        get
        {
            return Convert.ToDouble(Base) / Divider;
        }
    }

    public long ConvertTimeToIndex(double time)
    {
        return Convert.ToInt32(time * Base / Divider);
    }
    public double ConvertIndexToTime(long index)
    {
        return (double)index / Base * Divider;
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
        return $"{FrameTime}";
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

    public static implicit operator double(Fps fps)
    {
        return fps.FrameTime;
    }
}

