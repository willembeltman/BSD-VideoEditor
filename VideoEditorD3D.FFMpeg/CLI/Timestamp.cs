using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.FFMpeg.CLI;

public readonly struct TimeStamp
{
    public TimeStamp()
    {
        TimeSpan = new TimeSpan(0, 0, 0, 0);
    }
    public TimeStamp(string time)
    {
        if (string.IsNullOrWhiteSpace(time))
            throw new ArgumentException("Invalid timestamp format", nameof(time));

        var parts = time.Split(':', '.');

        if (parts.Length == 3) // mm:ss.SSS formaat
        {
            var minutes = int.Parse(parts[0]);
            var seconds = int.Parse(parts[1]);
            var milliseconds = int.Parse(parts[2].PadRight(3, '0'));
            TimeSpan = new TimeSpan(0, minutes, seconds, milliseconds);
        }
        else if (parts.Length == 4) // HH:mm:ss.SSS formaat
        {
            var hours = int.Parse(parts[0]);
            var minutes = int.Parse(parts[1]);
            var seconds = int.Parse(parts[2]);
            var milliseconds = int.Parse(parts[3].PadRight(3, '0'));
            TimeSpan = new TimeSpan(hours, minutes, seconds, milliseconds);
        }
        else
        {
            throw new FormatException("Invalid timestamp format. Expected HH:mm:ss.SSS or mm:ss.SSS");
        }
    }
    public TimeStamp(int hours, int minutes, int seconds, int milliseconds)
    {
        TimeSpan = new TimeSpan(hours, minutes, seconds, milliseconds);
    }
    public TimeStamp(TimeSpan time)
    {
        TimeSpan = time;
    }
    public TimeStamp(long frameIndex, Fps fps)
    {
        double timeInSeconds = frameIndex * fps.Base / fps.Divider;
        TimeSpan = TimeSpan.FromSeconds(timeInSeconds);
    }
    public TimeStamp(double timeInSeconds)
    {
        TimeSpan = TimeSpan.FromSeconds(timeInSeconds);
    }

    TimeSpan TimeSpan { get; }
    public int Hours => TimeSpan.Hours;
    public int Minutes => TimeSpan.Minutes;
    public int Seconds => TimeSpan.Seconds;
    public int Milliseconds => TimeSpan.Milliseconds;
    public double TotalSeconds => TimeSpan.TotalSeconds;
    public double ConvertToTime()
    {
        return TimeSpan.TotalSeconds;
    }

    public static bool operator ==(TimeStamp p1, TimeStamp p2)
    {
        return p1.Equals(p2);
    }
    public static bool operator !=(TimeStamp p1, TimeStamp p2)
    {
        return !p1.Equals(p2);
    }
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (!(obj is TimeStamp)) return false;

        var other = obj as TimeStamp?;
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
