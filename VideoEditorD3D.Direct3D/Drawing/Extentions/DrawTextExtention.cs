using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.TexturesWithVerticies;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VideoEditorD3D.Direct3D.Drawing;

public static class DrawTextExtention
{
    public static Size DrawText(this GraphicsLayer graphicsLayer, string text, int left, int top, int width = -1, int height = -1, string font = "Ebrima", float fontSize = 10f, FontStyle fontStyle = FontStyle.Regular, int letterSpacing = -2, RawColor4? foreColor = null, RawColor4? backColor = null)
    {
        foreColor ??= new RawColor4(1, 1, 1, 1);
        backColor ??= new RawColor4(0, 0, 0, 0);

        var absoluteLeft = left + graphicsLayer.AbsoluteLeft;
        var absoluteTop = top + graphicsLayer.AbsoluteTop;
        var currentLeft = absoluteLeft;
        var currentTop = absoluteTop;
        var currentBottom = 0;
        var maxRight = 0;

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

                if (width != -1 && height != -1 && absoluteLeft + width < right)
                {
                    // Nieuwe regel
                    currentLeft = absoluteLeft;
                    currentTop = currentBottom;
                    right = currentLeft + texture.Width + letterSpacing;
                }

                var bottom = currentTop + texture.Height;
                if (currentBottom < bottom)
                    currentBottom = bottom;
                if (maxRight < right)
                    maxRight = right;

                // Vertices laten maken
                var fillVertices = graphicsLayer.CreateTextureVerticesForRectangle(currentLeft, currentTop, texture.Width, texture.Height);
                var fillVerticesBuffer = Buffer.Create(graphicsLayer.Device, BindFlags.VertexBuffer, fillVertices);

                // En dit in een model stoppen
                var image = new CachedTextureWithVerticies(fillVertices, fillVerticesBuffer, texture);
                graphicsLayer.TextureImages.Add(image);

                currentLeft = right;
            }
            // Nieuwe regel
            currentLeft = absoluteLeft;
            currentTop = currentBottom;
        }

        return new Size(maxRight - absoluteLeft, currentBottom - absoluteTop);
    }
}
