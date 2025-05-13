using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoEditor.UI
{
    public partial class TimelineControl
    {
        private void InitializeComponent()
        {
            HScrollBarControl = new HScrollBar();
            SuspendLayout();
            // 
            // scrollBarControl
            // 
            HScrollBarControl.Location = new Point(0, 406);
            HScrollBarControl.Name = "scrollBarControl";
            HScrollBarControl.Size = new Size(665, 24);
            HScrollBarControl.TabIndex = 2;
            HScrollBarControl.Scroll += ScrollBarControl_Scroll;
            // 
            // TimelineControl
            // 
            AllowDrop = true;
            BackColor = Color.Black;
            Controls.Add(HScrollBarControl);
            DoubleBuffered = true;
            Name = "TimelineControl";
            Size = new Size(665, 430);
            Load += TimelineControl_Load;
            DragDrop += TimelineControl_DragDrop;
            DragEnter += TimelineControl_DragEnter;
            DragOver += TimelineControl_DragOver;
            DragLeave += TimelineControl_DragLeave;
            Paint += TimelineControl_Paint;
            KeyDown += TimelineControl_KeyDown;
            MouseDown += TimelineControl_MouseDown;
            MouseMove += TimelineControl_MouseMove;
            MouseUp += TimelineControl_MouseUp;
            MouseWheel += TimelineControl_MouseWheel;
            Resize += TimelineControl_Resize;
            ResumeLayout(false);
        }

        private HScrollBar HScrollBarControl;
    }
}
