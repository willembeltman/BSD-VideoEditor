using SharpDX.Mathematics.Interop;
using System.Runtime.InteropServices;

namespace VideoEditorD3D.Direct3D.Vertices;

[StructLayout(LayoutKind.Sequential)]
public struct TextureVertex
{
    public RawVector2 Position;
    public RawVector2 UV;
}
