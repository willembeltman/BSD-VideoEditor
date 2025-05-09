namespace VideoEditorD3D.Types;

public readonly struct Resolution
{
    public Resolution()
    {
        Width = 1920;
        Height = 1080;
    }
    public Resolution(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public int Width { get; }
    public int Height { get; }

    public static bool operator ==(Resolution p1, Resolution p2)
    {
        return p1.Equals(p2);
    }
    public static bool operator !=(Resolution p1, Resolution p2)
    {
        return !p1.Equals(p2);
    }
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (!(obj is Resolution?)) return false;

        var other = obj as Resolution?;
        if (other == null) return false;
        if (Width != other.Value.Width) return false;
        if (Height != other.Value.Height) return false;
        return true;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    public override string ToString()
    {
        return $"{Width}x{Height}";
    }

    public static bool TryParse(int? width, int? height, out Resolution resolution)
    {
        resolution = new Resolution();
        if (width == null || height == null) return false;
        resolution = new Resolution(width.Value, height.Value);
        return true;
    }
}