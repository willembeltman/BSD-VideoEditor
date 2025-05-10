using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Forms.Generic;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms;

public class Form(IApplicationForm applicationForm) : BackControl(applicationForm, null, null)
{
}
