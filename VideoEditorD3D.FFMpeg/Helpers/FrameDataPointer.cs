using System.Runtime.InteropServices;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.FFMpeg.Helpers;

public class FrameDataPointer : IDisposable
{
    private GCHandle _handle;

    public FrameDataPointer(Frame frame)
    {
        _handle = GCHandle.Alloc(frame.Buffer, GCHandleType.Pinned);
    }

    public nint DataPointer => _handle.AddrOfPinnedObject();

    public void Dispose()
    {
        if (_handle.IsAllocated)
            _handle.Free();
    }
}