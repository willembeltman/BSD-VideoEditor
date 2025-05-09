using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Textures;

namespace VideoEditorD3D.Direct3D;

public class Characters(IApplication Application) : IDisposable
{
    private CharacterTexture[] TextItems = [];

    public CharacterTexture GetOrCreate(char character, string font, float fontSize, RawColor4 backColor, RawColor4 foreColor)
    {
        var item = TextItems
            .FirstOrDefault(a =>
                a.Char == character &&
                a.FontName == font &&
                a.FontSize == fontSize);
        if (item == null)
        {
            item = new CharacterTexture(character, font, fontSize, backColor, foreColor, Application.Device);

            Array.Resize(ref TextItems, TextItems.Length + 1);
            TextItems[^1] = item;
            return item;
        }
        return item;
    }

    public void Dispose()
    {
        foreach (var item in TextItems)
        {
            item.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}
