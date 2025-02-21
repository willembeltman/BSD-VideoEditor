namespace VideoEditor.Types;

public readonly struct Timestamp
{
    public Timestamp()
    {
        Hours = 0;
        Minutes = 0;
        Seconds = 0;
        Milliseconds = 0;
    }
    public Timestamp(string time)
    {
        if (string.IsNullOrWhiteSpace(time))
            throw new ArgumentException("Invalid timestamp format", nameof(time));

        var parts = time.Split(':', '.');

        if (parts.Length == 3) // mm:ss.SSS formaat
        {
            Minutes = int.Parse(parts[0]);
            Seconds = int.Parse(parts[1]);
            Milliseconds = int.Parse(parts[2].PadRight(3, '0'));
        }
        else if (parts.Length == 4) // HH:mm:ss.SSS formaat
        {
            Hours = int.Parse(parts[0]);
            Minutes = int.Parse(parts[1]);
            Seconds = int.Parse(parts[2]);
            Milliseconds = int.Parse(parts[3].PadRight(3, '0'));
        }
        else
        {
            throw new FormatException("Invalid timestamp format. Expected HH:mm:ss.SSS or mm:ss.SSS");
        }
    }
    public Timestamp(int hours, int minutes, int seconds, int milliseconds)
    {
        Hours = hours;
        Minutes = minutes;
        Seconds = seconds;
        Milliseconds = milliseconds;
    }
    public Timestamp(TimeSpan time)
    {
        Hours = time.Hours;
        Minutes = time.Minutes;
        Seconds = time.Seconds;
        Milliseconds = time.Milliseconds;
    }
    public Timestamp(long frameIndex, Fps fps)
    {
        double timeInSeconds = frameIndex * fps.Base / fps.Divider;
        var timeSpan = TimeSpan.FromSeconds(timeInSeconds);
        Hours = timeSpan.Hours;
        Minutes = timeSpan.Minutes;
        Seconds = timeSpan.Seconds;
        Milliseconds = timeSpan.Milliseconds;
    }

    public int Hours { get; }
    public int Minutes { get; }
    public int Seconds { get; }
    public int Milliseconds { get; }

    public static bool operator ==(Timestamp p1, Timestamp p2)
    {
        return p1.Equals(p2);
    }
    public static bool operator !=(Timestamp p1, Timestamp p2)
    {
        return !p1.Equals(p2);
    }
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (!(obj is Timestamp)) return false;

        var other = obj as Timestamp?;
        if (other == null) return false;
        if (Hours != other.Value.Hours) return false;
        if (Minutes != other.Value.Minutes) return false;
        if (Seconds != other.Value.Seconds) return false;
        if (Milliseconds != other.Value.Milliseconds) return false;

        return true;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Hours, Minutes, Seconds, Milliseconds);
    }

    public override string ToString() => $"{Hours:D2}:{Minutes:D2}:{Seconds:D2}.{Milliseconds:D3}";
}
