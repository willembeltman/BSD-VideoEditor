using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Textures;

namespace VideoEditorD3D.Direct3D;

public class CharacterCollection(IApplicationForm Application) : IDisposable
{
    private CharacterTexture[] TextItems = [];

    public CharacterTexture GetOrCreate(char character, string font, float fontSize, FontStyle fontStyle, RawColor4 backColor, RawColor4 foreColor)
    {
        var item = TextItems
            .FirstOrDefault(a =>
                a.Char == character &&
                a.FontName == font &&
                a.FontSize == fontSize && 
                a.FontStyle == fontStyle && 
                a.BackColor.Equals(backColor) &&
                a.ForeColor.Equals(foreColor));
        if (item == null)
        {
            item = new CharacterTexture(
                character,
                font,
                fontSize, 
                fontStyle,
                backColor,
                foreColor,
                Application.Device);
            Add(item);
        }
        return item;
    }

    // Resizes the array to accommodate the new item then adds it to the end of the array.
    private void Add(CharacterTexture item)
    {
        Array.Resize(ref TextItems, TextItems.Length + 1);
        TextItems[^1] = item;
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
