using VideoEditorD3D.Direct3D.Textures;
using VideoEditorD3D.Interfaces;
using Device = SharpDX.Direct3D11.Device;

namespace VideoEditorD3D.Direct3D
{
    public class CharacterCollection : IDisposable
    {
        public CharacterCollection(IApplication application)
        {
            Application = application;
            TextItems = Array.Empty<CharacterTexture>();
        }

        IApplication Application;
        CharacterTexture[] TextItems;
        Device Device => Application.Device;

        public CharacterTexture GetOrCreate(char character, string font, float fontSize, Color backColor, Color foreColor)
        {
            var item = TextItems
                .FirstOrDefault(a =>
                    a.Char == character &&
                    a.FontName == font &&
                    a.FontSize == fontSize);
            if (item == null)
            {
                item = new CharacterTexture(character, font, fontSize, backColor, foreColor, Device);

                Array.Resize(ref TextItems, TextItems.Length + 1);
                TextItems[TextItems.Length - 1] = item;
                return item;
            }
            return item;
        }

        public void Dispose()
        {
            foreach (var item in TextItems)
            {
                item.TextureBitmap.Dispose();
            }
        }
    }
}
