using System.Collections;
using Control = VideoEditorD3D.Direct3D.Forms.Control;

namespace VideoEditorD3D.Direct3D.Collections
{
    public class GraphicsLayerCollection(Control control) : IEnumerable<GraphicsLayer>
    {
        private Control Control = control;
        private GraphicsLayer[] CanvasLayerList = [];

        public void Add(GraphicsLayer layer)
        {
            var newArray = new GraphicsLayer[CanvasLayerList.Length + 1];
            Array.Copy(CanvasLayerList, newArray, CanvasLayerList.Length);
            newArray[^1] = layer;
            CanvasLayerList = newArray;
        }
        public void Remove(GraphicsLayer layer)
        {
            var deleted = 0;
            for (int i = 0; i < CanvasLayerList.Length; i++)
            {
                if (CanvasLayerList[i] == layer)
                {
                    deleted++;
                }
            }
            var newArray = new GraphicsLayer[CanvasLayerList.Length - deleted];
            var newIndex = 0;
            for (int i = 0; i < CanvasLayerList.Length; i++)
            {
                if (CanvasLayerList[i] != layer)
                {
                    newArray[newIndex] = CanvasLayerList[i];
                    newIndex++;
                }
            }
            CanvasLayerList = newArray;
        }
        public GraphicsLayer Create()
        {
            var layer = new GraphicsLayer(Control.ApplicationForm, Control);
            Add(layer);
            return layer;
        }

        public IEnumerator<GraphicsLayer> GetEnumerator()
        {
            foreach (var layer in CanvasLayerList)
            {
                yield return layer;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
