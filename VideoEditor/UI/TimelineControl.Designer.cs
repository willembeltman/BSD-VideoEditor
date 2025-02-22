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
            scrollBarControl = new HScrollBar();
            SuspendLayout();
            // 
            // scrollBarControl
            // 
            scrollBarControl.Location = new Point(0, 398);
            scrollBarControl.Name = "scrollBarControl";
            scrollBarControl.Size = new Size(665, 32);
            scrollBarControl.TabIndex = 2;
            scrollBarControl.Scroll += ScrollBarControl_Scroll;
            // 
            // TimelineControl
            // 
            AllowDrop = true;
            BackColor = Color.Black;
            Controls.Add(scrollBarControl);
            DoubleBuffered = true;
            Name = "TimelineControl";
            Size = new Size(665, 430);
            Load += TimelineControl_Load;
            DragDrop += TimelineControl_DragDrop;
            DragEnter += TimelineControl_DragEnter;
            DragOver += TimelineControl_DragOver;
            DragLeave += TimelineControl_DragLeave;
            Paint += TimelineControl_Paint;
            MouseDown += TimelineControl_MouseDown;
            MouseMove += TimelineControl_MouseMove;
            MouseUp += TimelineControl_MouseUp;
            MouseWheel += TimelineControl_MouseWheel;
            Resize += TimelineControl_Resize;
            ResumeLayout(false);
        }

        private HScrollBar scrollBarControl;
    }
}
