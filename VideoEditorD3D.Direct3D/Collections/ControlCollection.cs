using System.Collections;
using Control = VideoEditorD3D.Direct3D.Forms.Control;

namespace VideoEditorD3D.Direct3D.Collections
{
    public class ControlCollection(Control control) : IEnumerable<Control>
    {
        private Control Control = control;
        private Control[] ControlList = [];
        public void Add(Control control)
        {
            var newArray = new Control[ControlList.Length + 1];
            Array.Copy(ControlList, newArray, ControlList.Length);
            newArray[^1] = control;
            ControlList = newArray;

            Control.Invalidate();
        }

        public void Remove(Control control)
        {
            var deleted = 0;
            for (int i = 0; i < ControlList.Length; i++)
            {
                if (ControlList[i] == control)
                {
                    deleted++;
                }
            }
            var newArray = new Control[ControlList.Length - deleted];
            var newIndex = 0;
            for (int i = 0; i < ControlList.Length; i++)
            {
                if (ControlList[i] != control)
                {
                    newArray[newIndex] = ControlList[i];
                    newIndex++;
                }
            }
            ControlList = newArray;

            Control.Invalidate();
        }

        public IEnumerator<Control> GetEnumerator()
        {
            foreach (var control in ControlList)
            {
                yield return control;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
