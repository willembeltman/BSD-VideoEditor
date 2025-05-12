using Control = VideoEditorD3D.Direct3D.Forms.Control;

namespace VideoEditorD3D.Direct3D.Collections
{
    public class GraphicsLayerCollection : ObservableArrayCollection<GraphicsLayer>, IDisposable
    {
        private readonly Control Control;

        public GraphicsLayerCollection(Control control)
        {
            Control = control;
            Changed += OnChanged;
        }

        private void OnChanged(object? sender, GraphicsLayer? e)
        {
            Control.Invalidate();
        }

        public GraphicsLayer CreateNewLayer()
        {
            var layer = new GraphicsLayer(Control.ApplicationForm, Control);
            Add(layer);
            return layer;
        }

        public void Dispose()
        {
            foreach (var layer in this)
            {
                layer.Dispose();
            }
        }
    }
}
