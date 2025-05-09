namespace VideoEditorD3D.Direct3D;

public class WindowsScaling
{
    public WindowsScaling(nint handle)
    {
        Handle = handle;
    }

    public nint Handle { get; }

    private float? _Scaling { get; set; }
    public float Scaling
    {
        get
        {
            _Scaling = _Scaling ?? GetDpiForWindow(Handle) / 96.0f;
            return _Scaling.Value;
        }
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern int GetDpiForWindow(nint hwnd);
}
