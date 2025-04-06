namespace VideoEditor.Helpers;

public class WindowsScaling
{
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern int GetDpiForWindow(nint hwnd);

    public WindowsScaling(nint handle)
    {
        Handle = handle;
    }

    public nint Handle { get; }

    public double GetScaling() => GetDpiForWindow(Handle) / 96.0d;

    private double? _Scaling { get; set; }
    public double Scaling
    {
        get
        {
            _Scaling = _Scaling ?? GetScaling();
            return _Scaling.Value;
        }
    }

    public static implicit operator double(WindowsScaling windowsScaling)
    {
        return windowsScaling.Scaling;
    }
}