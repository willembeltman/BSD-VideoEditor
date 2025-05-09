using SharpDX.Mathematics.Interop;
using System.Runtime.InteropServices;

namespace VideoEditorD3D.Direct3D.Vertices;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex
{
    public RawVector2 Position;
    public RawColor4 Color;
}


