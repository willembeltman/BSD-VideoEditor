using SharpDX;

namespace VideoEditorD3D.Direct3D.Forms
{
    public class Control
    {
        Rectangle ClientRectangle { get; set; }
        public Control[] Controls { get; set; } = [];
        public void AddControl(Control control)
        {
            var newArray = new Control[Controls.Length + 1];
            Array.Copy(Controls, newArray, Controls.Length);
            newArray[newArray.Length - 1] = control;
            Controls = newArray;
        }
        public void RemoveControl(Control control)
        {
            var deleted = 0;
            for (int i = 0; i < Controls.Length; i++)
            {
                if (Controls[i] == control)
                {
                    deleted++;
                }
            }
            var newArray = new Control[Controls.Length - deleted];
            var newIndex = 0;
            for (int i = 0; i < Controls.Length; i++)
            {
                if (Controls[i] != control)
                {
                    newArray[newIndex] = Controls[i];
                    newIndex++;
                }
            }
            Controls = newArray;
        }

        public virtual void Draw(Canvas canvas)
        {
            foreach (Control control in Controls)
            {
                control.Draw(canvas);
            }
        }
    }
}
