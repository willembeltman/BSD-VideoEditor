using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Textures;

namespace VideoEditorD3D.Direct3D.Collections;

public class CharacterCollection(IApplicationForm application) : ObservableArrayCollection<CharacterTexture>, IDisposable
{
    private readonly IApplicationForm Application = application;

    public CharacterTexture GetOrCreate(char character, string font, float fontSize, FontStyle fontStyle, RawColor4 backColor, RawColor4 foreColor)
    {
        var item = this
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

    public void Dispose()
    {
        foreach (var item in this)
        {
            item.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}
