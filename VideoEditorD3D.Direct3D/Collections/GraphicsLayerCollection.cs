using System.Collections;
using Control = VideoEditorD3D.Direct3D.Forms.Control;

namespace VideoEditorD3D.Direct3D.Collections
{
    public class GraphicsLayerCollection : ObservableArrayCollection<GraphicsLayer>
    {
        private Control Control;

        public GraphicsLayerCollection(Control control)
        {
            Control = control;
            Changed += (sender, args) =>
            {
                Control.Invalidate();
            };
        }
        public GraphicsLayer Create()
        {
            var layer = new GraphicsLayer(Control.ApplicationForm, Control);
            Add(layer);
            return layer;
        }
    }
}
