using System.Runtime.InteropServices;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Helpers;

public class FrameDataPointer : IDisposable
{
    private GCHandle _handle;

    public FrameDataPointer(IFrame frame)
    {
        _handle = GCHandle.Alloc(frame.Data, GCHandleType.Pinned);
    }

    public nint DataPointer => _handle.AddrOfPinnedObject();

    public void Dispose()
    {
        if (_handle.IsAllocated)
            _handle.Free();
    }
}