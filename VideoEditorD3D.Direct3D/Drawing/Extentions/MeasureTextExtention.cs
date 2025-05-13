using SharpDX.Mathematics.Interop;

namespace VideoEditorD3D.Direct3D.Drawing;

public static class MeasureTextExtention
{
    public static Size MeasureText(this GraphicsLayer graphicsLayer, string text, int width = -1, int height = -1, string font = "Ebrima", float fontSize = 10f, FontStyle fontStyle = FontStyle.Regular, int letterSpacing = -2, RawColor4? foreColor = null, RawColor4? backColor = null)
    {
        foreColor ??= new RawColor4(1, 1, 1, 1);
        backColor ??= new RawColor4(0, 0, 0, 0);

        var currentLeft = 0;
        var maxRight = 0;
        var currentTop = 0;
        var currentBottom = 0;

        var currentText = text.Replace("\r", "");
        var rows = currentText.Split('\n');

        foreach (var row in rows)
        {
            foreach (var character in row)
            {
                // Text item aanmaken of ophalen voor het huidige character
                var texture = graphicsLayer.Characters.GetOrCreate(character, font, fontSize, fontStyle, backColor.Value, foreColor.Value);

                // Berekenen
                var right = currentLeft + texture.Width + letterSpacing;

                if (width != -1 && height != -1 && width < right)
                {
                    // Nieuwe regel
                    currentLeft = 0;
                    currentTop = currentBottom;
                    right = currentLeft + texture.Width + letterSpacing;
                }

                var bottom = currentTop + texture.Height;
                if (currentBottom < bottom)
                    currentBottom = bottom;
                if (maxRight < right)
                    maxRight = right;

                currentLeft = right;
            }
            // Nieuwe regel
            currentLeft = 0;
            currentTop = currentBottom;
        }
        return new Size(maxRight, currentBottom);
    }
}
