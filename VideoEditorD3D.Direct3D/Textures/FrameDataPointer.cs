using System.Runtime.InteropServices;

namespace VideoEditorD3D.Direct3D.Textures;

public class FrameDataPointer : IDisposable
{
    private GCHandle _handle;

    public FrameDataPointer(byte[] frameBuffer)
    {
        _handle = GCHandle.Alloc(frameBuffer, GCHandleType.Pinned);
    }

    public nint DataPointer => _handle.AddrOfPinnedObject();

    public void Dispose()
    {
        if (_handle.IsAllocated)
            _handle.Free();
    }
}