namespace VideoEditor.Static
{
    public static class ColorExtentions
    {
        public static Color Blend(
            this Color originalColor,
            Color newColor)
        {
            // Alpha waarden normaliseren naar [0,1]
            float alpha1 = originalColor.A / 255f;
            float alpha2 = newColor.A / 255f;

            // Nieuwe alpha berekenen
            float alphaResult = alpha2 + alpha1 * (1 - alpha2);

            if (alphaResult == 0)
                return Color.Transparent; // Volledig transparant

            // RGB-componenten berekenen
            byte BlendChannel(byte c1, byte c2)
            {
                return (byte)(((c2 * alpha2 + c1 * alpha1 * (1 - alpha2)) / alphaResult) + 0.5f);
            }

            byte r = BlendChannel(originalColor.R, newColor.R);
            byte g = BlendChannel(originalColor.G, newColor.G);
            byte b = BlendChannel(originalColor.B, newColor.B);
            byte a = (byte)(alphaResult * 255 + 0.5f);

            return Color.FromArgb(a, r, g, b);
        }
    }
}
