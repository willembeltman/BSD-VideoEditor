using System.Runtime.InteropServices;
using SharpDX.Mathematics.Interop;

namespace VideoEditorD3D.Direct3D.Types;

[StructLayout(LayoutKind.Sequential)]
public struct TextureVertex
{
    public RawVector2 Position;
    public RawVector2 UV;
}
